Namespace Forms

    Partial Class RulesEditorForm
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
            rsHeading = New Label()
            ruleList = New ListBox()
            addRuleBtn = New Button()
            delRuleBtn = New Button()
            matchLbl = New Label()
            matchBox = New TextBox()
            nzLbl = New Label()
            nzHint = New Label()
            nzGrid = New DataGridView()
            colName = New DataGridViewTextBoxColumn()
            colTf = New DataGridViewTextBoxColumn()
            buildLine = New Panel()
            buildEnable = New CheckBox()
            tgtLbl = New Label()
            targetBox = New TextBox()
            srcLbl = New Label()
            sourcesBox = New TextBox()
            srcHint = New Label()
            sepLbl = New Label()
            sepBox = New TextBox()
            skipEmptyChk = New CheckBox()
            okBtn = New Button()
            cancelBtn = New Button()
            CType(nzGrid, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' rsHeading
            ' 
            rsHeading.AutoSize = True
            rsHeading.Location = New Point(14, 12)
            rsHeading.Name = "rsHeading"
            rsHeading.Size = New Size(62, 15)
            rsHeading.TabIndex = 0
            rsHeading.Text = "RULE SETS"
            ' 
            ' ruleList
            ' 
            ruleList.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left
            ruleList.IntegralHeight = False
            ruleList.ItemHeight = 15
            ruleList.Location = New Point(14, 32)
            ruleList.Name = "ruleList"
            ruleList.Size = New Size(190, 593)
            ruleList.TabIndex = 0
            ' 
            ' addRuleBtn
            ' 
            addRuleBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            addRuleBtn.Location = New Point(14, 633)
            addRuleBtn.Name = "addRuleBtn"
            addRuleBtn.Size = New Size(92, 26)
            addRuleBtn.TabIndex = 1
            addRuleBtn.Text = "＋ Add"
            ' 
            ' delRuleBtn
            ' 
            delRuleBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            delRuleBtn.Location = New Point(112, 633)
            delRuleBtn.Name = "delRuleBtn"
            delRuleBtn.Size = New Size(92, 26)
            delRuleBtn.TabIndex = 2
            delRuleBtn.Text = "✕ Delete"
            ' 
            ' matchLbl
            ' 
            matchLbl.AutoSize = True
            matchLbl.Location = New Point(224, 14)
            matchLbl.Name = "matchLbl"
            matchLbl.Size = New Size(309, 15)
            matchLbl.TabIndex = 3
            matchLbl.Text = "Match (file name or glob, e.g. capacitors.csv or res_*.csv):"
            ' 
            ' matchBox
            ' 
            matchBox.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            matchBox.Location = New Point(224, 32)
            matchBox.Name = "matchBox"
            matchBox.Size = New Size(521, 23)
            matchBox.TabIndex = 3
            ' 
            ' nzLbl
            ' 
            nzLbl.AutoSize = True
            nzLbl.Location = New Point(224, 64)
            nzLbl.Name = "nzLbl"
            nzLbl.Size = New Size(279, 15)
            nzLbl.TabIndex = 4
            nzLbl.Text = "Per-column transforms  (separate multiple with  |  ):"
            ' 
            ' nzHint
            ' 
            nzHint.AutoSize = True
            nzHint.Location = New Point(224, 82)
            nzHint.Name = "nzHint"
            nzHint.Size = New Size(417, 15)
            nzHint.TabIndex = 5
            nzHint.Text = "known: upper · lower · trim · stripspaces · collapsespaces · replace:FROM=>TO"
            ' 
            ' nzGrid
            ' 
            nzGrid.AllowUserToAddRows = False
            nzGrid.AllowUserToResizeRows = False
            nzGrid.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            nzGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            nzGrid.Columns.AddRange(New DataGridViewColumn() {colName, colTf})
            nzGrid.Location = New Point(224, 102)
            nzGrid.Name = "nzGrid"
            nzGrid.RowHeadersVisible = False
            nzGrid.Size = New Size(521, 377)
            nzGrid.TabIndex = 4
            ' 
            ' colName
            ' 
            colName.HeaderText = "Column"
            colName.Name = "colName"
            colName.ReadOnly = True
            colName.Width = 190
            ' 
            ' colTf
            ' 
            colTf.HeaderText = "Transforms (a|b|c)"
            colTf.Name = "colTf"
            colTf.Width = 280
            ' 
            ' buildLine
            ' 
            buildLine.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            buildLine.Location = New Point(224, 487)
            buildLine.Name = "buildLine"
            buildLine.Size = New Size(521, 1)
            buildLine.TabIndex = 6
            ' 
            ' buildEnable
            ' 
            buildEnable.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            buildEnable.AutoSize = True
            buildEnable.Location = New Point(224, 495)
            buildEnable.Name = "buildEnable"
            buildEnable.Size = New Size(215, 19)
            buildEnable.TabIndex = 5
            buildEnable.Text = "Build a column from other columns"
            ' 
            ' tgtLbl
            ' 
            tgtLbl.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            tgtLbl.AutoSize = True
            tgtLbl.Location = New Point(224, 523)
            tgtLbl.Name = "tgtLbl"
            tgtLbl.Size = New Size(87, 15)
            tgtLbl.TabIndex = 7
            tgtLbl.Text = "Target column:"
            ' 
            ' targetBox
            ' 
            targetBox.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            targetBox.Location = New Point(334, 520)
            targetBox.Name = "targetBox"
            targetBox.Size = New Size(200, 23)
            targetBox.TabIndex = 6
            ' 
            ' srcLbl
            ' 
            srcLbl.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            srcLbl.AutoSize = True
            srcLbl.Location = New Point(224, 555)
            srcLbl.Name = "srcLbl"
            srcLbl.Size = New Size(87, 15)
            srcLbl.TabIndex = 8
            srcLbl.Text = "Sources:"
            ' 
            ' sourcesBox
            ' 
            sourcesBox.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            sourcesBox.Location = New Point(334, 552)
            sourcesBox.Name = "sourcesBox"
            sourcesBox.Size = New Size(411, 23)
            sourcesBox.TabIndex = 7
            '
            ' srcHint
            '
            srcHint.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            srcHint.AutoSize = True
            srcHint.Location = New Point(334, 576)
            srcHint.Name = "srcHint"
            srcHint.Size = New Size(200, 15)
            srcHint.TabIndex = 12
            srcHint.Text = "(columns, or ""literals"" in double quotes)"
            '
            ' sepLbl
            '
            sepLbl.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            sepLbl.AutoSize = True
            sepLbl.Location = New Point(224, 603)
            sepLbl.Name = "sepLbl"
            sepLbl.Size = New Size(60, 15)
            sepLbl.TabIndex = 9
            sepLbl.Text = "Separator:"
            ' 
            ' sepBox
            ' 
            sepBox.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            sepBox.Location = New Point(334, 600)
            sepBox.Name = "sepBox"
            sepBox.Size = New Size(60, 23)
            sepBox.TabIndex = 8
            ' 
            ' skipEmptyChk
            ' 
            skipEmptyChk.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
            skipEmptyChk.AutoSize = True
            skipEmptyChk.Location = New Point(414, 601)
            skipEmptyChk.Name = "skipEmptyChk"
            skipEmptyChk.Size = New Size(159, 19)
            skipEmptyChk.TabIndex = 9
            skipEmptyChk.Text = "Skip empty source values"
            ' 
            ' okBtn
            ' 
            okBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            okBtn.Location = New Point(577, 631)
            okBtn.Name = "okBtn"
            okBtn.Size = New Size(84, 28)
            okBtn.TabIndex = 10
            okBtn.Text = "OK"
            ' 
            ' cancelBtn
            ' 
            cancelBtn.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
            cancelBtn.DialogResult = DialogResult.Cancel
            cancelBtn.Location = New Point(667, 631)
            cancelBtn.Name = "cancelBtn"
            cancelBtn.Size = New Size(84, 28)
            cancelBtn.TabIndex = 11
            cancelBtn.Text = "Cancel"
            ' 
            ' RulesEditorForm
            ' 
            AcceptButton = okBtn
            CancelButton = cancelBtn
            ClientSize = New Size(771, 671)
            Controls.Add(rsHeading)
            Controls.Add(ruleList)
            Controls.Add(addRuleBtn)
            Controls.Add(delRuleBtn)
            Controls.Add(matchLbl)
            Controls.Add(matchBox)
            Controls.Add(nzLbl)
            Controls.Add(nzHint)
            Controls.Add(nzGrid)
            Controls.Add(buildLine)
            Controls.Add(buildEnable)
            Controls.Add(tgtLbl)
            Controls.Add(targetBox)
            Controls.Add(srcLbl)
            Controls.Add(sourcesBox)
            Controls.Add(srcHint)
            Controls.Add(sepLbl)
            Controls.Add(sepBox)
            Controls.Add(skipEmptyChk)
            Controls.Add(okBtn)
            Controls.Add(cancelBtn)
            MinimizeBox = False
            MinimumSize = New Size(756, 579)
            Name = "RulesEditorForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Normalizer Rules  (normalizer.json)"
            CType(nzGrid, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents rsHeading As System.Windows.Forms.Label
        Friend WithEvents ruleList As System.Windows.Forms.ListBox
        Friend WithEvents addRuleBtn As System.Windows.Forms.Button
        Friend WithEvents delRuleBtn As System.Windows.Forms.Button
        Friend WithEvents matchLbl As System.Windows.Forms.Label
        Friend WithEvents matchBox As System.Windows.Forms.TextBox
        Friend WithEvents nzLbl As System.Windows.Forms.Label
        Friend WithEvents nzHint As System.Windows.Forms.Label
        Friend WithEvents nzGrid As System.Windows.Forms.DataGridView
        Friend WithEvents colName As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents colTf As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents buildLine As System.Windows.Forms.Panel
        Friend WithEvents buildEnable As System.Windows.Forms.CheckBox
        Friend WithEvents tgtLbl As System.Windows.Forms.Label
        Friend WithEvents targetBox As System.Windows.Forms.TextBox
        Friend WithEvents srcLbl As System.Windows.Forms.Label
        Friend WithEvents sourcesBox As System.Windows.Forms.TextBox
        Friend WithEvents srcHint As System.Windows.Forms.Label
        Friend WithEvents sepLbl As System.Windows.Forms.Label
        Friend WithEvents sepBox As System.Windows.Forms.TextBox
        Friend WithEvents skipEmptyChk As System.Windows.Forms.CheckBox
        Friend WithEvents okBtn As System.Windows.Forms.Button
        Friend WithEvents cancelBtn As System.Windows.Forms.Button
    End Class

End Namespace
