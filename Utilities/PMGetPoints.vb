Imports Entergy.PerformanceManagement.PMDB

Public Class PMGetPoints
#Region "Points Fetching"
    Public Function GetPointValue(ByVal cID As Integer, ByVal dataSeries As String, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Dim list As List(Of UPMPointsDTO) = Me.GetPoints(cID, dataSeries, month, year)
        If list.Count = 0 Then
            list = Me.GetPoints(cID, dataSeries, Periods.GetQuarter(month), year)
            If list.Count = 0 Then
                Return list(0).YValue
            Else
                Return Nothing
            End If
        Else
            Return list(0).YValue
        End If
    End Function
    Public Function GetYTDValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "YTD", month, year)
    End Function
    Public Function GetTargetValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Target", month, year)
    End Function
    Public Function GetMinimumValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Minimum", month, year)
    End Function
    Public Function GetMaximumValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Maximum", month, year)
    End Function
    Public Function GetMonthlyValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Monthly", month, year)
    End Function
    Public Function GetMonthlyTargetValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Monthly Target", month, year)
    End Function
    Public Function GetMonthlyMinimumValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Monthly Minimum", month, year)
    End Function
    Public Function GetMonthlyMaximumValue(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Return GetPointValue(cID, "Monthly Maximum", month, year)
    End Function
    Public Function GetPoints(ByVal cID As Integer, ByVal dataSeries As String, ByVal month As String, ByVal year As Integer) As List(Of UPMPointsDTO)
        Return GetPointsbyDSID(GetDSID(cID, dataSeries), month, year)
    End Function
    Public Function GetPoints(ByVal cID As Integer, ByVal dataSeries As String, ByVal year As Integer) As List(Of UPMPointsDTO)
        Return GetPointsbyDSID(GetDSID(cID, dataSeries), year)
    End Function
    Private Function GetPointsbyDSID(ByVal dsID As Integer, ByVal month As String, ByVal year As Integer) As List(Of UPMPointsDTO)
        Dim db As New PMPointsDB
        Return db.GetUPMPoints(dsID, year, month)
    End Function
    Private Function GetPointsbyDSID(ByVal dsID As Integer, ByVal year As Integer) As List(Of UPMPointsDTO)
        Dim db As New PMPointsDB
        Return db.GetUPMPoints(dsID, year)
    End Function
    Private Function GetDSID(ByVal cID As Integer, ByVal dataSeries As String) As Integer
        Dim dsID As Integer = -1
        Dim db As New PMDataSeriesDB
        For Each item As DataSeriesDTO In db.GetUPMDataSeries(cID)
            If item.Description.ToUpper = dataSeries.ToUpper Then
                dsID = item.DSID
            End If
        Next
        Return dsID
    End Function
#End Region
End Class
