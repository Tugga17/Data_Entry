Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports Entergy.PerformanceManagement.PMDB
Imports Entergy.PerformanceManagement.PMDTO
Imports System.Configuration.ConfigurationManager
Imports Entergy.PerformanceManagement.PMUtilities

Public Class PMDataEntry
    Inherits RadForm
    Private _presenter As New myPresenter
    Private _metric As PMPoint
    Private _modified As Boolean = False
    Private _year As Integer '= GetReportingYear()
    Private _selectedSeries As PMPoint.PMMetricDataSeries
    Private _selectedMetricSeriesPoints As PMPoint.PMMetricsDataSeriesPoints
    Private _dataSeries As List(Of PMMetricDataSeries)
    Private _cID As Integer 'PMPoint
    Dim _path As String = String.Empty
    Dim settings As New My.MySettings
    'Private _repyear As Integer = settings.ReportingYear
    Private _month As String
    Private _MetricDSP As PMPoint
    Private _Comments As CommentsDTO
    Public _AllComments As Dictionary(Of Integer, CommentsDTO)

    Public JANComments As String
    Public JANModified As Boolean
    Public FEBComments As String
    Public FEBModified As Boolean
    Public MARComments As String
    Public MARModified As Boolean
    Public APRComments As String
    Public APRModified As Boolean
    Public MAYComments As String
    Public MAYModified As Boolean
    Public JUNComments As String
    Public JUNModified As Boolean
    Public JULComments As String
    Public JULModified As Boolean
    Public AUGComments As String
    Public AUGModified As Boolean
    Public SEPComments As String
    Public SEPModified As Boolean
    Public OCTComments As String
    Public OCTModified As Boolean
    Public NOVComments As String
    Public NOVModified As Boolean
    Public DECComments As String
    Public DECModified As Boolean

    Private Sub PMDataEntry_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If IsNothing(_metric) Then  'If a metric has not been selected
            'Me.GetReportingYear() 'get the latest reporting year from mysettings
            'Me.PMLoadReportingYear()  'load reporting year values into combo box
            Me.SetUpMetricGrid() 'build the metric grid 
            Me.LoadMetrics() 'load metric data into the metric grid
            'LoadMonthsForDLB()
            tiAllMonths.IsSelected = True
            rtxtDEAMComments.Enabled = False
            tiAllMonths.Enabled = False
            tiAllDS.Enabled = False
            tiAllDS_Months.Enabled = False
            rpDETabs.Size = New System.Drawing.Size(55, 17)
            rpDETabs.Visible = False
            rpDEGrid.Size = New System.Drawing.Size(1185, 900)
            rgvDEMetrics.Size = New System.Drawing.Size(1185, 900)
            rbtnDESave.Visible = False
            rbtnDECancel.Visible = False

        Else
            'Me.GetReportingYear()
            Me.PMLoadReportingYear()
            LoadDataSeriesforDLB()
            rtxtDEAMComments.Enabled = False
            'LoadMonthsForDLB()
            tiAllMonths.Enabled = False
            Me.LoadMetric(_metric.MetricData.CID)
            SetUpMetricGrid()
            rpDETabs.Size = New System.Drawing.Size(1185, 600)
            rpDETabs.Location = New System.Drawing.Point(0, 0)
            rpDETabs.Visible = True
            rpDEGrid.Size = New System.Drawing.Size(0, 0)
            rpDEGrid.Visible = False
            rbtnDESave.Visible = True
            rbtnDECancel.Visible = True
            rbtnDESave.Location = New System.Drawing.Point(780, 628)
            rbtnDECancel.Location = New System.Drawing.Point(861, 628)
            rgvDEMetrics.Size = New System.Drawing.Size(1185, 301)
            tiAllMonths.IsSelected = True
            tiAllMonths.Enabled = True
            tiAllDS.Enabled = True
            tiAllDS_Months.Enabled = True
            _AllComments = GetComments()
            DetermineLevels()

        End If
    End Sub
    Public Function GetComments() As Dictionary(Of Integer, CommentsDTO)
        Dim data As New Dictionary(Of Integer, CommentsDTO)
        Dim db As New ScorecardCommentsDB
        For x As Integer = 1 To 12
            Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, x, _metric.MetricData.CID)
            If dto.Count = 1 Then
                data.Add(x, dto(0))
            Else
                Dim anything As New CommentsDTO
                anything.cID = _metric.MetricData.CID
                anything.ReportingYear = _year
                anything.Month = x
                data.Add(x, anything)
            End If

        Next
        Return data
    End Function
    Public Property Metric() As PMPoint
        Get
            Return _metric
        End Get
        Set(ByVal value As PMPoint)
            _metric = value
        End Set
    End Property
    'Public Property repYear() As Integer
    '    Get
    '        Return _repyear
    '    End Get
    '    Set(ByVal value As Integer)
    '        _repyear = value
    '    End Set
    'End Property
    Private Sub PMLoadReportingYearTab3()
        If Not IsNothing(_metric) Then
            Dim pre As New myPresenter
            Dim list As New List(Of Integer)
            Me.rcbAllDS_MonthsYear.Text = String.Empty
            For x As Integer = _year - 5 To _year + 5
                list.Add(x)
            Next
            pre.LoadCombo(Me.rcbAllDS_MonthsYear, list)
            Me.rcbAllDS_MonthsYear.SelectedText = _year

        End If
    End Sub

    Private Sub PMLoadReportingYearTab2()
        If Not IsNothing(_metric) Then
            Dim pre As New myPresenter
            Dim list As New List(Of Integer)
            Me.rcbDEADSYear.Text = String.Empty
            For x As Integer = _year - 5 To _year + 5
                list.Add(x)
            Next
            pre.LoadCombo(Me.rcbDEADSYear, list)
            Me.rcbDEADSYear.SelectedText = _year

        End If
    End Sub
    Private Sub PMLoadReportingYear()
        If Not IsNothing(_metric) Then
            Dim pre As New myPresenter
            Dim list As New List(Of Integer)
            Me.rcbDEAMYear.Text = String.Empty
            For x As Integer = _year - 5 To _year + 5
                list.Add(x)
            Next
            pre.LoadCombo(Me.rcbDEAMYear, list)
            'Me.rcbDEAMYear.SelectedText = CStr(_year)
            Me.rcbDEAMYear.SelectedText = _year
        End If
        
    End Sub
    'Private Sub GetReportingYear()
    '    Dim s As New My.MySettings
    '    _year = s.ReportingYear
    'End Sub
    
    Public Sub SetUpMetricGrid()
        rgvDEMetrics.MasterGridViewTemplate.AutoGenerateColumns = False
        rgvDEMetrics.MasterGridViewTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        rgvDEMetrics.MasterGridViewTemplate.AllowAddNewRow = False
        rgvDEMetrics.MasterGridViewTemplate.AllowEditRow = False
        rgvDEMetrics.MasterGridViewTemplate.EnableFiltering = True
        rgvDEMetrics.MasterGridViewTemplate.EnableGrouping = False
        rgvDEMetrics.EnableAlternatingRowColor = True

        rgvDEMetrics.Columns.Clear()
        Dim col As New GridViewTextBoxColumn("MetricID", "MetricID")
        col.Width = 75
        col.HeaderText = "Metric ID"
        col.AllowFiltering = True
        rgvDEMetrics.Columns.Add(col)
        Dim col2 As New GridViewTextBoxColumn("MetricName", "MetricName")
        col2.Width = 300
        col2.HeaderText = "Metric Name"
        rgvDEMetrics.Columns.Add(col2)
        Dim col3 As New GridViewTextBoxColumn("PMContactID", "PMContactID")
        col3.Width = 50
        col3.HeaderText = "Contact"
        rgvDEMetrics.Columns.Add(col3)
        Dim col4 As New GridViewCheckBoxColumn("Active", "Active")

        col4.Width = 50
        col4.HeaderText = "Active"
        rgvDEMetrics.Columns.Add(col4)

        Dim col10 As New GridViewCheckBoxColumn("IncentiveMeasure", "IncentiveMeasure")
        col10.Width = 50
        col10.HeaderText = "Incentive"
        rgvDEMetrics.Columns.Add(col10)

        Dim col8 As New GridViewCheckBoxColumn("CompositeMetric", "CompositeMetric")
        col8.Width = 60
        col8.HeaderText = "Composite"
        col8.AllowFiltering = True
        rgvDEMetrics.Columns.Add(col8)

        Dim col9 As New GridViewTextBoxColumn("ManagedByDesc", "ManagedByDesc")
        col9.Width = 75
        col9.HeaderText = "Managed By"
        col9.AllowFiltering = True
        rgvDEMetrics.Columns.Add(col9)
    End Sub
    Private Sub LoadMetrics()
        'Dim settings As New My.MySettings
        'Dim year As Integer = settings.ReportingYear
        Dim db As New PMMetricsDB
        Dim dbT As New PMTargetTrackingDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetrics()
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetric(ByVal CID As Integer)
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetrics(CID)
        _presenter.LoadGridView(rgvDEMetrics, list)
        '_metric.MetricData.
    End Sub
    Private Sub LoadGridViewByMonths(ByVal gridView As RadGridView, ByVal list As List(Of UPMPointsDTO))
        _presenter.LoadGridView(rgvDEAMPoints, list)
    End Sub
#Region "All Months TAB"
    Public Sub SetUpAllMonthsGrid()

        'If IsNothing(_selectedSeries) Then Exit Sub
        If IsNothing(_metric) Then Exit Sub

        rgvDEAMPoints.MasterGridViewTemplate.AutoGenerateColumns = False
        'rgvDEAMPoints.MasterGridViewTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        rgvDEAMPoints.MasterGridViewTemplate.AllowAddNewRow = False
        rgvDEAMPoints.MasterGridViewTemplate.AllowEditRow = True
        'rgvDEAMPoints.MasterGridViewTemplate.EnableFiltering = True
        'rgvDEAMPoints.MasterGridViewTemplate.EnableGrouping = False
        'rgvDEAMPoints.EnableAlternatingRowColor = True

        rgvDEAMPoints.Columns.Clear()
        Dim col As New GridViewTextBoxColumn("PointsDescription", "PointsDescription")
        col.Width = 75
        col.HeaderText = "Months"
        col.AllowFiltering = True
        rgvDEAMPoints.Columns.Add(col)
        Dim col2 As New GridViewTextBoxColumn("YValue", "YValue")
        col2.Width = 110
        col2.HeaderText = "Points"
        rgvDEAMPoints.Columns.Add(col2)

        Dim bind As New System.ComponentModel.BindingList(Of UPMPointsDTO)
        For Each dto As UPMPointsDTO In _selectedSeries.Points
            bind.Add(dto)
        Next
        _presenter.LoadGridView(Me.rgvDEAMPoints, bind)
        'If (rgvDEAMPoints.MasterGridViewTemplate.Rows.Count > 0) Then
        '    rgvDEAMPoints.MasterGridViewTemplate.Rows(0).IsSelected = True
        'End If


    End Sub
    Private Sub LoadDataSeriesforDLB()
        'If IsNothing(rgvDEMetrics.CurrentCell.Value) Then Exit Sub
        If Not IsNothing(_metric) Then
            'Dim db As New PMDataSeriesDB
            Dim list As New List(Of DataSeriesDTO) '= db.GetUPMDataSeries()
            For Each Item As DataSeriesDTO In _metric.GetSeriesDataList()
                list.Add(Item)
            Next

            rcbDEAMDS.DataSource = Nothing
            rcbDEAMDS.DataSource = list
            rcbDEAMDS.DisplayMember = "Description"
            rcbDEAMDS.ValueMember = "DataSeries"
            rcbDEAMDS.SelectedValue = 1
            rtxtDEAMMetric.Text = _metric.MetricData.MetricID
            rlblMetricIDNameTab1.Text = _metric.MetricData.MetricID & "--" & _metric.MetricData.MetricName

        End If
    End Sub
    'Private Sub rtxtDEAMComments_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtxtDEAMComments.TextChanged
    '    Dim commdto As New CommentsDTO
    '    If rtxtDEAMComments.Text <> "" Then
    '        Dim dto As UPMPointsDTO
    '        dto = rgvDEAMPoints.SelectedRows(0).DataBoundItem
    '        'For Each Item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
    '        'For x As Integer = 1 To 12
    '        'Dim dto As New UPMPointsDTO
    '        'dto.MonthValue = Item.MonthValue
    '        Select Case dto.MonthValue
    '            'Select Case rgvDEAMPoints.DataSource(Item).MonthValue
    '            Case 1
    '                'commdto.JANComment = Me.rtxtDEAMComments.Text
    '                JANComments = Me.rtxtDEAMComments.Text
    '                JANModified = True
    '            Case 2
    '                'commdto.FEBComment = Me.rtxtDEAMComments.Text
    '                FEBComments = Me.rtxtDEAMComments.Text
    '                FEBModified = True
    '            Case 3
    '                'commdto.MARComment = Me.rtxtDEAMComments.Text
    '                MARComments = Me.rtxtDEAMComments.Text
    '                MARModified = True
    '            Case 4
    '                'commdto.APRComment = Me.rtxtDEAMComments.Text
    '                APRComments = Me.rtxtDEAMComments.Text
    '                APRModified = True
    '            Case 5
    '                'commdto.MAYComment = Me.rtxtDEAMComments.Text
    '                MAYComments = Me.rtxtDEAMComments.Text
    '                MAYModified = True
    '            Case 6
    '                'commdto.JUNComment = Me.rtxtDEAMComments.Text
    '                JUNComments = Me.rtxtDEAMComments.Text
    '                JUNModified = True
    '            Case 7
    '                'commdto.JULCOmment = Me.rtxtDEAMComments.Text
    '                JULComments = Me.rtxtDEAMComments.Text
    '                JULModified = True
    '            Case 8
    '                'commdto.AUGComment = Me.rtxtDEAMComments.Text
    '                AUGComments = Me.rtxtDEAMComments.Text
    '                AUGModified = True
    '            Case 9
    '                'commdto.SEPComment = Me.rtxtDEAMComments.Text
    '                SEPComments = Me.rtxtDEAMComments.Text
    '                SEPModified = True
    '            Case 10
    '                'commdto.OCTComment = Me.rtxtDEAMComments.Text
    '                OCTComments = Me.rtxtDEAMComments.Text
    '                OCTModified = True
    '            Case 11
    '                'commdto.NOVComment = Me.rtxtDEAMComments.Text
    '                NOVComments = Me.rtxtDEAMComments.Text
    '                NOVModified = True
    '            Case 12
    '                'commdto.DECComment = Me.rtxtDEAMComments.Text
    '                DECComments = Me.rtxtDEAMComments.Text
    '                DECModified = True
    '        End Select
    '        'Next
    '        'Next
    '        'Else
    '        '    DECComments = ""
    '        '    JANComments = ""
    '        '    FEBComments = ""
    '        '    MAYComments = ""
    '        '    MARComments = ""
    '        '    APRComments = ""
    '        '    JUNComments = ""
    '        '    JULComments = ""
    '        '    AUGComments = ""
    '        '    SEPComments = ""
    '        '    OCTComments = ""
    '        '    NOVComments = ""
    '    End If
    '    commdto.JANComment = JANComments
    '    commdto.FEBComment = FEBComments
    '    commdto.MARComment = MARComments
    '    commdto.APRComment = APRComments
    '    commdto.MAYComment = MAYComments
    '    commdto.JUNComment = JUNComments
    '    commdto.JULCOmment = JULComments
    '    commdto.AUGComment = AUGComments
    '    commdto.SEPComment = SEPComments
    '    commdto.OCTComment = OCTComments
    '    commdto.NOVComment = NOVComments
    '    commdto.DECComment = DECComments
    'End Sub
    'Private Sub rgvDEAMPoints_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rgvDEAMPoints.Click
    '    'Dim db As New ScorecardCommentsDB
    '    If rcbDEAMYear.SelectedText = "" Then
    '        Dim db As New ScorecardCommentsDB
    '        Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue, _metric.MetricData.CID)
    '        'Me.rtxtDEAMComments.Text = ""
    '        Me.rtxtDEAMComments.Enabled = True
    '        If dto.Count = 1 Then
    '            Me.rtxtDEAMComments.Text = dto(0).Comments
    '        Else
    '            Me.rtxtDEAMComments.Text = ""
    '        End If
    '        Dim commdto As New CommentsDTO

    '        Select Case rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue
    '            Case 1
    '                rlblDEAMComments.Text = "January Comments"
    '                'commdto.JANComment = Me.rtxtDEAMComments.Text
    '            Case 2
    '                rlblDEAMComments.Text = "February Comments"
    '                'commdto.FEBComment = rtxtDEAMComments.Text
    '            Case 3
    '                rlblDEAMComments.Text = "March Comments"
    '                'commdto.MARComment = rtxtDEAMComments.Text
    '            Case 4
    '                rlblDEAMComments.Text = "April Comments"
    '            Case 5
    '                rlblDEAMComments.Text = "May Comments"
    '            Case 6
    '                rlblDEAMComments.Text = "June Comments"
    '            Case 7
    '                rlblDEAMComments.Text = "July Comments"
    '            Case 8
    '                rlblDEAMComments.Text = "August Comments"
    '            Case 9
    '                rlblDEAMComments.Text = "September Comments"
    '            Case 10
    '                rlblDEAMComments.Text = "October Comments"
    '            Case 11
    '                rlblDEAMComments.Text = "November Comments"
    '            Case 12
    '                rlblDEAMComments.Text = "December Comments"
    '        End Select
    '    Else
    '        Dim selectedyear As Integer = rcbDEAMYear.SelectedText
    '        Dim db As New ScorecardCommentsDB
    '        Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(selectedyear, rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue, _metric.MetricData.CID)
    '        Me.rtxtDEAMComments.Text = ""
    '        Me.rtxtDEAMComments.Enabled = True
    '        If dto.Count = 1 Then
    '            Me.rtxtDEAMComments.Text = dto(0).Comments
    '        End If
    '        Select Case rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue
    '            Case 1
    '                rlblDEADSComments.Text = "January Comments"
    '            Case 2
    '                rlblDEADSComments.Text = "February Comments"
    '            Case 3
    '                rlblDEADSComments.Text = "March Comments"
    '            Case 4
    '                rlblDEADSComments.Text = "April Comments"
    '            Case 5
    '                rlblDEADSComments.Text = "May Comments"
    '            Case 6
    '                rlblDEADSComments.Text = "June Comments"
    '            Case 7
    '                rlblDEADSComments.Text = "July Comments"
    '            Case 8
    '                rlblDEADSComments.Text = "August Comments"
    '            Case 9
    '                rlblDEADSComments.Text = "September Comments"
    '            Case 10
    '                rlblDEADSComments.Text = "October Comments"
    '            Case 11
    '                rlblDEADSComments.Text = "November Comments"
    '            Case 12
    '                rlblDEADSComments.Text = "December Comments"
    '        End Select
    '    End If
    'End Sub
    Private Sub DataEntryAMTab(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles rmiEditM.Click
        If rgvDEMetrics.SelectedRows.Count = 0 Or rgvDEMetrics.SelectedRows.Count > 1 Then Exit Sub
        rtxtDEAMComments.Enabled = False
        Dim MetricPoints As New PMPoint
        Dim MetricDS As New PMPoint
        Me.PMLoadReportingYear()
        MetricPoints.GetMetric(Me.GetSelectedMetric, rcbDEAMYear.Text)


    End Sub
    Private Function GetSelectedMetric() As MetricDTO
        Dim dto As MetricDTO
        If rgvDEMetrics.SelectedRows.Count = 1 Then
            dto = rgvDEMetrics.SelectedRows(0).DataBoundItem
            '_AllComments = GetComments()
        Else
            dto = Nothing
        End If
        Return dto
    End Function
    Private Sub GetPointsDEAM()
        If Not IsNothing(_selectedSeries) Then
            _selectedSeries.Points.Clear()
            For Each item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
                item.ChangedBy = Environment.UserName
                _selectedSeries.Points.Add(item)
            Next
        End If
    End Sub
    Private Sub rgvDEAMPoints_DataError(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GridViewDataErrorEventArgs) Handles rgvDEAMPoints.DataError
        If e.Exception.Message = "Object of type 'System.String' cannot be converted to type 'System.Nullable`1[System.Decimal]'." Then
            e.Cancel = True
        End If
    End Sub
    Private Sub rgvDEAMPoints_ValueChanging(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.ValueChangingEventArgs) Handles rgvDEAMPoints.ValueChanging
        If Not IsNothing(rgvDEAMPoints.CurrentRow) Then
            If Not IsNothing(rgvDEAMPoints.CurrentRow.DataBoundItem) Then
                'e.OldValue <> e.NewValue And 
                If CType(rgvDEAMPoints.CurrentRow.DataBoundItem, UPMPointsDTO).ModificationType = IModificationIndicator.ModificationTypes.NotModified Then
                    CType(rgvDEAMPoints.CurrentRow.DataBoundItem, UPMPointsDTO).ModificationType = IModificationIndicator.ModificationTypes.Edited
                End If
            End If
        End If
    End Sub
    Private Sub rcbDEAMDS_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEAMDS.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEAMDS.SelectedValue) Then
            'load points from dataseries that just lost focus.
            Me.GetPointsDEAM()
            _selectedSeries = _metric.GetDataSeries(Me.rcbDEAMDS.SelectedValue)

            'If _selectedSeries.Points.Count = 0 Then
            '    AddMonths()
            'End If
            SetUpAllMonthsGrid()
            DetermineLevels()
            'Me.rtxtDEAMComments.Text = ""
        End If
    End Sub
