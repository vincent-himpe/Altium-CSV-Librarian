Namespace Global.CsvLibrarian

    ''' <summary>
    ''' The Help window content, in a tiny human-friendly markup that <c>HelpForm</c> parses.
    ''' This file is compiled into the program (it is NOT shipped as a separate data file),
    ''' so editing help is just editing this string.
    '''
    ''' FORMAT:
    '''   • Any text OUTSIDE curly braces is a SECTION HEADER (rendered in accent blue).
    '''   • Each <c>{action, description}</c> is a ROW: the part before the first comma is the
    '''     shortcut/action (bold), the rest is the description.
    '''   • "~" is the ONLY line break (so "~~" makes a blank line). Nothing else wraps
    '''     manually — long text word-wraps on its own.
    '''   • A field may START with a colour code that PERSISTS until the next code:
    '''     [R] red, [G] green, [B] blue, [W] white, [Y] yellow, [M] muted grey, [/] reset.
    '''     No code = default colour (blue header, light action, muted description).
    '''   • Do not use "{" or "}" inside a description; commas are fine (only the first splits).
    '''   • NOTE (VB): every line below must end with "&" — no blank lines inside the literal.
    ''' </summary>
    Friend Module HelpContent

        Public Const Text As String =
            "[G]Definitions" &
            "{[Y]Index column,[M] Contains Auto-generated GUID that serve as the index and are read-only. Altium uses this as the unique ID to find a part. This is not user-editable.}" &
            "{[Y]Working Folder, [M]A folder on disk containing libraries (individual CSV files).The working folder is shared through an ODBC connector (microoft ODBC Text Driver) as a database to Altium.}" &
            "{[Y]Library,[M] A CSV file containing parts of a same kind. These will show up as individual tables through the ODBC driver.}" &
            "[G]Basic Navigation Operations" &
            "{[Y]Up / Down Arrows,[M] Move one row up or down}" &
            "{[Y]Left / Right Arrows ·  Tab,[M] Move horizontally between cells}" &
            "{[Y]Enter, [M]Commit the edit and move down (not right)}" &
            "{[Y]Current row,[M] The row holding the active cell is highlighted in light blue background with the active cell in dark blue background}" &
            "[/]File Menu" &
            "{Ctrl + O, Open a working folder}" &
            "{F5, Reload the working folder}" &
            "{Open Working Folder, Open the working folder in Explorer (File menu)}" &
            "{Create Archive, Create a date/time stamped Zip archive of all CSVs in the working library.}" &
            "{Ctrl + Shift + L, Create a new library in the working folder}" &
            "{Ctrl + S, Save the active file. A backup is written first}" &
            "{Ctrl + Shift + S, Save all modified files}" &
            "{Recently Used, Reopen one of the last five folders (File menu)}" &
            "{Alt + F4, Exit. Prompts to save or discard unsaved changes}" &
            "Edit Menu" &
            "{Ctrl + C, Copy: Copies the current cell to BOTH the internal clipboard and the Windows clipboard}" &
            "{Ctrl + Shift + C, Copy to windows: Copies the current cell to the Windows clipboard only}" &
            "{Ctrl + V, Paste: Pastes clipboard data in current cell. Uses the internal clipboard, or the Windows clipboard if the internal one is empty}" &
            "{Ctrl + Shift + V, Paste From Windows Clipboard: Always paste from the Windows clipboard}" &
            "{Delete Row, (Trashcan on Toolbar) Delete the current row after a confirmation.No hotkey on purpose.}" &
            "{[Y]NOTE: ,[R] !!! The GUID is lost,This cannot be undone !!!}" &
            "{[/]Ctrl + F, Find: Prompts for the search string}" &
            "{Ctrl + Shift + F, Find Cell: Use the current cell as the search string}" &
            "{F3, Find Next: Find Next cell containing the search string}" &
            "{Shift + F3, Find Next in Column: Next match in the current column only}" &
            "{Ctrl + R, Replace: Replace current cell contents with the clipboard value}" &
            "{Ctrl + Shift + R, Replace Next: Next match becomes the clipboard value}" &
            "{Ctrl + Alt + R, Replace in Column: Next match in the current column becomes the clipboard value}" &
            "{Insert, Appends a blank row at the bottom of the file with a fresh GUID}" &
            "{Ctrl + Insert, Appends a row cloned from the last row with a fresh GUID}" &
            "{Ctrl + D, Copy Down (Safe). Copies from the cell above, only if it is empty, then moves down. In the Location column it will auto-increment the numeric suffix of the value above, so MARS177 becomes MARS178}" &
            "{Ctrl + Shift + D, Copy Down (Force). Copies the cell above verbatim (This will not increment location) and moves down}" &
            "{Manage Columns…, Add, rename, delete or reorder columns, and migrate data between columns}" &
            "{Ctrl + Q, AutoSize Columns: Autosize all columns to fit their content}" &
            "{Ctrl + U, Unsort: Restore the view to the CSV row order}" &
            "{Ctrl + N, Normalize: Clean up the data of the active file. This applies the rules set in the Normalizer settings}" &
            "Tools" &
            "{Convert ID,If a Column with header ID exists it will be converted to Index and GUIDS will be generated for every row. Used to migrate old data.}" &
            "{Ctrl + W, Quick Web Lookup. Search all QuickLook sites for the current row's part number}" &
            "{Ctrl + E, Export Row. Write the current row to a .EXPORT JSON file}" &
            "{Import Records…, Browse and import .EXPORT files}" &
            "{Location Analysis, Scans the Location column across all open files. Reports numbering gaps, duplicate locations with their files, and any entries containing a hyphen. Cells with several comma-separated locations are split for analysis. Offers a Print button for the missing list}" &
            "{Integrity Check, Checks for duplicate GUIDs across all open files. The toolbar icon turns green when the index is clean and red when duplicates are found}" &
            "{Location Report, Print Description, Manufacturer Part Number 1 and Location for the chosen files}" &
            "{Collated BOM, Merge the open files into collated.csv, de-duplicated by part number plus description}" &
            "{Ctrl + L, Find Location. Prompts for a string and reports which open files contain it in their Location column}" &
            "Settings" &
            "{Normalizer Rules…, Edit the normalizer.json rule sets}" &
            "{Webcrawler,Edit the definition of the websites used for data lookup}" &
            "{API Integration…, Store supplier API credentials}" &
            "{Advanced Clipboard…, Edit the 10x10 clipboard grid (Settings menu)}" &
            "{Options…, Autosave interval and toggle, auto-normalize on save, and archive on exit}" &
            "{Show Config Files, Open the settings and config folder}" &
            "Web Lookup" &
            "{Right-click a part number, Search the configured sites for a Manufacturer Part Number 1 cell}" &
            "Advanced Clipboard" &
            "{Alt + Down / Alt + Up, Increment or decrement the clipboard bank (0-9, rolls over). The status bar at the bottom shows the contents of the selected bank}" &
            "{Ctrl + 0…9, Copy the current cell into clipboard column 0-9 at the current clipboard bank}" &
            "{Alt + 0…9, Paste clipboard column 0-9 from the current bank into the current cell}" &
            "{(numeric keypad), The digit shortcuts also work on the numeric keypad}" &
            "Backups" &
            "{Backups dropdown, Pick Current for the live editable file, or a timestamp to preview that backup read-only. A preview shows a light, inverted, non-editable grid. Selecting a backup while the file is unsaved offers Save and Proceed}" &
            "{Restore, Pull the previewed backup into the working file as an unsaved change. Only Restore changes the file}" &
            "{Auto-save, Saves modified files a short time after typing stops. Toggle in Options}"

    End Module

End Namespace
