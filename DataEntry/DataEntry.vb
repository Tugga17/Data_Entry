Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports Entergy.PerformanceManagement.PMDB
Imports Entergy.PerformanceManagement.PMDTO
Imports System.Configuration.ConfigurationManager
Imports Entergy.PerformanceManagement.PMUtilities
Imports PerformanceManagement.DEPoints

Public Class DataEntry
    Inherits RadForm
    Private _DEPoint As DEPoints
    Private _presenter As New myPresenter
    Private _metric As MetricDTO 'Set do not alter
    Private _AllComments As Dictionary(Of Integer, CommentsDTO)

    Private Sub DataEntry_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        rtsDataEntry.SelectedTab = rtsDataEntry.Items(0)
        Me.rlblCRTab1Ind.Visible = False
        Me.rlblCRTab2Ind.Visible = False
        'Me.rlblCNRTab1.Visible = False
        'Me.rlblCNRTab2.Visible = False
    End Sub
    Private Sub rtsDataEntry_TabSelected(ByVal sender As Object, ByVal args As Telerik.WinControls.UI.TabEventArgs) Handles rtsDataEntry.TabSelected
        Select Case args.TabItem.Name
            Case "tiAllMonths"
                SetUpDEAllMonthsTab()
            Case "tiAllDS"
                SetUpDEAllDataSeriesTab()
            Case "tiAllDS_Months"
                SetUpDEAllDataSeriesMonthTab()
        End Select
    End Sub
    Private Sub SetUpDEAllMonthsTab()
        SetUpAllMonthsGrid()
        rtxtDEAllMonthsComments.Enabled = False
        Me.PMLoadReportingYear()
        _AllComments = GetComments(_metric.CID, rcbDEAllMMonthsYear.Text)
        _DEPoint = New DEPoints
        _DEPoint.GetMetric(Me.GetSelectedMetric, rcbDEAllMMonthsYear.Text)
        LoadDataSeriesforDLB()
        'Me.rlblMetricIDNameTab1.Text = _metric.MetricID_Name
    End Sub
    Private Sub SetUpDEAllDataSeriesTab()
        SetUpAllDataSeriesGrid()
        rtxtDEAllDataSeriesComments.Enabled = False
        Me.PMLoadReportingYearTab2()
        _AllComments = GetComments(_metric.CID, rcbDEAllDataSeriesYear.Text)
        _DEPoint = New DEPoints
        _DEPoint.GetMetric(Me.GetSelectedMetric, rcbDEAllDataSeriesYear.Text)
        LoadMonthsForDLB()

        'Me.rlblMetricIDNameTab2.Text = _metric.MetricID_Name
    End Sub
    Private Sub SetUpDEAllDataSeriesMonthTab()
        SetUpAllMonthsDSGrid()
        Me.PMLoadReportingYearTab3()
        _DEPoint = New DEPoints
        _DEPoint.GetMetric(Me.GetSelectedMetric, rcbAllDS_MonthsYear.Text)
        'Me.rlblMetricIDNameTab3.Text = _metric.MetricID_Name
        DataEntryTABbyDSMonths(Me.rcbAllDS_MonthsYear.Text)
        rlblMetricIDNameTab3.Text = _metric.MetricID & "--" & _metric.MetricName
    End Sub
#Region "Misc"
    Public Property Metric() As MetricDTO 'rec and sets metric record
        Get
            Return _metric
        End Get
        Set(ByVal value As MetricDTO)
            _metric = value
        End Set
    End Property
#End Region
#Region "Comments"
    Public Function GetComments(ByVal CID As Integer, ByVal Year As Integer) As Dictionary(Of Integer, CommentsDTO)
        Dim data As New Dictionary(Of Integer, CommentsDTO)
        Dim db As New ScorecardCommentsDB

        For x As Integer = 1 To 12

            Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(Year, x, CID)
            If dto.Count = 1 Then
                dto(0).ClearBuffer()
                data.Add(x, dto(0))
            Else
                Dim DEcomments As New CommentsDTO
                DEcomments.cID = _metric.CID
                'If tiAllMonths.IsSelected = True Then
                DEcomments.ReportingYear = Year
                'Else
                'D()Ecomments.ReportingYear = Year
                'End If
                DEcomments.Month = x
                DEcomments.ChangedBy = Environment.UserName
                DEcomments.ClearBuffer()
                data.Add(x, DEcomments)
            End If
        Next
        Return data
    End Function
    Private Function getcurrentrow(ByVal grid As DataGridView) As UPMPointsDTO
        Dim dto As UPMPointsDTO = Nothing
        If grid.SelectedRows.Count = 1 Then
            dto = grid.SelectedRows(0).DataBoundItem
        End If
        Return dto
    End Function
