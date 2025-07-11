Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports Entergy.PerformanceManagement.PMDB
Imports Entergy.PerformanceManagement.PMDTO
Imports System.Configuration.ConfigurationManager
Imports Entergy.PerformanceManagement.PMUtilities
Public Class PMDude
    Inherits RadForm
    Private _presenter As New myPresenter
    Private _metric As PMPoint
    Private _modified As Boolean = False
    Private _year As Integer
    Private _selectedSeries As PMPoint.PMMetricDataSeries
    Private _selectedMetricSeriesPoints As PMPoint.PMMetricsDataSeriesPoints
    Private _cID As Integer 'PMPoint
    Dim _path As String = String.Empty
    Dim settings As New My.MySettings
    'Private _repyear As Integer = settings.ReportingYear
    Private _month As String
    Private _MetricDSP As PMPoint

    Private Sub rbtnDEDUDEBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDEDUDEBrowse.Click
        'Dim fileName As String = String.Empty
        'Dim errors As String = String.Empty

        Dim dialog As New OpenFileDialog
        dialog.InitialDirectory = "C:\"
        'dialog.Filter = "xls files (*.xls)|*.xls |xlsx files (*.xlsx)|*.xlsx |xlsm files (*.xlsm)|*.xlsm"
        dialog.Filter = "xls files (*.xls, *.xlsx, *.xlsm)|*.xls;*.xlsx;*.xlsm"

        If dialog.ShowDialog = DialogResult.OK Then
            rtxtDEDUDEFile.Text = dialog.FileName
            'errors = Me.ImportData(fileName)
        End If
        rtxtDUDEResults.Text = String.Empty

    End Sub

    Private Sub rbtnDEDUDEImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDEDUDEImport.Click
        Dim dude As New ImportDUDE
        Dim errors As String
        Try
            errors = dude.ImportData(rtxtDEDUDEFile.Text)
            rtxtDUDEResults.Text = errors
        Catch ex As Exception
            RadMessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub rtxtDEDUDEFile_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtxtDEDUDEFile.TextChanged
        rtxtDUDEResults.Text = String.Empty
    End Sub
End Class