#End Region
#Region "Events"
    Private Sub rgvDEMetrics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rgvDEMetrics.Click
        If IsNothing(rgvDEMetrics.CurrentCell.Value) Then Exit Sub
        'Me.LoadMonthsForDLB()
        'rgvDEMetrics.Enabled = False
        DataEntryAMTab(sender, e)
        'DataEntryGo2(sender, e)
        Me.SetUpAllMonthsGrid()
        'Me.SetUpAllDataSeriesGrid()


    End Sub
    Private Sub rtsDataEntry_TabSelected(ByVal sender As System.Object, ByVal args As Telerik.WinControls.UI.TabEventArgs) Handles rtsDataEntry.TabSelected
        If args.TabItem.Name = "tiAllMonths" Then
            'Me.LoadDataSeriesforDLB()
        End If
        If args.TabItem.Name = "tiAllDS" Then

        End If
    End Sub
#End Region
    'Sub rgvDEMetrics_RowFormatting(ByVal sender As Object, ByVal e As RowFormattingEventArgs)
    '    If DirectCast(e.RowElement.Tag, Boolean) = True Then
    '        e.RowElement.BackColor = Color.Red
    '        e.RowElement.DrawFill = True
    '    Else
    '        e.RowElement.ResetValue(VisualElement.BackColorProperty)
    '    End If
    '    For Each row As GridViewRowInfo In Me.rgvDEMetrics.Rows
    '        row.IsSelected = True
    '    Next
    'End Sub
#Region "Universal"
    'Private Sub AddMonths()
    '    Me.AddMonths(_selectedSeries.DataSeries.Description)
    'End Sub
    'Private Sub AddMonths(ByVal dsDescription As String)
    '    Dim months() As String = {"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"}
    '    Me.AddRows(months, dsDescription)
    'End Sub
    'Private Sub AddRows(ByVal values As String(), ByVal dsDescription As String)
    '    Dim curPoints As New System.ComponentModel.BindingList(Of UPMPointsDTO)
    '    If Me.rgvDEAMPoints.Rows.Count > 0 Then
    '        curPoints = Me.rgvDEAMPoints.DataSource
    '        If MessageBox.Show("To continute the existing points will be deleted. Do you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
    '            MessageBox.Show("You can not add any points if there are currently any points for the data series. Please delete all points first")
    '            Exit Sub
    '        End If
    '    End If
    '    Dim point As UPMPointsDTO
    '    Dim row As Telerik.WinControls.UI.GridViewDataRowInfo
    '    Dim x As Integer = 1
    '    Dim bind As New System.ComponentModel.BindingList(Of UPMPointsDTO)

    '    For Each desc As String In values
    '        point = New UPMPointsDTO
    '        point.PointsDescription = desc
    '        point.DataSeriesDescription = dsDescription
    '        point.C_ID = _metric.MetricData.CID
    '        point.MetricID = _metric.MetricData.MetricID
    '        point.MonthValue = x
    '        point.ReportingYear = My.Settings.ReportingYear
    '        point.ModificationType = IModificationIndicator.ModificationTypes.Added
    '        'TransferPointValue(point, curPoints)
    '        row = New Telerik.WinControls.UI.GridViewDataRowInfo(point, Me.rgvDEAMPoints.MasterGridViewInfo)
    '        bind.Add(point)
    '        x = x + 1
    '    Next
    '    Me.rgvDEAMPoints.DataSource = bind
    'End Sub
    'Private Function GetPath() As String
    '    Dim dia As New FolderBrowserDialog
    '    If _path > String.Empty Then
    '        dia.SelectedPath = _path
    '    End If
    '    dia.ShowDialog()
    '    _path = dia.SelectedPath
    '    Return _path
    'End Function