#End Region
#Region "Performance & Achievement Levels"
    Private Sub getPerfLevelMessage(ByVal perflevel As String, ByVal month As String)
        Select Case perflevel
            Case "Below Min"
                RadMessageBox.Show("<html>Actual for <b>" + month + " </b>is <b>" + perflevel + "</b>, a comment is required.</html>")
                Me.rlblCRTab1Ind.Visible = True
                Me.rlblCRTab2Ind.Visible = True
            Case "Minium"
                RadMessageBox.Show("<html>The comments field is empty.  Please enter a comment to continue saving or select cancel.</html>")
            Case "Target"
                RadMessageBox.Show("<html>The comments field is empty.  Please enter a comment to continue saving or select cancel.</html>")
            Case "Maximum"
                RadMessageBox.Show("<html>The comments field is empty.  Please enter a comment to continue saving or select cancel.</html>")
        End Select
    End Sub
    Private Function getPerfLevelForeColor(ByVal perflevel As String) As Color
        Select Case perflevel
            Case "Below Min"
                Return Color.Black
            Case "Minimum"
                Return Color.Black
            Case "Target"
                Return Color.Black
            Case "Maximum"
                Return Color.AntiqueWhite
            Case Else
                Return Color.Black
        End Select
    End Function
    Private Function getPerfLevelColor(ByVal perflevel As String) As Color
        Select Case perflevel
            Case "Below Min"
                Return Color.Red
            Case "Minimum"
                Return Color.Yellow
            Case "Target"
                Return Color.White
            Case "Maximum"
                Return Color.Green
            Case Else
                Return Color.White
        End Select
    End Function
    Public Sub DetermineLevels(ByVal dto As UPMPointsDTO, ByRef perfLevel As String, ByRef achLevel As Nullable(Of Integer))
        Dim db As New PMPointsDB
        Dim invdb As New PMMetricsDB
        perfLevel = String.Empty
        achLevel = Nothing
        If dto.dsDescription = "YTD" Then
            Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(dto.CID, dto.MonthDesc, dto.ReportingYear)
            Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(dto.CID, dto.MonthDesc, dto.ReportingYear)
            Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(dto.CID, dto.MonthDesc, dto.ReportingYear)
            Dim inverse As String = invdb.GetMetricInvScale(dto.CID)
            If mindto.Count = 1 And maxdto.Count = 1 And targetdto.Count = 1 And Not IsDBNull(dto.NullableYValue) Then
                Dim min As Nullable(Of Decimal)
                If IsDBNull(mindto(0).NullableYValue) Then min = Nothing Else min = mindto(0).YValue
                Dim max As Nullable(Of Decimal)
                If IsDBNull(maxdto(0).NullableYValue) Then min = Nothing Else max = maxdto(0).YValue
                Dim tar As Nullable(Of Decimal)
                If IsDBNull(targetdto(0).NullableYValue) Then tar = Nothing Else tar = targetdto(0).YValue
                Dim act As Nullable(Of Decimal)
                If IsDBNull(dto.NullableYValue) Then act = Nothing Else act = dto.YValue
                If Not IsNothing(min) And Not IsNothing(max) And Not IsNothing(tar) And Not IsNothing(act) Then
                    perfLevel = db.GetPerformanceLevel(min, tar, max, act, inverse)
                    achLevel = invdb.Get0to200Scale(min, tar, max, act, inverse)
                End If
            End If
        End If
        If dto.dsDescription = "MONTHLY" Then
            Dim mindto As List(Of UPMPointsDTO) = db.GetMonthlyMinimumValue(dto.CID, dto.MonthDesc, dto.ReportingYear)
            Dim maxdto As List(Of UPMPointsDTO) = db.GetMonthlyMaximumValue(dto.CID, dto.MonthDesc, dto.ReportingYear)
            Dim targetdto As List(Of UPMPointsDTO) = db.GetMonthlyTargetValue(dto.CID, dto.MonthDesc, dto.ReportingYear)
            Dim inverse As String = invdb.GetMetricInvScale(dto.CID)
            If mindto.Count = 1 And maxdto.Count = 1 And targetdto.Count = 1 And Not IsDBNull(dto.NullableYValue) Then
                Dim min As Nullable(Of Decimal)
                If IsDBNull(mindto(0).NullableYValue) Then min = Nothing Else min = mindto(0).YValue
                Dim max As Nullable(Of Decimal)
                If IsDBNull(maxdto(0).NullableYValue) Then min = Nothing Else max = maxdto(0).YValue
                Dim tar As Decimal
                If IsDBNull(targetdto(0).NullableYValue) Then tar = Nothing Else tar = targetdto(0).YValue
                Dim act As Nullable(Of Decimal)
                If IsDBNull(dto.NullableYValue) Then act = Nothing Else act = dto.YValue
                If Not IsNothing(min) And Not IsNothing(max) And Not IsNothing(tar) And Not IsNothing(act) Then
                    perfLevel = db.GetPerformanceLevel(min, tar, max, act, inverse)
                    achLevel = invdb.Get0to200Scale(min, tar, max, act, inverse)
                End If
            End If
        End If
        If dto.dsDescription = "PE" Then
            Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(dto.CID, "DEC", dto.ReportingYear)
            Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(dto.CID, "DEC", dto.ReportingYear)
            Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(dto.CID, "DEC", dto.ReportingYear)
            Dim inverse As String = invdb.GetMetricInvScale(dto.CID)
            If mindto.Count = 1 And maxdto.Count = 1 And targetdto.Count = 1 And Not IsDBNull(dto.NullableYValue) Then
                Dim min As Nullable(Of Decimal)
                If IsDBNull(mindto(0).NullableYValue) Then min = Nothing Else min = mindto(0).YValue
                Dim max As Nullable(Of Decimal)
                If IsDBNull(maxdto(0).NullableYValue) Then min = Nothing Else max = maxdto(0).YValue
                Dim tar As Nullable(Of Decimal)
                If IsDBNull(targetdto(0).NullableYValue) Then tar = Nothing Else tar = targetdto(0).YValue
                Dim act As Nullable(Of Decimal)
                If IsDBNull(dto.NullableYValue) Then act = Nothing Else act = dto.YValue
                If Not IsNothing(min) And Not IsNothing(max) And Not IsNothing(tar) And Not IsNothing(act) Then
                    perfLevel = db.GetPerformanceLevel(min, tar, max, act, inverse)
                    achLevel = invdb.Get0to200Scale(min, tar, max, act, inverse)
                End If
            End If
        End If
    End Sub

