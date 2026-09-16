Namespace Forms

    Partial Class ColumnsForm
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
            Me.lblHeading = New System.Windows.Forms.Label()
            Me.lblHint = New System.Windows.Forms.Label()
            Me.lstColumns = New System.Windows.Forms.ListBox()
            Me.btnAdd = New System.Windows.Forms.Button()
            Me.btnRename = New System.Windows.Forms.Button()
            Me.btnDelete = New System.Windows.Forms.Button()
            Me.btnUp = New System.Windows.Forms.Button()
            Me.btnDown = New System.Windows.Forms.Button()
            Me.btnOk = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.grpMigrate = New System.Windows.Forms.GroupBox()
            Me.rbMerge = New System.Windows.Forms.RadioButton()
            Me.rbReplace = New System.Windows.Forms.RadioButton()
            Me.rbCombine = New System.Windows.Forms.RadioButton()
            Me.btnMigrate = New System.Windows.Forms.Button()
            Me.grpMigrate.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblHeading
            '
            Me.lblHeading.AutoSize = True
            Me.lblHeading.Location = New System.Drawing.Point(16, 12)
            Me.lblHeading.Name = "lblHeading"
            Me.lblHeading.Size = New System.Drawing.Size(60, 15)
            Me.lblHeading.TabIndex = 0
            Me.lblHeading.Text = "COLUMNS"
            '
            'lblHint
            '
            Me.lblHint.AutoSize = True
            Me.lblHint.Location = New System.Drawing.Point(16, 30)
            Me.lblHint.MaximumSize = New System.Drawing.Size(430, 0)
            Me.lblHint.Name = "lblHint"
            Me.lblHint.Size = New System.Drawing.Size(420, 15)
            Me.lblHint.TabIndex = 1
            Me.lblHint.Text = "Standard library fields can't be renamed or deleted. The first column is the read-only GUID and stays first."
            '
            'lstColumns
            '
            Me.lstColumns.BackColor = System.Drawing.Color.FromArgb(51, 51, 55)
            Me.lstColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.lstColumns.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220)
            Me.lstColumns.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lstColumns.IntegralHeight = False
            Me.lstColumns.ItemHeight = 18
            Me.lstColumns.Location = New System.Drawing.Point(16, 70)
            Me.lstColumns.Name = "lstColumns"
            Me.lstColumns.Size = New System.Drawing.Size(300, 284)
            Me.lstColumns.TabIndex = 2
            '
            'btnAdd
            '
            Me.btnAdd.Location = New System.Drawing.Point(328, 70)
            Me.btnAdd.Name = "btnAdd"
            Me.btnAdd.Size = New System.Drawing.Size(124, 28)
            Me.btnAdd.TabIndex = 3
            Me.btnAdd.Text = "＋ Add"
            Me.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'btnRename
            '
            Me.btnRename.Location = New System.Drawing.Point(328, 106)
            Me.btnRename.Name = "btnRename"
            Me.btnRename.Size = New System.Drawing.Size(124, 28)
            Me.btnRename.TabIndex = 4
            Me.btnRename.Text = "✎ Rename"
            Me.btnRename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'btnDelete
            '
            Me.btnDelete.Location = New System.Drawing.Point(328, 142)
            Me.btnDelete.Name = "btnDelete"
            Me.btnDelete.Size = New System.Drawing.Size(124, 28)
            Me.btnDelete.TabIndex = 5
            Me.btnDelete.Text = "✕ Delete"
            Me.btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'btnUp
            '
            Me.btnUp.Location = New System.Drawing.Point(328, 190)
            Me.btnUp.Name = "btnUp"
            Me.btnUp.Size = New System.Drawing.Size(124, 28)
            Me.btnUp.TabIndex = 6
            Me.btnUp.Text = "▲ Move Up"
            Me.btnUp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'btnDown
            '
            Me.btnDown.Location = New System.Drawing.Point(328, 226)
            Me.btnDown.Name = "btnDown"
            Me.btnDown.Size = New System.Drawing.Size(124, 28)
            Me.btnDown.TabIndex = 7
            Me.btnDown.Text = "▼ Move Down"
            Me.btnDown.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'btnOk
            '
            Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnOk.Location = New System.Drawing.Point(266, 434)
            Me.btnOk.Name = "btnOk"
            Me.btnOk.Size = New System.Drawing.Size(80, 28)
            Me.btnOk.TabIndex = 8
            Me.btnOk.Text = "OK"
            '
            'btnCancel
            '
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(360, 434)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(80, 28)
            Me.btnCancel.TabIndex = 9
            Me.btnCancel.Text = "Cancel"
            '
            'grpMigrate
            '
            Me.grpMigrate.Controls.Add(Me.rbMerge)
            Me.grpMigrate.Controls.Add(Me.rbReplace)
            Me.grpMigrate.Controls.Add(Me.rbCombine)
            Me.grpMigrate.Controls.Add(Me.btnMigrate)
            Me.grpMigrate.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220)
            Me.grpMigrate.Location = New System.Drawing.Point(16, 360)
            Me.grpMigrate.Name = "grpMigrate"
            Me.grpMigrate.Size = New System.Drawing.Size(436, 66)
            Me.grpMigrate.TabIndex = 10
            Me.grpMigrate.TabStop = False
            Me.grpMigrate.Text = "Migrate"
            '
            'rbMerge
            '
            Me.rbMerge.AutoSize = True
            Me.rbMerge.Checked = True
            Me.rbMerge.Location = New System.Drawing.Point(12, 30)
            Me.rbMerge.Name = "rbMerge"
            Me.rbMerge.Size = New System.Drawing.Size(61, 19)
            Me.rbMerge.TabIndex = 0
            Me.rbMerge.TabStop = True
            Me.rbMerge.Text = "Merge"
            '
            'rbReplace
            '
            Me.rbReplace.AutoSize = True
            Me.rbReplace.Location = New System.Drawing.Point(85, 30)
            Me.rbReplace.Name = "rbReplace"
            Me.rbReplace.Size = New System.Drawing.Size(67, 19)
            Me.rbReplace.TabIndex = 1
            Me.rbReplace.Text = "Replace"
            '
            'rbCombine
            '
            Me.rbCombine.AutoSize = True
            Me.rbCombine.Location = New System.Drawing.Point(164, 30)
            Me.rbCombine.Name = "rbCombine"
            Me.rbCombine.Size = New System.Drawing.Size(72, 19)
            Me.rbCombine.TabIndex = 2
            Me.rbCombine.Text = "Combine"
            '
            'btnMigrate
            '
            Me.btnMigrate.Location = New System.Drawing.Point(306, 24)
            Me.btnMigrate.Name = "btnMigrate"
            Me.btnMigrate.Size = New System.Drawing.Size(118, 30)
            Me.btnMigrate.TabIndex = 3
            Me.btnMigrate.Text = "Migrate"
            '
            'ColumnsForm
            '
            Me.AcceptButton = Me.btnOk
            Me.CancelButton = Me.btnCancel
            Me.BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
            Me.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220)
            Me.ClientSize = New System.Drawing.Size(460, 470)
            Me.Controls.Add(Me.grpMigrate)
            Me.Controls.Add(Me.lblHeading)
            Me.Controls.Add(Me.lblHint)
            Me.Controls.Add(Me.lstColumns)
            Me.Controls.Add(Me.btnAdd)
            Me.Controls.Add(Me.btnRename)
            Me.Controls.Add(Me.btnDelete)
            Me.Controls.Add(Me.btnUp)
            Me.Controls.Add(Me.btnDown)
            Me.Controls.Add(Me.btnOk)
            Me.Controls.Add(Me.btnCancel)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ColumnsForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Manage Columns"
            Me.grpMigrate.ResumeLayout(False)
            Me.grpMigrate.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents lblHeading As System.Windows.Forms.Label
        Friend WithEvents lblHint As System.Windows.Forms.Label
        Friend WithEvents lstColumns As System.Windows.Forms.ListBox
        Friend WithEvents btnAdd As System.Windows.Forms.Button
        Friend WithEvents btnRename As System.Windows.Forms.Button
        Friend WithEvents btnDelete As System.Windows.Forms.Button
        Friend WithEvents btnUp As System.Windows.Forms.Button
        Friend WithEvents btnDown As System.Windows.Forms.Button
        Friend WithEvents btnOk As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents grpMigrate As System.Windows.Forms.GroupBox
        Friend WithEvents rbMerge As System.Windows.Forms.RadioButton
        Friend WithEvents rbReplace As System.Windows.Forms.RadioButton
        Friend WithEvents rbCombine As System.Windows.Forms.RadioButton
        Friend WithEvents btnMigrate As System.Windows.Forms.Button
    End Class

End Namespace
