Imports System.Data
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports CsvLibrarian.Forms
Imports CsvLibrarian.Services

Namespace Global.CsvLibrarian

    Friend Module Program

        <STAThread()>
        Friend Sub Main(args As String())
            ' Headless smoke test:  CsvLibrarian.exe --selftest <folder> [reportPath]
            If args IsNot Nothing AndAlso args.Length >= 2 AndAlso
               String.Equals(args(0), "--selftest", StringComparison.OrdinalIgnoreCase) Then
                Dim report = If(args.Length >= 3, args(2), Path.Combine(args(1), "selftest-report.txt"))
                SelfTest(args(1), report)
                Return
            End If

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            Dim startFolder As String = Nothing
            If args IsNot Nothing AndAlso args.Length > 0 Then
                startFolder = args(0)
            End If

            Application.Run(New MainForm(startFolder))
        End Sub

        ''' <summary>
        ''' Exercises the core pipeline (read → inject GUIDs → normalize → write)
        ''' against every CSV in a folder and writes a plain-text report.
        ''' </summary>
        Private Sub SelfTest(folder As String, reportPath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("CSV Librarian self-test")
            sb.AppendLine("Folder: " & folder)
            sb.AppendLine(New String("-"c, 60))

            Dim config = RuleStore.CreateDefaultConfig()

            For Each csvPath In Directory.GetFiles(folder, "*.csv")
                Dim name = Path.GetFileName(csvPath)
                Try
                    Dim table = CsvService.ReadCsv(csvPath)
                    sb.AppendLine($"{name}: {table.Rows.Count} rows, {table.Columns.Count} cols")
                    sb.AppendLine("  columns: " & String.Join(", ",
                        table.Columns.Cast(Of DataColumn).Select(Function(c) c.ColumnName)))

                    Dim injected = GuidService.InjectGuids(table)
                    sb.AppendLine("  GUIDs injected: " & injected.ToString())

                    Dim rs = NormalizerService.FindRuleSet(config, name)
                    If rs IsNot Nothing Then
                        Dim result = NormalizerService.Apply(table, rs)
                        If result.HasMissing Then
                            sb.AppendLine("  normalize: MISSING COLUMNS -> " & String.Join(", ", result.MissingColumns))
                        Else
                            sb.AppendLine($"  normalize: {result.ChangedCells} cell(s) changed")
                        End If
                    Else
                        sb.AppendLine("  normalize: no matching rule")
                    End If

                    ' Round-trip write to a temp copy, re-read, and compare row/col counts.
                    Dim tmp = Path.Combine(Path.GetTempPath(), "csvlib_" & name)
                    CsvService.WriteCsv(tmp, table)
                    Dim reread = CsvService.ReadCsv(tmp)
                    Dim ok = (reread.Rows.Count = table.Rows.Count AndAlso reread.Columns.Count = table.Columns.Count)
                    sb.AppendLine($"  round-trip: {reread.Rows.Count} rows, {reread.Columns.Count} cols -> {(If(ok, "OK", "MISMATCH"))}")
                    File.Delete(tmp)
                Catch ex As Exception
                    sb.AppendLine($"  ERROR: {ex.Message}")
                End Try
                sb.AppendLine()
            Next

            File.WriteAllText(reportPath, sb.ToString(), New UTF8Encoding(False))
        End Sub

    End Module

End Namespace
