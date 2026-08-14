Namespace Forms

    Partial Class MainForm
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
            components = New ComponentModel.Container()
            menuStrip = New MenuStrip()
            mnuFile = New ToolStripMenuItem()
            mnuOpen = New ToolStripMenuItem()
            mnuReload = New ToolStripMenuItem()
            mnuOpenWorkFolder = New ToolStripMenuItem()
            mnuNewLibrary = New ToolStripMenuItem()
            sepFileArchive = New ToolStripSeparator()
            mnuCreateArchive = New ToolStripMenuItem()
            sepFile1 = New ToolStripSeparator()
            mnuSave = New ToolStripMenuItem()
            mnuSaveAll = New ToolStripMenuItem()
            sepFile2 = New ToolStripSeparator()
            mnuRecent = New ToolStripMenuItem()
            mnuExit = New ToolStripMenuItem()
            mnuEdit = New ToolStripMenuItem()
            CopyToolStripMenuItem = New ToolStripMenuItem()
            PasteToolStripMenuItem = New ToolStripMenuItem()
            sepEditDel = New ToolStripSeparator()
            mnuDeleteRow = New ToolStripMenuItem()
            ToolStripMenuItem1 = New ToolStripSeparator()
            FindToolStripMenuItem = New ToolStripMenuItem()
            FindCellToolStripMenuItem = New ToolStripMenuItem()
            FindNextToolStripMenuItem = New ToolStripMenuItem()
            FindInColumnToolStripMenuItem = New ToolStripMenuItem()
            ToolStripMenuItem2 = New ToolStripSeparator()
            ReplaceToolStripMenuItem = New ToolStripMenuItem()
            ReplaceNextToolStripMenuItem = New ToolStripMenuItem()
            ReplaceInSameColumnToolStripMenuItem = New ToolStripMenuItem()
            ToolStripMenuItem3 = New ToolStripSeparator()
            mnuAddRow = New ToolStripMenuItem()
            mnuAddClone = New ToolStripMenuItem()
            mnuCopyAbove = New ToolStripMenuItem()
            sepEdit1 = New ToolStripSeparator()
            mnuColumns = New ToolStripMenuItem()
            mnuAutosize = New ToolStripMenuItem()
            sepEdit2 = New ToolStripSeparator()
            mnuNormalize = New ToolStripMenuItem()
            sepEdit3 = New ToolStripSeparator()
            mnuRemove = New ToolStripMenuItem()
            mnuHelp = New ToolStripMenuItem()
            mnuHelpRef = New ToolStripMenuItem()
            sepHelp1 = New ToolStripSeparator()
            mnuAbout = New ToolStripMenuItem()
            ToolsToolStripMenuItem = New ToolStripMenuItem()
            mnu_tools_ConvertID = New ToolStripMenuItem()
            QuickWebLookupToolStripMenuItem = New ToolStripMenuItem()
            mnuSettings = New ToolStripMenuItem()
            mnuRules = New ToolStripMenuItem()
            mnuWebCrawler = New ToolStripMenuItem()
            mnuAdvancedClipboard = New ToolStripMenuItem()
            mnuOptions = New ToolStripMenuItem()
            sepSettings1 = New ToolStripSeparator()
            mnuShowConfig = New ToolStripMenuItem()
            sepSettings2 = New ToolStripSeparator()
            AdvancedClipboardFunctionsToolStripMenuItem = New ToolStripMenuItem()
            CopyToolStripMenuItem1 = New ToolStripMenuItem()
            Copy0ToolStripMenuItem = New ToolStripMenuItem()
            Copy1ToolStripMenuItem = New ToolStripMenuItem()
            Copy2ToolStripMenuItem = New ToolStripMenuItem()
            Copy3ToolStripMenuItem = New ToolStripMenuItem()
            Copy4ToolStripMenuItem = New ToolStripMenuItem()
            Copy5ToolStripMenuItem = New ToolStripMenuItem()
            Copy6ToolStripMenuItem = New ToolStripMenuItem()
            Copy7ToolStripMenuItem = New ToolStripMenuItem()
            Copy8ToolStripMenuItem = New ToolStripMenuItem()
            Copy9ToolStripMenuItem = New ToolStripMenuItem()
            PasteToolStripMenuItem1 = New ToolStripMenuItem()
            Paste0ToolStripMenuItem = New ToolStripMenuItem()
            Paste1ToolStripMenuItem = New ToolStripMenuItem()
            Paste2ToolStripMenuItem = New ToolStripMenuItem()
            Paste3ToolStripMenuItem = New ToolStripMenuItem()
            Paste4ToolStripMenuItem = New ToolStripMenuItem()
            Paste5ToolStripMenuItem = New ToolStripMenuItem()
            Paste6ToolStripMenuItem = New ToolStripMenuItem()
            Paste7ToolStripMenuItem = New ToolStripMenuItem()
            Paste8ToolStripMenuItem = New ToolStripMenuItem()
            Paste9ToolStripMenuItem = New ToolStripMenuItem()
            IncrementToolStripMenuItem = New ToolStripMenuItem()
            DecrementToolStripMenuItem = New ToolStripMenuItem()
            statusStrip = New StatusStrip()
            folderStatus = New ToolStripStatusLabel()
            AdvancedClipboardContent = New ToolStripStatusLabel()
            findStatus = New ToolStripStatusLabel()
            replaceStatus = New ToolStripStatusLabel()
            rowCountStatus = New ToolStripStatusLabel()
            saveStatus = New ToolStripStatusLabel()
            split = New SplitContainer()
            grpLibraries = New GroupBox()
            fileList = New ListBox()
            grid = New DataGridView()
            emptyPanel = New Panel()
            lblEmptyIcon = New Label()
            lblEmpty1 = New Label()
            lblEmpty2 = New Label()
            divider = New Panel()
            gridToolbar = New Panel()
            backupCluster = New FlowLayoutPanel()
            lblBackups = New Label()
            cboBackups = New ComboBox()
            btnRestore = New Button()
            rightCluster = New FlowLayoutPanel()
            btnAutosize = New Button()
            btnNormalize = New Button()
            btnSave = New Button()
            btnSaveAll = New Button()
            bindingSrc = New BindingSource(components)
            autosaveTimer = New Timer(components)
            menuStrip.SuspendLayout()
            statusStrip.SuspendLayout()
            CType(split, ComponentModel.ISupportInitialize).BeginInit()
            split.Panel1.SuspendLayout()
            split.Panel2.SuspendLayout()
            split.SuspendLayout()
            grpLibraries.SuspendLayout()
            CType(grid, ComponentModel.ISupportInitialize).BeginInit()
            emptyPanel.SuspendLayout()
            gridToolbar.SuspendLayout()
            backupCluster.SuspendLayout()
            rightCluster.SuspendLayout()
            CType(bindingSrc, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' menuStrip
            ' 
            menuStrip.Items.AddRange(New ToolStripItem() {mnuFile, mnuEdit, mnuHelp, ToolsToolStripMenuItem, mnuSettings})
            menuStrip.Location = New Point(0, 0)
            menuStrip.Name = "menuStrip"
            menuStrip.Size = New Size(1280, 24)
            menuStrip.TabIndex = 0
            ' 
            ' mnuFile
            ' 
            mnuFile.DropDownItems.AddRange(New ToolStripItem() {mnuOpen, mnuReload, mnuOpenWorkFolder, mnuNewLibrary, sepFileArchive, mnuCreateArchive, sepFile1, mnuSave, mnuSaveAll, sepFile2, mnuRecent, mnuExit})
            mnuFile.Name = "mnuFile"
            mnuFile.Size = New Size(37, 20)
            mnuFile.Text = "&File"
            ' 
            ' mnuOpen
            ' 
            mnuOpen.Name = "mnuOpen"
            mnuOpen.ShortcutKeys = Keys.Control Or Keys.O
            mnuOpen.Size = New Size(227, 22)
            mnuOpen.Text = "&Open Folder…"
            ' 
            ' mnuReload
            ' 
            mnuReload.Name = "mnuReload"
            mnuReload.ShortcutKeys = Keys.F5
            mnuReload.Size = New Size(227, 22)
            mnuReload.Text = "&Reload Folder"
            ' 
            ' mnuOpenWorkFolder
            ' 
            mnuOpenWorkFolder.Enabled = False
            mnuOpenWorkFolder.Name = "mnuOpenWorkFolder"
            mnuOpenWorkFolder.ShortcutKeys = Keys.Control Or Keys.L
            mnuOpenWorkFolder.Size = New Size(227, 22)
            mnuOpenWorkFolder.Text = "Open &Working Folder"
            ' 
            ' mnuNewLibrary
            ' 
            mnuNewLibrary.Enabled = False
            mnuNewLibrary.Name = "mnuNewLibrary"
            mnuNewLibrary.ShortcutKeys = Keys.Control Or Keys.Shift Or Keys.L
            mnuNewLibrary.Size = New Size(227, 22)
            mnuNewLibrary.Text = "&New Library…"
            ' 
            ' sepFileArchive
            ' 
            sepFileArchive.Name = "sepFileArchive"
            sepFileArchive.Size = New Size(224, 6)
            ' 
            ' mnuCreateArchive
            ' 
            mnuCreateArchive.Enabled = False
            mnuCreateArchive.Name = "mnuCreateArchive"
            mnuCreateArchive.Size = New Size(227, 22)
            mnuCreateArchive.Text = "Create &Archive"
            ' 
            ' sepFile1
            ' 
            sepFile1.Name = "sepFile1"
            sepFile1.Size = New Size(224, 6)
            ' 
            ' mnuSave
            ' 
            mnuSave.Name = "mnuSave"
            mnuSave.ShortcutKeys = Keys.Control Or Keys.S
            mnuSave.Size = New Size(227, 22)
            mnuSave.Text = "&Save"
            ' 
            ' mnuSaveAll
            ' 
            mnuSaveAll.Name = "mnuSaveAll"
            mnuSaveAll.ShortcutKeys = Keys.Control Or Keys.Shift Or Keys.S
            mnuSaveAll.Size = New Size(227, 22)
            mnuSaveAll.Text = "Save &All"
            ' 
            ' sepFile2
            ' 
            sepFile2.Name = "sepFile2"
            sepFile2.Size = New Size(224, 6)
            ' 
            ' mnuRecent
            ' 
            mnuRecent.Name = "mnuRecent"
            mnuRecent.Size = New Size(227, 22)
            mnuRecent.Text = "Recently &Used"
            ' 
            ' mnuExit
            ' 
            mnuExit.Name = "mnuExit"
            mnuExit.ShortcutKeys = Keys.Alt Or Keys.F4
            mnuExit.Size = New Size(227, 22)
            mnuExit.Text = "E&xit"
            ' 
            ' mnuEdit
            ' 
            mnuEdit.DropDownItems.AddRange(New ToolStripItem() {CopyToolStripMenuItem, PasteToolStripMenuItem, sepEditDel, mnuDeleteRow, ToolStripMenuItem1, FindToolStripMenuItem, FindCellToolStripMenuItem, FindNextToolStripMenuItem, FindInColumnToolStripMenuItem, ToolStripMenuItem2, ReplaceToolStripMenuItem, ReplaceNextToolStripMenuItem, ReplaceInSameColumnToolStripMenuItem, ToolStripMenuItem3, mnuAddRow, mnuAddClone, mnuCopyAbove, sepEdit1, mnuColumns, mnuAutosize, sepEdit2, mnuNormalize, sepEdit3, mnuRemove})
            mnuEdit.Name = "mnuEdit"
            mnuEdit.Size = New Size(39, 20)
            mnuEdit.Text = "&Edit"
            ' 
            ' CopyToolStripMenuItem
            ' 
            CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
            CopyToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.C
            CopyToolStripMenuItem.Size = New Size(270, 22)
            CopyToolStripMenuItem.Text = "Copy"
            ' 
            ' PasteToolStripMenuItem
            ' 
            PasteToolStripMenuItem.Name = "PasteToolStripMenuItem"
            PasteToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.V
            PasteToolStripMenuItem.Size = New Size(270, 22)
            PasteToolStripMenuItem.Text = "Paste"
            ' 
            ' sepEditDel
            ' 
            sepEditDel.Name = "sepEditDel"
            sepEditDel.Size = New Size(267, 6)
            ' 
            ' mnuDeleteRow
            ' 
            mnuDeleteRow.Name = "mnuDeleteRow"
            mnuDeleteRow.Size = New Size(270, 22)
            mnuDeleteRow.Text = "Delete Row"
            ' 
            ' ToolStripMenuItem1
            ' 
            ToolStripMenuItem1.Name = "ToolStripMenuItem1"
            ToolStripMenuItem1.Size = New Size(267, 6)
            ' 
            ' FindToolStripMenuItem
            ' 
            FindToolStripMenuItem.Name = "FindToolStripMenuItem"
            FindToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.F
            FindToolStripMenuItem.Size = New Size(270, 22)
            FindToolStripMenuItem.Text = "Find"
            ' 
            ' FindCellToolStripMenuItem
            ' 
            FindCellToolStripMenuItem.Name = "FindCellToolStripMenuItem"
            FindCellToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.Shift Or Keys.F
            FindCellToolStripMenuItem.Size = New Size(270, 22)
            FindCellToolStripMenuItem.Text = "Find Cell"
            ' 
            ' FindNextToolStripMenuItem
            ' 
            FindNextToolStripMenuItem.Name = "FindNextToolStripMenuItem"
            FindNextToolStripMenuItem.ShortcutKeys = Keys.F3
            FindNextToolStripMenuItem.Size = New Size(270, 22)
            FindNextToolStripMenuItem.Text = "Find Next"
            ' 
            ' FindInColumnToolStripMenuItem
            ' 
            FindInColumnToolStripMenuItem.Name = "FindInColumnToolStripMenuItem"
            FindInColumnToolStripMenuItem.ShortcutKeys = Keys.Shift Or Keys.F3
            FindInColumnToolStripMenuItem.Size = New Size(270, 22)
            FindInColumnToolStripMenuItem.Text = "Find Next in Column"
            ' 
            ' ToolStripMenuItem2
            ' 
            ToolStripMenuItem2.Name = "ToolStripMenuItem2"
            ToolStripMenuItem2.Size = New Size(267, 6)
            ' 
            ' ReplaceToolStripMenuItem
            ' 
            ReplaceToolStripMenuItem.Name = "ReplaceToolStripMenuItem"
            ReplaceToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.R
            ReplaceToolStripMenuItem.Size = New Size(270, 22)
            ReplaceToolStripMenuItem.Text = "Replace"
            ' 
            ' ReplaceNextToolStripMenuItem
            ' 
            ReplaceNextToolStripMenuItem.Name = "ReplaceNextToolStripMenuItem"
            ReplaceNextToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.Shift Or Keys.R
            ReplaceNextToolStripMenuItem.Size = New Size(270, 22)
            ReplaceNextToolStripMenuItem.Text = "Replace Next"
            ' 
            ' ReplaceInSameColumnToolStripMenuItem
            ' 
            ReplaceInSameColumnToolStripMenuItem.Name = "ReplaceInSameColumnToolStripMenuItem"
            ReplaceInSameColumnToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.Alt Or Keys.R
            ReplaceInSameColumnToolStripMenuItem.Size = New Size(270, 22)
            ReplaceInSameColumnToolStripMenuItem.Text = "Replace In Same Column"
            ' 
            ' ToolStripMenuItem3
            ' 
            ToolStripMenuItem3.Name = "ToolStripMenuItem3"
            ToolStripMenuItem3.Size = New Size(267, 6)
            ' 
            ' mnuAddRow
            ' 
            mnuAddRow.Name = "mnuAddRow"
            mnuAddRow.ShortcutKeys = Keys.Insert
            mnuAddRow.Size = New Size(270, 22)
            mnuAddRow.Text = "&Add Row"
            ' 
            ' mnuAddClone
            ' 
            mnuAddClone.Name = "mnuAddClone"
            mnuAddClone.ShortcutKeys = Keys.Control Or Keys.Insert
            mnuAddClone.Size = New Size(270, 22)
            mnuAddClone.Text = "Add &Cloned Row"
            ' 
            ' mnuCopyAbove
            ' 
            mnuCopyAbove.Name = "mnuCopyAbove"
            mnuCopyAbove.ShortcutKeys = Keys.Control Or Keys.D
            mnuCopyAbove.Size = New Size(270, 22)
            mnuCopyAbove.Text = "Copy From Cell A&bove"
            ' 
            ' sepEdit1
            ' 
            sepEdit1.Name = "sepEdit1"
            sepEdit1.Size = New Size(267, 6)
            ' 
            ' mnuColumns
            ' 
            mnuColumns.Name = "mnuColumns"
            mnuColumns.Size = New Size(270, 22)
            mnuColumns.Text = "&Manage Columns…"
            ' 
            ' mnuAutosize
            ' 
            mnuAutosize.Name = "mnuAutosize"
            mnuAutosize.ShortcutKeys = Keys.Control Or Keys.Q
            mnuAutosize.Size = New Size(270, 22)
            mnuAutosize.Text = "Auto&size Columns"
            ' 
            ' sepEdit2
            ' 
            sepEdit2.Name = "sepEdit2"
            sepEdit2.Size = New Size(267, 6)
            ' 
            ' mnuNormalize
            ' 
            mnuNormalize.Name = "mnuNormalize"
            mnuNormalize.ShortcutKeys = Keys.Control Or Keys.N
            mnuNormalize.Size = New Size(270, 22)
            mnuNormalize.Text = "&Normalize Fields"
            ' 
            ' sepEdit3
            ' 
            sepEdit3.Name = "sepEdit3"
            sepEdit3.Size = New Size(267, 6)
            sepEdit3.Visible = False
            ' 
            ' mnuRemove
            ' 
            mnuRemove.Enabled = False
            mnuRemove.Name = "mnuRemove"
            mnuRemove.Size = New Size(270, 22)
            mnuRemove.Text = "&Remove File"
            mnuRemove.Visible = False
            ' 
            ' mnuHelp
            ' 
            mnuHelp.Alignment = ToolStripItemAlignment.Right
            mnuHelp.DropDownItems.AddRange(New ToolStripItem() {mnuHelpRef, sepHelp1, mnuAbout})
            mnuHelp.Name = "mnuHelp"
            mnuHelp.Size = New Size(44, 20)
            mnuHelp.Text = "&Help"
            ' 
            ' mnuHelpRef
            ' 
            mnuHelpRef.Name = "mnuHelpRef"
            mnuHelpRef.ShortcutKeys = Keys.F1
            mnuHelpRef.Size = New Size(253, 22)
            mnuHelpRef.Text = "&Keyboard && Feature Reference"
            ' 
            ' sepHelp1
            ' 
            sepHelp1.Name = "sepHelp1"
            sepHelp1.Size = New Size(250, 6)
            ' 
            ' mnuAbout
            ' 
            mnuAbout.Name = "mnuAbout"
            mnuAbout.Size = New Size(253, 22)
            mnuAbout.Text = "&About…"
            ' 
            ' ToolsToolStripMenuItem
            ' 
            ToolsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {mnu_tools_ConvertID, QuickWebLookupToolStripMenuItem})
            ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
            ToolsToolStripMenuItem.Size = New Size(47, 20)
            ToolsToolStripMenuItem.Text = "Tools"
            ' 
            ' mnu_tools_ConvertID
            ' 
            mnu_tools_ConvertID.Name = "mnu_tools_ConvertID"
            mnu_tools_ConvertID.Size = New Size(220, 22)
            mnu_tools_ConvertID.Text = "Convert ID"
            ' 
            ' QuickWebLookupToolStripMenuItem
            ' 
            QuickWebLookupToolStripMenuItem.Name = "QuickWebLookupToolStripMenuItem"
            QuickWebLookupToolStripMenuItem.ShortcutKeys = Keys.Control Or Keys.W
            QuickWebLookupToolStripMenuItem.Size = New Size(220, 22)
            QuickWebLookupToolStripMenuItem.Text = "Quick Web Lookup"
            ' 
            ' mnuSettings
            ' 
            mnuSettings.DropDownItems.AddRange(New ToolStripItem() {mnuRules, mnuWebCrawler, mnuAdvancedClipboard, mnuOptions, sepSettings1, mnuShowConfig, sepSettings2, AdvancedClipboardFunctionsToolStripMenuItem})
            mnuSettings.Name = "mnuSettings"
            mnuSettings.Size = New Size(61, 20)
            mnuSettings.Text = "&Settings"
            ' 
            ' mnuRules
            ' 
            mnuRules.Name = "mnuRules"
            mnuRules.Size = New Size(231, 22)
            mnuRules.Text = "Normalizer &Rules…"
            ' 
            ' mnuWebCrawler
            ' 
            mnuWebCrawler.Name = "mnuWebCrawler"
            mnuWebCrawler.Size = New Size(231, 22)
            mnuWebCrawler.Text = "&WebCrawler…"
            ' 
            ' mnuAdvancedClipboard
            ' 
            mnuAdvancedClipboard.Name = "mnuAdvancedClipboard"
            mnuAdvancedClipboard.Size = New Size(231, 22)
            mnuAdvancedClipboard.Text = "Advanced &Clipboard…"
            ' 
            ' mnuOptions
            ' 
            mnuOptions.Name = "mnuOptions"
            mnuOptions.Size = New Size(231, 22)
            mnuOptions.Text = "&Options…"
            ' 
            ' sepSettings1
            ' 
            sepSettings1.Name = "sepSettings1"
            sepSettings1.Size = New Size(228, 6)
            ' 
            ' mnuShowConfig
            ' 
            mnuShowConfig.Name = "mnuShowConfig"
            mnuShowConfig.Size = New Size(231, 22)
            mnuShowConfig.Text = "Show &Config Files"
            ' 
            ' sepSettings2
            ' 
            sepSettings2.Name = "sepSettings2"
            sepSettings2.Size = New Size(228, 6)
            ' 
            ' AdvancedClipboardFunctionsToolStripMenuItem
            ' 
            AdvancedClipboardFunctionsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {CopyToolStripMenuItem1, PasteToolStripMenuItem1, IncrementToolStripMenuItem, DecrementToolStripMenuItem})
            AdvancedClipboardFunctionsToolStripMenuItem.Name = "AdvancedClipboardFunctionsToolStripMenuItem"
            AdvancedClipboardFunctionsToolStripMenuItem.Size = New Size(231, 22)
            AdvancedClipboardFunctionsToolStripMenuItem.Text = "AdvancedClipboardFunctions"
            AdvancedClipboardFunctionsToolStripMenuItem.Visible = False
            ' 
            ' CopyToolStripMenuItem1
            ' 
            CopyToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {Copy0ToolStripMenuItem, Copy1ToolStripMenuItem, Copy2ToolStripMenuItem, Copy3ToolStripMenuItem, Copy4ToolStripMenuItem, Copy5ToolStripMenuItem, Copy6ToolStripMenuItem, Copy7ToolStripMenuItem, Copy8ToolStripMenuItem, Copy9ToolStripMenuItem})
            CopyToolStripMenuItem1.Name = "CopyToolStripMenuItem1"
            CopyToolStripMenuItem1.Size = New Size(189, 22)
            CopyToolStripMenuItem1.Text = "Copy"
            ' 
            ' Copy0ToolStripMenuItem
            ' 
            Copy0ToolStripMenuItem.Name = "Copy0ToolStripMenuItem"
            Copy0ToolStripMenuItem.Size = New Size(111, 22)
            Copy0ToolStripMenuItem.Text = "Copy 0"
            ' 
            ' Copy1ToolStripMenuItem
            ' 
            Copy1ToolStripMenuItem.Name = "Copy1ToolStripMenuItem"
            Copy1ToolStripMenuItem.Size = New Size(111, 22)
            Copy1ToolStripMenuItem.Text = "Copy 1"
            ' 
            ' Copy2ToolStripMenuItem
            ' 
            Copy2ToolStripMenuItem.Name = "Copy2ToolStripMenuItem"
            Copy2ToolStripMenuItem.Size = New Size(111, 22)
            Copy2ToolStripMenuItem.Text = "Copy 2"
            ' 
            ' Copy3ToolStripMenuItem
            ' 
            Copy3ToolStripMenuItem.Name = "Copy3ToolStripMenuItem"
            Copy3ToolStripMenuItem.Size = New Size(111, 22)
            Copy3ToolStripMenuItem.Text = "Copy 3"
            ' 
            ' Copy4ToolStripMenuItem
            ' 
            Copy4ToolStripMenuItem.Name = "Copy4ToolStripMenuItem"
            Copy4ToolStripMenuItem.Size = New Size(111, 22)
            Copy4ToolStripMenuItem.Text = "Copy 4"
            ' 
            ' Copy5ToolStripMenuItem
            ' 
            Copy5ToolStripMenuItem.Name = "Copy5ToolStripMenuItem"
            Copy5ToolStripMenuItem.Size = New Size(111, 22)
            Copy5ToolStripMenuItem.Text = "Copy 5"
            ' 
            ' Copy6ToolStripMenuItem
            ' 
            Copy6ToolStripMenuItem.Name = "Copy6ToolStripMenuItem"
            Copy6ToolStripMenuItem.Size = New Size(111, 22)
            Copy6ToolStripMenuItem.Text = "Copy 6"
            ' 
            ' Copy7ToolStripMenuItem
            ' 
            Copy7ToolStripMenuItem.Name = "Copy7ToolStripMenuItem"
            Copy7ToolStripMenuItem.Size = New Size(111, 22)
            Copy7ToolStripMenuItem.Text = "Copy 7"
            ' 
            ' Copy8ToolStripMenuItem
            ' 
            Copy8ToolStripMenuItem.Name = "Copy8ToolStripMenuItem"
            Copy8ToolStripMenuItem.Size = New Size(111, 22)
            Copy8ToolStripMenuItem.Text = "Copy 8"
            ' 
            ' Copy9ToolStripMenuItem
            ' 
            Copy9ToolStripMenuItem.Name = "Copy9ToolStripMenuItem"
            Copy9ToolStripMenuItem.Size = New Size(111, 22)
            Copy9ToolStripMenuItem.Text = "Copy 9"
            ' 
            ' PasteToolStripMenuItem1
            ' 
            PasteToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {Paste0ToolStripMenuItem, Paste1ToolStripMenuItem, Paste2ToolStripMenuItem, Paste3ToolStripMenuItem, Paste4ToolStripMenuItem, Paste5ToolStripMenuItem, Paste6ToolStripMenuItem, Paste7ToolStripMenuItem, Paste8ToolStripMenuItem, Paste9ToolStripMenuItem})
            PasteToolStripMenuItem1.Name = "PasteToolStripMenuItem1"
            PasteToolStripMenuItem1.Size = New Size(189, 22)
            PasteToolStripMenuItem1.Text = "Paste"
            ' 
            ' Paste0ToolStripMenuItem
            ' 
            Paste0ToolStripMenuItem.Name = "Paste0ToolStripMenuItem"
            Paste0ToolStripMenuItem.Size = New Size(111, 22)
            Paste0ToolStripMenuItem.Text = "Paste 0"
            ' 
            ' Paste1ToolStripMenuItem
            ' 
            Paste1ToolStripMenuItem.Name = "Paste1ToolStripMenuItem"
            Paste1ToolStripMenuItem.Size = New Size(111, 22)
            Paste1ToolStripMenuItem.Text = "Paste 1"
            ' 
            ' Paste2ToolStripMenuItem
            ' 
            Paste2ToolStripMenuItem.Name = "Paste2ToolStripMenuItem"
            Paste2ToolStripMenuItem.Size = New Size(111, 22)
            Paste2ToolStripMenuItem.Text = "Paste 2"
            ' 
            ' Paste3ToolStripMenuItem
            ' 
            Paste3ToolStripMenuItem.Name = "Paste3ToolStripMenuItem"
            Paste3ToolStripMenuItem.Size = New Size(111, 22)
            Paste3ToolStripMenuItem.Text = "Paste 3"
            ' 
            ' Paste4ToolStripMenuItem
            ' 
            Paste4ToolStripMenuItem.Name = "Paste4ToolStripMenuItem"
            Paste4ToolStripMenuItem.Size = New Size(111, 22)
            Paste4ToolStripMenuItem.Text = "Paste 4"
            ' 
            ' Paste5ToolStripMenuItem
            ' 
            Paste5ToolStripMenuItem.Name = "Paste5ToolStripMenuItem"
            Paste5ToolStripMenuItem.Size = New Size(111, 22)
            Paste5ToolStripMenuItem.Text = "Paste 5"
            ' 
            ' Paste6ToolStripMenuItem
            ' 
            Paste6ToolStripMenuItem.Name = "Paste6ToolStripMenuItem"
            Paste6ToolStripMenuItem.Size = New Size(111, 22)
            Paste6ToolStripMenuItem.Text = "Paste 6"
            ' 
            ' Paste7ToolStripMenuItem
            ' 
            Paste7ToolStripMenuItem.Name = "Paste7ToolStripMenuItem"
            Paste7ToolStripMenuItem.Size = New Size(111, 22)
            Paste7ToolStripMenuItem.Text = "Paste 7"
            ' 
            ' Paste8ToolStripMenuItem
            ' 
            Paste8ToolStripMenuItem.Name = "Paste8ToolStripMenuItem"
            Paste8ToolStripMenuItem.Size = New Size(111, 22)
            Paste8ToolStripMenuItem.Text = "Paste 8"
            ' 
            ' Paste9ToolStripMenuItem
            ' 
            Paste9ToolStripMenuItem.Name = "Paste9ToolStripMenuItem"
            Paste9ToolStripMenuItem.Size = New Size(111, 22)
            Paste9ToolStripMenuItem.Text = "Paste 9"
            ' 
            ' IncrementToolStripMenuItem
            ' 
            IncrementToolStripMenuItem.Name = "IncrementToolStripMenuItem"
            IncrementToolStripMenuItem.ShortcutKeys = Keys.Alt Or Keys.Down
            IncrementToolStripMenuItem.Size = New Size(189, 22)
            IncrementToolStripMenuItem.Text = "Increment"
            ' 
            ' DecrementToolStripMenuItem
            ' 
            DecrementToolStripMenuItem.Name = "DecrementToolStripMenuItem"
            DecrementToolStripMenuItem.ShortcutKeys = Keys.Alt Or Keys.Up
            DecrementToolStripMenuItem.Size = New Size(189, 22)
            DecrementToolStripMenuItem.Text = "Decrement"
            ' 
            ' statusStrip
            ' 
            statusStrip.Items.AddRange(New ToolStripItem() {folderStatus, AdvancedClipboardContent, findStatus, replaceStatus, rowCountStatus, saveStatus})
            statusStrip.Location = New Point(0, 756)
            statusStrip.Name = "statusStrip"
            statusStrip.Size = New Size(1280, 24)
            statusStrip.TabIndex = 3
            ' 
            ' folderStatus
            ' 
            folderStatus.BorderSides = ToolStripStatusLabelBorderSides.Right
            folderStatus.BorderStyle = Border3DStyle.Etched
            folderStatus.Name = "folderStatus"
            folderStatus.Size = New Size(84, 19)
            folderStatus.Text = "Work Library :"
            folderStatus.TextAlign = ContentAlignment.MiddleLeft
            ' 
            ' AdvancedClipboardContent
            ' 
            AdvancedClipboardContent.BorderSides = ToolStripStatusLabelBorderSides.Right
            AdvancedClipboardContent.BorderStyle = Border3DStyle.Etched
            AdvancedClipboardContent.Name = "AdvancedClipboardContent"
            AdvancedClipboardContent.Size = New Size(911, 19)
            AdvancedClipboardContent.Spring = True
            AdvancedClipboardContent.TextAlign = ContentAlignment.MiddleLeft
            ' 
            ' findStatus
            ' 
            findStatus.BorderSides = ToolStripStatusLabelBorderSides.Right
            findStatus.BorderStyle = Border3DStyle.Etched
            findStatus.Name = "findStatus"
            findStatus.Size = New Size(90, 19)
            findStatus.Text = "Find : -Empty- "
            ' 
            ' replaceStatus
            ' 
            replaceStatus.BorderSides = ToolStripStatusLabelBorderSides.Right
            replaceStatus.BorderStyle = Border3DStyle.Etched
            replaceStatus.Name = "replaceStatus"
            replaceStatus.Size = New Size(89, 19)
            replaceStatus.Text = "Replace With : "
            ' 
            ' rowCountStatus
            ' 
            rowCountStatus.BorderSides = ToolStripStatusLabelBorderSides.Right
            rowCountStatus.BorderStyle = Border3DStyle.Etched
            rowCountStatus.Name = "rowCountStatus"
            rowCountStatus.Size = New Size(4, 19)
            rowCountStatus.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' saveStatus
            ' 
            saveStatus.Name = "saveStatus"
            saveStatus.Size = New Size(87, 19)
            saveStatus.Text = "No folder open"
            saveStatus.TextAlign = ContentAlignment.MiddleRight
            ' 
            ' split
            ' 
            split.Dock = DockStyle.Fill
            split.FixedPanel = FixedPanel.Panel1
            split.Location = New Point(0, 24)
            split.Name = "split"
            ' 
            ' split.Panel1
            ' 
            split.Panel1.Controls.Add(grpLibraries)
            split.Panel1.Padding = New Padding(6)
            ' 
            ' split.Panel2
            ' 
            split.Panel2.Controls.Add(grid)
            split.Panel2.Controls.Add(emptyPanel)
            split.Panel2.Controls.Add(divider)
            split.Panel2.Controls.Add(gridToolbar)
            split.Size = New Size(1280, 732)
            split.SplitterDistance = 220
            split.SplitterWidth = 1
            split.TabIndex = 2
            ' 
            ' grpLibraries
            ' 
            grpLibraries.Controls.Add(fileList)
            grpLibraries.Dock = DockStyle.Fill
            grpLibraries.Location = New Point(6, 6)
            grpLibraries.Name = "grpLibraries"
            grpLibraries.Padding = New Padding(6, 3, 6, 6)
            grpLibraries.Size = New Size(208, 720)
            grpLibraries.TabIndex = 0
            grpLibraries.TabStop = False
            grpLibraries.Text = "Libraries"
            ' 
            ' fileList
            ' 
            fileList.Dock = DockStyle.Fill
            fileList.IntegralHeight = False
            fileList.ItemHeight = 15
            fileList.Location = New Point(6, 19)
            fileList.Name = "fileList"
            fileList.Size = New Size(196, 695)
            fileList.TabIndex = 0
            ' 
            ' grid
            ' 
            grid.AllowUserToAddRows = False
            grid.AllowUserToResizeRows = False
            grid.BorderStyle = BorderStyle.None
            grid.ColumnHeadersHeight = 30
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            grid.Dock = DockStyle.Fill
            grid.Location = New Point(0, 33)
            grid.Name = "grid"
            grid.RowHeadersVisible = False
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect
            grid.Size = New Size(1059, 699)
            grid.TabIndex = 3
            grid.Visible = False
            ' 
            ' emptyPanel
            ' 
            emptyPanel.Controls.Add(lblEmptyIcon)
            emptyPanel.Controls.Add(lblEmpty1)
            emptyPanel.Controls.Add(lblEmpty2)
            emptyPanel.Dock = DockStyle.Fill
            emptyPanel.Location = New Point(0, 33)
            emptyPanel.Name = "emptyPanel"
            emptyPanel.Size = New Size(1059, 699)
            emptyPanel.TabIndex = 2
            ' 
            ' lblEmptyIcon
            ' 
            lblEmptyIcon.AutoSize = True
            lblEmptyIcon.Location = New Point(500, 90)
            lblEmptyIcon.Name = "lblEmptyIcon"
            lblEmptyIcon.Size = New Size(19, 15)
            lblEmptyIcon.TabIndex = 0
            lblEmptyIcon.Text = "🗂"
            ' 
            ' lblEmpty1
            ' 
            lblEmpty1.AutoSize = True
            lblEmpty1.Location = New Point(480, 150)
            lblEmpty1.Name = "lblEmpty1"
            lblEmpty1.Size = New Size(88, 15)
            lblEmpty1.TabIndex = 1
            lblEmpty1.Text = "No file selected"
            ' 
            ' lblEmpty2
            ' 
            lblEmpty2.AutoSize = True
            lblEmpty2.Location = New Point(430, 180)
            lblEmpty2.Name = "lblEmpty2"
            lblEmpty2.Size = New Size(186, 30)
            lblEmpty2.TabIndex = 2
            lblEmpty2.Text = "Open a folder from the File menu," & vbCrLf & "then click a file to start editing."
            lblEmpty2.TextAlign = ContentAlignment.MiddleCenter
            ' 
            ' divider
            ' 
            divider.Dock = DockStyle.Top
            divider.Location = New Point(0, 32)
            divider.Name = "divider"
            divider.Size = New Size(1059, 1)
            divider.TabIndex = 1
            ' 
            ' gridToolbar
            ' 
            gridToolbar.Controls.Add(backupCluster)
            gridToolbar.Controls.Add(rightCluster)
            gridToolbar.Dock = DockStyle.Top
            gridToolbar.Location = New Point(0, 0)
            gridToolbar.Name = "gridToolbar"
            gridToolbar.Size = New Size(1059, 32)
            gridToolbar.TabIndex = 0
            ' 
            ' backupCluster
            ' 
            backupCluster.AutoSize = True
            backupCluster.AutoSizeMode = AutoSizeMode.GrowAndShrink
            backupCluster.Controls.Add(lblBackups)
            backupCluster.Controls.Add(cboBackups)
            backupCluster.Controls.Add(btnRestore)
            backupCluster.Dock = DockStyle.Right
            backupCluster.Location = New Point(764, 0)
            backupCluster.Name = "backupCluster"
            backupCluster.Padding = New Padding(4, 3, 6, 3)
            backupCluster.Size = New Size(295, 32)
            backupCluster.TabIndex = 1
            backupCluster.WrapContents = False
            ' 
            ' lblBackups
            ' 
            lblBackups.AutoSize = True
            lblBackups.Location = New Point(4, 11)
            lblBackups.Margin = New Padding(0, 8, 3, 0)
            lblBackups.Name = "lblBackups"
            lblBackups.Size = New Size(54, 15)
            lblBackups.TabIndex = 0
            lblBackups.Text = "Backups:"
            ' 
            ' cboBackups
            ' 
            cboBackups.DropDownStyle = ComboBoxStyle.DropDownList
            cboBackups.FormattingEnabled = True
            cboBackups.Location = New Point(61, 7)
            cboBackups.Margin = New Padding(0, 4, 0, 0)
            cboBackups.Name = "cboBackups"
            cboBackups.Size = New Size(150, 23)
            cboBackups.TabIndex = 1
            ' 
            ' btnRestore
            ' 
            btnRestore.Location = New Point(217, 6)
            btnRestore.Margin = New Padding(6, 3, 0, 0)
            btnRestore.Name = "btnRestore"
            btnRestore.Size = New Size(72, 24)
            btnRestore.TabIndex = 2
            btnRestore.Text = "Restore"
            btnRestore.UseVisualStyleBackColor = True
            ' 
            ' rightCluster
            ' 
            rightCluster.AutoSize = True
            rightCluster.AutoSizeMode = AutoSizeMode.GrowAndShrink
            rightCluster.Controls.Add(btnAutosize)
            rightCluster.Controls.Add(btnNormalize)
            rightCluster.Controls.Add(btnSave)
            rightCluster.Controls.Add(btnSaveAll)
            rightCluster.Dock = DockStyle.Left
            rightCluster.Location = New Point(0, 0)
            rightCluster.Name = "rightCluster"
            rightCluster.Padding = New Padding(4, 3, 4, 3)
            rightCluster.Size = New Size(158, 32)
            rightCluster.TabIndex = 2
            rightCluster.WrapContents = False
            ' 
            ' btnAutosize
            ' 
            btnAutosize.Location = New Point(6, 5)
            btnAutosize.Margin = New Padding(2)
            btnAutosize.Name = "btnAutosize"
            btnAutosize.Size = New Size(30, 24)
            btnAutosize.TabIndex = 0
            btnAutosize.Text = "↔"
            btnAutosize.UseVisualStyleBackColor = True
            ' 
            ' btnNormalize
            ' 
            btnNormalize.Location = New Point(40, 5)
            btnNormalize.Margin = New Padding(2)
            btnNormalize.Name = "btnNormalize"
            btnNormalize.Size = New Size(30, 24)
            btnNormalize.TabIndex = 1
            btnNormalize.Text = "✨"
            btnNormalize.UseVisualStyleBackColor = True
            ' 
            ' btnSave
            ' 
            btnSave.Location = New Point(84, 5)
            btnSave.Margin = New Padding(12, 2, 2, 2)
            btnSave.Name = "btnSave"
            btnSave.Size = New Size(30, 24)
            btnSave.TabIndex = 2
            btnSave.Text = "💾"
            btnSave.UseVisualStyleBackColor = True
            ' 
            ' btnSaveAll
            ' 
            btnSaveAll.Location = New Point(118, 5)
            btnSaveAll.Margin = New Padding(2)
            btnSaveAll.Name = "btnSaveAll"
            btnSaveAll.Size = New Size(34, 24)
            btnSaveAll.TabIndex = 3
            btnSaveAll.Text = "💾*"
            btnSaveAll.UseVisualStyleBackColor = True
            ' 
            ' autosaveTimer
            ' 
            autosaveTimer.Interval = 1000
            ' 
            ' MainForm
            ' 
            ClientSize = New Size(1280, 780)
            Controls.Add(split)
            Controls.Add(statusStrip)
            Controls.Add(menuStrip)
            KeyPreview = True
            MainMenuStrip = menuStrip
            MinimumSize = New Size(860, 560)
            Name = "MainForm"
            StartPosition = FormStartPosition.CenterScreen
            Text = "Altium CSV Librarian"
            menuStrip.ResumeLayout(False)
            menuStrip.PerformLayout()
            statusStrip.ResumeLayout(False)
            statusStrip.PerformLayout()
            split.Panel1.ResumeLayout(False)
            split.Panel2.ResumeLayout(False)
            CType(split, ComponentModel.ISupportInitialize).EndInit()
            split.ResumeLayout(False)
            grpLibraries.ResumeLayout(False)
            CType(grid, ComponentModel.ISupportInitialize).EndInit()
            emptyPanel.ResumeLayout(False)
            emptyPanel.PerformLayout()
            gridToolbar.ResumeLayout(False)
            gridToolbar.PerformLayout()
            backupCluster.ResumeLayout(False)
            backupCluster.PerformLayout()
            rightCluster.ResumeLayout(False)
            CType(bindingSrc, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()
        End Sub

        Friend WithEvents menuStrip As System.Windows.Forms.MenuStrip
        Friend WithEvents mnuFile As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuOpen As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuReload As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuNewLibrary As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuOpenWorkFolder As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepFileArchive As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuCreateArchive As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepFile1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuSave As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuSaveAll As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepFile2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuRecent As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuExit As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuEdit As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuAddRow As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuAddClone As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuCopyAbove As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepEdit1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuColumns As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuAutosize As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepEdit2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuNormalize As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepEdit3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuRemove As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuSettings As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuRules As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuWebCrawler As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuAdvancedClipboard As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuOptions As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepSettings1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuShowConfig As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepSettings2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuHelp As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents mnuHelpRef As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents sepHelp1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents mnuAbout As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents statusStrip As System.Windows.Forms.StatusStrip
        Friend WithEvents folderStatus As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents AdvancedClipboardContent As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents findStatus As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents replaceStatus As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents rowCountStatus As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents saveStatus As System.Windows.Forms.ToolStripStatusLabel
        Friend WithEvents split As System.Windows.Forms.SplitContainer
        Friend WithEvents grpLibraries As System.Windows.Forms.GroupBox
        Friend WithEvents fileList As System.Windows.Forms.ListBox
        Friend WithEvents gridToolbar As System.Windows.Forms.Panel
        Friend WithEvents rightCluster As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents btnAutosize As System.Windows.Forms.Button
        Friend WithEvents btnNormalize As System.Windows.Forms.Button
        Friend WithEvents btnSave As System.Windows.Forms.Button
        Friend WithEvents btnSaveAll As System.Windows.Forms.Button
        Friend WithEvents backupCluster As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents lblBackups As System.Windows.Forms.Label
        Friend WithEvents cboBackups As System.Windows.Forms.ComboBox
        Friend WithEvents btnRestore As System.Windows.Forms.Button
        Friend WithEvents divider As System.Windows.Forms.Panel
        Friend WithEvents grid As System.Windows.Forms.DataGridView
        Friend WithEvents emptyPanel As System.Windows.Forms.Panel
        Friend WithEvents lblEmptyIcon As System.Windows.Forms.Label
        Friend WithEvents lblEmpty1 As System.Windows.Forms.Label
        Friend WithEvents lblEmpty2 As System.Windows.Forms.Label
        Friend WithEvents bindingSrc As System.Windows.Forms.BindingSource
        Friend WithEvents autosaveTimer As System.Windows.Forms.Timer
        Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents PasteToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents sepEditDel As ToolStripSeparator
        Friend WithEvents mnuDeleteRow As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
        Friend WithEvents FindToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents FindNextToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents FindCellToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents FindInColumnToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
        Friend WithEvents ReplaceToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ReplaceNextToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ReplaceInSameColumnToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
        Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents mnu_tools_ConvertID As ToolStripMenuItem
        Friend WithEvents QuickWebLookupToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents AdvancedClipboardFunctionsToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents CopyToolStripMenuItem1 As ToolStripMenuItem
        Friend WithEvents Copy0ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy1ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy2ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy3ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy4ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy5ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy6ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy7ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy8ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Copy9ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents PasteToolStripMenuItem1 As ToolStripMenuItem
        Friend WithEvents Paste0ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste1ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste2ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste3ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste4ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste5ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste6ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste7ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste8ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents Paste9ToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents IncrementToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents DecrementToolStripMenuItem As ToolStripMenuItem
    End Class

End Namespace
