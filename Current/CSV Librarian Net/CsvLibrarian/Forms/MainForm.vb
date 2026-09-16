Imports System.Data
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports CsvLibrarian.Models
Imports CsvLibrarian.Services
Imports CsvLibrarian.Theme
Imports VbInteraction = Microsoft.VisualBasic.Interaction

Namespace Forms

    ''' <summary>
    ''' Main window: folder-based CSV editor with a dark theme, sidebar file list,
    ''' bound DataGridView, search/filter, editable columns, a generic normalizer,
    ''' autosave and rolling backups.
    '''
    ''' The control layout lives in MainForm.Designer.vb (so it renders on the VS
    ''' design surface); the dark theme is applied at runtime in <see cref="ApplyTheme"/>.
    ''' </summary>
    Public Class MainForm

        ' ── State ────────────────────────────────────────────────────────────────
        Private ReadOnly _docs As New Dictionary(Of String, CsvDocument)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _order As New List(Of String)()
        Private _activeName As String = Nothing
        Private _folder As String = Nothing
        Private _config As New NormalizerConfig()
        Private ReadOnly _settings As AppSettings = AppSettings.Load()

        Private _loading As Boolean = False        ' guards CellValueChanged during binds
        Private _autosaveRemaining As Integer = 0
        Private _startFolder As String = Nothing
        Private _baseCaption As String = "Altium CSV Librarian"  ' set from the title at startup
        Private ReadOnly _toolTip As New ToolTip()

        ' The non-modal Import Records window (single instance).
        Private _importForm As ImportRecordsForm = Nothing

        ' Row index currently painted with the light-cyan "current row" highlight.
        Private _highlightRow As Integer = -1

        ' Backup preview: when previewing, the grid shows a read-only, light "inverted"
        ' view of a backup file and the active document is left untouched.
        Private _previewing As Boolean = False
        Private _previewPath As String = Nothing
        Private _suppressBackupEvent As Boolean = False   ' guards cboBackups during repopulate
        Private Const CurrentLabel As String = "Current"
        Private Shared ReadOnly DividerItem As New Object()   ' sentinel: the separator row

        ' Right-click "search this part number" menu (built on demand from the web config).
        Private WithEvents _webMenu As New ContextMenuStrip()
        Private _webMenuCellText As String = ""
        Private ReadOnly _webMenuQueries As New List(Of String)()   ' enabled engines' queries, in menu order

        ' ── Construction ───────────────────────────────────────────────────────────

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
            ApplyTheme()
            ' Append the app version to the caption at startup, e.g.
            ' "Altium CSV Librarian V 0.1". The open file name is added later.
            Me.Text = $"{Me.Text} V {AppInfo.Version}"
            _baseCaption = Me.Text
            UpdateFindReplaceStatus()
            WireAdvancedClipboardCopyPaste()
        End Sub

        Public Sub New(startFolder As String)
            Me.New()
            _startFolder = startFolder
        End Sub

        ''' <summary>
        ''' The dark Altium-style colours are set directly on the controls in the
        ''' designer (or inherited from the form). This method only handles what the
        ''' designer can't cleanly do: fonts, the flat dark toolbar buttons (visual-
        ''' styled buttons ignore BackColor, so they need FlatStyle), the toolbar strip
        ''' shade, the data grid, tooltips, and the empty-state labels.
        ''' </summary>
        Private Sub ApplyTheme()
            Me.Font = New Font("Segoe UI", 9.0F)

            ' Dark menus/dropdowns/context menus (applies app-wide via the manager).
            ToolStripManager.Renderer = New DarkMenuRenderer()

            fileList.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
            lblBackups.Font = New Font("Segoe UI", 9.0F)
            lblBackups.ForeColor = Color.FromArgb(220, 220, 220)
            cboBackups.Font = New Font("Segoe UI", 9.0F)

            ' Toolbar strip a touch lighter than the form so it reads as a band.
            gridToolbar.BackColor = Color.FromArgb(62, 62, 66)
            rightCluster.BackColor = Color.FromArgb(62, 62, 66)
            backupCluster.BackColor = Color.FromArgb(62, 62, 66)
            divider.BackColor = Color.FromArgb(80, 80, 85)

            ' Toolbar buttons: flat, dark (FlatStyle is required for the BackColor to
            ' render — visual-styled buttons ignore it).
            DarkTheme.StyleFlatButtons(btnAutosize, btnNormalize, btnSave, btnSaveAll, btnUnsort, btnDeleteRow,
                                       btnLocationAnalysis, btnIntegrityCheck, btnLocationReport, btnCreateArchive, btnRestore)

            ' Glyph buttons use Segoe Fluent Icons → Segoe MDL2 Assets → Segoe UI Symbol.
            Dim glyphFont = DarkTheme.GlyphFont(12.0F, FontStyle.Bold)
            For Each b As Button In {btnAutosize, btnNormalize, btnSave, btnSaveAll, btnUnsort, btnDeleteRow,
                                     btnLocationAnalysis, btnIntegrityCheck, btnLocationReport, btnCreateArchive}
                b.Font = glyphFont
            Next
            btnAutosize.Text = ChrW(&HE799).ToString()          ' fit width
            btnNormalize.Text = ChrW(&HE794).ToString()         ' edit / normalize
            btnSave.Text = ChrW(&HE105).ToString()              ' save
            btnSaveAll.Text = ChrW(&HE105).ToString()           ' save all
            btnUnsort.Text = ChrW(&HE8B2).ToString()            ' remove sort
            btnDeleteRow.Text = ChrW(&HE74D).ToString()         ' delete
            btnLocationAnalysis.Text = ChrW(&HE163).ToString()  ' location analysis
            btnIntegrityCheck.Text = ChrW(&HF28B).ToString()    ' integrity check
            btnLocationReport.Text = ChrW(&HE749).ToString()    ' location report (print)
            btnCreateArchive.Text = ChrW(&HF012).ToString()     ' create archive
            btnRestore.Font = New Font("Segoe UI", 9.0F)

            ' Icon colours: light blue, except Save All (green) and Delete Row (red).
            For Each b As Button In {btnAutosize, btnNormalize, btnSave, btnUnsort,
                                     btnLocationAnalysis, btnIntegrityCheck, btnLocationReport, btnCreateArchive}
                b.ForeColor = DarkTheme.IconBlue
            Next
            btnSaveAll.ForeColor = DarkTheme.IconGreen
            btnDeleteRow.ForeColor = DarkTheme.IconRed

            _toolTip.SetToolTip(cboBackups, "Backups of the current file (newest first)")
            _toolTip.SetToolTip(btnRestore, "Load the selected backup into the grid (does not autosave)")
            _toolTip.SetToolTip(btnAutosize, "Autosize columns (Ctrl+Q)")
            _toolTip.SetToolTip(btnNormalize, "Normalize Description (Ctrl+N)")
            _toolTip.SetToolTip(btnSave, "Save (Ctrl+S)")
            _toolTip.SetToolTip(btnSaveAll, "Save All (Ctrl+Shift+S)")
            _toolTip.SetToolTip(btnUnsort, "Unsort — restore CSV order (Ctrl+U)")
            _toolTip.SetToolTip(btnDeleteRow, "Delete the current row (cannot be undone)")
            _toolTip.SetToolTip(btnLocationAnalysis, "Location Analysis")
            _toolTip.SetToolTip(btnIntegrityCheck, "Integrity Check")
            _toolTip.SetToolTip(btnLocationReport, "Location Report")
            _toolTip.SetToolTip(btnCreateArchive, "Create Archive")

            StyleGrid()
            ' Copy uses our own single-cell clipboard, never the Windows clipboard.
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable

            ' Empty-state labels (background inherited dark from the form).
            lblEmptyIcon.BackColor = Color.Transparent
            lblEmptyIcon.ForeColor = Color.FromArgb(90, 90, 96)
            lblEmptyIcon.Font = New Font("Segoe UI Emoji", 34.0F)
            lblEmpty1.BackColor = Color.Transparent
            lblEmpty1.ForeColor = Color.FromArgb(157, 157, 160)
            lblEmpty1.Font = New Font("Segoe UI", 14.0F)
            lblEmpty2.BackColor = Color.Transparent
            lblEmpty2.ForeColor = Color.FromArgb(120, 120, 124)
            lblEmpty2.Font = New Font("Segoe UI", 9.5F)
            CenterEmptyLabels()
        End Sub

        ''' <summary>Dark grid: light monospaced text on two shades of grey (alternating
        ''' rows); dark bold header; blue selection.</summary>
        Private Sub StyleGrid()
            DarkTheme.StyleGrid(grid)
            ' Main grid uses a monospaced cell/header font and a little padding.
            grid.DefaultCellStyle.Font = New Font("Consolas", 9.5F)
            grid.DefaultCellStyle.Padding = New Padding(3, 0, 3, 0)
            grid.ColumnHeadersDefaultCellStyle.Font = New Font("Consolas", 9.0F, FontStyle.Bold)
            grid.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 4, 4, 4)
            grid.RowTemplate.Height = 24
        End Sub

        Private Sub CenterEmptyLabels()
            lblEmptyIcon.Left = (emptyPanel.Width - lblEmptyIcon.Width) \ 2
            lblEmptyIcon.Top = 90
            lblEmpty1.Left = (emptyPanel.Width - lblEmpty1.Width) \ 2
            lblEmpty1.Top = 150
            lblEmpty2.Left = (emptyPanel.Width - lblEmpty2.Width) \ 2
            lblEmpty2.Top = 180
        End Sub

        Private Sub emptyPanel_Resize(sender As Object, e As EventArgs) Handles emptyPanel.Resize
            CenterEmptyLabels()
        End Sub

        ' ── Startup / shutdown ────────────────────────────────────────────────────

        Private Sub HandleFormLoad(sender As Object, e As EventArgs) Handles Me.Load
            ' Populate the global Advanced Clipboard from the settings JSON (fail-safe:
            ' blanks if the JSON has no such data yet).
            AppInfo.AdvancedClipboardFromJagged(_settings.AdvancedClipboard)
            UpdateAdvancedClipboardStatus()   ' show row 0 initially
            RefreshRecentMenu()

            Dim folder As String = _startFolder
            If String.IsNullOrEmpty(folder) AndAlso Not String.IsNullOrEmpty(_settings.LastFolder) Then
                folder = _settings.LastFolder
            End If

            If Not String.IsNullOrEmpty(folder) AndAlso Directory.Exists(folder) Then
                LoadFolder(folder)
            Else
                ShowEmpty()
                SetStatus("No folder open", Palette.TextMuted)
            End If
        End Sub

        Private Sub HandleFormShown(sender As Object, e As EventArgs) Handles Me.Shown
            ' Smooth repaints on scroll / row insert for large tables (no flicker/lag).
            NativeDark.EnableDoubleBuffer(grid)

            ' Dark scrollbars (best-effort; needs Win10 1809+ and created handles).
            NativeDark.ApplyDarkScrollbars(grid)
            NativeDark.ApplyDarkScrollbars(fileList)
            NativeDark.ApplyDarkScrollbars(cboBackups)
        End Sub

        Private Sub HandleFormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            grid.EndEdit()

            ' Unsaved work on exit always prompts (regardless of the autosave toggle).
            If _order.Any(Function(fn) _docs(fn).Dirty) Then
                Dim r = ConfirmDialog.Choose(Me, "Unsaved changes",
                    "You have unsaved changes. What would you like to do?",
                    yesText:="Save and Exit", noText:="Discard and Exit", cancelText:="Keep Editing")
                Select Case r
                    Case DialogResult.Cancel
                        e.Cancel = True
                        Return
                    Case DialogResult.Yes
                        SaveAllDirty()
                        ' Case No: leave files dirty (changes are discarded on close).
                End Select
            End If

            ' Optionally archive the working folder on the way out (best-effort).
            If _settings.ArchiveOnExit Then
                Try
                    BuildArchive()
                Catch
                    ' Never block exit on an archive failure.
                End Try
            End If

            ' Persist the Advanced Clipboard (same operation the form's Close uses).
            _settings.AdvancedClipboard = AppInfo.AdvancedClipboardToJagged()
            _settings.LastFolder = If(_folder, "")
            _settings.Save()
        End Sub

        ' ── Folder / file loading ─────────────────────────────────────────────────

        Private Sub OpenFolderDialog()
            Using dlg As New FolderBrowserDialog()
                dlg.Description = "Select a folder containing CSV libraries"
                If Not String.IsNullOrEmpty(_folder) Then dlg.SelectedPath = _folder
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    LoadFolder(dlg.SelectedPath)
                    AddRecentFolder(dlg.SelectedPath)
                End If
            End Using
        End Sub

        ' ── Recently used folders ────────────────────────────────────────────────────

        ''' <summary>Add (or promote) a folder to the top of the recent list, cap at 5,
        ''' persist, and rebuild the menu.</summary>
        Private Sub AddRecentFolder(folder As String)
            If String.IsNullOrEmpty(folder) Then Return
            If _settings.RecentFolders Is Nothing Then _settings.RecentFolders = New List(Of String)()
            _settings.RecentFolders.RemoveAll(
                Function(f) String.Equals(f, folder, StringComparison.OrdinalIgnoreCase))
            _settings.RecentFolders.Insert(0, folder)
            While _settings.RecentFolders.Count > 5
                _settings.RecentFolders.RemoveAt(_settings.RecentFolders.Count - 1)
            End While
            _settings.Save()
            RefreshRecentMenu()
        End Sub

        ''' <summary>Rebuild the "Recently Used" submenu from settings (disabled if empty).</summary>
        Private Sub RefreshRecentMenu()
            mnuRecent.DropDownItems.Clear()
            Dim list = _settings.RecentFolders
            If list Is Nothing OrElse list.Count = 0 Then
                mnuRecent.Enabled = False
                Return
            End If
            mnuRecent.Enabled = True
            For Each folder In list
                ' Show the leaf folder name; the full path lives in the tooltip.
                Dim leaf = New DirectoryInfo(folder.TrimEnd(Path.DirectorySeparatorChar,
                                                            Path.AltDirectorySeparatorChar)).Name
                If String.IsNullOrEmpty(leaf) Then leaf = folder      ' e.g. a drive root
                Dim item As New ToolStripMenuItem(leaf) With {
                    .Tag = folder,
                    .ToolTipText = folder
                }
                AddHandler item.Click, AddressOf RecentItem_Click
                mnuRecent.DropDownItems.Add(item)
            Next
        End Sub

        Private Sub RecentItem_Click(sender As Object, e As EventArgs)
            Dim item = TryCast(sender, ToolStripMenuItem)
            If item Is Nothing Then Return
            Dim folder = TryCast(item.Tag, String)
            If String.IsNullOrEmpty(folder) Then Return
            If Not Directory.Exists(folder) Then
                ConfirmDialog.Notify(Me, "Recently Used", "The folder no longer exists:" & vbCrLf & folder,
                                     icon:=DialogIcon.Warning)
                _settings.RecentFolders.RemoveAll(
                    Function(f) String.Equals(f, folder, StringComparison.OrdinalIgnoreCase))
                _settings.Save()
                RefreshRecentMenu()
                Return
            End If
            ' Switching from the list does not reorder it — only Ctrl+O alters the list.
            LoadFolder(folder)
        End Sub

        Private Sub ReloadFolder()
            If Not String.IsNullOrEmpty(_folder) Then LoadFolder(_folder)
        End Sub

        ''' <summary>Open the current working folder in Windows Explorer.</summary>
        Private Sub OpenWorkingFolder()
            If String.IsNullOrEmpty(_folder) OrElse Not Directory.Exists(_folder) Then Return
            Try
                Process.Start(New ProcessStartInfo With {.FileName = _folder, .UseShellExecute = True})
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Open Working Folder", "Could not open the folder:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Warning)
            End Try
        End Sub

        Private Sub LoadFolder(folder As String)
            ' Persist unsaved work from a previous folder first.
            For Each fn In _order.ToList()
                If _docs(fn).Dirty Then SaveFile(fn, silent:=True)
            Next

            _folder = folder
            folderStatus.Text = "Work Library : " & folder
            _docs.Clear()
            _order.Clear()
            _activeName = Nothing

            ' Load normalizer rules (or seed a default file the first time).
            Try
                Dim cfgPath = RuleStore.GetConfigPath(folder)
                If File.Exists(cfgPath) Then
                    _config = RuleStore.Load(folder)
                Else
                    _config = RuleStore.CreateDefaultConfig()
                    RuleStore.Save(folder, _config)
                End If
            Catch ex As Exception
                _config = New NormalizerConfig()
                ConfirmDialog.Notify(Me, "Rules", "Could not read normalizer.json:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Warning)
            End Try

            Dim patched As New List(Of String)()
            For Each csvPath In Directory.GetFiles(folder, "*.csv").OrderBy(Function(p) p, StringComparer.OrdinalIgnoreCase)
                Try
                    Dim table = CsvService.ReadCsv(csvPath)
                    Dim injected = GuidService.InjectGuids(table)
                    Dim doc As New CsvDocument(csvPath, table)
                    Dim nm = doc.FileName
                    _docs(nm) = doc
                    _order.Add(nm)
                    If injected Then
                        BackupService.MakeBackup(csvPath)
                        CsvService.WriteCsv(csvPath, table)
                        table.AcceptChanges()
                        patched.Add(nm)
                    End If
                Catch ex As Exception
                    Debug.WriteLine($"Could not load {csvPath}: {ex.Message}")
                End Try
            Next

            _order.Sort(StringComparer.OrdinalIgnoreCase)
            RefreshFileList()
            SetIntegrityIcon(IntegrityState.Neutral)   ' the set of files changed

            If _order.Count > 0 Then
                OpenFile(_order(0))
                If patched.Count > 0 Then
                    SetStatus($"GUIDs added & saved: {patched.Count} file(s)", Palette.Success)
                Else
                    SetStatus($"Loaded {_order.Count} file(s)", Palette.Success)
                End If
            Else
                ShowEmpty()
                SetStatus("No CSV files found", Palette.TextMuted)
            End If

            _settings.LastFolder = folder
        End Sub

        Private Sub OpenFile(name As String)
            If Not _docs.ContainsKey(name) Then Return
            Dim doc = _docs(name)

            ' Leave any backup preview: switching files always returns to editable/dark.
            If _previewing Then EnterEditableStyle()

            _loading = True
            _activeName = name
            bindingSrc.DataSource = doc.Table
            grid.DataSource = bindingSrc
            _loading = False

            ApplyGuidColumnStyle()
            AutoSizeColumns()
            ShowGrid()
            Me.Text = $"{_baseCaption} ({name})"
            UpdateRowCount()
            If bindingSrc IsNot Nothing Then bindingSrc.Filter = Nothing
            RefreshFileList()
            RefreshBackupList()
            UpdateStatusFromDirty(doc.Dirty)
        End Sub

        ' ── Backups (grid-toolbar dropdown + Restore) ──────────────────────────────

        ''' <summary>
        ''' Populate the toolbar dropdown: a "Current" entry (the live editable file),
        ''' then a divider, then the file's backups (newest first). Selecting Current
        ''' returns to the editable file; selecting a backup previews it read-only.
        ''' Reselects the previewed backup (if any) so a mid-preview refresh is seamless.
        ''' </summary>
        Private Sub RefreshBackupList()
            _suppressBackupEvent = True
            cboBackups.BeginUpdate()
            cboBackups.Items.Clear()

            Dim hasFile = _activeName IsNot Nothing AndAlso _docs.ContainsKey(_activeName)
            If hasFile Then
                cboBackups.Items.Add(CurrentLabel)
                Dim backups = BackupService.ListBackups(_docs(_activeName).Path)
                If backups.Count > 0 Then
                    cboBackups.Items.Add(DividerItem)
                    For Each b In backups
                        cboBackups.Items.Add(b)
                    Next
                End If
            End If
            cboBackups.EndUpdate()

            Dim idx As Integer = 0
            If _previewing AndAlso _previewPath IsNot Nothing Then
                Dim found = FindBackupIndex(_previewPath)
                If found >= 0 Then idx = found
            End If
            If cboBackups.Items.Count > 0 Then cboBackups.SelectedIndex = Math.Min(idx, cboBackups.Items.Count - 1)
            cboBackups.Enabled = hasFile
            UpdateBackupButtons()
            _suppressBackupEvent = False
        End Sub

        ''' <summary>Index of the dropdown item whose backup path matches, or -1.</summary>
        Private Function FindBackupIndex(path As String) As Integer
            For i As Integer = 0 To cboBackups.Items.Count - 1
                Dim bi = TryCast(cboBackups.Items(i), BackupInfo)
                If bi IsNot Nothing AndAlso String.Equals(bi.Path, path, StringComparison.OrdinalIgnoreCase) Then Return i
            Next
            Return -1
        End Function

        ''' <summary>Restore is only meaningful when a backup (not "Current") is showing.</summary>
        Private Sub UpdateBackupButtons()
            btnRestore.Enabled = TypeOf cboBackups.SelectedItem Is BackupInfo
        End Sub

        Private Sub SelectCurrentSuppressed()
            _suppressBackupEvent = True
            If cboBackups.Items.Count > 0 Then cboBackups.SelectedIndex = 0
            _suppressBackupEvent = False
            UpdateBackupButtons()
        End Sub

        Private Sub SelectBackupSuppressed(path As String)
            Dim idx = FindBackupIndex(path)
            If idx >= 0 Then
                _suppressBackupEvent = True
                cboBackups.SelectedIndex = idx
                _suppressBackupEvent = False
            End If
            UpdateBackupButtons()
        End Sub

        ''' <summary>Dropdown changed: preview a backup, or return to the editable file.</summary>
        Private Sub cboBackups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBackups.SelectedIndexChanged
            If _suppressBackupEvent Then Return
            Dim item = cboBackups.SelectedItem
            If item Is DividerItem Then
                ' The separator isn't a real choice — snap back to what we were showing.
                SelectBackupOrCurrentSuppressed()
                Return
            End If
            If TypeOf item Is BackupInfo Then
                PreviewBackup(DirectCast(item, BackupInfo))
            Else
                ShowCurrent()
            End If
            UpdateBackupButtons()
        End Sub

        Private Sub SelectBackupOrCurrentSuppressed()
            If _previewing AndAlso _previewPath IsNot Nothing Then
                SelectBackupSuppressed(_previewPath)
            Else
                SelectCurrentSuppressed()
            End If
        End Sub

        ''' <summary>
        ''' Load a backup into the grid as a read-only, light "inverted" preview. The
        ''' active document is left as-is. The very first hop from the editable file into
        ''' history, when the file is dirty, prompts to Save and Proceed (or Cancel).
        ''' </summary>
        Private Sub PreviewBackup(entry As BackupInfo)
            If entry Is Nothing OrElse _activeName Is Nothing Then Return
            Dim path = entry.Path
            Dim label = entry.ToString()
            If Not File.Exists(path) Then
                SetStatus("That backup no longer exists on disk", Palette.Warning)
                RefreshBackupList()
                Return
            End If

            ' First hop from the live file into history: persist unsaved edits first.
            If Not _previewing AndAlso _docs(_activeName).Dirty Then
                Dim proceed = ConfirmDialog.Ask(Me, "Unsaved changes",
                    $"""{_activeName}"" has unsaved edits." & vbCrLf & vbCrLf &
                    "Save the file before viewing this backup?",
                    okText:="Save and Proceed", cancelText:="Cancel")
                If Not proceed Then
                    SelectCurrentSuppressed()
                    Return
                End If
                SaveActive(silent:=True)   ' also refreshes the backup list (suppressed)
            End If

            Dim table As DataTable
            Try
                table = CsvService.ReadCsv(path)
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Backup preview", "Could not read the backup:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
                SelectCurrentSuppressed()
                Return
            End Try

            grid.EndEdit()
            StopAutosave()
            _previewing = True
            _previewPath = path
            _loading = True
            grid.DataSource = Nothing
            bindingSrc.DataSource = table
            grid.DataSource = bindingSrc
            _loading = False
            grid.ReadOnly = True
            StyleGridPreview()
            AutoSizeColumns()
            Me.Text = $"{_baseCaption} ({_activeName} — backup {label}) [read-only]"
            UpdateFileScopedMenus()
            ' If we saved above, the list refresh reset the dropdown to Current — repoint it.
            SelectBackupSuppressed(path)
            SetStatus($"Previewing backup {label} — read-only", Palette.Warning)
        End Sub

        ''' <summary>Return the grid to the live, editable, dark-themed active document.</summary>
        Private Sub ShowCurrent()
            If _activeName Is Nothing Then Return
            Dim wasPreview = _previewing
            EnterEditableStyle()
            If wasPreview Then
                grid.EndEdit()
                _loading = True
                grid.DataSource = Nothing
                bindingSrc.DataSource = _docs(_activeName).Table
                grid.DataSource = bindingSrc
                _loading = False
                ApplyGuidColumnStyle()
                AutoSizeColumns()
            End If
            Me.Text = $"{_baseCaption} ({_activeName})"
            UpdateFileScopedMenus()
            UpdateStatusFromDirty(_docs(_activeName).Dirty)
            UpdateBackupButtons()
        End Sub

        ''' <summary>Clear preview state and restore the normal dark, editable grid look.</summary>
        Private Sub EnterEditableStyle()
            _previewing = False
            _previewPath = Nothing
            grid.ReadOnly = False
            StyleGrid()
        End Sub

        ''' <summary>Inverted light read-only look so a backup is obviously not the live file:
        ''' black text on 95%/90% white alternating rows, light selection, grey header.</summary>
        Private Sub StyleGridPreview()
            grid.BackgroundColor = Color.FromArgb(242, 242, 242)
            grid.GridColor = Color.FromArgb(200, 200, 204)
            grid.EnableHeadersVisualStyles = False
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(210, 210, 214)
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 210, 214)
            grid.ColumnHeadersDefaultCellStyle.Font = New Font("Consolas", 9.0F, FontStyle.Bold)
            grid.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 4, 4, 4)
            grid.DefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242)      ' 95% white
            grid.DefaultCellStyle.ForeColor = Color.Black
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 205, 235)
            grid.DefaultCellStyle.SelectionForeColor = Color.Black
            grid.DefaultCellStyle.Font = New Font("Consolas", 9.5F)
            grid.DefaultCellStyle.Padding = New Padding(3, 0, 3, 0)
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230)   ' 90% white
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(180, 205, 235)
            grid.RowTemplate.Height = 24
        End Sub

        ''' <summary>Load the selected backup into the active document and rebind the
        ''' grid. The restore is marked as an unsaved change so the user can persist it
        ''' with Save, but it intentionally does NOT start the autosave countdown.</summary>
        Private Sub RestoreSelectedBackup()
            If _activeName Is Nothing Then Return
            Dim entry = TryCast(cboBackups.SelectedItem, BackupInfo)
            If entry Is Nothing Then Return
            If Not File.Exists(entry.Path) Then
                SetStatus("That backup no longer exists on disk", Palette.Warning)
                RefreshBackupList()
                Return
            End If

            Dim proceed = ConfirmDialog.Ask(Me, "Restore backup",
                $"Restore the backup from {entry.ToString()} into ""{_activeName}""?" & vbCrLf & vbCrLf &
                "This replaces the current grid contents. It will not be saved automatically — use Save to keep it.",
                okText:="Restore", cancelText:="Cancel")
            If Not proceed Then Return

            Dim table As DataTable = Nothing
            Try
                table = CsvService.ReadCsv(entry.Path)
                GuidService.InjectGuids(table)   ' safety: back-fill if an old backup lacks GUIDs
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Restore backup", "Could not read the backup:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
                Return
            End Try

            grid.EndEdit()
            ' Stop any pending autosave so the restored data is never auto-persisted,
            ' then leave preview mode and rebind (guarded by _loading, so CellValueChanged
            ' does not fire). The restored data becomes the live editable file.
            StopAutosave()
            EnterEditableStyle()
            RebindActiveTable(table)
            _docs(_activeName).Dirty = True
            SetIntegrityIcon(IntegrityState.Neutral)   ' restored data invalidates the last check
            Me.Text = $"{_baseCaption} ({_activeName})"
            RefreshFileList()
            RefreshBackupList()      ' back on "Current"
            UpdateRowCount()
            UpdateFileScopedMenus()
            UpdateStatusFromDirty(True)
            SetStatus($"Restored backup {entry.ToString()} — not yet saved", Palette.Success)
        End Sub

        ' ── File list sidebar ──────────────────────────────────────────────────────

        Private Sub RefreshFileList()
            fileList.BeginUpdate()
            fileList.Items.Clear()
            For Each fn In _order
                Dim marker = If(_docs(fn).Dirty, "  ●", "")
                Dim display = Path.GetFileNameWithoutExtension(fn).ToUpperInvariant()
                fileList.Items.Add(display & marker)
            Next
            If _activeName IsNot Nothing Then
                Dim idx = _order.IndexOf(_activeName)
                If idx >= 0 Then fileList.SelectedIndex = idx
            End If
            fileList.EndUpdate()
        End Sub

        Private Sub OnFileSelected(sender As Object, e As EventArgs) Handles fileList.SelectedIndexChanged
            Dim idx = fileList.SelectedIndex
            If idx < 0 OrElse idx >= _order.Count Then Return
            Dim name = _order(idx)
            If Not String.Equals(name, _activeName, StringComparison.OrdinalIgnoreCase) Then
                OpenFile(name)
            End If
        End Sub

        Private Sub RemoveActiveFile()
            If _activeName Is Nothing Then Return
            Dim name = _activeName
            If _docs(name).Dirty Then
                If Not ConfirmDialog.Ask(Me, "Unsaved changes",
                    $"""{name}"" has unsaved changes. Remove from the list anyway?" & vbCrLf &
                    "(This does not delete the file on disk.)",
                    okText:="Remove", cancelText:="Cancel", icon:=DialogIcon.Warning) Then Return
            End If
            _docs.Remove(name)
            _order.Remove(name)
            _activeName = Nothing
            If _order.Count > 0 Then
                OpenFile(_order(_order.Count - 1))
            Else
                grid.DataSource = Nothing
                ShowEmpty()
                RefreshFileList()
                SetStatus("No file open", Palette.TextMuted)
            End If
        End Sub

        ' ── Grid helpers ────────────────────────────────────────────────────────────

        Private Sub OnDataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles grid.DataBindingComplete
            ApplyGuidColumnStyle()
        End Sub

        ''' <summary>When the current cell moves, repaint just the old and new rows so
        ''' the light-cyan current-row highlight follows the cursor. Cheap: only two
        ''' rows are invalidated; the colour itself is applied in <see cref="grid_CellFormatting"/>.</summary>
        Private Sub OnCurrentCellChanged(sender As Object, e As EventArgs) Handles grid.CurrentCellChanged
            If _highlightRow >= 0 AndAlso _highlightRow < grid.RowCount Then grid.InvalidateRow(_highlightRow)
            _highlightRow = If(grid.CurrentCell IsNot Nothing, grid.CurrentCell.RowIndex, -1)
            If _highlightRow >= 0 AndAlso _highlightRow < grid.RowCount Then grid.InvalidateRow(_highlightRow)
        End Sub

        ''' <summary>Tint the current cell's row light-cyan (foreground untouched). The
        ''' selected cell still renders with the standard selection colours, since those
        ''' take over for selected cells.</summary>
        Private Sub grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles grid.CellFormatting
            ' Index/GUID column is read-only: keep it a uniform muted grey on every row —
            ' it must not alternate, take the current-row tint, or highlight on selection,
            ' so it always reads as "not editable" (in both the live and backup views).
            If e.ColumnIndex = 0 Then
                Dim guidBack = If(_previewing, Color.FromArgb(222, 222, 226), Color.FromArgb(42, 42, 46))
                Dim guidFore = If(_previewing, Color.FromArgb(90, 90, 95), Color.FromArgb(157, 157, 160))
                e.CellStyle.BackColor = guidBack
                e.CellStyle.ForeColor = guidFore
                e.CellStyle.SelectionBackColor = guidBack
                e.CellStyle.SelectionForeColor = guidFore
                Return
            End If

            If grid.CurrentCell IsNot Nothing AndAlso e.RowIndex = grid.CurrentCell.RowIndex Then
                ' Light tint while previewing a backup, the usual blue-grey otherwise.
                e.CellStyle.BackColor = If(_previewing, Color.FromArgb(205, 222, 240), Color.FromArgb(110, 145, 175))
            End If
        End Sub

        ''' <summary>Owner-draw the file list so the selected item uses the active-cell
        ''' blue (dark background / light text otherwise).</summary>
        Private Sub fileList_DrawItem(sender As Object, e As DrawItemEventArgs) Handles fileList.DrawItem
            If e.Index < 0 Then Return
            Dim text = fileList.Items(e.Index).ToString()
            Dim selected = (e.State And DrawItemState.Selected) = DrawItemState.Selected
            Dim back = If(selected, Color.FromArgb(61, 92, 135), fileList.BackColor)
            Dim fore = If(selected, Color.White, fileList.ForeColor)
            Using b As New SolidBrush(back)
                e.Graphics.FillRectangle(b, e.Bounds)
            End Using
            TextRenderer.DrawText(e.Graphics, text, fileList.Font, e.Bounds, fore,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
            If selected Then e.DrawFocusRectangle()
        End Sub

        ''' <summary>Owner-draw the backups dropdown so the highlighted list item uses
        ''' the active-cell blue (the closed display stays dark).</summary>
        Private Sub cboBackups_DrawItem(sender As Object, e As DrawItemEventArgs) Handles cboBackups.DrawItem
            If e.Index < 0 Then
                e.DrawBackground()
                Return
            End If
            Dim item = cboBackups.Items(e.Index)

            ' Separator row: fill dark, draw a thin rule, never highlight.
            If item Is DividerItem Then
                Using bg As New SolidBrush(cboBackups.BackColor)
                    e.Graphics.FillRectangle(bg, e.Bounds)
                End Using
                Dim y = e.Bounds.Top + e.Bounds.Height \ 2
                Using p As New Pen(Color.FromArgb(90, 90, 95))
                    e.Graphics.DrawLine(p, e.Bounds.Left + 4, y, e.Bounds.Right - 4, y)
                End Using
                Return
            End If

            Dim text As String
            If TypeOf item Is BackupInfo Then
                text = DirectCast(item, BackupInfo).ToString()
            Else
                text = Convert.ToString(item)   ' the "Current" label
            End If
            Dim editPortion = (e.State And DrawItemState.ComboBoxEdit) = DrawItemState.ComboBoxEdit
            Dim selected = (Not editPortion) AndAlso ((e.State And DrawItemState.Selected) = DrawItemState.Selected)
            Dim back = If(selected, Color.FromArgb(61, 92, 135), cboBackups.BackColor)
            Dim fore = If(selected, Color.White, cboBackups.ForeColor)
            Using b As New SolidBrush(back)
                e.Graphics.FillRectangle(b, e.Bounds)
            End Using
            TextRenderer.DrawText(e.Graphics, text, cboBackups.Font, e.Bounds, fore,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        End Sub

        ''' <summary>Mark the GUID (first) column read-only with a standard, subtle
        ''' greyed background so it reads as non-editable.</summary>
        Private Sub ApplyGuidColumnStyle()
            If grid.Columns.Count = 0 Then Return
            Dim guidCol = grid.Columns(0)
            guidCol.ReadOnly = True
            If _previewing Then
                ' Match the light read-only preview (a shade darker than the cells).
                guidCol.DefaultCellStyle.BackColor = Color.FromArgb(222, 222, 226)
                guidCol.DefaultCellStyle.ForeColor = Color.FromArgb(90, 90, 95)
            Else
                guidCol.DefaultCellStyle.BackColor = Color.FromArgb(42, 42, 46)
                guidCol.DefaultCellStyle.ForeColor = Color.FromArgb(157, 157, 160)
            End If
        End Sub

        Private Sub AutoSizeColumns()
            If grid.Columns.Count = 0 Then Return
            grid.SuspendLayout()
            For i As Integer = 0 To grid.Columns.Count - 1
                grid.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.DisplayedCells)
                Dim w = grid.Columns(i).Width
                If i = 0 Then
                    grid.Columns(i).Width = Math.Min(Math.Max(w, 240), 300)
                Else
                    grid.Columns(i).Width = Math.Max(90, Math.Min(w, 600))
                End If
            Next
            grid.ResumeLayout()
        End Sub

        ''' <summary>Remove any column sort so the grid shows rows in their underlying
        ''' CSV/DataTable order again, and clear the header sort glyphs.</summary>
        Private Sub Unsort()
            If ActiveTable Is Nothing Then Return
            grid.EndEdit()
            Try
                bindingSrc.RemoveSort()
            Catch
                ' Nothing to remove.
            End Try
            For Each c As DataGridViewColumn In grid.Columns
                c.HeaderCell.SortGlyphDirection = SortOrder.None
            Next
        End Sub

        Private Sub ShowGrid()
            emptyPanel.Visible = False
            grid.Visible = True
            grid.BringToFront()
            UpdateFileScopedMenus()
        End Sub

        Private Sub ShowEmpty()
            grid.Visible = False
            emptyPanel.Visible = True
            emptyPanel.BringToFront()
            Me.Text = _baseCaption
            rowCountStatus.Text = ""
            RefreshBackupList()
            UpdateFileScopedMenus()
        End Sub

        Private Sub UpdateRowCount()
            If _activeName Is Nothing Then Return
            Dim n = _docs(_activeName).Table.Rows.Count
            rowCountStatus.Text = $"{n} row{If(n = 1, "", "s")} "
        End Sub

        Private ReadOnly Property ActiveTable As DataTable
            Get
                If _activeName Is Nothing Then Return Nothing
                Return _docs(_activeName).Table
            End Get
        End Property

        ' ── Editing ──────────────────────────────────────────────────────────────

        Private Sub OnCellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellValueChanged
            If _loading OrElse _previewing Then Return
            MarkActiveDirty()
        End Sub

        Private Sub MarkActiveDirty()
            If _activeName Is Nothing Then Return
            ' The sidebar only changes when the file first becomes dirty — rebuilding it
            ' on every keystroke/insert is wasted work (and flicker) on large tables.
            Dim wasDirty = _docs(_activeName).Dirty
            _docs(_activeName).Dirty = True
            If Not wasDirty Then RefreshFileList()
            UpdateRowCount()
            UpdateStatusFromDirty(True)
            RestartAutosave()
            SetIntegrityIcon(IntegrityState.Neutral)   ' an edit invalidates the last check
        End Sub

        Private Sub AddRow(cloneLast As Boolean)
            If _previewing Then Return
            Dim table = ActiveTable
            If table Is Nothing Then Return
            grid.EndEdit()

            Dim row = table.NewRow()
            For i As Integer = 0 To table.Columns.Count - 1
                If i = 0 Then
                    row(i) = GuidService.NewGuid()
                ElseIf cloneLast AndAlso table.Rows.Count > 0 Then
                    row(i) = table.Rows(table.Rows.Count - 1)(i)
                Else
                    row(i) = ""
                End If
            Next
            AppendRowAndSelect(row, beginEdit:=True)
        End Sub

        ''' <summary>
        ''' Add an already-populated row to the active table, mark the file dirty, then
        ''' select and scroll to the row's first editable cell. Shared by <see cref="AddRow"/>
        ''' and <see cref="ImportRecordIntoGrid"/>. Selection/scroll is best-effort.
        ''' </summary>
        Private Function AppendRowAndSelect(row As DataRow, beginEdit As Boolean) As Integer
            Dim table = ActiveTable
            If table Is Nothing Then Return -1
            table.Rows.Add(row)
            MarkActiveDirty()

            Dim viewIndex As Integer = -1
            Try
                viewIndex = bindingSrc.Count - 1
                If viewIndex >= 0 Then
                    grid.ClearSelection()
                    Dim colIndex = If(grid.Columns.Count > 1, 1, 0)
                    grid.CurrentCell = grid.Rows(viewIndex).Cells(colIndex)
                    grid.FirstDisplayedScrollingRowIndex = Math.Max(0, viewIndex)
                    If beginEdit AndAlso colIndex > 0 Then grid.BeginEdit(True)
                End If
            Catch
                ' Selection is best-effort.
            End Try
            Return viewIndex
        End Function

        ''' <summary>Copy Down (Safe): copy the cell above only when the cell above has
        ''' data AND the current cell is empty. On success, move down only if the cell
        ''' below exists and is also empty. The GUID column and top row are protected.</summary>
        Private Sub CopyDownSafe()
            If _previewing Then Return
            If grid.CurrentCell Is Nothing Then Return
            Dim col = grid.CurrentCell.ColumnIndex
            Dim rowView = grid.CurrentCell.RowIndex
            If col = 0 Then Return                        ' never touch the GUID column
            If rowView <= 0 Then Return                   ' no cell above
            grid.EndEdit()

            Dim aboveVal = grid.Rows(rowView - 1).Cells(col).Value
            Dim aboveText = If(aboveVal Is Nothing, "", aboveVal.ToString())
            If aboveText.Length = 0 Then Return           ' cell above empty -> do nothing

            Dim curVal = grid.CurrentCell.Value
            Dim curText = If(curVal Is Nothing, "", curVal.ToString())
            If curText.Length > 0 Then Return             ' current cell has data -> do nothing

            ' In the Location column, Copy Down increments the numeric suffix
            ' (MARS177 -> MARS178). Every other column copies the value verbatim.
            Dim valueToSet = If(IsLocationColumn(col), IncrementLocation(aboveText), aboveText)
            grid.CurrentCell.Value = valueToSet           ' fires CellValueChanged -> dirty

            ' Move down only if there is a cell below and it is empty.
            If rowView + 1 < grid.RowCount Then
                Dim belowVal = grid.Rows(rowView + 1).Cells(col).Value
                Dim belowText = If(belowVal Is Nothing, "", belowVal.ToString())
                If belowText.Length = 0 Then
                    grid.ClearSelection()
                    grid.CurrentCell = grid.Rows(rowView + 1).Cells(col)
                    grid.CurrentCell.Selected = True
                End If
            End If
        End Sub

        ''' <summary>True if the given grid column is the "Location" column.</summary>
        Private Function IsLocationColumn(colIndex As Integer) As Boolean
            Return colIndex >= 0 AndAlso colIndex < grid.Columns.Count AndAlso
                   String.Equals(grid.Columns(colIndex).HeaderText, "Location", StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>Increment the trailing numeric portion of a location, preserving the
        ''' prefix and the digit width (MARS177 → MARS178, A0099 → A0100). Text with no
        ''' trailing digits is returned unchanged.</summary>
        Private Shared Function IncrementLocation(text As String) As String
            Dim m = Regex.Match(text, "^(.*?)(\d+)$")
            If Not m.Success Then Return text
            Dim n As Long
            If Not Long.TryParse(m.Groups(2).Value, n) Then Return text
            Return m.Groups(1).Value & (n + 1).ToString().PadLeft(m.Groups(2).Value.Length, "0"c)
        End Function

        Private Sub CopyFromAbove()
            If grid.CurrentCell Is Nothing Then Return
            Dim col = grid.CurrentCell.ColumnIndex
            Dim rowView = grid.CurrentCell.RowIndex
            If col = 0 Then Return                        ' never touch the GUID column
            If rowView <= 0 Then Return
            grid.EndEdit()
            Dim above = grid.Rows(rowView - 1).Cells(col).Value
            grid.CurrentCell.Value = above               ' fires CellValueChanged -> dirty
            ' Move the selection down one row, if there is one.
            If rowView + 1 < grid.RowCount Then
                grid.ClearSelection()
                grid.CurrentCell = grid.Rows(rowView + 1).Cells(col)
                grid.CurrentCell.Selected = True
            End If
        End Sub

        ''' <summary>Delete the row of the current cell (after confirmation), then keep
        ''' the selection on the same position (clamped) in the same column.</summary>
        Private Sub DeleteSelectedRow()
            If _previewing Then Return
            If ActiveTable Is Nothing Then Return
            If grid.CurrentCell Is Nothing Then Return
            grid.EndEdit()
            Dim rowIndex = grid.CurrentCell.RowIndex
            Dim colIndex = grid.CurrentCell.ColumnIndex
            If rowIndex < 0 OrElse rowIndex >= bindingSrc.Count Then Return

            If Not ConfirmDialog.Ask(Me, "Delete Row",
                "Delete the selected row? This action cannot be undone.",
                okText:="Delete", cancelText:="Cancel") Then Return

            bindingSrc.RemoveAt(rowIndex)      ' removes the row from the bound table
            ' RemoveAt only marks the underlying DataRow as Deleted; it stays in
            ' table.Rows until AcceptChanges, and touching a Deleted row throws
            ' ("Deleted row information cannot be accessed") on the next save /
            ' normalize / report. Commit it now so no Deleted-state row lingers.
            ActiveTable.AcceptChanges()
            MarkActiveDirty()

            ' Keep the selection near where the row was.
            Dim newRow = Math.Min(rowIndex, grid.RowCount - 1)
            RestoreCurrentCell(newRow, colIndex)
        End Sub

        ' ── Normalizer ───────────────────────────────────────────────────────────

        Private Sub RunNormalizer()
            If _previewing Then Return
            If _activeName Is Nothing Then Return
            Dim rs = NormalizerService.FindRuleSet(_config, _activeName)
            If rs Is Nothing Then
                SetStatus($"No normalizer rule matches {_activeName}", Palette.TextMuted)
                ConfirmDialog.Notify(Me, "Normalize", $"No normalizer rule set matches ""{_activeName}"".")
                Return
            End If

            grid.EndEdit()
            Dim result = NormalizerService.Apply(ActiveTable, rs)
            If result.HasMissing Then
                ConfirmDialog.Notify(Me, "Normalize",
                    "These columns referenced by the rule are missing:" & vbCrLf &
                    String.Join(", ", result.MissingColumns), icon:=DialogIcon.Warning)
                Return
            End If

            If result.ChangedCells > 0 Then
                MarkActiveDirty()
                grid.Refresh()
                SetStatus($"Normalized: {result.ChangedCells} cell(s) updated", Palette.Success)
            Else
                SetStatus("Already normalized — no changes", Palette.TextMuted)
            End If
        End Sub

        Private Sub OpenRulesEditor()
            If String.IsNullOrEmpty(_folder) Then
                ConfirmDialog.Notify(Me, "Rules", "Open a folder first.", icon:=DialogIcon.Warning)
                Return
            End If
            Dim columns As New List(Of String)()
            If ActiveTable IsNot Nothing Then
                For Each c As DataColumn In ActiveTable.Columns
                    columns.Add(c.ColumnName)
                Next
            End If
            Using dlg As New RulesEditorForm(_config, _activeName, columns)
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    _config = dlg.ResultConfig
                    Try
                        RuleStore.Save(_folder, _config)
                        UpdateFileScopedMenus()   ' rule set may now match/unmatch this file
                        SetStatus("Rules saved to normalizer.json", Palette.Success)
                    Catch ex As Exception
                        ConfirmDialog.Notify(Me, "Rules", "Could not save rules:" & vbCrLf & ex.Message,
                                             icon:=DialogIcon.Error)
                    End Try
                End If
            End Using
        End Sub

        ' ── Column manager ─────────────────────────────────────────────────────────

        Private Sub OpenColumnManager()
            If ActiveTable Is Nothing Then Return
            grid.EndEdit()
            ' Self-heal the mandatory schema before letting the user edit columns.
            NormalizeMandatorySchema()
            Using dlg As New ColumnsForm(ActiveTable)
                If dlg.ShowDialog(Me) = DialogResult.OK AndAlso dlg.Changed Then
                    RebindActiveTable(dlg.BuildResult())
                    MarkActiveDirty()
                    SetStatus("Columns updated", Palette.Success)
                End If
            End Using
        End Sub

        ''' <summary>Swap the active document's table and rebind the grid to it. The
        ''' current cell (row/column index) is preserved across the rebind — this stays
        ''' within the same file, unlike <see cref="OpenFile"/> which switches files.</summary>
        Private Sub RebindActiveTable(newTable As DataTable)
            Dim restoreRow As Integer = -1, restoreCol As Integer = -1
            If grid.CurrentCell IsNot Nothing Then
                restoreRow = grid.CurrentCell.RowIndex
                restoreCol = grid.CurrentCell.ColumnIndex
            End If
            _docs(_activeName).Table = newTable
            _loading = True
            grid.DataSource = Nothing
            bindingSrc.DataSource = newTable
            grid.DataSource = bindingSrc
            _loading = False
            ApplyGuidColumnStyle()
            AutoSizeColumns()
            RestoreCurrentCell(restoreRow, restoreCol)
        End Sub

        ''' <summary>
        ''' Ensure the immutable library fields (<see cref="AppInfo.NewLibraryHeaders"/>)
        ''' are present, at the front of the table, in the canonical order. Missing
        ''' fields are created; out-of-order fields are reordered (moving their data);
        ''' any extra columns are kept, in their current order, after the mandatory
        ''' block. If anything changed, the corrected table is rebound and saved.
        ''' </summary>
        Private Sub NormalizeMandatorySchema()
            Dim source = ActiveTable
            If source Is Nothing Then Return

            Dim mandatory = AppInfo.NewLibraryHeaders.Split(","c).
                Select(Function(h) h.Trim()).ToList()
            Dim mandatorySet As New HashSet(Of String)(mandatory, StringComparer.OrdinalIgnoreCase)
            Dim existing = source.Columns.Cast(Of DataColumn)().
                Select(Function(c) c.ColumnName).ToList()
            Dim extras = existing.Where(Function(n) Not mandatorySet.Contains(n)).ToList()

            Dim desired As New List(Of String)()
            desired.AddRange(mandatory)
            desired.AddRange(extras)

            ' Already correct? (same names, same order — case-insensitive)
            Dim ok = (existing.Count = desired.Count)
            If ok Then
                For i As Integer = 0 To desired.Count - 1
                    If Not String.Equals(existing(i), desired(i), StringComparison.OrdinalIgnoreCase) Then
                        ok = False
                        Exit For
                    End If
                Next
            End If
            If ok Then Return

            ' Rebuild in the desired order, moving cell data across by name.
            Dim result As New DataTable(source.TableName)
            For Each colName In desired
                result.Columns.Add(colName, GetType(String))
            Next
            For Each srcRow As DataRow In source.Rows
                Dim r = result.NewRow()
                For Each colName In desired
                    If source.Columns.Contains(colName) Then
                        Dim v = srcRow(colName)
                        r(colName) = If(v Is DBNull.Value, "", Convert.ToString(v))
                    Else
                        r(colName) = ""      ' newly created field
                    End If
                Next
                result.Rows.Add(r)
            Next

            ' A freshly created Index column starts blank — back-fill GUIDs.
            GuidService.InjectGuids(result)

            RebindActiveTable(result)
            UpdateRowCount()
            _docs(_activeName).Dirty = True
            SaveFile(_activeName, silent:=True)   ' persist the corrected schema
            SetStatus("Mandatory fields normalized & saved", Palette.Success)
        End Sub

        ' ── Saving ───────────────────────────────────────────────────────────────

        Private Sub SaveActive(silent As Boolean)
            If _activeName IsNot Nothing Then SaveFile(_activeName, silent)
        End Sub

        Private Sub SaveAllDirty()
            Dim dirty = _order.Where(Function(n) _docs(n).Dirty).ToList()
            For Each fn In dirty
                SaveFile(fn, silent:=True)
            Next
            If dirty.Count > 0 Then
                SetStatus($"Saved {dirty.Count} file{If(dirty.Count = 1, "", "s")}", Palette.Success)
            End If
        End Sub

        Private Sub SaveFile(name As String, silent As Boolean)
            If Not _docs.ContainsKey(name) Then Return
            Dim doc = _docs(name)

            ' Remember the active grid's current cell so it can be restored after the
            ' save (some save paths refresh the grid and would otherwise lose it).
            Dim restoreRow As Integer = -1, restoreCol As Integer = -1
            If name = _activeName Then
                grid.EndEdit()
                If grid.CurrentCell IsNot Nothing Then
                    restoreRow = grid.CurrentCell.RowIndex
                    restoreCol = grid.CurrentCell.ColumnIndex
                End If
            End If

            ' Optionally normalize the file's data before writing it.
            If _settings.AutoNormalizeOnSave Then
                Dim rs = NormalizerService.FindRuleSet(_config, name)
                If rs IsNot Nothing Then NormalizerService.Apply(doc.Table, rs)
            End If

            Try
                BackupService.MakeBackup(doc.Path)
                CsvService.WriteCsv(doc.Path, doc.Table)
                doc.Table.AcceptChanges()
                doc.Dirty = False
                StopAutosave()
                RefreshFileList()
                If name = _activeName Then
                    RefreshBackupList()   ' the save just created a new backup
                    UpdateStatusFromDirty(False)
                    RestoreCurrentCell(restoreRow, restoreCol)
                End If
                If Not silent Then SetStatus($"Saved {name}", Palette.Success)
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Save failed", $"Could not save {name}:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
            End Try
        End Sub

        ''' <summary>Re-select the given cell if the coordinates are still in range.</summary>
        Private Sub RestoreCurrentCell(rowIndex As Integer, colIndex As Integer)
            If rowIndex < 0 OrElse colIndex < 0 Then Return
            If rowIndex >= grid.RowCount OrElse colIndex >= grid.ColumnCount Then Return
            Try
                grid.ClearSelection()
                grid.CurrentCell = grid.Rows(rowIndex).Cells(colIndex)
                grid.CurrentCell.Selected = True
            Catch
                ' Best-effort restore.
            End Try
        End Sub

        ' ── Autosave ───────────────────────────────────────────────────────────────

        Private Sub RestartAutosave()
            ' Honour the Options toggle. Files still track dirty; they just won't
            ' auto-save. Unsaved work is caught by the exit prompt instead.
            If Not _settings.AutosaveEnabled Then Return
            _autosaveRemaining = Math.Max(5, _settings.AutosaveSeconds)
            If Not autosaveTimer.Enabled Then autosaveTimer.Start()
        End Sub

        Private Sub StopAutosave()
            autosaveTimer.Stop()
            _autosaveRemaining = 0
        End Sub

        Private Sub OnAutosaveTick(sender As Object, e As EventArgs) Handles autosaveTimer.Tick
            _autosaveRemaining -= 1
            If _autosaveRemaining > 0 Then
                If _activeName IsNot Nothing AndAlso _docs(_activeName).Dirty Then
                    SetStatus($"Auto-save in {_autosaveRemaining}s", Palette.Warning)
                End If
            Else
                StopAutosave()
                SaveAllDirty()
            End If
        End Sub

        ' ── Status ────────────────────────────────────────────────────────────────

        ''' <summary>
        ''' Update the right-hand status message. The status bar is intentionally
        ''' left with standard Windows styling, so the <paramref name="color"/>
        ''' argument (a dark-theme accent) is accepted for call-site convenience but
        ''' not applied.
        ''' </summary>
        Private Sub SetStatus(text As String, color As Color)
            saveStatus.Text = text
        End Sub

        Private Sub UpdateStatusFromDirty(dirty As Boolean)
            If dirty Then
                SetStatus("Unsaved changes", Palette.Warning)
            Else
                SetStatus("All saved", Palette.Success)
            End If
        End Sub

        ''' <summary>Escape clears the app clipboard and the find string (unless a
        ''' cell is being edited, where Escape cancels the edit as usual).</summary>
        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If keyData = Keys.Escape AndAlso Not grid.IsCurrentCellInEditMode Then
                AppInfo.OurClipboard = ""
                AppInfo.FindString = ""
                UpdateFindReplaceStatus()
                SetStatus("Cleared find string & clipboard", Palette.TextMuted)
                Return True
            End If

            ' Advanced Clipboard on the numeric keypad. The menu shortcuts already cover
            ' the top-row digits (Ctrl+0..9 copy, Alt+0..9 paste); mirror them for NumPad
            ' so laptops without a numpad and full keyboards both work.
            Dim keyCode As Keys = keyData And Keys.KeyCode
            Dim mods As Keys = keyData And Keys.Modifiers

            ' Windows-clipboard variants (Ctrl+Shift+C / Ctrl+Shift+V).
            If mods = (Keys.Control Or Keys.Shift) Then
                If keyCode = Keys.C Then
                    CopyCellWindowsOnly()
                    Return True
                ElseIf keyCode = Keys.V Then
                    PasteFromWindows()
                    Return True
                End If
            End If

            If keyCode >= Keys.NumPad0 AndAlso keyCode <= Keys.NumPad9 Then
                Dim n As Integer = CInt(keyCode) - CInt(Keys.NumPad0)
                If mods = Keys.Control Then
                    AdvancedClipboardCopy(n)
                    Return True
                ElseIf mods = Keys.Alt Then
                    AdvancedClipboardPaste(n)
                    Return True
                End If
            End If

            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function

        Private Sub ShowHelp()
            Using dlg As New HelpForm()
                dlg.ShowDialog(Me)
            End Using
        End Sub

        Private Sub ShowAbout()
            Using dlg As New AboutForm()
                dlg.ShowDialog(Me)
            End Using
        End Sub

        ''' <summary>Exit. The unsaved-changes prompt and archive-on-exit are handled
        ''' centrally in <see cref="HandleFormClosing"/>, which this triggers.</summary>
        Private Sub ExitApp()
            Me.Close()
        End Sub

        ''' <summary>Enable/disable menu items based on current state.</summary>
        Private Sub UpdateFileScopedMenus()
            Dim hasFile = _activeName IsNot Nothing
            ' While previewing a backup the grid is read-only — editing is disabled.
            Dim editable = hasFile AndAlso Not _previewing
            For Each it In New ToolStripItem() {mnuSave, mnuSaveAll, mnuAddRow, mnuAddClone,
                                                mnuCopyDownSafe, mnuCopyAbove, mnuColumns,
                                                mnuUnsort, mnuRemove, mnuDeleteRow}
                it.Enabled = editable
            Next
            mnuAutosize.Enabled = hasFile   ' resizing columns is fine even in a preview
            ' Normalize Description only applies when a rule set matches this editable file.
            Dim canNormalize = editable AndAlso
                NormalizerService.FindRuleSet(_config, _activeName) IsNot Nothing
            mnuNormalize.Enabled = canNormalize
            ' Mirror the enable state onto the grid-toolbar icon buttons.
            btnSave.Enabled = editable
            btnSaveAll.Enabled = editable
            btnAutosize.Enabled = hasFile
            btnNormalize.Enabled = canNormalize
            btnUnsort.Enabled = editable
            btnDeleteRow.Enabled = editable
            ' Analysis buttons read the open documents (harmless during a preview too).
            btnLocationAnalysis.Enabled = hasFile
            btnIntegrityCheck.Enabled = hasFile
            btnLocationReport.Enabled = hasFile
            ' These only need a working folder, not an open file.
            Dim hasFolder = Not String.IsNullOrEmpty(_folder)
            mnuNewLibrary.Enabled = hasFolder
            mnuOpenWorkFolder.Enabled = hasFolder
            mnuCreateArchive.Enabled = hasFolder
            btnCreateArchive.Enabled = hasFolder
        End Sub

        ' ── Grid-toolbar icon buttons ────────────────────────────────────────────

        Private Sub btnAutosize_Click(sender As Object, e As EventArgs) Handles btnAutosize.Click
            AutoSizeColumns()
        End Sub

        Private Sub btnDeleteRow_Click(sender As Object, e As EventArgs) Handles btnDeleteRow.Click
            DeleteSelectedRow()
        End Sub

        Private Sub btnNormalize_Click(sender As Object, e As EventArgs) Handles btnNormalize.Click
            RunNormalizer()
        End Sub

        Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
            SaveActive(silent:=False)
        End Sub

        Private Sub btnSaveAll_Click(sender As Object, e As EventArgs) Handles btnSaveAll.Click
            SaveAllDirty()
        End Sub

        Private Sub btnUnsort_Click(sender As Object, e As EventArgs) Handles btnUnsort.Click
            Unsort()
        End Sub

        Private Sub btnRestore_Click(sender As Object, e As EventArgs) Handles btnRestore.Click
            RestoreSelectedBackup()
        End Sub

        ' Toolbar shortcuts to the Tools/File menu features (same handlers).
        Private Sub btnLocationAnalysis_Click(sender As Object, e As EventArgs) Handles btnLocationAnalysis.Click
            AnalyzeLocations()
        End Sub

        Private Sub btnIntegrityCheck_Click(sender As Object, e As EventArgs) Handles btnIntegrityCheck.Click
            IntegrityCheck()
        End Sub

        Private Sub btnLocationReport_Click(sender As Object, e As EventArgs) Handles btnLocationReport.Click
            LocationReport()
        End Sub

        Private Sub btnCreateArchive_Click(sender As Object, e As EventArgs) Handles btnCreateArchive.Click
            CreateArchive()
        End Sub

        ''' <summary>Last Integrity Check outcome shown on the toolbar icon.</summary>
        Private Enum IntegrityState
            Neutral     ' not checked / stale after an edit
            Pass        ' green glyph — index clean
            Failed      ' white glyph on red — duplicates found
        End Enum

        ''' <summary>Colour the Integrity Check toolbar icon to reflect the last result.</summary>
        Private Sub SetIntegrityIcon(state As IntegrityState)
            Select Case state
                Case IntegrityState.Pass
                    btnIntegrityCheck.BackColor = DarkTheme.ButtonFace
                    btnIntegrityCheck.ForeColor = DarkTheme.IconGreen
                Case IntegrityState.Failed
                    btnIntegrityCheck.BackColor = DarkTheme.IconRed
                    btnIntegrityCheck.ForeColor = Color.White
                Case Else
                    btnIntegrityCheck.BackColor = DarkTheme.ButtonFace
                    btnIntegrityCheck.ForeColor = DarkTheme.IconBlue
            End Select
        End Sub

        ' ── Copy / Paste (app-local clipboard, not the Windows clipboard) ───────────

        Private Function TopLeftSelectedCell() As DataGridViewCell
            Dim best As DataGridViewCell = Nothing
            For Each c As DataGridViewCell In grid.SelectedCells
                If best Is Nothing OrElse c.RowIndex < best.RowIndex OrElse
                   (c.RowIndex = best.RowIndex AndAlso c.ColumnIndex < best.ColumnIndex) Then
                    best = c
                End If
            Next
            If best Is Nothing Then best = grid.CurrentCell
            Return best
        End Function

        ''' <summary>Ctrl+C: copy the cell to BOTH the internal clipboard and Windows.</summary>
        Private Sub CopyCell()
            Dim src = TopLeftSelectedCell()
            If src Is Nothing Then Return
            Dim text = If(src.Value Is Nothing, "", src.Value.ToString())
            AppInfo.OurClipboard = text
            SetWindowsClipboard(text)
            UpdateFindReplaceStatus()
            SetStatus("Copied cell", Palette.Success)
        End Sub

        ''' <summary>Ctrl+Shift+C: copy the cell to the Windows clipboard ONLY (internal untouched).</summary>
        Private Sub CopyCellWindowsOnly()
            Dim src = TopLeftSelectedCell()
            If src Is Nothing Then Return
            SetWindowsClipboard(If(src.Value Is Nothing, "", src.Value.ToString()))
            SetStatus("Copied cell to Windows clipboard", Palette.Success)
        End Sub

        ''' <summary>Best-effort write to the Windows clipboard (empty text clears it).</summary>
        Private Shared Sub SetWindowsClipboard(text As String)
            Try
                If String.IsNullOrEmpty(text) Then
                    Clipboard.Clear()
                Else
                    Clipboard.SetText(text)
                End If
            Catch
                ' The clipboard can be transiently locked by another app — non-fatal.
            End Try
        End Sub

        ''' <summary>Ctrl+V: paste the internal clipboard; if it is empty, fall back to Windows.</summary>
        Private Sub PasteCell()
            If _previewing Then Return
            If grid.CurrentCell Is Nothing Then Return
            grid.EndEdit()

            ' Prefer the app clipboard; if empty, fall back to the Windows clipboard text
            ' (without storing it in OurClipboard — they are separate).
            Dim value As String
            If Not String.IsNullOrEmpty(AppInfo.OurClipboard) Then
                value = AppInfo.OurClipboard
            Else
                value = WindowsClipboardText()
                If String.IsNullOrEmpty(value) Then Return
            End If
            PasteValueIntoColumn(value)
        End Sub

        ''' <summary>Ctrl+Shift+V: always paste from the Windows clipboard.</summary>
        Private Sub PasteFromWindows()
            If _previewing Then Return
            If grid.CurrentCell Is Nothing Then Return
            grid.EndEdit()
            Dim value = WindowsClipboardText()
            If String.IsNullOrEmpty(value) Then Return
            PasteValueIntoColumn(value)
        End Sub

        Private Shared Function WindowsClipboardText() As String
            Try
                Return Clipboard.GetText()
            Catch
                Return ""
            End Try
        End Function

        ''' <summary>Write <paramref name="value"/> into every selected cell in the current
        ''' column (or just the current cell); read-only cells (GUID) are skipped.</summary>
        Private Sub PasteValueIntoColumn(value As String)
            Dim targetCol = grid.CurrentCell.ColumnIndex
            Dim targets As New List(Of DataGridViewCell)()
            For Each c As DataGridViewCell In grid.SelectedCells
                If c.ColumnIndex = targetCol Then targets.Add(c)
            Next
            If targets.Count = 0 Then targets.Add(grid.CurrentCell)

            Dim n = 0
            For Each c In targets
                If c.ReadOnly Then Continue For
                c.Value = value                     ' fires CellValueChanged -> dirty
                n += 1
            Next
            If n > 0 Then SetStatus($"Pasted into {n} cell{If(n = 1, "", "s")}", Palette.Success)
        End Sub

        ' ── Status: Find / Replace-With fields ──────────────────────────────────────

        Private Sub UpdateFindReplaceStatus()
            findStatus.Text = "Find : " &
                If(String.IsNullOrEmpty(AppInfo.FindString), "-Empty-", AppInfo.FindString) & " "
            replaceStatus.Text = "Replace with (Clipboard) : " &
                If(String.IsNullOrEmpty(AppInfo.OurClipboard), "-Empty-", AppInfo.OurClipboard) & " "
        End Sub

        ''' <summary>Public hook: refresh the find/clipboard status labels after another
        ''' window changes AppInfo.OurClipboard (e.g. the Import Records viewer).</summary>
        Public Sub RefreshClipboardStatus()
            UpdateFindReplaceStatus()
        End Sub

        Private Sub SetFindString(value As String)
            AppInfo.FindString = value
            UpdateFindReplaceStatus()
        End Sub

        ' ── Find commands ───────────────────────────────────────────────────────────
        ' FindString holds the search term. Find / Find Cell only SET it; the actual
        ' navigation is done by Find Next / Find Next in Column, which may match any
        ' column (including the index column).

        ''' <summary>Find (Ctrl+F): prompt for a search string and store it.</summary>
        Private Sub FindPrompt()
            Dim seed = VbInteraction.InputBox("Find text:", "Find", AppInfo.FindString)
            If String.IsNullOrEmpty(seed) Then Return
            SetFindString(seed)
        End Sub

        ''' <summary>Find Cell (Ctrl+Shift+F): store the current cell's text (top-left
        ''' of the selection) as the search string.</summary>
        Private Sub FindCellIntoFindString()
            Dim src = TopLeftSelectedCell()
            If src Is Nothing Then Return
            SetFindString(If(src.Value Is Nothing, "", src.Value.ToString()))
        End Sub

        ''' <summary>Find Next (F3): select the next cell containing FindString.</summary>
        Private Sub FindNextInTable()
            DoFindNext(-1)
        End Sub

        ''' <summary>Find Next in Column (Shift+F3): next match within this column.</summary>
        Private Sub FindNextInColumn()
            If grid.CurrentCell Is Nothing Then Return
            DoFindNext(grid.CurrentCell.ColumnIndex)
        End Sub

        Private Sub DoFindNext(scopeColumn As Integer)
            If String.IsNullOrEmpty(AppInfo.FindString) Then
                SetStatus("No search string — use Find first", Palette.Warning)
                Return
            End If
            Dim cell = NextMatchCell(AppInfo.FindString, scopeColumn, skipIndex:=False)
            If cell Is Nothing Then
                SetStatus($"'{AppInfo.FindString}' not found", Palette.Warning)
            Else
                SelectCell(cell)
                SetStatus($"Found '{AppInfo.FindString}'", Palette.Success)
            End If
        End Sub

        ''' <summary>
        ''' Next cell after the current one (wrapping) whose text contains
        ''' <paramref name="needle"/> (case-insensitive). scopeColumn &gt;= 0 limits the
        ''' search to that column; skipIndex excludes the index column (column 0).
        ''' </summary>
        Private Function NextMatchCell(needle As String, scopeColumn As Integer, skipIndex As Boolean) As DataGridViewCell
            If String.IsNullOrEmpty(needle) Then Return Nothing
            Dim rows = grid.RowCount, cols = grid.ColumnCount
            If rows = 0 OrElse cols = 0 Then Return Nothing

            Dim positions As New List(Of Point)()      ' X = column, Y = row
            If scopeColumn >= 0 Then
                If skipIndex AndAlso scopeColumn = 0 Then Return Nothing
                For r = 0 To rows - 1
                    positions.Add(New Point(scopeColumn, r))
                Next
            Else
                For r = 0 To rows - 1
                    For c = 0 To cols - 1
                        If skipIndex AndAlso c = 0 Then Continue For
                        positions.Add(New Point(c, r))
                    Next
                Next
            End If
            If positions.Count = 0 Then Return Nothing

            Dim curRow = If(grid.CurrentCell IsNot Nothing, grid.CurrentCell.RowIndex, 0)
            Dim curCol = If(grid.CurrentCell IsNot Nothing, grid.CurrentCell.ColumnIndex, 0)
            Dim startIdx = positions.FindIndex(Function(p) p.Y = curRow AndAlso p.X = curCol)
            If startIdx < 0 Then startIdx = 0
            Dim total = positions.Count

            For k = 1 To total
                Dim p = positions((startIdx + k) Mod total)
                Dim cell = grid.Rows(p.Y).Cells(p.X)
                Dim txt = If(cell.Value Is Nothing, "", cell.Value.ToString())
                If txt.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 Then Return cell
            Next
            Return Nothing
        End Function

        Private Sub SelectCell(cell As DataGridViewCell)
            grid.ClearSelection()
            Try
                grid.CurrentCell = cell
            Catch
            End Try
            cell.Selected = True
        End Sub

        ' ── Replace ──────────────────────────────────────────────────────────────────
        ' Replace operations search for FindString and substitute OurClipboard.
        ' The index column (column 0) is NEVER modified (find is still allowed there).

        ''' <summary>Case-insensitive substring replace of all occurrences.</summary>
        Private Function ReplaceCI(text As String, find As String, repl As String) As String
            If String.IsNullOrEmpty(find) Then Return text
            Dim sb As New System.Text.StringBuilder()
            Dim i = 0
            While i < text.Length
                If i + find.Length <= text.Length AndAlso
                   String.Compare(text, i, find, 0, find.Length, StringComparison.OrdinalIgnoreCase) = 0 Then
                    sb.Append(repl)
                    i += find.Length
                Else
                    sb.Append(text(i))
                    i += 1
                End If
            End While
            Return sb.ToString()
        End Function

        ''' <summary>Replace (Ctrl+R): copy the current cell's text into FindString,
        ''' then overwrite the current cell with OurClipboard — always, regardless of
        ''' any match. Focus does not move; the index column is never written.</summary>
        Private Sub ReplaceCurrentCell()
            If _previewing Then Return
            Dim cell = grid.CurrentCell
            If cell Is Nothing Then Return
            ' 1. Seed FindString from the current cell (a read — allowed anywhere).
            SetFindString(If(cell.Value Is Nothing, "", cell.Value.ToString()))
            ' 2. Overwrite the current cell with the clipboard — but only if the
            '    clipboard has content, and never the index column.
            If String.IsNullOrEmpty(AppInfo.OurClipboard) Then
                SetStatus("Clipboard is empty — nothing to replace with", Palette.Warning)
                Return
            End If
            If cell.ColumnIndex = 0 OrElse cell.ReadOnly Then Return
            cell.Value = AppInfo.OurClipboard      ' does not move focus
            MarkActiveDirty()                      ' (re)start the autosave countdown
            SetStatus("Replaced cell", Palette.Success)
        End Sub

        ''' <summary>Replace Next (Ctrl+Shift+R): find the next cell containing
        ''' FindString and substitute OurClipboard. Index column skipped.</summary>
        Private Sub ReplaceNext()
            ReplaceNextInScope(-1)
        End Sub

        ''' <summary>Replace in Same Column (Ctrl+Alt+R): as Replace Next, restricted
        ''' to the current column.</summary>
        Private Sub ReplaceInSameColumn()
            If grid.CurrentCell Is Nothing Then Return
            ReplaceNextInScope(grid.CurrentCell.ColumnIndex)
        End Sub

        Private Sub ReplaceNextInScope(scopeColumn As Integer)
            If _previewing Then Return
            If String.IsNullOrEmpty(AppInfo.OurClipboard) Then
                SetStatus("Clipboard is empty — nothing to replace with", Palette.Warning)
                Return
            End If
            If String.IsNullOrEmpty(AppInfo.FindString) Then
                SetStatus("No search string — use Find first", Palette.Warning)
                Return
            End If
            Dim cell = NextMatchCell(AppInfo.FindString, scopeColumn, skipIndex:=True)
            If cell Is Nothing Then
                SetStatus($"'{AppInfo.FindString}' not found", Palette.Warning)
                Return
            End If
            SelectCell(cell)
            Dim txt = If(cell.Value Is Nothing, "", cell.Value.ToString())
            cell.Value = ReplaceCI(txt, AppInfo.FindString, AppInfo.OurClipboard)
            MarkActiveDirty()                      ' (re)start the autosave countdown
            SetStatus("Replaced occurrence", Palette.Success)
        End Sub

        ' ── Edit-menu handlers ──────────────────────────────────────────────────────

        Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
            CopyCell()
        End Sub

        Private Sub PasteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteToolStripMenuItem.Click
            PasteCell()
        End Sub

        Private Sub mnuDeleteRow_Click(sender As Object, e As EventArgs) Handles mnuDeleteRow.Click
            DeleteSelectedRow()
        End Sub

        Private Sub FindToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindToolStripMenuItem.Click
            FindPrompt()
            FindNextInTable()
        End Sub

        Private Sub FindNextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindNextToolStripMenuItem.Click
            FindNextInTable()
        End Sub

        Private Sub FindCellToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindCellToolStripMenuItem.Click
            FindCellIntoFindString()
            FindNextInTable()
        End Sub

        Private Sub FindInColumnToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindInColumnToolStripMenuItem.Click
            FindNextInColumn()
        End Sub

        Private Sub ReplaceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceToolStripMenuItem.Click
            ReplaceCurrentCell()
        End Sub

        Private Sub ReplaceNextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceNextToolStripMenuItem.Click
            ReplaceNext()
        End Sub

        Private Sub ReplaceInSameColumnToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceInSameColumnToolStripMenuItem.Click
            ReplaceInSameColumn()
        End Sub

        ''' <summary>Create a new empty library CSV in the current working folder.</summary>
        Private Sub NewLibrary()
            If String.IsNullOrEmpty(_folder) Then Return

            Dim name = VbInteraction.InputBox("New library name (without extension):",
                                              "New Library", "").Trim()
            If name.Length = 0 Then Return

            ' Accept a typed ".csv" gracefully.
            If name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) Then
                name = name.Substring(0, name.Length - 4).Trim()
            End If
            If name.Length = 0 Then Return

            If name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 Then
                ConfirmDialog.Notify(Me, "New Library",
                    "That name contains characters that aren't allowed in a file name.", icon:=DialogIcon.Warning)
                Return
            End If

            Dim fileName = name & ".csv"
            Dim fullPath = Path.Combine(_folder, fileName)
            If File.Exists(fullPath) Then
                ConfirmDialog.Notify(Me, "New Library", $"""{fileName}"" already exists in this folder.",
                                     icon:=DialogIcon.Warning)
                Return
            End If

            ' Build a header-only table from the global schema, then write it.
            Dim table As New DataTable(name)
            For Each h In AppInfo.NewLibraryHeaders.Split(","c)
                table.Columns.Add(h.Trim(), GetType(String))
            Next
            Try
                CsvService.WriteCsv(fullPath, table)
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "New Library", "Could not create the file:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
                Return
            End Try

            ' Register, keep the list sorted, and open it.
            _docs(fileName) = New CsvDocument(fullPath, table)
            If Not _order.Contains(fileName) Then _order.Add(fileName)
            _order.Sort(StringComparer.OrdinalIgnoreCase)
            RefreshFileList()
            OpenFile(fileName)
            SetStatus($"Created {fileName}", Palette.Success)
        End Sub

        ''' <summary>
        ''' Zip every CSV in the current working folder into
        ''' "&lt;folder&gt;-M-d-yyyy-H-mm.zip" placed in that folder. Dirty files are saved
        ''' first so the archive reflects the current data.
        ''' </summary>
        Private Sub CreateArchive()
            If String.IsNullOrEmpty(_folder) OrElse Not Directory.Exists(_folder) Then Return

            ' Persist unsaved edits so the archive is current.
            SaveAllDirty()

            If Directory.GetFiles(_folder, "*.csv").Length = 0 Then
                ConfirmDialog.Notify(Me, "Create Archive", "There are no CSV files in the working folder to archive.",
                                     icon:=DialogIcon.Warning)
                Return
            End If

            Try
                Dim zipName = BuildArchive()
                If zipName IsNot Nothing Then
                    SetStatus($"Archived to {zipName}", Palette.Success)
                End If
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Create Archive", "Could not create the archive:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' Zip every CSV in the working folder into "&lt;folder&gt;-M-d-yyyy-H-mm.zip"
        ''' placed in that folder, and return the zip's file name. Returns Nothing if
        ''' there is no folder or no CSV files. Throws on an I/O error (callers decide
        ''' whether to surface it).
        ''' </summary>
        Private Function BuildArchive() As String
            If String.IsNullOrEmpty(_folder) OrElse Not Directory.Exists(_folder) Then Return Nothing
            Dim csvs = Directory.GetFiles(_folder, "*.csv")
            If csvs.Length = 0 Then Return Nothing

            Dim folderName = New DirectoryInfo(
                _folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).Name
            Dim stamp = DateTime.Now.ToString("M-d-yyyy-H-mm")
            Dim zipName = $"{folderName}-{stamp}.zip"
            Dim zipPath = Path.Combine(_folder, zipName)

            If File.Exists(zipPath) Then File.Delete(zipPath)
            Using archive = ZipFile.Open(zipPath, ZipArchiveMode.Create)
                For Each f In csvs
                    archive.CreateEntryFromFile(f, Path.GetFileName(f), CompressionLevel.Optimal)
                Next
            End Using
            Return zipName
        End Function

        ' ── Export a single record ──────────────────────────────────────────────────

        Private Shared ReadOnly ExportJsonOpts As New JsonSerializerOptions With {
            .WriteIndented = True,
            .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }

        ''' <summary>
        ''' Export the current grid row as a JSON file of "header": "value" pairs (the
        ''' GUID/Index column included), written to the working folder with a ".EXPORT"
        ''' extension. The file name comes from the "Manufacturer Part Number 1" cell,
        ''' upper-cased and sanitized so only [A-Za-z0-9] survive (everything else
        ''' becomes a hyphen). If that field is blank, the user is prompted; the entered
        ''' text is upper-cased and sanitized the same way.
        ''' </summary>
        Private Sub ExportRecord()
            If ActiveTable Is Nothing OrElse String.IsNullOrEmpty(_folder) Then Return
            grid.EndEdit()

            Dim rowIndex = If(grid.CurrentCell IsNot Nothing, grid.CurrentCell.RowIndex, -1)
            If rowIndex < 0 OrElse rowIndex >= grid.RowCount Then
                SetStatus("No row selected to export", Palette.Warning)
                Return
            End If

            ' Header -> value for every column of this row (grid order; GUID included).
            Dim record As New Dictionary(Of String, String)()
            For i As Integer = 0 To grid.Columns.Count - 1
                Dim cellVal = grid.Rows(rowIndex).Cells(i).Value
                record(grid.Columns(i).HeaderText) = If(cellVal Is Nothing, "", cellVal.ToString())
            Next

            ' Base file name: the part number, or a prompt (upper-cased) if it is blank.
            Dim mpn As String = ""
            Dim mpnCol = FindColumnIndex(PartNumberColumn)
            If mpnCol >= 0 Then
                Dim v = grid.Rows(rowIndex).Cells(mpnCol).Value
                mpn = If(v Is Nothing, "", v.ToString())
            End If

            Dim baseName As String
            If String.IsNullOrWhiteSpace(mpn) Then
                Dim entered = VbInteraction.InputBox(
                    "Manufacturer Part Number 1 is empty. Enter a name for the export file:",
                    "Export Record", "").Trim()
                If entered.Length = 0 Then
                    SetStatus("Export cancelled", Palette.TextMuted)
                    Return
                End If
                baseName = SanitizeFileName(entered.ToUpperInvariant())
            Else
                baseName = SanitizeFileName(mpn.ToUpperInvariant())
            End If
            If baseName.Length = 0 Then
                SetStatus("No usable characters for a file name", Palette.Warning)
                Return
            End If

            Dim exportPath = Path.Combine(_folder, baseName & ".EXPORT")
            Try
                File.WriteAllText(exportPath, JsonSerializer.Serialize(record, ExportJsonOpts),
                                  New UTF8Encoding(False))
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Export Record", "Could not write the export file:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Error)
                Return
            End Try
            SetStatus($"Exported {baseName}.EXPORT", Palette.Success)

            ' If the Import Records window is open, show the new file immediately.
            If _importForm IsNot Nothing AndAlso Not _importForm.IsDisposed Then _importForm.RefreshFiles()
        End Sub

        ''' <summary>Index of the grid column whose bound name matches, or -1.</summary>
        Private Function FindColumnIndex(dataName As String) As Integer
            For i As Integer = 0 To grid.Columns.Count - 1
                If String.Equals(ColumnDataName(grid.Columns(i)), dataName, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next
            Return -1
        End Function

        ''' <summary>Keep only ASCII letters/digits; replace every other character with
        ''' a hyphen (so the result is safe as a file name).</summary>
        Private Shared Function SanitizeFileName(value As String) As String
            If value Is Nothing Then Return ""
            Dim sb As New StringBuilder(value.Length)
            For Each ch As Char In value
                If (ch >= "0"c AndAlso ch <= "9"c) OrElse
                   (ch >= "A"c AndAlso ch <= "Z"c) OrElse
                   (ch >= "a"c AndAlso ch <= "z"c) Then
                    sb.Append(ch)
                Else
                    sb.Append("-"c)
                End If
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Append a new row to the active grid, copying values from
        ''' <paramref name="record"/> for every column whose name matches
        ''' (case-insensitive) — the GUID/Index column included, and NO new GUID is
        ''' generated. Returns the list of matched column names, or Nothing if there is
        ''' no active table. Used by the Import Records window.
        ''' </summary>
        Public Function ImportRecordIntoGrid(record As IDictionary(Of String, String)) As List(Of String)
            Dim table = ActiveTable
            If table Is Nothing Then Return Nothing
            grid.EndEdit()

            Dim matched As New List(Of String)()
            Dim row = table.NewRow()
            For i As Integer = 0 To table.Columns.Count - 1
                row(i) = ""
            Next
            For Each kv In record
                If table.Columns.Contains(kv.Key) Then
                    row(table.Columns(kv.Key)) = If(kv.Value, "")
                    matched.Add(kv.Key)
                End If
            Next
            AppendRowAndSelect(row, beginEdit:=False)

            Return matched
        End Function

        ' ── Menu event handlers ──────────────────────────────────────────────────

        Private Sub mnuOpen_Click(sender As Object, e As EventArgs) Handles mnuOpen.Click
            OpenFolderDialog()
        End Sub

        Private Sub mnuReload_Click(sender As Object, e As EventArgs) Handles mnuReload.Click
            ReloadFolder()
        End Sub

        Private Sub mnuOpenWorkFolder_Click(sender As Object, e As EventArgs) Handles mnuOpenWorkFolder.Click
            OpenWorkingFolder()
        End Sub

        Private Sub mnuNewLibrary_Click(sender As Object, e As EventArgs) Handles mnuNewLibrary.Click
            NewLibrary()
        End Sub

        Private Sub mnuCreateArchive_Click(sender As Object, e As EventArgs) Handles mnuCreateArchive.Click
            CreateArchive()
        End Sub

        Private Sub mnuSave_Click(sender As Object, e As EventArgs) Handles mnuSave.Click
            SaveActive(silent:=False)
        End Sub

        Private Sub mnuSaveAll_Click(sender As Object, e As EventArgs) Handles mnuSaveAll.Click
            SaveAllDirty()
        End Sub

        Private Sub mnuExit_Click(sender As Object, e As EventArgs) Handles mnuExit.Click
            ExitApp()
        End Sub

        Private Sub mnuAddRow_Click(sender As Object, e As EventArgs) Handles mnuAddRow.Click
            AddRow(cloneLast:=False)
        End Sub

        Private Sub mnuAddClone_Click(sender As Object, e As EventArgs) Handles mnuAddClone.Click
            AddRow(cloneLast:=True)
        End Sub

        Private Sub mnuCopyAbove_Click(sender As Object, e As EventArgs) Handles mnuCopyAbove.Click
            CopyFromAbove()
        End Sub

        Private Sub mnuCopyDownSafe_Click(sender As Object, e As EventArgs) Handles mnuCopyDownSafe.Click
            CopyDownSafe()
        End Sub

        Private Sub mnuColumns_Click(sender As Object, e As EventArgs) Handles mnuColumns.Click
            OpenColumnManager()
        End Sub

        Private Sub mnuAutosize_Click(sender As Object, e As EventArgs) Handles mnuAutosize.Click
            AutoSizeColumns()
        End Sub

        Private Sub mnuUnsort_Click(sender As Object, e As EventArgs) Handles mnuUnsort.Click
            Unsort()
        End Sub

        Private Sub mnuNormalize_Click(sender As Object, e As EventArgs) Handles mnuNormalize.Click
            RunNormalizer()
        End Sub

        Private Sub mnuRemove_Click(sender As Object, e As EventArgs) Handles mnuRemove.Click
            RemoveActiveFile()
        End Sub

        Private Sub mnuRules_Click(sender As Object, e As EventArgs) Handles mnuRules.Click
            OpenRulesEditor()
        End Sub

        Private Sub mnuWebCrawler_Click(sender As Object, e As EventArgs) Handles mnuWebCrawler.Click
            OpenWebCrawler()
        End Sub

        ''' <summary>
        ''' Tools ▸ Convert ID. If the active table has an "ID" column and no "Index"
        ''' column, fill the ID column with fresh GUIDs (one per row, blank rows
        ''' included), rename it to "Index", and move it to the front. Marks the file
        ''' dirty for a manual save but does NOT start the autosave countdown.
        ''' </summary>
        Private Sub ConvertIdToIndex()
            Dim table = ActiveTable
            If table Is Nothing Then Return
            grid.EndEdit()

            If Not table.Columns.Contains("ID") Then
                ConfirmDialog.Notify(Me, "Convert ID", "This table has no ""ID"" column to convert.")
                Return
            End If
            If table.Columns.Contains("Index") Then
                ConfirmDialog.Notify(Me, "Convert ID", "This table already has an ""Index"" column.")
                Return
            End If

            Dim idCol = table.Columns("ID")
            ' Fresh GUID for every row, including blank ones.
            For Each row As DataRow In table.Rows
                row(idCol) = GuidService.NewGuid()
            Next
            ' Rename to Index and make it the first column.
            idCol.ColumnName = "Index"
            idCol.SetOrdinal(0)

            ' Refresh the grid to reflect the new name/order. Direct DataTable edits do
            ' not raise CellValueChanged, so nothing has started autosave; stop it just
            ' in case a prior edit left it counting, and mark dirty for a manual save.
            StopAutosave()
            RebindActiveTable(table)
            _docs(_activeName).Dirty = True
            RefreshFileList()
            UpdateRowCount()
            UpdateStatusFromDirty(True)
            SetStatus("Converted ID → Index — not yet saved", Palette.Success)
        End Sub

        ''' <summary>Open the WebCrawler search-engine configuration window.</summary>
        Private Sub OpenWebCrawler()
            Using dlg As New SearchEnginesForm()
                dlg.ShowDialog(Me)
            End Using
        End Sub

        Private Sub mnuApiIntegration_Click(sender As Object, e As EventArgs) Handles mnuApiIntegration.Click
            OpenApiIntegration()
        End Sub

        ''' <summary>Open the Supplier API Integration window (edits the shared settings).</summary>
        Private Sub OpenApiIntegration()
            Using dlg As New SupplierApiForm(_settings)
                dlg.ShowDialog(Me)
            End Using
        End Sub

        Private Sub mnuOptions_Click(sender As Object, e As EventArgs) Handles mnuOptions.Click
            OpenOptions()
        End Sub

        Private Sub mnuAdvancedClipboard_Click(sender As Object, e As EventArgs) Handles mnuAdvancedClipboard.Click
            OpenAdvancedClipboard()
        End Sub

        ''' <summary>Open the Advanced Clipboard editor (modal). It reads/writes the
        ''' global array and persists to JSON on Close.</summary>
        Private Sub OpenAdvancedClipboard()
            Using dlg As New AdvancedClipboardForm(_settings)
                dlg.ShowDialog(Me)
            End Using
            ' The displayed row's data may have changed — refresh the status field.
            UpdateAdvancedClipboardStatus()
        End Sub

        ' ── Advanced Clipboard index (status-bar row selector) ───────────────────────

        Private Sub IncrementToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IncrementToolStripMenuItem.Click
            AppInfo.AdvancedClipboardIndex =
                (AppInfo.AdvancedClipboardIndex + 1) Mod AppInfo.AdvancedClipboardRows   ' 9 -> 0
            UpdateAdvancedClipboardStatus()
        End Sub

        Private Sub DecrementToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DecrementToolStripMenuItem.Click
            AppInfo.AdvancedClipboardIndex =
                (AppInfo.AdvancedClipboardIndex + AppInfo.AdvancedClipboardRows - 1) Mod AppInfo.AdvancedClipboardRows   ' 0 -> 9
            UpdateAdvancedClipboardStatus()
        End Sub

        ''' <summary>Show the selected AdvancedClipboard row in the status bar as
        ''' "(idx) 0: [..] | 1: [..] | … | 9: [..]".</summary>
        Private Sub UpdateAdvancedClipboardStatus()
            Dim idx = AppInfo.AdvancedClipboardIndex
            Dim xstring As String = "(" & idx & ") "
            For x As Integer = 0 To AppInfo.AdvancedClipboardCols - 1
                xstring &= "[ " & x & " : " & If(AppInfo.AdvancedClipboard(idx, x), "") & " ] "
                If x = 4 Then xstring &= " -  "
                'If x < AppInfo.AdvancedClipboardCols - 1 Then xstring &= " | "
            Next
            AdvancedClipboardContent.Text = xstring
        End Sub

        ' ── Advanced Clipboard copy/paste (Ctrl+0..9 copy, Alt+0..9 paste) ───────────

        ''' <summary>Assign the digit shortcuts (Ctrl+n copy, Alt+n paste) and click
        ''' handlers to the Copy 0..9 / Paste 0..9 menu items. Done in code (not the
        ''' designer) so it survives designer regeneration; each item's Tag carries its
        ''' column number.</summary>
        Private Sub WireAdvancedClipboardCopyPaste()
            Dim copyItems = {Copy0ToolStripMenuItem, Copy1ToolStripMenuItem, Copy2ToolStripMenuItem,
                             Copy3ToolStripMenuItem, Copy4ToolStripMenuItem, Copy5ToolStripMenuItem,
                             Copy6ToolStripMenuItem, Copy7ToolStripMenuItem, Copy8ToolStripMenuItem,
                             Copy9ToolStripMenuItem}
            Dim pasteItems = {Paste0ToolStripMenuItem, Paste1ToolStripMenuItem, Paste2ToolStripMenuItem,
                              Paste3ToolStripMenuItem, Paste4ToolStripMenuItem, Paste5ToolStripMenuItem,
                              Paste6ToolStripMenuItem, Paste7ToolStripMenuItem, Paste8ToolStripMenuItem,
                              Paste9ToolStripMenuItem}
            Dim digits = {Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
                          Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9}
            For i As Integer = 0 To 9
                copyItems(i).ShortcutKeys = Keys.Control Or digits(i)
                copyItems(i).Tag = i
                AddHandler copyItems(i).Click, AddressOf AdvancedCopy_Click
                pasteItems(i).ShortcutKeys = Keys.Alt Or digits(i)
                pasteItems(i).Tag = i
                AddHandler pasteItems(i).Click, AddressOf AdvancedPaste_Click
            Next
        End Sub

        Private Sub AdvancedCopy_Click(sender As Object, e As EventArgs)
            Dim item = TryCast(sender, ToolStripMenuItem)
            If item Is Nothing OrElse item.Tag Is Nothing Then Return
            AdvancedClipboardCopy(CInt(item.Tag))
        End Sub

        Private Sub AdvancedPaste_Click(sender As Object, e As EventArgs)
            Dim item = TryCast(sender, ToolStripMenuItem)
            If item Is Nothing OrElse item.Tag Is Nothing Then Return
            AdvancedClipboardPaste(CInt(item.Tag))
        End Sub

        ''' <summary>Copy the current grid cell into AdvancedClipboard(index, col).
        ''' The GUID/index column is protected (never a copy source).</summary>
        Private Sub AdvancedClipboardCopy(col As Integer)
            If col < 0 OrElse col >= AppInfo.AdvancedClipboardCols Then Return
            If grid.CurrentCell Is Nothing Then Return
            If grid.CurrentCell.ColumnIndex = 0 Then
                SetStatus("Index/GUID column is protected", Palette.Warning)
                Return
            End If
            grid.EndEdit()
            Dim v = grid.CurrentCell.Value
            AppInfo.AdvancedClipboard(AppInfo.AdvancedClipboardIndex, col) = If(v Is Nothing, "", v.ToString())
            UpdateAdvancedClipboardStatus()
            SetStatus($"Copied cell → Advanced Clipboard [{AppInfo.AdvancedClipboardIndex},{col}]", Palette.Success)
        End Sub

        ''' <summary>Paste AdvancedClipboard(index, col) into the current grid cell.
        ''' The GUID/index column (and any read-only cell) is protected.</summary>
        Private Sub AdvancedClipboardPaste(col As Integer)
            If _previewing Then Return
            If col < 0 OrElse col >= AppInfo.AdvancedClipboardCols Then Return
            If grid.CurrentCell Is Nothing Then Return
            If grid.CurrentCell.ColumnIndex = 0 OrElse grid.CurrentCell.ReadOnly Then
                SetStatus("Index/GUID column is protected", Palette.Warning)
                Return
            End If
            grid.EndEdit()
            If Trim(AppInfo.AdvancedClipboard(AppInfo.AdvancedClipboardIndex, col)) <> "" Then  ' guard against empty cells in advancedclipboard
                grid.CurrentCell.Value = AppInfo.AdvancedClipboard(AppInfo.AdvancedClipboardIndex, col)  ' fires CellValueChanged -> dirty
                SetStatus($"Pasted Advanced Clipboard [{AppInfo.AdvancedClipboardIndex},{col}] → cell", Palette.Success)
            Else
                SetStatus($"Advancedclipboard cell empty.", Palette.Success)
            End If
        End Sub

        ''' <summary>Open the Options window (edits the shared AppSettings in place).</summary>
        Private Sub OpenOptions()
            Using dlg As New OptionsForm(_settings)
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    ' Reconcile the running autosave state with the new setting.
                    If Not _settings.AutosaveEnabled Then StopAutosave()
                    SetStatus("Options saved", Palette.Success)
                End If
            End Using
        End Sub

        Private Sub mnuShowConfig_Click(sender As Object, e As EventArgs) Handles mnuShowConfig.Click
            ShowConfigFiles()
        End Sub

        ''' <summary>Open Windows Explorer at the folder holding the JSON config files
        ''' (settings + WebCrawler config), under %LocalAppData%\CsvLibrarian.</summary>
        Private Sub ShowConfigFiles()
            ' GetConfigPath() creates the folder if needed; take its directory.
            Dim dir As String = Path.GetDirectoryName(WebConfigStore.GetConfigPath())
            Try
                Process.Start(New ProcessStartInfo With {.FileName = dir, .UseShellExecute = True})
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Show Config Files", "Could not open the config folder:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Warning)
            End Try
        End Sub

        ' ── Right-click web search (Manufacturer Part Number column) ─────────────────

        ''' <summary>Data column whose cells offer the web-search context menu.</summary>
        Private Const PartNumberColumn As String = "Manufacturer Part Number 1"

        ''' <summary>Effective bound-column name (falls back to Name when unbound).</summary>
        Private Shared Function ColumnDataName(col As DataGridViewColumn) As String
            If col Is Nothing Then Return ""
            Return If(String.IsNullOrEmpty(col.DataPropertyName), col.Name, col.DataPropertyName)
        End Function

        ''' <summary>
        ''' On right-click in the "Manufacturer Part Number 1" column, offer a menu of
        ''' the enabled search engines from the web config. If the config is missing or
        ''' defines no enabled engines, no menu is shown (any other column, no menu).
        ''' Only the website names are listed — the query is used on click, not shown.
        ''' Every defined engine is listed and searchable regardless of its Enabled
        ''' flag; only the topmost "All" item obeys Enabled (it searches every enabled
        ''' engine at once, and is disabled when none are enabled).
        ''' </summary>
        Private Sub grid_CellContextMenuStripNeeded(sender As Object, e As DataGridViewCellContextMenuStripNeededEventArgs) Handles grid.CellContextMenuStripNeeded
            e.ContextMenuStrip = Nothing
            If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return          ' header / row header
            If Not String.Equals(ColumnDataName(grid.Columns(e.ColumnIndex)), PartNumberColumn,
                                 StringComparison.OrdinalIgnoreCase) Then Return

            ' Every engine with a non-blank website. Missing file / none defined -> no menu.
            Dim engines = WebConfigStore.Load().Engines.
                Where(Function(en) en IsNot Nothing AndAlso
                                   Not String.IsNullOrWhiteSpace(en.Website)).ToList()
            If engines.Count = 0 Then Return

            ' Capture the right-clicked cell's contents for the click handler.
            Dim cell = grid.Rows(e.RowIndex).Cells(e.ColumnIndex)
            _webMenuCellText = If(cell.Value Is Nothing, "", cell.Value.ToString())
            ' "All" searches only the enabled engines.
            _webMenuQueries.Clear()
            For Each en In engines.Where(Function(en2) en2.Enabled)
                _webMenuQueries.Add(If(en.Query, ""))
            Next

            _webMenu.Items.Clear()
            ' Topmost "All" (Tag left Nothing to mark it), then a divider, then each engine.
            _webMenu.Items.Add(New ToolStripMenuItem("All") With {.Enabled = _webMenuQueries.Count > 0})
            _webMenu.Items.Add(New ToolStripSeparator())
            For Each en In engines
                _webMenu.Items.Add(New ToolStripMenuItem(en.Website) With {.Tag = If(en.Query, "")})
            Next
            e.ContextMenuStrip = _webMenu
        End Sub

        ''' <summary>Handle a click: "All" (Tag is Nothing) searches every enabled engine;
        ''' otherwise the clicked engine's query is used (regardless of its Enabled flag).</summary>
        Private Sub WebMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles _webMenu.ItemClicked
            Dim query = TryCast(e.ClickedItem.Tag, String)
            If query Is Nothing Then
                For Each q In _webMenuQueries
                    OpenSearch(q)
                Next
            Else
                OpenSearch(query)
            End If
        End Sub

        ''' <summary>Open the default browser at (query &amp; URL-encoded right-click
        ''' cell contents).</summary>
        Private Sub OpenSearch(query As String)
            OpenWebSearch(query, _webMenuCellText)
        End Sub

        ''' <summary>Open the default browser at (query &amp; URL-encoded cell text).</summary>
        Private Sub OpenWebSearch(query As String, cellText As String)
            Dim url = If(query, "") & Uri.EscapeDataString(If(cellText, ""))
            If url.Length = 0 Then Return
            Try
                Process.Start(New ProcessStartInfo With {.FileName = url, .UseShellExecute = True})
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Open Website", "Could not open the browser:" & vbCrLf & ex.Message,
                                     icon:=DialogIcon.Warning)
            End Try
        End Sub

        ''' <summary>
        ''' Tools ▸ Quick Web Lookup (Ctrl+W). Take the "Manufacturer Part Number 1"
        ''' value from the current cursor row and search every engine whose QuickLook
        ''' flag is set (the Enabled flag is ignored here).
        ''' </summary>
        Private Sub QuickWebLookup()
            If ActiveTable Is Nothing OrElse grid.CurrentCell Is Nothing Then Return
            Dim rowIndex = grid.CurrentCell.RowIndex
            If rowIndex < 0 Then Return

            ' Locate the part-number column (by bound name, any cursor column).
            Dim colIndex = -1
            For i = 0 To grid.Columns.Count - 1
                If String.Equals(ColumnDataName(grid.Columns(i)), PartNumberColumn,
                                 StringComparison.OrdinalIgnoreCase) Then
                    colIndex = i
                    Exit For
                End If
            Next
            If colIndex < 0 Then
                SetStatus($"No '{PartNumberColumn}' column in this file", Palette.Warning)
                Return
            End If

            Dim cellValue = grid.Rows(rowIndex).Cells(colIndex).Value
            Dim partNo = If(cellValue Is Nothing, "", cellValue.ToString())
            If String.IsNullOrWhiteSpace(partNo) Then
                SetStatus("The current row has no part number to look up", Palette.Warning)
                Return
            End If

            ' QuickLook engines only; Enabled is ignored on purpose.
            Dim engines = WebConfigStore.Load().Engines.
                Where(Function(en) en IsNot Nothing AndAlso en.QuickLook AndAlso
                                   Not String.IsNullOrWhiteSpace(en.Website)).ToList()
            If engines.Count = 0 Then
                SetStatus("No QuickLook search engines are configured", Palette.Warning)
                Return
            End If

            For Each en In engines
                OpenWebSearch(If(en.Query, ""), partNo)
            Next
            SetStatus($"Quick lookup: {engines.Count} site{If(engines.Count = 1, "", "s")}", Palette.Success)
        End Sub

        Private Sub mnuHelpRef_Click(sender As Object, e As EventArgs) Handles mnuHelpRef.Click
            ShowHelp()
        End Sub

        Private Sub mnuAbout_Click(sender As Object, e As EventArgs) Handles mnuAbout.Click
            ShowAbout()
        End Sub

        Private Sub mnu_tools_ConvertID_Click(sender As Object, e As EventArgs) Handles mnu_tools_ConvertID.Click
            ConvertIdToIndex()
        End Sub

        Private Sub ExportRowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportRowToolStripMenuItem.Click
            ExportRecord()
        End Sub

        Private Sub ImportRecordsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportRecordsToolStripMenuItem.Click
            OpenImportRecords()
        End Sub

        ''' <summary>Open (or re-focus) the non-modal Import Records window for the
        ''' current working folder. A single instance is kept.</summary>
        Private Sub OpenImportRecords()
            If String.IsNullOrEmpty(_folder) Then
                ConfirmDialog.Notify(Me, "Import Records", "Open a working folder first.", icon:=DialogIcon.Warning)
                Return
            End If
            If _importForm IsNot Nothing AndAlso Not _importForm.IsDisposed Then
                _importForm.Activate()
                Return
            End If
            _importForm = New ImportRecordsForm(_folder)
            AddHandler _importForm.FormClosed, Sub() _importForm = Nothing
            _importForm.Show(Me)   ' non-modal; owned so it stays above the main window
        End Sub

        Private Sub QuickWebLookupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QuickWebLookupToolStripMenuItem.Click
            QuickWebLookup()
        End Sub

    End Class

End Namespace
