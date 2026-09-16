Imports System.Drawing
Imports System.Windows.Forms
Imports CsvLibrarian
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>
    ''' Lets the user pick which open files go into the Location Report. A checked list
    ''' box (all files checked by default) with Check All / Check None, plus Print and
    ''' Cancel. The selection is transient — nothing is written back to the JSON files.
    ''' Dark themed to match the rest of the app.
    ''' </summary>
    Public Class PrintSelectForm

        Private ReadOnly _files As IReadOnlyList(Of LocationReportFile)

        Public Sub New(files As IReadOnlyList(Of LocationReportFile))
            InitializeComponent()
            _files = files
            StyleDark()
            For Each f In _files
                fileList.Items.Add(f.FileName, True)   ' start with everything checked
            Next
        End Sub

        ''' <summary>The files the user left checked, in the original order.</summary>
        Public ReadOnly Property SelectedFiles As List(Of LocationReportFile)
            Get
                Dim result As New List(Of LocationReportFile)()
                For i As Integer = 0 To _files.Count - 1
                    If fileList.GetItemChecked(i) Then result.Add(_files(i))
                Next
                Return result
            End Get
        End Property

        ''' <summary>Flat dark buttons to match the scheme (list items are owner-drawn).</summary>
        Private Sub StyleDark()
            DarkTheme.StyleFlatButtons(btnCheckAll, btnCheckNone, btnPrint, btnCancel)
        End Sub

        ''' <summary>Owner-draw each row: dark background, blue selection, a classic
        ''' checkbox glyph, and light (or white-when-selected) text.</summary>
        Private Sub fileList_DrawItem(sender As Object, e As DrawItemEventArgs) Handles fileList.DrawItem
            If e.Index < 0 Then Return
            Dim selected = (e.State And DrawItemState.Selected) = DrawItemState.Selected
            Dim backColor = If(selected, Color.FromArgb(61, 92, 135), Color.FromArgb(37, 37, 41))
            Using bg As New SolidBrush(backColor)
                e.Graphics.FillRectangle(bg, e.Bounds)
            End Using

            Dim glyph As Integer = 14
            Dim gy As Integer = e.Bounds.Top + (e.Bounds.Height - glyph) \ 2
            ControlPaint.DrawCheckBox(e.Graphics, e.Bounds.Left + 3, gy, glyph, glyph,
                If(fileList.GetItemChecked(e.Index), ButtonState.Checked, ButtonState.Normal))

            Dim txt = Convert.ToString(fileList.Items(e.Index))
            Dim fg = If(selected, Color.White, Color.FromArgb(220, 220, 220))
            Dim textRect As New Rectangle(e.Bounds.Left + glyph + 8, e.Bounds.Top,
                                          e.Bounds.Width - glyph - 10, e.Bounds.Height)
            TextRenderer.DrawText(e.Graphics, txt, fileList.Font, textRect, fg,
                TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        End Sub

        Private Sub btnCheckAll_Click(sender As Object, e As EventArgs) Handles btnCheckAll.Click
            SetAll(True)
        End Sub

        Private Sub btnCheckNone_Click(sender As Object, e As EventArgs) Handles btnCheckNone.Click
            SetAll(False)
        End Sub

        Private Sub SetAll(state As Boolean)
            For i As Integer = 0 To fileList.Items.Count - 1
                fileList.SetItemChecked(i, state)
            Next
        End Sub

    End Class

End Namespace
