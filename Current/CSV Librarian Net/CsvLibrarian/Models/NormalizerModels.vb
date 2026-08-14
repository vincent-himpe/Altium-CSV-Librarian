Imports System.Collections.Generic
Imports System.Text.Json.Serialization

Namespace Models

    ''' <summary>
    ''' Root of the normalizer.json rules file. A folder may contain one such file;
    ''' each <see cref="NormalizerRuleSet"/> targets one or more CSV files by name.
    ''' </summary>
    Public Class NormalizerConfig
        <JsonPropertyName("version")>
        Public Property Version As Integer = 1

        <JsonPropertyName("ruleSets")>
        Public Property RuleSets As List(Of NormalizerRuleSet) = New List(Of NormalizerRuleSet)()
    End Class

    ''' <summary>
    ''' A set of rules applied to files whose name matches <see cref="Match"/>.
    ''' Match may be an exact file name ("capacitors.csv") or a wildcard glob
    ''' ("res_*.csv", "*"). Matching is case-insensitive.
    ''' </summary>
    Public Class NormalizerRuleSet
        <JsonPropertyName("match")>
        Public Property Match As String = ""

        ''' <summary>Per-column cleanup transforms, applied first.</summary>
        <JsonPropertyName("normalizations")>
        Public Property Normalizations As List(Of ColumnNormalization) = New List(Of ColumnNormalization)()

        ''' <summary>Optional rule that composes a target column from source columns.</summary>
        <JsonPropertyName("build")>
        Public Property Build As BuildRule = Nothing
    End Class

    ''' <summary>
    ''' Transforms applied in order to a single column's value.
    ''' Supported transform tokens (case-insensitive):
    '''   upper, lower, trim, stripspaces, collapsespaces,
    '''   replace:FROM=&gt;TO   (literal substring replace)
    ''' </summary>
    Public Class ColumnNormalization
        <JsonPropertyName("column")>
        Public Property Column As String = ""

        <JsonPropertyName("transforms")>
        Public Property Transforms As List(Of String) = New List(Of String)()
    End Class

    ''' <summary>
    ''' Builds <see cref="Target"/> by joining the (already-normalized) values of
    ''' <see cref="Sources"/> with <see cref="Separator"/>.
    ''' </summary>
    Public Class BuildRule
        <JsonPropertyName("target")>
        Public Property Target As String = ""

        <JsonPropertyName("sources")>
        Public Property Sources As List(Of String) = New List(Of String)()

        <JsonPropertyName("separator")>
        Public Property Separator As String = "-"

        ''' <summary>When true, empty source values are skipped rather than
        ''' producing empty segments (e.g. "A--C").</summary>
        <JsonPropertyName("skipEmpty")>
        Public Property SkipEmpty As Boolean = True
    End Class

End Namespace
