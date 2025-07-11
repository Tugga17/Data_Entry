Imports Entergy.PerformanceManagement.PMDB

Public Class CompositeMetricDB
    Public Function GetCompositeCalculationOrder() As List(Of Integer)
        Dim db As New PMMetricsDB
        Return db.GetUPMMetricCompositeCalcOrder
    End Function
    Public Sub ComputeAllMetrics(ByVal month As String, ByVal year As Integer)
        Dim order As List(Of Integer) = GetCompositeCalculationOrder()
        For Each item As Integer In order
            Dim w As New CompositeWorker(item)
            w.ComputeActuals(month, year)
        Next
    End Sub
End Class
