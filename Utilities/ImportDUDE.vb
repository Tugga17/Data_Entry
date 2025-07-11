Imports System.Data.OleDb
Imports Entergy.PerformanceManagement.PMDB


Public Class ImportDUDE
    Private _overwrite As Boolean = False
    Private _failedMonthLocked As Boolean
    Public Function ImportData(ByVal file As String) As String
        Dim sb As New System.Text.StringBuilder
        Dim i As Integer
        Dim list As List(Of UPMPointsDTO)
        Dim exList As List(Of UPMPointsDTO)
        Dim errors As New List(Of UPMPointsDTO)
        Dim beforeErrors As New List(Of UPMPointsDTO)
        Dim db As New SecurityUsersDB
        Dim totCount As Integer

        _failedMonthLocked = False

        exList = GetExcelData(file)
        totCount = exList.Count

        If Not _overwrite Then
            exList = Me.GetPointsNotLockedByYear(exList, beforeErrors)
            If beforeErrors.Count > 0 Then
                sb.AppendLine()
                sb.AppendLine(beforeErrors.Count & " of " & totCount & " records can not be imported because the year is locked by the system.")
                sb.AppendLine("The following can not be imported due to the year's data begin locked by the system:")
                sb.AppendLine(BuildErrorLog(beforeErrors))
                If db.IsUserInMasterGroup(Environment.UserName) Then
                    sb.AppendLine()
                    sb.AppendLine("Please verify the data in the dude file is correct and check the Check box above and resubmit dude file.")
                    _failedMonthLocked = True
                End If
                errors.Clear()
            End If
        End If
        'If Not _overwrite Then
        '    exList = Me.GetPointsNotLockedByMonth(exList, beforeErrors)
        '    If beforeErrors.Count > 0 Then
        '        sb.AppendLine()
        '        sb.AppendLine(beforeErrors.Count & " of " & totCount & " records can not be imported due the the month's data is locked by the system.")
        '        sb.AppendLine("The following can not be imported due the the month's data is locked by the system:")
        '        sb.AppendLine(BuildErrorLog(beforeErrors))
        '        If db.IsUserInMasterGroup(Environment.UserName) Then
        '            sb.AppendLine()
        '            sb.AppendLine("Please verify the data in the dude file is correct and check the Check box above and resubmit dude file.")
        '            _failedMonthLocked = True
        '        End If
        '        errors.Clear()
        '    End If
        'End If

        exList = Me.GetPointsNotLockedBySeries(exList, beforeErrors)
        If beforeErrors.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine(beforeErrors.Count & " of " & totCount & " records can not be imported due the the data series' data is locked by the system.")
            sb.AppendLine("The following can not be imported due the the data series' data is locked by the system:")
            sb.AppendLine(BuildErrorLog(beforeErrors))
            errors.Clear()
        End If


        list = Me.VerifyPointsInDB(exList, errors)
        If errors.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine(errors.Count & " of " & totCount & " records can not be imported since they are not valid points in the database.")
            sb.AppendLine("The following are not valid points in the Perfman Database.")
            sb.AppendLine(BuildErrorLog(errors))
            errors.Clear()
        End If

        If Not db.IsUserInMasterGroup(Environment.UserName) Then
            list = Me.GetChartsUserPermission(list, errors)
            If errors.Count > 0 Then
                sb.AppendLine()
                sb.AppendLine(errors.Count & " of " & totCount & " records can not be imported since you do not have permission to update the metrics.")
                sb.AppendLine("You do not have permission to update the following metrics.")
                sb.AppendLine(BuildErrorLog(errors))
                errors.Clear()
            End If
        End If

        list = Me.GetPointsWithGoodValues(list, errors)
        If errors.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine(errors.Count & " of " & totCount & " records can not be imported since they have point values which are non numeric.")
            sb.AppendLine("The following metrics have point values which are non numeric.")
            sb.AppendLine(BuildErrorLog(errors))
            errors.Clear()
        End If

        i = Me.SaveExcelData(list, errors)
        If errors.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine(errors.Count & " of " & totCount & " records can not be imported since they are not valid metrics.")
            sb.AppendLine("The following metrics are not valid metrics according to the Perfman Database.")
            sb.AppendLine(BuildErrorLog(errors))
            errors.Clear()
        End If
        sb.Insert(0, i.ToString & " of " & totCount & " Metrics saved sucessfully to the Perfman Database" & vbCrLf)
        Return sb.ToString
    End Function

    Private Function GetExcelData(ByVal fileWithPath As String) As List(Of UPMPointsDTO)
        Dim list As New List(Of UPMPointsDTO)
        Dim dto As UPMPointsDTO
        Dim reader As OleDbDataReader
        Dim connString As String
        Dim conn As OleDbConnection

        Try
            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & fileWithPath & ";Extended Properties=Excel 12.0;"
            conn = New OleDbConnection(connString)
            conn.Open()
        Catch ex2 As Exception
            Throw New Exception("Could not open Excel file. Contact Administrator. " + vbCrLf + ex2.Message)
        End Try


        ' create your excel connection object using the connection string

        Try
            Dim cmd As New OleDbCommand("SELECT * FROM [Perfman_Data$]", conn)

            reader = cmd.ExecuteReader

            While reader.Read
                If DBNulltoText(reader.GetValue(3)).Trim > "" Then
                    dto = New UPMPointsDTO
                    dto.MetricID = DBNulltoText(reader.GetValue(0)).Trim
                    dto.DataSeriesDescription = DBNulltoText(reader.GetValue(1)).Trim
                    dto.PointsDescription = DBNulltoText(reader.GetValue(2)).Trim
                    dto.YValue = DBNulltoText(reader.GetValue(3)).Trim
                    dto.ReportingYear = DBNulltoZero(reader.GetValue(4)).Trim
                    dto.UserID = Environment.UserName
                    list.Add(dto)
                End If
            End While

            Return list
        Catch ex As System.Data.OleDb.OleDbException
            Throw New Exception("The spreadsheet must have a worksheet called Perfman_Data with 5 columns in the following order: " & vbCrLf & "ChartID" & vbCrLf & "Data Series" & vbCrLf & "Point Description" & vbCrLf & "Point Value" & vbCrLf & "Reporting Year" & vbCrLf & ex.Message, ex)
        Finally
            If conn.State = Data.ConnectionState.Open Then conn.Close()
        End Try

    End Function

    Private Function SaveExcelData(ByVal list As List(Of UPMPointsDTO), ByRef errorList As List(Of UPMPointsDTO)) As Integer
        Dim i As Integer
        Dim j As Integer
        Dim db As New PMPointsDB
        Dim badList As New List(Of UPMPointsDTO)
        For Each dto As UPMPointsDTO In list
            Try
                db.SavePointsArchive(dto)
                j = db.SaveUPMPoints(dto)
                If j = 1 Then
                    i = i + 1
                Else
                    badList.Add(dto)
                End If
            Catch ex As Exception
                badList.Add(dto)
            End Try
        Next
        errorList = badList
        Return i
    End Function

    Private Function VerifyPointsInDB(ByVal list As List(Of UPMPointsDTO), ByRef badlist As List(Of UPMPointsDTO)) As List(Of UPMPointsDTO)
        Dim goodList As New List(Of UPMPointsDTO)
        Dim dto As UPMPointsDTO
        'Using dto.ChangedBy as the dataseries name for error logging purposes
        Dim db As New PMPointsDB
        For Each item As UPMPointsDTO In list
            dto = db.GetUPMPointFromExcel(item.MetricID, item.ReportingYear, item.PointsDescription, item.DataSeriesDescription)
            If IsNothing(dto) Then
                dto = New UPMPointsDTO
                dto.MetricID = item.MetricID
                dto.ReportingYear = item.ReportingYear
                dto.PointsDescription = item.PointsDescription
                dto.YValue = item.YValue
                dto.ChangedBy = item.DataSeriesDescription
                badlist.Add(dto)
            Else
                dto.YValue = item.YValue
                dto.ChangedBy = item.DataSeriesDescription
                goodList.Add(dto)
            End If
        Next
        Return goodList
    End Function

    Private Function GetPointsWithGoodValues(ByVal list As List(Of UPMPointsDTO), ByRef errorList As List(Of UPMPointsDTO)) As List(Of UPMPointsDTO)
        Dim badList As New List(Of UPMPointsDTO)
        Dim goodList As New List(Of UPMPointsDTO)
        For Each dto As UPMPointsDTO In list
            If IsNumeric(dto.YValue) Then
                goodList.Add(dto)
            Else
                badList.Add(dto)
            End If
        Next
        errorList = badList
        Return goodList
    End Function
    Private Function GetPointsNotLockedByYear(ByVal list As List(Of UPMPointsDTO), ByRef errorlist As List(Of UPMPointsDTO)) As List(Of UPMPointsDTO)
        Dim badList As New List(Of UPMPointsDTO)
        Dim goodList As New List(Of UPMPointsDTO)
        Dim yearsLocked As List(Of String) = Me.CreateYearLocked
        For Each dto As UPMPointsDTO In list
            If Not yearsLocked.Contains(dto.ReportingYear) Then
                goodList.Add(dto)
            Else
                badList.Add(dto)
            End If
        Next
        errorlist = badList
        Return goodList
    End Function
    Private Function GetPointsNotLockedByMonth(ByVal list As List(Of UPMPointsDTO), ByRef errorList As List(Of UPMPointsDTO)) As List(Of UPMPointsDTO)
        Dim badList As New List(Of UPMPointsDTO)
        Dim goodList As New List(Of UPMPointsDTO)
        Dim monthsLocked As List(Of String) = Me.CreateMonthsLocked
        For Each dto As UPMPointsDTO In list
            If Not monthsLocked.Contains(dto.ReportingYear & dto.PointsDescription.ToUpper) Then
                goodList.Add(dto)
            Else
                badList.Add(dto)
            End If
        Next
        errorList = badList
        Return goodList
    End Function
    Private Function GetPointsNotLockedBySeries(ByVal list As List(Of UPMPointsDTO), ByRef errorList As List(Of UPMPointsDTO)) As List(Of UPMPointsDTO)

        Dim db As New SecurityLocksDB
        Dim badList As New List(Of UPMPointsDTO)
        Dim goodList As New List(Of UPMPointsDTO)
        Dim seriesLocked As List(Of String) = db.GetSecurityLockSeries
        For Each dto As UPMPointsDTO In list
            If Not seriesLocked.Contains(dto.DataSeriesDescription) Then
                goodList.Add(dto)
            Else
                badList.Add(dto)
            End If
        Next
        errorList = badList
        Return goodList
    End Function

    Private Function GetChartsUserPermission(ByVal list As List(Of UPMPointsDTO), ByRef errorList As List(Of UPMPointsDTO)) As List(Of UPMPointsDTO)
        Dim db As New SecurityVerificationDB
        Dim cDB As New PMMetricsDB

        Dim permList As List(Of SecurityExcelPermissionsDTO) = db.GetSecurityExcel(Environment.UserName)
        'Dim myList As List(Of MetricDTO) = cDB.GetMyMetrics(Environment.UserName)
        Dim badList As New List(Of UPMPointsDTO)
        Dim goodList As New List(Of UPMPointsDTO)
        Dim perm As Boolean = False
        For Each dto As UPMPointsDTO In list
            perm = False
            For Each item As SecurityExcelPermissionsDTO In permList
                If dto.MetricID = item.MetricID Then
                    If Not item.CentrallyManaged Then
                        If Not item.AllowEditTargets And (dto.ChangedBy.ToUpper = "TARGET" Or dto.ChangedBy.ToUpper = "MAXIMUM" Or dto.ChangedBy.ToUpper = "MINIMUM") Then
                            perm = False
                        Else
                            perm = True
                        End If
                    Else
                        perm = False
                    End If
                End If
            Next
            'For Each item As MetricDTO In myList
            '    If dto.MetricID = item.ChartID Then
            '        perm = True
            '    End If
            'Next
            If perm Then
                goodList.Add(dto)
            Else
                badList.Add(dto)
            End If
        Next
        errorList = badList
        Return goodList
    End Function
    Private Function CreateYearLocked() As List(Of String)
        Dim db As New PMLockYearDB
        Dim yearsLocked As List(Of LockYearDTO) = db.GetLockedYear()
        Dim list As New List(Of String)
        For Each dto As LockYearDTO In yearsLocked
            'If dto.Locked Then
            list.Add(dto.Year)
            'End If
        Next
        Return list
    End Function
    Private Function CreateMonthsLocked() As List(Of String)
        Dim db As New SecurityLocksDB
        Dim monthsLocked As List(Of SecurityLockMonthDTO) = db.GetSecurityLockMonth()
        Dim list As New List(Of String)
        For Each dto As SecurityLockMonthDTO In monthsLocked
            If dto.Locked Then
                If Not list.Contains(dto.ReportingYear & dto.ReportingPeriod) Then
                    list.Add(dto.ReportingYear & dto.ReportingPeriod.ToUpper)
                End If
            End If
        Next
        Return list
    End Function

    Private Function BuildErrorLog(ByVal errors As List(Of UPMPointsDTO)) As String
        Dim sb As New System.Text.StringBuilder
        'Using dto.ChangedBy as the dataseries name for error logging purposes
        sb.AppendLine("Year" & vbTab & "Chart ID" & vbTab & "Data Series" & vbTab & "Point" & vbTab & "Value")
        For Each dto As UPMPointsDTO In errors
            sb.AppendLine(dto.ReportingYear.ToString & vbTab & dto.MetricID & vbTab & dto.ChangedBy & vbTab & vbTab & dto.PointsDescription & vbTab & dto.YValue.ToString)
        Next
        Return sb.ToString
    End Function


    Private Function DBNulltoText(ByVal value As Object) As String
        If IsDBNull(value) Then
            Return String.Empty
        Else
            Return value.ToString
        End If
    End Function

    Private Function DBNulltoZero(ByVal value As Object) As String
        If IsDBNull(value) Then
            Return Decimal.Zero
        Else
            Return value
        End If
    End Function

End Class

