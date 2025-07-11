Public Class CompositeMetric
    Private _parent As MetricCompositeDetailDTO
    Private _list As New List(Of MetricCompositeDetailDTO)
    Public Property CompositeMetric() As MetricCompositeDetailDTO
        Get
            Return _parent
        End Get
        Set(ByVal value As MetricCompositeDetailDTO)
            _parent = value
        End Set
    End Property
    Public Property ComponentMetrics() As List(Of MetricCompositeDetailDTO)
        Get
            Return _list
        End Get
        Set(ByVal value As List(Of MetricCompositeDetailDTO))
            _list = value
        End Set
    End Property
End Class