#End Region
#Region "All Months TAB"
    Public Sub SetUpAllMonthsGrid()
        dgvPointsByMonths.AutoGenerateColumns = False
        dgvPointsByMonths.AllowUserToAddRows = False
        dgvPointsByMonths.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPointsByMonths.Columns.Clear()

        Dim col As New DataGridViewTextBoxColumn()
        col.DataPropertyName = "MonthDesc"
        col.Width = 75
        col.HeaderText = "Months"
        col.ReadOnly = True
        dgvPointsByMonths.Columns.Add(col)

        Dim db As New PMMetricsDB
        Dim SecUGDB As New SecurityUserGroupsDB
        Dim SecurityUserAdmin As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "Admin")
        Dim SecurityUserPWRUPM As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRUPM")
        Dim col2 As New DataGridViewTextBoxColumn()
        col2.DataPropertyName = "YValue"
        col2.Width = 110
        col2.HeaderText = "Points"
        If SecurityUserAdmin = True Or SecurityUserPWRUPM = True Or db.IsUserDataProvider(_metric.CID, Environment.UserName) = True Then
            col2.ReadOnly = False
        Else
            col2.ReadOnly = True
        End If
        dgvPointsByMonths.Columns.Add(col2)

        Dim col3 As New DataGridViewTextBoxColumn()
        col3.DataPropertyName = "PerfLevel"
        col3.Width = 110
        col3.HeaderText = "Perf Level"
        col3.ReadOnly = True
        dgvPointsByMonths.Columns.Add(col3)
        Dim col4 As New DataGridViewTextBoxColumn()
        col4.DataPropertyName = "AchLevel"
        col4.Width = 110
        col4.HeaderText = "Ach Level"
        col4.ReadOnly = True
        dgvPointsByMonths.Columns.Add(col4)
    End Sub

    Private Sub rtxtDEAMComments_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtxtDEAllMonthsComments.TextChanged
        Dim dto As UPMPointsDTO = getcurrentrow(dgvPointsByMonths)
        If Not IsNothing(dto) Then
            If _AllComments(dto.month).Comments <> rtxtDEAllMonthsComments.Text Then
                _AllComments(dto.month).Comments = rtxtDEAllMonthsComments.Text ' get old row months and put comments in dict
            End If
        End If
    End Sub
    Private Sub dgvPointsByMonths_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvPointsByMonths.CellEndEdit
        If e.ColumnIndex = 1 Then
            Dim value As UPMPointsDTO = dgvPointsByMonths.Rows(e.RowIndex).DataBoundItem

            DetermineLevels(value, value.PerfLevel, value.AchLevel)

            'item.ModificationType = IModificationIndicator.ModificationTypes.Edited
            Dim db As New PMMetricsDB
            Dim series As String = value.dsDescription
            Dim newrowdata As UPMPointsDTO = dgvPointsByMonths.SelectedRows(0).DataBoundItem
            If _metric.CommentNotRequired = False Then
                Me.rlblCNRTab1.Visible = False
                If series = "YTD" Or series = "MONTHLY" Or series = "PE" Then
                    If db.IsUserDataProvider(newrowdata.CID, Environment.UserName) = True Then

                        DetermineLevels(newrowdata, newrowdata.PerfLevel, newrowdata.AchLevel)
                        If newrowdata.PerfLevel = "Below Min" Then
                            If Me.rtxtDEAllMonthsComments.Text = "" Then
                                getPerfLevelMessage(newrowdata.PerfLevel, value.MonthDesc)
                            End If
                        Else
                            rlblCRTab1Ind.Visible = False
                        End If
                    End If
                End If
            Else
                Me.rlblCNRTab1.Visible = True
            End If
            
        End If
    End Sub
    Private Sub dgvDEAMPoints_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvPointsByMonths.SelectionChanged
        If dgvPointsByMonths.SelectedRows.Count = 1 Then
            Dim db As New PMMetricsDB
            Dim SecUGDB As New SecurityUserGroupsDB
            Dim SecurityUserAdmin As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "Admin")
            Dim SecurityUserPWRUPM As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRUPM")

            If SecurityUserAdmin = True Or SecurityUserPWRUPM = True Or db.IsUserCommentProvider(_metric.CID, Environment.UserName) = True Then
                Me.rtxtDEAllMonthsComments.Enabled = True
            Else
                Me.rtxtDEAllMonthsComments.Enabled = False
            End If

            'get comments for oldrow
            Dim newrowdata As UPMPointsDTO = dgvPointsByMonths.SelectedRows(0).DataBoundItem
            Select Case newrowdata.month
                Case 1
                    rlblDEAMComments.Text = "January Comment"
                Case 2
                    rlblDEAMComments.Text = "February Comment"
                Case 3
                    rlblDEAMComments.Text = "March Comment"
                Case 4
                    rlblDEAMComments.Text = "April Comment"
                Case 5
                    rlblDEAMComments.Text = "May Comment"
                Case 6
                    rlblDEAMComments.Text = "June Comment"
                Case 7
                    rlblDEAMComments.Text = "July Comment"
                Case 8
                    rlblDEAMComments.Text = "August Comment"
                Case 9
                    rlblDEAMComments.Text = "September Comment"
                Case 10
                    rlblDEAMComments.Text = "October Comment"
                Case 11
                    rlblDEAMComments.Text = "November Comment"
                Case 12
                    rlblDEAMComments.Text = "December Comment"
            End Select

            If Not IsNothing(newrowdata) Then
                rtxtDEAllMonthsComments.Text = _AllComments(newrowdata.month).Comments ' get what is in dict and place in new row
            End If

            'Dim db As New PMMetricsDB
            Dim series As String = rcbDEAllMonthsDataSeries.Text
            If _metric.CommentNotRequired = False Then
                Me.rlblCNRTab1.Visible = False
                If series = "YTD" Or series = "MONTHLY" Or series = "PE" Then
                    If db.IsUserCommentProvider(newrowdata.CID, Environment.UserName) = True Then

                        DetermineLevels(newrowdata, newrowdata.PerfLevel, newrowdata.AchLevel)
                        If newrowdata.PerfLevel = "Below Min" Then
                            If Me.rtxtDEAllMonthsComments.Text = "" Then
                                getPerfLevelMessage(newrowdata.PerfLevel, newrowdata.MonthDesc)
                            End If
                        Else
                            rlblCRTab1Ind.Visible = False
                        End If
                    End If
                End If
            Else
                Me.rlblCNRTab1.Visible = True
            End If
        Else
            Me.rtxtDEAllMonthsComments.Text = ""
        End If
    End Sub
    Private Sub dgvDEAMPoints_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles dgvPointsByMonths.CellFormatting
        Dim colindex As Integer = e.ColumnIndex
        Dim series As String = rcbDEAllMonthsDataSeries.Text

        If colindex = 2 And (series = "YTD" Or series = "MONTHLY" Or series = "PE") Then
            e.CellStyle.BackColor = getPerfLevelColor(e.Value)
        End If
    End Sub
    Private Sub LoadDataSeriesforDLB()
        rcbDEAllMonthsDataSeries.DataSource = Nothing
        rcbDEAllMonthsDataSeries.DataSource = _DEPoint.GetSeriesDataList

        rcbDEAllMonthsDataSeries.DisplayMember = "Description"
        rcbDEAllMonthsDataSeries.ValueMember = "DSID"

        rtxtDEAMMetric.Text = _metric.MetricID
        rlblMetricIDNameTab1.Text = _metric.MetricID & "--" & _metric.MetricName
    End Sub
    Private Sub DataEntryAMTab(ByVal dsID As Integer, ByVal year As Integer)
        Dim dsdto As MetricDTO
        Dim db As New PMDataSeriesDB
        dsdto = Metric
        'rtxtDEAMComments.Enabled = False
        ' _AllComments = GetComments()

        Dim resultset As List(Of UPMPointsDTO) = db.GetMetricDSPointsByDSID(dsdto.CID, dsID, year)
        For Each item As UPMPointsDTO In resultset
            Dim perflevel As String = String.Empty
            Dim achlevel As Nullable(Of Integer) = Nothing
            DetermineLevels(item, perflevel, achlevel)
            item.PerfLevel = perflevel
            If perflevel = "" Then
                achlevel = Nothing
                item.AchLevel = achlevel
            Else
                item.AchLevel = achlevel
            End If
        Next
        dgvPointsByMonths.DataSource = Nothing
        dgvPointsByMonths.DataSource = resultset

    End Sub
    Private Function GetSelectedMetric() As MetricDTO
        Dim dto As MetricDTO
        dto = _metric
        Return dto
    End Function
    Private Function GetPointsDEAM(ByVal dsID As Integer) As DEPoints.PMMetricDataSeries
        Dim selectedSeries As New DEPoints.PMMetricDataSeries
        For Each item As UPMPointsDTO In Me.dgvPointsByMonths.DataSource
            item.ChangedBy = Environment.UserName
            selectedSeries.Points.Add(item)
        Next
        Return selectedSeries
    End Function
    Private Sub rgvDEAMPoints_DataError(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GridViewDataErrorEventArgs)
        If e.Exception.Message = "Object of type 'System.String' cannot be converted to type 'System.Nullable`1[System.Decimal]'." Then
            e.Cancel = True
        End If
    End Sub
    Private Sub rcbDEAMDS_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEAllMonthsDataSeries.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEAllMonthsDataSeries.SelectedValue) And rcbDEAllMMonthsYear.Text > "" Then
            DataEntryAMTab(Me.rcbDEAllMonthsDataSeries.SelectedValue, rcbDEAllMMonthsYear.Text)
        End If
    End Sub
    Private Sub rcbDEAMYear_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rcbDEAllMMonthsYear.TextChanged
        If rcbDEAllMMonthsYear.Text = String.Empty Then Exit Sub
        Dim db As New PMLockYearDB
        'Dim lydto As New LockYearDTO
        Dim lydto As List(Of LockYearDTO) = db.GetLock(rcbDEAllMMonthsYear.Text)
        If lydto.Count = 0 Then
            rbtnDESave.Enabled = True
            rlblDataLocked.Text = ""
        ElseIf lydto(0).Locked = True Then
            rbtnDESave.Enabled = False
            rlblDataLocked.Text = "Data Locked for Selected Year"
        Else
            rbtnDESave.Enabled = True
            rlblDataLocked.Text = ""
        End If
    End Sub
    Private Sub rcbDEAMYear_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEAllMMonthsYear.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEAllMonthsDataSeries.SelectedValue) And rcbDEAllMMonthsYear.Text > "" Then
            DataEntryAMTab(Me.rcbDEAllMonthsDataSeries.SelectedValue, rcbDEAllMMonthsYear.Text)
        End If
    End Sub
    Private Sub PMLoadReportingYear()
        Dim pre As New myPresenter
        Dim list As New List(Of Integer)
        'Me.rcbDEAMYear.Text = String.Empty
        For x As Integer = Date.Now.Year - 5 To Date.Now.Year + 5
            list.Add(x)
        Next
        pre.LoadCombo(Me.rcbDEAllMMonthsYear, list)
        Dim db As New PMLockYearDB
        'Dim lydto As New LockYearDTO
        Dim year As Integer = Date.Now.Year
        If Date.Now.Month = 1 Then
            year = Date.Now.Year - 1
        End If
        Me.rcbDEAllMMonthsYear.Text = year
    End Sub
