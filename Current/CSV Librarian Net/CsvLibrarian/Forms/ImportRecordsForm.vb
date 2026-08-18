Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Windows.Forms

Namespace Forms

    ''' <summary>
    ''' Non-modal viewer for ".EXPORT" record files in the working folder. Left: a list
    ''' of the export files (extension hidden). Right: a read-only two-column grid
    ''' (Field / Data) showing the selected file's JSON key/value pairs, styled like the
    ''' main grid. Close closes the window; Delete removes the selected file from disk;
    ''' Import is a placeholder for later.
    ''' </summary>
    Public Class ImportRecordsForm

        Private ReadOnly _folder As String
        ''' <summary>Full paths, parallel to <c>lstFiles.Items</c>.</summary>
        Private ReadOnly _files As New List(Of String)()
        ''' <summary>Imported flag per file, parallel to <c>lstFiles.Items</c>.</summary>
        Private ReadOnly _imported As New List(Of Boolean)()

        ''' <summary>JSON property that marks a file as already imported.</summary>
        Private Const ImportMarkerKey As String = "File imported"

        ''' <summary>The GUID/Index field name — never copied to the clipboard.</summary>
        Private Const GuidFieldName As String = "Index"

        Private Shared ReadOnly JsonOpts As New JsonSerializerOptions With {
            .WriteIndented = True,
            .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
            _folder = ""
            StyleGrid()
        End Sub

        Public Sub New(folder As String)
            InitializeComponent()
            _folder = folder
            StyleGrid()
            RefreshFileList()
            UpdateButtons()
        End Sub

        ''' <summary>Mirror the main grid: white/light-grey rows, dark-grey monospaced
        ''' text, medium-grey bold header, greyed read-only first column.</summary>
        Private Sub StyleGrid()
            fileGrid.BackgroundColor = Color.White
            fileGrid.DefaultCellStyle.BackColor = Color.White
            fileGrid.DefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60)
            fileGrid.DefaultCellStyle.Font = New Font("Consolas", 9.5F)
            fileGrid.DefaultCellStyle.Padding = New Padding(3, 0, 3, 0)
            fileGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
            fileGrid.EnableHeadersVisualStyles = False
            fileGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(190, 190, 190)
            fileGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            fileGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(190, 190, 190)
            fileGrid.ColumnHeadersDefaultCellStyle.Font = New Font("Consolas", 9.0F, FontStyle.Bold)
            fileGrid.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 4, 4, 4)
            fileGrid.RowTemplate.Height = 24
            ' First column: greyed, like the main GUID column.
            colField.DefaultCellStyle.BackColor = SystemColors.Control
            colField.DefaultCellStyle.ForeColor = SystemColors.GrayText
            ' Never copy to the Windows clipboard (and the grid is read-only anyway).
            fileGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable
        End Sub

        ''' <summary>Public hook so the main window can refresh the list after an
        ''' export writes a new .EXPORT file.</summary>
        Public Sub RefreshFiles()
            RefreshFileList()
            UpdateButtons()
        End Sub

        ''' <summary>List every *.EXPORT file in the folder (extension hidden).</summary>
        Private Sub RefreshFileList()
            ' Remember the current selection so we can restore it after a refresh.
            Dim previous As String = Nothing
            If lstFiles.SelectedIndex >= 0 AndAlso lstFiles.SelectedIndex < _files.Count Then
                previous = _files(lstFiles.SelectedIndex)
            End If

            lstFiles.BeginUpdate()
            lstFiles.Items.Clear()
            _files.Clear()
            _imported.Clear()
            If Not String.IsNullOrEmpty(_folder) AndAlso Directory.Exists(_folder) Then
                For Each f In Directory.GetFiles(_folder, "*.EXPORT").
                        OrderBy(Function(p) p, StringComparer.OrdinalIgnoreCase)
                    _files.Add(f)
                    _imported.Add(IsFileImported(f))
                    lstFiles.Items.Add(Path.GetFileNameWithoutExtension(f))
                Next
            End If
            lstFiles.EndUpdate()

            If previous IsNot Nothing Then
                Dim idx = _files.FindIndex(Function(p) String.Equals(p, previous, StringComparison.OrdinalIgnoreCase))
                If idx >= 0 Then lstFiles.SelectedIndex = idx
            End If
            If lstFiles.SelectedIndex < 0 Then LoadSelectedFile()   ' clears the grid
        End Sub

        ''' <summary>Read the selected file's JSON into the Field/Data grid, and reflect
        ''' whether it has already been imported (greyed out, Import disabled).</summary>
        Private Sub LoadSelectedFile()
            fileGrid.Rows.Clear()
            Dim idx = lstFiles.SelectedIndex
            If idx < 0 OrElse idx >= _files.Count Then
                ApplyImportedState(False)
                Return
            End If

            Dim imported = False
            Try
                Dim json = File.ReadAllText(_files(idx), Encoding.UTF8)
                Using doc = JsonDocument.Parse(json)
                    If doc.RootElement.ValueKind = JsonValueKind.Object Then
                        For Each prop In doc.RootElement.EnumerateObject()
                            If String.Equals(prop.Name, ImportMarkerKey, StringComparison.OrdinalIgnoreCase) Then imported = True
                            Dim val As String
                            If prop.Value.ValueKind = JsonValueKind.String Then
                                val = prop.Value.GetString()
                            Else
                                val = prop.Value.ToString()
                            End If
                            fileGrid.Rows.Add(prop.Name, val)
                        Next
                    End If
                End Using
            Catch ex As Exception
                fileGrid.Rows.Add("(could not read file)", ex.Message)
            End Try

            ApplyImportedState(imported)
        End Sub

        ''' <summary>Already-imported files show a 25% grey grid with Import disabled;
        ''' otherwise the normal styling is restored.</summary>
        Private Sub ApplyImportedState(imported As Boolean)
            If imported Then
                Dim g = Color.FromArgb(191, 191, 191)   ' 25% grey (25% black over white)
                fileGrid.DefaultCellStyle.BackColor = g
                fileGrid.AlternatingRowsDefaultCellStyle.BackColor = g
                fileGrid.ColumnHeadersDefaultCellStyle.BackColor = g
                colField.DefaultCellStyle.BackColor = g
                btnImport.Enabled = False
            Else
                StyleGrid()   ' restore the normal colours
                btnImport.Enabled = lstFiles.SelectedIndex >= 0
            End If
        End Sub

        Private Sub UpdateButtons()
            Dim hasSel = lstFiles.SelectedIndex >= 0
            btnDelete.Enabled = hasSel
            If Not hasSel Then btnImport.Enabled = False   ' import-enable is otherwise set by ApplyImportedState
        End Sub

        ' ── Event handlers ─────────────────────────────────────────────────────────

        Private Sub lstFiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstFiles.SelectedIndexChanged
            LoadSelectedFile()
            UpdateButtons()
        End Sub

        ''' <summary>Clicking a Data (second column) cell copies its contents to the
        ''' app clipboard (AppInfo.OurClipboard), like Copy on the main grid. Works
        ''' regardless of the file's imported state or cell highlighting.</summary>
        Private Sub fileGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles fileGrid.CellClick
            If e.RowIndex < 0 OrElse e.ColumnIndex <> colData.Index Then Return
            ' Never copy the GUID/Index value.
            Dim field = Convert.ToString(fileGrid.Rows(e.RowIndex).Cells(colField.Index).Value)
            If String.Equals(field, GuidFieldName, StringComparison.OrdinalIgnoreCase) Then Return
            Dim v = fileGrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
            AppInfo.OurClipboard = If(v Is Nothing, "", v.ToString())
            Dim main = TryCast(Me.Owner, MainForm)
            If main IsNot Nothing Then main.RefreshClipboardStatus()
        End Sub

        ''' <summary>Give files that carry the "File imported" marker a grey background
        ''' (black text, still selectable) — just a visual cue.</summary>
        Private Sub lstFiles_DrawItem(sender As Object, e As DrawItemEventArgs) Handles lstFiles.DrawItem
            If e.Index < 0 Then Return
            Dim text = lstFiles.Items(e.Index).ToString()
            Dim selected = (e.State And DrawItemState.Selected) = DrawItemState.Selected
            Dim imported = e.Index < _imported.Count AndAlso _imported(e.Index)

            Dim backColor As Color, foreColor As Color
            If selected Then
                backColor = SystemColors.Highlight
                foreColor = SystemColors.HighlightText
            ElseIf imported Then
                backColor = Color.FromArgb(210, 210, 210)   ' grey background cue
                foreColor = lstFiles.ForeColor
            Else
                backColor = lstFiles.BackColor
                foreColor = lstFiles.ForeColor
            End If

            Using b As New SolidBrush(backColor)
                e.Graphics.FillRectangle(b, e.Bounds)
            End Using
            TextRenderer.DrawText(e.Graphics, text, lstFiles.Font, e.Bounds, foreColor,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
            If selected Then e.DrawFocusRectangle()
        End Sub

        ''' <summary>True if the file carries the "File imported" marker.</summary>
        Private Shared Function IsFileImported(filePath As String) As Boolean
            Try
                Dim json = File.ReadAllText(filePath, Encoding.UTF8)
                Using doc = JsonDocument.Parse(json)
                    If doc.RootElement.ValueKind = JsonValueKind.Object Then
                        Dim tmp As JsonElement
                        Return doc.RootElement.TryGetProperty(ImportMarkerKey, tmp)
                    End If
                End Using
            Catch
                ' Unreadable -> treat as not imported.
            End Try
            Return False
        End Function

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
            Dim idx = lstFiles.SelectedIndex
            If idx < 0 OrElse idx >= _files.Count Then Return
            Dim filePath = _files(idx)
            Dim r = MessageBox.Show(Me,
                $"Delete ""{Path.GetFileName(filePath)}"" from disk? This cannot be undone.",
                "Delete Export File", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If r <> DialogResult.Yes Then Return
            Try
                File.Delete(filePath)
            Catch ex As Exception
                MessageBox.Show(Me, "Could not delete the file:" & vbCrLf & ex.Message,
                                "Delete Export File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try
            fileGrid.Rows.Clear()
            RefreshFileList()
            UpdateButtons()
        End Sub

        Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
            Dim idx = lstFiles.SelectedIndex
            If idx < 0 OrElse idx >= _files.Count Then Return
            Dim filePath = _files(idx)

            ' Build the record from the displayed rows, skipping the import marker.
            Dim record As New Dictionary(Of String, String)()
            For Each row As DataGridViewRow In fileGrid.Rows
                Dim field = Convert.ToString(row.Cells(colField.Index).Value)
                If String.IsNullOrEmpty(field) Then Continue For
                If String.Equals(field, ImportMarkerKey, StringComparison.OrdinalIgnoreCase) Then Continue For
                record(field) = Convert.ToString(row.Cells(colData.Index).Value)
            Next

            ' Push into the main window's grid.
            Dim main = TryCast(Me.Owner, MainForm)
            If main Is Nothing Then Return
            Dim matched = main.ImportRecordIntoGrid(record)
            If matched Is Nothing Then
                MessageBox.Show(Me, "Open a CSV file in the main window before importing.",
                                "Import Records", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' Highlight the imported (matching) fields' data cells in light green.
            Dim matchedSet As New HashSet(Of String)(matched, StringComparer.OrdinalIgnoreCase)
            For Each row As DataGridViewRow In fileGrid.Rows
                Dim field = Convert.ToString(row.Cells(colField.Index).Value)
                If matchedSet.Contains(field) Then
                    row.Cells(colData.Index).Style.BackColor = Color.LightGreen
                End If
            Next

            ' Mark the file so it can't be re-imported, and lock the Import button.
            Try
                MarkFileImported(filePath)
            Catch ex As Exception
                MessageBox.Show(Me, "The record was imported, but the file could not be marked:" &
                                vbCrLf & ex.Message,
                                "Import Records", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Try
            btnImport.Enabled = False

            ' Grey the file in the list now (keep the green grid feedback in place).
            If idx < _imported.Count Then _imported(idx) = True
            lstFiles.Invalidate()
        End Sub

        ''' <summary>Rewrite the file with the "File imported" marker added, preserving
        ''' its existing key/value pairs.</summary>
        Private Shared Sub MarkFileImported(filePath As String)
            Dim record As New Dictionary(Of String, String)()
            Dim json = File.ReadAllText(filePath, Encoding.UTF8)
            Using doc = JsonDocument.Parse(json)
                If doc.RootElement.ValueKind = JsonValueKind.Object Then
                    For Each prop In doc.RootElement.EnumerateObject()
                        If String.Equals(prop.Name, ImportMarkerKey, StringComparison.OrdinalIgnoreCase) Then Continue For
                        If prop.Value.ValueKind = JsonValueKind.String Then
                            record(prop.Name) = prop.Value.GetString()
                        Else
                            record(prop.Name) = prop.Value.ToString()
                        End If
                    Next
                End If
            End Using
            record(ImportMarkerKey) = "true"
            File.WriteAllText(filePath, JsonSerializer.Serialize(record, JsonOpts), New UTF8Encoding(False))
        End Sub

    End Class

End Namespace
