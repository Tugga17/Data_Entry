Imports Entergy.PerformanceManagement.PMUtilities

Public Class myPresenter
    Public Sub LoadCombo(ByRef combo As Telerik.WinControls.UI.RadComboBox, ByVal source As IList, ByVal display As String)
        Me.LoadCombo(combo, source, display, display)
    End Sub
    Public Sub LoadCombo(ByRef combo As Telerik.WinControls.UI.RadComboBox, ByVal source As IList, ByVal display As String, ByVal value As String)
        combo.DataSource = Nothing
        combo.DataSource = source
        combo.DisplayMember = display
        combo.ValueMember = value
    End Sub
    Public Sub LoadCombo(ByRef combo As Telerik.WinControls.UI.RadComboBox, ByVal source As IList)
        combo.DataSource = Nothing
        combo.DataSource = source
    End Sub
    Public Sub LoadOpCoCombo(ByRef combo As Telerik.WinControls.UI.RadComboBox)
        Me.LoadCombo(combo, pmConversions.OpCoList.ToArray)
    End Sub
    Public Sub LoadMonthCombo(ByRef combo As Telerik.WinControls.UI.RadComboBox)
        Me.LoadCombo(combo, pmConversions.MonthList.ToArray)
    End Sub
    Public Sub LoadMonthComboNum(ByRef combo As Telerik.WinControls.UI.RadComboBox)
        Me.LoadCombo(combo, pmConversions.MonthListNum.ToArray)
    End Sub
    Public Sub LoadListBox(ByRef combo As Telerik.WinControls.UI.RadListBox, ByVal source As IList, ByVal display As String)
        Me.LoadListBox(combo, source, display, display)
    End Sub
    Public Sub LoadListBox(ByRef combo As Telerik.WinControls.UI.RadListBox, ByVal source As IList, ByVal display As String, ByVal value As String)
        combo.DataSource = Nothing
        combo.DataSource = source
        combo.DisplayMember = display
        combo.ValueMember = value
    End Sub
    Public Sub LoadListBox(ByRef combo As Telerik.WinControls.UI.RadListBox, ByVal source As IList)
        combo.DataSource = Nothing
        combo.DataSource = source
    End Sub
    Public Sub LoadGridView(ByRef grid As Telerik.WinControls.UI.RadGridView, ByVal source As IList)
        grid.DataSource = source
    End Sub
End Class
