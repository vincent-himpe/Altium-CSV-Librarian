Namespace Forms

    Partial Class SearchEnginesForm
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
            engineGrid = New DataGridView()
            colWebsite = New DataGridViewTextBoxColumn()
            colQuery = New DataGridViewTextBoxColumn()
            colEnabled = New DataGridViewCheckBoxColumn()
            colQuickLook = New DataGridViewCheckBoxColumn()
            btnAddRow = New Button()
            btnRemoveRow = New Button()
            okBtn = New Button()
            cancelBtn = New Button()
            CType(engineGrid, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            '
            ' hintLbl
            '
            hintLbl.AutoSize = True
            hintLbl.Location = New Point(14, 12)
            hintLbl.Name = "hintLbl"
            hintLbl.Size = New Size(360, 15)
            hintLbl.TabIndex = 0
            hintLbl.Text = "Rows with a blank Website are discarded when you click OK."
            '
            ' engineGrid
            '
            engineGrid.AllowUserToAddRows = False
            engineGrid.AllowUserToResizeRows = False
            engineGrid.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            engineGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            engineGrid.Columns.AddRange(New DataGridViewColumn() {colWebsite, colQuery, colEnabled, colQuickLook})
            engineGrid.Location = New Point(14, 34)
            engineGrid.Name = "engineGrid"
            engineGrid.RowHeadersVisible = False
            engineGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            engineGrid.Size = New Size(692, 366)
            engineGrid.TabIndex = 1
            '
            ' colWebsite
            '
            colWebsite.HeaderText = "Website"
            colWebsite.Name = "colWebsite"
            colWebsite.Width = 260
            '
            ' colQuery
            '
            colQuery.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            colQuery.HeaderText = "Query"
            colQuery.Name = "colQuery"
            '
            ' colEnabled
            '
            colEnabled.HeaderText = "Enabled"
            colEnabled.Name = "colEnabled"
            colEnabled.Width = 70
            '
            ' colQuickLook
            '
            colQuickLook.HeaderText = "QuickLook"
            colQuickLook.Name = "colQuickLook"
            colQuickLook.Width = 80
            '
            ' btnAddRow
            '
            btnAddRow.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            btnAddRow.Location = New Point(14, 414)
            btnAddRow.Name = "btnAddRow"
            btnAddRow.Size = New Size(100, 28)
            btnAddRow.TabIndex = 2
            btnAddRow.Text = "Add Row"
            btnAddRow.UseVisualStyleBackColor = True
            '
            ' btnRemoveRow
            '
            btnRemoveRow.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            btnRemoveRow.Location = New Point(120, 414)
            btnRemoveRow.Name = "btnRemoveRow"
            btnRemoveRow.Size = New Size(110, 28)
            btnRemoveRow.TabIndex = 3
            btnRemoveRow.Text = "Remove Row"
            btnRemoveRow.UseVisualStyleBackColor = True
            '
            ' okBtn
            '
            okBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            okBtn.Location = New Point(536, 414)
            okBtn.Name = "okBtn"
            okBtn.Size = New Size(84, 28)
            okBtn.TabIndex = 4
            okBtn.Text = "OK"
            okBtn.UseVisualStyleBackColor = True
            '
            ' cancelBtn
            '
            cancelBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            cancelBtn.DialogResult = DialogResult.Cancel
            cancelBtn.Location = New Point(626, 414)
            cancelBtn.Name = "cancelBtn"
            cancelBtn.Size = New Size(84, 28)
            cancelBtn.TabIndex = 5
            cancelBtn.Text = "Cancel"
            cancelBtn.UseVisualStyleBackColor = True
            '
            ' SearchEnginesForm
            '
            CancelButton = cancelBtn
            BackColor = Color.FromArgb(45, 45, 48)
            ForeColor = Color.FromArgb(220, 220, 220)
            ClientSize = New Size(720, 456)
            Controls.Add(hintLbl)
            Controls.Add(engineGrid)
            Controls.Add(btnAddRow)
            Controls.Add(btnRemoveRow)
            Controls.Add(okBtn)
            Controls.Add(cancelBtn)
            MinimizeBox = False
            MinimumSize = New Size(560, 360)
            Name = "SearchEnginesForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Searchengines"
            CType(engineGrid, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents hintLbl As System.Windows.Forms.Label
        Friend WithEvents engineGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colWebsite As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colQuery As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colEnabled As System.Windows.Forms.DataGridViewCheckBoxColumn
        Friend WithEvents colQuickLook As System.Windows.Forms.DataGridViewCheckBoxColumn
        Friend WithEvents btnAddRow As System.Windows.Forms.Button
        Friend WithEvents btnRemoveRow As System.Windows.Forms.Button
        Friend WithEvents okBtn As System.Windows.Forms.Button
        Friend WithEvents cancelBtn As System.Windows.Forms.Button
    End Class

End Namespace
