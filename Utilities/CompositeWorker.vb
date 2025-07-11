Imports Entergy.PerformanceManagement.PMDB

Public Class CompositeWorker
    Public Enum Methods
        Sum = 1
        WeightedAverage = 2
        ZeroToTwoHundredWeightedAverage = 3
    End Enum
    Private Enum TargetTypes
        Monthly = 0
        YTD
        PE
    End Enum
    Private _method As Methods
    Private _cID As Integer
    Public ReadOnly Property ComputationMethod() As Methods
        Get
            Return _method
        End Get
    End Property
    Public Sub New(ByVal cID As Integer)
        _cID = cID
        _method = Me.GetCompositeMethod(cID)
    End Sub
#Region "Actuals Computation"
    Public Function ComputeActuals(ByVal month As String, ByVal year As Integer) As Boolean
        Dim db As New MetricDB
        Dim monthly As Boolean = db.IsMonthly(_cID)
        Dim ret As Boolean = False
        ret = Me.ComputeYTDActuals(month, year)
        If monthly Then
            ret = ret And Me.ComputeMonthlyActuals(month, year)
        End If
        Return ret
    End Function
    Private Function ComputeYTDActuals(ByVal month As String, ByVal year As Integer) As Boolean
        If Me.VerifyActuals(_cID, month, year, TargetTypes.YTD) Then
            Dim comp As CompositeMetric = Me.CreateCompositeMetric(_cID, month, year, TargetTypes.YTD)
            Dim points As List(Of UPMPointsDTO) = ComputePoints(comp, TargetTypes.YTD)
            'saves point
            SavePoints(points)
            'saves detail history
            SaveComposite(comp)
            Return True
        End If

    End Function

    Public Function ComputePEActuals(ByVal month As String, ByVal year As Integer) As Boolean
        If Me.VerifyActuals(_cID, month, year, TargetTypes.YTD) Then
            Dim comp As CompositeMetric = Me.CreateCompositeMetric(_cID, month, year, TargetTypes.PE)
            Dim points As List(Of UPMPointsDTO) = ComputePoints(comp, TargetTypes.PE)
            'saves point
            SavePoints(points)
            'saves detail history
            SaveComposite(comp)
            Return True
        End If

    End Function
    Private Function ComputeMonthlyActuals(ByVal month As String, ByVal year As Integer) As Boolean
        If Me.VerifyActuals(_cID, month, year, TargetTypes.Monthly) Then
            Dim comp As CompositeMetric = Me.CreateCompositeMetric(_cID, month, year, TargetTypes.Monthly)
            Dim points As List(Of UPMPointsDTO) = ComputePoints(comp, TargetTypes.Monthly)
            'saves point
            SavePoints(points)
            'saves detail history
            SaveComposite(comp)
            Return True
        End If

    End Function
#End Region
#Region "Target Computation"
    Public Sub ComputeTargets(ByVal year As Integer)
        Dim db As New MetricDB
        Dim monthly As Boolean = db.IsMonthly(_cID)
        Me.ComputeYTDTargets(year)
        If monthly Then
            Me.ComputeMonthlyTargets(year)
        End If
    End Sub
    Private Sub ComputeMonthlyTargets(ByVal year As Integer)
        If Me.VerifyTargets(_cID, year, TargetTypes.Monthly) = True Then
            ComputeTargetValues(_cID, year, TargetTypes.Monthly)
        End If
    End Sub
    Private Sub ComputeYTDTargets(ByVal year As Integer)
        If Me.VerifyTargets(_cID, year, TargetTypes.YTD) = True Then
            ComputeTargetValues(_cID, year, TargetTypes.YTD)
        End If
    End Sub
    Private Sub ComputeTargetValues(ByVal cID As Integer, ByVal year As Integer, ByVal ytd_monthly As TargetTypes)
        Dim weights As List(Of MetricCompositesDTO) = Me.GetCompositeComponents(cID)
        For Each ds As String In Me.GetTargetDataSeries(ytd_monthly)
            SavePoints(Me.ComputePoints(cID, ds, year))
        Next
    End Sub
    Private Function ComputePoints(ByVal cID As Integer, ByVal dataseries As String, ByVal year As Integer) As List(Of UPMPointsDTO)
        Dim parentPoints As List(Of UPMPointsDTO) = GetCompositePoints(cID, dataseries, year)
        Dim sum As Dictionary(Of Integer, Decimal) = CreatePointDictionary(parentPoints)
        For Each child As MetricCompositesDTO In Me.GetCompositeComponents(cID)
            For Each dto As UPMPointsDTO In GetCompositePoints(child.MadeOfCID, dataseries, year)
                sum(dto.MonthValue) = ComputeTargets(sum(dto.MonthValue), dto, child.Weight)
            Next
        Next
        UpdatePoints(parentPoints, sum)
        Return parentPoints
    End Function
