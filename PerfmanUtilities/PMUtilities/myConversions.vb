Public Class myConversions

    Public Shared Function MMMtoMonth(ByVal MMM As String) As String
        Return String.Format("{0:MMMM}", CDate("01 " + MMM + " " + Now.Year.ToString))
    End Function
    Public Shared Function OpCotoTitle(ByVal opCo As String) As String
        Dim title As String = ""
        Select Case opCo.ToUpper
            Case "AR"
                title = "Entergy Arkansas, Inc."
            Case "LA"
                title = "Louisiana - ELL and EGSL"
            Case "GSL"
                title = "Louisiana - EGSL"
            Case "MS"
                title = "Entergy Mississippi, Inc."
            Case "NO"
                title = "Entergy New Orleans, Inc."
            Case "TX"
                title = "Entergy Texas, Inc."
            Case "US"
                title = "Utility Support"
            Case "X"
                title = "Entergy Utility Operations"
        End Select
        Return title
    End Function
    Public Shared Function OpCoList() As List(Of String)
        Dim list As New List(Of String)
        list.Add("AR")
        list.Add("LA")
        list.Add("GSL")
        list.Add("MS")
        list.Add("NO")
        list.Add("TX")
        list.Add("US")
        list.Add("X")
        Return list
    End Function
    Public Shared Function PerformanceList() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Below Min")
        list.Add("Minimum")
        list.Add("Target")
        list.Add("Maximum")
        Return list
    End Function

    Public Shared Function GetLevelBackgroudColor(ByVal level As String) As Drawing.Color
        Select Case level
            Case "Below Min"
                Return Drawing.Color.Red
            Case "Minimum"
                Return Drawing.Color.Yellow
            Case "Target"
                Return Drawing.Color.White
            Case "Maximum"
                Return Drawing.Color.Green
            Case Else
                Return Drawing.Color.Silver
        End Select
    End Function

    Public Shared Function GetLevelFontColor(ByVal level As String) As Drawing.Color
        Select Case level
            Case "Maximum"
                Return Drawing.Color.White
            Case Else
                Return Drawing.Color.Black
        End Select
    End Function

    Public Shared Function GetMonthNumber(ByVal mmm As String) As Integer
        Select Case mmm.ToUpper
            Case "JAN"
                Return 1
            Case "FEB"
                Return 2
            Case "MAR"
                Return 3
            Case "APR"
                Return 4
            Case "MAY"
                Return 5
            Case "JUN"
                Return 6
            Case "JUL"
                Return 7
            Case "AUG"
                Return 8
            Case "SEP"
                Return 9
            Case "OCT"
                Return 10
            Case "NOV"
                Return 11
            Case "DEC"
                Return 12
        End Select
    End Function
    Public Shared Function GetMonthDesc(ByVal month As Integer) As String
        Select Case month
            Case 1
                Return "JAN"
            Case 2
                Return "FEB"
            Case 3
                Return "MAR"
            Case 4
                Return "APR"
            Case 5
                Return "MAY"
            Case 6
                Return "JUN"
            Case 7
                Return "JUL"
            Case 8
                Return "AUG"
            Case 9
                Return "SEP"
            Case 10
                Return "OCT"
            Case 11
                Return "NOV"
            Case 12
                Return "DEC"
        End Select
    End Function
    Public Shared Function GetMonthMMM(ByVal month As Integer) As String
        Select Case month
            Case 1
                Return "Jan"
            Case 2
                Return "Feb"
            Case 3
                Return "Mar"
            Case 4
                Return "Apr"
            Case 5
                Return "May"
            Case 6
                Return "Jun"
            Case 7
                Return "Jul"
            Case 8
                Return "Aug"
            Case 9
                Return "Sep"
            Case 10
                Return "Oct"
            Case 11
                Return "Nov"
            Case 12
                Return "Dec"
            Case Else
                Return String.Empty
        End Select
    End Function

    Public Shared Function MonthList() As List(Of String)
        Dim list As New List(Of String)
        list.Add("Jan")
        list.Add("Feb")
        list.Add("Mar")
        list.Add("Apr")
        list.Add("May")
        list.Add("Jun")
        list.Add("Jul")
        list.Add("Aug")
        list.Add("Sep")
        list.Add("Oct")
        list.Add("Nov")
        list.Add("Dec")
        Return list
    End Function
    Public Shared Function QuarterList() As List(Of String)
        Dim list As New List(Of String)
        list.Add("QTR1")
        list.Add("QTR2")
        list.Add("QTR3")
        list.Add("QTR4")
        Return list
    End Function
    Public Shared Function HalfList() As List(Of String)
        Dim list As New List(Of String)
        list.Add("HALF1")
        list.Add("HALF2")
        Return list
    End Function
    Public Shared Function GetPrevReportingPeriod(ByVal period As String, ByVal year As Integer) As CriteriaDTO
        Dim criteria As New CriteriaDTO
        Select Case period.ToUpper
            Case "JAN"
                criteria.ReportingPeriod = "Dec"
                criteria.ReportingYear = year - 1
            Case "FEB"
                criteria.ReportingPeriod = "Jan"
                criteria.ReportingYear = year
            Case "MAR"
                criteria.ReportingPeriod = "Feb"
                criteria.ReportingYear = year
            Case "APR"
                criteria.ReportingPeriod = "Mar"
                criteria.ReportingYear = year
            Case "MAY"
                criteria.ReportingPeriod = "Apr"
                criteria.ReportingYear = year
            Case "JUN"
                criteria.ReportingPeriod = "May"
                criteria.ReportingYear = year
            Case "JUL"
                criteria.ReportingPeriod = "Jun"
                criteria.ReportingYear = year
            Case "AUG"
                criteria.ReportingPeriod = "Jul"
                criteria.ReportingYear = year
            Case "SEP"
                criteria.ReportingPeriod = "Aug"
                criteria.ReportingYear = year
            Case "OCT"
                criteria.ReportingPeriod = "Sep"
                criteria.ReportingYear = year
            Case "NOV"
                criteria.ReportingPeriod = "Oct"
                criteria.ReportingYear = year
            Case "DEC"
                criteria.ReportingPeriod = "Nov"
                criteria.ReportingYear = year
            Case "QTR1"
                criteria.ReportingPeriod = "QTR4"
                criteria.ReportingYear = year - 1
            Case "QTR2"
                criteria.ReportingPeriod = "QTR1"
                criteria.ReportingYear = year
            Case "QTR3"
                criteria.ReportingPeriod = "QTR2"
                criteria.ReportingYear = year
            Case "QTR4"
                criteria.ReportingPeriod = "QTR3"
                criteria.ReportingYear = year
            Case "HALF1"
                criteria.ReportingPeriod = "HALF2"
                criteria.ReportingYear = year - 1
            Case "HALF2"
                criteria.ReportingPeriod = "HALF1"
                criteria.ReportingYear = year
            Case Else
                criteria.ReportingPeriod = period
                criteria.ReportingYear = year
        End Select
        Return criteria
    End Function
    Public Shared Function GetClosedQuarter(ByVal month As String) As String
        Select Case month.ToUpper
            Case "JAN", "FEB", "MAR"
                Return "QTR4"
            Case "APR", "MAY", "JUN"
                Return "QTR1"
            Case "JUL", "AUG", "SEP"
                Return "QTR2"
            Case "OCT", "NOV", "DEC"
                Return "QTR3"
            Case Else
                Return String.Empty
        End Select
    End Function
    Public Shared Function GetClosedQuarterYear(ByVal month As String, ByVal year As Integer) As Integer
        Select Case month.ToUpper
            Case "JAN", "FEB", "MAR"
                Return year - 1
            Case "APR", "MAY", "JUN"
                Return year
            Case "JUL", "AUG", "SEP"
                Return year
            Case "OCT", "NOV", "DEC"
                Return year
            Case Else
                Return String.Empty
        End Select
    End Function
    Public Shared Function GetClosedHalf(ByVal month As String) As String

        Select Case month.ToUpper
            Case "JAN", "FEB", "MAR", "APR", "MAY", "JUN"
                Return "HALF2"
            Case "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
                Return "HALF1"
            Case Else
                Return String.Empty
        End Select

    End Function

    Public Shared Function GetClosedHalfYear(ByVal month As String, ByVal year As Integer) As Integer

        Select Case month.ToUpper
            Case "JAN", "FEB", "MAR", "APR", "MAY", "JUN"
                Return year - 1
            Case "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
                Return year
            Case Else
                Return String.Empty
        End Select

    End Function
    Public Shared Function GetCurrentQuarter(ByVal month As String) As String
        Select Case month.ToUpper
            Case "JAN", "FEB", "MAR"
                Return "QTR1"
            Case "APR", "MAY", "JUN"
                Return "QTR2"
            Case "JUL", "AUG", "SEP"
                Return "QTR3"
            Case "OCT", "NOV", "DEC"
                Return "QTR4"
            Case Else
                Return String.Empty
        End Select
    End Function

    Public Shared Function GetQuartersLastMonth(ByVal qtr As String) As String
        Select Case qtr.ToUpper
            Case "QTR1"
                Return "MAR"
            Case "QTR2"
                Return "JUN"
            Case "QTR3"
                Return "SEP"
            Case "QTR4"
                Return "DEC"
            Case Else
                Return String.Empty
        End Select
    End Function


    Public Shared Function GetCurrentHalf(ByVal month As String) As String

        Select Case month.ToUpper
            Case "JAN", "FEB", "MAR", "APR", "MAY", "JUN"
                Return "HALF1"
            Case "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"
                Return "HALF2"
            Case Else
                Return String.Empty
        End Select

    End Function
End Class
