Namespace Forms

    Partial Class AdvancedClipboardForm
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
            hintLbl = New Label()
            clipGrid = New DataGridView()
            closeBtn = New Button()
            cancelBtn = New Button()
            CType(clipGrid, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            '
            ' hintLbl
            '
            hintLbl.AutoSize = True
            hintLbl.Location = New Point(14, 12)
            hintLbl.Name = "hintLbl"
            hintLbl.Size = New Size(340, 15)
            hintLbl.TabIndex = 0
            hintLbl.Text = "Close saves changes to the clipboard; Cancel discards them."
            '
            ' clipGrid
            '
            clipGrid.AllowUserToAddRows = False
            clipGrid.AllowUserToResizeRows = False
            clipGrid.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            clipGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            clipGrid.Location = New Point(14, 34)
            clipGrid.Name = "clipGrid"
            clipGrid.RowHeadersWidth = 44
            clipGrid.SelectionMode = DataGridViewSelectionMode.CellSelect
            clipGrid.Size = New Size(760, 290)
            clipGrid.TabIndex = 1
            '
            ' closeBtn
            '
            closeBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            closeBtn.Location = New Point(596, 334)
            closeBtn.Name = "closeBtn"
            closeBtn.Size = New Size(84, 28)
            closeBtn.TabIndex = 2
            closeBtn.Text = "Close"
            closeBtn.UseVisualStyleBackColor = True
            '
            ' cancelBtn
            '
            cancelBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            cancelBtn.DialogResult = DialogResult.Cancel
            cancelBtn.Location = New Point(690, 334)
            cancelBtn.Name = "cancelBtn"
            cancelBtn.Size = New Size(84, 28)
            cancelBtn.TabIndex = 3
            cancelBtn.Text = "Cancel"
            cancelBtn.UseVisualStyleBackColor = True
            '
            ' AdvancedClipboardForm
            '
            CancelButton = cancelBtn
            ClientSize = New Size(788, 374)
            Controls.Add(hintLbl)
            Controls.Add(clipGrid)
            Controls.Add(closeBtn)
            Controls.Add(cancelBtn)
            MinimizeBox = False
            MinimumSize = New Size(520, 300)
            Name = "AdvancedClipboardForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Advanced Clipboard"
            CType(clipGrid, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents hintLbl As System.Windows.Forms.Label
        Friend WithEvents clipGrid As System.Windows.Forms.DataGridView
        Friend WithEvents closeBtn As System.Windows.Forms.Button
        Friend WithEvents cancelBtn As System.Windows.Forms.Button
    End Class

End Namespace
