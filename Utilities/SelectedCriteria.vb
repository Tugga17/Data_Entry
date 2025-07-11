Public Class SelectedCriteria
    Private Shared _Instance As CriteriaDTO
    Private Sub New()
    End Sub
    Public Shared Function GetInstance() As CriteriaDTO
        If IsNothing(_Instance) Then
            _Instance = New CriteriaDTO
        End If
        Return _Instance
    End Function
    Public Shared Sub SetInstance(ByVal instance As CriteriaDTO)
        _Instance = instance
    End Sub
End Class
