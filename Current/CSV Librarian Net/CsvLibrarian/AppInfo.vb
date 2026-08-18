Imports System
Imports System.Collections.Generic

Namespace Global.CsvLibrarian

    ''' <summary>Application-wide constants.</summary>
    Public Module AppInfo

        ''' <summary>
        ''' Application version. Appended to the main window caption at startup,
        ''' e.g. "Altium CSV Librarian V 0.1".
        ''' </summary>
        Public Const Version As String = "-Beta- 0.7"

        ''' <summary>
        ''' Comma-separated header row written to a brand-new library CSV
        ''' (File ▸ New Library). Edit this list to change the default schema.
        ''' </summary>
        Public Const NewLibraryHeaders As String =
            "Index,Library Ref,Library Path,Footprint Ref,Footprint Path,Description," &
            "Manufacturer 1,Manufacturer Part Number 1,Location,LCSC,Digikey,Mouser,TME"

        ''' <summary>Body text shown in the About window.</summary>
        Public Const AboutString As String =
            "Designed by Vincent Himpe." & vbCrLf &
            "Coded by Claude" & vbCrLf & vbCrLf &
            "Released under Creative Commons 0 License" & vbCrLf &
            "No implied warranties. User assumes all risk."

        ''' <summary>
        ''' App-local single-cell clipboard used by Edit ▸ Copy / Paste. This is
        ''' deliberately NOT the Windows clipboard. Empty at startup.
        ''' </summary>
        Public OurClipboard As String = ""

        ''' <summary>
        ''' The current search string (set by Find / Find Cell / Find in Column) and
        ''' the "replace with" value used by the Replace-Next commands. Empty at startup.
        ''' </summary>
        Public FindString As String = ""

        ''' <summary>Fixed dimensions of the <see cref="AdvancedClipboard"/> grid.</summary>
        Public Const AdvancedClipboardRows As Integer = 10
        Public Const AdvancedClipboardCols As Integer = 10

        ''' <summary>
        ''' A 10×10 grid of free-text clipboard slots, edited via the Advanced Clipboard
        ''' window and persisted in the settings JSON. May be read/written anywhere in
        ''' the app. Populated from the JSON at startup (empty strings if absent).
        ''' </summary>
        Public AdvancedClipboard(AdvancedClipboardRows - 1, AdvancedClipboardCols - 1) As String

        ''' <summary>Currently selected row (0..9) of <see cref="AdvancedClipboard"/>,
        ''' shown in the status bar. Rolls over on increment/decrement. Session-only
        ''' (not persisted); starts at 0.</summary>
        Public AdvancedClipboardIndex As Integer = 0

        ''' <summary>Set every cell of <see cref="AdvancedClipboard"/> to "".</summary>
        Public Sub ClearAdvancedClipboard()
            For r As Integer = 0 To AdvancedClipboardRows - 1
                For c As Integer = 0 To AdvancedClipboardCols - 1
                    AdvancedClipboard(r, c) = ""
                Next
            Next
        End Sub

        ''' <summary>Convert the 2-D clipboard into a jagged list for JSON storage.</summary>
        Public Function AdvancedClipboardToJagged() As List(Of List(Of String))
            Dim outer As New List(Of List(Of String))()
            For r As Integer = 0 To AdvancedClipboardRows - 1
                Dim inner As New List(Of String)()
                For c As Integer = 0 To AdvancedClipboardCols - 1
                    inner.Add(If(AdvancedClipboard(r, c), ""))
                Next
                outer.Add(inner)
            Next
            Return outer
        End Function

        ''' <summary>
        ''' Load the 2-D clipboard from a jagged list (from JSON). Fail-safe: every cell
        ''' starts as "", so missing / short / absent data simply yields empty strings.
        ''' </summary>
        Public Sub AdvancedClipboardFromJagged(data As List(Of List(Of String)))
            ClearAdvancedClipboard()
            If data Is Nothing Then Return
            For r As Integer = 0 To Math.Min(AdvancedClipboardRows, data.Count) - 1
                Dim inner = data(r)
                If inner Is Nothing Then Continue For
                For c As Integer = 0 To Math.Min(AdvancedClipboardCols, inner.Count) - 1
                    AdvancedClipboard(r, c) = If(inner(c), "")
                Next
            Next
        End Sub

    End Module

End Namespace