#End Region
#Region "Composite Object"
    Public Function GetCompositeMetricDetail(ByVal month As String, ByVal year As Integer) As CompositeMetric
        Dim com As New CompositeMetric
        com.CompositeMetric = Me.GetDetailParent(_cID, month, year)
        com.ComponentMetrics = Me.GetDetailChildren(_cID, month, year)
        Return com
    End Function
    Private Function CreateCompositeMetric(ByVal cid As Integer, ByVal month As String, ByVal year As Integer, ByVal ytd_monthly As TargetTypes) As CompositeMetric
        Dim c As List(Of MetricCompositesDTO) = Me.VerifyComponents(cid)
        Dim composite As New CompositeMetric
        composite.CompositeMetric = Me.CreateCompositeDTO(cid, month, year, ytd_monthly, 1, 0, 0)
        For Each child As MetricCompositesDTO In c
            composite.ComponentMetrics.Add(Me.CreateCompositeDTO(child.MadeOfCID, month, year, ytd_monthly, child.Weight, cid, child.Sort))
        Next
        Return composite
    End Function
    Private Function CreateCompositeDTO(ByVal cID As Integer, ByVal month As String, ByVal year As Integer, ByVal ytd_monthly As TargetTypes, ByVal weight As Decimal, ByVal parentCID As Integer, ByVal order As Integer) As MetricCompositeDetailDTO
        Dim dto As New MetricCompositeDetailDTO

        dto.ReportingMonth = month
        dto.ReportingYear = year
        dto.CID = cID
        dto.ParentCID = parentCID
        dto.Weight = weight
        dto.InverseScale = Me.GetInverseScale(cID)
        dto.DetailSort = order
        Select Case ytd_monthly
            Case TargetTypes.YTD
                If parentCID > 0 Then dto.ActualValue = GetCompositePoint(cID, "YTD", month, year)
                dto.Minimum = GetCompositePoint(cID, "Minimum", month, year)
                dto.Target = GetCompositePoint(cID, "Target", month, year)
                dto.Maximum = GetCompositePoint(cID, "Maximum", month, year)
                dto.YTDorMonthly = "YTD"
            Case TargetTypes.Monthly
                If parentCID > 0 Then dto.ActualValue = GetCompositePoint(cID, "Monthly", month, year)
                dto.Minimum = GetCompositePoint(cID, "Monthly Minimum", month, year)
                dto.Target = GetCompositePoint(cID, "Monthly Target", month, year)
                dto.Maximum = GetCompositePoint(cID, "Monthly Maximum", month, year)
                dto.YTDorMonthly = "Monthly"
            Case TargetTypes.PE
                If parentCID > 0 Then dto.ActualValue = GetCompositePoint(cID, "PE", month, year)
                dto.Minimum = GetCompositePoint(cID, "Minimum", "DEC", year)
                dto.Target = GetCompositePoint(cID, "Target", "DEC", year)
                dto.Maximum = GetCompositePoint(cID, "Maximum", "DEC", year)
                dto.YTDorMonthly = "PE"
        End Select

        If parentCID > 0 Then
            dto.Score = Me.ComputeScore(dto, ytd_monthly)
            dto.WeightedScore = Me.ComputeWeightedScore(dto, dto.Weight, ytd_monthly)
            dto.PerformanceLevel = Me.GetPerformanceLevel(dto.Minimum, dto.Target, dto.Maximum, dto.ActualValue, dto.InverseScale)
        End If
        Return dto
    End Function
