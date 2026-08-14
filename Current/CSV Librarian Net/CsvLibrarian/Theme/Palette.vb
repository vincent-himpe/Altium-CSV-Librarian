Imports System.Drawing

Namespace Theme

    ''' <summary>
    ''' Dark-theme colour palette, mirroring the HTML/Python design tokens used by
    ''' the original Altium CSV Librarian.
    ''' </summary>
    Public Module Palette

        Public ReadOnly Bg As Color = ColorFromHex("#0f1117")
        Public ReadOnly Surface As Color = ColorFromHex("#181b24")
        Public ReadOnly Panel As Color = ColorFromHex("#1e2130")
        Public ReadOnly Border As Color = ColorFromHex("#2a2f42")
        Public ReadOnly Accent As Color = ColorFromHex("#4f8ef7")
        Public ReadOnly AccentDark As Color = ColorFromHex("#3a72d8")
        Public ReadOnly Danger As Color = ColorFromHex("#e05c5c")
        Public ReadOnly Success As Color = ColorFromHex("#3ecf8e")
        Public ReadOnly Warning As Color = ColorFromHex("#f9a825")
        Public ReadOnly TextColor As Color = ColorFromHex("#e2e6f0")
        Public ReadOnly TextMuted As Color = ColorFromHex("#7a82a0")
        Public ReadOnly TextDim As Color = ColorFromHex("#3d4460")
        Public ReadOnly GuidBg As Color = ColorFromHex("#242a3a")
        Public ReadOnly GuidFg As Color = ColorFromHex("#BFBFBF")
        Public ReadOnly HeaderBg As Color = ColorFromHex("#151720")
        Public ReadOnly RowAlt As Color = ColorFromHex("#181c2b")
        Public ReadOnly SelectionBg As Color = ColorFromHex("#28304a")
        Public ReadOnly Gold As Color = ColorFromHex("#CFB53B")
        Public ReadOnly BannerBg As Color = ColorFromHex("#000000")

        ''' <summary>Parse a "#rrggbb" hex string into a Color.</summary>
        Public Function ColorFromHex(hex As String) As Color
            hex = hex.TrimStart("#"c)
            Dim r As Integer = Convert.ToInt32(hex.Substring(0, 2), 16)
            Dim g As Integer = Convert.ToInt32(hex.Substring(2, 2), 16)
            Dim b As Integer = Convert.ToInt32(hex.Substring(4, 2), 16)
            Return Color.FromArgb(r, g, b)
        End Function

    End Module

End Namespace
