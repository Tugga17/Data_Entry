Imports Entergy.PerformanceManagement.PMUtilities
Imports Telerik.WinControls.UI
Imports Telerik.WinControls
Imports Entergy.PerformanceManagement.PMDB

Public Class PMCommentProviderAdd
    Inherits RadForm
    Private Sub rtxtCommentProvider_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtxtCommentProvider.TextChanged
        Dim db As New PMMetricsDB
        If Me.rtxtCommentProvider.Text > "" Then Me.rlblCommProvOP.Text = db.GetProviderName(Me.rtxtCommentProvider.Text)
    End Sub
    Public ReadOnly Property UserID As String
        Get
            Return Me.rtxtCommentProvider.Text
        End Get
    End Property
    Private Sub PMCommentProviderAdd_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.rlblCommProvOP.Text = String.Empty
    End Sub

    'Private Sub rbtnCPOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnCPOK.Click
    '    Dim db As New PMMetricsDB
    '    If Me.rtxtCommentProvider.Text > "" Then Me.rlblCommProvOP.Text = db.GetProviderName(Me.rtxtCommentProvider.Text)
    'End Sub

    'Private Sub rbtnCPCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnCPCancel.Click

    'End Sub
End Class