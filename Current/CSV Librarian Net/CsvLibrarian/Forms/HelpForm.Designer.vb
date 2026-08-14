Namespace Forms

    Partial Class HelpForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.pnlContent = New System.Windows.Forms.FlowLayoutPanel()
            Me.btnClose = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'pnlContent
            '
            Me.pnlContent.AutoScroll = True
            Me.pnlContent.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me.pnlContent.Location = New System.Drawing.Point(0, 8)
            Me.pnlContent.Name = "pnlContent"
            Me.pnlContent.Padding = New System.Windows.Forms.Padding(16, 4, 16, 4)
            Me.pnlContent.Size = New System.Drawing.Size(500, 504)
            Me.pnlContent.TabIndex = 1
            Me.pnlContent.WrapContents = False
            '
            'btnClose
            '
            Me.btnClose.Location = New System.Drawing.Point(396, 520)
            Me.btnClose.Name = "btnClose"
            Me.btnClose.Size = New System.Drawing.Size(90, 28)
            Me.btnClose.TabIndex = 2
            Me.btnClose.Text = "Close"
            '
            'HelpForm
            '
            Me.AcceptButton = Me.btnClose
            Me.CancelButton = Me.btnClose
            Me.ClientSize = New System.Drawing.Size(500, 560)
            Me.Controls.Add(Me.pnlContent)
            Me.Controls.Add(Me.btnClose)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "HelpForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Keyboard & Feature Reference"
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents pnlContent As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents btnClose As System.Windows.Forms.Button
    End Class

End Namespace
