<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DataEntryGrid
    Inherits Telerik.WinControls.RadForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim ThemeSource13 As Telerik.WinControls.ThemeSource = New Telerik.WinControls.ThemeSource()
        Dim ThemeSource14 As Telerik.WinControls.ThemeSource = New Telerik.WinControls.ThemeSource()
        Me.RadContextMenuManager1 = New Telerik.WinControls.UI.RadContextMenuManager()
        Me.rgvDEMetrics = New Telerik.WinControls.UI.RadGridView()
        Me.DataEntryMenuItems = New Telerik.WinControls.UI.RadContextMenu()
        Me.rmiCalComp = New Telerik.WinControls.UI.RadMenuItem()
        Me.rmiCalCompPE = New Telerik.WinControls.UI.RadMenuItem()
        Me.rmiCarryOver = New Telerik.WinControls.UI.RadMenuItem()
        Me.RadThemeManager1 = New Telerik.WinControls.RadThemeManager()
        Me.RadMenuSeparatorItem1 = New Telerik.WinControls.UI.RadMenuSeparatorItem()
        Me.RadMenuSeparatorItem2 = New Telerik.WinControls.UI.RadMenuSeparatorItem()
        CType(Me.rgvDEMetrics, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rgvDEMetrics.MasterGridViewTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rgvDEMetrics
        '
        Me.rgvDEMetrics.BackColor = System.Drawing.Color.Transparent
        Me.rgvDEMetrics.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rgvDEMetrics.Font = New System.Drawing.Font("Nyala", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rgvDEMetrics.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.rgvDEMetrics.MasterGridViewTemplate.AllowAddNewRow = False
        Me.rgvDEMetrics.MasterGridViewTemplate.AllowCellContextMenu = False
        Me.rgvDEMetrics.MasterGridViewTemplate.AllowColumnHeaderContextMenu = False
        Me.rgvDEMetrics.MasterGridViewTemplate.EnableGrouping = False
        Me.rgvDEMetrics.MinimumSize = New System.Drawing.Size(1185, 0)
        Me.rgvDEMetrics.Name = "rgvDEMetrics"
        Me.RadContextMenuManager1.SetRadContextMenu(Me.rgvDEMetrics, Me.DataEntryMenuItems)
        '
        '
        '
        Me.rgvDEMetrics.RootElement.MinSize = New System.Drawing.Size(1185, 0)
        Me.rgvDEMetrics.Size = New System.Drawing.Size(1339, 1017)
        Me.rgvDEMetrics.TabIndex = 1
        Me.rgvDEMetrics.ThemeName = "Vista"
        '
        'DataEntryMenuItems
        '
        Me.DataEntryMenuItems.Items.AddRange(New Telerik.WinControls.RadItem() {Me.rmiCalComp, Me.RadMenuSeparatorItem1, Me.rmiCalCompPE, Me.RadMenuSeparatorItem2, Me.rmiCarryOver})
        '
        'rmiCalComp
        '
        Me.rmiCalComp.Font = New System.Drawing.Font("Nyala", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rmiCalComp.Name = "rmiCalComp"
        Me.rmiCalComp.Text = "Calculate Composites"
        '
        'rmiCalCompPE
        '
        Me.rmiCalCompPE.Font = New System.Drawing.Font("Nyala", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rmiCalCompPE.Name = "rmiCalCompPE"
        Me.rmiCalCompPE.Text = "Calculate Composites PE"
        '
        'rmiCarryOver
        '
        Me.rmiCarryOver.Font = New System.Drawing.Font("Nyala", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rmiCarryOver.Name = "rmiCarryOver"
        Me.rmiCarryOver.Text = "Carry Over Points"
        '
        'RadThemeManager1
        '
        ThemeSource13.StorageType = Telerik.WinControls.ThemeStorageType.Resource
        ThemeSource13.ThemeLocation = "Nyala11pointV2.xml"
        ThemeSource14.StorageType = Telerik.WinControls.ThemeStorageType.Resource
        ThemeSource14.ThemeLocation = "Desert.xml"
        Me.RadThemeManager1.LoadedThemes.AddRange(New Telerik.WinControls.ThemeSource() {ThemeSource13, ThemeSource14})
        '
        'RadMenuSeparatorItem1
        '
        Me.RadMenuSeparatorItem1.Name = "RadMenuSeparatorItem1"
        Me.RadMenuSeparatorItem1.Text = "RadMenuSeparatorItem1"
        '
        'RadMenuSeparatorItem2
        '
        Me.RadMenuSeparatorItem2.Name = "RadMenuSeparatorItem2"
        Me.RadMenuSeparatorItem2.Text = "RadMenuSeparatorItem2"
        '
        'DataEntryGrid
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1339, 1017)
        Me.Controls.Add(Me.rgvDEMetrics)
        Me.Font = New System.Drawing.Font("Nyala", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "DataEntryGrid"
        Me.RadContextMenuManager1.SetRadContextMenu(Me, Me.DataEntryMenuItems)
        Me.Text = "Data Entry"
        Me.ThemeName = "Office2007Blue"
        CType(Me.rgvDEMetrics.MasterGridViewTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rgvDEMetrics, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RadContextMenuManager1 As Telerik.WinControls.UI.RadContextMenuManager
    Friend WithEvents RadThemeManager1 As Telerik.WinControls.RadThemeManager
    Friend WithEvents rgvDEMetrics As Telerik.WinControls.UI.RadGridView
    Friend WithEvents DataEntryMenuItems As Telerik.WinControls.UI.RadContextMenu
    Friend WithEvents rmiCalComp As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rmiCalCompPE As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents rmiCarryOver As Telerik.WinControls.UI.RadMenuItem
    Friend WithEvents RadMenuSeparatorItem1 As Telerik.WinControls.UI.RadMenuSeparatorItem
    Friend WithEvents RadMenuSeparatorItem2 As Telerik.WinControls.UI.RadMenuSeparatorItem
End Class
