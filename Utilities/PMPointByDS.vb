Imports Entergy.PerformanceManagement.PMDTO
Imports Entergy.PerformanceManagement.PMDB
Public Class PMPointByDS
#Region "Privates"
    Private _metric As MetricDTO
    'Private _dataSeries As List(Of PMMetricDataSeries)
    Private _dataSeriesbyMonth As List(Of PMMetricsDataSeriesPoints)
    Private _compComp As List(Of MetricCompositesDTO)
    Private _repyear As UPMPointsDTO
    Private _dseries As DataSeriesDTO

    Private Sub PopulateMetricData_AllDS(ByVal dseries As DataSeriesDTO, ByVal year As Integer)
        Dim db As New PMDataSeriesDB
        Dim ds As PMMetricsDataSeriesPoints
        Dim s As New My.MySettings
        Dim list As New List(Of DataSeriesDTO)
        _dseries = dseries
        'list = db.GetUPMDataSeriesByMonth(dseries.DSID, dseries.ReportingYear, dseries.month)

        'inc = incDB.GetIncentive(metric.CID, metric.ReportingYear)
        _dataSeriesbyMonth = New List(Of PMMetricsDataSeriesPoints)
        For Each dto As DataSeriesDTO In list
            ds = New PMMetricsDataSeriesPoints(dto, year)
            _dataSeriesbyMonth.Add(ds)
        Next
    End Sub
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
    Public Sub GetDS(ByVal dseries As DataSeriesDTO, ByVal year As Integer)
        Me.PopulateMetricData_AllDS(dseries, year)
    End Sub
    Public Sub RefreshDSPointsByYear(ByVal grid As Object)
        Dim db As New PMPointsDB
        Dim s As New My.MySettings
        Dim year As Integer = grid.SelectedText
        For Each ds As PMMetricsDataSeriesPoints In _dataSeriesbyMonth
            If Not IsNothing(ds.DataSeries.DSID) Then
                ds.Points = db.GetUPMPoints(ds.DataSeries.DSID, year)
            End If
        Next

    End Sub

#End Region
    Public Class PMMetricsDataSeriesPoints
        Implements ICloneable

        Private _seriesbyMonth As DataSeriesDTO
        Private _points As List(Of UPMPointsDTO)

        Public Sub New()
            _seriesbyMonth = New DataSeriesDTO
            _points = New List(Of UPMPointsDTO)
        End Sub
        Public Sub New(ByVal series As DataSeriesDTO, ByVal year As Integer)
            Dim Pointslist As List(Of UPMPointsDTO)
            Dim s As New My.MySettings
            Dim db As New PMPointsDB
            Dim point As UPMPointsDTO
            _seriesbyMonth = series
            _points = New List(Of UPMPointsDTO)
            Pointslist = db.GetUPMPointsDS(series.DSID, year, series.month)
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
            Dim newDTO As New PMMetricsDataSeriesPoints
            newDTO.DataSeries = _seriesbyMonth.Clone
            Dim tmpPoints As New List(Of UPMPointsDTO)
            For Each dto As UPMPointsDTO In _points
                tmpPoints.Add(dto.Clone)
            Next
            newDTO.Points = tmpPoints
            Return newDTO
        End Function
    End Class
End Class
