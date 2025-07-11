<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PMCommentProviderAdd
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
        Me.rlblCommentProvider = New Telerik.WinControls.UI.RadLabel()
        Me.rtxtCommentProvider = New Telerik.WinControls.UI.RadTextBox()
        Me.rlblCommProvOP = New Telerik.WinControls.UI.RadLabel()
        Me.rbtnCPOK = New Telerik.WinControls.UI.RadButton()
        Me.rbtnCPCancel = New Telerik.WinControls.UI.RadButton()
        CType(Me.rlblCommentProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rtxtCommentProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rlblCommProvOP, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rbtnCPOK, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.rbtnCPCancel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rlblCommentProvider
        '
        Me.rlblCommentProvider.BackColor = System.Drawing.Color.Transparent
        Me.rlblCommentProvider.Location = New System.Drawing.Point(13, 13)
        Me.rlblCommentProvider.Name = "rlblCommentProvider"
        Me.rlblCommentProvider.Size = New System.Drawing.Size(139, 22)
        Me.rlblCommentProvider.TabIndex = 0
        Me.rlblCommentProvider.Text = "Comment Provider"
        '
        'rtxtCommentProvider
        '
        Me.rtxtCommentProvider.Location = New System.Drawing.Point(148, 8)
        Me.rtxtCommentProvider.Name = "rtxtCommentProvider"
        Me.rtxtCommentProvider.Size = New System.Drawing.Size(100, 27)
        Me.rtxtCommentProvider.TabIndex = 1
        '
        'rlblCommProvOP
        '
        Me.rlblCommProvOP.Location = New System.Drawing.Point(261, 13)
        Me.rlblCommProvOP.MinimumSize = New System.Drawing.Size(100, 0)
        Me.rlblCommProvOP.Name = "rlblCommProvOP"
        '
        '
        '
        Me.rlblCommProvOP.RootElement.MinSize = New System.Drawing.Size(100, 0)
        Me.rlblCommProvOP.Size = New System.Drawing.Size(100, 22)
        Me.rlblCommProvOP.TabIndex = 2
        Me.rlblCommProvOP.Text = "...."
        '
        'rbtnCPOK
        '
        Me.rbtnCPOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.rbtnCPOK.Location = New System.Drawing.Point(148, 56)
        Me.rbtnCPOK.Name = "rbtnCPOK"
        Me.rbtnCPOK.Size = New System.Drawing.Size(75, 23)
        Me.rbtnCPOK.TabIndex = 3
        Me.rbtnCPOK.Text = "OK"
        '
        'rbtnCPCancel
        '
        Me.rbtnCPCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.rbtnCPCancel.Location = New System.Drawing.Point(261, 56)
        Me.rbtnCPCancel.Name = "rbtnCPCancel"
        Me.rbtnCPCancel.Size = New System.Drawing.Size(75, 23)
        Me.rbtnCPCancel.TabIndex = 4
        Me.rbtnCPCancel.Text = "Cancel"
        '
        'PMCommentProviderAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 21.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(194, Byte), Integer), CType(CType(214, Byte), Integer), CType(CType(247, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(446, 105)
        Me.Controls.Add(Me.rbtnCPCancel)
        Me.Controls.Add(Me.rbtnCPOK)
        Me.Controls.Add(Me.rlblCommProvOP)
        Me.Controls.Add(Me.rtxtCommentProvider)
        Me.Controls.Add(Me.rlblCommentProvider)
        Me.Font = New System.Drawing.Font("Nyala", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "PMCommentProviderAdd"
        Me.Text = "Add Comment Provider"
        CType(Me.rlblCommentProvider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rtxtCommentProvider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rlblCommProvOP, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rbtnCPOK, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.rbtnCPCancel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rlblCommentProvider As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rtxtCommentProvider As Telerik.WinControls.UI.RadTextBox
    Friend WithEvents rlblCommProvOP As Telerik.WinControls.UI.RadLabel
    Friend WithEvents rbtnCPOK As Telerik.WinControls.UI.RadButton
    Friend WithEvents rbtnCPCancel As Telerik.WinControls.UI.RadButton
End Class
