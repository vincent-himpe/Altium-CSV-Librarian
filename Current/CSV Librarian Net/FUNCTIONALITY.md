# CSV Librarian — Functionality Reference

## Main Window

### File menu
- **Open Folder… (Ctrl+O)** — Choose a working folder; loads every `.csv` in it, back-fills missing GUIDs, and adds the folder to the Recently Used list.
- **Reload Folder (F5)** — Re-reads the current working folder from disk, discarding the in-memory view (unsaved edits are saved first).
- **Open Working Folder** — Opens the current working folder in Windows Explorer. (Ctrl+L now belongs to Tools ▸ Find Location.)
- **New Library… (Ctrl+Shift+L)** — Creates a new empty CSV in the working folder using the mandatory library headers.
- **Create Archive** — Zips every CSV in the working folder into `<folder>-M-d-yyyy-H-mm.zip` (saves dirty files first).
- **Save (Ctrl+S)** — Backs up and writes the active file to disk.
- **Save All (Ctrl+Shift+S)** — Saves every file with unsaved changes.
- **Recently Used ▸** — Submenu of up to 5 recently opened folders; selecting one opens it.
- **Exit** — Closes the app; if there are unsaved changes it prompts Save / Discard / Cancel, and archives the folder first when "Archive on exit" is enabled.

### Edit menu
- **Copy (Ctrl+C)** — Copies the selected cell's text to **both** the app's internal clipboard and the Windows clipboard.
- **Copy to Windows only (Ctrl+Shift+C)** — Copies the current cell to the Windows clipboard only; the internal clipboard is left unchanged.
- **Paste (Ctrl+V)** — Pastes the internal clipboard into the selected cells of the current column; if the internal clipboard is empty it falls back to the Windows clipboard. The GUID/Index column is never written.
- **Paste from Windows (Ctrl+Shift+V)** — Always pastes from the Windows clipboard, ignoring the internal clipboard.
- **Delete Row** — Deletes the current row after an OK/Cancel prompt warning the action cannot be undone. No hotkey (also on the grid toolbar).
- **Find (Ctrl+F)** — Prompts for a search string and stores it.
- **Find Cell (Ctrl+Shift+F)** — Sets the search string from the current cell's contents.
- **Find Next (F3)** — Selects the next cell containing the search string (wraps).
- **Find Next in Column (Shift+F3)** — As Find Next, restricted to the current column.
- **Replace (Ctrl+R)** — Seeds the search string from the current cell, then overwrites that cell with the clipboard value.
- **Replace Next (Ctrl+Shift+R)** — Finds the next match of the search string and substitutes the clipboard value into it.
- **Replace In Same Column (Ctrl+Alt+R)** — As Replace Next, restricted to the current column.
- **Add Row (Insert)** — Appends a blank row with a freshly generated GUID.
- **Add Cloned Row (Ctrl+Insert)** — Appends a row copying the previous row's values (with a new GUID).
- **Copy Down (Safe) (Ctrl+D)** — Copies the cell above into the current cell only when the cell above has data and the current cell is empty; then moves down if the next cell is also empty. In the **Location** column it instead increments the numeric suffix of the value above (e.g. `MARS177` → `MARS178`, preserving the digit width); Ctrl+Shift+D is unaffected.
- **Copy Down (Force) (Ctrl+Shift+D)** — Copies the cell above into the current cell unconditionally (overwriting), then moves the selection down one row.
- **Manage Columns…** — Opens the Manage Columns window.
- **Autosize Columns (Ctrl+Q)** — Fits each column's width to its content.
- **Unsort (Ctrl+U)** — Clears any column sort so rows return to their original CSV order.
- **Normalize Fields (Ctrl+N)** — Applies the normalizer rule set that matches the active file.

