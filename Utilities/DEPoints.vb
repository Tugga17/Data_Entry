Imports Entergy.PerformanceManagement.PMDTO
Imports Entergy.PerformanceManagement.PMDB
Public Class DEPoints
#Region "Privates"
    Private _metric As MetricDTO
    Private _dataSeries As List(Of PMMetricDataSeries)
    Private _dataSeriesbyMonth As List(Of PMMetricsDataSeriesPoints)
    Private _compComp As List(Of MetricCompositesDTO)
    Private _repyear As UPMPointsDTO
    Private _dseries As DataSeriesDTO
    Private _months As MonthsDTO
    Private _month As String
    Private _getmonth As List(Of PMLoadMonths)
    Public _currentmonth As Integer
    Private monthList As List(Of PMLoadMonths)
    Public ReadOnly Property DataSeries As List(Of PMMetricDataSeries)
        Get
            Return _dataSeries
        End Get
    End Property

    Private Sub PopulateDSPointsGridByMonth(ByVal dseries As MetricDTO, ByVal year As Integer)
        _metric = dseries
        Dim db As New PMDataSeriesDB
        Dim s As New My.MySettings
        Dim resultset = db.GetUPMDataSeriesByMonth(dseries.CID, year, PMDataEntry.rcbDEADSMonth.SelectedValue)

    End Sub
    Public Sub InitializeBuffers()


    End Sub
    Private Sub PopulateMetricData_AllDS(ByVal dseries As MetricDTO)
        Dim db As New PMDataSeriesDB
        'Dim ds As PMMetricsDataSeriesPoints
        Dim s As New My.MySettings
        Dim list As List(Of DataSeriesDTO)
        _metric = dseries
        Dim mb As New PMMonthsDB
        Dim monthlist = New List(Of PMMonthsDB)
        'Dim currmonth As PMLoadMonths
        'monthlist = mb.GetUPMMonths()
        'For Each mto As MonthsDTO In monthlist
        list = db.GetUPMDataSeries(dseries.CID)
        _dataSeriesbyMonth = New List(Of PMMetricsDataSeriesPoints)

    End Sub
    Private Sub PopulateMetricData(ByVal metric As MetricDTO, ByVal year As Integer)
        Dim db As New PMDataSeriesDB
        Dim ds As PMMetricDataSeries
        Dim list As List(Of DataSeriesDTO)
        'Dim incDB As New PMIncentiveDB
        'Dim inc As List(Of IncentivesDTO)
        _metric = metric
        list = db.GetUPMDataSeries(metric.CID)

        'inc = incDB.GetIncentive(metric.CID, metric.ReportingYear)
        _dataSeries = New List(Of PMMetricDataSeries)
        For Each dto As DataSeriesDTO In list
            ds = New PMMetricDataSeries(dto, year)
            _dataSeries.Add(ds)
        Next

    End Sub
    Public Sub PopulateMonthData(ByVal metric As MonthsDTO)
        Dim db As New PMMonthsDB
        Dim ds As PMLoadMonths
        Dim List As List(Of MonthsDTO)
        _months = metric
        List = db.GetUPMMonths()
        _getmonth = New List(Of PMLoadMonths)
        For Each dto As MonthsDTO In List
            ds = New PMLoadMonths(dto)
            _getmonth.Add(ds)
        Next
    End Sub
    Public Function GetDataSeries(ByVal dataSeries As Integer) As PMMetricDataSeries
        Dim series As PMMetricDataSeries = Nothing

        For Each ds As PMMetricDataSeries In _dataSeries
            If ds.DataSeries.DataSeries = dataSeries Then series = ds
        Next
        Return series
    End Function
    Public Function GetDataSeriesByMonth(ByVal dataseries As Integer) As PMMetricsDataSeriesPoints
        Dim series As PMMetricsDataSeriesPoints = Nothing

        For Each ds As PMMetricsDataSeriesPoints In _dataSeriesbyMonth
            If ds.DataSeries.DataSeries = dataseries Then series = ds
        Next
        Return series
    End Function
