Imports Entergy.PerformanceManagement.PMDTO
Imports Entergy.PerformanceManagement.PMDB
Public Class PMLexicon
    Private _lexicon As LexiconDTO

#Region "Public"
    Public Property LexiconData() As LexiconDTO
        Get
            Return _lexicon
        End Get
        Set(ByVal value As LexiconDTO)
            _lexicon = value
        End Set
    End Property
    Public Property LexiconOwner As LexiconDTO
        Get
            Return _lexicon
        End Get
        Set(ByVal value As LexiconDTO)
            _lexicon = value
        End Set
    End Property
    Public Sub SaveLexicon(ByVal year As Integer)
        Dim trans As New SaveLexiconTransaction(_lexicon, year)
        trans.PerformLexiconTransaction()

    End Sub
#End Region
End Class
