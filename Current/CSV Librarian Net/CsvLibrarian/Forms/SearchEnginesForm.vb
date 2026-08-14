Imports System.Linq
Imports System.Windows.Forms
Imports CsvLibrarian.Models
Imports CsvLibrarian.Services

Namespace Forms

    ''' <summary>
    ''' WebCrawler configuration window: a table of search engines (Website, Query,
    ''' Enabled, QuickLook) persisted to "Altium CSV Librarian WebConfig.JSON". OK saves and
    ''' closes; Cancel closes without saving. Rows whose Website is blank are
    ''' discarded on save. Uses the standard Windows colour scheme (no theming).
    ''' </summary>
    Public Class SearchEnginesForm

        Public Sub New()
            InitializeComponent()
            LoadIntoGrid()
        End Sub

        ''' <summary>Populate the grid from the saved config.</summary>
        Private Sub LoadIntoGrid()
            Dim cfg = WebConfigStore.Load()
            engineGrid.Rows.Clear()
            For Each eng In cfg.Engines
                Dim idx = engineGrid.Rows.Add()
                Dim row = engineGrid.Rows(idx)
                row.Cells(colWebsite.Index).Value = If(eng.Website, "")
                row.Cells(colQuery.Index).Value = If(eng.Query, "")
                row.Cells(colEnabled.Index).Value = eng.Enabled
                row.Cells(colQuickLook.Index).Value = eng.QuickLook
            Next
        End Sub

        ''' <summary>Add a blank row, enabled by default, and focus its Website cell.</summary>
        Private Sub AddRow()
            engineGrid.EndEdit()
            Dim idx = engineGrid.Rows.Add()
            engineGrid.Rows(idx).Cells(colEnabled.Index).Value = True
            engineGrid.Rows(idx).Cells(colQuickLook.Index).Value = False
            engineGrid.CurrentCell = engineGrid.Rows(idx).Cells(colWebsite.Index)
        End Sub

        ''' <summary>Remove the selected row(s), or the current row if none selected.</summary>
        Private Sub RemoveSelectedRows()
            engineGrid.EndEdit()
            Dim rows = engineGrid.SelectedRows.Cast(Of DataGridViewRow)().
                Where(Function(r) Not r.IsNewRow).ToList()
            If rows.Count = 0 AndAlso engineGrid.CurrentRow IsNot Nothing AndAlso
               Not engineGrid.CurrentRow.IsNewRow Then
                rows.Add(engineGrid.CurrentRow)
            End If
            For Each r In rows
                engineGrid.Rows.Remove(r)
            Next
        End Sub

        ''' <summary>Build a config from the grid, discarding blank-Website rows.</summary>
        Private Function BuildConfig() As WebConfig
            Dim cfg As New WebConfig()
            For Each row As DataGridViewRow In engineGrid.Rows
                If row.IsNewRow Then Continue For
                Dim website = If(Convert.ToString(row.Cells(colWebsite.Index).Value), "").Trim()
                If website.Length = 0 Then Continue For        ' discard blank-website rows
                Dim query = If(Convert.ToString(row.Cells(colQuery.Index).Value), "")
                Dim rawEnabled = row.Cells(colEnabled.Index).Value
                Dim enabled As Boolean = TypeOf rawEnabled Is Boolean AndAlso CBool(rawEnabled)
                Dim rawQuick = row.Cells(colQuickLook.Index).Value
                Dim quickLook As Boolean = TypeOf rawQuick Is Boolean AndAlso CBool(rawQuick)
                cfg.Engines.Add(New SearchEngine With {
                    .Website = website, .Query = query, .Enabled = enabled, .QuickLook = quickLook})
            Next
            Return cfg
        End Function

        ' ── Event handlers ─────────────────────────────────────────────────────────

        Private Sub btnAddRow_Click(sender As Object, e As EventArgs) Handles btnAddRow.Click
            AddRow()
        End Sub

        Private Sub btnRemoveRow_Click(sender As Object, e As EventArgs) Handles btnRemoveRow.Click
            RemoveSelectedRows()
        End Sub

        Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
            engineGrid.EndEdit()
            Try
                WebConfigStore.Save(BuildConfig())
            Catch ex As Exception
                MessageBox.Show(Me, "Could not save the web config:" & vbCrLf & ex.Message,
                                "WebCrawler", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return   ' keep the window open so nothing is lost
            End Try
            Me.DialogResult = DialogResult.OK   ' closes the modal dialog
        End Sub

    End Class

End Namespace