#End Region
#Region "Save Points"
    Private Sub rbtnDESave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDESave.Click
        If tiAllMonths.IsSelected Then
            SaveAllMonthTABData()
        ElseIf tiAllDS.IsSelected Then
            SaveAllDSTABData()
        Else
            SaveAllDSMonthTABData()
        End If
    End Sub
    Public Sub SaveAllMonthTABData()
        If Me.rtxtDEAMMetric.Text = "" Or Me.rtxtDEAMMetric.Text = String.Empty Then
            RadMessageBox.Show("The Metric ID is required")
            Exit Sub
        End If
        If Me.rcbDEAMYear.Text = "" Or Me.rcbDEAMYear.Text = String.Empty Then
            RadMessageBox.Show("The reporting Year is required")
            Exit Sub
        End If

        _metric.MetricData.MetricID = rtxtDEAMMetric.Text
        '_MetricDSP.RepYearData.ReportingYear = rcbDEAMYear.Text

        Dim descs As New List(Of String)
        For Each series As DataSeriesDTO In _metric.GetSeriesDataList
            If descs.Contains(series.Description) Then
                RadMessageBox.Show("The data series: " + series.Description + " has been used twice. Please fix.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error)
                Exit Sub
            Else
                descs.Add(series.Description)
            End If
        Next
        Dim sorts As New List(Of Integer)
        For Each series As DataSeriesDTO In _metric.GetSeriesDataList
            If sorts.Contains(series.DataSeries) Then
                RadMessageBox.Show("The data series sort: " + series.DataSeries + " has been used twice. Please fix.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error)
                Exit Sub
            Else
                sorts.Add(series.DataSeries)
            End If
        Next
        Dim db As New PMPointsDB
        'Dim comdb As New ScorecardCommentsDB
        For Each item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
            If item.ModificationType = IModificationIndicator.ModificationTypes.Edited And Not IsDBNull(item.NullableYValue) Then
                db.SaveUPMPoints(item)
                db.SavePointsArchive(item)

            End If
        Next
        Dim commdto As New CommentsDTO
        For Each Item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
            'this section is for saving comments
            Dim comdb As New ScorecardCommentsDB
            Dim dto As New CommentsDTO
            dto.ReportingYear = rcbDEAMYear.Text 'SelectedText '_year
            'dto.Month = rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue
            'dto.Comments = Me.rtxtDEAMComments.Text
            dto.MetricID = _metric.MetricData.MetricID
            dto.MetricName = _metric.MetricData.MetricName
            dto.Reviewed = False
            dto.UserID = Environment.UserName
            Select Case Item.PointsDescription
                Case "JAN"
                    dto.Month = 1
                    dto.Comments = JANComments.ToString
                    If JANModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "FEB"
                    dto.Month = 2
                    dto.Comments = FEBComments
                    If FEBModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "MAR"
                    dto.Month = 3
                    dto.Comments = MARComments
                    If MARModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "APR"
                    dto.Month = 4
                    dto.Comments = APRComments
                    If APRModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "MAY"
                    dto.Month = 5
                    dto.Comments = MAYComments
                    If MAYModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "JUN"
                    dto.Month = 6
                    dto.Comments = JUNComments
                    If JUNModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "JUL"
                    dto.Month = 7
                    dto.Comments = JULComments
                    If JULModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "AUG"
                    dto.Month = 8
                    dto.Comments = AUGComments
                    If AUGModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "SEP"
                    dto.Month = 9
                    dto.Comments = SEPComments
                    If SEPModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "OCT"
                    dto.Month = 10
                    dto.Comments = OCTComments
                    If OCTModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "NOV"
                    dto.Month = 11
                    dto.Comments = NOVComments
                    If NOVModified = True Then
                        comdb.SaveComments(dto)
                    End If
                Case "DEC"
                    dto.Month = 12
                    dto.Comments = DECComments
                    If DECModified = True Then
                        comdb.SaveComments(dto)
                    End If
            End Select
        Next
    End Sub
    Public Sub SaveAllDSTABData()
        If Me.rtxtDEADSMetric.Text = "" Or Me.rtxtDEADSMetric.Text = String.Empty Then
            RadMessageBox.Show("The Metric ID is required")
            Exit Sub
        End If
        If Me.rcbDEADSYear.Text = "" Or Me.rcbDEADSYear.Text = String.Empty Then
            RadMessageBox.Show("The reporting Year is required")
            Exit Sub
        End If

        _metric.MetricData.MetricID = rtxtDEADSMetric.Text
        '_MetricDSP.RepYearData.ReportingYear = rcbDEAMYear.Text
        Dim descs As New List(Of String)
        For Each series As DataSeriesDTO In _metric.GetSeriesDataList
            If descs.Contains(series.Description) Then
                RadMessageBox.Show("The data series: " + series.Description + " has been used twice. Please fix.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error)
                Exit Sub
            Else
                descs.Add(series.Description)
            End If
        Next
        
        Dim sorts As New List(Of Integer)
        For Each month As MonthsDTO In _metric.GetMonthDataList
            If sorts.Contains(month.MonthVal) Then
                RadMessageBox.Show("The month sort: " + month.MonthVal + " has been used twice. Please fix.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error)
                Exit Sub
            Else
                sorts.Add(month.MonthVal)
            End If
        Next
        Dim db As New PMPointsDB
        For Each Item As UPMPointsDTO In Me.rgvDEADSPoints.DataSource
            If Item.ModificationType = IModificationIndicator.ModificationTypes.Edited And Not IsDBNull(Item.NullableYValue) Then
                db.SaveUPMPointsTAB2(Item)
                db.SavePointsArchiveTAB2(Item)
            End If
        Next
    End Sub
    Public Sub SaveAllDSMonthTABData()
        If Me.rtxtAllDSMonthsMetric.Text = "" Or Me.rtxtAllDSMonthsMetric.Text = String.Empty Then
            RadMessageBox.Show("The Metric ID is required")
            Exit Sub
        End If
        If Me.rcbAllDS_MonthsYear.Text = "" Or Me.rcbAllDS_MonthsYear.Text = String.Empty Then
            RadMessageBox.Show("The reporting Year is required")
            Exit Sub
        End If

        _metric.MetricData.MetricID = rtxtAllDSMonthsMetric.Text
        '_MetricDSP.RepYearData.ReportingYear = rcbDEAMYear.Text
        Dim descs As New List(Of String)
        For Each series As DataSeriesDTO In _metric.GetSeriesDataList
            If descs.Contains(series.Description) Then
                RadMessageBox.Show("The data series: " + series.Description + " has been used twice. Please fix.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error)
                Exit Sub
            Else
                descs.Add(series.Description)
            End If
        Next
        Dim sorts As New List(Of Integer)
        For Each series As DataSeriesDTO In _metric.GetSeriesDataList
            If sorts.Contains(series.DataSeries) Then
                RadMessageBox.Show("The data series sort: " + series.DataSeries + " has been used twice. Please fix.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error)
                Exit Sub
            Else
                sorts.Add(series.DataSeries)
            End If
        Next
        Dim db As New PMPointsDB
        For Each Item As UPMPointsDTO In Me.rgvDEADSMPoints.DataSource
            'If Item.ModificationType = IModificationIndicator.ModificationTypes.Edited Then 'And Not IsDBNull(Item.NullableYValue) Then
            For x As Integer = 1 To 12
                Dim dto As New UPMPointsDTO
                dto.C_ID = Item.CID
                Select Case x
                    Case 1
                        If Item._JANModified = True And Not IsDBNull(Item.NullableJANYValue) Then
                            dto.YValue = Item.JANPoints
                            dto.ReportingYear = Item.JANReportingYear
                            dto.MonthValue = Item.JANMonthValue
                            dto.MonthDesc = Item.JANMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case (2)
                        If Item._FEBModified = True And Not IsDBNull(Item.NullableFEBYValue) Then
                            dto.YValue = Item.FEBPoints
                            dto.ReportingYear = Item.FEBReportingYear
                            dto.MonthValue = Item.FEBMonthValue
                            dto.MonthDesc = Item.FEBMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 3
                        If Item._MARModified = True And Not IsDBNull(Item.NullableMARYValue) Then
                            dto.YValue = Item.MARPoints
                            dto.ReportingYear = Item.MARReportingYear
                            dto.MonthValue = Item.MARMonthValue
                            dto.MonthDesc = Item.MARMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 4
                        If Item._APRModified = True And Not IsDBNull(Item.NullableAPRYValue) Then
                            dto.YValue = Item.APRPoints
                            dto.ReportingYear = Item.APRReportingYear
                            dto.MonthValue = Item.APRMonthValue
                            dto.MonthDesc = Item.APRMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 5
                        If Item._MAYModified = True And Not IsDBNull(Item.NullableMAYYValue) Then
                            dto.YValue = Item.MAYPoints
                            dto.ReportingYear = Item.MayReportingYear
                            dto.MonthValue = Item.MayMonthValue
                            dto.MonthDesc = Item.MayMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 6
                        If Item._JUNModified = True And Not IsDBNull(Item.NullableJUNYValue) Then
                            dto.YValue = Item.JUNPoints
                            dto.ReportingYear = Item.JunReportingYear
                            dto.MonthValue = Item.JunMonthValue
                            dto.MonthDesc = Item.JunMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 7
                        If Item._JULModified = True And Not IsDBNull(Item.NullableJULYValue) Then
                            dto.YValue = Item.JULPoints
                            dto.ReportingYear = Item.JULReportingYear
                            dto.MonthValue = Item.JULMonthValue
                            dto.MonthDesc = Item.JulMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 8
                        If Item._AUGModified = True And Not IsDBNull(Item.NullableAUGYValue) Then
                            dto.YValue = Item.AUGPoints
                            dto.ReportingYear = Item.AUGReportingYear
                            dto.MonthValue = Item.AUGMonthValue
                            dto.MonthDesc = Item.AUGMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 9
                        If Item._SEPModified = True And Not IsDBNull(Item.NullableSEPYValue) Then
                            dto.YValue = Item.SEPPoints
                            dto.ReportingYear = Item.SEPReportingYear
                            dto.MonthValue = Item.SEPMonthValue
                            dto.MonthDesc = Item.SEPMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 10
                        If Item._OCTModified = True And Not IsDBNull(Item.NullableOCTYValue) Then
                            dto.YValue = Item.OCTPoints
                            dto.ReportingYear = Item.OCTReportingYear
                            dto.MonthValue = Item.OCTMonthValue
                            dto.MonthDesc = Item.OCTMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 11
                        If Item._NOVModified = True And Not IsDBNull(Item.NullableNOVYValue) Then
                            dto.YValue = Item.NOVPoints
                            dto.ReportingYear = Item.NOVReportingYear
                            dto.MonthValue = Item.NOVMonthValue
                            dto.MonthDesc = Item.NOVMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                    Case 12
                        If Item._DECModified = True And Not IsDBNull(Item.NullableDECYValue) Then
                            dto.YValue = Item.DECPoints
                            dto.ReportingYear = Item.DECReportingYear
                            dto.MonthValue = Item.DECMonthValue
                            dto.MonthDesc = Item.DECMonDesc

                            dto.DSID = Item.DSID
                            db.SaveUPMPointsTAB3(dto)
                        End If
                End Select
                'dto.ReportingYear = Item.ReportingYear
                'dto.DSID = Item.DSID
                'dto.PointsDescription = Item.MonthDesc
                'dto.MonthValue = Item.MonthValue
                'dto.UserID = Item.UserID
                'db.SaveUPMPointsTAB3(dto)
                'db.SavePointsArchiveTAB3(dto)
            Next


            'End If

        Next
    End Sub

#End Region
    Private Sub rbtnDEAMCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDECancel.Click

    End Sub
#Region "Save Comments"
    Private Sub rbtnDEAMSaveComments_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDEAMSaveComments.Click
        'this section is for saving comments
        Dim db As New ScorecardCommentsDB
        Dim dto As New CommentsDTO
        dto.ReportingYear = rcbDEAMYear.Text 'SelectedText '_year
        dto.Month = rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue
        dto.Comments = Me.rtxtDEAMComments.Text
        dto.MetricID = _metric.MetricData.MetricID
        dto.MetricName = _metric.MetricData.MetricName
        dto.Reviewed = False
        dto.UserID = Environment.UserName
        db.SaveComments(dto)
        RadMessageBox.Show("Comments Saved")
    End Sub
    Private Sub rtbnDEADSSaveComments_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtbnDEADSSaveComments.Click
        'this section is for saving comments
        Dim db As New ScorecardCommentsDB
        Dim dto As New CommentsDTO
        dto.ReportingYear = rcbDEADSYear.Text '.SelectedText '_year
        dto.Month = rcbDEADSMonth.SelectedValue
        dto.Comments = Me.rtxtDEADSComments.Text
        dto.MetricID = _metric.MetricData.MetricID
        dto.MetricName = _metric.MetricData.MetricName
        dto.Reviewed = False
        dto.UserID = Environment.UserName
        db.SaveComments(dto)
        RadMessageBox.Show("Comments Saved")
    End Sub
#End Region
#Region "All Data-Series TAB"
    Private Sub tiAllDS_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tiAllDS.Click
        'tiAllDS.IsSelected = True
        LoadMonthsForDLB()
        'PMLoadReportingYearTab2()
        'DeterminePerformanceLevelsTAB2()
        'PMLoadReportingYearTab2()
        'Me.SetUpAllDataSeriesGrid()
        'DataEntryTABbyDS(sender, e)
    End Sub
    Private Sub LoadGridView(ByVal gridView As RadGridView, ByVal list As List(Of UPMPointsDTO))
        _presenter.LoadGridView(rgvDEADSPoints, list)
    End Sub

    Private Sub DataEntryTABbyDS(ByVal sender As Object, ByVal e As System.EventArgs)
        If rgvDEMetrics.SelectedRows.Count = 0 Or rgvDEMetrics.SelectedRows.Count > 1 Then Exit Sub
        Dim dsdto As MetricDTO
        If rgvDEMetrics.SelectedRows.Count = 1 Then
            dsdto = rgvDEMetrics.SelectedRows(0).DataBoundItem
        Else
            dsdto = Nothing
        End If
        Me.rtxtDEADSComments.Enabled = False
        Dim db As New PMDataSeriesDB
        Dim s As New My.MySettings
        If Me.rcbDEADSYear.SelectedText = "" Then
            Dim resultset As List(Of UPMPointsDTO) = db.GetUPMDataSeriesByMonth(dsdto.CID, _year, rcbDEADSMonth.SelectedValue)
            If resultset.Count = 0 Then
                Dim DSset As List(Of DataSeriesDTO) = db.GetUPMDataSeries(dsdto.CID)
                LoadGridView(rgvDEADSPoints, resultset)

            Else
                LoadGridView(rgvDEADSPoints, resultset)
            End If

        Else
            Dim selectedYear = rcbDEADSYear.SelectedText
            Dim resultset As List(Of UPMPointsDTO) = db.GetUPMDataSeriesByMonth(dsdto.CID, selectedYear, rcbDEADSMonth.SelectedValue)
            If resultset.Count = 0 Then
                Dim DSset As List(Of DataSeriesDTO) = db.GetUPMDataSeries(dsdto.CID)
                LoadGridView(rgvDEADSPoints, resultset)
            Else
                LoadGridView(rgvDEADSPoints, resultset)
            End If
        End If

    End Sub
    Public Function GetSelectedMetricTab2() As MetricDTO
        Dim dsdto As MetricDTO
        If rgvDEMetrics.SelectedRows.Count = 1 Then
            dsdto = rgvDEMetrics.SelectedRows(0).DataBoundItem
        Else
            dsdto = Nothing
        End If
        Return dsdto
    End Function
    Private Sub LoadMonth()
        Dim p As New myPresenter
        p.LoadMonthComboNum(Me.rcbDEADSMonth)
        _month = String.Format("{0:MMM}", Now.AddMonths(-1))
        rcbDEADSMonth.SelectedText = _month
    End Sub
    Public Sub LoadMonthsForDLB()
        If Not IsNothing(_metric) Then
            Dim db As New PMMonthsDB
            'Dim ds As PMLoadMonths
            Dim List As List(Of MonthsDTO)
            _metric = Metric
            List = db.GetUPMMonths()

            rcbDEADSMonth.DataSource = Nothing
            rcbDEADSMonth.DataSource = List
            rcbDEADSMonth.DisplayMember = "MonthDescription"
            rcbDEADSMonth.ValueMember = "MonthVal"
            'rcbDEADSMonth.SelectedValue = 1

            rtxtDEADSMetric.Text = _metric.MetricData.MetricID
            rlblMetricIDNameTab2.Text = _metric.MetricData.MetricID & "--" & _metric.MetricData.MetricName
        End If
    End Sub
    Private Sub rcbDEADSMonth_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEADSMonth.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEADSMonth.SelectedValue) Then
            'LoadMonthsForDLB()
            If rcbDEADSYear.SelectedText = "" Then
                PMLoadReportingYearTab2()
            End If

            Me.SetUpAllDataSeriesGrid()
            DataEntryTABbyDS(sender, e)
            DetermineLevelsTAB2()
            'GetCommentsTAB2()
            Select Case Me.rcbDEADSMonth.SelectedValue
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
    Private Sub GetPointsDEADS()
        If Not IsNothing(_selectedMetricSeriesPoints) Then
            _selectedMetricSeriesPoints.Points.Clear()
            For Each Item As UPMPointsDTO In Me.rgvDEADSPoints.DataSource
                Item.ChangedBy = Environment.UserName
                _selectedMetricSeriesPoints.Points.Add(Item)
            Next
        End If
    End Sub
    Private Sub rcbDEADSYear_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEADSYear.SelectedIndexChanged
        If Not IsNothing(Me.rcbDEADSYear.SelectedText) Then
            '_metric.RefreshDSPointsByYear(rcbDEADSYear)
            'RefreshTAB2GridbyYear(rcbDEADSYear.SelectedValue)
            Me.SetUpAllDataSeriesGrid()
            DataEntryTABbyDS(sender, e)
            DetermineLevelsTAB2()
            If rcbDEADSYear.SelectedText = "" Then
                Dim db As New ScorecardCommentsDB
                Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, rgvDEADSPoints.SelectedRows(0).DataBoundItem.Month, _metric.MetricData.CID)
                Me.rtxtDEADSComments.Text = ""
                If dto.Count = 1 Then
                    Me.rtxtDEADSComments.Text = dto(0).Comments
                End If
            Else
                Dim selectedYear As Integer = rcbDEADSYear.Text '.SelectedText
                Dim db As New ScorecardCommentsDB
                Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(selectedYear, rgvDEADSPoints.SelectedRows(0).DataBoundItem.Month, _metric.MetricData.CID)
                Me.rtxtDEADSComments.Text = ""
                If dto.Count = 1 Then
                    Me.rtxtDEADSComments.Text = dto(0).Comments
                End If
            End If
        End If
    End Sub
    Public Sub RefreshTAB2GridbyYear(ByVal grid As Object)
        Dim s As New My.MySettings
        Dim year As Integer = grid.SelectedText
        Me.SetUpAllDataSeriesGrid()
        'DataEntryTABbyDS(sender, e)
    End Sub
    Public Sub SetUpAllDataSeriesGrid()
        If IsNothing(_selectedSeries) Then Exit Sub

        rgvDEADSPoints.MasterGridViewTemplate.AutoGenerateColumns = False
        'rgvDEAMPoints.MasterGridViewTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        rgvDEADSPoints.MasterGridViewTemplate.AllowAddNewRow = False
        rgvDEADSPoints.MasterGridViewTemplate.AllowEditRow = True
        'rgvDEAMPoints.MasterGridViewTemplate.EnableFiltering = True
        'rgvDEAMPoints.MasterGridViewTemplate.EnableGrouping = False
        rgvDEADSPoints.EnableAlternatingRowColor = True

        rgvDEADSPoints.Columns.Clear()

        Dim col As New GridViewTextBoxColumn("dsDescription", "dsDescription")
        col.Width = 150
        col.HeaderText = "Data Series"
        col.AllowFiltering = True
        rgvDEADSPoints.Columns.Add(col)
        Dim col2 As New GridViewTextBoxColumn("YValue", "YValue")
        col2.Width = 110
        col2.HeaderText = "Points"
        rgvDEADSPoints.Columns.Add(col2)
    End Sub
    Public Sub GetCommentsTAB2()
        Dim db As New ScorecardCommentsDB
        Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, rcbDEADSMonth.SelectedValue, _metric.MetricData.CID)
        Me.rtxtDEADSComments.Text = ""
        If dto.Count = 1 Then
            Me.rtxtDEADSComments.Text = dto(0).Comments
        Else
            Me.rtxtDEADSComments.Text = ""
        End If
    End Sub
    Private Sub rgvDEADSPoints_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rgvDEADSPoints.Click
        Dim db As New ScorecardCommentsDB
        Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, rgvDEADSPoints.SelectedRows(0).DataBoundItem.Month, _metric.MetricData.CID)
        Me.rtxtDEADSComments.Text = ""
        Me.rtxtDEADSComments.Enabled = True
        If dto.Count = 1 Then
            Me.rtxtDEADSComments.Text = dto(0).Comments
        End If
    End Sub
    Private Sub rcbDEAMYear_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbDEAMYear.SelectedIndexChanged
        _metric.RefreshDSPointsByYear(rcbDEAMYear)
        Me.SetUpAllMonthsGrid()
        DetermineLevels()
        Dim selectedYear As Integer = rcbDEAMYear.Text 'SelectedText
        Dim db As New ScorecardCommentsDB
        Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(selectedYear, rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue, _metric.MetricData.CID)
        Me.rtxtDEAMComments.Text = ""
        If dto.Count = 1 Then
            Me.rtxtDEAMComments.Text = dto(0).Comments
        End If
    End Sub
   
    'Private Sub rgvDEAMPoints_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rgvDEAMPoints.SelectionChanged
    '    If rcbDEAMYear.SelectedText = "" Then
    '        Dim db As New ScorecardCommentsDB
    '        Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue, _metric.MetricData.CID)
    '        Me.rtxtDEAMComments.Enabled = True

    '        If dto.Count = 1 Then
    '            Me.rtxtDEAMComments.Text = dto(0).Comments
    '            Select Case dto(0).Month
    '                Case 1
    '                    dto(0).JANComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "January Comments"
    '                Case 2
    '                    dto(0).FEBComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "February Comments"
    '                Case 3
    '                    dto(0).MARComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "March Comments"
    '                Case 4
    '                    dto(0).APRComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "April Comments"
    '                Case 5
    '                    dto(0).MAYComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "MAY Comments"
    '                Case 6
    '                    dto(0).JUNComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "June Comments"
    '                Case 7
    '                    dto(0).JULCOmment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "July Comments"
    '                Case 8
    '                    dto(0).AUGComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "Auguest Comments"
    '                Case 9
    '                    dto(0).SEPComment = Me.rtxtDEAMComments.Text
    '                    rlblDEAMComments.Text = "September Comments"
    '            End Select
    '        Else
    '            Me.rtxtDEAMComments.Text = ""
    '        End If
    '    Else
    '        Dim selectedyear As Integer = rcbDEAMYear.SelectedText
    '        Dim db As New ScorecardCommentsDB
    '        Dim dto As List(Of CommentsDTO) = db.GetCommentsByMetricID(selectedyear, rgvDEAMPoints.SelectedRows(0).DataBoundItem.MonthValue, _metric.MetricData.MetricID)
    '        Me.rtxtDEAMComments.Text = ""
    '        If dto.Count = 1 Then
    '            Me.rtxtDEAMComments.Text = dto(0).Comments
    '            Select Case _month
    '                Case 1
    '                    dto(0).JANComment = Me.rtxtDEAMComments.Text
    '                Case 2
    '                    dto(0).FEBComment = Me.rtxtDEAMComments.Text
    '                Case 3
    '                    dto(0).MARComment = Me.rtxtDEAMComments.Text
    '            End Select
    '        End If
    '    End If

    'End Sub
#End Region
#Region "All Months Across & Data Series Down"
    Private Sub tiAllDS_Months_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tiAllDS_Months.Click
        Me.SetUpAllMonthsDSGrid()
        PMLoadReportingYearTab3()
        DataEntryTABbyDSMonths(sender, e)
    End Sub
    Private Sub LoadGridViewTAB3(ByVal gridView As RadGridView, ByVal list As List(Of UPMPointsDTO))
        _presenter.LoadGridView(rgvDEADSMPoints, list)
    End Sub
    Private Sub DataEntryTABbyDSMonths(ByVal sender As Object, ByVal e As System.EventArgs)
        If rgvDEMetrics.SelectedRows.Count = 0 Or rgvDEMetrics.SelectedRows.Count > 1 Then Exit Sub
        Dim dsdto As MetricDTO
        If rgvDEMetrics.SelectedRows.Count = 1 Then
            dsdto = rgvDEMetrics.SelectedRows(0).DataBoundItem
        Else
            dsdto = Nothing
        End If

        Dim db As New PMDataSeriesDB
        Dim s As New My.MySettings
        If rcbAllDS_MonthsYear.SelectedText = "" Then
            Dim resultset As List(Of UPMPointsDTO) = db.GetUPMDataSeriesPointsByMonths(dsdto.CID, _year)
            For Each Item As UPMPointsDTO In resultset
                Item.InitializeBuffers()
            Next
            LoadGridViewTAB3(rgvDEADSMPoints, resultset)
        Else
            Dim selectedyear As Integer = rcbAllDS_MonthsYear.SelectedText
            Dim resultset As List(Of UPMPointsDTO) = db.GetUPMDataSeriesPointsByMonths(dsdto.CID, selectedyear)
            For Each Item As UPMPointsDTO In resultset
                Item.InitializeBuffers()
            Next
            LoadGridViewTAB3(rgvDEADSMPoints, resultset)
        End If
        rtxtAllDSMonthsMetric.Text = _metric.MetricData.MetricID
        rlblMetricIDNameTab3.Text = _metric.MetricData.MetricID & "--" & _metric.MetricData.MetricName
    End Sub
    Public Sub SetUpAllMonthsDSGrid()
        If IsNothing(_metric) Then Exit Sub

        rgvDEADSMPoints.MasterGridViewTemplate.AutoGenerateColumns = False
        rgvDEADSMPoints.MasterGridViewTemplate.AllowAddNewRow = False
        rgvDEADSMPoints.MasterGridViewTemplate.AllowEditRow = True
        rgvDEADSMPoints.EnableAlternatingRowColor = True
        rgvDEADSMPoints.GridElement.BorderWidth = 3
        rgvDEADSMPoints.GridElement.BorderColor = Color.SlateBlue
        rgvDEADSMPoints.Columns.Clear()

        Dim col1 As New GridViewTextBoxColumn("dsDescription", "dsDescription")
        col1.Width = 150
        col1.HeaderText = "Data Series"
        col1.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col1)
        Dim col2 As New GridViewTextBoxColumn("JANPoints", "JANPoints")
        col2.Width = 85
        col2.HeaderText = "JAN"
        col2.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col2)
        Dim col3 As New GridViewTextBoxColumn("FEBPoints", "FEBPoints")
        col3.Width = 85
        col3.HeaderText = "FEB"
        col3.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col3)
        Dim col4 As New GridViewTextBoxColumn("MARPoints", "MARPoints")
        col4.Width = 85
        col4.HeaderText = "MAR"
        col4.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col4)
        Dim col5 As New GridViewTextBoxColumn("APRPoints", "APRPoints")
        col5.Width = 85
        col5.HeaderText = "APR"
        col5.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col5)
        Dim col6 As New GridViewTextBoxColumn("MAYPoints", "MAYPoints")
        col6.Width = 85
        col6.HeaderText = "MAY"
        col6.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col6)
        Dim col7 As New GridViewTextBoxColumn("JUNPoints", "JUNPoints")
        col7.Width = 85
        col7.HeaderText = "JUN"
        col7.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col7)
        Dim col8 As New GridViewTextBoxColumn("JULPoints", "JULPoints")
        col8.Width = 85
        col8.HeaderText = "JUL"
        col8.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col8)
        Dim col9 As New GridViewTextBoxColumn("AUGPoints", "AUGPoints")
        col9.Width = 85
        col9.HeaderText = "AUG"
        col9.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col9)
        Dim col10 As New GridViewTextBoxColumn("SEPPoints", "SEPPoints")
        col10.Width = 85
        col10.HeaderText = "SEP"
        col10.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col10)
        Dim col11 As New GridViewTextBoxColumn("OCTPoints", "OCTPoints")
        col11.Width = 85
        col11.HeaderText = "OCT"
        col11.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col11)
        Dim col12 As New GridViewTextBoxColumn("NOVPoints", "NOVPoints")
        col12.Width = 85
        col12.HeaderText = "NOV"
        col12.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col12)
        Dim col13 As New GridViewTextBoxColumn("DECPoints", "DECPoints")
        col13.Width = 85
        col13.HeaderText = "DEC"
        col13.AllowFiltering = True
        rgvDEADSMPoints.Columns.Add(col13)
    End Sub

    Private Sub rcbAllDS_MonthsYear_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rcbAllDS_MonthsYear.SelectedIndexChanged
        If rgvDEMetrics.SelectedRows.Count = 0 Or rgvDEMetrics.SelectedRows.Count > 1 Then Exit Sub
        Dim dsdto As MetricDTO
        If rgvDEMetrics.SelectedRows.Count = 1 Then
            dsdto = rgvDEMetrics.SelectedRows(0).DataBoundItem
        Else
            dsdto = Nothing
        End If

        Dim db As New PMDataSeriesDB
        Dim s As New My.MySettings
        If rcbAllDS_MonthsYear.SelectedText = "" Then
            Dim resultset = db.GetUPMDataSeriesPointsByMonths(dsdto.CID, _year)
            _presenter.LoadGridView(rgvDEADSMPoints, resultset)
        Else
            Dim AllDSMonths_SelectedYear As Integer = rcbAllDS_MonthsYear.SelectedText
            Dim resultset = db.GetUPMDataSeriesPointsByMonths(dsdto.CID, AllDSMonths_SelectedYear)
            _presenter.LoadGridView(rgvDEADSMPoints, resultset)
        End If

    End Sub