#End Region
#Region "Calculate Points"
    Private Function ComputePoints(ByRef composite As CompositeMetric, ByVal ytd_monthly As TargetTypes) As List(Of UPMPointsDTO)
        Dim totalscore As Decimal
        Dim totalweight As Decimal
        Dim totalweightedscore As Decimal
        For Each child As MetricCompositeDetailDTO In composite.ComponentMetrics
            totalscore = Decimal.Add(totalscore, child.Score)
            totalweight = Decimal.Add(totalweight, child.Weight)
            totalweightedscore = Decimal.Add(totalweightedscore, child.WeightedScore)
        Next
        composite.CompositeMetric.Score = totalweightedscore
        composite.CompositeMetric.Weight = totalweight
        composite.CompositeMetric.WeightedScore = totalweightedscore
        If _method = Methods.ZeroToTwoHundredWeightedAverage Then
            composite.CompositeMetric.ActualValue = Decimal.Round(totalweightedscore, 0, MidpointRounding.AwayFromZero)
        Else
            composite.CompositeMetric.ActualValue = totalweightedscore
        End If
        composite.CompositeMetric.PerformanceLevel = Me.GetPerformanceLevel(composite.CompositeMetric.Minimum, composite.CompositeMetric.Target, composite.CompositeMetric.Maximum, composite.CompositeMetric.ActualValue, composite.CompositeMetric.InverseScale)

        Dim parent As List(Of UPMPointsDTO) = GetCompositePoints(composite.CompositeMetric.CID, Me.GetActualDataSeries(ytd_monthly)(0), composite.CompositeMetric.ReportingMonth, composite.CompositeMetric.ReportingYear)
        If parent.Count = 1 Then
            parent(0).YValue = composite.CompositeMetric.ActualValue
        End If

        Return parent
    End Function
#End Region
#Region "Calculations"
    Private Function ComputeScore(ByVal dto As MetricCompositeDetailDTO, ByVal ytd_monthly As TargetTypes) As Decimal
        Dim ret As Decimal = Decimal.Zero
        Select Case _method
            Case Methods.Sum, Methods.WeightedAverage
                ret = dto.ActualValue
            Case Methods.ZeroToTwoHundredWeightedAverage
                ret = Decimal.Round(Me.Get0To200Scale(dto.Minimum, dto.Target, dto.Maximum, dto.ActualValue, dto.InverseScale), 0, MidpointRounding.AwayFromZero)
            Case Else
                Throw New Exception("Set the composite metric calculation method.")
        End Select
        Return ret
    End Function
    Private Function ComputeWeightedScore(ByVal dto As MetricCompositeDetailDTO, ByVal weight As Decimal, ByVal ytd_monthly As TargetTypes) As Decimal
        Dim ret As Decimal = Decimal.Zero
        Select Case _method
            Case Methods.Sum
                ret = dto.ActualValue
            Case Methods.ZeroToTwoHundredWeightedAverage, Methods.WeightedAverage
                ret = Decimal.Multiply(ComputeScore(dto, ytd_monthly), weight)
            Case Else
                Throw New Exception("Set the composite metric calculation method.")
        End Select
        Return ret
    End Function
    Private Function ComputeTargets(ByVal cumulative As Decimal, ByVal dto As UPMPointsDTO, ByVal weight As Decimal) As Decimal
        Dim ret As Decimal = Decimal.Zero
        Select Case _method
            Case Methods.Sum
                ret = Decimal.Add(cumulative, dto.YValue)
            Case Methods.WeightedAverage
                ret = Decimal.Add(cumulative, Decimal.Multiply(dto.YValue, weight))
            Case Methods.ZeroToTwoHundredWeightedAverage
                'determine the data series
                Select Case dto.DataSeriesDescription.ToUpper
                    Case "TARGET", "MONTHLY TARGET"
                        ret = 100
                    Case "MINIMUM", "MONTHLY MINIMUM"
                        ret = 25
                    Case "MAXIMUM", "MONTHLY MAXIMUM"
                        ret = 200
                    Case "YTD", "MONTHLY", "PE"
                        ret = Decimal.Add(cumulative, Decimal.Multiply(Get0To200Scale(dto.C_ID, dto.PointsDescription.ToUpper, dto.ReportingYear, dto.DataSeriesDescription.ToUpper), weight))
                End Select
            Case Else
                Throw New Exception("Set the composite metric calculation method.")
        End Select
        Return ret
    End Function
    Private Function CreatePointDictionary(ByVal data As List(Of UPMPointsDTO)) As Dictionary(Of Integer, Decimal)
        Dim dict As New Dictionary(Of Integer, Decimal)
        For Each item As UPMPointsDTO In data
            dict.Add(item.MonthValue, 0.0)
        Next
        Return dict
    End Function
    Private Sub UpdatePoints(ByRef data As List(Of UPMPointsDTO), ByVal values As Dictionary(Of Integer, Decimal))
        For Each item As UPMPointsDTO In data
            item.YValue = values(item.MonthValue)
        Next
    End Sub
    'Private Function WeightedPoints(ByVal weights As List(Of ChartCompositesDTO), ByVal dataSeries As String) As List(Of UPMPointsDTO)
    '    If weights.Count = 0 Then Return Nothing
    '    Dim p As New PMGetPoints
    '    Dim baseLine As List(Of UPMPointsDTO) = p.GetPoints(weights(0).CID, dataSeries, _year)
    '    Dim sum As Dictionary(Of Integer, Decimal) = CreatePointDictionary(baseLine)
    '    For Each item As ChartCompositesDTO In weights
    '        Dim list As List(Of UPMPointsDTO) = p.GetPoints(item.MadeOfCID, dataSeries, _year)
    '        For Each dto As UPMPointsDTO In list
    '            sum(dto.XValue) = Decimal.Add(sum(dto.XValue), dto.YValue)
    '        Next
    '    Next
    '    UpdatePoints(baseLine, sum)
    '    Return baseLine
    'End Function
