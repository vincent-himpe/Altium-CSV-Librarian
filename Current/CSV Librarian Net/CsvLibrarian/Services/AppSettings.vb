Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.Json

Namespace Services

    ''' <summary>
    ''' Tiny persisted settings store (last-opened folder, autosave seconds),
    ''' kept as JSON under
    ''' %LocalAppData%\CsvLibrarian\Altium CSV Librarian Settings.json.
    ''' </summary>
    Public Class AppSettings

        Public Property LastFolder As String = ""
        Public Property AutosaveSeconds As Integer = 30

        ''' <summary>Whether the autosave countdown runs at all (Options window).</summary>
        Public Property AutosaveEnabled As Boolean = True

        ''' <summary>Run the normalizer on a file whenever it is saved.</summary>
        Public Property AutoNormalizeOnSave As Boolean = False

        ''' <summary>Create a zip archive of the working folder when the app exits.</summary>
        Public Property ArchiveOnExit As Boolean = False

        ''' <summary>Most-recently-opened working folders, newest first (max 5).</summary>
        Public Property RecentFolders As List(Of String) = New List(Of String)()

        ''' <summary>Persisted contents of the 10×10 Advanced Clipboard grid, stored as
        ''' a jagged list (rows of columns). Empty/absent -> initialized to blanks.</summary>
        Public Property AdvancedClipboard As List(Of List(Of String)) = New List(Of List(Of String))()

        ''' <summary>Supplier API credentials (Supplier API Integration window).
        ''' Absent in JSON -> a blank record, saved on the next write.</summary>
        Public Property SupplierApi As SupplierApiSettings = New SupplierApiSettings()

        Private Shared ReadOnly JsonOpts As New JsonSerializerOptions With {.WriteIndented = True}

        Private Shared Function SettingsPath() As String
            Dim dir As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CsvLibrarian")
            Directory.CreateDirectory(dir)
            Return Path.Combine(dir, "Altium CSV Librarian Settings.json")
        End Function

        Public Shared Function Load() As AppSettings
            Try
                Dim path As String = SettingsPath()
                If File.Exists(path) Then
                    Dim json As String = File.ReadAllText(path, Encoding.UTF8)
                    Dim s = JsonSerializer.Deserialize(Of AppSettings)(json, JsonOpts)
                    If s IsNot Nothing Then
                        If s.SupplierApi Is Nothing Then s.SupplierApi = New SupplierApiSettings()
                        Return s
                    End If
                End If
            Catch
                ' Ignore corrupt settings; fall back to defaults.
            End Try
            Return New AppSettings()
        End Function

        Public Sub Save()
            Try
                File.WriteAllText(SettingsPath(), JsonSerializer.Serialize(Me, JsonOpts),
                                  New UTF8Encoding(False))
            Catch
                ' Non-fatal.
            End Try
        End Sub

    End Class

    ''' <summary>Per-supplier API credentials edited in the Supplier API Integration
    ''' window. All blank by default.</summary>
    Public Class SupplierApiSettings
        Public Property DigikeyClientId As String = ""
        Public Property DigikeyAccessToken As String = ""
        Public Property MouserApiKey As String = ""
        Public Property LcscApiKey As String = ""
        Public Property LcscApiSecret As String = ""
        Public Property TmeApiToken As String = ""
    End Class

End Namespace