#End Region

    Private Sub rgvDEADSMPoints_CellEndEdit(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles rgvDEADSMPoints.CellEndEdit
        'Dim dto As New UPMPointsDTO
        'Dim list As New List(Of UPMPointsDTO)

        'If dto.JANModificationType = IModificationIndicator.ModificationTypes.Edited Then
        '    dto.SetJANBufferEdited()
        'End If
        'If dto.FEBModificationType = IModificationIndicator.ModificationTypes.Edited Then
        '    dto.SetBufferModified()
        'End If

    End Sub

    Private Sub rgvDEADSMPoints_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgvDEADSMPoints.SelectionChanged

    End Sub

    Private Sub rgvDEADSPoints_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgvDEADSPoints.SelectionChanged
        If rcbDEADSYear.SelectedText = "" Then
            Dim db As New ScorecardCommentsDB
            Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(_year, rgvDEADSPoints.SelectedRows(0).DataBoundItem.Month, _metric.MetricData.CID)
            Me.rtxtDEADSComments.Text = ""
            If dto.Count = 1 Then
                Me.rtxtDEADSComments.Text = dto(0).Comments
            End If
        Else
            Dim selectedyear As Integer = rcbDEADSYear.SelectedText
            Dim db As New ScorecardCommentsDB
            Dim dto As List(Of CommentsDTO) = db.GetCommentsByCID(selectedyear, rgvDEADSPoints.SelectedRows(0).DataBoundItem.Month, _metric.MetricData.CID)
            Me.rtxtDEADSComments.Text = ""
            If dto.Count = 1 Then
                Me.rtxtDEADSComments.Text = dto(0).Comments
            End If
        End If
    End Sub
    Private Sub rmiDERefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rmiDERefresh.Click
        Me.LoadMetrics()
    End Sub
    Private Sub rgvDEAMPoints_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rgvDEAMPoints.ValueChanged

    End Sub

#Region "Performance & Acheivement Levels"

    Public Sub DetermineLevelsTAB2()
        Dim db As New PMPointsDB
        Dim invdb As New PMMetricsDB
        Dim cbmon = rcbDEADSMonth.SelectedText
        Dim ds As String
        Dim dsloc As Integer
        Dim gridindex As Integer = 0
        Dim gridYear As Integer
        If rcbDEADSYear.Text = "" Then
            gridYear = CInt(rcbDEADSYear.SelectedText)
        Else
            gridYear = CInt(rcbDEADSYear.Text)
        End If
        If gridYear <= _year Then
            SetUpAllDataSeriesGrid()
            ClearPerformanceLevelsTAB2()
            ClearAcheivementLevelsTAB2()
            For Each Item As UPMPointsDTO In Me.rgvDEADSPoints.DataSource
                'ds = rgvDEADSPoints.CurrentRow.DataBoundItem.dsDescription
                'need the current index of the grid during each pass of the for each
                ds = rgvDEADSPoints.SelectedRows(0).DataBoundItem.dsDescription
                dsloc = rgvDEADSPoints.SelectedRows(0).ViewInfo.CurrentIndex
                Dim gridDS = Item.dsDescription
                Dim gridmon = Item.MonthDesc
                Dim gridyvalue = Item.YValue

                Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, gridmon, gridYear)
                Dim gridminvalue As Nullable(Of Decimal)
                gridminvalue = mindto(0).YValue
                Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, gridmon, gridYear)
                Dim gridmaxvalue As Nullable(Of Decimal)
                gridmaxvalue = maxdto(0).YValue
                Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, gridmon, gridYear)
                Dim gridtargetvalue As Nullable(Of Decimal)
                gridtargetvalue = targetdto(0).YValue
                Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
                Dim perflevel As String
                Dim achlevel As Integer


                If Not IsNothing(gridminvalue) And Not IsNothing(gridtargetvalue) And Not IsNothing(gridmaxvalue) And Not IsNothing(gridyvalue) Then
                    perflevel = db.GetPerformanceLevel(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    'achlevel = invdb.Get0to200Scale(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    Select Case gridDS
                        Case "MONTHLY"
                            Dim MinMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMinimumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                            Dim MinMonValue As Decimal
                            MinMonValue = MinMonthlydto(0).YValue
                            Dim MaxMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMaximumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                            Dim MaxMonValue As Nullable(Of Decimal)
                            MaxMonValue = MaxMonthlydto(0).YValue
                            Dim TargetMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyTargetValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                            Dim targetMonValue As Nullable(Of Decimal)
                            targetMonValue = TargetMonthlydto(0).YValue

                            If Not IsNothing(MinMonValue) And Not IsNothing(targetMonValue) And Not IsNothing(MaxMonValue) And Not IsNothing(gridyvalue) Then
                                achlevel = invdb.Get0to200Scale(MinMonValue, targetMonValue, MaxMonValue, gridyvalue, inverse)

                                Select Case ds
                                    Case "MONTHLY"
                                        Select Case gridindex
                                            Case 0
                                                rlblAllDSFirstAchLevel.Text = achlevel
                                            Case 1
                                                rlblAllDSSecondAchLevel.Text = achlevel
                                            Case 2
                                                rlblAllDSThirdAchLevel.Text = achlevel
                                            Case 3
                                                rlblAllDSFourthAchLevel.Text = achlevel
                                            Case 4
                                                rlblAllDSFifthAchLevel.Text = achlevel
                                            Case 5
                                                rlblAllDSSixthAchLevel.Text = achlevel
                                            Case 6
                                                rlblAllDSSeventhAchLevel.Text = achlevel
                                            Case 7
                                                rlblAllDSEightAchLevel.Text = achlevel
                                            Case 8
                                                rlblAllDSNinthAchLevel.Text = achlevel
                                            Case 9
                                                rlblAllDSTenthAchLevel.Text = achlevel
                                        End Select
                                End Select
                            End If
                            Select Case gridindex 'dsloc
                                Case 0
                                    rlblAllDSFirstPerformance.Text = perflevel
                                    'rlblAllDSFirstAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFirstPerformance.BackColor = Color.Red
                                            rgvDEADSPoints.Rows(0).Cells(0).CellElement.ForeColor = Color.White
                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFirstPerformance.BackColor = Color.Yellow
                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.ForeColor = Color.White
                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFirstPerformance.BackColor = Color.White
                                            rgvDEADSPoints.Rows.Item(0).Cells(0).CellElement.ForeColor = Color.White

                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.ForeColor = Color.White
                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.BackColor = Color.Gray
                                        Case "Maximum"
                                            rlblAllDSFirstPerformance.BackColor = Color.Green
                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.ForeColor = Color.White
                                            'rgvDEADSPoints.Rows(0).Cells(0).CellElement.BackColor = Color.Green
                                    End Select
                                Case 1
                                    rlblAllDSSecondPerformance.Text = perflevel
                                    'rlblAllDSSecondAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSecondPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSecondPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSecondPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSecondPerformance.BackColor = Color.Green
                                    End Select
                                Case 2
                                    rlblAllDSThirdPerformance.Text = perflevel
                                    ' rlblAllDSThirdAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSThirdPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSThirdPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSThirdPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSThirdPerformance.BackColor = Color.Green
                                    End Select
                                Case 3
                                    rlblAllDSFourthPerformance.Text = perflevel
                                    'rlblAllDSFourthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFourthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFourthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFourthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFourthPerformance.BackColor = Color.Green
                                    End Select
                                Case 4
                                    rlblAllDSFifthPerformance.Text = perflevel
                                    'rlblAllDSFifthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFifthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFifthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFifthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFifthPerformance.BackColor = Color.Green
                                    End Select
                                Case 5
                                    rlblAllDSSixthPerformance.Text = perflevel
                                    'rlblAllDSSixthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSixthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSixthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSixthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSixthPerformance.BackColor = Color.Green
                                    End Select
                                Case 6
                                    rlblAllDSSeventhPerformance.Text = perflevel
                                    ' rlblAllDSSeventhAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSeventhPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Green
                                    End Select
                                Case 7
                                    rlblAllDSEightPerformance.Text = perflevel
                                    'rlblAllDSEightAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSEightPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSEightPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSEightPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSEightPerformance.BackColor = Color.Green
                                    End Select
                                Case 8
                                    rlblAllDSNinthPerformance.Text = perflevel
                                    'rlblAllDSNinthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSNinthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSNinthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSNinthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSNinthPerformance.BackColor = Color.Green
                                    End Select
                                Case 9
                                    rlblAllDSTenthPerformance.Text = perflevel
                                    'rlblAllDSTenthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSTenthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSTenthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSTenthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSTenthPerformance.BackColor = Color.Green
                                    End Select
                            End Select
                        Case "YTD"
                            Select Case gridindex 'dsloc
                                Case 0
                                    rlblAllDSFirstPerformance.Text = perflevel
                                    rlblAllDSFirstAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFirstPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFirstPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFirstPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFirstPerformance.BackColor = Color.Green
                                    End Select
                                Case 1
                                    rlblAllDSSecondPerformance.Text = perflevel
                                    rlblAllDSSecondAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSecondPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSecondPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSecondPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSecondPerformance.BackColor = Color.Green
                                    End Select
                                Case 2
                                    rlblAllDSThirdPerformance.Text = perflevel
                                    rlblAllDSThirdAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSThirdPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSThirdPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSThirdPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSThirdPerformance.BackColor = Color.Green
                                    End Select
                                Case 3
                                    rlblAllDSFourthPerformance.Text = perflevel
                                    rlblAllDSFourthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFourthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFourthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFourthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFourthPerformance.BackColor = Color.Green
                                    End Select
                                Case 4
                                    rlblAllDSFifthPerformance.Text = perflevel
                                    rlblAllDSFifthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFifthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFifthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFifthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFifthPerformance.BackColor = Color.Green
                                    End Select
                                Case 5
                                    rlblAllDSSixthPerformance.Text = perflevel
                                    rlblAllDSSixthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSixthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSixthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSixthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSixthPerformance.BackColor = Color.Green
                                    End Select
                                Case 6
                                    rlblAllDSSeventhPerformance.Text = perflevel
                                    rlblAllDSSeventhAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSeventhPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Green
                                    End Select
                                Case 7
                                    rlblAllDSEightPerformance.Text = perflevel
                                    rlblAllDSEightAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSEightPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSEightPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSEightPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSEightPerformance.BackColor = Color.Green
                                    End Select
                                Case 8
                                    rlblAllDSNinthPerformance.Text = perflevel
                                    rlblAllDSNinthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSNinthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSNinthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSNinthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSNinthPerformance.BackColor = Color.Green
                                    End Select
                                Case 9
                                    rlblAllDSTenthPerformance.Text = perflevel
                                    rlblAllDSTenthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSTenthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSTenthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSTenthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSTenthPerformance.BackColor = Color.Green
                                    End Select
                            End Select
                        Case "PE"
                            Select Case gridindex 'dsloc
                                Case 0
                                    rlblAllDSFirstPerformance.Text = perflevel
                                    rlblAllDSFirstAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFirstPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFirstPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFirstPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFirstPerformance.BackColor = Color.Green
                                    End Select
                                Case 1
                                    rlblAllDSSecondPerformance.Text = perflevel
                                    rlblAllDSSecondAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSecondPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSecondPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSecondPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSecondPerformance.BackColor = Color.Green
                                    End Select
                                Case 2
                                    rlblAllDSThirdPerformance.Text = perflevel
                                    rlblAllDSThirdAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSThirdPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSThirdPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSThirdPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSThirdPerformance.BackColor = Color.Green
                                    End Select
                                Case 3
                                    rlblAllDSFourthPerformance.Text = perflevel
                                    rlblAllDSFourthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFourthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFourthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFourthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFourthPerformance.BackColor = Color.Green
                                    End Select
                                Case 4
                                    rlblAllDSFifthPerformance.Text = perflevel
                                    rlblAllDSFifthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSFifthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSFifthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSFifthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSFifthPerformance.BackColor = Color.Green
                                    End Select
                                Case 5
                                    rlblAllDSSixthPerformance.Text = perflevel
                                    rlblAllDSSixthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSixthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSixthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSixthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSixthPerformance.BackColor = Color.Green
                                    End Select
                                Case 6
                                    rlblAllDSSeventhPerformance.Text = perflevel
                                    rlblAllDSSeventhAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSSeventhPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSSeventhPerformance.BackColor = Color.Green
                                    End Select
                                Case 7
                                    rlblAllDSEightPerformance.Text = perflevel
                                    rlblAllDSEightAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSEightPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSEightPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSEightPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSEightPerformance.BackColor = Color.Green
                                    End Select
                                Case 8
                                    rlblAllDSNinthPerformance.Text = perflevel
                                    rlblAllDSNinthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSNinthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSNinthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSNinthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSNinthPerformance.BackColor = Color.Green
                                    End Select
                                Case 9
                                    rlblAllDSTenthPerformance.Text = perflevel
                                    rlblAllDSTenthAchLevel.Text = achlevel
                                    Select Case perflevel
                                        Case "Below Min"
                                            rlblAllDSTenthPerformance.BackColor = Color.Red
                                        Case "Minimum"
                                            rlblAllDSTenthPerformance.BackColor = Color.Yellow
                                        Case "Target"
                                            rlblAllDSTenthPerformance.BackColor = Color.White
                                        Case "Maximum"
                                            rlblAllDSTenthPerformance.BackColor = Color.Green
                                    End Select
                            End Select
                    End Select
                    gridindex = gridindex + 1
                Else
                    rlblAllDSFirstAchLevel.Text = ""
                    rlblAllDSSecondAchLevel.Text = ""
                    rlblAllDSThirdAchLevel.Text = ""
                    rlblAllDSFourthAchLevel.Text = ""
                    rlblAllDSFifthAchLevel.Text = ""
                    rlblAllDSSixthAchLevel.Text = ""
                    rlblAllDSSeventhAchLevel.Text = ""
                    rlblAllDSEightAchLevel.Text = ""
                    rlblAllDSNinthAchLevel.Text = ""
                    rlblAllDSTenthAchLevel.Text = ""
                    rlblAllDSEleventhAchLevel.Text = ""
                    rlblAllDSTwelveAchLevel.Text = ""

                    rlblAllDSFirstPerformance.BackColor = Color.Transparent
                    rlblAllDSFirstPerformance.Text = ""
                    rlblAllDSSecondPerformance.BackColor = Color.Transparent
                    rlblAllDSSecondPerformance.Text = ""
                    rlblAllDSThirdPerformance.BackColor = Color.Transparent
                    rlblAllDSThirdPerformance.Text = ""
                    rlblAllDSFourthPerformance.BackColor = Color.Transparent
                    rlblAllDSFourthPerformance.Text = ""
                    rlblAllDSFifthPerformance.BackColor = Color.Transparent
                    rlblAllDSFifthPerformance.Text = ""
                    rlblAllDSSixthPerformance.BackColor = Color.Transparent
                    rlblAllDSSixthPerformance.Text = ""
                    rlblAllDSSeventhPerformance.BackColor = Color.Transparent
                    rlblAllDSSeventhPerformance.Text = ""
                    rlblAllDSEightPerformance.BackColor = Color.Transparent
                    rlblAllDSEightPerformance.Text = ""
                    rlblAllDSNinthPerformance.BackColor = Color.Transparent
                    rlblAllDSNinthPerformance.Text = ""
                    rlblAllDSTenthPerformance.BackColor = Color.Transparent
                    rlblAllDSTenthPerformance.Text = ""
                End If
            Next
        Else
            rlblAllDSFirstAchLevel.Text = "No values for " + gridYear.ToString
            rlblAllDSSecondAchLevel.Text = ""
            rlblAllDSThirdAchLevel.Text = ""
            rlblAllDSFourthAchLevel.Text = ""
            rlblAllDSFifthAchLevel.Text = ""
            rlblAllDSSixthAchLevel.Text = ""
            rlblAllDSSeventhAchLevel.Text = ""
            rlblAllDSEightAchLevel.Text = ""
            rlblAllDSNinthAchLevel.Text = ""
            rlblAllDSTenthAchLevel.Text = ""
            rlblAllDSEleventhAchLevel.Text = ""
            rlblAllDSTwelveAchLevel.Text = ""

            rlblAllDSFirstPerformance.BackColor = Color.Transparent
            rlblAllDSFirstPerformance.Text = ""
            rlblAllDSSecondPerformance.BackColor = Color.Transparent
            rlblAllDSSecondPerformance.Text = ""
            rlblAllDSThirdPerformance.BackColor = Color.Transparent
            rlblAllDSThirdPerformance.Text = ""
            rlblAllDSFourthPerformance.BackColor = Color.Transparent
            rlblAllDSFourthPerformance.Text = ""
            rlblAllDSFifthPerformance.BackColor = Color.Transparent
            rlblAllDSFifthPerformance.Text = ""
            rlblAllDSSixthPerformance.BackColor = Color.Transparent
            rlblAllDSSixthPerformance.Text = ""
            rlblAllDSSeventhPerformance.BackColor = Color.Transparent
            rlblAllDSSeventhPerformance.Text = ""
            rlblAllDSEightPerformance.BackColor = Color.Transparent
            rlblAllDSEightPerformance.Text = ""
            rlblAllDSNinthPerformance.BackColor = Color.Transparent
            rlblAllDSNinthPerformance.Text = ""
            rlblAllDSTenthPerformance.BackColor = Color.Transparent
            rlblAllDSTenthPerformance.Text = ""
        End If

    End Sub
    Public Sub ClearAcheivementLevelsTAB1()
        rlblAchLevelDECValue.Text = ""
        rlblAchLevelJANValue.Text = ""
        rlblAchLevelFEBValue.Text = ""
        rlblAchLevelMARValue.Text = ""
        rlblAchLevelAPRValue.Text = ""
        rlblAchLevelMAYValue.Text = ""
        rlblAchLevelJUNValue.Text = ""
        rlblAchLevelJULValue.Text = ""
        rlblAchLevelAUGValue.Text = ""
        rlblAchLevelSEPValue.Text = ""
        rlblAchLevelOCTValue.Text = ""
        rlblAchLevelNOVValue.Text = ""
    End Sub
    Public Sub ClearAcheivementLevelsTAB2()
        rlblAllDSFirstAchLevel.Text = ""
        rlblAllDSSecondAchLevel.Text = ""
        rlblAllDSThirdAchLevel.Text = ""
        rlblAllDSFourthAchLevel.Text = ""
        rlblAllDSFifthAchLevel.Text = ""
        rlblAllDSSixthAchLevel.Text = ""
        rlblAllDSSeventhAchLevel.Text = ""
        rlblAllDSEightAchLevel.Text = ""
        rlblAllDSNinthAchLevel.Text = ""
        rlblAllDSTenthAchLevel.Text = ""
        rlblAllDSEleventhAchLevel.Text = ""
        rlblAllDSTwelveAchLevel.Text = ""
    End Sub
    Public Sub ClearPerformanceLevelsTAB1()
        rlblJANPerformance.BackColor = Color.Transparent
        rlblJANPerformance.Text = ""
        rlblFEBPerformance.BackColor = Color.Transparent
        rlblFEBPerformance.Text = ""
        rlblMARPerformance.BackColor = Color.Transparent
        rlblMARPerformance.Text = ""
        rlblAPRPerformance.BackColor = Color.Transparent
        rlblAPRPerformance.Text = ""
        rlblMayPerformance.BackColor = Color.Transparent
        rlblMayPerformance.Text = ""
        rlblJUNPerformance.BackColor = Color.Transparent
        rlblJUNPerformance.Text = ""
        rlblJULPerformance.BackColor = Color.Transparent
        rlblJULPerformance.Text = ""
        rlblAUGPerformance.BackColor = Color.Transparent
        rlblAUGPerformance.Text = ""
        rlblSEPPerformance.BackColor = Color.Transparent
        rlblSEPPerformance.Text = ""
        rlblOCTPerformance.BackColor = Color.Transparent
        rlblOCTPerformance.Text = ""
        rlblNOVPerformance.BackColor = Color.Transparent
        rlblNOVPerformance.Text = ""
        rlblDECPerformance.BackColor = Color.Transparent
        rlblDECPerformance.Text = ""
    End Sub
    Public Sub ClearPerformanceLevelsTAB2()
        rlblAllDSFirstPerformance.BackColor = Color.Transparent
        rlblAllDSFirstPerformance.Text = ""
        rlblAllDSSecondPerformance.BackColor = Color.Transparent
        rlblAllDSSecondPerformance.Text = ""
        rlblAllDSThirdPerformance.BackColor = Color.Transparent
        rlblAllDSThirdPerformance.Text = ""
        rlblAllDSFourthPerformance.BackColor = Color.Transparent
        rlblAllDSFourthPerformance.Text = ""
        rlblAllDSFifthPerformance.BackColor = Color.Transparent
        rlblAllDSFifthPerformance.Text = ""
        rlblAllDSSixthPerformance.BackColor = Color.Transparent
        rlblAllDSSixthPerformance.Text = ""
        rlblAllDSSeventhPerformance.BackColor = Color.Transparent
        rlblAllDSSeventhPerformance.Text = ""
        rlblAllDSEightPerformance.BackColor = Color.Transparent
        rlblAllDSEightPerformance.Text = ""
        rlblAllDSNinthPerformance.BackColor = Color.Transparent
        rlblAllDSNinthPerformance.Text = ""
        rlblAllDSTenthPerformance.BackColor = Color.Transparent
        rlblAllDSTenthPerformance.Text = ""
    End Sub
    Public Sub DetermineLevels()
        Dim db As New PMPointsDB
        Dim invdb As New PMMetricsDB
        SetUpAllMonthsGrid()
        ClearPerformanceLevelsTAB1()
        ClearAcheivementLevelsTAB1()
        If rcbDEAMDS.SelectedText = "MONTHLY" And CInt(rcbDEAMYear.Text) <= _year Then
            For Each Item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
                Dim gridmon = Item.PointsDescription
                Dim gridyvalue = Item.YValue
                Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridminvalue As Nullable(Of Decimal)
                gridminvalue = mindto(0).YValue
                Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridmaxvalue As Nullable(Of Decimal)
                gridmaxvalue = maxdto(0).YValue
                Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridtargetvalue As Nullable(Of Decimal)
                gridtargetvalue = targetdto(0).YValue
                Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
                Dim perflevel As String
                Dim achlevel As Integer

                Dim MinMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMinimumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim MinMonValue As Nullable(Of Decimal)
                MinMonValue = MinMonthlydto(0).YValue
                Dim MaxMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMaximumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim MaxMonValue As Nullable(Of Decimal)
                MaxMonValue = MaxMonthlydto(0).YValue
                Dim TargetMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyTargetValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim targetMonValue As Nullable(Of Decimal)
                targetMonValue = TargetMonthlydto(0).YValue

                'Check MONTHLY DATA-SERIES ACHEIVEMENT LEVELS
                If rcbDEAMDS.SelectedText = "MONTHLY" Then
                    If Not IsNothing(MinMonValue) And Not IsNothing(targetMonValue) And Not IsNothing(MaxMonValue) And Not IsNothing(gridyvalue) Then
                        achlevel = invdb.Get0to200Scale(MinMonValue, targetMonValue, MaxMonValue, gridyvalue, inverse)
                        Select Case gridmon
                            Case "JAN"
                                rlblAchLevelJANValue.Text = achlevel
                            Case "FEB"
                                rlblAchLevelFEBValue.Text = achlevel
                            Case "MAR"
                                rlblAchLevelMARValue.Text = achlevel
                            Case "APR"
                                rlblAchLevelAPRValue.Text = achlevel
                            Case "MAY"
                                rlblAchLevelMAYValue.Text = achlevel
                            Case "JUN"
                                rlblAchLevelJUNValue.Text = achlevel
                            Case "JUL"
                                rlblAchLevelJULValue.Text = achlevel
                            Case "AUG"
                                rlblAchLevelAUGValue.Text = achlevel
                            Case "SEP"
                                rlblAchLevelSEPValue.Text = achlevel
                            Case "OCT"
                                rlblAchLevelOCTValue.Text = achlevel
                            Case "NOV"
                                rlblAchLevelNOVValue.Text = achlevel
                            Case "DEC"
                                rlblAchLevelDECValue.Text = achlevel
                        End Select
                        'Else
                        '    rlblAchLevelDECValue.Text = ""
                        '    rlblAchLevelJANValue.Text = ""
                        '    rlblAchLevelFEBValue.Text = ""
                        '    rlblAchLevelMARValue.Text = ""
                        '    rlblAchLevelAPRValue.Text = ""
                        '    rlblAchLevelMAYValue.Text = ""
                        '    rlblAchLevelJUNValue.Text = ""
                        '    rlblAchLevelJULValue.Text = ""
                        '    rlblAchLevelAUGValue.Text = ""
                        '    rlblAchLevelSEPValue.Text = ""
                        '    rlblAchLevelOCTValue.Text = ""
                        '    rlblAchLevelNOVValue.Text = ""
                    End If
                End If

                If Not IsNothing(gridminvalue) And Not IsNothing(gridtargetvalue) And Not IsNothing(gridmaxvalue) And Not IsNothing(gridyvalue) Then
                    perflevel = db.GetPerformanceLevel(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    'achlevel = invdb.Get0to200Scale(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    Select Case gridmon
                        Case "JAN"
                            rlblJANPerformance.Text = perflevel
                            'rlblAchLevelJANValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJANPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJANPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJANPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJANPerformance.BackColor = Color.Green
                            End Select
                        Case "FEB"
                            rlblFEBPerformance.Text = perflevel
                            'rlblAchLevelFEBValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblFEBPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblFEBPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblFEBPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblFEBPerformance.BackColor = Color.Green
                            End Select
                        Case "MAR"
                            rlblMARPerformance.Text = perflevel
                            'rlblAchLevelMARValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblMARPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblMARPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblMARPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblMARPerformance.BackColor = Color.Green
                            End Select
                        Case "APR"
                            rlblAPRPerformance.Text = perflevel
                            'rlblAchLevelAPRValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAPRPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAPRPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAPRPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAPRPerformance.BackColor = Color.Green
                            End Select
                        Case "MAY"
                            rlblMayPerformance.Text = perflevel
                            'rlblAchLevelMAYValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblMayPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblMayPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblMayPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblMayPerformance.BackColor = Color.Green
                            End Select
                        Case "JUN"
                            rlblJUNPerformance.Text = perflevel
                            'rlblAchLevelJUNValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJUNPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJUNPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJUNPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJUNPerformance.BackColor = Color.Green
                            End Select
                        Case "JUL"
                            rlblJULPerformance.Text = perflevel
                            'rlblAchLevelJULValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJULPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJULPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJULPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJULPerformance.BackColor = Color.Green
                            End Select
                        Case "AUG"
                            rlblAUGPerformance.Text = perflevel
                            'rlblAchLevelAUGValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAUGPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAUGPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAUGPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAUGPerformance.BackColor = Color.Green
                            End Select
                        Case "SEP"
                            rlblSEPPerformance.Text = perflevel
                            'rlblAchLevelSEPValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblSEPPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblSEPPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblSEPPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblSEPPerformance.BackColor = Color.Green
                            End Select
                        Case "OCT"
                            rlblOCTPerformance.Text = perflevel
                            'rlblAchLevelOCTValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblOCTPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblOCTPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblOCTPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblOCTPerformance.BackColor = Color.Green
                            End Select
                        Case "NOV"
                            rlblNOVPerformance.Text = perflevel
                            'rlblAchLevelNOVValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblNOVPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblNOVPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblNOVPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblNOVPerformance.BackColor = Color.Green
                            End Select
                        Case "DEC"
                            rlblDECPerformance.Text = perflevel
                            'rlblAchLevelDECValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblDECPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblDECPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblDECPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblDECPerformance.BackColor = Color.Green
                            End Select
                    End Select
                End If
            Next
        ElseIf rcbDEAMDS.SelectedText = "YTD" And CInt(rcbDEAMYear.Text) <= _year Then
            For Each Item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
                Dim gridmon = Item.PointsDescription
                Dim gridyvalue = Item.YValue
                Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridminvalue As Nullable(Of Decimal)
                gridminvalue = mindto(0).YValue
                Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridmaxvalue As Nullable(Of Decimal)
                gridmaxvalue = maxdto(0).YValue
                Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridtargetvalue As Nullable(Of Decimal)
                gridtargetvalue = targetdto(0).YValue
                Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
                Dim perflevel As String
                Dim achlevel As Integer
                If Not IsNothing(gridminvalue) And Not IsNothing(gridtargetvalue) And Not IsNothing(gridmaxvalue) And Not IsNothing(gridyvalue) Then
                    perflevel = db.GetPerformanceLevel(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    achlevel = invdb.Get0to200Scale(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    Select Case gridmon
                        Case "JAN"
                            rlblJANPerformance.Text = perflevel
                            rlblAchLevelJANValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJANPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJANPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJANPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJANPerformance.BackColor = Color.Green
                            End Select
                        Case "FEB"
                            rlblFEBPerformance.Text = perflevel
                            rlblAchLevelFEBValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblFEBPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblFEBPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblFEBPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblFEBPerformance.BackColor = Color.Green
                            End Select
                        Case "MAR"
                            rlblMARPerformance.Text = perflevel
                            rlblAchLevelMARValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblMARPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblMARPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblMARPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblMARPerformance.BackColor = Color.Green
                            End Select
                        Case "APR"
                            rlblAPRPerformance.Text = perflevel
                            rlblAchLevelAPRValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAPRPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAPRPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAPRPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAPRPerformance.BackColor = Color.Green
                            End Select
                        Case "MAY"
                            rlblMayPerformance.Text = perflevel
                            rlblAchLevelMAYValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblMayPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblMayPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblMayPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblMayPerformance.BackColor = Color.Green
                            End Select
                        Case "JUN"
                            rlblJUNPerformance.Text = perflevel
                            rlblAchLevelJUNValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJUNPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJUNPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJUNPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJUNPerformance.BackColor = Color.Green
                            End Select
                        Case "JUL"
                            rlblJULPerformance.Text = perflevel
                            rlblAchLevelJULValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJULPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJULPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJULPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJULPerformance.BackColor = Color.Green
                            End Select
                        Case "AUG"
                            rlblAUGPerformance.Text = perflevel
                            rlblAchLevelAUGValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAUGPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAUGPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAUGPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAUGPerformance.BackColor = Color.Green
                            End Select
                        Case "SEP"
                            rlblSEPPerformance.Text = perflevel
                            rlblAchLevelSEPValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblSEPPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblSEPPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblSEPPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblSEPPerformance.BackColor = Color.Green
                            End Select
                        Case "OCT"
                            rlblOCTPerformance.Text = perflevel
                            rlblAchLevelOCTValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblOCTPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblOCTPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblOCTPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblOCTPerformance.BackColor = Color.Green
                            End Select
                        Case "NOV"
                            rlblNOVPerformance.Text = perflevel
                            rlblAchLevelNOVValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblNOVPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblNOVPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblNOVPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblNOVPerformance.BackColor = Color.Green
                            End Select
                        Case "DEC"
                            rlblDECPerformance.Text = perflevel
                            rlblAchLevelDECValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblDECPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblDECPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblDECPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblDECPerformance.BackColor = Color.Green
                            End Select
                    End Select
                End If
            Next
        ElseIf rcbDEAMDS.SelectedText = "PE" And CInt(rcbDEAMYear.Text) <= _year Then
            For Each Item As UPMPointsDTO In Me.rgvDEAMPoints.DataSource
                Dim gridmon = Item.PointsDescription
                Dim gridyvalue = Item.YValue
                Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridminvalue As Nullable(Of Decimal)
                gridminvalue = mindto(0).YValue
                Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridmaxvalue As Nullable(Of Decimal)
                gridmaxvalue = maxdto(0).YValue
                Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, gridmon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
                Dim gridtargetvalue As Nullable(Of Decimal)
                gridtargetvalue = targetdto(0).YValue
                Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
                Dim perflevel As String
                Dim achlevel As Integer
                If Not IsNothing(gridminvalue) And Not IsNothing(gridtargetvalue) And Not IsNothing(gridmaxvalue) And Not IsNothing(gridyvalue) Then
                    perflevel = db.GetPerformanceLevel(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    achlevel = invdb.Get0to200Scale(gridminvalue, gridtargetvalue, gridmaxvalue, gridyvalue, inverse)
                    Select Case gridmon
                        Case "JAN"
                            rlblJANPerformance.Text = perflevel
                            rlblAchLevelJANValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJANPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJANPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJANPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJANPerformance.BackColor = Color.Green
                            End Select
                        Case "FEB"
                            rlblFEBPerformance.Text = perflevel
                            rlblAchLevelFEBValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblFEBPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblFEBPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblFEBPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblFEBPerformance.BackColor = Color.Green
                            End Select
                        Case "MAR"
                            rlblMARPerformance.Text = perflevel
                            rlblAchLevelMARValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblMARPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblMARPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblMARPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblMARPerformance.BackColor = Color.Green
                            End Select
                        Case "APR"
                            rlblAPRPerformance.Text = perflevel
                            rlblAchLevelAPRValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAPRPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAPRPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAPRPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAPRPerformance.BackColor = Color.Green
                            End Select
                        Case "MAY"
                            rlblMayPerformance.Text = perflevel
                            rlblAchLevelMAYValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblMayPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblMayPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblMayPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblMayPerformance.BackColor = Color.Green
                            End Select
                        Case "JUN"
                            rlblJUNPerformance.Text = perflevel
                            rlblAchLevelJUNValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJUNPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJUNPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJUNPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJUNPerformance.BackColor = Color.Green
                            End Select
                        Case "JUL"
                            rlblJULPerformance.Text = perflevel
                            rlblAchLevelJULValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblJULPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblJULPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblJULPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblJULPerformance.BackColor = Color.Green
                            End Select
                        Case "AUG"
                            rlblAUGPerformance.Text = perflevel
                            rlblAchLevelAUGValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAUGPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAUGPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAUGPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAUGPerformance.BackColor = Color.Green
                            End Select
                        Case "SEP"
                            rlblSEPPerformance.Text = perflevel
                            rlblAchLevelSEPValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblSEPPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblSEPPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblSEPPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblSEPPerformance.BackColor = Color.Green
                            End Select
                        Case "OCT"
                            rlblOCTPerformance.Text = perflevel
                            rlblAchLevelOCTValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblOCTPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblOCTPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblOCTPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblOCTPerformance.BackColor = Color.Green
                            End Select
                        Case "NOV"
                            rlblNOVPerformance.Text = perflevel
                            rlblAchLevelNOVValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblNOVPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblNOVPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblNOVPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblNOVPerformance.BackColor = Color.Green
                            End Select
                        Case "DEC"
                            rlblDECPerformance.Text = perflevel
                            rlblAchLevelDECValue.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblDECPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblDECPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblDECPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblDECPerformance.BackColor = Color.Green
                            End Select
                    End Select
                End If
            Next
        Else
            rlblAchLevelDECValue.Text = ""
            rlblAchLevelJANValue.Text = ""
            rlblAchLevelFEBValue.Text = ""
            rlblAchLevelMARValue.Text = ""
            rlblAchLevelAPRValue.Text = ""
            rlblAchLevelMAYValue.Text = ""
            rlblAchLevelJUNValue.Text = ""
            rlblAchLevelJULValue.Text = ""
            rlblAchLevelAUGValue.Text = ""
            rlblAchLevelSEPValue.Text = ""
            rlblAchLevelOCTValue.Text = ""
            rlblAchLevelNOVValue.Text = ""

            rlblJANPerformance.BackColor = Color.Transparent
            rlblJANPerformance.Text = ""
            rlblFEBPerformance.BackColor = Color.Transparent
            rlblFEBPerformance.Text = ""
            rlblMARPerformance.BackColor = Color.Transparent
            rlblMARPerformance.Text = ""
            rlblAPRPerformance.BackColor = Color.Transparent
            rlblAPRPerformance.Text = ""
            rlblMayPerformance.BackColor = Color.Transparent
            rlblMayPerformance.Text = ""
            rlblJUNPerformance.BackColor = Color.Transparent
            rlblJUNPerformance.Text = ""
            rlblJULPerformance.BackColor = Color.Transparent
            rlblJULPerformance.Text = ""
            rlblAUGPerformance.BackColor = Color.Transparent
            rlblAUGPerformance.Text = ""
            rlblSEPPerformance.BackColor = Color.Transparent
            rlblSEPPerformance.Text = ""
            rlblOCTPerformance.BackColor = Color.Transparent
            rlblOCTPerformance.Text = ""
            rlblNOVPerformance.BackColor = Color.Transparent
            rlblNOVPerformance.Text = ""
            rlblDECPerformance.BackColor = Color.Transparent
            rlblDECPerformance.Text = ""
        End If

    End Sub
    Private Sub rgvDEADSPoints_CellEndEdit(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles rgvDEADSPoints.CellEndEdit
        Dim db As New PMPointsDB
        Dim invdb As New PMMetricsDB
        'Dim dto As List(Of UPMPointsDTO) = db.GetYTDValue(Metric.MetricData.CID, rgvDEAMPoints.SelectedRows(0).DataBoundItem.PointsDescription, _year)
        Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
        Dim yValue As Decimal
        Dim ymon As String
        Dim ymonvalue As Integer
        Dim ds As String
        Dim dsloc As Integer
        Dim perflevel As String = String.Empty
        Dim achlevel As Integer
        'You need to include the month value of the row selected and edited.
        yValue = rgvDEADSPoints.SelectedRows(0).DataBoundItem.YValue 'dto(0).YValue
        ymon = rcbDEADSMonth.SelectedText 'rgvDEADSPoints.SelectedRows(0).DataBoundItem.PointsDescription
        ymonvalue = rcbDEADSMonth.SelectedValue
        ds = rgvDEADSPoints.SelectedRows(0).DataBoundItem.dsDescription
        dsloc = rgvDEADSPoints.SelectedRows(0).ViewInfo.CurrentIndex
        Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEADSYear.SelectedValue))
        Dim minvalue As Decimal
        minvalue = mindto(0).YValue
        Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEADSYear.SelectedValue))
        Dim maxvalue As Decimal
        maxvalue = maxdto(0).YValue
        Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEADSYear.SelectedValue))
        Dim targetvalue As Decimal
        targetvalue = targetdto(0).YValue

        'Check MONTHLY DATA-SERIES ACHEIVEMENT LEVELS
        If rgvDEADSPoints.SelectedRows(0).DataBoundItem.dsDescription = "MONTHLY" Then
            Dim MinMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMinimumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
            Dim MinMonValue As Nullable(Of Decimal)
            MinMonValue = MinMonthlydto(0).YValue
            Dim MaxMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMaximumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
            Dim MaxMonValue As Nullable(Of Decimal)
            MaxMonValue = MaxMonthlydto(0).YValue
            Dim TargetMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyTargetValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
            Dim targetMonValue As Nullable(Of Decimal)

            targetMonValue = TargetMonthlydto(0).YValue
            If Not IsNothing(MinMonValue) And Not IsNothing(targetMonValue) And Not IsNothing(MaxMonValue) And Not IsNothing(yValue) Then
                achlevel = invdb.Get0to200Scale(MinMonValue, targetMonValue, MaxMonValue, yValue, inverse)
            End If
            Select Case ds
                Case "MONTHLY"
                    Select Case dsloc
                        Case 0
                            rlblAllDSFirstAchLevel.Text = achlevel
                        Case 1
                            rlblAllDSSecondAchLevel.Text = achlevel
                        Case 2
                            rlblAllDSThirdAchLevel.Text = achlevel
                        Case 3
                            rlblAllDSFourthAchLevel.Text = achlevel
                        Case 4
                            rlblAllDSFifthAchLevel.Text = achlevel
                        Case 5
                            rlblAllDSSixthAchLevel.Text = achlevel
                        Case 6
                            rlblAllDSSeventhAchLevel.Text = achlevel
                        Case 7
                            rlblAllDSEightAchLevel.Text = achlevel
                        Case 8
                            rlblAllDSNinthAchLevel.Text = achlevel
                        Case 9
                            rlblAllDSTenthAchLevel.Text = achlevel
                    End Select
            End Select
            'Else
            '    rlblAchLevelDECValue.Text = ""
            '    rlblAchLevelJANValue.Text = ""
            '    rlblAchLevelFEBValue.Text = ""
            '    rlblAchLevelMARValue.Text = ""
            '    rlblAchLevelAPRValue.Text = ""
            '    rlblAchLevelMAYValue.Text = ""
            '    rlblAchLevelJUNValue.Text = ""
            '    rlblAchLevelJULValue.Text = ""
            '    rlblAchLevelAUGValue.Text = ""
            '    rlblAchLevelSEPValue.Text = ""
            '    rlblAchLevelOCTValue.Text = ""
            '    rlblAchLevelNOVValue.Text = ""
        End If

        If rgvDEADSPoints.SelectedRows(0).DataBoundItem.dsDescription = "YTD" Or rgvDEADSPoints.SelectedRows(0).DataBoundItem.dsDescription = "MONTHLY" Or rgvDEADSPoints.SelectedRows(0).DataBoundItem.dsDescription = "PE" Then
            If Not IsNothing(minvalue) And Not IsNothing(targetvalue) And Not IsNothing(maxvalue) And Not IsNothing(yValue) Then
                perflevel = db.GetPerformanceLevel(minvalue, targetvalue, maxvalue, yValue, inverse)
                achlevel = invdb.Get0to200Scale(minvalue, targetvalue, maxvalue, yValue, inverse)
            End If
            Select Case ds
                Case "MONTHLY"
                    Select Case dsloc
                        Case 0
                            rlblAllDSFirstPerformance.Text = perflevel
                            'rlblAllDSFirstAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFirstPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFirstPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFirstPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFirstPerformance.BackColor = Color.Green
                            End Select
                        Case 1
                            rlblAllDSSecondPerformance.Text = perflevel
                            rlblAllDSSecondAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSecondPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSecondPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSecondPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSecondPerformance.BackColor = Color.Green
                            End Select
                        Case 2
                            rlblAllDSThirdPerformance.Text = perflevel
                            'rlblAllDSThirdAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSThirdPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSThirdPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSThirdPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSThirdPerformance.BackColor = Color.Green
                            End Select
                        Case 3
                            rlblAllDSFourthPerformance.Text = perflevel
                            'rlblAllDSFourthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFourthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFourthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFourthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFourthPerformance.BackColor = Color.Green
                            End Select
                        Case 4
                            rlblAllDSFifthPerformance.Text = perflevel
                            'rlblAllDSFifthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFifthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFifthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFifthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFifthPerformance.BackColor = Color.Green
                            End Select
                        Case 5
                            rlblAllDSSixthPerformance.Text = perflevel
                            'rlblAllDSSixthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSixthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSixthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSixthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSixthPerformance.BackColor = Color.Green
                            End Select
                        Case 6
                            rlblAllDSSeventhPerformance.Text = perflevel
                            'rlblAllDSSeventhAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSeventhPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Green
                            End Select
                        Case 7
                            rlblAllDSEightPerformance.Text = perflevel
                            'rlblAllDSEightAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSEightPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSEightPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSEightPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSEightPerformance.BackColor = Color.Green
                            End Select
                        Case 8
                            rlblAllDSNinthPerformance.Text = perflevel
                            ' rlblAllDSNinthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSNinthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSNinthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSNinthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSNinthPerformance.BackColor = Color.Green
                            End Select
                        Case 9
                            rlblAllDSTenthPerformance.Text = perflevel
                            ' rlblAllDSTenthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSTenthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSTenthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSTenthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSTenthPerformance.BackColor = Color.Green
                            End Select
                    End Select
                Case "YTD"
                    Select Case dsloc
                        Case 0
                            rlblAllDSFirstPerformance.Text = perflevel
                            rlblAllDSFirstAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFirstPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFirstPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFirstPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFirstPerformance.BackColor = Color.Green
                            End Select
                        Case 1
                            rlblAllDSSecondPerformance.Text = perflevel
                            rlblAllDSSecondAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSecondPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSecondPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSecondPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSecondPerformance.BackColor = Color.Green
                            End Select
                        Case 2
                            rlblAllDSThirdPerformance.Text = perflevel
                            rlblAllDSThirdAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSThirdPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSThirdPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSThirdPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSThirdPerformance.BackColor = Color.Green
                            End Select
                        Case 3
                            rlblAllDSFourthPerformance.Text = perflevel
                            rlblAllDSFourthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFourthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFourthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFourthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFourthPerformance.BackColor = Color.Green
                            End Select
                        Case 4
                            rlblAllDSFifthPerformance.Text = perflevel
                            rlblAllDSFifthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFifthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFifthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFifthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFifthPerformance.BackColor = Color.Green
                            End Select
                        Case 5
                            rlblAllDSSixthPerformance.Text = perflevel
                            rlblAllDSSixthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSixthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSixthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSixthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSixthPerformance.BackColor = Color.Green
                            End Select
                        Case 6
                            rlblAllDSSeventhPerformance.Text = perflevel
                            rlblAllDSSeventhAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSeventhPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Green
                            End Select
                        Case 7
                            rlblAllDSEightPerformance.Text = perflevel
                            rlblAllDSEightAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSEightPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSEightPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSEightPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSEightPerformance.BackColor = Color.Green
                            End Select
                        Case 8
                            rlblAllDSNinthPerformance.Text = perflevel
                            rlblAllDSNinthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSNinthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSNinthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSNinthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSNinthPerformance.BackColor = Color.Green
                            End Select
                        Case 9
                            rlblAllDSTenthPerformance.Text = perflevel
                            rlblAllDSTenthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSTenthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSTenthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSTenthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSTenthPerformance.BackColor = Color.Green
                            End Select
                    End Select
                Case "PE"
                    Select Case dsloc
                        Case 0
                            rlblAllDSFirstPerformance.Text = perflevel
                            rlblAllDSFirstAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFirstPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFirstPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFirstPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFirstPerformance.BackColor = Color.Green
                            End Select
                        Case 1
                            rlblAllDSSecondPerformance.Text = perflevel
                            rlblAllDSSecondAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSecondPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSecondPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSecondPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSecondPerformance.BackColor = Color.Green
                            End Select
                        Case 2
                            rlblAllDSThirdPerformance.Text = perflevel
                            rlblAllDSThirdAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSThirdPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSThirdPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSThirdPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSThirdPerformance.BackColor = Color.Green
                            End Select
                        Case 3
                            rlblAllDSFourthPerformance.Text = perflevel
                            rlblAllDSFourthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFourthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFourthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFourthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFourthPerformance.BackColor = Color.Green
                            End Select
                        Case 4
                            rlblAllDSFifthPerformance.Text = perflevel
                            rlblAllDSFifthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSFifthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSFifthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSFifthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSFifthPerformance.BackColor = Color.Green
                            End Select
                        Case 5
                            rlblAllDSSixthPerformance.Text = perflevel
                            rlblAllDSSixthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSixthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSixthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSixthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSixthPerformance.BackColor = Color.Green
                            End Select
                        Case 6
                            rlblAllDSSeventhPerformance.Text = perflevel
                            rlblAllDSSeventhAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSSeventhPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSSeventhPerformance.BackColor = Color.Green
                            End Select
                        Case 7
                            rlblAllDSEightPerformance.Text = perflevel
                            rlblAllDSEightAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSEightPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSEightPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSEightPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSEightPerformance.BackColor = Color.Green
                            End Select
                        Case 8
                            rlblAllDSNinthPerformance.Text = perflevel
                            rlblAllDSNinthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSNinthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSNinthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSNinthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSNinthPerformance.BackColor = Color.Green
                            End Select
                        Case 9
                            rlblAllDSTenthPerformance.Text = perflevel
                            rlblAllDSTenthAchLevel.Text = achlevel
                            Select Case perflevel
                                Case "Below Min"
                                    rlblAllDSTenthPerformance.BackColor = Color.Red
                                Case "Minimum"
                                    rlblAllDSTenthPerformance.BackColor = Color.Yellow
                                Case "Target"
                                    rlblAllDSTenthPerformance.BackColor = Color.White
                                Case "Maximum"
                                    rlblAllDSTenthPerformance.BackColor = Color.Green
                            End Select
                    End Select
            End Select
        Else
            rlblAllDSFirstAchLevel.Text = ""
            rlblAllDSSecondAchLevel.Text = ""
            rlblAllDSThirdAchLevel.Text = ""
            rlblAllDSFourthAchLevel.Text = ""
            rlblAllDSFifthAchLevel.Text = ""
            rlblAllDSSixthAchLevel.Text = ""
            rlblAllDSSeventhAchLevel.Text = ""
            rlblAllDSEightAchLevel.Text = ""
            rlblAllDSNinthAchLevel.Text = ""
            rlblAllDSTenthAchLevel.Text = ""
            rlblAllDSEleventhAchLevel.Text = ""
            rlblAllDSTwelveAchLevel.Text = ""

            rlblAllDSFirstPerformance.BackColor = Color.Transparent
            rlblAllDSFirstPerformance.Text = ""
            rlblAllDSSecondPerformance.BackColor = Color.Transparent
            rlblAllDSSecondPerformance.Text = ""
            rlblAllDSThirdPerformance.BackColor = Color.Transparent
            rlblAllDSThirdPerformance.Text = ""
            rlblAllDSFourthPerformance.BackColor = Color.Transparent
            rlblAllDSFourthPerformance.Text = ""
            rlblAllDSFifthPerformance.BackColor = Color.Transparent
            rlblAllDSFifthPerformance.Text = ""
            rlblAllDSSixthPerformance.BackColor = Color.Transparent
            rlblAllDSSixthPerformance.Text = ""
            rlblAllDSSeventhPerformance.BackColor = Color.Transparent
            rlblAllDSSeventhPerformance.Text = ""
            rlblAllDSEightPerformance.BackColor = Color.Transparent
            rlblAllDSEightPerformance.Text = ""
            rlblAllDSNinthPerformance.BackColor = Color.Transparent
            rlblAllDSNinthPerformance.Text = ""
            rlblAllDSTenthPerformance.BackColor = Color.Transparent
            rlblAllDSTenthPerformance.Text = ""

        End If
    End Sub
    Private Sub rgvDEAMPoints_CellEndEdit(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles rgvDEAMPoints.CellEndEdit
        Dim db As New PMPointsDB
        Dim invdb As New PMMetricsDB
        'Dim dto As List(Of UPMPointsDTO) = db.GetYTDValue(Metric.MetricData.CID, rgvDEAMPoints.SelectedRows(0).DataBoundItem.PointsDescription, _year)
        Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
        Dim yValue As Decimal
        Dim ymon As String
        Dim perflevel As String = String.Empty
        Dim achlevel As Integer
        'You need to include the month value of the row selected and edited.
        yValue = rgvDEAMPoints.SelectedRows(0).DataBoundItem.YValue 'dto(0).YValue
        ymon = rgvDEAMPoints.SelectedRows(0).DataBoundItem.PointsDescription
        Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
        Dim minvalue As Nullable(Of Decimal)
        minvalue = mindto(0).YValue
        Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
        Dim maxvalue As Nullable(Of Decimal)
        maxvalue = maxdto(0).YValue
        Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
        Dim targetvalue As Nullable(Of Decimal)
        targetvalue = targetdto(0).YValue

        'Check MONTHLY ACHEIVEMENT LEVELS
        If rcbDEAMDS.SelectedText = "MONTHLY" Then
            Dim MinMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMinimumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
            Dim MinMonValue As Nullable(Of Decimal)
            MinMonValue = MinMonthlydto(0).YValue
            Dim MaxMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMaximumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
            Dim MaxMonValue As Nullable(Of Decimal)
            MaxMonValue = MaxMonthlydto(0).YValue
            Dim TargetMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyTargetValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
            Dim targetMonValue As Nullable(Of Decimal)
            targetMonValue = TargetMonthlydto(0).YValue

            If Not IsNothing(MinMonValue) And Not IsNothing(targetMonValue) And Not IsNothing(MaxMonValue) And Not IsNothing(yValue) Then
                achlevel = invdb.Get0to200Scale(MinMonValue, targetMonValue, MaxMonValue, yValue, inverse)
            End If
            Select Case ymon
                Case "JAN"
                    rlblAchLevelJANValue.Text = achlevel
                Case "FEB"
                    rlblAchLevelFEBValue.Text = achlevel
                Case "MAR"
                    rlblAchLevelMARValue.Text = achlevel
                Case "APR"
                    rlblAchLevelAPRValue.Text = achlevel
                Case "MAY"
                    rlblAchLevelMAYValue.Text = achlevel
                Case "JUN"
                    rlblAchLevelJUNValue.Text = achlevel
                Case "JUL"
                    rlblAchLevelJULValue.Text = achlevel
                Case "AUG"
                    rlblAchLevelAUGValue.Text = achlevel
                Case "SEP"
                    rlblAchLevelSEPValue.Text = achlevel
                Case "OCT"
                    rlblAchLevelOCTValue.Text = achlevel
                Case "NOV"
                    rlblAchLevelNOVValue.Text = achlevel
                Case "DEC"
                    rlblAchLevelDECValue.Text = achlevel
            End Select
            'Else
            '    rlblAchLevelDECValue.Text = ""
            '    rlblAchLevelJANValue.Text = ""
            '    rlblAchLevelFEBValue.Text = ""
            '    rlblAchLevelMARValue.Text = ""
            '    rlblAchLevelAPRValue.Text = ""
            '    rlblAchLevelMAYValue.Text = ""
            '    rlblAchLevelJUNValue.Text = ""
            '    rlblAchLevelJULValue.Text = ""
            '    rlblAchLevelAUGValue.Text = ""
            '    rlblAchLevelSEPValue.Text = ""
            '    rlblAchLevelOCTValue.Text = ""
            '    rlblAchLevelNOVValue.Text = ""
        End If

        'Check YTD and PE ACHEIVEMENT LEVELS
        If rcbDEAMDS.SelectedText = "YTD" Or rcbDEAMDS.SelectedText = "PE" Then
            If Not IsNothing(minvalue) And Not IsNothing(targetvalue) And Not IsNothing(maxvalue) And Not IsNothing(yValue) Then
                achlevel = invdb.Get0to200Scale(minvalue, targetvalue, maxvalue, yValue, inverse)
            End If
            Select Case ymon
                Case "JAN"
                    rlblAchLevelJANValue.Text = achlevel
                Case "FEB"
                    rlblAchLevelFEBValue.Text = achlevel
                Case "MAR"
                    rlblAchLevelMARValue.Text = achlevel
                Case "APR"
                    rlblAchLevelAPRValue.Text = achlevel
                Case "MAY"
                    rlblAchLevelMAYValue.Text = achlevel
                Case "JUN"
                    rlblAchLevelJUNValue.Text = achlevel
                Case "JUL"
                    rlblAchLevelJULValue.Text = achlevel
                Case "AUG"
                    rlblAchLevelAUGValue.Text = achlevel
                Case "SEP"
                    rlblAchLevelSEPValue.Text = achlevel
                Case "OCT"
                    rlblAchLevelOCTValue.Text = achlevel
                Case "NOV"
                    rlblAchLevelNOVValue.Text = achlevel
                Case "DEC"
                    rlblAchLevelDECValue.Text = achlevel
            End Select
            'Else
            '    rlblAchLevelDECValue.Text = ""
            '    rlblAchLevelJANValue.Text = ""
            '    rlblAchLevelFEBValue.Text = ""
            '    rlblAchLevelMARValue.Text = ""
            '    rlblAchLevelAPRValue.Text = ""
            '    rlblAchLevelMAYValue.Text = ""
            '    rlblAchLevelJUNValue.Text = ""
            '    rlblAchLevelJULValue.Text = ""
            '    rlblAchLevelAUGValue.Text = ""
            '    rlblAchLevelSEPValue.Text = ""
            '    rlblAchLevelOCTValue.Text = ""
            '    rlblAchLevelNOVValue.Text = ""
        End If

        'CHECK Performance Levels YTD, PE and MONTHLY
        If rcbDEAMDS.SelectedText = "YTD" Or rcbDEAMDS.SelectedText = "PE" Or rcbDEAMDS.SelectedText = "MONTHLY" Then
            If Not IsNothing(minvalue) And Not IsNothing(targetvalue) And Not IsNothing(maxvalue) And Not IsNothing(yValue) Then
                perflevel = db.GetPerformanceLevel(minvalue, targetvalue, maxvalue, yValue, inverse)
                'achlevel = invdb.Get0to200Scale(minvalue, targetvalue, maxvalue, yValue, inverse)
            End If
            Select Case ymon
                Case "JAN"
                    rlblJANPerformance.Text = perflevel
                    'rlblAchLevelJANValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblJANPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblJANPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblJANPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblJANPerformance.BackColor = Color.Green
                    End Select
                Case "FEB"
                    rlblFEBPerformance.Text = perflevel
                    'rlblAchLevelFEBValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblFEBPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblFEBPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblFEBPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblFEBPerformance.BackColor = Color.Green
                    End Select
                Case "MAR"
                    rlblMARPerformance.Text = perflevel
                    'rlblAchLevelMARValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblMARPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblMARPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblMARPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblMARPerformance.BackColor = Color.Green
                    End Select
                Case "APR"
                    rlblAPRPerformance.Text = perflevel
                    'rlblAchLevelAPRValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblAPRPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblAPRPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblAPRPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblAPRPerformance.BackColor = Color.Green
                    End Select
                Case "MAY"
                    rlblMayPerformance.Text = perflevel
                    'rlblAchLevelMAYValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblMayPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblMayPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblMayPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblMayPerformance.BackColor = Color.Green
                    End Select
                Case "JUN"
                    rlblJUNPerformance.Text = perflevel
                    'rlblAchLevelJUNValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblJUNPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblJUNPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblJUNPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblJUNPerformance.BackColor = Color.Green
                    End Select
                Case "JUL"
                    rlblJULPerformance.Text = perflevel
                    'rlblAchLevelJULValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblJULPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblJULPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblJULPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblJULPerformance.BackColor = Color.Green
                    End Select
                Case "AUG"
                    rlblAUGPerformance.Text = perflevel
                    'rlblAchLevelAUGValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblAUGPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblAUGPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblAUGPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblAUGPerformance.BackColor = Color.Green
                    End Select
                Case "SEP"
                    rlblSEPPerformance.Text = perflevel
                    'rlblAchLevelSEPValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblSEPPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblSEPPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblSEPPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblSEPPerformance.BackColor = Color.Green
                    End Select
                Case "OCT"
                    rlblOCTPerformance.Text = perflevel
                    'rlblAchLevelOCTValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblOCTPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblOCTPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblOCTPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblOCTPerformance.BackColor = Color.Green
                    End Select
                Case "NOV"
                    rlblNOVPerformance.Text = perflevel
                    'rlblAchLevelNOVValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblNOVPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblNOVPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblNOVPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblNOVPerformance.BackColor = Color.Green
                    End Select
                Case "DEC"
                    rlblDECPerformance.Text = perflevel
                    'rlblAchLevelDECValue.Text = achlevel
                    Select Case perflevel
                        Case "Below Min"
                            rlblDECPerformance.BackColor = Color.Red
                        Case "Minimum"
                            rlblDECPerformance.BackColor = Color.Yellow
                        Case "Target"
                            rlblDECPerformance.BackColor = Color.White
                        Case "Maximum"
                            rlblDECPerformance.BackColor = Color.Green
                    End Select
            End Select
        Else
            'rlblAchLevelDECValue.Text = ""
            'rlblAchLevelJANValue.Text = ""
            'rlblAchLevelFEBValue.Text = ""
            'rlblAchLevelMARValue.Text = ""
            'rlblAchLevelAPRValue.Text = ""
            'rlblAchLevelMAYValue.Text = ""
            'rlblAchLevelJUNValue.Text = ""
            'rlblAchLevelJULValue.Text = ""
            'rlblAchLevelAUGValue.Text = ""
            'rlblAchLevelSEPValue.Text = ""
            'rlblAchLevelOCTValue.Text = ""
            'rlblAchLevelNOVValue.Text = ""

            rlblJANPerformance.BackColor = Color.Transparent
            rlblJANPerformance.Text = ""
            rlblFEBPerformance.BackColor = Color.Transparent
            rlblFEBPerformance.Text = ""
            rlblMARPerformance.BackColor = Color.Transparent
            rlblMARPerformance.Text = ""
            rlblAPRPerformance.BackColor = Color.Transparent
            rlblAPRPerformance.Text = ""
            rlblMayPerformance.BackColor = Color.Transparent
            rlblMayPerformance.Text = ""
            rlblJUNPerformance.BackColor = Color.Transparent
            rlblJUNPerformance.Text = ""
            rlblJULPerformance.BackColor = Color.Transparent
            rlblJULPerformance.Text = ""
            rlblAUGPerformance.BackColor = Color.Transparent
            rlblAUGPerformance.Text = ""
            rlblSEPPerformance.BackColor = Color.Transparent
            rlblSEPPerformance.Text = ""
            rlblOCTPerformance.BackColor = Color.Transparent
            rlblOCTPerformance.Text = ""
            rlblNOVPerformance.BackColor = Color.Transparent
            rlblNOVPerformance.Text = ""
            rlblDECPerformance.BackColor = Color.Transparent
            rlblDECPerformance.Text = ""
        End If
    End Sub
#End Region

    'Private Sub rgvDEAMPoints_ViewCellFormatting(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.CellFormattingEventArgs) Handles rgvDEAMPoints.ViewCellFormatting
    '    'e.CellElement.ForeColor = Color.Blue
    '    'e.CellElement.RowElement.BackColor = Color.Yellow
    '    Dim db As New PMPointsDB
    '    Dim invdb As New PMMetricsDB
    '    'Dim dto As List(Of UPMPointsDTO) = db.GetYTDValue(Metric.MetricData.CID, rgvDEAMPoints.SelectedRows(0).DataBoundItem.PointsDescription, _year)
    '    Dim inverse As String = invdb.GetMetricInvScale(Metric.MetricData.CID)
    '    Dim yValue As Decimal
    '    Dim ymon As String
    '    Dim perflevel As String = String.Empty
    '    Dim achlevel As Integer
    '    'You need to include the month value of the row selected and edited.
    '    yValue = rgvDEAMPoints.SelectedRows(0).DataBoundItem.YValue 'dto(0).YValue
    '    ymon = rgvDEAMPoints.SelectedRows(0).DataBoundItem.PointsDescription
    '    Dim mindto As List(Of UPMPointsDTO) = db.GetMinimumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
    '    Dim minvalue As Nullable(Of Decimal)
    '    minvalue = mindto(0).YValue
    '    Dim maxdto As List(Of UPMPointsDTO) = db.GetMaximumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
    '    Dim maxvalue As Nullable(Of Decimal)
    '    maxvalue = maxdto(0).YValue
    '    Dim targetdto As List(Of UPMPointsDTO) = db.GetTargetValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
    '    Dim targetvalue As Nullable(Of Decimal)
    '    targetvalue = targetdto(0).YValue

    '    'Check MONTHLY ACHEIVEMENT LEVELS
    '    If rcbDEAMDS.SelectedText = "MONTHLY" Then
    '        Dim MinMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMinimumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
    '        Dim MinMonValue As Nullable(Of Decimal)
    '        MinMonValue = MinMonthlydto(0).YValue
    '        Dim MaxMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyMaximumValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
    '        Dim MaxMonValue As Nullable(Of Decimal)
    '        MaxMonValue = MaxMonthlydto(0).YValue
    '        Dim TargetMonthlydto As List(Of UPMPointsDTO) = db.GetMonthlyTargetValue(Metric.MetricData.CID, ymon, If(IsNothing(rcbDEAMYear.SelectedValue) = True, _year, rcbDEAMYear.SelectedValue))
    '        Dim targetMonValue As Nullable(Of Decimal)
    '        targetMonValue = TargetMonthlydto(0).YValue

    '        'If Not IsNothing(MinMonValue) And Not IsNothing(targetMonValue) And Not IsNothing(MaxMonValue) And Not IsNothing(yValue) Then
    '        '    achlevel = invdb.Get0to200Scale(MinMonValue, targetMonValue, MaxMonValue, yValue, inverse)
    '        'End If
    '        'Select Case ymon
    '        '    Case "JAN"
    '        '        rlblAchLevelJANValue.Text = achlevel
    '        '    Case "FEB"
    '        '        rlblAchLevelFEBValue.Text = achlevel
    '        '    Case "MAR"
    '        '        rlblAchLevelMARValue.Text = achlevel
    '        '    Case "APR"
    '        '        rlblAchLevelAPRValue.Text = achlevel
    '        '    Case "MAY"
    '        '        rlblAchLevelMAYValue.Text = achlevel
    '        '    Case "JUN"
    '        '        rlblAchLevelJUNValue.Text = achlevel
    '        '    Case "JUL"
    '        '        rlblAchLevelJULValue.Text = achlevel
    '        '    Case "AUG"
    '        '        rlblAchLevelAUGValue.Text = achlevel
    '        '    Case "SEP"
    '        '        rlblAchLevelSEPValue.Text = achlevel
    '        '    Case "OCT"
    '        '        rlblAchLevelOCTValue.Text = achlevel
    '        '    Case "NOV"
    '        '        rlblAchLevelNOVValue.Text = achlevel
    '        '    Case "DEC"
    '        '        rlblAchLevelDECValue.Text = achlevel
    '        'End Select
    '        'Else
    '        '    rlblAchLevelDECValue.Text = ""
    '        '    rlblAchLevelJANValue.Text = ""
    '        '    rlblAchLevelFEBValue.Text = ""
    '        '    rlblAchLevelMARValue.Text = ""
    '        '    rlblAchLevelAPRValue.Text = ""
    '        '    rlblAchLevelMAYValue.Text = ""
    '        '    rlblAchLevelJUNValue.Text = ""
    '        '    rlblAchLevelJULValue.Text = ""
    '        '    rlblAchLevelAUGValue.Text = ""
    '        '    rlblAchLevelSEPValue.Text = ""
    '        '    rlblAchLevelOCTValue.Text = ""
    '        '    rlblAchLevelNOVValue.Text = ""
    '    End If

    '    'Check YTD and PE ACHEIVEMENT LEVELS
    '    'If rcbDEAMDS.SelectedText = "YTD" Or rcbDEAMDS.SelectedText = "PE" Then
    '    '    If Not IsNothing(minvalue) And Not IsNothing(targetvalue) And Not IsNothing(maxvalue) And Not IsNothing(yValue) Then
    '    '        achlevel = invdb.Get0to200Scale(minvalue, targetvalue, maxvalue, yValue, inverse)
    '    '    End If
    '    '    Select Case ymon
    '    '        Case "JAN"
    '    '            rlblAchLevelJANValue.Text = achlevel
    '    '        Case "FEB"
    '    '            rlblAchLevelFEBValue.Text = achlevel
    '    '        Case "MAR"
    '    '            rlblAchLevelMARValue.Text = achlevel
    '    '        Case "APR"
    '    '            rlblAchLevelAPRValue.Text = achlevel
    '    '        Case "MAY"
    '    '            rlblAchLevelMAYValue.Text = achlevel
    '    '        Case "JUN"
    '    '            rlblAchLevelJUNValue.Text = achlevel
    '    '        Case "JUL"
    '    '            rlblAchLevelJULValue.Text = achlevel
    '    '        Case "AUG"
    '    '            rlblAchLevelAUGValue.Text = achlevel
    '    '        Case "SEP"
    '    '            rlblAchLevelSEPValue.Text = achlevel
    '    '        Case "OCT"
    '    '            rlblAchLevelOCTValue.Text = achlevel
    '    '        Case "NOV"
    '    '            rlblAchLevelNOVValue.Text = achlevel
    '    '        Case "DEC"
    '    '            rlblAchLevelDECValue.Text = achlevel
    '    '    End Select
    '    '    'Else
    '    '    '    rlblAchLevelDECValue.Text = ""
    '    '    '    rlblAchLevelJANValue.Text = ""
    '    '    '    rlblAchLevelFEBValue.Text = ""
    '    '    '    rlblAchLevelMARValue.Text = ""
    '    '    '    rlblAchLevelAPRValue.Text = ""
    '    '    '    rlblAchLevelMAYValue.Text = ""
    '    '    '    rlblAchLevelJUNValue.Text = ""
    '    '    '    rlblAchLevelJULValue.Text = ""
    '    '    '    rlblAchLevelAUGValue.Text = ""
    '    '    '    rlblAchLevelSEPValue.Text = ""
    '    '    '    rlblAchLevelOCTValue.Text = ""
    '    '    '    rlblAchLevelNOVValue.Text = ""
    '    'End If

    '    'CHECK Performance Levels YTD, PE and MONTHLY
    '    If rcbDEAMDS.SelectedText = "YTD" Or rcbDEAMDS.SelectedText = "PE" Or rcbDEAMDS.SelectedText = "MONTHLY" Then
    '        If Not IsNothing(minvalue) And Not IsNothing(targetvalue) And Not IsNothing(maxvalue) And Not IsNothing(yValue) Then
    '            perflevel = db.GetPerformanceLevel(minvalue, targetvalue, maxvalue, yValue, inverse)
    '            'achlevel = invdb.Get0to200Scale(minvalue, targetvalue, maxvalue, yValue, inverse)
    '        End If
    '        Select Case e.CellElement.Text
    '            Case "JAN"
    '                rlblJANPerformance.Text = perflevel
    '                'rlblAchLevelJANValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblJANPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblJANPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblJANPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gold
    '                    Case "Maximum"
    '                        rlblJANPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "FEB"
    '                rlblFEBPerformance.Text = perflevel
    '                'rlblAchLevelFEBValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblFEBPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblFEBPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblFEBPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblFEBPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "MAR"
    '                rlblMARPerformance.Text = perflevel
    '                'rlblAchLevelMARValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblMARPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblMARPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblMARPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblMARPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "APR"
    '                rlblAPRPerformance.Text = perflevel
    '                'rlblAchLevelAPRValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblAPRPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblAPRPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblAPRPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblAPRPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "MAY"
    '                rlblMayPerformance.Text = perflevel
    '                'rlblAchLevelMAYValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblMayPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblMayPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblMayPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblMayPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "JUN"
    '                rlblJUNPerformance.Text = perflevel
    '                'rlblAchLevelJUNValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblJUNPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblJUNPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblJUNPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblJUNPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "JUL"
    '                rlblJULPerformance.Text = perflevel
    '                'rlblAchLevelJULValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblJULPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblJULPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblJULPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblJULPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "AUG"
    '                rlblAUGPerformance.Text = perflevel
    '                'rlblAchLevelAUGValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblAUGPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblAUGPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblAUGPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblAUGPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "SEP"
    '                rlblSEPPerformance.Text = perflevel
    '                'rlblAchLevelSEPValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblSEPPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblSEPPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblSEPPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblSEPPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "OCT"
    '                rlblOCTPerformance.Text = perflevel
    '                'rlblAchLevelOCTValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblOCTPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblOCTPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblOCTPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblOCTPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "NOV"
    '                rlblNOVPerformance.Text = perflevel
    '                'rlblAchLevelNOVValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblNOVPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblNOVPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblNOVPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblNOVPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '            Case "DEC"
    '                rlblDECPerformance.Text = perflevel
    '                'rlblAchLevelDECValue.Text = achlevel
    '                Select Case perflevel
    '                    Case "Below Min"
    '                        rlblDECPerformance.BackColor = Color.Red
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Red
    '                    Case "Minimum"
    '                        rlblDECPerformance.BackColor = Color.Yellow
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Yellow
    '                    Case "Target"
    '                        rlblDECPerformance.BackColor = Color.White
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Gray
    '                    Case "Maximum"
    '                        rlblDECPerformance.BackColor = Color.Green
    '                        e.CellElement.ForeColor = Color.White
    '                        e.CellElement.BackColor = Color.Green
    '                End Select
    '        End Select
    '    Else
    '        'rlblAchLevelDECValue.Text = ""
    '        'rlblAchLevelJANValue.Text = ""
    '        'rlblAchLevelFEBValue.Text = ""
    '        'rlblAchLevelMARValue.Text = ""
    '        'rlblAchLevelAPRValue.Text = ""
    '        'rlblAchLevelMAYValue.Text = ""
    '        'rlblAchLevelJUNValue.Text = ""
    '        'rlblAchLevelJULValue.Text = ""
    '        'rlblAchLevelAUGValue.Text = ""
    '        'rlblAchLevelSEPValue.Text = ""
    '        'rlblAchLevelOCTValue.Text = ""
    '        'rlblAchLevelNOVValue.Text = ""

    '        rlblJANPerformance.BackColor = Color.Transparent
    '        rlblJANPerformance.Text = ""
    '        rlblFEBPerformance.BackColor = Color.Transparent
    '        rlblFEBPerformance.Text = ""
    '        rlblMARPerformance.BackColor = Color.Transparent
    '        rlblMARPerformance.Text = ""
    '        rlblAPRPerformance.BackColor = Color.Transparent
    '        rlblAPRPerformance.Text = ""
    '        rlblMayPerformance.BackColor = Color.Transparent
    '        rlblMayPerformance.Text = ""
    '        rlblJUNPerformance.BackColor = Color.Transparent
    '        rlblJUNPerformance.Text = ""
    '        rlblJULPerformance.BackColor = Color.Transparent
    '        rlblJULPerformance.Text = ""
    '        rlblAUGPerformance.BackColor = Color.Transparent
    '        rlblAUGPerformance.Text = ""
    '        rlblSEPPerformance.BackColor = Color.Transparent
    '        rlblSEPPerformance.Text = ""
    '        rlblOCTPerformance.BackColor = Color.Transparent
    '        rlblOCTPerformance.Text = ""
    '        rlblNOVPerformance.BackColor = Color.Transparent
    '        rlblNOVPerformance.Text = ""
    '        rlblDECPerformance.BackColor = Color.Transparent
    '        rlblDECPerformance.Text = ""
    '    End If
    'End Sub

    Private Sub tiAllMonths_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles tiAllMonths.Click
        Dim MetricPoints As New PMPoint
        MetricPoints.GetCurrentMetric(Me.GetSelectedMetric, rcbDEAMYear.Text)
        SetUpAllMonthsGrid()
        'LoadDataSeriesforDLB()
        'DataEntryAMTab(sender, e)

        'LoadGridViewByMonths(rgvDEAMPoints, _dataSeries)
    End Sub
    Private Sub PopulateCurrentMetricData(ByVal metric As MetricDTO, ByVal year As Integer)
        Dim db As New PMDataSeriesDB
        Dim ds As PMMetricDataSeries
        Dim s As New My.MySettings
        Dim list As List(Of DataSeriesDTO)
        'Dim incDB As New PMIncentiveDB
        'Dim inc As List(Of IncentivesDTO)
        '_metric = metric
        list = db.GetUPMDataSeries(metric.CID)

        'inc = incDB.GetIncentive(metric.CID, metric.ReportingYear)
        Dim dataseries As List(Of PMMetricDataSeries)
        dataseries = New List(Of PMMetricDataSeries)
        For Each dto As DataSeriesDTO In list
            ds = New PMMetricDataSeries(dto, year)
            dataseries.Add(ds)
        Next
        dataseries = dataseries
    End Sub
    Public Class PMMetricDataSeries
        Implements ICloneable

        Private _series As DataSeriesDTO
        Private _points As List(Of UPMPointsDTO)

        Public Sub New()
            _series = New DataSeriesDTO
            _points = New List(Of UPMPointsDTO)
        End Sub
        Public Sub New(ByVal series As DataSeriesDTO, ByVal year As Integer)
            Dim Pointslist As List(Of UPMPointsDTO)
            Dim s As New My.MySettings
            Dim db As New PMPointsDB
            Dim point As UPMPointsDTO
            _series = series
            _points = New List(Of UPMPointsDTO)
            Pointslist = db.GetUPMPoints(series.DSID, year) ', series.Description)
            For Each dto As UPMPointsDTO In Pointslist
                point = New UPMPointsDTO
                point = dto
                _points.Add(point)
            Next
        End Sub
        Public Property DataSeries() As DataSeriesDTO
            Get
                Return _series
            End Get
            Set(ByVal value As DataSeriesDTO)
                _series = value
            End Set
        End Property
        Public Property Points() As List(Of UPMPointsDTO)
            Get
                Return _points
            End Get
            Set(ByVal value As List(Of UPMPointsDTO))
                _points = value
            End Set
        End Property

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim newDTO As New PMMetricDataSeries
            newDTO.DataSeries = _series.Clone
            Dim tmpPoints As New List(Of UPMPointsDTO)
            For Each dto As UPMPointsDTO In _points
                tmpPoints.Add(dto.Clone)
            Next
            newDTO.Points = tmpPoints
            Return newDTO
        End Function
    End Class
    Private Sub rgvDEAMPoints_CurrentRowChanged(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.CurrentRowChangedEventArgs) Handles rgvDEAMPoints.CurrentRowChanged
        'get comments for oldrow
        Dim oldrow As GridViewDataRowInfo = e.OldRow
        Dim newrow As GridViewDataRowInfo = e.CurrentRow
        Dim oldrowdata As UPMPointsDTO
        Dim newrowdata As UPMPointsDTO
        If Not IsNothing(e.OldRow.DataBoundItem) Then
            oldrowdata = oldrow.DataBoundItem ' get month
            _AllComments(oldrowdata.month).Comments = rtxtDEAMComments.Text ' get old row months and put comments in dict
        End If
        If Not IsNothing(e.CurrentRow.DataBoundItem) Then
            newrowdata = newrow.DataBoundItem
            rtxtDEAMComments.Text = _AllComments(newrowdata.month).Comments ' get what is in dict and place in new row
        End If

    End Sub
End Class