#End Region
#Region "Verification"
    Private Function VerifyTargets(ByVal cID As Integer, ByVal year As Integer, ByVal ytd_monthly As TargetTypes) As Boolean
        Dim comp As List(Of MetricCompositesDTO) = Me.VerifyComponents(cID)
        For Each component As MetricCompositesDTO In comp
            For Each dataseries As String In Me.GetTargetDataSeries(ytd_monthly)
                VerifyPoints(component.MadeOfCID, dataseries, year)
                VerifyFrequency(cID, component.CID, dataseries, year)
            Next
        Next
        Return True
    End Function
    Private Function VerifyActuals(ByVal cID As Integer, ByVal month As String, ByVal year As Integer, ByVal ytd_monthly As TargetTypes) As Boolean
        Dim comp As List(Of MetricCompositesDTO) = Me.VerifyComponents(cID)
        For Each component As MetricCompositesDTO In comp
            For Each dataseries As String In Me.GetActualDataSeries(ytd_monthly)
                VerifyPoints(component.MadeOfCID, dataseries, month, year)
                VerifyFrequency(cID, component.CID, dataseries, year)
            Next
        Next
        Return True
    End Function
    'Private Function Verify(ByVal cID As Integer, ByVal dataseries As String, ByVal year As Integer)
    '    Return Verify(cID, dataseries, String.Empty, year)
    'End Function
    'Private Function Verify(ByVal cID As Integer, ByVal dataseries As String, ByVal month As String, ByVal year As Integer)

    '    For Each item As ChartCompositesDTO In list
    '        Me.VerifyFrequency(cID, item.CID, dataseries, year)
    '    Next
    '    For Each item As ChartCompositesDTO In list
    '        Me.VerifyPoints(cID, dataseries, month, year)
    '    Next
    '    Return True
    'End Function
    Private Function VerifyComponents(ByVal cID As Integer) As List(Of MetricCompositesDTO)
        Dim list As List(Of MetricCompositesDTO) = Me.GetCompositeComponents(cID)
        If list.Count = 0 Then
            Throw New Exception("The composite metric has no weights. Please verify. If weights just entered, please save chart and try again.")
        End If
        Return list
    End Function
    Public Function VerifyFrequency(ByVal madeOfCID As Integer, ByVal year As Integer) As Boolean
        If Me.GetChartPeriods(_cID, year).Count <> Me.GetChartPeriods(madeOfCID, year).Count Then
            Return False
        End If
        Return True
    End Function
    Private Function VerifyFrequency(ByVal cID As Integer, ByVal madeOfCID As Integer, ByVal dataSeries As String, ByVal year As Integer) As Boolean
        If Me.GetChartPeriods(cID, year).Count <> Me.GetChartPeriods(madeOfCID, year).Count Then
            Throw New Exception("Not all charts have the same point frequency. Please verify.")
        End If
        Return True
    End Function
    Private Function VerifyPoints(ByVal madeOfCID As Integer, ByVal dataSeries As String, ByVal year As Integer) As Boolean
        Return Me.VerifyPoints(madeOfCID, dataSeries, String.Empty, year)
    End Function
    Private Function VerifyPoints(ByVal madeOfCID As Integer, ByVal dataSeries As String, ByVal month As String, ByVal year As Integer) As Boolean
        Dim verify As List(Of UPMPointsDTO)
        If month = String.Empty Then
            verify = GetCompositePoints(madeOfCID, dataSeries, year)
        Else
            verify = GetCompositePoints(madeOfCID, dataSeries, month, year)
        End If
        'For Each item As UPMPointsDTO In verify
        'If IsNothing(item.YValue) Then
        'Throw New Exception("The data series: " + dataSeries + " for chart: " + item.MetricID + " for period: " + item.PointsDescription + " is missing. All values must be entered before computing the composites values.")
        'End If
        'Next
        Return True
    End Function
    Private Function GetCompositePoint(ByVal madeOfCID As Integer, ByVal dataSeries As String, ByVal month As String, ByVal year As Integer) As Nullable(Of Decimal)
        Dim list As List(Of UPMPointsDTO) = Me.GetCompositePoints(madeOfCID, dataSeries, month, year)
        If list.Count = 1 Then
            Return list(0).YValue
        Else
            Return Nothing
        End If
    End Function
    Private Function GetCompositePoints(ByVal madeOfCID As Integer, ByVal dataSeries As String, ByVal year As Integer) As List(Of UPMPointsDTO)
        Dim p As New PMGetPoints
        Dim points As List(Of UPMPointsDTO) = p.GetPoints(madeOfCID, dataSeries, year)
        ReplaceWithTarget(dataSeries, points)
        Return points
    End Function
    Private Function GetCompositePoints(ByVal madeOfCID As Integer, ByVal dataSeries As String, ByVal month As String, ByVal year As Integer) As List(Of UPMPointsDTO)
        Dim p As New PMGetPoints
        Dim points As List(Of UPMPointsDTO) = p.GetPoints(madeOfCID, dataSeries, month, year)
        ReplaceWithTarget(dataSeries, points)
        Return points
    End Function
    Private Sub ReplaceWithTarget(ByVal dataSeries As String, ByRef points As List(Of UPMPointsDTO))
        Dim tar As String = String.Empty
        Dim p As New PMGetPoints
        Dim db As New PMMetricsDB
        If dataSeries.ToUpper = "YTD" Or dataSeries.ToUpper = "MONTHLY" Or dataSeries.ToUpper = "PE" Then
            Select Case dataSeries.ToUpper
                Case "YTD", "PE"
                    tar = "Target"
                Case "MONTHLY"
                    tar = "Monthly Target"
            End Select
            For Each item As UPMPointsDTO In points
                If IsNothing(item.YValue) Then
                    Dim dto As MetricDTO = db.GetMetric(item.MetricID)
                    If dto.IncentiveShowAtTarget Then
                        Dim replace As List(Of UPMPointsDTO) = p.GetPoints(item.C_ID, tar, item.PointsDescription, item.ReportingYear)
                        If replace.Count = 1 Then
                            item.YValue = replace(0).YValue
                        End If
                    End If
                End If
            Next
        End If
    End Sub
