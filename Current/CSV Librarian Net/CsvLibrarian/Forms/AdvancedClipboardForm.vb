Imports System.Windows.Forms
Imports CsvLibrarian.Services

Namespace Forms

    ''' <summary>
    ''' Editor for the global <see cref="AppInfo.AdvancedClipboard"/> 10×10 grid.
    ''' The DataGridView is the working copy: it is filled from the global array on
    ''' open and never touches the global until Close. Close pushes the grid back
    ''' into the global array and saves the settings JSON; Cancel discards. The form
    ''' is application-modal, so no external code changes the array while it is open.
    ''' </summary>
    Public Class AdvancedClipboardForm

        Private ReadOnly _settings As AppSettings

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
            _settings = New AppSettings()
            BuildGrid()
            LoadFromGlobal()
        End Sub

        Public Sub New(settings As AppSettings)
            InitializeComponent()
            _settings = settings
            BuildGrid()
            LoadFromGlobal()
        End Sub

        ''' <summary>Create the fixed 10×10 grid (columns + rows), 1-based headers.</summary>
        Private Sub BuildGrid()
            clipGrid.Columns.Clear()
            For c As Integer = 0 To AppInfo.AdvancedClipboardCols - 1
                Dim col As New DataGridViewTextBoxColumn() With {
                    .HeaderText = c.ToString(),
                    .Name = "col" & c.ToString(),
                    .SortMode = DataGridViewColumnSortMode.NotSortable,
                    .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
                clipGrid.Columns.Add(col)
            Next
            clipGrid.Rows.Clear()
            clipGrid.Rows.Add(AppInfo.AdvancedClipboardRows)
            For r As Integer = 0 To AppInfo.AdvancedClipboardRows - 1
                clipGrid.Rows(r).HeaderCell.Value = r.ToString()
            Next
        End Sub

        ''' <summary>Fill the grid (working copy) from the global array.</summary>
        Private Sub LoadFromGlobal()
            For r As Integer = 0 To AppInfo.AdvancedClipboardRows - 1
                For c As Integer = 0 To AppInfo.AdvancedClipboardCols - 1
                    clipGrid.Rows(r).Cells(c).Value = If(AppInfo.AdvancedClipboard(r, c), "")
                Next
            Next
        End Sub

        ''' <summary>Push the grid (working copy) back into the global array.</summary>
        Private Sub CommitToGlobal()
            For r As Integer = 0 To AppInfo.AdvancedClipboardRows - 1
                For c As Integer = 0 To AppInfo.AdvancedClipboardCols - 1
                    AppInfo.AdvancedClipboard(r, c) =
                        If(Convert.ToString(clipGrid.Rows(r).Cells(c).Value), "")
                Next
            Next
        End Sub

        Private Sub closeBtn_Click(sender As Object, e As EventArgs) Handles closeBtn.Click
            clipGrid.EndEdit()
            CommitToGlobal()
            Try
                ' Same persist operation the program-exit code uses.
                _settings.AdvancedClipboard = AppInfo.AdvancedClipboardToJagged()
                _settings.Save()
            Catch ex As Exception
                MessageBox.Show(Me, "Could not save the advanced clipboard:" & vbCrLf & ex.Message,
                                "Advanced Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return   ' keep the window open so nothing is lost
            End Try
            Me.DialogResult = DialogResult.OK
        End Sub

    End Class

End Namespace