#End Region
#Region "All Data-Series TAB"
    Private Sub PMLoadReportingYearTab2()
        Dim pre As New myPresenter
        Dim list As New List(Of Integer)
        'Me.rcbDEADSYear.Text = String.Empty
        For x As Integer = Date.Now.Year - 5 To Date.Now.Year + 5
            list.Add(x)
        Next
        pre.LoadCombo(Me.rcbDEAllDataSeriesYear, list)
        Dim year As Integer = Date.Now.Year
        If Date.Now.Month = 1 Then
            year = Date.Now.Year - 1
        End If
        Me.rcbDEAllDataSeriesYear.Text = year
    End Sub
    Private Sub rcbDEADSYear_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rcbDEAllDataSeriesYear.TextChanged
        If rcbDEAllDataSeriesYear.Text = String.Empty Then Exit Sub
        Dim db As New PMLockYearDB
        'Dim lydto As New LockYearDTO
        Dim lydto As List(Of LockYearDTO) = db.GetLock(rcbDEAllDataSeriesYear.Text)
        If lydto.Count = 0 Then
            rbtnDESave.Enabled = True
            rlblDataLocked.Text = ""
        ElseIf lydto(0).Locked = True Then
            rbtnDESave.Enabled = False
            rlblDataLocked.Text = "Data Locked for Selected Year"
        Else
            rbtnDESave.Enabled = True
            rlblDataLocked.Text = ""
        End If
    End Sub
    Private Sub DataEntryTABbyDS(ByVal month As Integer, ByVal year As Integer)
        Dim dsdto As MetricDTO
        Dim db As New PMDataSeriesDB
        dsdto = Metric

        Dim resultset As List(Of UPMPointsDTO) = db.GetUPMDataSeriesByMonth(dsdto.CID, year, rcbDEAllDataSeriesMonth.SelectedValue)
        For Each item As UPMPointsDTO In resultset
            Dim perflevel As String = String.Empty
            Dim achlevel As Nullable(Of Integer) = Nothing
            DetermineLevels(item, perflevel, achlevel)
            item.PerfLevel = perflevel
            If item.PerfLevel = "" Then
                item.AchLevel = Nothing 'System.DBNull.Value
            Else
                item.AchLevel = achlevel
            End If
        Next
        dgvPointsByDataSeries.DataSource = Nothing
        dgvPointsByDataSeries.DataSource = resultset
    End Sub
    Private Sub rtxtDEADSComments_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtxtDEAllDataSeriesComments.TextChanged
        Dim dto As UPMPointsDTO = getcurrentrow(dgvPointsByDataSeries)
        If Not IsNothing(dto) Then
            _AllComments(dto.month).Comments = rtxtDEAllDataSeriesComments.Text ' get old row months and put comments in dict
        End If
    End Sub
    Private Sub dgvPointsByDataSeries_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvPointsByDataSeries.CellEndEdit
        If e.ColumnIndex = 1 Then
            Dim value As UPMPointsDTO = dgvPointsByDataSeries.Rows(e.RowIndex).DataBoundItem

            DetermineLevels(value, value.PerfLevel, value.AchLevel)

            Dim db As New PMMetricsDB
            Dim series As String = value.dsDescription
            Dim newrowdata As UPMPointsDTO = dgvPointsByDataSeries.SelectedRows(0).DataBoundItem
            'If IsNothing(newrowdata.AchLevel) Then
            '    Dim achlevel As Nullable(Of Integer) = Nothing
            '    newrowdata.AchLevel = achlevel
            'End If
            If _metric.CommentNotRequired = False Then
                Me.rlblCNRTab2.Visible = False
                If series = "YTD" Or series = "MONTHLY" Or series = "PE" Then
                    If db.IsUserDataProvider(newrowdata.CID, Environment.UserName) = True Then
                        DetermineLevels(newrowdata, newrowdata.PerfLevel, newrowdata.AchLevel)
                        If newrowdata.PerfLevel = "Below Min" Then
                            If Me.rtxtDEAllMonthsComments.Text = "" Then
                                getPerfLevelMessage(newrowdata.PerfLevel, value.MonthDesc)
                            End If
                        Else
                            rlblCRTab2Ind.Visible = False
                        End If
                    End If
                End If
            Else
                Me.rlblCNRTab2.Visible = True
            End If
        End If
    End Sub
    Private Sub dgvDEADSPoints_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvPointsByDataSeries.SelectionChanged
        If dgvPointsByDataSeries.SelectedRows.Count = 1 Then
            Dim db As New PMMetricsDB
            Dim SecUGDB As New SecurityUserGroupsDB
            Dim SecurityUserAdmin As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "Admin")
            Dim SecurityUserPWRUPM As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRUPM")
            Dim SecurityUserCP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "CP")

            If SecurityUserAdmin = True Or SecurityUserPWRUPM = True Or db.IsUserCommentProvider(_metric.CID, Environment.UserName) = True Then
                Me.rtxtDEAllDataSeriesComments.Enabled = True
            Else
                Me.rtxtDEAllDataSeriesComments.Enabled = False
            End If

            'get comments for oldrow
            Dim newrowdata As UPMPointsDTO = dgvPointsByDataSeries.SelectedRows(0).DataBoundItem
            rtxtDEAllDataSeriesComments.Text = _AllComments(newrowdata.month).Comments ' get what is in dict and place in new row
            'Dim db As New PMMetricsDB
            Dim series As String = newrowdata.dsDescription
            If _metric.CommentNotRequired = False Then
                Me.rlblCNRTab2.Visible = False
                If series = "YTD" Or series = "MONTHLY" Or series = "PE" Then
                    If db.IsUserCommentProvider(newrowdata.CID, Environment.UserName) = True Then

                        DetermineLevels(newrowdata, newrowdata.PerfLevel, newrowdata.AchLevel)
                        If newrowdata.PerfLevel = "Below Min" Then
                            If Me.rtxtDEAllDataSeriesComments.Text = "" Then
                                getPerfLevelMessage(newrowdata.PerfLevel, newrowdata.MonthDesc)
                            End If
                        Else
                            rlblCRTab2Ind.Visible = False
                        End If
                    End If
                End If
            Else
                Me.rlblCNRTab2.Visible = True
            End If
        Else
            Me.rtxtDEAllDataSeriesComments.Text = ""
        End If
    End Sub
    Private Sub dgvDEADSPoints_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles dgvPointsByDataSeries.CellFormatting
        Dim colindex As Integer = e.ColumnIndex
        Dim months As String = rcbDEAllDataSeriesMonth.Text
        If colindex = 2 Then
            e.CellStyle.BackColor = getPerfLevelColor(e.Value)
        End If
    End Sub
    Public Sub LoadMonthsForDLB()
        rcbDEAllDataSeriesMonth.DataSource = Nothing
        rcbDEAllDataSeriesMonth.DataSource = _DEPoint.GetMonthDataList

        rcbDEAllDataSeriesMonth.DisplayMember = "MonthDescription"
        rcbDEAllDataSeriesMonth.ValueMember = "MonthVal"

        Dim CurrMon As Integer = IIf((Month(Today()) - 1) = 0, 12, (Month(Today()) - 1))
        rcbDEAllDataSeriesMonth.SelectedValue = CurrMon

        rtxtDEADSMetric.Text = _metric.MetricID
        rlblMetricIDNameTab2.Text = _metric.MetricID & "--" & _metric.MetricName
    End Sub

    Private Sub rcbDEADSYear_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEAllDataSeriesYear.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEAllDataSeriesMonth.SelectedValue) And rcbDEAllDataSeriesYear.Text > "" Then
            DataEntryTABbyDS(Me.rcbDEAllDataSeriesMonth.SelectedValue, rcbDEAllDataSeriesYear.Text)
        End If
    End Sub
    Private Sub rcbDEADSMonth_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEAllDataSeriesMonth.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEAllDataSeriesMonth.SelectedValue) And rcbDEAllDataSeriesYear.Text > "" Then
            DataEntryTABbyDS(Me.rcbDEAllDataSeriesMonth.SelectedValue, rcbDEAllDataSeriesYear.Text)
            Select Case Me.rcbDEAllDataSeriesMonth.SelectedValue
                Case 1
                    rlblDEADSComments.Text = "January Comments"
                Case 2
                    rlblDEADSComments.Text = "February Comments"
                Case 3
                    rlblDEADSComments.Text = "March Comments"
                Case 4
                    rlblDEADSComments.Text = "April Comments"
                Case 5
                    rlblDEADSComments.Text = "May Comments"
                Case 6
                    rlblDEADSComments.Text = "June Comments"
                Case 7
                    rlblDEADSComments.Text = "July Comments"
                Case 8
                    rlblDEADSComments.Text = "August Comments"
                Case 9
                    rlblDEADSComments.Text = "September Comments"
                Case 10
                    rlblDEADSComments.Text = "October Comments"
                Case 11
                    rlblDEADSComments.Text = "November Comments"
                Case 12
                    rlblDEADSComments.Text = "December Comments"
            End Select
        End If
    End Sub
    Public Sub SetUpAllDataSeriesGrid()
        dgvPointsByDataSeries.AutoGenerateColumns = False
        dgvPointsByDataSeries.AllowUserToAddRows = False
        dgvPointsByDataSeries.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPointsByDataSeries.Columns.Clear()

        Dim db As New PMMetricsDB
        Dim SecUGDB As New SecurityUserGroupsDB
        Dim SecurityUserAdmin As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "Admin")
        Dim SecurityUserPWRUPM As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRUPM")
        Dim SecurityUserDP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "DP")

        Dim col As New DataGridViewTextBoxColumn()
        col.DataPropertyName = "dsDescription"
        col.Width = 150
        col.HeaderText = "Data Series"
        col.ReadOnly = True
        dgvPointsByDataSeries.Columns.Add(col)

        Dim col2 As New DataGridViewTextBoxColumn()
        col2.DataPropertyName = "YValue"
        col2.Width = 110
        col2.HeaderText = "Points"

        If SecurityUserAdmin = True Or SecurityUserPWRUPM = True Or db.IsUserDataProvider(_metric.CID, Environment.UserName) = True Then
            col2.ReadOnly = False
        Else
            col2.ReadOnly = True
        End If
        dgvPointsByDataSeries.Columns.Add(col2)

        Dim col3 As New DataGridViewTextBoxColumn()
        col3.DataPropertyName = "PerfLevel"
        col3.Width = 110
        col3.HeaderText = "Perf Level"
        col3.ReadOnly = True
        dgvPointsByDataSeries.Columns.Add(col3)
        Dim col4 As New DataGridViewTextBoxColumn()
        col4.DataPropertyName = "AchLevel"
        col4.Width = 110
        col4.HeaderText = "Ach Level"
        col4.ReadOnly = True
        dgvPointsByDataSeries.Columns.Add(col4)
    End Sub
