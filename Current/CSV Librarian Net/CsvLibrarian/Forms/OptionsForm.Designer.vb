Namespace Forms

    Partial Class OptionsForm
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
            lblAutosave = New Label()
            txtAutosave = New TextBox()
            lblSecs = New Label()
            chkAutosaveEnabled = New CheckBox()
            chkAutoNormalize = New CheckBox()
            chkArchiveExit = New CheckBox()
            okBtn = New Button()
            cancelBtn = New Button()
            SuspendLayout()
            '
            ' lblAutosave
            '
            lblAutosave.AutoSize = True
            lblAutosave.Location = New Point(16, 21)
            lblAutosave.Name = "lblAutosave"
            lblAutosave.Size = New Size(101, 15)
            lblAutosave.TabIndex = 0
            lblAutosave.Text = "Autosave seconds:"
            '
            ' txtAutosave
            '
            txtAutosave.Location = New Point(150, 18)
            txtAutosave.Name = "txtAutosave"
            txtAutosave.Size = New Size(80, 23)
            txtAutosave.TabIndex = 1
            '
            ' lblSecs
            '
            lblSecs.AutoSize = True
            lblSecs.Location = New Point(236, 21)
            lblSecs.Name = "lblSecs"
            lblSecs.Size = New Size(28, 15)
            lblSecs.TabIndex = 2
            lblSecs.Text = "sec"
            '
            ' chkAutosaveEnabled
            '
            chkAutosaveEnabled.AutoSize = True
            chkAutosaveEnabled.Location = New Point(18, 58)
            chkAutosaveEnabled.Name = "chkAutosaveEnabled"
            chkAutosaveEnabled.Size = New Size(160, 19)
            chkAutosaveEnabled.TabIndex = 3
            chkAutosaveEnabled.Text = "Autosave Timer Enabled"
            '
            ' chkAutoNormalize
            '
            chkAutoNormalize.AutoSize = True
            chkAutoNormalize.Location = New Point(18, 85)
            chkAutoNormalize.Name = "chkAutoNormalize"
            chkAutoNormalize.Size = New Size(163, 19)
            chkAutoNormalize.TabIndex = 4
            chkAutoNormalize.Text = "Auto Normalize on Save"
            '
            ' chkArchiveExit
            '
            chkArchiveExit.AutoSize = True
            chkArchiveExit.Location = New Point(18, 112)
            chkArchiveExit.Name = "chkArchiveExit"
            chkArchiveExit.Size = New Size(106, 19)
            chkArchiveExit.TabIndex = 5
            chkArchiveExit.Text = "Archive on exit"
            '
            ' okBtn
            '
            okBtn.Location = New Point(196, 170)
            okBtn.Name = "okBtn"
            okBtn.Size = New Size(80, 28)
            okBtn.TabIndex = 6
            okBtn.Text = "OK"
            okBtn.UseVisualStyleBackColor = True
            '
            ' cancelBtn
            '
            cancelBtn.DialogResult = DialogResult.Cancel
            cancelBtn.Location = New Point(286, 170)
            cancelBtn.Name = "cancelBtn"
            cancelBtn.Size = New Size(80, 28)
            cancelBtn.TabIndex = 7
            cancelBtn.Text = "Cancel"
            cancelBtn.UseVisualStyleBackColor = True
            '
            ' OptionsForm
            '
            AcceptButton = okBtn
            CancelButton = cancelBtn
            ClientSize = New Size(382, 214)
            Controls.Add(lblAutosave)
            Controls.Add(txtAutosave)
            Controls.Add(lblSecs)
            Controls.Add(chkAutosaveEnabled)
            Controls.Add(chkAutoNormalize)
            Controls.Add(chkArchiveExit)
            Controls.Add(okBtn)
            Controls.Add(cancelBtn)
            FormBorderStyle = FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            Name = "OptionsForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Options"
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents lblAutosave As System.Windows.Forms.Label
        Friend WithEvents txtAutosave As System.Windows.Forms.TextBox
        Friend WithEvents lblSecs As System.Windows.Forms.Label
        Friend WithEvents chkAutosaveEnabled As System.Windows.Forms.CheckBox
        Friend WithEvents chkAutoNormalize As System.Windows.Forms.CheckBox
        Friend WithEvents chkArchiveExit As System.Windows.Forms.CheckBox
        Friend WithEvents okBtn As System.Windows.Forms.Button
        Friend WithEvents cancelBtn As System.Windows.Forms.Button
    End Class

End Namespace
