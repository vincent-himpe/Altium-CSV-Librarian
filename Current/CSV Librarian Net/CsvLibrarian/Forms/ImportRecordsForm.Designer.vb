Namespace Forms

    Partial Class ImportRecordsForm
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
            lblFiles = New Label()
            lstFiles = New ListBox()
            fileGrid = New DataGridView()
            colField = New DataGridViewTextBoxColumn()
            colData = New DataGridViewTextBoxColumn()
            btnDelete = New Button()
            btnImport = New Button()
            btnClose = New Button()
            CType(fileGrid, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            '
            ' lblFiles
            '
            lblFiles.Anchor = AnchorStyles.Top Or AnchorStyles.Left
            lblFiles.AutoSize = True
            lblFiles.Location = New Point(12, 12)
            lblFiles.Name = "lblFiles"
            lblFiles.Size = New Size(85, 15)
            lblFiles.TabIndex = 0
            lblFiles.Text = "Export files"
            '
            ' lstFiles
            '
            lstFiles.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left
            lstFiles.DrawMode = DrawMode.OwnerDrawFixed
            lstFiles.IntegralHeight = False
            lstFiles.Location = New Point(12, 32)
            lstFiles.Name = "lstFiles"
            lstFiles.Size = New Size(200, 374)
            lstFiles.TabIndex = 1
            '
            ' fileGrid
            '
            fileGrid.AllowUserToAddRows = False
            fileGrid.AllowUserToResizeRows = False
            fileGrid.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            fileGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            fileGrid.Columns.AddRange(New DataGridViewColumn() {colField, colData})
            fileGrid.Location = New Point(224, 32)
            fileGrid.Name = "fileGrid"
            fileGrid.ReadOnly = True
            fileGrid.RowHeadersVisible = False
            fileGrid.SelectionMode = DataGridViewSelectionMode.CellSelect
            fileGrid.Size = New Size(484, 374)
            fileGrid.TabIndex = 2
            '
            ' colField
            '
            colField.HeaderText = "Field"
            colField.Name = "colField"
            colField.ReadOnly = True
            colField.Width = 150
            '
            ' colData
            '
            colData.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            colData.HeaderText = "Data"
            colData.Name = "colData"
            colData.ReadOnly = True
            '
            ' btnDelete
            '
            btnDelete.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            btnDelete.Location = New Point(12, 416)
            btnDelete.Name = "btnDelete"
            btnDelete.Size = New Size(90, 28)
            btnDelete.TabIndex = 3
            btnDelete.Text = "Delete"
            btnDelete.UseVisualStyleBackColor = True
            '
            ' btnImport
            '
            btnImport.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnImport.Location = New Point(534, 416)
            btnImport.Name = "btnImport"
            btnImport.Size = New Size(84, 28)
            btnImport.TabIndex = 4
            btnImport.Text = "Import"
            btnImport.UseVisualStyleBackColor = True
            '
            ' btnClose
            '
            btnClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            btnClose.Location = New Point(624, 416)
            btnClose.Name = "btnClose"
            btnClose.Size = New Size(84, 28)
            btnClose.TabIndex = 5
            btnClose.Text = "Close"
            btnClose.UseVisualStyleBackColor = True
            '
            ' ImportRecordsForm
            '
            CancelButton = btnClose
            ClientSize = New Size(720, 456)
            Controls.Add(lblFiles)
            Controls.Add(lstFiles)
            Controls.Add(fileGrid)
            Controls.Add(btnDelete)
            Controls.Add(btnImport)
            Controls.Add(btnClose)
            MinimumSize = New Size(520, 360)
            Name = "ImportRecordsForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Import Records"
            CType(fileGrid, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents lblFiles As System.Windows.Forms.Label
        Friend WithEvents lstFiles As System.Windows.Forms.ListBox
        Friend WithEvents fileGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colField As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colData As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents btnDelete As System.Windows.Forms.Button
        Friend WithEvents btnImport As System.Windows.Forms.Button
        Friend WithEvents btnClose As System.Windows.Forms.Button
    End Class

End Namespace