#End Region
#Region "All Months Across & Data Series Down"
    Private Sub rcbAllDS_MonthsYear_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbAllDS_MonthsYear.SelectedIndexChanged
        If rcbAllDS_MonthsYear.Text > "" Then
            DataEntryTABbyDSMonths(Me.rcbAllDS_MonthsYear.Text)
        End If
    End Sub
    Private Sub rcbAllDS_MonthsYear_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rcbAllDS_MonthsYear.TextChanged
        If rcbAllDS_MonthsYear.Text = String.Empty Then Exit Sub
        Dim db As New PMLockYearDB
        'Dim lydto As New LockYearDTO
        Dim lydto As List(Of LockYearDTO) = db.GetLock(rcbAllDS_MonthsYear.Text)
        If lydto.Count = 0 Then
            rbtnDESave.Enabled = True
            rlblDataLocked.Text = ""
        ElseIf lydto(0).Locked = True Then
            rbtnDESave.Enabled = False
            rlblDataLocked.Text = "Data Locked for Selected Year"
        Else
            rbtnDESave.Enabled = True
            rlblDataLocked.Text = ""
        End If
    End Sub

    Private Sub PMLoadReportingYearTab3()
        Dim pre As New myPresenter
        Dim list As New List(Of Integer)
        'Me.rcbAllDS_MonthsYear.Text = String.Empty
        For x As Integer = Date.Now.Year - 5 To Date.Now.Year + 5
            list.Add(x)
        Next
        pre.LoadCombo(Me.rcbAllDS_MonthsYear, list)

        Dim year As Integer = Date.Now.Year
        If Date.Now.Month = 1 Then
            year = Date.Now.Year - 1
        End If
        Me.rcbAllDS_MonthsYear.Text = year
    End Sub
    Private Sub DataEntryTABbyDSMonths(ByVal year As Integer)
        Dim dsdto As MetricDTO
        Dim db As New PMDataSeriesDB
        Dim ib As New UPMPointsDTO
        dsdto = Metric
        'Load data points into grid
        Dim resultset As List(Of UPMPointsDTO) = db.GetUPMDataSeriesPointsByMonths(dsdto.CID, year)
        ib.InitializeBuffers()
        For Each item As UPMPointsDTO In resultset
            Dim perflevel As String = ""
            Dim achlevel As Nullable(Of Integer) = Nothing

            item.MonthDesc = item.JANMonDesc
            item.ReportingYear = item.JANReportingYear
            item.YValue = item.JANPoints
            DetermineLevels(item, perflevel, achlevel)
            item.JANPerflevel = perflevel

            item.MonthDesc = item.FEBMonDesc
            item.ReportingYear = item.FEBReportingYear
            item.YValue = item.FEBPoints
            DetermineLevels(item, perflevel, achlevel)
            item.FEBPerflevel = perflevel

            item.MonthDesc = item.MARMonDesc
            item.ReportingYear = item.MARReportingYear
            item.YValue = item.MARPoints
            DetermineLevels(item, perflevel, achlevel)
            item.MARPerflevel = perflevel

            item.MonthDesc = item.APRMonDesc
            item.ReportingYear = item.APRReportingYear
            item.YValue = item.APRPoints
            DetermineLevels(item, perflevel, achlevel)
            item.APRPerflevel = perflevel

            item.MonthDesc = item.MayMonDesc
            item.ReportingYear = item.MayReportingYear
            item.YValue = item.MAYPoints
            DetermineLevels(item, perflevel, achlevel)
            item.MayPerflevel = perflevel

            item.MonthDesc = item.JunMonDesc
            item.ReportingYear = item.JunReportingYear
            item.YValue = item.JUNPoints
            DetermineLevels(item, perflevel, achlevel)
            item.JUNPerflevel = perflevel

            item.MonthDesc = item.JulMonDesc
            item.ReportingYear = item.JULReportingYear
            item.YValue = item.JULPoints
            DetermineLevels(item, perflevel, achlevel)
            item.JULPerflevel = perflevel

            item.MonthDesc = item.AUGMonDesc
            item.ReportingYear = item.AUGReportingYear
            item.YValue = item.AUGPoints
            DetermineLevels(item, perflevel, achlevel)
            item.AUGPerflevel = perflevel

            item.MonthDesc = item.SEPMonDesc
            item.ReportingYear = item.SEPReportingYear
            item.YValue = item.SEPPoints
            DetermineLevels(item, perflevel, achlevel)
            item.SEPPerflevel = perflevel
            perflevel = String.Empty
            achlevel = 0

            item.MonthDesc = item.OCTMonDesc
            item.ReportingYear = item.OCTReportingYear
            item.YValue = item.OCTPoints
            DetermineLevels(item, perflevel, achlevel)
            item.OCTPerflevel = perflevel

            item.MonthDesc = item.NOVMonDesc
            item.ReportingYear = item.NOVReportingYear
            item.YValue = item.NOVPoints
            DetermineLevels(item, perflevel, achlevel)
            item.NOVPerflevel = perflevel
            perflevel = String.Empty
            achlevel = 0

            item.MonthDesc = item.DECMonDesc
            item.ReportingYear = item.DECReportingYear
            item.YValue = item.DECPoints
            DetermineLevels(item, perflevel, achlevel)
            item.DECPerflevel = perflevel

            item.InitializeBuffers()
        Next
        dgvPointsMatrixDSM.DataSource = Nothing
        dgvPointsMatrixDSM.DataSource = resultset
    End Sub
    Public Sub SetUpAllMonthsDSGrid()
        If IsNothing(_metric) Then Exit Sub
        dgvPointsMatrixDSM.AutoGenerateColumns = False
        dgvPointsMatrixDSM.AllowUserToAddRows = False
        dgvPointsMatrixDSM.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPointsMatrixDSM.Columns.Clear()

        Dim col1 As New DataGridViewTextBoxColumn()
        col1.DataPropertyName = "dsDescription"
        col1.Width = 130
        col1.HeaderText = "Data Series"
        col1.ReadOnly = True
        dgvPointsMatrixDSM.Columns.Add(col1)

        Dim db As New PMMetricsDB
        Dim SecUGDB As New SecurityUserGroupsDB
        Dim SecurityUserAdmin As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "Admin")
        Dim SecurityUserPWRUPM As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRUPM")
        Dim SecurityUserDP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "DP")
        Dim SecurityUserBDP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "BDP")

        If SecurityUserAdmin = True Or SecurityUserPWRUPM = True Or db.IsUserDataProvider(_metric.CID, Environment.UserName) = True Then
            Dim col2 As New DataGridViewTextBoxColumn()
            col2.DataPropertyName = "JANPoints"
            col2.Width = 85
            col2.HeaderText = "JAN"
            col2.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col2)
            Dim col3 As New DataGridViewTextBoxColumn()
            col3.DataPropertyName = "FEBPoints"
            col3.Width = 85
            col3.HeaderText = "FEB"
            col3.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col3)
            Dim col4 As New DataGridViewTextBoxColumn()
            col4.DataPropertyName = "MARPoints"
            col4.Width = 85
            col4.HeaderText = "MAR"
            col4.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col4)
            Dim col5 As New DataGridViewTextBoxColumn()
            col5.DataPropertyName = "APRPoints"
            col5.Width = 85
            col5.HeaderText = "APR"
            col5.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col5)
            Dim col6 As New DataGridViewTextBoxColumn()
            col6.DataPropertyName = "MAYPoints"
            col6.Width = 85
            col6.HeaderText = "MAY"
            col6.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col6)
            Dim col7 As New DataGridViewTextBoxColumn()
            col7.DataPropertyName = "JUNPoints"
            col7.Width = 85
            col7.HeaderText = "JUN"
            col7.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col7)
            Dim col8 As New DataGridViewTextBoxColumn()
            col8.DataPropertyName = "JULPoints"
            col8.Width = 85
            col8.HeaderText = "JUL"
            col8.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col8)
            Dim col9 As New DataGridViewTextBoxColumn()
            col9.DataPropertyName = "AUGPoints"
            col9.Width = 85
            col9.HeaderText = "AUG"
            col9.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col9)
            Dim col10 As New DataGridViewTextBoxColumn()
            col10.DataPropertyName = "SEPPoints"
            col10.Width = 85
            col10.HeaderText = "SEP"
            col10.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col10)
            Dim col11 As New DataGridViewTextBoxColumn()
            col11.DataPropertyName = "OCTPoints"
            col11.Width = 85
            col11.HeaderText = "OCT"
            col11.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col11)
            Dim col12 As New DataGridViewTextBoxColumn()
            col12.DataPropertyName = "NOVPoints"
            col12.Width = 85
            col12.HeaderText = "NOV"
            col12.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col12)
            Dim col13 As New DataGridViewTextBoxColumn()
            col13.DataPropertyName = "DECPoints"
            col13.Width = 85
            col13.HeaderText = "DEC"
            col13.ReadOnly = False
            dgvPointsMatrixDSM.Columns.Add(col13)
        Else
            Dim col2 As New DataGridViewTextBoxColumn()
            col2.DataPropertyName = "JANPoints"
            col2.Width = 85
            col2.HeaderText = "JAN"
            col2.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col2)
            Dim col3 As New DataGridViewTextBoxColumn()
            col3.DataPropertyName = "FEBPoints"
            col3.Width = 85
            col3.HeaderText = "FEB"
            col3.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col3)
            Dim col4 As New DataGridViewTextBoxColumn()
            col4.DataPropertyName = "MARPoints"
            col4.Width = 85
            col4.HeaderText = "MAR"
            col4.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col4)
            Dim col5 As New DataGridViewTextBoxColumn()
            col5.DataPropertyName = "APRPoints"
            col5.Width = 85
            col5.HeaderText = "APR"
            col5.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col5)
            Dim col6 As New DataGridViewTextBoxColumn()
            col6.DataPropertyName = "MAYPoints"
            col6.Width = 85
            col6.HeaderText = "MAY"
            col6.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col6)
            Dim col7 As New DataGridViewTextBoxColumn()
            col7.DataPropertyName = "JUNPoints"
            col7.Width = 85
            col7.HeaderText = "JUN"
            col7.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col7)
            Dim col8 As New DataGridViewTextBoxColumn()
            col8.DataPropertyName = "JULPoints"
            col8.Width = 85
            col8.HeaderText = "JUL"
            col8.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col8)
            Dim col9 As New DataGridViewTextBoxColumn()
            col9.DataPropertyName = "AUGPoints"
            col9.Width = 85
            col9.HeaderText = "AUG"
            col9.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col9)
            Dim col10 As New DataGridViewTextBoxColumn()
            col10.DataPropertyName = "SEPPoints"
            col10.Width = 85
            col10.HeaderText = "SEP"
            col10.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col10)
            Dim col11 As New DataGridViewTextBoxColumn()
            col11.DataPropertyName = "OCTPoints"
            col11.Width = 85
            col11.HeaderText = "OCT"
            col11.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col11)
            Dim col12 As New DataGridViewTextBoxColumn()
            col12.DataPropertyName = "NOVPoints"
            col12.Width = 85
            col12.HeaderText = "NOV"
            col12.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col12)
            Dim col13 As New DataGridViewTextBoxColumn()
            col13.DataPropertyName = "DECPoints"
            col13.Width = 85
            col13.HeaderText = "DEC"
            col13.ReadOnly = True
            dgvPointsMatrixDSM.Columns.Add(col13)
        End If
    End Sub
    Private Sub dgvPointsMatrixDSM_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvPointsMatrixDSM.CellEndEdit
        Dim colindex As Integer = e.ColumnIndex
        Dim perflevel As String = String.Empty
        Dim achlevel As Nullable(Of Integer) = Nothing
        Dim value As UPMPointsDTO = dgvPointsMatrixDSM.Rows(e.RowIndex).DataBoundItem
        Select Case colindex
            Case 1
                value.MonthDesc = "JAN"
                value.NullableYValue = value.JANPoints
                DetermineLevels(value, perflevel, achlevel)
                value.JANPerflevel = perflevel
            Case 2
                value.MonthValue = 2
                value.MonthDesc = "FEB"
                value.NullableYValue = value.FEBPoints
                DetermineLevels(value, perflevel, achlevel)
                value.FEBPerflevel = perflevel
            Case 3
                value.MonthDesc = "MAR"
                value.NullableYValue = value.MARPoints
                DetermineLevels(value, perflevel, achlevel)
                value.MARPerflevel = perflevel
            Case 4
                value.MonthDesc = "APR"
                value.NullableYValue = value.APRPoints
                DetermineLevels(value, perflevel, achlevel)
                value.APRPerflevel = perflevel
            Case 5
                value.MonthDesc = "MAY"
                value.NullableYValue = value.MAYPoints
                DetermineLevels(value, perflevel, achlevel)
                value.MayPerflevel = perflevel
            Case 6
                value.MonthDesc = "JUN"
                value.NullableYValue = value.JUNPoints
                DetermineLevels(value, perflevel, achlevel)
                value.JUNPerflevel = perflevel
            Case 7
                value.MonthDesc = "JUL"
                value.NullableYValue = value.JULPoints
                DetermineLevels(value, perflevel, achlevel)
                value.JULPerflevel = perflevel
            Case 8
                value.MonthDesc = "AUG"
                value.NullableYValue = value.AUGPoints
                DetermineLevels(value, perflevel, achlevel)
                value.AUGPerflevel = perflevel
            Case 9
                value.MonthDesc = "SEP"
                value.NullableYValue = value.SEPPoints
                DetermineLevels(value, perflevel, achlevel)
                value.SEPPerflevel = perflevel
            Case 10
                value.MonthDesc = "OCT"
                value.NullableYValue = value.OCTPoints
                DetermineLevels(value, perflevel, achlevel)
                value.OCTPerflevel = perflevel
            Case 11
                value.MonthDesc = "NOV"
                value.NullableYValue = value.NOVPoints
                DetermineLevels(value, perflevel, achlevel)
                value.NOVPerflevel = perflevel
            Case 12
                value.MonthDesc = "DEC"
                value.NullableYValue = value.DECPoints
                DetermineLevels(value, perflevel, achlevel)
                value.DECPerflevel = perflevel
        End Select
    End Sub
    Private Sub dgvDEAMDSPoints_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvPointsMatrixDSM.SelectionChanged
        If dgvPointsMatrixDSM.SelectedRows.Count = 1 Then

        End If
    End Sub
    Private Sub dgvDEAMDSPoints_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles dgvPointsMatrixDSM.CellFormatting
        Dim colindex As Integer = e.ColumnIndex

        Dim rowindex As Integer = e.RowIndex
        'Dim colHeader As String = e.ColumnIndex
        'Dim months As String = rcbDEADSMonth.Text
        Dim item As UPMPointsDTO = dgvPointsMatrixDSM.Rows(e.RowIndex).DataBoundItem

        Select Case colindex
            Case 1
                e.CellStyle.BackColor = getPerfLevelColor(item.JANPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.JANPerflevel)
            Case 2
                e.CellStyle.BackColor = getPerfLevelColor(item.FEBPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.FEBPerflevel)
            Case 3
                e.CellStyle.BackColor = getPerfLevelColor(item.MARPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.MARPerflevel)
            Case 4
                e.CellStyle.BackColor = getPerfLevelColor(item.APRPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.APRPerflevel)
            Case 5
                e.CellStyle.BackColor = getPerfLevelColor(item.MayPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.MayPerflevel)
            Case 6
                e.CellStyle.BackColor = getPerfLevelColor(item.JUNPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.JUNPerflevel)
            Case 7
                e.CellStyle.BackColor = getPerfLevelColor(item.JULPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.JULPerflevel)
            Case 8
                e.CellStyle.BackColor = getPerfLevelColor(item.AUGPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.AUGPerflevel)
            Case 9
                e.CellStyle.BackColor = getPerfLevelColor(item.SEPPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.SEPPerflevel)
            Case 10
                e.CellStyle.BackColor = getPerfLevelColor(item.OCTPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.OCTPerflevel)
            Case 11
                e.CellStyle.BackColor = getPerfLevelColor(item.NOVPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.NOVPerflevel)
            Case 12
                e.CellStyle.BackColor = getPerfLevelColor(item.DECPerflevel)
                e.CellStyle.ForeColor = getPerfLevelForeColor(item.DECPerflevel)
        End Select

    End Sub
