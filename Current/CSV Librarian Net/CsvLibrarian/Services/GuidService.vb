Imports System.Data

Namespace Services

    ''' <summary>
    ''' GUID helpers. The first column of every library CSV is a stable GUID;
    ''' blank cells there are back-filled with a fresh GUID on load.
    ''' </summary>
    Public Module GuidService

        Public Function NewGuid() As String
            Return Guid.NewGuid().ToString()
        End Function

        ''' <summary>
        ''' Fill any blank first-column cell with a new GUID.
        ''' Returns True if at least one GUID was injected.
        ''' </summary>
        Public Function InjectGuids(table As DataTable) As Boolean
            If table Is Nothing OrElse table.Columns.Count = 0 Then Return False

            Dim guidCol As DataColumn = table.Columns(0)
            Dim injected As Boolean = False

            For Each row As DataRow In table.Rows
                Dim current As String = If(row(guidCol) Is DBNull.Value, "", Convert.ToString(row(guidCol)))
                If String.IsNullOrWhiteSpace(current) Then
                    row(guidCol) = NewGuid()
                    injected = True
                End If
            Next

            Return injected
        End Function

    End Module

End Namespace
