# Altium CSV Librarian

While Altium supports real databases like Firebird, Oracle, MySQL Access and others, or even Excel workbooks, through the ODBC interface, those systems bring overhead : you need a network accessible server, it is hard to manipulate and you often need to wear the IT hat or the SQL wizard hat to set it up and maintain it.
This is an alternative that does not require such overhead.

CSV Files are simple:
- Human readable. Not binary blobs.
- You can throw them in GitHub or any other version control system that has push/pull mechanism.
- Editable with a text editor if needed.
- CSV is a standardized format.

Using Microsoft's ODBC Text driver (standard included in any windows installation) you can create a virtual database from CSV files stored in a given folder. This virtual database is accessible through the ODBC driver and Altium sees this as a valid datasource.

Even though CSV files can be edited with something as simple as notepad, there are still some issues you can encounter.
- CSV uses a comma to delineate fields. Some part numbers contain commas. CSV has a mechanism to encapsulate strings in "" pairs, and can even escape such sequences.
- Adding or removing a column can be annoying. if a field is removed from a row all the data "shifts" and keeping stuff in the right column becomes difficult.
- Maintaining an indexing system is the main issue. Altium must have a uniquely identifiable key to be able to pull new data from already instantiated parts.

That's where this tool comes in the picture.

The Altium CSV Librarian allows you to quickly manipulate a set of of CSV Files. 
- allows you to create new tables and automatically adds the correctly named columns mandatory to link symbols and footprints.
- maintains an indexing system by assigning a GUID for every component.
- Automatically injects GUID when importing data or converting data. It also protects the GUID from inadvertent editing by the user.
- Add/ remove and reorder columns.
- powerful task-oriented editing capabilities.
- formatting and filtering rule engine that can cleanup data and build descriptor strings based on expressions.
- part migration tools : move a part from one table to another with automatic field mapping and GUID preservation.
- advanced clipboard that allows to store up to 100 snippets of often used strings
- ability to work on multiple datasets, each with their own rules.
- auto backup of files on save with history tracking and auto-removal of stale files (round robin mechanism) 
- Timed auto-save. After a set idle time all dirty files are saved
- Archive creation of the dataset as a time/date stamped ZIP file.
- part lookup through web browsers. you can specify what search engines, the url and more. shortcut keys allow you to fast look up data for a given part.
- many more tools

# CSV Librarian — Functionality Reference

## Main Window

### File menu
- **Open Folder… (Ctrl+O)** — Choose a working folder; loads every `.csv` in it, back-fills missing GUIDs, and adds the folder to the Recently Used list.
- **Reload Folder (F5)** — Re-reads the current working folder from disk, discarding the in-memory view (unsaved edits are saved first). To be used if outside tools have added files.
- **Open Working Folder (Ctrl+L)** — Opens the current working folder in Windows Explorer.
- **New Library… (Ctrl+Shift+L)** — Creates a new empty CSV in the working folder and sets up the mandatory library headers for Altium.
- **Create Archive** — Zips every CSV in the working folder into `<folder>-M-d-yyyy-H-mm.zip` (saves dirty files first).
- **Save (Ctrl+S)** — Backs up and writes the active file to disk. Certain actions can be taken on save like data cleanup and normalisation.
- **Save All (Ctrl+Shift+S)** — Saves every file with unsaved changes.
- **Recently Used ▸** — Submenu of up to 5 recently opened folders; selecting one opens it. Allows for rapid switching between libraries.
- **Exit** — Closes the app; if there are unsaved changes it prompts Save / Discard / Cancel, and archives the folder first when "Archive on exit" is enabled.

