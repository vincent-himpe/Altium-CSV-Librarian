Imports System.Data
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Namespace Services

    ''' <summary>
    ''' Reads and writes CSV files as <see cref="DataTable"/>s.
    '''
    ''' Reading uses <see cref="TextFieldParser"/> so quoted fields, embedded
    ''' commas and newlines are handled correctly. Writing quotes every field
    ''' (matching the original tool's QUOTE_ALL behaviour) and emits UTF-8 without
    ''' a BOM using CRLF line endings.
    ''' </summary>
    Public Module CsvService

        ''' <summary>Read a CSV file into a DataTable of string columns.</summary>
        Public Function ReadCsv(path As String) As DataTable
            Dim table As New DataTable()
            table.TableName = System.IO.Path.GetFileNameWithoutExtension(path)

            ' UTF-8 with BOM detection (mirrors Python's utf-8-sig read).
            Using parser As New TextFieldParser(path, New UTF8Encoding(False), True)
                parser.TextFieldType = FieldType.Delimited
                parser.SetDelimiters(",")
                parser.HasFieldsEnclosedInQuotes = True
                parser.TrimWhiteSpace = False

                If parser.EndOfData Then
                    Return table
                End If

                ' Header row -> unique column names.
                Dim rawHeaders As String() = parser.ReadFields()
                Dim used As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                For i As Integer = 0 To rawHeaders.Length - 1
                    Dim name As String = If(rawHeaders(i), "").Trim()
                    If name.Length = 0 Then name = "Column" & (i + 1).ToString()
                    Dim unique As String = name
                    Dim suffix As Integer = 2
                    While used.Contains(unique)
                        unique = name & "_" & suffix.ToString()
                        suffix += 1
                    End While
                    used.Add(unique)
                    table.Columns.Add(unique, GetType(String))
                Next

                Dim colCount As Integer = table.Columns.Count

                ' Data rows.
                While Not parser.EndOfData
                    Dim fields As String()
                    Try
                        fields = parser.ReadFields()
                    Catch ex As MalformedLineException
                        ' Skip a malformed line rather than aborting the whole file.
                        Continue While
                    End Try
                    If fields Is Nothing Then Continue While

                    ' Skip blank lines (a lone empty field, or all fields empty).
                    If fields.All(Function(f) String.IsNullOrEmpty(f)) Then Continue While

                    Dim row As DataRow = table.NewRow()
                    For c As Integer = 0 To colCount - 1
                        row(c) = If(c < fields.Length, If(fields(c), ""), "")
                    Next
                    table.Rows.Add(row)
                End While
            End Using

            table.AcceptChanges()
            Return table
        End Function

        ''' <summary>Write a DataTable to CSV: every field quoted, UTF-8 (no BOM), CRLF.</summary>
        Public Sub WriteCsv(path As String, table As DataTable)
            Dim sb As New StringBuilder()

            ' Header row.
            Dim headerCells As New List(Of String)()
            For Each col As DataColumn In table.Columns
                headerCells.Add(QuoteField(col.ColumnName))
            Next
            sb.Append(String.Join(",", headerCells))
            sb.Append(vbCrLf)

            ' Data rows.
            For Each row As DataRow In table.Rows
                Dim cells As New List(Of String)()
                For Each col As DataColumn In table.Columns
                    Dim value As String = If(row(col) Is DBNull.Value, "", Convert.ToString(row(col)))
                    cells.Add(QuoteField(If(value, "")))
                Next
                sb.Append(String.Join(",", cells))
                sb.Append(vbCrLf)
            Next

            ' UTF-8 without BOM.
            File.WriteAllText(path, sb.ToString(), New UTF8Encoding(False))
        End Sub

        ''' <summary>Wrap a field in quotes, doubling any internal quotes.</summary>
        Private Function QuoteField(value As String) As String
            If value Is Nothing Then value = ""
            Return """" & value.Replace("""", """""") & """"
        End Function

    End Module

End Namespace
