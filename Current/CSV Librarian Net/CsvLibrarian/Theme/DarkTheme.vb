Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms

Namespace Theme

    ''' <summary>
    ''' Static helpers for the repetitive dark-scheme styling that has to be done in
    ''' code-behind (flat buttons, DataGridView cell/header/selection colours). This is
    ''' NOT a runtime theming/palette abstraction — static controls still get their
    ''' colours set directly in each form's designer. These helpers only remove the
    ''' identical boilerplate loops that every form was repeating. Callers set anything
    ''' form-specific (header font, row-header colours, a muted key column) after calling.
    '''
    ''' The colour constants are the single source of truth for the palette; see
    ''' reference_dark_scheme for the values and rationale.
    ''' </summary>
    Friend Module DarkTheme

        ' ── Palette (literal RGB) ──────────────────────────────────────────────────
        Friend ReadOnly FormBackground As Color = Color.FromArgb(45, 45, 48)
        Friend ReadOnly Band As Color = Color.FromArgb(62, 62, 66)          ' header / toolbar band
        Friend ReadOnly RowPrimary As Color = Color.FromArgb(51, 51, 55)    ' grid grey shade 1
        Friend ReadOnly RowAlternate As Color = Color.FromArgb(37, 37, 41)  ' grid grey shade 2
        Friend ReadOnly TextNormal As Color = Color.FromArgb(220, 220, 220)
        Friend ReadOnly TextMuted As Color = Color.FromArgb(157, 157, 160)
        Friend ReadOnly Accent As Color = Color.FromArgb(61, 92, 135)       ' selection / active cell
        Friend ReadOnly ButtonFace As Color = Color.FromArgb(63, 63, 70)
        Friend ReadOnly ButtonBorder As Color = Color.FromArgb(85, 85, 90)
        Friend ReadOnly GridLines As Color = Color.FromArgb(0, 0, 64)       ' navy

        ' Toolbar glyph colours + the "Libraries" header band.
        Friend ReadOnly IconBlue As Color = Color.FromArgb(102, 178, 255)   ' default glyph
        Friend ReadOnly IconGreen As Color = Color.FromArgb(120, 200, 120)  ' Save All
        Friend ReadOnly IconRed As Color = Color.FromArgb(235, 80, 80)      ' Delete Row (destructive)
        Friend ReadOnly HeaderBlue As Color = Color.FromArgb(79, 148, 205)  ' label band, white text

        ' Resolved once: the best available icon-glyph font family.
        Private _glyphFamily As String = Nothing

        ''' <summary>
        ''' A glyph font using the first installed of "Segoe Fluent Icons" (Win11) →
        ''' "Segoe MDL2 Assets" (Win10) → "Segoe UI Symbol" (fallback). The chosen family
        ''' is cached after the first call.
        ''' </summary>
        Friend Function GlyphFont(size As Single, Optional style As FontStyle = FontStyle.Regular) As Font
            If _glyphFamily Is Nothing Then
                _glyphFamily = FirstInstalledFont(
                    {"Segoe Fluent Icons", "Segoe MDL2 Assets", "Segoe UI Symbol"}, "Segoe UI Symbol")
            End If
            Return New Font(_glyphFamily, size, style)
        End Function

        ''' <summary>First of <paramref name="candidates"/> that is actually installed, else the fallback.</summary>
        Private Function FirstInstalledFont(candidates As String(), fallback As String) As String
            Try
                Using ifc As New System.Drawing.Text.InstalledFontCollection()
                    Dim installed = ifc.Families.Select(Function(f) f.Name).ToHashSet(StringComparer.OrdinalIgnoreCase)
                    For Each name In candidates
                        If installed.Contains(name) Then Return name
                    Next
                End Using
            Catch
                ' Enumeration failed — just use the fallback.
            End Try
            Return fallback
        End Function

        ''' <summary>Make one or more buttons flat and dark (border, face, text).</summary>
        Friend Sub StyleFlatButtons(ParamArray buttons As Button())
            For Each b In buttons
                If b Is Nothing Then Continue For
                b.FlatStyle = FlatStyle.Flat
                b.FlatAppearance.BorderColor = ButtonBorder
                b.BackColor = ButtonFace
                b.ForeColor = TextNormal
                b.UseVisualStyleBackColor = False
            Next
        End Sub

        ''' <summary>
        ''' Apply the common dark grid look: dark background, navy gridlines, dark header
        ''' band, two-grey alternating rows and blue full-selection. Header font and any
        ''' other per-grid specifics (row headers, a muted column) are left to the caller.
        ''' </summary>
        Friend Sub StyleGrid(grid As DataGridView)
            grid.BackgroundColor = FormBackground
            grid.GridColor = GridLines
            grid.EnableHeadersVisualStyles = False
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            grid.ColumnHeadersDefaultCellStyle.BackColor = Band
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextNormal
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Band
            grid.DefaultCellStyle.BackColor = RowPrimary
            grid.DefaultCellStyle.ForeColor = TextNormal
            grid.DefaultCellStyle.SelectionBackColor = Accent
            grid.DefaultCellStyle.SelectionForeColor = Color.White
            grid.AlternatingRowsDefaultCellStyle.BackColor = RowAlternate
            grid.AlternatingRowsDefaultCellStyle.ForeColor = TextNormal
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Accent
        End Sub

    End Module

End Namespace
