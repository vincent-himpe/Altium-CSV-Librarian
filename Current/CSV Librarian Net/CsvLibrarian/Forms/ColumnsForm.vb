Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports VbInteraction = Microsoft.VisualBasic.Interaction

Namespace Forms

    ''' <summary>
    ''' Add / rename / delete / reorder the columns of the active table.
    ''' The standard library fields (see <see cref="AppInfo.NewLibraryHeaders"/>)
    ''' cannot be renamed or deleted, and the first column (the GUID) always stays
    ''' first. Uses the standard Windows colour scheme (no theming).
    ''' On OK, <see cref="BuildResult"/> produces a new DataTable with the desired
    ''' shape, preserving existing cell data by original column name.
    ''' </summary>
    Public Class ColumnsForm

        Private Class ColEntry
            Public Property Name As String
            ''' <summary>Existing source column name, or Nothing for a new column.</summary>
            Public Property OriginalName As String
        End Class

        Private ReadOnly _entries As New List(Of ColEntry)()
        ''' <summary>Working copy of the source table (holds the data, and any Migrate
        ''' edits). Kept private so Cancel discards everything.</summary>
        Private _work As DataTable = Nothing
        Private _changed As Boolean = False
        ''' <summary>Number of leading locked (protected) fields; the movable region
        ''' is everything from this index onward.</summary>
        Private _lockedCount As Integer = 0

        ''' <summary>Field names that cannot be renamed or deleted.</summary>
        Private Shared ReadOnly _protected As HashSet(Of String) = BuildProtected()

        Private Shared Function BuildProtected() As HashSet(Of String)
            Dim result As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each h In AppInfo.NewLibraryHeaders.Split(","c)
                result.Add(h.Trim())
            Next
            Return result
        End Function

        Private Shared Function IsProtected(name As String) As Boolean
            Return _protected.Contains(name)
        End Function

        Public ReadOnly Property Changed As Boolean
            Get
                Return _changed
            End Get
        End Property

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(source As DataTable)
            InitializeComponent()
            _work = source.Copy()          ' deep copy: schema + data (Migrate edits this)
            For Each c As DataColumn In source.Columns
                _entries.Add(New ColEntry With {.Name = c.ColumnName, .OriginalName = c.ColumnName})
            Next
            RefreshList(0)
        End Sub

        Private Sub RefreshList(selectIndex As Integer)
            ' Count the leading locked fields (they are always contiguous at the front).
            _lockedCount = 0
            While _lockedCount < _entries.Count AndAlso IsProtected(_entries(_lockedCount).Name)
                _lockedCount += 1
            End While

            lstColumns.BeginUpdate()
            lstColumns.Items.Clear()
            For i As Integer = 0 To _entries.Count - 1
                ' The GUID (first column) is shown as "Name [GUID]"; the stored
                ' field name is unchanged.
                Dim label = _entries(i).Name
                If i = 0 Then label &= "   [GUID]"
                lstColumns.Items.Add(label)
            Next
            lstColumns.EndUpdate()
            If _entries.Count > 0 Then
                lstColumns.SelectedIndex = Math.Max(0, Math.Min(selectIndex, _entries.Count - 1))
            End If
            UpdateButtons()
        End Sub

        Private Sub UpdateButtons()
            Dim idx = lstColumns.SelectedIndex
            Dim hasSel = idx >= 0
            Dim locked = hasSel AndAlso IsProtected(_entries(idx).Name)
            btnRename.Enabled = hasSel AndAlso Not locked
            btnDelete.Enabled = hasSel AndAlso Not locked AndAlso _entries.Count > 1
            ' Movable fields can only be reordered within the movable region
            ' (they can't cross into the locked block, and locked fields can't move).
            btnUp.Enabled = hasSel AndAlso Not locked AndAlso idx > _lockedCount
            btnDown.Enabled = hasSel AndAlso Not locked AndAlso idx < _entries.Count - 1
        End Sub

        ''' <summary>Owner-draw so locked fields read dark-grey-on-light-grey and a
        ''' divider is drawn between the locked block and the movable fields.</summary>
        Private Sub lstColumns_DrawItem(sender As Object, e As DrawItemEventArgs) Handles lstColumns.DrawItem
            If e.Index < 0 Then Return
            Dim text = lstColumns.Items(e.Index).ToString()
            Dim isLocked = e.Index < _entries.Count AndAlso IsProtected(_entries(e.Index).Name)
            Dim selected = (e.State And DrawItemState.Selected) = DrawItemState.Selected

            Dim backColor As Color
            Dim foreColor As Color
            If selected Then
                backColor = SystemColors.Highlight
                foreColor = SystemColors.HighlightText
            ElseIf isLocked Then
                backColor = Color.FromArgb(235, 235, 235)   ' light grey
                foreColor = Color.FromArgb(90, 90, 90)       ' dark grey
            Else
                backColor = lstColumns.BackColor
                foreColor = lstColumns.ForeColor
            End If

            Using b As New SolidBrush(backColor)
                e.Graphics.FillRectangle(b, e.Bounds)
            End Using

            ' Divider line at the top of the first movable field.
            If _lockedCount > 0 AndAlso _lockedCount < _entries.Count AndAlso e.Index = _lockedCount Then
                Using p As New Pen(Color.Gray)
                    e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Top)
                End Using
            End If

            Dim textBounds As New Rectangle(e.Bounds.Left + 4, e.Bounds.Top,
                                            e.Bounds.Width - 4, e.Bounds.Height)
            TextRenderer.DrawText(e.Graphics, text, lstColumns.Font, textBounds, foreColor,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)

            If selected Then e.DrawFocusRectangle()
        End Sub

        Private Function NameExists(name As String, excludeIndex As Integer) As Boolean
            For i As Integer = 0 To _entries.Count - 1
                If i <> excludeIndex AndAlso
                   String.Equals(_entries(i).Name, name, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Function PromptName(title As String, defaultValue As String) As String
            Return VbInteraction.InputBox("Column name:", title, defaultValue).Trim()
        End Function

        Private Sub AddColumn()
            Dim name = PromptName("Add Column", "")
            If name.Length = 0 Then Return
            If NameExists(name, -1) Then
                MessageBox.Show(Me, "A column with that name already exists.", "Add Column",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            _entries.Add(New ColEntry With {.Name = name, .OriginalName = Nothing})
            _changed = True
            RefreshList(_entries.Count - 1)
        End Sub

        Private Sub RenameSelected()
            Dim idx = lstColumns.SelectedIndex
            If idx < 0 Then Return
            If IsProtected(_entries(idx).Name) Then Return       ' standard field — locked
            Dim name = PromptName("Rename Column", _entries(idx).Name)
            If name.Length = 0 Then Return
            If String.Equals(name, _entries(idx).Name, StringComparison.Ordinal) Then Return
            If NameExists(name, idx) Then
                MessageBox.Show(Me, "A column with that name already exists.", "Rename Column",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            _entries(idx).Name = name
            _changed = True
            RefreshList(idx)
        End Sub

        Private Sub DeleteSelected()
            Dim idx = lstColumns.SelectedIndex
            If idx <= 0 Then Return                      ' never the GUID column
            If IsProtected(_entries(idx).Name) Then Return  ' standard field — locked
            Dim r = MessageBox.Show(Me,
                $"Delete column ""{_entries(idx).Name}"" and its data?",
                "Delete Column", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If r <> DialogResult.Yes Then Return
            _entries.RemoveAt(idx)
            _changed = True
            RefreshList(idx - 1)
        End Sub

        Private Sub MoveSelected(delta As Integer)
            Dim idx = lstColumns.SelectedIndex
            If idx < 0 Then Return
            If IsProtected(_entries(idx).Name) Then Return          ' locked fields can't move
            Dim target = idx + delta
            ' Stay within the movable region — never cross into the locked block.
            If target < _lockedCount OrElse target >= _entries.Count Then Return
            Dim tmp = _entries(idx)
            _entries(idx) = _entries(target)
            _entries(target) = tmp
            _changed = True
            RefreshList(target)
        End Sub

        ''' <summary>Build a new DataTable with the chosen columns/order, copying cell
        ''' data (including any Migrate edits) across from the working copy by original
        ''' column name.</summary>
        Public Function BuildResult() As DataTable
            Dim result As New DataTable(_work.TableName)
            For Each entry In _entries
                result.Columns.Add(entry.Name, GetType(String))
            Next
            For Each srcRow As DataRow In _work.Rows
                Dim newRow = result.NewRow()
                For Each entry In _entries
                    If entry.OriginalName IsNot Nothing AndAlso _work.Columns.Contains(entry.OriginalName) Then
                        Dim v = srcRow(entry.OriginalName)
                        newRow(entry.Name) = If(v Is DBNull.Value, "", Convert.ToString(v))
                    Else
                        newRow(entry.Name) = ""
                    End If
                Next
                result.Rows.Add(newRow)
            Next
            Return result
        End Function

        ' ── Migrate ──────────────────────────────────────────────────────────────────

        ''' <summary>Find the entry whose current name matches (case-insensitive).</summary>
        Private Function EntryForName(name As String) As ColEntry
            For Each en In _entries
                If String.Equals(en.Name, name, StringComparison.OrdinalIgnoreCase) Then Return en
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Migrate the selected (source) column's data into a prompted target column,
        ''' per the chosen mode. Operates on the working copy, so it only takes effect
        ''' if the user clicks OK.
        '''   Merge   – copy source into target only where target is empty.
        '''   Replace – copy source into target for every row (overwrite).
        '''   Combine – if target empty copy source; else append ","&amp;source.
        ''' </summary>
        Private Sub Migrate()
            Dim idx = lstColumns.SelectedIndex
            If idx < 0 Then Return
            Dim srcEntry = _entries(idx)

            Dim targetName = VbInteraction.InputBox("Migrate data into which column?",
                                                    "Migrate", "").Trim()
            If targetName.Length = 0 Then Return

            Dim tgtEntry = EntryForName(targetName)
            If tgtEntry Is Nothing Then
                MessageBox.Show(Me, $"There is no column named ""{targetName}"".", "Migrate",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If tgtEntry Is srcEntry Then
                MessageBox.Show(Me, "The source and target columns are the same.", "Migrate",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            If _entries.IndexOf(tgtEntry) = 0 Then
                MessageBox.Show(Me, "The GUID/Index column can't be a migration target.", "Migrate",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Resolve to the working-table columns (keyed by original name).
            Dim srcCol = srcEntry.OriginalName
            Dim tgtCol = tgtEntry.OriginalName
            If srcCol Is Nothing OrElse tgtCol Is Nothing OrElse
               Not _work.Columns.Contains(srcCol) OrElse Not _work.Columns.Contains(tgtCol) Then
                MessageBox.Show(Me,
                    "Migration works on existing columns with data. A newly added column has no " &
                    "data yet — click OK to apply your column changes first, then reopen Manage Columns.",
                    "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim mode As String = "Merge"
            If rbReplace.Checked Then mode = "Replace"
            If rbCombine.Checked Then mode = "Combine"

            Dim n = 0
            For Each row As DataRow In _work.Rows
                Dim sVal = If(row(srcCol) Is DBNull.Value, "", Convert.ToString(row(srcCol)))
                Dim tVal = If(row(tgtCol) Is DBNull.Value, "", Convert.ToString(row(tgtCol)))
                Select Case mode
                    Case "Replace"
                        row(tgtCol) = sVal
                        n += 1
                    Case "Combine"
                        If sVal.Length > 0 Then
                            If tVal.Length = 0 Then
                                row(tgtCol) = sVal
                            Else
                                row(tgtCol) = tVal & "," & sVal
                            End If
                            n += 1
                        End If
                    Case Else   ' Merge
                        If tVal.Length = 0 AndAlso sVal.Length > 0 Then
                            row(tgtCol) = sVal
                            n += 1
                        End If
                End Select
            Next

            If n > 0 Then _changed = True
            MessageBox.Show(Me,
                $"Migrated ""{srcEntry.Name}"" → ""{tgtEntry.Name}"" ({mode}): {n} row(s) updated.",
                "Migrate", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        ' ── Event handlers ─────────────────────────────────────────────────────────

        Private Sub lstColumns_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstColumns.SelectedIndexChanged
            UpdateButtons()
        End Sub

        Private Sub lstColumns_DoubleClick(sender As Object, e As EventArgs) Handles lstColumns.DoubleClick
            RenameSelected()
        End Sub

        Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
            AddColumn()
        End Sub

        Private Sub btnRename_Click(sender As Object, e As EventArgs) Handles btnRename.Click
            RenameSelected()
        End Sub

        Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
            DeleteSelected()
        End Sub

        Private Sub btnUp_Click(sender As Object, e As EventArgs) Handles btnUp.Click
            MoveSelected(-1)
        End Sub

        Private Sub btnDown_Click(sender As Object, e As EventArgs) Handles btnDown.Click
            MoveSelected(1)
        End Sub

        Private Sub btnMigrate_Click(sender As Object, e As EventArgs) Handles btnMigrate.Click
            Migrate()
        End Sub

    End Class

End Namespace
