# Altium CSV Librarian .NET

A dark-themed, folder-based CSV editor for Altium component libraries, written in
**VB.NET / WinForms (.NET 8)**. It is a re-imagining of the original Python
`altium_csv_librarian` tool, sharing its core workflow but adding three new
capabilities.

## What it does

- Standard Windows chrome: a **menu bar** (File / Edit / Settings / Help), a
  **status bar** (folder path on the left, save status on the right), and a native
  list box for the file browser.
- **Open a folder** → every `*.csv` in it loads automatically into the sidebar.
- **Editable grid** bound to each file. The **first column is the GUID** column:
  auto-generated, read-only, greyed out. Blank GUID cells are back-filled on load.
- **Add rows** (blank, or cloned from the last row), edit inline, navigate with the
  keyboard.
- **Autosave** ~30 s after typing stops, and on close.
- **Rolling backups**: before each save the previous file is copied into `.backups/`,
  keeping the 5 most recent versions per file.

## New in the .NET version

1. **Editable columns** — `▤ Columns…` opens a manager to **add / rename / delete /
   reorder** columns. The GUID column is protected (rename-only). Existing cell data
   is preserved by column name when the table is rebuilt.
2. **Grid-toolbar icon buttons** — quick Autosize (`Ctrl+Q`), Normalize Description
   (`Ctrl+N`), Save (`Ctrl+S`), and Save All (`Ctrl+Shift+S`).
3. **Generic, rule-based normalizer** — the capacitor-specific "build Description"
   logic is now fully data-driven via **`normalizer.json`** (created in the folder on
   first open). Each rule set targets files by name or glob and defines:
   - per-column **transforms**: `upper`, `lower`, `trim`, `stripspaces`,
     `collapsespaces`, `replace:FROM=>TO` (applied in order), and
   - an optional **build** rule that composes a target column (e.g. `Description`)
     from other columns with a separator.

   Edit the file by hand, or use the in-app editor (`⚙ Rules…`). Run it with
   `✨ Normalize` or `Ctrl+T`.

## Menus

All shortcuts are shown next to their menu items.

- **File** — Open Folder (`Ctrl+O`), Reload Folder (`F5`), Open Working Folder
  (`Ctrl+L`, opens Explorer), New Library (`Ctrl+Shift+L`) — both enabled once a
  folder is open, — , Save (`Ctrl+S`), Save All (`Ctrl+Shift+S`), — ,
  Exit (`Alt+F4`, saves all first).
- **Edit** — Add Row (`Insert`), Add Cloned Row (`Ctrl+Insert`), Copy From Cell Above
  (`Ctrl+D`), — , Manage Columns…, Autosize Columns (`Ctrl+Q`), — , Normalize
  Description (`Ctrl+N`, enabled only when a rule matches the file), — , Remove File.
- **Settings** — Normalizer Rules…
- **Help** (right-aligned) — Keyboard & Feature Reference (`F1`).

## Keyboard shortcuts

| Key | Action |
|-----|--------|
| `Ctrl+O` | Open folder |
| `F5` | Reload folder |
| `Ctrl+L` | Open working folder in Explorer |
| `Ctrl+Shift+L` | New library (needs an open folder) |
| `Ctrl+S` | Save the active file |
| `Ctrl+Shift+S` | Save all modified files |
| `Alt+F4` | Exit (saves all first) |
| `Insert` | Append a blank row |
| `Ctrl+Insert` | Append row (cloned from last) |
| `Ctrl+D` | Copy value from the cell above |
| `Ctrl+N` | Normalize Description (active file) |
| `Ctrl+Q` | Autosize columns |
| `F1` | Help |

## Build & run

Requires the **.NET 8 Desktop Runtime** (already present on this machine) and, to
build, the **.NET SDK** (9.x installed).

```bash
dotnet build "CsvLibrarian/CsvLibrarian.vbproj"
```

Or open **`CsvLibrarian.sln`** in Visual Studio 2022 and press F5.

Run the built app, optionally passing a folder to open on startup:

```bash
CsvLibrarian.exe "C:\path\to\csv\folder"
```

### Self-contained single-file exe (no runtime needed)

To produce one portable `CsvLibrarian.exe` (~72 MB) with the .NET 8 runtime bundled
in — runs on any Windows x64 machine with nothing installed:

```bash
dotnet publish "CsvLibrarian/CsvLibrarian.vbproj" -p:PublishProfile=SelfContained-win-x64
```

Output: `CsvLibrarian/bin/Release/net8.0-windows/publish/win-x64/CsvLibrarian.exe`.
In Visual Studio 2022: right-click the project → **Publish** → the
**SelfContained-win-x64** profile.

The last-opened folder is remembered between sessions
(`%LocalAppData%\CsvLibrarian\settings.json`).

### Diagnostics

A headless self-test exercises read → GUID-inject → normalize → round-trip write and
writes a report, without opening the UI:

```bash
CsvLibrarian.exe --selftest "C:\path\to\csv\folder" report.txt
```

## Project layout

```
CsvLibrarian/
  Program.vb              Entry point (+ --selftest hook)
  Models/                 CsvDocument, normalizer rule models
  Services/               CSV IO, backups, GUID, normalizer engine, rule store, settings
  Forms/                  MainForm, ColumnsForm, RulesEditorForm, HelpForm
  Theme/                  Dark palette + themed-control factory
```

## Example `normalizer.json`

```json
{
  "version": 1,
  "ruleSets": [
    {
      "match": "capacitors.csv",
      "normalizations": [
        { "column": "Value",      "transforms": ["lower", "replace:f=>F", "stripspaces"] },
        { "column": "Voltage",    "transforms": ["upper", "stripspaces"] },
        { "column": "Dielectric", "transforms": ["upper", "stripspaces"] }
      ],
      "build": {
        "target": "Description",
        "sources": ["Library Ref", "Value", "Voltage", "Dielectric"],
        "separator": "-",
        "skipEmpty": true
      }
    }
  ]
}
```
