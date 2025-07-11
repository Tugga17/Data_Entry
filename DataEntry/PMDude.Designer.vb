<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PMDude
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
        Dim ThemeSource4 As Telerik.WinControls.ThemeSource = New Telerik.WinControls.ThemeSource()
        Me.RadThemeManager1 = New Telerik.WinControls.RadThemeManager()
        Me.rlblDEDUDEFile = New Telerik.WinControls.UI.RadLabel()
        Me.rtxtDEDUDEFile = New Telerik.WinControls.UI.RadTextBox()
        Me.rbtnDEDUDEBrowse = New Telerik.WinControls.UI.RadButton()
        Me.rbtnDEDUDEImport = New Telerik.WinControls.UI.RadButton()
        Me.rlblDEDUEResults = New Telerik.WinControls.UI.RadLabel()
        Me.rtxtDUDEResults = New Telerik.WinControls.UI.RadTextBox()
        CType(Me.rlblDEDUDEFile, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rtxtDEDUDEFile, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rbtnDEDUDEBrowse, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rbtnDEDUDEImport, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rlblDEDUEResults, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rtxtDUDEResults, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadThemeManager1
        '
        ThemeSource4.StorageType = Telerik.WinControls.ThemeStorageType.Resource
        ThemeSource4.ThemeLocation = "Nyala11pointV2"
        Me.RadThemeManager1.LoadedThemes.AddRange(New Telerik.WinControls.ThemeSource() {ThemeSource4})
        '
        'rlblDEDUDEFile
        '
        Me.rlblDEDUDEFile.BackColor = System.Drawing.Color.Transparent
        Me.rlblDEDUDEFile.Location = New System.Drawing.Point(12, 21)
        Me.rlblDEDUDEFile.Name = "rlblDEDUDEFile"
        Me.rlblDEDUDEFile.Size = New System.Drawing.Size(230, 22)
        Me.rlblDEDUDEFile.TabIndex = 1
        Me.rlblDEDUDEFile.Text = "Import Metric Data - Select File"
        '
        'rtxtDEDUDEFile
        '
        Me.rtxtDEDUDEFile.Location = New System.Drawing.Point(12, 51)
        Me.rtxtDEDUDEFile.Name = "rtxtDEDUDEFile"
        Me.rtxtDEDUDEFile.ReadOnly = True
        Me.rtxtDEDUDEFile.Size = New System.Drawing.Size(570, 27)
        Me.rtxtDEDUDEFile.TabIndex = 2
        '
        'rbtnDEDUDEBrowse
        '
        Me.rbtnDEDUDEBrowse.Location = New System.Drawing.Point(615, 55)
        Me.rbtnDEDUDEBrowse.Name = "rbtnDEDUDEBrowse"
        Me.rbtnDEDUDEBrowse.Size = New System.Drawing.Size(75, 23)
        Me.rbtnDEDUDEBrowse.TabIndex = 3
        Me.rbtnDEDUDEBrowse.Text = "Browse"
        '
        'rbtnDEDUDEImport
        '
        Me.rbtnDEDUDEImport.Location = New System.Drawing.Point(12, 86)
        Me.rbtnDEDUDEImport.Name = "rbtnDEDUDEImport"
        Me.rbtnDEDUDEImport.Size = New System.Drawing.Size(110, 23)
        Me.rbtnDEDUDEImport.TabIndex = 4
        Me.rbtnDEDUDEImport.Text = "Import File"
        '
        'rlblDEDUEResults
        '
        Me.rlblDEDUEResults.BackColor = System.Drawing.Color.Transparent
        Me.rlblDEDUEResults.Location = New System.Drawing.Point(12, 126)
        Me.rlblDEDUEResults.Name = "rlblDEDUEResults"
        Me.rlblDEDUEResults.Size = New System.Drawing.Size(56, 22)
        Me.rlblDEDUEResults.TabIndex = 5
        Me.rlblDEDUEResults.Text = "Results"
        '
        'rtxtDUDEResults
        '
        Me.rtxtDUDEResults.Location = New System.Drawing.Point(12, 154)
        Me.rtxtDUDEResults.Multiline = True
        Me.rtxtDUDEResults.Name = "rtxtDUDEResults"
        '
        '
        '
        Me.rtxtDUDEResults.RootElement.StretchVertically = True
        Me.rtxtDUDEResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.rtxtDUDEResults.Size = New System.Drawing.Size(842, 472)
        Me.rtxtDUDEResults.TabIndex = 6
        '
        'PMDude
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 21.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(895, 638)
        Me.Controls.Add(Me.rtxtDUDEResults)
        Me.Controls.Add(Me.rlblDEDUEResults)
        Me.Controls.Add(Me.rbtnDEDUDEImport)
        Me.Controls.Add(Me.rbtnDEDUDEBrowse)
        Me.Controls.Add(Me.rtxtDEDUDEFile)
        Me.Controls.Add(Me.rlblDEDUDEFile)
        Me.Font = New System.Drawing.Font("Nyala", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "PMDude"
        Me.Text = "Import DUDE Files"
        CType(Me.rlblDEDUDEFile, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rtxtDEDUDEFile, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rbtnDEDUDEBrowse, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rbtnDEDUDEImport, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rlblDEDUEResults, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rtxtDUDEResults, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RadThemeManager1 As Telerik.WinControls.RadThemeManager
    Friend WithEvents rlblDEDUDEFile As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rtxtDEDUDEFile As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents rbtnDEDUDEBrowse As Telerik.WinControls.UI.RadButton
    Friend WithEvents rbtnDEDUDEImport As Telerik.WinControls.UI.RadButton
    Friend WithEvents rlblDEDUEResults As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rtxtDUDEResults As Telerik.WinControls.UI.RadTextBox
End Class
