Namespace Forms

    Partial Class PrintSelectForm
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
            lblHeader = New Label()
            fileList = New CheckedListBox()
            btnCheckAll = New Button()
            btnCheckNone = New Button()
            btnPrint = New Button()
            btnCancel = New Button()
            SuspendLayout()
            '
            ' lblHeader
            '
            lblHeader.AutoSize = True
            lblHeader.Location = New Point(12, 12)
            lblHeader.Name = "lblHeader"
            lblHeader.Size = New Size(210, 15)
            lblHeader.TabIndex = 0
            lblHeader.Text = "Select files to include in the report:"
            '
            ' fileList
            '
            fileList.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            fileList.BackColor = Color.FromArgb(37, 37, 41)
            fileList.ForeColor = Color.FromArgb(220, 220, 220)
            fileList.BorderStyle = BorderStyle.FixedSingle
            fileList.CheckOnClick = True
            fileList.DrawMode = DrawMode.OwnerDrawFixed
            fileList.IntegralHeight = False
            fileList.ItemHeight = 20
            fileList.Location = New Point(12, 33)
            fileList.Name = "fileList"
            fileList.Size = New Size(360, 300)
            fileList.TabIndex = 1
            '
            ' btnCheckAll
            '
            btnCheckAll.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            btnCheckAll.Location = New Point(12, 345)
            btnCheckAll.Name = "btnCheckAll"
            btnCheckAll.Size = New Size(90, 28)
            btnCheckAll.TabIndex = 2
            btnCheckAll.Text = "Check All"
            btnCheckAll.UseVisualStyleBackColor = False
            '
            ' btnCheckNone
            '
            btnCheckNone.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            btnCheckNone.Location = New Point(108, 345)
            btnCheckNone.Name = "btnCheckNone"
            btnCheckNone.Size = New Size(90, 28)
            btnCheckNone.TabIndex = 3
            btnCheckNone.Text = "Check None"
            btnCheckNone.UseVisualStyleBackColor = False
            '
            ' btnPrint
            '
            btnPrint.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnPrint.DialogResult = DialogResult.OK
            btnPrint.Location = New Point(196, 345)
            btnPrint.Name = "btnPrint"
            btnPrint.Size = New Size(84, 28)
            btnPrint.TabIndex = 4
            btnPrint.Text = "Print"
            btnPrint.UseVisualStyleBackColor = False
            '
            ' btnCancel
            '
            btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnCancel.DialogResult = DialogResult.Cancel
            btnCancel.Location = New Point(288, 345)
            btnCancel.Name = "btnCancel"
            btnCancel.Size = New Size(84, 28)
            btnCancel.TabIndex = 5
            btnCancel.Text = "Cancel"
            btnCancel.UseVisualStyleBackColor = False
            '
            ' PrintSelectForm
            '
            AcceptButton = btnPrint
            CancelButton = btnCancel
            BackColor = Color.FromArgb(45, 45, 48)
            ForeColor = Color.FromArgb(220, 220, 220)
            ClientSize = New Size(384, 385)
            Controls.Add(lblHeader)
            Controls.Add(fileList)
            Controls.Add(btnCheckAll)
            Controls.Add(btnCheckNone)
            Controls.Add(btnPrint)
            Controls.Add(btnCancel)
            MaximizeBox = False
            MinimizeBox = False
            MinimumSize = New Size(320, 320)
            Name = "PrintSelectForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Location Report — Select Files"
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents lblHeader As System.Windows.Forms.Label
        Friend WithEvents fileList As System.Windows.Forms.CheckedListBox
        Friend WithEvents btnCheckAll As System.Windows.Forms.Button
        Friend WithEvents btnCheckNone As System.Windows.Forms.Button
        Friend WithEvents btnPrint As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
    End Class

End Namespace