#End Region
#Region "Save Points"
    Private Sub rbtnDESave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDESave.Click
        If tiAllMonths.IsSelected Then
            SaveAllMonthTABData()
        ElseIf tiAllDS.IsSelected Then
            SaveAllDataSeriesTABData()
        Else
            SaveAllDataSeriesMonthsTABData()
        End If
    End Sub
    Public Sub SaveAllMonthTABData()
        Dim value As UPMPointsDTO = dgvPointsByMonths.SelectedRows(0).DataBoundItem
        Dim mdb As New PMMetricsDB
        Dim series As String = value.dsDescription
        Dim newrowdata As UPMPointsDTO = dgvPointsByMonths.SelectedRows(0).DataBoundItem
        If _metric.CommentNotRequired = False Then
            If series = "YTD" Or series = "MONTHLY" Or series = "PE" Then
                If mdb.IsUserCommentProvider(newrowdata.CID, Environment.UserName) = True And mdb.IsUserDataProvider(newrowdata.CID, Environment.UserName) = False Then
                    DetermineLevels(newrowdata, newrowdata.PerfLevel, newrowdata.AchLevel)
                    If Me.rtxtDEAllMonthsComments.Text = "" And newrowdata.PerfLevel = "Below Min" Then
                        getPerfLevelMessage(newrowdata.PerfLevel, value.MonthDesc)
                        'Exit Sub
                    End If
                End If
            End If
        End If

        Dim db As New PMPointsDB
        For Each item As UPMPointsDTO In Me.dgvPointsByMonths.DataSource
            If item.ModificationType = IModificationIndicator.ModificationTypes.Edited Then 'And Not IsDBNull(item.NullableYValue) Then
                db.SavePointsArchive(item)
                db.SaveUPMPoints(item)
            End If
        Next
        Dim dbcomments As New ScorecardCommentsDB
        For Each Item As CommentsDTO In _AllComments.Values
            If Item.Modified = True Then
                dbcomments.SaveComments(Item)
            End If
        Next

        RadMessageBox.Show("<html><b><size=8>Metric " + rlblMetricIDNameTab1.Text + "</b> saved successfully!</html>")
    End Sub
    Public Sub SaveAllDataSeriesTABData()
        Dim value As UPMPointsDTO = dgvPointsByDataSeries.SelectedRows(0).DataBoundItem
        Dim mdb As New PMMetricsDB
        Dim series As String = value.dsDescription
        Dim newrowdata As UPMPointsDTO = dgvPointsByDataSeries.SelectedRows(0).DataBoundItem
        If _metric.CommentNotRequired = False Then
            If series = "YTD" Or series = "MONTHLY" Or series = "PE" Then
                If mdb.IsUserCommentProvider(newrowdata.CID, Environment.UserName) = True And mdb.IsUserDataProvider(newrowdata.CID, Environment.UserName) = False Then
                    DetermineLevels(newrowdata, newrowdata.PerfLevel, newrowdata.AchLevel)
                    If Me.rtxtDEAllDataSeriesComments.Text = "" Then
                        getPerfLevelMessage(newrowdata.PerfLevel, value.MonthDesc)
                        'Exit Sub
                    End If
                End If
            End If
        End If
        
        Dim PointsSavedAllDataSeries As Boolean
        Dim CommentsSavedAllDataSeries As Boolean
        Dim db As New PMPointsDB
        For Each Item As UPMPointsDTO In Me.dgvPointsByDataSeries.DataSource
            If Item.ModificationType = IModificationIndicator.ModificationTypes.Edited Then
                db.SavePointsArchiveTAB2(Item)
                db.SaveUPMPointsTAB2(Item)
                PointsSavedAllDataSeries = True
            End If
        Next
        Dim dbcomments As New ScorecardCommentsDB
        For Each Item As CommentsDTO In _AllComments.Values
            If Item.Modified = True Then
                dbcomments.SaveComments(Item)
                CommentsSavedAllDataSeries = True
            End If
        Next
        If PointsSavedAllDataSeries = True And CommentsSavedAllDataSeries = True Then
            RadMessageBox.Show("<html>Points and comments saved successfully for the <b><size=8>" + rlblMetricIDNameTab2.Text + "</b></html> metric.")
        ElseIf PointsSavedAllDataSeries = False And CommentsSavedAllDataSeries = True Then
            RadMessageBox.Show("<html>Comments saved successfully for the <b><size=8>" + rlblMetricIDNameTab2.Text + "</b></html> metric.")
        ElseIf PointsSavedAllDataSeries = True And CommentsSavedAllDataSeries = False Then
            RadMessageBox.Show("<html>Points saved successfully for the <b><size=8>" + rlblMetricIDNameTab2.Text + "</b></html> metric.")
        End If
        PointsSavedAllDataSeries = False
        CommentsSavedAllDataSeries = False
    End Sub
    Public Sub SaveAllDataSeriesMonthsTABData()
        Dim year As Integer = rcbAllDS_MonthsYear.Text
        Dim db As New PMPointsDB
        Dim PointsSavedAllDSMonths As Boolean

        For Each Item As UPMPointsDTO In Me.dgvPointsMatrixDSM.DataSource
            For x As Integer = 1 To 12
                Dim dto As New UPMPointsDTO
                dto.C_ID = Item.CID
                Select Case x
                    Case 1
                        If Item._JANModified = True Then
                            dto.YValue = Item.JANPoints
                            dto.ReportingYear = If(Item.JANReportingYear = 0, year, Item.JANReportingYear)
                            dto.MonthValue = If(Item.JANMonthValue = 0, 1, Item.JANMonthValue)
                            dto.MonthDesc = If(Item.JANMonDesc = "", "JAN", Item.JANMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case (2)
                        If Item._FEBModified = True Then
                            dto.YValue = Item.FEBPoints
                            dto.ReportingYear = If(Item.FEBReportingYear = 0, year, Item.FEBReportingYear)
                            dto.MonthValue = If(Item.FEBMonthValue = 0, 2, Item.FEBMonthValue)
                            dto.MonthDesc = If(Item.FEBMonDesc = "", "FEB", Item.FEBMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 3
                        If Item._MARModified = True Then
                            dto.YValue = Item.MARPoints
                            dto.ReportingYear = If(Item.MARReportingYear = 0, year, Item.MARReportingYear)
                            dto.MonthValue = If(Item.MARMonthValue = 0, 3, Item.MARMonthValue)
                            dto.MonthDesc = If(Item.MARMonDesc = "", "MAR", Item.MARMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 4
                        If Item._APRModified = True Then
                            dto.YValue = Item.APRPoints
                            dto.ReportingYear = If(Item.APRReportingYear = 0, year, Item.APRReportingYear)
                            dto.MonthValue = If(Item.APRMonthValue = 0, 4, Item.APRMonthValue)
                            dto.MonthDesc = If(Item.APRMonDesc = "", "APR", Item.APRMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 5
                        If Item._MAYModified = True Then
                            dto.YValue = Item.MAYPoints
                            dto.ReportingYear = If(Item.MayReportingYear = 0, year, Item.MayReportingYear)
                            dto.MonthValue = If(Item.MayMonthValue = 0, 5, Item.MayMonthValue)
                            dto.MonthDesc = If(Item.MayMonDesc = "", "MAY", Item.MayMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 6
                        If Item._JUNModified = True Then
                            dto.YValue = Item.JUNPoints
                            dto.ReportingYear = If(Item.JANReportingYear = 0, year, Item.JunReportingYear)
                            dto.MonthValue = If(Item.JunMonthValue = 0, 6, Item.JunMonthValue)
                            dto.MonthDesc = If(Item.JunMonDesc = "", "JUN", Item.JunMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 7
                        If Item._JULModified = True Then
                            dto.YValue = Item.JULPoints
                            dto.ReportingYear = If(Item.JULReportingYear = 0, year, Item.JULReportingYear)
                            dto.MonthValue = If(Item.JULMonthValue = 0, 7, Item.JULMonthValue)
                            dto.MonthDesc = If(Item.JulMonDesc = "", "JUL", Item.JulMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 8
                        If Item._AUGModified = True Then
                            dto.YValue = Item.AUGPoints
                            dto.ReportingYear = If(Item.AUGReportingYear = 0, year, Item.AUGReportingYear)
                            dto.MonthValue = If(Item.AUGMonthValue = 0, 8, Item.AUGMonthValue)
                            dto.MonthDesc = If(Item.AUGMonDesc = "", "AUG", Item.AUGMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 9
                        If Item._SEPModified = True Then
                            dto.YValue = Item.SEPPoints
                            dto.ReportingYear = If(Item.SEPReportingYear = 0, year, Item.SEPReportingYear)
                            dto.MonthValue = If(Item.SEPMonthValue = 0, 9, Item.SEPMonthValue)
                            dto.MonthDesc = If(Item.SEPMonDesc = "", "SEP", Item.SEPMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 10
                        If Item._OCTModified = True Then
                            dto.YValue = Item.OCTPoints
                            dto.ReportingYear = If(Item.OCTReportingYear = 0, year, Item.OCTReportingYear)
                            dto.MonthValue = If(Item.OCTMonthValue = 0, 10, Item.OCTMonthValue)
                            dto.MonthDesc = If(Item.OCTMonDesc = "", "OCT", Item.OCTMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 11
                        If Item._NOVModified = True Then
                            dto.YValue = Item.NOVPoints
                            dto.ReportingYear = If(Item.NOVReportingYear = 0, year, Item.NOVReportingYear)
                            dto.MonthValue = If(Item.NOVMonthValue = 0, 11, Item.NOVMonthValue)
                            dto.MonthDesc = If(Item.NOVMonDesc = "", "NOV", Item.NOVMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                    Case 12
                        If Item._DECModified = True Then
                            dto.YValue = Item.DECPoints
                            dto.ReportingYear = If(Item.DECReportingYear = 0, year, Item.DECReportingYear)
                            dto.MonthValue = If(Item.DECMonthValue = 0, 12, Item.DECMonthValue)
                            dto.MonthDesc = If(Item.DECMonDesc = "", "DEC", Item.DECMonDesc)

                            dto.DSID = Item.DSID
                            db.SavePointsArchiveTAB3(dto)
                            db.SaveUPMPointsTAB3(dto)
                            PointsSavedAllDSMonths = True
                        End If
                End Select
            Next
        Next
        If PointsSavedAllDSMonths = True Then
            RadMessageBox.Show("<html>Points saved successfully for the <size=8><b>" + rlblMetricIDNameTab3.Text + "</b> metric.")
        End If
        PointsSavedAllDSMonths = False
    End Sub
#End Region
#Region "Save Comments"
    Private Sub rbtnDEAMSaveComments_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'this section is for saving comments
        Dim db As New ScorecardCommentsDB
        Dim dto As New CommentsDTO
        dto.ReportingYear = rcbDEAllMMonthsYear.Text 'SelectedText '_year
        dto.Month = dgvPointsByMonths.SelectedRows(0).DataBoundItem.MonthValue
        dto.Comments = Me.rtxtDEAllMonthsComments.Text
        dto.MetricID = _metric.MetricID
        dto.MetricName = _metric.MetricName
        dto.Reviewed = False
        dto.UserID = Environment.UserName
        db.SaveComments(dto)
        RadMessageBox.Show("Comments Saved")
    End Sub
    Private Sub rtbnDEADSSaveComments_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'this section is for saving comments
        Dim db As New ScorecardCommentsDB
        Dim dto As New CommentsDTO
        dto.ReportingYear = rcbDEAllDataSeriesYear.Text '.SelectedText '_year
        dto.Month = rcbDEAllDataSeriesMonth.SelectedValue
        dto.Comments = Me.rtxtDEAllDataSeriesComments.Text
        dto.MetricID = _metric.MetricID
        dto.MetricName = _metric.MetricName
        dto.Reviewed = False
        dto.UserID = Environment.UserName
        db.SaveComments(dto)
        RadMessageBox.Show("Comments Saved")
    End Sub
#End Region
End Class