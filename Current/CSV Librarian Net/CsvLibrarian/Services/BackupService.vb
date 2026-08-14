Imports System.IO
Imports System.Text.RegularExpressions

Namespace Services

    ''' <summary>One backup file on disk, with the timestamp encoded in its name.</summary>
    Public Class BackupInfo
        Public Property Path As String
        Public Property Timestamp As DateTime

        ''' <summary>Display text shown in the toolbar dropdown.</summary>
        Public Overrides Function ToString() As String
            Return Timestamp.ToString("yyyy/MM/dd - HH:mm")
        End Function
    End Class

    ''' <summary>
    ''' Copies a file into a ".backups" sub-folder before it is overwritten,
    ''' keeping only the most recent <see cref="MaxBackups"/> versions per file.
    ''' </summary>
    Public Module BackupService

        Public Const BackupFolderName As String = ".backups"
        Public Const MaxBackups As Integer = 5

        ''' <summary>
        ''' Enumerate the backups that exist for <paramref name="path"/>, newest first.
        ''' Returns an empty list if the file has no backups yet.
        ''' </summary>
        Public Function ListBackups(path As String) As List(Of BackupInfo)
            Dim result As New List(Of BackupInfo)()
            If String.IsNullOrEmpty(path) Then Return result

            Dim dir As String = System.IO.Path.GetDirectoryName(path)
            If String.IsNullOrEmpty(dir) Then Return result
            Dim backupDir As String = System.IO.Path.Combine(dir, BackupFolderName)
            If Not Directory.Exists(backupDir) Then Return result

            Dim stem As String = System.IO.Path.GetFileNameWithoutExtension(path)
            Dim ext As String = System.IO.Path.GetExtension(path)
            ' Capture the timestamp group; tolerate the "-N" same-second collision suffix.
            Dim pattern As New Regex(
                "^" & Regex.Escape(stem) & "\.(\d{8}-\d{6})(-\d+)?" & Regex.Escape(ext) & "$",
                RegexOptions.IgnoreCase)

            For Each fi In New DirectoryInfo(backupDir).GetFiles()
                Dim m = pattern.Match(fi.Name)
                If Not m.Success Then Continue For
                Dim ts As DateTime
                If DateTime.TryParseExact(m.Groups(1).Value, "yyyyMMdd-HHmmss",
                                          System.Globalization.CultureInfo.InvariantCulture,
                                          System.Globalization.DateTimeStyles.None, ts) Then
                    result.Add(New BackupInfo With {.Path = fi.FullName, .Timestamp = ts})
                End If
            Next

            ' Newest first; break same-second ties by name so the order is stable.
            result.Sort(Function(a, b)
                            Dim c = b.Timestamp.CompareTo(a.Timestamp)
                            If c <> 0 Then Return c
                            Return String.Compare(b.Path, a.Path, StringComparison.Ordinal)
                        End Function)
            Return result
        End Function

        ''' <summary>
        ''' Copy <paramref name="path"/> to
        ''' .backups/{stem}.{yyyyMMdd-HHmmss}{ext}, then prune old backups.
        ''' No-op if the source file does not yet exist.
        ''' </summary>
        Public Sub MakeBackup(path As String)
            If Not File.Exists(path) Then Return

            Dim dir As String = System.IO.Path.GetDirectoryName(path)
            Dim backupDir As String = System.IO.Path.Combine(dir, BackupFolderName)
            Directory.CreateDirectory(backupDir)

            Dim stem As String = System.IO.Path.GetFileNameWithoutExtension(path)
            Dim ext As String = System.IO.Path.GetExtension(path)
            Dim ts As String = DateTime.Now.ToString("yyyyMMdd-HHmmss")
            Dim dest As String = System.IO.Path.Combine(backupDir, stem & "." & ts & ext)

            ' Avoid clobbering a backup made in the same second.
            Dim counter As Integer = 1
            While File.Exists(dest)
                dest = System.IO.Path.Combine(backupDir, stem & "." & ts & "-" & counter.ToString() & ext)
                counter += 1
            End While

            File.Copy(path, dest, overwrite:=False)

            PruneBackups(backupDir, stem, ext)
        End Sub

        Private Sub PruneBackups(backupDir As String, stem As String, ext As String)
            ' Match {stem}.{timestamp}{ext} (timestamp may carry a "-N" collision suffix).
            Dim pattern As New Regex(
                "^" & Regex.Escape(stem) & "\.\d{8}-\d{6}(-\d+)?" & Regex.Escape(ext) & "$",
                RegexOptions.IgnoreCase)

            Dim backups = New DirectoryInfo(backupDir).GetFiles().
                Where(Function(fi) pattern.IsMatch(fi.Name)).
                OrderBy(Function(fi) fi.Name, StringComparer.Ordinal).
                ToList()

            Dim removeCount As Integer = backups.Count - MaxBackups
            For i As Integer = 0 To removeCount - 1
                Try
                    backups(i).Delete()
                Catch
                    ' Ignore individual delete failures (file locked, etc.).
                End Try
            Next
        End Sub

    End Module

End Namespace