#End Region
#Region "Metric Details"
    Private Function GetChartPeriods(ByVal cID As Integer, ByVal year As Integer) As List(Of String)
        Dim ret As New List(Of String)
        For Each item As UPMPointsDTO In GetCompositePoints(cID, "YTD", year)
            ret.Add(item.PointsDescription)
        Next
        Return ret
    End Function
    Private Function GetTargetDataSeries(ByVal targetType As TargetTypes) As List(Of String)
        Dim list As New List(Of String)
        If targetType = TargetTypes.YTD Then
            list.Add("MINIMUM")
            list.Add("TARGET")
            list.Add("MAXIMUM")
        ElseIf targetType = TargetTypes.PE Then
            list.Add("MINIMUM")
            list.Add("TARGET")
            list.Add("MAXIMUM")
        Else
            list.Add("Monthly Minimum")
            list.Add("Monthly Target")
            list.Add("Monthly Maximum")
        End If
        Return list
    End Function
    Private Function GetActualDataSeries(ByVal targetType As TargetTypes) As List(Of String)
        Dim list As New List(Of String)
        If targetType = TargetTypes.YTD Then
            list.Add("YTD")
        ElseIf targetType = TargetTypes.PE Then
            list.Add("PE")
        Else
            list.Add("Monthly")
        End If
        Return list
    End Function
    'Private Function CreatePeriodsByMonth(ByVal month As String) As List(Of String)
    '    Dim list As New List(Of String)
    '    list.Add(month.ToUpper)
    '    list.Add(Periods.GetQuarter(month))
    '    list.Add(Periods.GetHalf(month))
    '    Return list
    'End Function
