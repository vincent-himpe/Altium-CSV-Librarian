Imports System.Data
Imports System.Text
Imports System.Text.RegularExpressions
Imports CsvLibrarian.Models

Namespace Services

    ''' <summary>Outcome of running the normalizer against a table.</summary>
    Public Class NormalizeResult
        Public Property Matched As Boolean
        Public Property ChangedCells As Integer
        Public Property MissingColumns As List(Of String) = New List(Of String)()

        Public ReadOnly Property HasMissing As Boolean
            Get
                Return MissingColumns.Count > 0
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Applies <see cref="NormalizerRuleSet"/>s to a <see cref="DataTable"/>:
    ''' per-column text transforms followed by an optional composed column.
    ''' Fully data-driven — no component type is hard-coded.
    ''' </summary>
    Public Module NormalizerService

        ''' <summary>
        ''' Pick the best matching rule set for a file name. An exact
        ''' (case-insensitive) name match wins; otherwise the first glob match in
        ''' declaration order is used. Returns Nothing if none match.
        ''' </summary>
        Public Function FindRuleSet(config As NormalizerConfig, fileName As String) As NormalizerRuleSet
            If config Is Nothing OrElse config.RuleSets Is Nothing Then Return Nothing

            ' Exact match first.
            For Each rs In config.RuleSets
                If String.Equals(rs.Match, fileName, StringComparison.OrdinalIgnoreCase) Then
                    Return rs
                End If
            Next

            ' Then glob match.
            For Each rs In config.RuleSets
                If IsGlobMatch(rs.Match, fileName) Then
                    Return rs
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>Apply a rule set to the table in place, reporting what changed.</summary>
        Public Function Apply(table As DataTable, ruleSet As NormalizerRuleSet) As NormalizeResult
            Dim result As New NormalizeResult With {.Matched = True}
            If table Is Nothing OrElse ruleSet Is Nothing Then
                result.Matched = False
                Return result
            End If

            ' Verify required columns exist.
            Dim required As New List(Of String)()
            For Each n In ruleSet.Normalizations
                required.Add(n.Column)
            Next
            If ruleSet.Build IsNot Nothing Then
                required.Add(ruleSet.Build.Target)
                ' Quoted literals are constants, not columns.
                For Each src In ruleSet.Build.Sources
                    If Not IsLiteral(src) Then required.Add(src)
                Next
            End If
            For Each colName In required.Distinct(StringComparer.OrdinalIgnoreCase)
                If Not table.Columns.Contains(colName) Then
                    result.MissingColumns.Add(colName)
                End If
            Next
            If result.HasMissing Then Return result

            For Each row As DataRow In table.Rows
                If row.RowState = DataRowState.Deleted Then Continue For   ' a pending-delete row can't be read/written
                ' 1. Per-column transforms.
                For Each norm In ruleSet.Normalizations
                    Dim col As String = norm.Column
                    Dim original As String = CellText(row, col)
                    Dim transformed As String = ApplyTransforms(original, norm.Transforms)
                    If Not String.Equals(transformed, original, StringComparison.Ordinal) Then
                        row(col) = transformed
                        result.ChangedCells += 1
                    End If
                Next

                ' 2. Compose target column.
                If ruleSet.Build IsNot Nothing Then
                    Dim parts As New List(Of String)()
                    For Each src In ruleSet.Build.Sources
                        If IsLiteral(src) Then
                            parts.Add(LiteralValue(src))       ' fixed string, always included
                        Else
                            Dim v As String = CellText(row, src)
                            If ruleSet.Build.SkipEmpty AndAlso String.IsNullOrEmpty(v) Then Continue For
                            parts.Add(v)
                        End If
                    Next
                    Dim composed As String = String.Join(ruleSet.Build.Separator, parts)
                    Dim existing As String = CellText(row, ruleSet.Build.Target)
                    If Not String.Equals(composed, existing, StringComparison.Ordinal) Then
                        row(ruleSet.Build.Target) = composed
                        result.ChangedCells += 1
                    End If
                End If
            Next

            Return result
        End Function

        ''' <summary>True if a build source is a double-quoted literal, e.g. "RES".</summary>
        Private Function IsLiteral(token As String) As Boolean
            Return token IsNot Nothing AndAlso token.Length >= 2 AndAlso
                   token.StartsWith("""", StringComparison.Ordinal) AndAlso
                   token.EndsWith("""", StringComparison.Ordinal)
        End Function

        ''' <summary>The text of a quoted literal, with surrounding quotes removed
        ''' and any doubled inner quotes ("") collapsed to one.</summary>
        Private Function LiteralValue(token As String) As String
            Dim inner = token.Substring(1, token.Length - 2)
            Return inner.Replace("""""", """")
        End Function

        Private Function CellText(row As DataRow, col As String) As String
            ' A Deleted/Detached row has no accessible field data (row.Table may be Nothing).
            If row.RowState = DataRowState.Deleted OrElse row.RowState = DataRowState.Detached Then Return ""
            If Not row.Table.Columns.Contains(col) Then Return ""
            Dim v As Object = row(col)
            If v Is DBNull.Value Then Return ""
            Return If(Convert.ToString(v), "")
        End Function

        ''' <summary>Apply an ordered list of transform tokens to a value.</summary>
        Public Function ApplyTransforms(value As String, transforms As IEnumerable(Of String)) As String
            Dim v As String = If(value, "")
            If transforms Is Nothing Then Return v
            For Each t In transforms
                v = ApplyTransform(v, t)
            Next
            Return v
        End Function

        ''' <summary>
        ''' Apply a single transform token. Recognized (case-insensitive):
        '''   upper, lower, trim, stripspaces, collapsespaces,
        '''   replace:FROM=&gt;TO
        ''' Unknown tokens are ignored.
        ''' </summary>
        Public Function ApplyTransform(value As String, token As String) As String
            Dim v As String = If(value, "")
            If String.IsNullOrWhiteSpace(token) Then Return v

            Dim t As String = token.Trim()
            Dim lower As String = t.ToLowerInvariant()

            If lower.StartsWith("replace:") Then
                Dim body As String = t.Substring("replace:".Length)
                Dim idx As Integer = body.IndexOf("=>", StringComparison.Ordinal)
                If idx >= 0 Then
                    Dim fromStr As String = body.Substring(0, idx)
                    Dim toStr As String = body.Substring(idx + 2)
                    If fromStr.Length > 0 Then
                        v = v.Replace(fromStr, toStr)
                    End If
                End If
                Return v
            End If

            Select Case lower
                Case "upper"
                    Return v.ToUpperInvariant()
                Case "lower"
                    Return v.ToLowerInvariant()
                Case "trim"
                    Return v.Trim()
                Case "stripspaces"
                    Return v.Replace(" ", "")
                Case "collapsespaces"
                    Return Regex.Replace(v, "\s+", " ").Trim()
                Case Else
                    Return v
            End Select
        End Function

        ''' <summary>The transform tokens the editor offers as a picklist.</summary>
        Public ReadOnly Property KnownTransforms As String()
            Get
                Return New String() {"upper", "lower", "trim", "stripspaces", "collapsespaces", "replace:FROM=>TO"}
            End Get
        End Property

        Private Function IsGlobMatch(pattern As String, text As String) As Boolean
            If String.IsNullOrEmpty(pattern) Then Return False
            Dim rx As String = "^" & Regex.Escape(pattern).Replace("\*", ".*").Replace("\?", ".") & "$"
            Return Regex.IsMatch(text, rx, RegexOptions.IgnoreCase)
        End Function

    End Module

End Namespace