### Edit menu
- **Copy (Ctrl+C)** — Copies the selected cell's text to the app's internal clipboard (not the Windows clipboard).
- **Paste (Ctrl+V)** — Pastes the internal clipboard into the selected cells of the current column; if the internal clipboard is empty it falls back to the Windows clipboard. The GUID/Index column is never written.
- **Delete Row** — Deletes the current row after a confirmation prompt. Use sparingly !
- **Find (Ctrl+F)** — Prompts for a search string and stores it.
- **Find Cell (Ctrl+Shift+F)** — Sets the search string from the current cell's contents.
- **Find Next (F3)** — Selects the next cell containing the search string (wraps).
- **Find Next in Column (Shift+F3)** — As Find Next, restricted to the current column.
- **Replace (Ctrl+R)** — Seeds the search string from the current cell, then overwrites that cell with the clipboard value.
- **Replace Next (Ctrl+Shift+R)** — Finds the next match of the search string and substitutes the clipboard value into it.
- **Replace In Same Column (Ctrl+Alt+R)** — As Replace Next, restricted to the current column.
- **Add Row (Insert)** — Appends a blank row with a freshly generated GUID.
- **Add Cloned Row (Ctrl+Insert)** — Appends a row copying the previous row's values (with a new GUID).
- **Copy Down (Safe) (Ctrl+D)** — Copies the cell above into the current cell only when the cell above has data and the current cell is empty; then moves down if the next cell is also empty.
- **Copy Down (Force) (Ctrl+Shift+D)** — Copies the cell above into the current cell unconditionally (overwriting), then moves the selection down one row.
- **Manage Columns…** — Opens the Manage Columns window.
- **Autosize Columns (Ctrl+Q)** — Fits each column's width to its content.
- **Unsort (Ctrl+U)** — Clears any user column sort so rows return to their original CSV order.
- **Normalize Fields (Ctrl+N)** — Applies the normalizer rule set that matches the active file.

### Tools menu
- **Convert ID** — If the table has an `ID` column and no `Index` column, fills `ID` with fresh GUIDs, renames it to `Index`, and moves it to the front. Used to build the initial index.
- **Quick Web Lookup (Ctrl+W)** — Opens the default browser for every search engine whose *QuickLook* flag is set, using the current row's "Manufacturer Part Number 1" value.
- **Export Row (Ctrl+E)** — Exports the current row to a `.EXPORT` JSON file (all columns incl. GUID) named from the part number (uppercased and sanitized). Used to move parts between tables.
- **Import Records…** — Opens the non-modal Import Records window to bring in data from `.EXPORT` files. Used to move parts between tables.

### Settings menu
- **Normalizer Rules…** — Opens the normalizer rule-set editor.
- **WebCrawler…** — Opens the search-engine list editor.
- **API Integration…** — Opens the Supplier API Integration window. (work in progress)
- **Advanced Clipboard…** — Opens the 10×10 Advanced Clipboard editor.
- **Options…** — Opens the application options window.
- **Show Config Files** — Opens the folder holding the settings/config JSON files in Explorer. Useful to move your settings to a different machine or share them with someone.
- **AdvancedClipboardFunctions ▸**
  The Advanced clipboard is a circular buffer of 10 sets of 10 storage spaces. The status bar shows the contents of the currently selected buffer. This allows you to store up to 10 often used strings like manufacturer names.
  The clipboard is persistent between program launches. contents are saved and loaded on program exit and start so data is preserved between sessions.
  - **Copy 0–9 (Ctrl+0…9)** — Copies the current cell into the Advanced Clipboard at the active index row, column 0–9.
  - **Paste 0–9 (Alt+0…9)** — Pastes the Advanced Clipboard value (active index row, column 0–9) into the current cell.
  - **Increment (Alt+Down)** — Advances the Advanced Clipboard index (rolls 9→0).
  - **Decrement (Alt+Up)** — Steps the Advanced Clipboard index back (rolls 0→9).

### Help menu
- **Keyboard & Feature Reference (F1)** — Opens the help window.
- **About…** — Shows version and credits.

### Grid toolbar buttons
- **↔ (Autosize)** — Same as Autosize Columns.
- **✨ (Normalize)** — Same as Normalize Fields.
- **💾 (Save)** — Saves the active file.
- **💾\* (Save All)** — Saves all dirty files.
- **U (Unsort)** — Same as Unsort.
- **Backups dropdown** — Lists the timestamped backups of the current file, newest first.
- **Restore** — Loads the selected backup into the grid as an unsaved change (does not autosave).

