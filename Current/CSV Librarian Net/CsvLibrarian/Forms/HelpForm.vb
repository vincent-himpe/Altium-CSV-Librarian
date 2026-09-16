Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>
    ''' Modal keyboard / feature reference. The content lives in <see cref="HelpContent.Text"/>
    ''' (a tiny markup, compiled in); this form parses and renders it.
    '''
    ''' MARKUP:
    '''   • Bare text (outside braces) = a section header. {action, description} = a row.
    '''   • "~" is the ONLY line break in a description ("~~" makes a blank line).
    '''   • A field may start with a colour code that PERSISTS until the next code:
    '''     [R] red, [G] green, [B] blue, [W] white, [Y] yellow, [M] muted grey, and
    '''     [/] to clear back to the default scheme. Without a code a field uses its default
    '''     (blue header, light action, muted description).
    ''' </summary>
    Public Class HelpForm

        Private Const KeyW As Integer = 190
        Private Const GapW As Integer = 12
        Private Const DescW As Integer = 740

        Private Shared ReadOnly DefaultAction As Color = Color.FromArgb(220, 220, 220)
        Private Shared ReadOnly DefaultDesc As Color = Color.FromArgb(170, 170, 174)

        Public Sub New()
            InitializeComponent()
            DarkTheme.StyleFlatButtons(btnClose)
            PopulateContent()
        End Sub

        Private Sub PopulateContent()
            pnlContent.SuspendLayout()
            For Each item In ParseHelp(HelpContent.Text)
                If item.IsHeader Then
                    Dim color = If(item.ActionColor.HasValue, item.ActionColor.Value, Palette.Accent)
                    Dim head = UiFactory.MakeLabel(item.Action, color, bold:=True, size:=9.0F)
                    head.Margin = New Padding(0, 10, 0, 2)
                    head.BackColor = Color.Transparent
                    pnlContent.Controls.Add(head)
                Else
                    pnlContent.Controls.Add(MakeRow(item))
                End If
            Next
            pnlContent.ResumeLayout()
        End Sub

        Private Function MakeRow(item As HelpItem) As Control
            Dim d As New Label With {
                .Text = item.Description,
                .AutoSize = True,
                .MaximumSize = New Size(DescW, 0),
                .Location = New Point(KeyW + GapW, 2),
                .ForeColor = If(item.DescColor.HasValue, item.DescColor.Value, DefaultDesc),
                .Font = New Font("Segoe UI", 9.0F)
            }
            Dim rowH = Math.Max(22, d.PreferredSize.Height)
            Dim k As New Label With {
                .Text = item.Action,
                .AutoSize = False,
                .Size = New Size(KeyW, rowH),
                .Location = New Point(0, 2),
                .TextAlign = ContentAlignment.TopLeft,
                .ForeColor = If(item.ActionColor.HasValue, item.ActionColor.Value, DefaultAction),
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
            }
            Dim row As New Panel With {
                .Size = New Size(KeyW + GapW + DescW, rowH + 6),
                .Margin = New Padding(0, 1, 0, 1)
            }
            row.Controls.Add(k)
            row.Controls.Add(d)
            Return row
        End Function

        ' ── Markup parser ─────────────────────────────────────────────────────────
        Private Structure HelpItem
            Public IsHeader As Boolean
            Public Action As String            ' header text, or the row's shortcut
            Public ActionColor As Color?       ' Nothing → default colour
            Public Description As String
            Public DescColor As Color?
        End Structure

        ''' <summary>Split the markup into headers and rows, tracking the persistent colour.</summary>
        Private Shared Iterator Function ParseHelp(src As String) As IEnumerable(Of HelpItem)
            Dim current As Color? = Nothing        ' persists across fields until a [x] code
            Dim i As Integer = 0
            While i < src.Length
                Dim brace = src.IndexOf("{"c, i)
                Dim headerRaw = If(brace < 0, src.Substring(i), src.Substring(i, brace - i))
                Dim header = TakeColor(headerRaw, current)
                If header.Length > 0 Then
                    Yield New HelpItem With {.IsHeader = True, .Action = header, .ActionColor = current}
                End If
                If brace < 0 Then Exit While

                Dim close = src.IndexOf("}"c, brace + 1)
                If close < 0 Then Exit While        ' malformed — stop gracefully
                Dim inner = src.Substring(brace + 1, close - brace - 1)
                Dim comma = inner.IndexOf(","c)
                Dim actionRaw = If(comma >= 0, inner.Substring(0, comma), inner)
                Dim descRaw = If(comma >= 0, inner.Substring(comma + 1), "")

                Dim action = TakeColor(actionRaw, current)
                Dim actionColor = current
                Dim desc = TakeColor(descRaw, current)
                Dim descColor = current
                Yield New HelpItem With {
                    .IsHeader = False,
                    .Action = action,
                    .ActionColor = actionColor,
                    .Description = ExpandDescription(desc),
                    .DescColor = descColor
                }
                i = close + 1
            End While
        End Function

        ''' <summary>If the (trimmed) field starts with a colour code, update
        ''' <paramref name="current"/> and strip it; return the remaining field text.</summary>
        Private Shared Function TakeColor(field As String, ByRef current As Color?) As String
            Dim t = If(field, "").Trim()
            If t.Length >= 3 AndAlso t(0) = "["c AndAlso t(2) = "]"c Then
                Dim code = t(1)
                If code = "/"c Then                 ' [/] → back to the default scheme
                    current = Nothing
                    Return t.Substring(3).Trim()
                End If
                Dim c = MapColor(code)
                If c.HasValue Then
                    current = c
                    Return t.Substring(3).Trim()
                End If
            End If
            Return t
        End Function

        Private Shared Function MapColor(letter As Char) As Color?
            Select Case Char.ToUpperInvariant(letter)
                Case "R"c : Return Color.FromArgb(235, 80, 80)     ' bright red
                Case "G"c : Return Color.FromArgb(120, 200, 120)   ' light green
                Case "B"c : Return Color.FromArgb(102, 178, 255)   ' light blue
                Case "W"c : Return Color.White
                Case "Y"c : Return Color.FromArgb(228, 208, 92)    ' yellow
                Case "M"c : Return Color.FromArgb(170, 170, 174)   ' muted grey (matches item default)
                Case Else : Return Nothing
            End Select
        End Function

        ''' <summary>"~" → line break (so "~~" makes a blank line); trims each line and the ends.</summary>
        Private Shared Function ExpandDescription(desc As String) As String
            Dim lines = desc.Replace("~", vbCrLf).Split({vbCrLf}, StringSplitOptions.None).
                             Select(Function(x) x.Trim())
            Return String.Join(vbCrLf, lines).Trim()
        End Function

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

    End Class

End Namespace
