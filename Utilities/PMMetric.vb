Imports Entergy.PerformanceManagement.PMDTO
Imports Entergy.PerformanceManagement.PMDB
Imports Telerik.WinControls

Public Class PMMetric
#Region "Privates"
    Private _metric As MetricDTO
    Private _dataSeries As List(Of PMMetricDataSeries)
    Private _compComp As List(Of MetricCompositesDTO)

    Public Property CompositeComponents() As List(Of MetricCompositesDTO)
        Get
            Return _compComp
        End Get
        Set(ByVal value As List(Of MetricCompositesDTO))
            _compComp = value
        End Set
    End Property
    Public Sub CopyMetricData(ByVal chart As MetricDTO)
        Me.PopulateMetricData(chart)
        Dim input1 As New PMEditInputBox
        Dim input2 As New PMEditInputBox
        Dim newID As String
        Dim newName As String
        input1.Label = "New Metric ID"
        input1.Title = "Copy Metric"
        input1.MaximumInputLength = 25

        input2.Label = "New Metric Name"
        input2.Title = "Copy Metric"
        input2.MaximumInputLength = 50

        If input1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            newID = input1.Value
            If input2.ShowDialog = Windows.Forms.DialogResult.OK Then
                newName = input2.Value
                Me.ChangeMetricIDS(newID, newName)
                Me.ClearYValues()
                Me.ClearCompositeValues()

                'Get rid of Inverse scale to make them set it.
                'Me.MetricData.InverseScale = String.Empty
                Me.ClearNotCopiedValues()
                Me.MetricData.CommentNotRequired = 0
                Me.MetricData.ConfidentialMeasure = 0
                Me.MetricData.CreatedBy = Environment.UserName
                Me.MetricData.IncentiveMeasure = 0
                Me.MetricData.IncentiveShowAtTarget = 0
                Me.ClearIDs()

                Me.SaveMetric(Date.Now.Year)
                Dim edit As New PMEditMetric
                edit.Metric = Me
                edit.ShowDialog()
            End If
        End If
    End Sub
    Private Sub ClearCompositeValues()
        Me.MetricData.CompositeMetric = False
    End Sub
    Private Sub ClearNotCopiedValues()
        'Me.MetricData.LexiconID = Nothing
        'For Each series As PMMetricDataSeries In _dataSeries
        '    'series.DataSeries.SpecialLabel = String.Empty
        '    series.DataSeries.FormatID = Nothing
        'Next
    End Sub
    Private Sub ChangeMetricIDS(ByVal metricID As String, ByVal MetricName As String)
        Me.MetricData.MetricID = metricID
        Me.MetricData.MetricName = MetricName
        'Me.MetricData.Description = MetricName
        Me.MetricData.ModificationType = IModificationIndicator.ModificationTypes.Added
        For Each series As PMMetricDataSeries In _dataSeries
            series.DataSeries.MetricID = metricID
            series.DataSeries.ModificationType = IModificationIndicator.ModificationTypes.Added
            For Each point As UPMPointsDTO In series.Points
                point.MetricID = metricID
                point.ModificationType = IModificationIndicator.ModificationTypes.Added
            Next
        Next
    End Sub
    Private Sub ClearIDs()
        _metric.CID = Nothing
        For Each s As PMMetricDataSeries In _dataSeries
            s.DataSeries.CID = Nothing
            s.DataSeries.DSID = Nothing
            For Each point As UPMPointsDTO In s.Points
                point.DS_ID = Nothing
                point.C_ID = Nothing
            Next
        Next
    End Sub
    Private Sub ClearYValues()
        For Each series As PMMetricDataSeries In _dataSeries
            For Each point As UPMPointsDTO In series.Points
                point.YValue = Nothing
            Next
        Next
    End Sub
    Public Function GetDataSeries(ByVal dataSeries As Integer) As PMMetricDataSeries
        Dim series As PMMetricDataSeries = Nothing

        For Each ds As PMMetricDataSeries In _dataSeries
            If ds.DataSeries.DataSeries = dataSeries Then series = ds
        Next
        Return series
    End Function
    Public Function GetDataSeriesByDSID(ByVal dsid As Integer) As PMMetricDataSeries
        Dim series As PMMetricDataSeries = Nothing

        For Each ds As PMMetricDataSeries In _dataSeries
            If ds.DataSeries.DSID = dsid Then series = ds
        Next
        Return series
    End Function
    Private Sub PrivateAddDataSeries(ByVal dataSeries As PMMetric.PMMetricDataSeries, ByVal year As Integer)
        dataSeries.DataSeries.MetricID = Me.MetricData.MetricID
        Dim s As New My.MySettings
        For Each point As UPMPointsDTO In dataSeries.Points
            point.MetricID = Me.MetricData.MetricID
            point.ReportingYear = year
            point.DataSeriesDescription = dataSeries.DataSeries.Description
        Next
        _dataSeries.Add(dataSeries)
    End Sub
    Private Sub SaveDataSeries(ByVal year As Integer)
        'Need to delete series and points first
        Dim pointsList As List(Of UPMPointsDTO)
        Dim dbDS As New PMDataSeriesDB
        Dim dbP As New PMPointsDB

        Dim s As New My.MySettings
        Dim seriesList As List(Of DataSeriesDTO) = dbDS.GetUPMDataSeries(_metric.CID)
        For Each dtoSeries As DataSeriesDTO In seriesList
            pointsList = dbP.GetUPMPoints(dtoSeries.DSID, year) ', dtoSeries.Description)
            For Each dtoPoints As UPMPointsDTO In pointsList
                dbP.DeleteUPMPoints(dtoPoints)
            Next
            dbDS.DeleteUPMDataSeries(dtoSeries)
        Next

        For Each series As PMMetricDataSeries In _dataSeries
            dbDS.SaveUPMDataSeries(series.DataSeries)
            For Each point As UPMPointsDTO In series.Points
                dbP.SaveUPMPoints(point)
            Next
        Next
    End Sub
    Private Sub PopulateMetricData(ByVal metric As MetricDTO)
        Dim db As New PMDataSeriesDB
        Dim ds As PMMetricDataSeries
        Dim s As New My.MySettings
        Dim list As List(Of DataSeriesDTO)
        'Dim incDB As New PMIncentiveDB
        'Dim inc As List(Of IncentivesDTO)
        _metric = metric
        list = db.GetUPMDataSeries(metric.CID)
        'inc = incDB.GetIncentive(metric.CID, metric.ReportingYear)
        _dataSeries = New List(Of PMMetricDataSeries)
        For Each dto As DataSeriesDTO In list
            ds = New PMMetricDataSeries(dto, Date.Now.Year)
            _dataSeries.Add(ds)
        Next
    End Sub
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
    Public Sub CopyMetric(ByVal metric As MetricDTO)
        Dim edit As New PMEditMetric
        Me.CopyMetricData(metric)
        'If PMEditInputBox.input1.ShowDialog() = Windows.Forms.DialogResult.OK Then
        'edit.Metric = Me
        'edit.ShowDialog()
        'End If

    End Sub
    Public Sub CopyAndSaveMetric(ByVal metric As MetricDTO, ByVal newMetricID As String, ByVal newMetricName As String, ByVal newLevel As Integer)
        Me.PopulateMetricData(metric)
        Me.MetricData.MetricLevelID = newLevel
        Me.ChangeMetricIDS(newMetricID, newMetricName)
        Me.ClearYValues()
        Me.ClearCompositeValues()
        Me.ClearNotCopiedValues()
        Me.ClearIDs()
        Me.SaveMetric(Date.Now.Year)
    End Sub
    Public Sub EditMetric(ByVal metric As MetricDTO)
        Dim edit As New PMEditMetric
        Me.PopulateMetricData(metric)
        edit.Metric = Me
        edit.ShowDialog()
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
    Public Sub AddDataSeries(ByVal dataSeries As PMMetricDataSeries)
        If IsNothing(_dataSeries) Then
            _dataSeries = New List(Of PMMetricDataSeries)
        End If
        _dataSeries.Add(dataSeries)
    End Sub
    Public Sub SaveMetric(ByVal year As Integer)
        Dim trans As New SaveMetricTransaction(_metric, year)
        trans.AddDataSeriesToTransaction(Me.GetSeriesDataList)
        For Each ds As PMMetricDataSeries In _dataSeries
            trans.AddPointsToTransaction(ds.Points)
        Next
        trans.AddCompositeToTransaction(Me.CompositeComponents)
        trans.PerformTransaction()

    End Sub
    Public Function GetSeriesDataList() As List(Of DataSeriesDTO)
        Dim list As New List(Of DataSeriesDTO)
        For Each series As PMMetricDataSeries In _dataSeries
            list.Add(series.DataSeries)
        Next
        Return list
    End Function
    Public Sub RefreshPoints(ByVal year As Integer)
        Dim db As New PMPointsDB
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
            Dim list As List(Of UPMPointsDTO)
            Dim s As New My.MySettings
            Dim db As New PMPointsDB
            Dim point As UPMPointsDTO
            _series = series
            _points = New List(Of UPMPointsDTO)
            list = db.GetUPMPoints(series.DSID, year) ', series.Description)
            For Each dto As UPMPointsDTO In list
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
End Class
