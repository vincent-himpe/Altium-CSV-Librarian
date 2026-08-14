Imports System.Collections.Generic
Imports System.Text.Json.Serialization

Namespace Models

    ''' <summary>
    ''' One search-engine entry for the WebCrawler window: a website, a query
    ''' template, whether it is enabled, and a QuickLook flag.
    ''' </summary>
    Public Class SearchEngine
        <JsonPropertyName("website")>
        Public Property Website As String = ""

        <JsonPropertyName("query")>
        Public Property Query As String = ""

        <JsonPropertyName("enabled")>
        Public Property Enabled As Boolean = True

        <JsonPropertyName("quicklook")>
        Public Property QuickLook As Boolean = False
    End Class

    ''' <summary>
    ''' Root of the "Altium CSV Librarian WebConfig.JSON" file: the list of
    ''' configured search engines.
    ''' </summary>
    Public Class WebConfig
        <JsonPropertyName("engines")>
        Public Property Engines As List(Of SearchEngine) = New List(Of SearchEngine)()
    End Class

End Namespace