### Grid interactions
- **Current-row highlight** — The row containing the selected cell is tinted light cyan; the selected cell keeps the standard selection colors.
- **Right-click on a "Manufacturer Part Number 1" cell** — Shows a menu of the configured search engines (plus an "All"); clicking one opens the browser with that engine's query and the cell value.
- **Esc** — Clears the current find string and the internal clipboard.
- **Status bar** — Shows the working folder, the Advanced Clipboard row contents, the find string, the paste/clipboard value, the row count, and the last command status.

## Manage Columns
- **Column list** — Shows all columns. The mandatory library fields and the GUID column are locked and cannot be moved or removed.
- **Add** — Adds a new column.
- **Rename** — Renames the selected (non-locked) column.
- **Delete** — Deletes the selected column and its data.
- **Move Up / Move Down** — Reorders the selected column within the movable region.
- **Migrate group** — Moves data from the selected (source) column into a prompted target column using the chosen mode: 
  - **Merge** (fill only empty target cells)
  - **Replace** (overwrite target)
  - or **Combine** (append to target, comma-separated).
- **OK / Cancel** — Applies the column changes / discards them.

## Normalizer Rules
The Normalizer is a very powerful tool that lets you define data sanitation and formatting per column. You can change capitalization, remove unwanted spaces, or replace characters.
The normalizer can also builds a field based of data in a given row. For example : in the resistors table, build a Description field consisting of the word Resistor, followed by value, tolerance, wattage and package from the data table.
This functionality allows you to build consistent description strings directly form table data. Normalization can be auto-triggered on file save, so you description field is always clean.
- **Rule-set list** — Lists all rule sets; only the rule set matching the active file is selectable (others are greyed and locked).
- **Add / Delete** — Creates a new rule set (matching the active file) / removes the selected one.
- **Match** — The file name or glob the rule set applies to.
- **Transforms grid** — Per-column transform tokens (e.g. upper, lower, trim, stripspaces, collapsespaces, replace:FROM=>TO).
- **Build-a-column controls** — Optionally compose a target column from source columns/literals with a separator and a skip-empty option.
- **OK / Cancel** — Saves the rules to `normalizer.json` / discards.

## WebCrawler (Search Engines)
The webcrawler can launch search queries to your favorite suppliers. Simply look up the search command for their webpage and crawler does the rest. You can launch one or all.
- **Grid** — Columns Website, Query, Enabled, QuickLook, one row per engine.
- **Add Row** — Adds a new engine row (Enabled on by default).
- **Remove Row** — Deletes the selected row(s).
- **OK / Cancel** — Saves the list (rows with a blank Website are dropped) / discards.

## Options
- **Autosave seconds** — Interval for the autosave countdown.
- **Autosave Timer Enabled** — Turns the autosave countdown on or off.
- **Auto Normalize on Save** — Runs the matching normalizer rule on each file when it is saved.
- **Archive on exit** — Creates a zip archive of the working folder when the app exits.
- **OK / Cancel** — Saves the settings / discards.

## Advanced Clipboard
- **10×10 grid** — Editable grid of clipboard slots (rows and columns labeled 0–9); the working copy for the global Advanced Clipboard.
- **Close** — Writes the grid back to the global clipboard and saves it to the settings file.
- **Cancel** — Discards changes.

## Supplier API Integration (work in progress, not accessible yet)
- **Digikey** — Client ID and Access Token fields.
- **Mouser** — API Key field.
- **LCSC** — API Key and API Secret fields.
- **TME** — API Token field.
- **OK / Cancel** — Saves the credentials to the settings file / discards.

## Import Records (non-modal)
- **File list** — Every `.EXPORT` file in the working folder (extension hidden); already-imported files show a grey background.
- **Field / Data grid** — Read-only view of the selected file's key/value pairs; clicking a Data cell copies its value to the internal clipboard (the GUID/Index value is excluded).
- **Import** — Adds a new row to the main grid, copying every matching column (including the GUID — no new GUID is generated), highlights the matched cells green, and marks the file as imported (an imported file is shown 25% grey with Import disabled).
- **Delete** — Removes the selected `.EXPORT` file from disk (with confirmation).
- **Close** — Closes the window.
