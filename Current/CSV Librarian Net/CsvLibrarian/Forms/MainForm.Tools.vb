Imports System.Data
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports CsvLibrarian.Models
Imports CsvLibrarian.Services
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>
    ''' Tools-menu analysis/reporting features split out of MainForm for readability:
    ''' Location Analysis, Integrity Check, Location Report and Collated BOM. These are a
    ''' partial of <see cref="MainForm"/> so they share its private state (_order, _docs,
    ''' _folder, the grid, SetStatus, …).
    ''' </summary>
    Partial Public Class MainForm

        ''' <summary>Every open document, in sidebar order. Shared by the analysis tools.</summary>
        Private Iterator Function OpenDocuments() As IEnumerable(Of CsvDocument)
            For Each docName In _order
                Yield _docs(docName)
            Next
        End Function

        Private Sub LocationAnalysisToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LocationAnalysisToolStripMenuItem.Click
            AnalyzeLocations()
        End Sub

        ''' <summary>
        ''' Scan the "Location" column across every open CSV file, group the values by
        ''' their leading (non-digit) prefix, and report any missing numbers within each
        ''' prefix's used range. A location is a prefix followed by a run of digits, e.g.
        ''' "A0012" or "MARS10"; leading zeros are ignored when comparing ("Q0012" = "Q12").
        ''' Only gaps *between* the lowest and highest number actually seen for a prefix are
        ''' reported (e.g. A0010, A0011, A0013 → A0012 missing). The comma-separated list of
        ''' missing locations is shown in a popup and copied to the Windows clipboard.
        ''' A cell may hold several comma-separated locations — each is analyzed separately
        ''' (the source table is never changed). Any location containing a hyphen is flagged
        ''' (with its file) as malformed and excluded from the gap analysis.
        ''' </summary>
        Private Sub AnalyzeLocations()
            ' A present entry: its numeric value plus the width of its digit text, so a
            ' reconstructed missing entry can copy the zero-padding of the value below it.
            Dim groups As New Dictionary(Of String, SortedDictionary(Of Long, Integer))(StringComparer.Ordinal)
            Dim rx As New Regex("^(.*?)(\d+)$")
            Dim sawLocationColumn As Boolean = False
            ' Locations containing a hyphen are malformed — collected here to flag, not analyze.
            Dim hyphenFlags As New List(Of String)()
            Dim hyphenSeen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            ' Every valid location → the file(s) it appears in, for duplicate detection.
            Dim occurrences As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

            For Each doc In OpenDocuments()
                Dim table = doc.Table
                If table Is Nothing OrElse Not table.Columns.Contains("Location") Then Continue For
                sawLocationColumn = True
                For Each row As DataRow In table.Rows
                    Dim raw = Convert.ToString(row("Location"))
                    If String.IsNullOrWhiteSpace(raw) Then Continue For
                    ' A cell may hold several comma-separated locations — split for analysis
                    ' only; the source table is never modified.
                    For Each part In raw.Split(","c)
                        Dim loc = part.Trim()
                        If loc.Length = 0 Then Continue For

                        ' Hyphens shouldn't appear in a location — flag it, skip the gap check.
                        If loc.IndexOf("-"c) >= 0 Then
                            Dim flag = $"{loc}  ({doc.FileName})"
                            If hyphenSeen.Add(flag) Then hyphenFlags.Add(flag)
                            Continue For
                        End If

                        ' Record the occurrence (by exact text) for duplicate detection.
                        Dim occ As List(Of String) = Nothing
                        If Not occurrences.TryGetValue(loc, occ) Then
                            occ = New List(Of String)()
                            occurrences(loc) = occ
                        End If
                        occ.Add(doc.FileName)

                        Dim m = rx.Match(loc)
                        If Not m.Success Then Continue For          ' no trailing digits — skip
                        Dim prefix = m.Groups(1).Value
                        Dim digits = m.Groups(2).Value
                        Dim value As Long
                        If Not Long.TryParse(digits, value) Then Continue For
                        Dim byNumber As SortedDictionary(Of Long, Integer) = Nothing
                        If Not groups.TryGetValue(prefix, byNumber) Then
                            byNumber = New SortedDictionary(Of Long, Integer)()
                            groups(prefix) = byNumber
                        End If
                        ' Keep the narrowest digit width seen for this number (stable).
                        If Not byNumber.ContainsKey(value) OrElse digits.Length < byNumber(value) Then
                            byNumber(value) = digits.Length
                        End If
                    Next
                Next
            Next

            If Not sawLocationColumn Then
                ConfirmDialog.Notify(Me, "Location Analysis",
                    "None of the open files have a ""Location"" column to analyze.", icon:=DialogIcon.Warning)
                Return
            End If

            ' Walk each prefix's sorted numbers and collect the gaps between neighbours.
            Dim missing As New List(Of String)()
            For Each prefix In groups.Keys.OrderBy(Function(p) p, StringComparer.Ordinal)
                Dim byNumber = groups(prefix)
                Dim prevValue As Long = -1
                Dim prevWidth As Integer = 0
                Dim first As Boolean = True
                For Each kv In byNumber
                    If Not first Then
                        For gap As Long = prevValue + 1 To kv.Key - 1
                            ' Pad the missing number to the width of the value just below it,
                            ' so it inherits that neighbour's zero-padding (A0011 → A0012).
                            missing.Add(prefix & gap.ToString().PadLeft(prevWidth, "0"c))
                        Next
                    End If
                    prevValue = kv.Key
                    prevWidth = kv.Value
                    first = False
                Next
            Next

            ' Locations used more than once (same bin assigned to multiple parts/files).
            Dim dups = occurrences.Where(Function(kv) kv.Value.Count > 1) _
                                  .OrderBy(Function(kv) kv.Key, StringComparer.OrdinalIgnoreCase) _
                                  .ToList()

            ' Combined report: hyphens, then duplicates, then the missing-location gaps.
            Dim report As New StringBuilder()
            If hyphenFlags.Count > 0 Then
                hyphenFlags.Sort(StringComparer.OrdinalIgnoreCase)
                report.AppendLine($"{hyphenFlags.Count} location(s) contain a hyphen ""-"" — these should be removed:")
                report.AppendLine()
                report.AppendLine(String.Join(vbCrLf, hyphenFlags))
                report.AppendLine()
                report.AppendLine()
            End If

            If dups.Count > 0 Then
                report.AppendLine($"{dups.Count} duplicate location(s) — the same location is used more than once:")
                report.AppendLine()
                For Each kv In dups
                    report.AppendLine(kv.Key)
                    For Each fn In kv.Value
                        report.AppendLine("    " & fn)
                    Next
                Next
                report.AppendLine()
            End If

            If missing.Count = 0 Then
                report.Append("No gaps found — every ""Location"" sequence is contiguous.")
            Else
                report.AppendLine($"{missing.Count} missing location{If(missing.Count = 1, "", "s")} (copied to clipboard):")
                report.AppendLine()
                report.Append(String.Join(", ", missing))
                Try
                    ' Clipboard gets one entry per line (CR/LF); the popup keeps the commas.
                    Clipboard.SetText(String.Join(vbCrLf, missing))
                Catch
                    ' Clipboard can transiently fail (locked by another app) — non-fatal.
                End Try
            End If

            Dim icon = If(hyphenFlags.Count > 0 OrElse dups.Count > 0, DialogIcon.Warning, DialogIcon.Information)

            ' When there are gaps, offer to print the missing list; otherwise just show it.
            If missing.Count > 0 Then
                If ConfirmDialog.Ask(Me, "Location Analysis", report.ToString().TrimEnd(),
                                     okText:="Print…", cancelText:="Close", icon:=icon) Then
                    ReportGenerator.GenerateMissingLocationReport(Me, missing,
                        Sub(msg)
                            SetStatus(msg, Palette.TextMuted)
                            statusStrip.Refresh()
                        End Sub)
                End If
            Else
                ConfirmDialog.Notify(Me, "Location Analysis", report.ToString().TrimEnd(), icon:=icon)
            End If

            Dim summary As New List(Of String)()
            If missing.Count > 0 Then summary.Add($"{missing.Count} missing")
            If dups.Count > 0 Then summary.Add($"{dups.Count} duplicate")
            If hyphenFlags.Count > 0 Then summary.Add($"{hyphenFlags.Count} with hyphen")
            If summary.Count = 0 Then
                SetStatus("Location analysis: no gaps", Palette.Success)
            Else
                SetStatus("Location analysis: " & String.Join(", ", summary), Palette.Warning)
            End If
        End Sub

        Private Sub IntegrityCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntegrityCheckToolStripMenuItem.Click
            IntegrityCheck()
        End Sub

        ''' <summary>
        ''' Collect the GUID strings from the index column (first column) of every open
        ''' CSV file and check that no GUID appears more than once across all files. Any
        ''' GUID found in two or more rows/files is reported — in a popup grouped by GUID
        ''' with the file(s) it lives in, and on the clipboard as "GUID : filename" lines
        ''' (one per occurrence, CR/LF separated). Reports "Index Clean" when all unique.
        ''' </summary>
        Private Sub IntegrityCheck()
            ' GUID → the file name of each row it appears in (repeats kept, so an in-file
            ' duplicate shows the same file twice — that is still a genuine collision).
            Dim occurrences As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each doc In OpenDocuments()
                Dim table = doc.Table
                If table Is Nothing OrElse table.Columns.Count = 0 Then Continue For
                For Each row As DataRow In table.Rows
                    Dim guid = Convert.ToString(row(0)).Trim()
                    If guid.Length = 0 Then Continue For        ' ignore blank GUID cells
                    Dim files As List(Of String) = Nothing
                    If Not occurrences.TryGetValue(guid, files) Then
                        files = New List(Of String)()
                        occurrences(guid) = files
                    End If
                    files.Add(doc.FileName)
                Next
            Next

            ' A GUID is a duplicate when it was seen in more than one row (any file).
            Dim dups = occurrences.Where(Function(kv) kv.Value.Count > 1) _
                                  .OrderBy(Function(kv) kv.Key, StringComparer.Ordinal) _
                                  .ToList()

            If dups.Count = 0 Then
                SetIntegrityIcon(IntegrityState.Pass)
                ConfirmDialog.Notify(Me, "Integrity Check", "Index Clean")
                SetStatus("Integrity check: index clean", Palette.Success)
                Return
            End If

            ' Popup: grouped by GUID with its file list. Clipboard: one "GUID : file" per row.
            Dim popup As New StringBuilder()
            Dim clip As New StringBuilder()
            For Each kv In dups
                popup.AppendLine(kv.Key)
                For Each id_fileName In kv.Value
                    popup.AppendLine("    " & id_fileName)
                    clip.AppendLine(kv.Key & " : " & id_fileName)
                Next
                popup.AppendLine()
            Next

            Try
                Clipboard.SetText(clip.ToString().TrimEnd())
            Catch
                ' Clipboard can transiently fail (locked by another app) — non-fatal.
            End Try

            SetIntegrityIcon(IntegrityState.Failed)
            ConfirmDialog.Notify(Me, "Integrity Check",
                $"{dups.Count} duplicate GUID{If(dups.Count = 1, "", "s")} found (copied to clipboard):" &
                vbCrLf & vbCrLf & popup.ToString().TrimEnd(), icon:=DialogIcon.Warning)
            SetStatus($"Integrity check: {dups.Count} duplicate GUID(s)", Palette.Warning)
        End Sub

        Private Sub LocationReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LocationReportToolStripMenuItem.Click
            LocationReport()
        End Sub

        ''' <summary>
        ''' Gather Description / Manufacturer Part Number 1 / Location from every open file
        ''' and hand them to <see cref="ReportGenerator"/> to sort, paginate and print. A
        ''' file is included when it has at least one of the three columns; a missing column
        ''' yields a blank cell, and rows blank in all three are dropped.
        ''' </summary>
        Private Sub LocationReport()
            Dim files As New List(Of LocationReportFile)()
            For Each doc In OpenDocuments()
                Dim table = doc.Table
                If table Is Nothing Then Continue For
                Dim hasDesc = table.Columns.Contains("Description")
                Dim hasMpn = table.Columns.Contains("Manufacturer Part Number 1")
                Dim hasLoc = table.Columns.Contains("Location")
                If Not (hasDesc OrElse hasMpn OrElse hasLoc) Then Continue For

                Dim rows As New List(Of String())()
                For Each row As DataRow In table.Rows
                    Dim d = If(hasDesc, Convert.ToString(row("Description")), "")
                    Dim m = If(hasMpn, Convert.ToString(row("Manufacturer Part Number 1")), "")
                    Dim l = If(hasLoc, Convert.ToString(row("Location")), "")
                    If String.IsNullOrWhiteSpace(d) AndAlso String.IsNullOrWhiteSpace(m) AndAlso String.IsNullOrWhiteSpace(l) Then Continue For
                    rows.Add(New String() {d, m, l})
                Next

                files.Add(New LocationReportFile With {.FileName = doc.FileName, .Rows = rows})
            Next

            If files.Count = 0 Then
                ' No candidate files — let the generator show the "open a folder" notice.
                ReportGenerator.GenerateLocationReport(Me, files)
                Return
            End If

            ' Let the user choose which files to include (transient — not persisted).
            Using picker As New PrintSelectForm(files)
                If picker.ShowDialog(Me) <> DialogResult.OK Then Return
                Dim chosen = picker.SelectedFiles
                If chosen.Count = 0 Then
                    ConfirmDialog.Notify(Me, "Location Report", "No files were selected for the report.")
                    Return
                End If
                ReportGenerator.GenerateLocationReport(Me, chosen,
                    Sub(msg)
                        SetStatus(msg, Palette.TextMuted)
                        statusStrip.Refresh()   ' force the label to repaint during the blocking Print()
                    End Sub)
            End Using
        End Sub

        Private Sub CollatedBomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CollatedBomToolStripMenuItem.Click
            CollatedBom()
        End Sub

        Private Sub FindLocationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindLocationToolStripMenuItem.Click
            FindLocation()
        End Sub

        ''' <summary>
        ''' Prompt for a string and report which open files have it in their "Location" column
        ''' (case-insensitive substring). Reports the file names (and per-file match counts);
        ''' does not open anything.
        ''' </summary>
        Private Sub FindLocation()
            If Not OpenDocuments().Any() Then
                ConfirmDialog.Notify(Me, "Find Location", "Open a folder with CSV files first.", icon:=DialogIcon.Warning)
                Return
            End If

            Dim query = InputDialog.Prompt(Me, "Find Location", "Enter a location to search for:",
                                           icon:=InputIcon.Find)
            If query Is Nothing Then Return          ' cancelled
            query = query.Trim()
            If query.Length = 0 Then Return

            Dim hits As New List(Of String)()
            Dim sawLocationColumn As Boolean = False
            For Each doc In OpenDocuments()
                Dim table = doc.Table
                If table Is Nothing OrElse Not table.Columns.Contains("Location") Then Continue For
                sawLocationColumn = True
                Dim count As Integer = 0
                For Each row As DataRow In table.Rows
                    Dim loc = Convert.ToString(row("Location"))
                    If Not String.IsNullOrEmpty(loc) AndAlso
                       loc.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 Then
                        count += 1
                    End If
                Next
                If count > 0 Then hits.Add($"{doc.FileName}  ({count} match{If(count = 1, "", "es")})")
            Next

            If Not sawLocationColumn Then
                ConfirmDialog.Notify(Me, "Find Location",
                    "None of the open files have a ""Location"" column to search.", icon:=DialogIcon.Warning)
                Return
            End If

            If hits.Count = 0 Then
                ConfirmDialog.Notify(Me, "Find Location", $"""{query}"" was not found in any Location field.")
                SetStatus($"Find location: ""{query}"" not found", Palette.TextMuted)
            Else
                ConfirmDialog.Notify(Me, "Find Location",
                    $"""{query}"" found in:" & vbCrLf & vbCrLf & String.Join(vbCrLf, hits))
                SetStatus($"Find location: ""{query}"" in {hits.Count} file(s)", Palette.Success)
            End If
        End Sub

        ''' <summary>
        ''' Read every CSV in the working folder (except collated.csv itself), keep only
        ''' the Description / Manufacturer 1 / Manufacturer Part Number 1 columns, drop
        ''' rows whose Manufacturer Part Number 1 duplicates an earlier row *with the same
        ''' Description*, sort by Description, and write the result to collated.csv
        ''' (overwriting). Reads from disk so it reflects the on-disk files.
        ''' </summary>
        Private Sub CollatedBom()
            If String.IsNullOrEmpty(_folder) OrElse Not Directory.Exists(_folder) Then
                ConfirmDialog.Notify(Me, "Collated BOM", "Open a working folder first.", icon:=DialogIcon.Warning)
                Return
            End If

            Const OutputName As String = "collated.csv"
            Dim outputPath = Path.Combine(_folder, OutputName)

            Dim sources = Directory.GetFiles(_folder, "*.csv") _
                .Where(Function(p) Not String.Equals(Path.GetFileName(p), OutputName, StringComparison.OrdinalIgnoreCase)) _
                .OrderBy(Function(p) p, StringComparer.OrdinalIgnoreCase) _
                .ToList()

            If sources.Count = 0 Then
                ConfirmDialog.Notify(Me, "Collated BOM", "There are no source CSV files to collate.", icon:=DialogIcon.Warning)
                Return
            End If

            Dim collected As New List(Of String())()
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)   ' key = MPN + Description
            Dim readErrors As New List(Of String)()

            For Each csvPath In sources
                Dim table As DataTable
                Try
                    table = CsvService.ReadCsv(csvPath)
                Catch ex As Exception
                    readErrors.Add(Path.GetFileName(csvPath))
                    Continue For
                End Try

                Dim hasDesc = table.Columns.Contains("Description")
                Dim hasMfr = table.Columns.Contains("Manufacturer 1")
                Dim hasMpn = table.Columns.Contains("Manufacturer Part Number 1")
                If Not (hasDesc OrElse hasMfr OrElse hasMpn) Then Continue For

                For Each row As DataRow In table.Rows
                    Dim desc = If(hasDesc, Convert.ToString(row("Description")), "")
                    Dim mfr = If(hasMfr, Convert.ToString(row("Manufacturer 1")), "")
                    Dim mpn = If(hasMpn, Convert.ToString(row("Manufacturer Part Number 1")), "")
                    If desc.Trim().Length = 0 AndAlso mfr.Trim().Length = 0 AndAlso mpn.Trim().Length = 0 Then Continue For

                    ' Duplicate only when BOTH the part number and the description match.
                    Dim key = mpn.Trim() & vbNullChar & desc.Trim()
                    If Not seen.Add(key) Then Continue For

                    collected.Add(New String() {desc, mfr, mpn})
                Next
            Next

            ' Sort by Description.
            Dim sorted = collected.OrderBy(Function(r) r(0), StringComparer.CurrentCultureIgnoreCase).ToList()

            ' Build the output table (header comes from the column names on write).
            Dim outTable As New DataTable("collated")
            outTable.Columns.Add("Description", GetType(String))
            outTable.Columns.Add("Manufacturer 1", GetType(String))
            outTable.Columns.Add("Manufacturer Part Number 1", GetType(String))
            For Each r In sorted
                outTable.Rows.Add(r(0), r(1), r(2))
            Next

            Try
                CsvService.WriteCsv(outputPath, outTable)
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Collated BOM", "Could not write collated.csv:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
                Return
            End Try

            Dim msg = $"collated.csv written: {sorted.Count} row(s) from {sources.Count} file(s)."
            If readErrors.Count > 0 Then
                msg &= vbCrLf & vbCrLf & $"Skipped (could not read): {String.Join(", ", readErrors)}"
            End If
            msg &= vbCrLf & vbCrLf & "Reopen the folder to see it in the file list."
            ConfirmDialog.Notify(Me, "Collated BOM", msg)
            SetStatus($"Collated BOM: {sorted.Count} row(s)", Palette.Success)
        End Sub

    End Class

End Namespace
