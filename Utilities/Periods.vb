Public Class Periods
    Public Shared Function GetQuarter(ByVal month As String) As String
        Dim ret As String = String.Empty
        Select Case month.ToUpper
            Case "JAN"
                ret = "QTR1"
            Case "FEB"
                ret = "QTR1"
            Case "MAR"
                ret = "QTR1"
            Case "APR"
                ret = "QTR2"
            Case "MAY"
                ret = "QTR2"
            Case "JUN"
                ret = "QTR2"
            Case "JUL"
                ret = "QTR3"
            Case "AUG"
                ret = "QTR3"
            Case "SEP"
                ret = "QTR3"
            Case "OCT"
                ret = "QTR4"
            Case "NOV"
                ret = "QTR4"
            Case "DEC"
                ret = "QTR4"
        End Select
        Return ret
    End Function
    Public Shared Function GetHalf(ByVal month As String) As String
        Dim ret As String = String.Empty
        Select Case month.ToUpper
            Case "JAN"
                ret = "HALF1"
            Case "FEB"
                ret = "HALF1"
            Case "MAR"
                ret = "HALF1"
            Case "APR"
                ret = "HALF1"
            Case "MAY"
                ret = "HALF1"
            Case "JUN"
                ret = "HALF1"
            Case "JUL"
                ret = "HALF2"
            Case "AUG"
                ret = "HALF2"
            Case "SEP"
                ret = "HALF2"
            Case "OCT"
                ret = "HALF2"
            Case "NOV"
                ret = "HALF2"
            Case "DEC"
                ret = "HALF2"
        End Select
        Return ret
    End Function
End Class
