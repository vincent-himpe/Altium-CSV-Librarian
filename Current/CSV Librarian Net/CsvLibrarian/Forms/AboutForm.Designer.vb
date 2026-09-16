Namespace Forms

    Partial Class AboutForm
        Inherits System.Windows.Forms.Form

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

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.lblAppTitle = New System.Windows.Forms.Label()
            Me.lblAbout = New System.Windows.Forms.Label()
            Me.btnOk = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'lblAppTitle
            '
            Me.lblAppTitle.AutoSize = True
            Me.lblAppTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
            Me.lblAppTitle.Location = New System.Drawing.Point(16, 18)
            Me.lblAppTitle.Name = "lblAppTitle"
            Me.lblAppTitle.Size = New System.Drawing.Size(240, 25)
            Me.lblAppTitle.TabIndex = 0
            Me.lblAppTitle.Text = "Altium CSV Librarian V"
            '
            'lblAbout
            '
            Me.lblAbout.Location = New System.Drawing.Point(18, 56)
            Me.lblAbout.Name = "lblAbout"
            Me.lblAbout.Size = New System.Drawing.Size(366, 152)
            Me.lblAbout.TabIndex = 1
            Me.lblAbout.Text = ""
            '
            'btnOk
            '
            Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnOk.Location = New System.Drawing.Point(302, 218)
            Me.btnOk.Name = "btnOk"
            Me.btnOk.Size = New System.Drawing.Size(82, 28)
            Me.btnOk.TabIndex = 2
            Me.btnOk.Text = "OK"
            '
            'AboutForm
            '
            Me.AcceptButton = Me.btnOk
            Me.CancelButton = Me.btnOk
            Me.BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
            Me.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220)
            Me.ClientSize = New System.Drawing.Size(400, 258)
            Me.Controls.Add(Me.lblAppTitle)
            Me.Controls.Add(Me.lblAbout)
            Me.Controls.Add(Me.btnOk)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "AboutForm"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "About"
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents lblAppTitle As System.Windows.Forms.Label
        Friend WithEvents lblAbout As System.Windows.Forms.Label
        Friend WithEvents btnOk As System.Windows.Forms.Button
    End Class

End Namespace