### Tools menu
- **Convert ID** — If the table has an `ID` column and no `Index` column, fills `ID` with fresh GUIDs, renames it to `Index`, and moves it to the front.
- **Quick Web Lookup (Ctrl+W)** — Opens the default browser for every search engine whose *QuickLook* flag is set, using the current row's "Manufacturer Part Number 1" value.
- **Export Row (Ctrl+E)** — Exports the current row to a `.EXPORT` JSON file (all columns incl. GUID) named from the part number (uppercased and sanitized).
- **Import Records…** — Opens the non-modal Import Records window.
- **Location Analysis** — Scans the `Location` column across *all* open files. Each value is a text prefix followed by digits (e.g. `A0012`, `MARS10`); leading zeros are ignored (`Q0012` = `Q12`). Values are grouped by prefix and the missing numbers *between* each prefix's lowest and highest used value are reported (e.g. `A0010, A0011, A0013` → `A0012`). Reconstructed entries inherit the zero-padding of the value just below the gap. The comma-separated list of missing locations is shown in a popup and copied to the clipboard. A `Location` cell may hold several comma-separated locations — each is analyzed separately (the source table is never modified). Any location containing a hyphen (`-`) is flagged (with its file name) as malformed and excluded from the gap check so it can be corrected. **Duplicate locations** (the same location used more than once) are also reported with their files, like the Integrity Check. When there are gaps, the dialog offers a **Print** button that produces a "Missing Location Report" — a striped single-column printout (missing location in the left 1/5 of the page, the rest blank for handwritten notes) with the black header/footer bands. No shortcut.
- **Integrity Check** — Collects the GUID from the index column (first column) of *all* open files and checks for duplicates across every file. Blank GUID cells are ignored. If any GUID appears in more than one row/file, a dark dialog lists each duplicate GUID with the file(s) it's in, and the clipboard gets one `GUID : filename` line per occurrence (CR/LF separated). If all GUIDs are unique, a dark dialog shows "Index Clean". The result is also shown on the toolbar Integrity Check icon: **green glyph when clean, white-on-red when duplicates were found**; the icon reverts to the neutral light-blue whenever the data or the set of open files changes (an edit, restore, or folder load). No shortcut.
- **Location Report** — Builds a printable report of `Description` / `Manufacturer Part Number 1` / `Location`, sorted by Description, each file starting on a new page. First a **file-selection dialog** (`PrintSelectForm`) lets you pick which open files to include — a checkbox list (all checked by default) with **Check All** / **Check None** and **Print** / **Cancel**; the selection is transient (never written to JSON). Columns are laid out ½ / ¼ / ¼ of the printable width on Letter (8.5×11). Rows alternate white / ~10%-grey backgrounds with plain black text. Header = filename (left) + print date-time (right); footer = "Altium CSV Librarian &lt;version&gt;" (left) + "Page x of y" (right), both **bold white-on-black at 8 pt**; data (and the bold column titles) is **7 pt**. Margins are tight (0.60″ left/right, 0.40″ top/bottom). Prints via the standard Windows print dialog (printer is user-selectable). While printing, the right status field shows "Printing in progress..." then "Printing done". All print code lives in `ReportGenerator.vb`. No shortcut.
- **Collated BOM** — Reads every CSV in the working folder from disk (skipping `collated.csv` itself), keeping only `Description` / `Manufacturer 1` / `Manufacturer Part Number 1`. Rows are de-duplicated where `Manufacturer Part Number 1` repeats **and** the `Description` matches (first occurrence kept); rows blank in all three columns are dropped. The result is sorted by Description and written to `collated.csv` in the working folder (overwriting), with the column-name header row on top. A source file missing one of the columns contributes blanks for it. The new file appears in the file list after the folder is reopened. No shortcut.
- **Find Location (Ctrl+L)** — Prompts (via the themed `InputDialog`, with a light-blue search glyph) for a string, then reports which *open* files contain it in their `Location` column (case-insensitive substring), with a per-file match count. It only reports the file names — it doesn't open anything.

### Settings menu
- **Normalizer Rules…** — Opens the normalizer rule-set editor.
- **WebCrawler…** — Opens the search-engine list editor.
- **API Integration…** — Opens the Supplier API Integration window.
- **Advanced Clipboard…** — Opens the 10×10 Advanced Clipboard editor.
- **Options…** — Opens the application options window.
- **Show Config Files** — Opens the folder holding the settings/config JSON files in Explorer.
- **AdvancedClipboardFunctions ▸**
  - **Copy 0–9 (Ctrl+0…9)** — Copies the current cell into the Advanced Clipboard at the active index row, column 0–9.
  - **Paste 0–9 (Alt+0…9)** — Pastes the Advanced Clipboard value (active index row, column 0–9) into the current cell.
  - **Increment (Alt+Down)** — Advances the Advanced Clipboard index (rolls 9→0).
  - **Decrement (Alt+Up)** — Steps the Advanced Clipboard index back (rolls 0→9).