#End Region
#Region "Database Access"

    Private Sub SaveComposite(ByVal composite As CompositeMetric)
        Dim dto As MetricCompositeDetailDTO = composite.CompositeMetric
        Dim db As New PMMetricsDB
        db.DeleteUPMMetricCompositeDetail(dto)
        db.SaveUPMChartCompositeDetail(dto)
        For Each child As MetricCompositeDetailDTO In composite.ComponentMetrics
            db.SaveUPMChartCompositeDetail(child)
        Next
    End Sub
    Private Function GetInverseScale(ByVal cID As Integer) As String
        Dim db As New PMMetricsDB
        Dim dto As MetricDTO = db.GetMetric(cID)
        Return dto.InverseScale
    End Function
    Private Function GetCompositeMethod(ByVal cID As Integer) As Methods
        Dim db As New PMMetricsDB
        Dim dto As MetricDTO = db.GetMetric(cID)
        Return dto.CompositeMethod
    End Function
    Private Function GetPerformanceLevel(ByVal min As Nullable(Of Decimal), ByVal tar As Nullable(Of Decimal), ByVal max As Nullable(Of Decimal), ByVal actual As Nullable(Of Decimal), ByVal inverseScale As String) As String
        Dim db As New PMMetricsDB
        If Not IsNothing(min) And Not IsNothing(tar) And Not IsNothing(max) And Not IsNothing(actual) Then
            Return db.GetPerformanceLevel(min, tar, max, actual, inverseScale)
        Else
            Return "NG"
        End If
    End Function
    Private Function GetDetailParent(ByVal cID As Integer, ByVal month As String, ByVal year As Integer) As MetricCompositeDetailDTO
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricCompositeDetailDTO) = db.GetUPMMetricCompositeDetailParent(cID, month, year)
        If list.Count = 1 Then
            Return list(0)
        Else
            Return Nothing
        End If
    End Function
    Private Function GetDetailChildren(ByVal parentCID As Integer, ByVal month As String, ByVal year As Integer) As List(Of MetricCompositeDetailDTO)
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricCompositeDetailDTO) = db.GetUPMMetricCompositeDetailChildren(parentCID, month, year)
        Return list
    End Function
    Private Function Get0To200Scale(ByVal min As Decimal, ByVal target As Decimal, ByVal max As Decimal, ByVal actual As Decimal, ByVal inverse As String) As Decimal
        Dim db As New PMMetricsDB
        Dim ret As Decimal
        ret = db.Get0to200Scale(min, target, max, actual, inverse)
        Return ret
    End Function
    Private Function Get0To200Scale(ByVal cid As Integer, ByVal month As String, ByVal year As Integer, ByVal ytd_monthly As String) As Decimal
        Dim db As New PMMetricsDB
        Dim ret As Decimal
        ret = db.Get0to200Scale(cid, month, year, ytd_monthly)
        Return ret
    End Function
    Private Function GetCompositeComponents(ByVal cID As Integer) As List(Of MetricCompositesDTO)
        Dim db As New PMMetricsDB
        Return db.GetUPMMetricComposites(cID)
    End Function
    Private Sub SavePoints(ByVal data As List(Of UPMPointsDTO))
        Dim db As New PMPointsDB
        For Each item As UPMPointsDTO In data
            db.SaveUPMPoints(item)
        Next
    End Sub
#End Region
End Class
