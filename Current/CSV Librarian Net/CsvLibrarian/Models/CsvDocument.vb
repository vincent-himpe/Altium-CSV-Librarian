Imports System.Data
Imports System.IO

Namespace Models

    ''' <summary>
    ''' One open CSV file: its on-disk path, the in-memory <see cref="DataTable"/>
    ''' backing the grid, and its unsaved-changes flag.
    ''' The first column is always treated as the read-only GUID column.
    ''' </summary>
    Public Class CsvDocument

        Public Property Path As String

        Public Property Table As DataTable

        Public Property Dirty As Boolean

        Public ReadOnly Property FileName As String
            Get
                Return System.IO.Path.GetFileName(Path)
            End Get
        End Property

        Public Sub New(path As String, table As DataTable)
            Me.Path = path
            Me.Table = table
            Me.Dirty = False
        End Sub

        ''' <summary>Name of the GUID column (first column), or Nothing if empty.</summary>
        Public ReadOnly Property GuidColumnName As String
            Get
                If Table Is Nothing OrElse Table.Columns.Count = 0 Then Return Nothing
                Return Table.Columns(0).ColumnName
            End Get
        End Property

    End Class

End Namespace