### Help menu
- **Keyboard & Feature Reference (F1)** — Opens the help window. Its content lives in `HelpText.vb` as a tiny compiled-in markup: bare text = section header; `{action, description}` = a row; `~` is the only line break (`~~` = blank line); a field may start with a persistent colour code (`[R]`/`[G]`/`[B]`/`[W]`/`[Y]`/`[M]` muted, `[/]` reset). Edit that file to change the help.
- **About…** — Shows version and credits.

### Grid toolbar buttons
The toolbar uses Segoe Fluent Icons glyphs (falling back to Segoe MDL2 Assets, then Segoe UI Symbol). Icons are light blue except Save All (green) and Delete Row (red).
- **Autosize** — Same as Autosize Columns.
- **Normalize** — Same as Normalize Fields.
- **Save** — Saves the active file.
- **Save All** — Saves all dirty files.
- **Unsort** — Same as Unsort.
- *(divider)* **Location Analysis** — Same as Tools ▸ Location Analysis.
- **Integrity Check** — Same as Tools ▸ Integrity Check.
- *(divider)* **Location Report** — Same as Tools ▸ Location Report.
- *(divider)* **Create Archive** — Same as File ▸ Create Archive.
- **Delete Row** — Sits at the right, just before the Backups group (with a divider). Same as the Edit ▸ Delete Row command; deletes the current row after an OK/Cancel "cannot be undone" confirmation.
- **Backups dropdown** — A **Current** entry (the live editable file), a divider, then the timestamped backups (newest first). Selecting a backup **previews** it read-only: the grid switches to an inverted light scheme (black text on 95%/90%-white alternating rows) and editing is disabled (scrolling and arrow-key navigation still work) so it's obvious you're looking at history. Selecting **Current** returns to the live, editable, dark grid. If the file has unsaved edits when you first open a backup, a dark **Save and Proceed / Cancel** dialog appears — Save and Proceed saves the file then shows the backup; Cancel stays on Current. Previewing never alters the file.
- **Restore** — Enabled only while a backup is being previewed. Pulls that backup into the working file as an unsaved change (does not autosave); confirmed via a dark Restore/Cancel dialog. Only Restore actually changes the file — selecting in the dropdown is preview only.

### Grid interactions
- **Current-row highlight** — The row containing the selected cell is tinted light cyan; the selected cell keeps the standard selection colors.
- **Right-click on a "Manufacturer Part Number 1" cell** — Shows a menu of the configured search engines (plus an "All"); clicking one opens the browser with that engine's query and the cell value.
- **Esc** — Clears the current find string and the internal clipboard.
- **Status bar** — Shows the working folder, the Advanced Clipboard row contents, the find string, the paste/clipboard value, the row count, and the save status.

## Manage Columns
- **Column list** — Shows all columns; mandatory library fields and the GUID column are locked.
- **Add** — Adds a new column.
- **Rename** — Renames the selected (non-locked) column.
- **Delete** — Deletes the selected column and its data.
- **Move Up / Move Down** — Reorders the selected column within the movable region.
- **Migrate group** — Moves data from the selected (source) column into a prompted target column using the chosen mode: **Merge** (fill only empty target cells), **Replace** (overwrite target), or **Combine** (append to target, comma-separated).
- **OK / Cancel** — Applies the column changes / discards them.

## Normalizer Rules
- **Rule-set list** — Lists all rule sets; only the rule set matching the active file is selectable (others are greyed and locked).
- **Add / Delete** — Creates a new rule set (matching the active file) / removes the selected one.
- **Match** — The file name or glob the rule set applies to.
- **Transforms grid** — Per-column transform tokens (e.g. upper, lower, trim, stripspaces, collapsespaces, replace:FROM=>TO).
- **Build-a-column controls** — Optionally compose a target column from source columns/literals with a separator and a skip-empty option.
- **OK / Cancel** — Saves the rules to `normalizer.json` / discards.

## WebCrawler (Search Engines)
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

## Supplier API Integration
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

## Location Report — Select Files (modal)
- **File list** — Checkbox list of every open file that has at least one of the report columns; all checked by default. Click anywhere on a row to toggle it.
- **Check All / Check None** — Toggle every file on or off.
- **Print** — Proceeds to the Windows print dialog for the checked files.
- **Cancel** — Aborts without printing. Selection is transient (not saved to JSON).
