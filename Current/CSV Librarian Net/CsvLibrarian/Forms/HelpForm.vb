Imports System.Drawing
Imports System.Windows.Forms
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>Modal keyboard / feature reference.</summary>
    Public Class HelpForm

        Private Shared ReadOnly Sections As (Title As String, Items As (Key As String, Desc As String)())() = {
            ("Navigation", New (String, String)() {
                ("↑ / ↓", "Move one row up / down"),
                ("← / →  ·  Tab", "Move between cells"),
                ("Enter", "Commit and move down")
            }),
            ("Editing", New (String, String)() {
                ("Insert", "Append a blank row with a new GUID"),
                ("Ctrl + Insert", "Append a row cloned from the last row"),
                ("Ctrl + D", "Copy the value from the cell above"),
                ("Ctrl + C", "Copy the current cell to the app clipboard"),
                ("Ctrl + V", "Paste into the selected cell(s), same column"),
                ("Ctrl + Q", "Autosize all columns to fit content"),
                ("Ctrl + N", "Normalize the Description on the active file")
            }),
            ("Find & Replace", New (String, String)() {
                ("Ctrl + F", "Find — prompt for the search string"),
                ("Ctrl + Shift + F", "Find Cell — use the current cell as search string"),
                ("F3", "Find Next — next cell containing the search string"),
                ("Shift + F3", "Find Next in Column — next match in this column"),
                ("Ctrl + R", "Replace — current cell ← clipboard (remembers old text)"),
                ("Ctrl + Shift + R", "Replace Next — next match ← clipboard"),
                ("Ctrl + Alt + R", "Replace in Column — next match in this column")
            }),
            ("Files", New (String, String)() {
                ("Ctrl + S", "Save the active file (backup written first)"),
                ("Ctrl + Shift + S", "Save all modified files"),
                ("Ctrl + L", "Open the working folder in Explorer"),
                ("Ctrl + Shift + L", "Create a new library in the working folder"),
                ("Alt + F4", "Save all modified files, then quit"),
                ("Auto-save", "Saves all modified files ~30 s after typing stops")
            }),
            ("Normalizer & Notes", New (String, String)() {
                ("⚙ Rules…", "Edit normalizer.json rule sets"),
                ("GUID column", "First column — auto-generated, read-only"),
                ("Backups", "Last 5 versions kept in .backups/")
            })
        }

        Public Sub New()
            InitializeComponent()
            PopulateContent()
        End Sub

        Private Sub PopulateContent()
            For Each section In Sections
                ' Section headers keep their accent colour; everything else is
                ' standard (black keys, dark-grey descriptions).
                Dim head = UiFactory.MakeLabel(section.Title.ToUpperInvariant(), Palette.Accent, bold:=True, size:=8.0F)
                head.Margin = New Padding(0, 10, 0, 2)
                pnlContent.Controls.Add(head)
                For Each item In section.Items
                    pnlContent.Controls.Add(MakeRow(item.Key, item.Desc))
                Next
            Next
        End Sub

        Private Function MakeRow(key As String, desc As String) As Control
            Dim row As New Panel With {.Size = New Size(452, 26), .Margin = New Padding(0, 1, 0, 1)}
            Dim k As New Label With {
                .Text = key,
                .Size = New Size(150, 22),
                .Location = New Point(0, 2),
                .TextAlign = ContentAlignment.MiddleLeft,
                .ForeColor = Color.Black,
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
            }
            Dim d As New Label With {
                .Text = desc,
                .AutoSize = False,
                .Size = New Size(290, 22),
                .Location = New Point(158, 2),
                .TextAlign = ContentAlignment.MiddleLeft,
                .ForeColor = Color.DimGray,
                .Font = New Font("Segoe UI", 9.0F)
            }
            row.Controls.Add(k)
            row.Controls.Add(d)
            Return row
        End Function

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

    End Class

End Namespace
