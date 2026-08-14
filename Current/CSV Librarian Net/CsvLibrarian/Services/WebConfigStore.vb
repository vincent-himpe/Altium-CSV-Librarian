Imports System.IO
Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports CsvLibrarian.Models

Namespace Services

    ''' <summary>
    ''' Loads and saves the WebCrawler search-engine list to
    ''' "Altium CSV Librarian WebConfig.JSON" under
    ''' %LocalAppData%\CsvLibrarian (the same location as settings.json).
    ''' </summary>
    Public Module WebConfigStore

        Public Const FileName As String = "Altium CSV Librarian WebConfig.JSON"

        Private ReadOnly JsonOpts As JsonSerializerOptions = BuildOptions()

        Private Function BuildOptions() As JsonSerializerOptions
            Return New JsonSerializerOptions With {
                .WriteIndented = True,
                .PropertyNameCaseInsensitive = True,
                .ReadCommentHandling = JsonCommentHandling.Skip,
                .AllowTrailingCommas = True,
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
        End Function

        ''' <summary>Full path to the config file (creates the folder if needed).</summary>
        Public Function GetConfigPath() As String
            Dim dir As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CsvLibrarian")
            Directory.CreateDirectory(dir)
            Return Path.Combine(dir, FileName)
        End Function

        ''' <summary>Load the config, or an empty one if the file is missing/corrupt.</summary>
        Public Function Load() As WebConfig
            Try
                Dim path As String = GetConfigPath()
                If File.Exists(path) Then
                    Dim json As String = File.ReadAllText(path, Encoding.UTF8)
                    If Not String.IsNullOrWhiteSpace(json) Then
                        Dim cfg = JsonSerializer.Deserialize(Of WebConfig)(json, JsonOpts)
                        If cfg IsNot Nothing Then
                            If cfg.Engines Is Nothing Then cfg.Engines = New List(Of SearchEngine)()
                            ' Migrate older files: if any engine lacks the quicklook
                            ' field it deserialized to the False default — persist it.
                            If NeedsQuickLookMigration(json) Then Save(cfg)
                            Return cfg
                        End If
                    End If
                End If
            Catch
                ' Ignore corrupt config; fall back to an empty list.
            End Try
            Return New WebConfig()
        End Function

        ''' <summary>True if the raw JSON has an engine object without a "quicklook"
        ''' property (i.e. it predates that field and should be re-saved).</summary>
        Private Function NeedsQuickLookMigration(json As String) As Boolean
            Try
                Using doc = JsonDocument.Parse(json)
                    Dim root = doc.RootElement
                    If root.ValueKind <> JsonValueKind.Object Then Return False
                    Dim engines As JsonElement
                    If Not root.TryGetProperty("engines", engines) Then Return False
                    If engines.ValueKind <> JsonValueKind.Array Then Return False
                    For Each el In engines.EnumerateArray()
                        If el.ValueKind = JsonValueKind.Object Then
                            Dim tmp As JsonElement
                            If Not el.TryGetProperty("quicklook", tmp) Then Return True
                        End If
                    Next
                End Using
            Catch
                ' Unparseable JSON: leave migration alone.
            End Try
            Return False
        End Function

        ''' <summary>Serialize the config to disk as UTF-8 (no BOM).</summary>
        Public Sub Save(config As WebConfig)
            Dim json As String = JsonSerializer.Serialize(config, JsonOpts)
            File.WriteAllText(GetConfigPath(), json, New UTF8Encoding(False))
        End Sub

    End Module

End Namespace
