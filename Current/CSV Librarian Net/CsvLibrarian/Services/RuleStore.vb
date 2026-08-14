Imports System.IO
Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports CsvLibrarian.Models

Namespace Services

    ''' <summary>
    ''' Loads and saves the per-folder <c>normalizer.json</c> rules file, which is
    ''' the source of truth for the generic normalizer. The in-app rules editor
    ''' reads and writes the same file.
    ''' </summary>
    Public Module RuleStore

        Public Const FileName As String = "normalizer.json"

        Private ReadOnly JsonOpts As JsonSerializerOptions = BuildOptions()

        Private Function BuildOptions() As JsonSerializerOptions
            Dim o As New JsonSerializerOptions With {
                .WriteIndented = True,
                .PropertyNameCaseInsensitive = True,
                .ReadCommentHandling = JsonCommentHandling.Skip,
                .AllowTrailingCommas = True,
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
            Return o
        End Function

        Public Function GetConfigPath(folder As String) As String
            Return Path.Combine(folder, FileName)
        End Function

        ''' <summary>
        ''' Load the config from a folder. Returns an empty config if the file
        ''' does not exist. Throws <see cref="JsonException"/> on malformed JSON.
        ''' </summary>
        Public Function Load(folder As String) As NormalizerConfig
            Dim path As String = GetConfigPath(folder)
            If Not File.Exists(path) Then
                Return New NormalizerConfig()
            End If
            Dim json As String = File.ReadAllText(path, Encoding.UTF8)
            If String.IsNullOrWhiteSpace(json) Then
                Return New NormalizerConfig()
            End If
            Dim cfg As NormalizerConfig = JsonSerializer.Deserialize(Of NormalizerConfig)(json, JsonOpts)
            Return If(cfg, New NormalizerConfig())
        End Function

        Public Sub Save(folder As String, config As NormalizerConfig)
            Dim path As String = GetConfigPath(folder)
            Dim json As String = JsonSerializer.Serialize(config, JsonOpts)
            File.WriteAllText(path, json, New UTF8Encoding(False))
        End Sub

        ''' <summary>
        ''' A starter config mirroring the original tool's hard-coded capacitor
        ''' rule, so the shipped capacitors.csv normalizes out of the box.
        ''' </summary>
        Public Function CreateDefaultConfig() As NormalizerConfig
            Dim cfg As New NormalizerConfig()
            Dim rs As New NormalizerRuleSet With {.Match = "capacitors.csv"}

            rs.Normalizations.Add(New ColumnNormalization With {
                .Column = "Value",
                .Transforms = New List(Of String) From {"lower", "replace:f=>F", "stripspaces"}})
            rs.Normalizations.Add(New ColumnNormalization With {
                .Column = "Voltage",
                .Transforms = New List(Of String) From {"upper", "stripspaces"}})
            rs.Normalizations.Add(New ColumnNormalization With {
                .Column = "Dielectric",
                .Transforms = New List(Of String) From {"upper", "stripspaces"}})

            rs.Build = New BuildRule With {
                .Target = "Description",
                .Sources = New List(Of String) From {"Library Ref", "Value", "Voltage", "Dielectric"},
                .Separator = "-",
                .SkipEmpty = True}

            cfg.RuleSets.Add(rs)
            Return cfg
        End Function

    End Module

End Namespace
