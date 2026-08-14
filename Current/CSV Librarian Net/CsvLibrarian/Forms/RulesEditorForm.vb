Imports System.Drawing
Imports System.Text.Json
Imports System.Windows.Forms
Imports CsvLibrarian.Models
Imports CsvLibrarian.Services
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>
    ''' Visual editor for the folder's normalizer.json. Left: the list of rule
    ''' sets (each targets files by name/glob). Right: for the current file, every
    ''' column (except Index) is listed with an editable Transforms cell, plus an
    ''' optional "build a column" rule. A column whose Transforms cell is blank is
    ''' left untouched. Standard Windows chrome; only the label text colours are kept.
    ''' Edits are made on a working copy; the caller persists <see cref="ResultConfig"/>.
    ''' </summary>
    Public Class RulesEditorForm

        Private _config As NormalizerConfig
        Private ReadOnly _activeFile As String
        Private ReadOnly _columns As List(Of String)
        Private _selectedIndex As Integer = -1
        Private _suppress As Boolean = False
        ''' <summary>The one rule set the user may select/edit — the active file's.
        ''' Others are shown greyed and locked. Nothing when the active file has no set.</summary>
        Private _activeRule As NormalizerRuleSet = Nothing

        Public ReadOnly Property ResultConfig As NormalizerConfig
            Get
                Return _config
            End Get
        End Property

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            _config = New NormalizerConfig()
            _activeFile = Nothing
            _columns = New List(Of String)()
            InitializeComponent()
            ApplyLabelColors()
        End Sub

        Public Sub New(config As NormalizerConfig, activeFile As String, columns As List(Of String))
            _config = CloneConfig(config)
            _activeFile = activeFile
            _columns = If(columns, New List(Of String)())
            _activeRule = NormalizerService.FindRuleSet(_config, _activeFile)
            InitializeComponent()
            ApplyLabelColors()
            RefreshRuleList(SelectMatching())
        End Sub

        Private Shared Function CloneConfig(cfg As NormalizerConfig) As NormalizerConfig
            If cfg Is Nothing Then Return New NormalizerConfig()
            Dim json = JsonSerializer.Serialize(cfg)
            Return JsonSerializer.Deserialize(Of NormalizerConfig)(json)
        End Function

        Private Function SelectMatching() As Integer
            If String.IsNullOrEmpty(_activeFile) Then Return If(_config.RuleSets.Count > 0, 0, -1)
            Dim rs = NormalizerService.FindRuleSet(_config, _activeFile)
            If rs Is Nothing Then Return If(_config.RuleSets.Count > 0, 0, -1)
            Return _config.RuleSets.IndexOf(rs)
        End Function

        ''' <summary>
        ''' On open, select the rule set that matches the active file. If none exists,
        ''' offer to create one (Add-rule behaviour), which becomes the active selection.
        ''' </summary>
        Private Sub OnFormLoad(sender As Object, e As EventArgs) Handles MyBase.Load
            If String.IsNullOrEmpty(_activeFile) Then Return

            If _activeRule IsNot Nothing Then
                Dim idx = _config.RuleSets.IndexOf(_activeRule)
                If idx >= 0 AndAlso idx <> ruleList.SelectedIndex Then
                    ruleList.SelectedIndex = idx      ' triggers OnRuleSelected -> reload
                End If
                Return
            End If

            Dim r = MessageBox.Show(Me,
                $"No normalizer rule set matches ""{_activeFile}"".{vbCrLf}Create one now?",
                "Normalizer Rules", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If r = DialogResult.Yes Then
                AddRuleSet()   ' creates a set matching the active file, selects & loads it
            End If
        End Sub

        ''' <summary>True if <paramref name="rs"/> is the active file's rule set.</summary>
        Private Function IsActiveFileRuleSet(rs As NormalizerRuleSet) As Boolean
            Return rs IsNot Nothing AndAlso ReferenceEquals(rs, _activeRule)
        End Function

        ''' <summary>The active file's column names (case-insensitive set).</summary>
        Private Function ActiveColumnSet() As HashSet(Of String)
            Return New HashSet(Of String)(_columns, StringComparer.OrdinalIgnoreCase)
        End Function

        ''' <summary>Drop normalizations whose column no longer exists in the file.</summary>
        Private Sub PruneOrphanNormalizations(rs As NormalizerRuleSet)
            If rs Is Nothing Then Return
            Dim shown = ActiveColumnSet()
            rs.Normalizations = rs.Normalizations.
                Where(Function(n) shown.Contains(n.Column)).ToList()
        End Sub

        ''' <summary>
        ''' Standard Windows colour scheme everywhere, except the label text colours,
        ''' which are kept as before. The divider gets a visible standard colour.
        ''' </summary>
        Private Sub ApplyLabelColors()
            rsHeading.ForeColor = Palette.Accent
            matchLbl.ForeColor = Palette.TextMuted
            nzLbl.ForeColor = Palette.TextMuted
            nzHint.ForeColor = Palette.TextDim
            tgtLbl.ForeColor = Palette.TextMuted
            srcLbl.ForeColor = Palette.TextMuted
            sepLbl.ForeColor = Palette.TextMuted
            srcHint.ForeColor = Palette.TextDim
            buildLine.BackColor = SystemColors.ControlDark

            ' Header row: medium grey background, bold black text.
            nzGrid.EnableHeadersVisualStyles = False
            nzGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(190, 190, 190)
            nzGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            nzGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(190, 190, 190)
            nzGrid.ColumnHeadersDefaultCellStyle.Font = New Font(nzGrid.Font, FontStyle.Bold)

            ' Owner-draw the rule list so non-active rule sets show greyed (disabled).
            ruleList.DrawMode = DrawMode.OwnerDrawFixed
        End Sub

        ''' <summary>Selection is locked to the active file's rule set when one is known.</summary>
        Private ReadOnly Property RestrictToActive As Boolean
            Get
                Return Not String.IsNullOrEmpty(_activeFile)
            End Get
        End Property

        ''' <summary>Index of the active file's rule set, or -1 if none.</summary>
        Private Function ActiveRuleIndex() As Integer
            If _activeRule Is Nothing Then Return -1
            Return _config.RuleSets.IndexOf(_activeRule)
        End Function

        ''' <summary>Grey out every rule set except the active file's, so the others
        ''' read as disabled (selection is additionally locked in OnRuleSelected).</summary>
        Private Sub ruleList_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ruleList.DrawItem
            If e.Index < 0 Then Return
            Dim text = ruleList.Items(e.Index).ToString()
            Dim locked = RestrictToActive AndAlso e.Index <> ActiveRuleIndex()
            Dim selected = (e.State And DrawItemState.Selected) = DrawItemState.Selected

            Dim backColor As Color, foreColor As Color
            If selected AndAlso Not locked Then
                backColor = SystemColors.Highlight
                foreColor = SystemColors.HighlightText
            ElseIf locked Then
                backColor = ruleList.BackColor
                foreColor = SystemColors.GrayText
            Else
                backColor = ruleList.BackColor
                foreColor = ruleList.ForeColor
            End If

            Using b As New SolidBrush(backColor)
                e.Graphics.FillRectangle(b, e.Bounds)
            End Using
            TextRenderer.DrawText(e.Graphics, text, ruleList.Font, e.Bounds, foreColor,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
            If selected AndAlso Not locked Then e.DrawFocusRectangle()
        End Sub

        ' ── Rule set list ────────────────────────────────────────────────────────

        ''' <summary>How a rule set's match pattern is shown in the list:
        ''' upper-case with any ".csv" extension removed (as in the main window).</summary>
        Private Shared Function DisplayMatch(match As String) As String
            If String.IsNullOrEmpty(match) Then Return "(unnamed)"
            Dim m = match
            If m.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                m = m.Substring(0, m.Length - 4)
            End If
            Return m.ToUpperInvariant()
        End Function

        Private Sub RefreshRuleList(selectIndex As Integer)
            _suppress = True
            ruleList.BeginUpdate()
            ruleList.Items.Clear()
            For Each rs In _config.RuleSets
                ruleList.Items.Add("  " & DisplayMatch(rs.Match))
            Next
            ruleList.EndUpdate()
            _suppress = False

            If _config.RuleSets.Count = 0 Then
                _selectedIndex = -1
                LoadEditor(Nothing)
            Else
                Dim idx = Math.Max(0, Math.Min(selectIndex, _config.RuleSets.Count - 1))
                ruleList.SelectedIndex = idx      ' triggers OnRuleSelected
            End If
            ' Delete only applies to a selectable (active) rule set.
            delRuleBtn.Enabled = If(RestrictToActive, ActiveRuleIndex() >= 0, _config.RuleSets.Count > 0)
        End Sub

        Private Sub OnRuleSelected(sender As Object, e As EventArgs) Handles ruleList.SelectedIndexChanged
            If _suppress Then Return
            ' Commit edits of the previously loaded rule set before switching.
            nzGrid.EndEdit()
            CommitNormalizationsToModel()

            ' Selection is locked to the active file's rule set: snap back if the user
            ' clicked one of the greyed (other-file) rule sets.
            Dim target = If(RestrictToActive, ActiveRuleIndex(), ruleList.SelectedIndex)
            If ruleList.SelectedIndex <> target Then
                _suppress = True
                ruleList.SelectedIndex = target      ' -1 clears the selection
                _suppress = False
            End If

            _selectedIndex = target
            LoadEditor(If(target >= 0 AndAlso target < _config.RuleSets.Count,
                          _config.RuleSets(target), Nothing))
        End Sub

        Private Sub AddRuleSet() Handles addRuleBtn.Click
            Dim rs As New NormalizerRuleSet With {.Match = If(String.IsNullOrEmpty(_activeFile), "newfile.csv", _activeFile)}
            _config.RuleSets.Add(rs)
            _activeRule = rs                     ' the new set becomes the active/selectable one
            RefreshRuleList(_config.RuleSets.Count - 1)
        End Sub

        Private Sub DeleteRuleSet() Handles delRuleBtn.Click
            If _selectedIndex < 0 OrElse _selectedIndex >= _config.RuleSets.Count Then Return
            Dim removed = _config.RuleSets(_selectedIndex)
            _config.RuleSets.RemoveAt(_selectedIndex)
            If ReferenceEquals(removed, _activeRule) Then _activeRule = Nothing
            RefreshRuleList(_selectedIndex - 1)
        End Sub

        ' ── Editor <-> model ───────────────────────────────────────────────────────

        Private Sub LoadEditor(rs As NormalizerRuleSet)
            _suppress = True
            Dim enabled = rs IsNot Nothing
            matchBox.Enabled = enabled
            nzGrid.Enabled = enabled
            buildEnable.Enabled = enabled

            nzGrid.Rows.Clear()
            If rs Is Nothing Then
                matchBox.Text = ""
                buildEnable.Checked = False
                SetBuildEnabled(False)
                _suppress = False
                Return
            End If

            matchBox.Text = rs.Match

            ' Keep the active file's rule set in step with its columns: any
            ' normalization for a column that no longer exists is dropped.
            If IsActiveFileRuleSet(rs) Then PruneOrphanNormalizations(rs)

            ' One row per current-file column (except the Index/GUID column). The
            ' Transforms cell is pre-filled from the rule set if it has an entry.
            For Each col In _columns
                If String.Equals(col, "Index", StringComparison.OrdinalIgnoreCase) Then Continue For
                Dim tf As String = ""
                Dim existing = rs.Normalizations.FirstOrDefault(
                    Function(n) String.Equals(n.Column, col, StringComparison.OrdinalIgnoreCase))
                If existing IsNot Nothing Then tf = String.Join("|", existing.Transforms)
                nzGrid.Rows.Add(col, tf)
            Next

            Dim hasBuild = rs.Build IsNot Nothing
            buildEnable.Checked = hasBuild
            If hasBuild Then
                targetBox.Text = rs.Build.Target
                sourcesBox.Text = String.Join("|", rs.Build.Sources)
                sepBox.Text = rs.Build.Separator
                skipEmptyChk.Checked = rs.Build.SkipEmpty
            Else
                targetBox.Text = ""
                sourcesBox.Text = ""
                sepBox.Text = "-"
                skipEmptyChk.Checked = True
            End If
            SetBuildEnabled(hasBuild)
            _suppress = False
        End Sub

        Private Sub SetBuildEnabled(enabledState As Boolean)
            targetBox.Enabled = enabledState
            sourcesBox.Enabled = enabledState
            sepBox.Enabled = enabledState
            skipEmptyChk.Enabled = enabledState
        End Sub

        Private Function CurrentRuleSet() As NormalizerRuleSet
            If _selectedIndex < 0 OrElse _selectedIndex >= _config.RuleSets.Count Then Return Nothing
            Return _config.RuleSets(_selectedIndex)
        End Function

        Private Sub CommitMatchToModel() Handles matchBox.TextChanged
            If _suppress Then Return
            Dim rs = CurrentRuleSet()
            If rs Is Nothing Then Return
            rs.Match = matchBox.Text.Trim()
            _suppress = True
            If _selectedIndex >= 0 AndAlso _selectedIndex < ruleList.Items.Count Then
                ruleList.Items(_selectedIndex) = "  " & DisplayMatch(rs.Match)
            End If
            _suppress = False
        End Sub

        ''' <summary>Rebuild the rule set's normalizations from the grid. Columns
        ''' whose Transforms cell is blank contribute nothing. Rules for columns not
        ''' shown in the grid (i.e. belonging to another file) are preserved — except
        ''' on the active file's rule set, where such orphans are intentionally dropped.</summary>
        Private Sub CommitNormalizationsToModel()
            Dim rs = CurrentRuleSet()
            If rs Is Nothing Then Return
            Dim shown = ActiveColumnSet()

            Dim fromGrid As New List(Of ColumnNormalization)()
            For Each row As DataGridViewRow In nzGrid.Rows
                If row.IsNewRow Then Continue For
                Dim col = Convert.ToString(row.Cells(0).Value)
                If String.IsNullOrWhiteSpace(col) Then Continue For
                Dim tfRaw = Convert.ToString(row.Cells(1).Value)
                If String.IsNullOrWhiteSpace(tfRaw) Then Continue For   ' blank -> no transform
                Dim transforms As New List(Of String)()
                For Each part In tfRaw.Split("|"c)
                    If part.Trim().Length > 0 Then transforms.Add(part.Trim())
                Next
                If transforms.Count = 0 Then Continue For
                fromGrid.Add(New ColumnNormalization With {.Column = col.Trim(), .Transforms = transforms})
            Next

            ' Preserve rules for columns this grid never showed (another file's
            ' schema), unless this is the active file's set (orphans pruned there).
            Dim preserved As New List(Of ColumnNormalization)()
            If Not IsActiveFileRuleSet(rs) Then
                For Each n In rs.Normalizations
                    If Not shown.Contains(n.Column) Then preserved.Add(n)
                Next
            End If

            rs.Normalizations = fromGrid.Concat(preserved).ToList()
        End Sub

        Private Sub OnBuildEnabledChanged(sender As Object, e As EventArgs) Handles buildEnable.CheckedChanged
            SetBuildEnabled(buildEnable.Checked)
            CommitBuildToModel()
        End Sub

        Private Sub CommitBuildToModel() Handles targetBox.TextChanged, sourcesBox.TextChanged,
                                               sepBox.TextChanged, skipEmptyChk.CheckedChanged
            If _suppress Then Return
            Dim rs = CurrentRuleSet()
            If rs Is Nothing Then Return
            If Not buildEnable.Checked Then
                rs.Build = Nothing
                Return
            End If
            Dim sources As New List(Of String)()
            For Each part In sourcesBox.Text.Split("|"c)
                If part.Trim().Length > 0 Then sources.Add(part.Trim())
            Next
            rs.Build = New BuildRule With {
                .Target = targetBox.Text.Trim(),
                .Sources = sources,
                .Separator = If(sepBox.Text.Length = 0, "-", sepBox.Text),
                .SkipEmpty = skipEmptyChk.Checked
            }
        End Sub

        Private Sub OnOk(sender As Object, e As EventArgs) Handles okBtn.Click
            ' Flush any in-progress grid edit, then commit all editors to the model.
            nzGrid.EndEdit()
            CommitMatchToModel()
            CommitNormalizationsToModel()
            CommitBuildToModel()
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

    End Class

End Namespace
