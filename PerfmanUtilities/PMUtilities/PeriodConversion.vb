Public Class PeriodConversion
    Public Function GetQuarter(ByVal month As String, ByVal year As Integer) As CriteriaDTO
        Dim period As New CriteriaDTO
        Select Case month
            Case "Dec"
                period.ReportingPeriod = "QTR4"
                period.ReportingYear = year
                'Case "Jan", "Feb"
                '    period.ReportingPeriod = "QTR4"
                '    period.ReportingYear = year - 1
            Case "Jan", "Feb", "Mar", "Apr", "May"
                period.ReportingPeriod = "QTR1"
                period.ReportingYear = year
            Case "Jun", "Jul", "Aug"
                period.ReportingPeriod = "QTR2"
                period.ReportingYear = year
            Case "Sep", "Oct", "Nov"
                period.ReportingPeriod = "QTR3"
                period.ReportingYear = year
        End Select
        Return period
    End Function
    Public Function GetHalf(ByVal month As String, ByVal year As Integer) As CriteriaDTO
        Dim period As New CriteriaDTO
        Select Case month
            Case "Dec"
                period.ReportingPeriod = "HALF2"
                period.ReportingYear = year
                'Case "Jan", "Feb", "Mar", "Apr", "May"
                '    period.ReportingPeriod = "HALF2"
                '    period.ReportingYear = year - 1
            Case "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov"
                period.ReportingPeriod = "HALF1"
                period.ReportingYear = year
        End Select
        Return period
    End Function
End Class