#End Region
#Region "Public"
    Public Property MetricData() As MetricDTO
        Get
            Return _metric
        End Get
        Set(ByVal value As MetricDTO)
            _metric = value
        End Set
    End Property
    Public Property RepYearData() As UPMPointsDTO
        Get
            Return _repyear
        End Get
        Set(ByVal value As UPMPointsDTO)
            _repyear = value
        End Set
    End Property
    'Public Property CID As MetricDTO
    '    Get
    '        Return _cID
    '    End Get
    '    Set(ByVal value As MetricDTO)
    '        _cID = value
    '    End Set
    'End Property
    Public Function GetNextAvailableDataSeries() As Integer
        Dim ret As Integer = 0
        For Each ds As PMMetricDataSeries In _dataSeries
            If ds.DataSeries.DataSeries > ret Then ret = ds.DataSeries.DataSeries
        Next
        Return ret + 1
    End Function
    Public Function IsDataSeriesUsed(ByVal dataSeries As Integer) As Boolean
        Dim ret As Boolean = False
        For Each ds As PMMetricDataSeries In _dataSeries
            If ds.DataSeries.DataSeries = dataSeries Then ret = True
        Next
        Return ret
    End Function
    Public Function IsDataSeriesDescriptionUsed(ByVal description As String) As Boolean
        Dim ret As Boolean = False
        For Each ds As PMMetricDataSeries In _dataSeries
            If ds.DataSeries.Description = description Then ret = True
        Next
        Return ret
    End Function
    Public Sub GetMetric(ByVal metric As MetricDTO, ByVal year As Integer)

        'Dim loadData As New DataEntry
        Me.PopulateMetricData(metric, year)
        Me.PopulateMonthData(_months)
        'loadData.Metric = Me.MetricData
        'loadData.rgvDEMetrics.Enabled = False
        'loadData.rpDEGrid.Visible = False
        'loadData.ShowDialog()

    End Sub
    Public Sub GetCurrentMetric(ByVal metric As MetricDTO, ByVal year As Integer)
        Dim LoadData As New DataEntry
        Me.PopulateMetricData(metric, year)
        LoadData.Metric = Me.MetricData

    End Sub
    Public Sub GetDS(ByVal metric As MetricDTO, ByVal year As Integer)
        Dim LoadData As New DataEntry
        Me.PopulateDSPointsGridByMonth(metric, Year)
        'e.PopulateMetricData_AllDS(metric)
        'Me.PopulateMonthData(_months)
        'LoadData.Metric = Me
        'LoadData.ShowDialog()
    End Sub
    Private Function HasPoints(ByVal year As Integer, ByVal dsID As Integer) As Boolean
        Dim db As New PMPointsDB
        Dim list As List(Of UPMPointsDTO) = db.GetUPMPoints(dsID, year)
        If list.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Function GetSeriesDataList() As List(Of DataSeriesDTO)
        Dim list As New List(Of DataSeriesDTO)
        For Each series As PMMetricDataSeries In _dataSeries
            list.Add(series.DataSeries)
        Next
        Return list
    End Function
    Public Function GetMonthDataList() As List(Of MonthsDTO)
        Dim list As New List(Of MonthsDTO)
        For Each month As PMLoadMonths In _getmonth
            list.Add(month.loadMonths)
        Next
        '    list.Add(series.DataSeries)
        'Next
        Return list
    End Function
    Public Sub RefreshPoints(ByVal year As Integer)
        Dim db As New PMPointsDB
        Dim s As New My.MySettings
        'Dim year As String = s.ReportingYear
        For Each ds As PMMetricDataSeries In _dataSeries
            If Not IsNothing(ds.DataSeries.DSID) Then
                ds.Points = db.GetUPMPoints(ds.DataSeries.DSID, year)
            End If
        Next
    End Sub
    Public Sub RefreshDSPointsByYear(ByVal grid As Object)
        Dim db As New PMPointsDB
        Dim s As New My.MySettings
        Dim year As Integer = grid.SelectedText
        For Each ds As PMMetricDataSeries In _dataSeries
            If Not IsNothing(ds.DataSeries.DSID) Then
                ds.Points = db.GetUPMPoints(ds.DataSeries.DSID, year)
            End If
        Next

    End Sub

#End Region
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
    Public Class PMMetricsDataSeriesPoints
        Implements ICloneable
        Private _month As MonthsDTO
        Private _seriesbyMonth As DataSeriesDTO
        Private _points As List(Of UPMPointsDTO)

        Public Sub New()
            _month = New MonthsDTO
            _seriesbyMonth = New DataSeriesDTO
            _points = New List(Of UPMPointsDTO)
        End Sub
        Public Sub New(ByVal series As DataSeriesDTO, ByVal month As Integer, ByVal year As Integer)
            Dim Pointslist As List(Of UPMPointsDTO)
            'Dim _currentMonth = New MonthsDTO
            Dim db As New PMPointsDB
            Dim point As UPMPointsDTO
            _seriesbyMonth = series
            _points = New List(Of UPMPointsDTO)
            Pointslist = db.GetUPMPointsDS(series.CID, year, month)
            For Each dto As UPMPointsDTO In Pointslist
                point = New UPMPointsDTO
                point = dto
                _points.Add(point)
            Next

        End Sub
        Public Property DataSeries() As DataSeriesDTO
            Get
                Return _seriesbyMonth
            End Get
            Set(ByVal value As DataSeriesDTO)
                _seriesbyMonth = value
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
            newDTO.DataSeries = _seriesbyMonth.Clone
            Dim tmpPoints As New List(Of UPMPointsDTO)
            For Each dto As UPMPointsDTO In _points
                tmpPoints.Add(dto.Clone)
            Next
            newDTO.Points = tmpPoints
            Return newDTO
        End Function
    End Class
    Public Class PMLoadMonths
        Implements ICloneable

        Private _months As MonthsDTO
        Private _getmonth As New List(Of MonthsDTO)
        Public Sub New()
            _months = New MonthsDTO
            '_points = New List(Of UPMPointsDTO)
        End Sub
        Public Sub New(ByVal month As MonthsDTO)
            Dim Monthlist As List(Of MonthsDTO)
            'Dim s As New My.MySettings
            Dim db As New PMMonthsDB
            'Dim point As MonthsDTO
            _months = month
            '_months = New List(Of MonthsDTO)
            Monthlist = db.GetUPMMonths()
            'For Each dto As MonthsDTO In Monthlist
            'point = New MonthsDTO
            'point = dto
            '_getmonth.Add(point)
            'Next
        End Sub
        Public Property loadMonths() As MonthsDTO
            Get
                Return _months
            End Get
            Set(ByVal value As MonthsDTO)
                _months = value
            End Set
        End Property

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim newDTO As New PMLoadMonths
            newDTO.loadMonths = _months.Clone
            Dim tmpMonths As New List(Of MonthsDTO)
            For Each dto As MonthsDTO In _getmonth
                tmpMonths.Add(dto.Clone)
            Next
            'newDTO.loadMonths = tmpMonths
            Return newDTO
        End Function
    End Class
    
End Class
