Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Linq
Imports System.Windows.Forms
Imports CsvLibrarian.Forms

Namespace Global.CsvLibrarian

    ''' <summary>
    ''' One file's worth of data for the Location Report. <see cref="Rows"/> is a list
    ''' of 3-element string arrays in the order {Description, Manufacturer Part Number 1,
    ''' Location}. The caller (MainForm) fills these; the generator sorts and paginates.
    ''' </summary>
    Public Class LocationReportFile
        Public Property FileName As String
        Public Property Rows As List(Of String())
    End Class

    ''' <summary>
    ''' Builds and prints the "Location Report": Description / Manufacturer Part Number 1 /
    ''' Location for every open file, sorted by Description, each file starting on a fresh
    ''' page. Columns are laid out ½ / ¼ / ¼ of the printable width on Letter paper. Rows
    ''' alternate white / 10%-grey backgrounds with plain black text. The header (filename
    ''' left, print date/time right) and footer ("Altium CSV Librarian &lt;ver&gt;" left,
    ''' "Page x of y" right) are white text on black. Printing goes through the standard
    ''' Windows print dialog so the user picks the target printer.
    ''' </summary>
    Public Module ReportGenerator

        ''' <summary>
        ''' Entry point called from the Tools ▸ Location Report menu.
        ''' <paramref name="statusCallback"/>, if supplied, is invoked with a short status
        ''' string when printing starts ("Printing in progress...") and finishes
        ''' ("Printing done") — the caller routes it to the status bar.
        ''' </summary>
        Public Sub GenerateLocationReport(owner As IWin32Window,
                                          files As IReadOnlyList(Of LocationReportFile),
                                          Optional statusCallback As Action(Of String) = Nothing)
            If files Is Nothing OrElse files.Count = 0 Then
                ConfirmDialog.Notify(owner, "Location Report", "Open a folder with CSV files first.",
                                     icon:=DialogIcon.Warning)
                Return
            End If
            If files.Sum(Function(f) If(f.Rows Is Nothing, 0, f.Rows.Count)) = 0 Then
                ConfirmDialog.Notify(owner, "Location Report",
                    "None of the selected files have any Description / Manufacturer Part Number 1 / Location data to report.",
                    icon:=DialogIcon.Warning)
                Return
            End If
            Dim printer As New LocationReportPrinter(files, statusCallback)
            printer.Run(owner)
        End Sub

        ''' <summary>
        ''' Print a single-column "Missing Location Report": each missing location on its own
        ''' line, occupying the left 1/5 of the page width (the rest left blank for handwritten
        ''' notes), striped like the Location Report, with a black header/footer band.
        ''' </summary>
        Public Sub GenerateMissingLocationReport(owner As IWin32Window,
                                                 missing As IReadOnlyList(Of String),
                                                 Optional statusCallback As Action(Of String) = Nothing)
            If missing Is Nothing OrElse missing.Count = 0 Then
                ConfirmDialog.Notify(owner, "Missing Location Report", "There are no missing locations to print.")
                Return
            End If
            Dim printer As New MissingLocationPrinter(missing, statusCallback)
            printer.Run(owner)
        End Sub

        ' ── Internal print engine ────────────────────────────────────────────────
        Private NotInheritable Class LocationReportPrinter

            ' Layout constants, in hundredths of an inch (the printer graphics page unit).
            Private Const Gap As Single = 6.0F
            Private Const CellPadX As Single = 6.0F
            Private Const RowPadY As Single = 8.0F

            Private ReadOnly _files As IReadOnlyList(Of LocationReportFile)
            Private ReadOnly _status As Action(Of String)
            Private ReadOnly _printedAt As DateTime = DateTime.Now

            ' Geometry (set once the printer/paper is known).
            Private _workWidth As Single
            Private _workHeight As Single
            Private _descW As Single
            Private _mpnW As Single
            Private _locW As Single

            ' Band / row metrics.
            Private _headerBandH As Single
            Private _footerBandH As Single
            Private _titleH As Single
            Private _dataLineH As Single

            ' Resources.
            Private _headerFont As Font
            Private _titleFont As Font
            Private _dataFont As Font
            Private _greyBrush As SolidBrush
            Private _cellFormat As StringFormat

            ' Paginated content.
            Private ReadOnly _pages As New List(Of PageContent)()
            Private _pageIndex As Integer

            Public Sub New(files As IReadOnlyList(Of LocationReportFile), statusCallback As Action(Of String))
                _files = files
                _status = statusCallback
            End Sub

            Private NotInheritable Class RenderRow
                Public Cells As String()
                Public Height As Single
                Public Shaded As Boolean
            End Class

            Private NotInheritable Class PageContent
                Public FileName As String
                Public ReadOnly Rows As New List(Of RenderRow)()
            End Class

            Public Sub Run(owner As IWin32Window)
                Using doc As New PrintDocument()
                    doc.DocumentName = "Location Report"
                    doc.OriginAtMargins = True                       ' (0,0) = top-left of the margin box
                    ' Margins in 1/100": left/right = 0.60" (−40% from 1"), top/bottom = 0.40" (−60%).
                    doc.DefaultPageSettings.Margins = New Margins(60, 60, 40, 40)
                    SelectLetter(doc)
                    AddHandler doc.PrintPage, AddressOf OnPrintPage
                    ' Status notifications fire on the UI thread inside Print().
                    AddHandler doc.BeginPrint, Sub(s, ev) _status?.Invoke("Printing in progress...")
                    AddHandler doc.EndPrint, Sub(s, ev) _status?.Invoke("Printing done")

                    Using dlg As New PrintDialog()
                        dlg.Document = doc
                        dlg.UseEXDialog = True
                        If dlg.ShowDialog(owner) <> DialogResult.OK Then Return
                    End Using

                    Try
                        ComputeGeometry(doc)
                        CreateResources()
                        BuildPages(doc)
                        If _pages.Count = 0 Then Return
                        _pageIndex = 0
                        doc.Print()
                    Catch ex As Exception
                        ConfirmDialog.Notify(owner, "Location Report",
                            "Could not print the report:" & vbCrLf & ex.Message, icon:=DialogIcon.Error)
                    Finally
                        DisposeResources()
                    End Try
                End Using
            End Sub

            ''' <summary>Prefer real "Letter" paper from the chosen printer; else set 8.5×11.</summary>
            Private Shared Sub SelectLetter(doc As PrintDocument)
                For Each size As PaperSize In doc.PrinterSettings.PaperSizes
                    If size.Kind = PaperKind.Letter Then
                        doc.DefaultPageSettings.PaperSize = size
                        Return
                    End If
                Next
                doc.DefaultPageSettings.PaperSize = New PaperSize("Letter", 850, 1100)
            End Sub

            Private Sub ComputeGeometry(doc As PrintDocument)
                Dim ps = doc.DefaultPageSettings
                _workWidth = ps.Bounds.Width - ps.Margins.Left - ps.Margins.Right
                _workHeight = ps.Bounds.Height - ps.Margins.Top - ps.Margins.Bottom
                _descW = _workWidth / 2.0F
                _mpnW = _workWidth / 4.0F
                _locW = _workWidth - _descW - _mpnW      ' remainder → avoids a rounding gap
            End Sub

            Private Sub CreateResources()
                _headerFont = New Font("Segoe UI", 8.0F, FontStyle.Bold)
                _titleFont = New Font("Segoe UI", 7.0F, FontStyle.Bold)
                _dataFont = New Font("Segoe UI", 7.0F, FontStyle.Regular)
                _greyBrush = New SolidBrush(Color.FromArgb(230, 230, 230))   ' ~10% grey
                _cellFormat = New StringFormat() With {.Trimming = StringTrimming.None}
            End Sub

            Private Sub DisposeResources()
                _headerFont?.Dispose()
                _titleFont?.Dispose()
                _dataFont?.Dispose()
                _greyBrush?.Dispose()
                _cellFormat?.Dispose()
            End Sub

            ''' <summary>Sort each file, measure every row, and pack rows into pages.</summary>
            Private Sub BuildPages(doc As PrintDocument)
                Using mg As Graphics = doc.PrinterSettings.CreateMeasurementGraphics()
                    mg.PageUnit = GraphicsUnit.Display        ' hundredths of an inch

                    _headerBandH = _headerFont.GetHeight(mg) + 12.0F
                    _footerBandH = _headerFont.GetHeight(mg) + 12.0F
                    _titleH = _titleFont.GetHeight(mg) + 8.0F
                    _dataLineH = _dataFont.GetHeight(mg)

                    Dim dataTop As Single = _headerBandH + Gap + _titleH + Gap
                    Dim dataBottom As Single = _workHeight - _footerBandH - Gap
                    Dim dataMaxH As Single = dataBottom - dataTop

                    For Each f In _files
                        Dim sorted = If(f.Rows, New List(Of String())()) _
                            .OrderBy(Function(r) r(0), StringComparer.CurrentCultureIgnoreCase).ToList()

                        Dim page As PageContent = NewPage(f.FileName)
                        Dim y As Single = 0.0F
                        Dim idx As Integer = 0
                        For Each cells In sorted
                            Dim h As Single = MeasureRowHeight(mg, cells)
                            If y + h > dataMaxH AndAlso page.Rows.Count > 0 Then
                                page = NewPage(f.FileName)      ' overflow → new page, same file
                                y = 0.0F
                            End If
                            page.Rows.Add(New RenderRow With {.Cells = cells, .Height = h, .Shaded = (idx Mod 2 = 1)})
                            y += h
                            idx += 1
                        Next
                    Next
                End Using
            End Sub

            Private Function NewPage(fileName As String) As PageContent
                Dim p As New PageContent With {.FileName = fileName}
                _pages.Add(p)
                Return p
            End Function

            Private Function MeasureRowHeight(g As Graphics, cells As String()) As Single
                Dim hd As Single = MeasureCell(g, cells(0), _descW)
                Dim hm As Single = MeasureCell(g, cells(1), _mpnW)
                Dim hl As Single = MeasureCell(g, cells(2), _locW)
                Return Math.Max(hd, Math.Max(hm, hl)) + RowPadY
            End Function

            Private Function MeasureCell(g As Graphics, text As String, colW As Single) As Single
                If String.IsNullOrEmpty(text) Then Return _dataLineH
                Dim layout As New SizeF(colW - 2.0F * CellPadX, 100000.0F)
                Dim sz As SizeF = g.MeasureString(text, _dataFont, layout, _cellFormat)
                Return Math.Max(sz.Height, _dataLineH)
            End Function

            Private Sub OnPrintPage(sender As Object, e As PrintPageEventArgs)
                Dim g As Graphics = e.Graphics
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
                Dim page As PageContent = _pages(_pageIndex)

                ' Header band: filename (left) / print date-time (right), white on black.
                g.FillRectangle(Brushes.Black, 0.0F, 0.0F, _workWidth, _headerBandH)
                DrawBand(g, page.FileName, _printedAt.ToString("yyyy-MM-dd HH:mm"), 0.0F, _headerBandH)

                ' Column titles (bold black) + underline.
                Dim titleY As Single = _headerBandH + Gap
                g.DrawString("Description", _titleFont, Brushes.Black, CellPadX, titleY)
                g.DrawString("Manufacturer Part Number 1", _titleFont, Brushes.Black, _descW + CellPadX, titleY)
                g.DrawString("Location", _titleFont, Brushes.Black, _descW + _mpnW + CellPadX, titleY)
                Dim lineY As Single = titleY + _titleFont.GetHeight(g) + 2.0F
                g.DrawLine(Pens.Black, 0.0F, lineY, _workWidth, lineY)

                ' Data rows: alternate white / 10%-grey background, plain black text.
                Dim y As Single = _headerBandH + Gap + _titleH + Gap
                For Each rr In page.Rows
                    If rr.Shaded Then g.FillRectangle(_greyBrush, 0.0F, y, _workWidth, rr.Height)
                    DrawCell(g, rr.Cells(0), 0.0F, y, _descW, rr.Height)
                    DrawCell(g, rr.Cells(1), _descW, y, _mpnW, rr.Height)
                    DrawCell(g, rr.Cells(2), _descW + _mpnW, y, _locW, rr.Height)
                    y += rr.Height
                Next

                ' Footer band: app + version (left) / page x of y (right), white on black.
                Dim footerY As Single = _workHeight - _footerBandH
                g.FillRectangle(Brushes.Black, 0.0F, footerY, _workWidth, _footerBandH)
                DrawBand(g, "Altium CSV Librarian " & AppInfo.Version,
                         $"Page {_pageIndex + 1} of {_pages.Count}", footerY, _footerBandH)

                _pageIndex += 1
                e.HasMorePages = _pageIndex < _pages.Count
            End Sub

            ''' <summary>Draw a black band's white left/right text, vertically centred.</summary>
            Private Sub DrawBand(g As Graphics, leftText As String, rightText As String, top As Single, height As Single)
                Dim ty As Single = top + (height - _headerFont.GetHeight(g)) / 2.0F
                g.DrawString(leftText, _headerFont, Brushes.White, CellPadX, ty)
                Dim rsz As SizeF = g.MeasureString(rightText, _headerFont)
                g.DrawString(rightText, _headerFont, Brushes.White, _workWidth - CellPadX - rsz.Width, ty)
            End Sub

            Private Sub DrawCell(g As Graphics, text As String, x As Single, y As Single, w As Single, h As Single)
                If String.IsNullOrEmpty(text) Then Return
                Dim rect As New RectangleF(x + CellPadX, y + RowPadY / 2.0F, w - 2.0F * CellPadX, h - RowPadY)
                g.DrawString(text, _dataFont, Brushes.Black, rect, _cellFormat)
            End Sub

        End Class

        ' ── Missing-location print engine ─────────────────────────────────────────
        Private NotInheritable Class MissingLocationPrinter

            Private Const Gap As Single = 6.0F
            Private Const CellPadX As Single = 6.0F
            Private Const RowPadY As Single = 8.0F

            Private ReadOnly _items As IReadOnlyList(Of String)
            Private ReadOnly _status As Action(Of String)
            Private ReadOnly _printedAt As DateTime = DateTime.Now

            Private _workWidth As Single
            Private _workHeight As Single
            Private _colW As Single          ' 1/5 of the page width
            Private _headerBandH As Single
            Private _footerBandH As Single
            Private _rowH As Single

            Private _headerFont As Font
            Private _dataFont As Font
            Private _greyBrush As SolidBrush
            Private ReadOnly _pageStarts As New List(Of Integer)()   ' first item index per page
            Private _pageIndex As Integer

            Public Sub New(items As IReadOnlyList(Of String), statusCallback As Action(Of String))
                _items = items
                _status = statusCallback
            End Sub

            Public Sub Run(owner As IWin32Window)
                Using doc As New PrintDocument()
                    doc.DocumentName = "Missing Location Report"
                    doc.OriginAtMargins = True
                    doc.DefaultPageSettings.Margins = New Margins(60, 60, 40, 40)
                    SelectLetter(doc)
                    AddHandler doc.PrintPage, AddressOf OnPrintPage
                    AddHandler doc.BeginPrint, Sub(s, ev) _status?.Invoke("Printing in progress...")
                    AddHandler doc.EndPrint, Sub(s, ev) _status?.Invoke("Printing done")

                    Using dlg As New PrintDialog()
                        dlg.Document = doc
                        dlg.UseEXDialog = True
                        If dlg.ShowDialog(owner) <> DialogResult.OK Then Return
                    End Using

                    Try
                        Dim ps = doc.DefaultPageSettings
                        _workWidth = ps.Bounds.Width - ps.Margins.Left - ps.Margins.Right
                        _workHeight = ps.Bounds.Height - ps.Margins.Top - ps.Margins.Bottom
                        _colW = _workWidth / 5.0F

                        _headerFont = New Font("Segoe UI", 8.0F, FontStyle.Bold)
                        _dataFont = New Font("Segoe UI", 7.0F, FontStyle.Regular)
                        _greyBrush = New SolidBrush(Color.FromArgb(230, 230, 230))   ' ~10% grey

                        Using mg As Graphics = doc.PrinterSettings.CreateMeasurementGraphics()
                            mg.PageUnit = GraphicsUnit.Display
                            _headerBandH = _headerFont.GetHeight(mg) + 12.0F
                            _footerBandH = _headerFont.GetHeight(mg) + 12.0F
                            _rowH = _dataFont.GetHeight(mg) + RowPadY
                        End Using

                        BuildPages()
                        If _pageStarts.Count = 0 Then Return
                        _pageIndex = 0
                        doc.Print()
                    Catch ex As Exception
                        ConfirmDialog.Notify(owner, "Missing Location Report",
                            "Could not print the report:" & vbCrLf & ex.Message, icon:=DialogIcon.Error)
                    Finally
                        _headerFont?.Dispose()
                        _dataFont?.Dispose()
                        _greyBrush?.Dispose()
                    End Try
                End Using
            End Sub

            Private Shared Sub SelectLetter(doc As PrintDocument)
                For Each size As PaperSize In doc.PrinterSettings.PaperSizes
                    If size.Kind = PaperKind.Letter Then
                        doc.DefaultPageSettings.PaperSize = size
                        Return
                    End If
                Next
                doc.DefaultPageSettings.PaperSize = New PaperSize("Letter", 850, 1100)
            End Sub

            ''' <summary>All rows are the same height, so pagination is just a row count.</summary>
            Private Sub BuildPages()
                Dim dataTop As Single = _headerBandH + Gap
                Dim dataBottom As Single = _workHeight - _footerBandH - Gap
                Dim perPage As Integer = Math.Max(1, CInt(Math.Floor((dataBottom - dataTop) / _rowH)))
                Dim i As Integer = 0
                While i < _items.Count
                    _pageStarts.Add(i)
                    i += perPage
                End While
            End Sub

            Private Sub OnPrintPage(sender As Object, e As PrintPageEventArgs)
                Dim g As Graphics = e.Graphics
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias

                ' Header band.
                g.FillRectangle(Brushes.Black, 0.0F, 0.0F, _workWidth, _headerBandH)
                DrawBand(g, "Missing Location Report", _printedAt.ToString("yyyy-MM-dd HH:mm"), 0.0F, _headerBandH)

                ' Rows for this page (fixed height, alternating full-width stripe).
                Dim startIdx As Integer = _pageStarts(_pageIndex)
                Dim endIdx As Integer = If(_pageIndex + 1 < _pageStarts.Count, _pageStarts(_pageIndex + 1), _items.Count)
                Dim y As Single = _headerBandH + Gap
                For i As Integer = startIdx To endIdx - 1
                    If (i - startIdx) Mod 2 = 1 Then g.FillRectangle(_greyBrush, 0.0F, y, _workWidth, _rowH)
                    Dim rect As New RectangleF(CellPadX, y + RowPadY / 2.0F, _colW - 2.0F * CellPadX, _rowH - RowPadY)
                    g.DrawString(_items(i), _dataFont, Brushes.Black, rect)
                    y += _rowH
                Next

                ' Footer band.
                Dim footerY As Single = _workHeight - _footerBandH
                g.FillRectangle(Brushes.Black, 0.0F, footerY, _workWidth, _footerBandH)
                DrawBand(g, "Altium CSV Librarian " & AppInfo.Version,
                         $"Page {_pageIndex + 1} of {_pageStarts.Count}", footerY, _footerBandH)

                _pageIndex += 1
                e.HasMorePages = _pageIndex < _pageStarts.Count
            End Sub

            Private Sub DrawBand(g As Graphics, leftText As String, rightText As String, top As Single, height As Single)
                Dim ty As Single = top + (height - _headerFont.GetHeight(g)) / 2.0F
                g.DrawString(leftText, _headerFont, Brushes.White, CellPadX, ty)
                Dim rsz As SizeF = g.MeasureString(rightText, _headerFont)
                g.DrawString(rightText, _headerFont, Brushes.White, _workWidth - CellPadX - rsz.Width, ty)
            End Sub

        End Class

    End Module

End Namespace
