Imports System.Windows.Forms
Imports CsvLibrarian.Services

Namespace Forms

    ''' <summary>
    ''' Editor for the persisted <see cref="AppSettings"/> (Altium CSV Librarian
    ''' Settings.json): autosave interval and the autosave / auto-normalize /
    ''' archive-on-exit toggles. OK writes the values back into the shared settings
    ''' object and saves it; Cancel leaves everything unchanged. Standard Windows
    ''' colour scheme (no theming).
    ''' </summary>
    Public Class OptionsForm

        Private ReadOnly _settings As AppSettings

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
            _settings = New AppSettings()
        End Sub

        Public Sub New(settings As AppSettings)
            InitializeComponent()
            _settings = settings
            txtAutosave.Text = _settings.AutosaveSeconds.ToString()
            chkAutosaveEnabled.Checked = _settings.AutosaveEnabled
            chkAutoNormalize.Checked = _settings.AutoNormalizeOnSave
            chkArchiveExit.Checked = _settings.ArchiveOnExit
        End Sub

        Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
            Dim secs As Integer
            If Not Integer.TryParse(txtAutosave.Text.Trim(), secs) OrElse secs < 5 Then
                MessageBox.Show(Me, "Enter the autosave interval as a whole number of seconds (5 or more).",
                                "Options", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            _settings.AutosaveSeconds = secs
            _settings.AutosaveEnabled = chkAutosaveEnabled.Checked
            _settings.AutoNormalizeOnSave = chkAutoNormalize.Checked
            _settings.ArchiveOnExit = chkArchiveExit.Checked

            Try
                _settings.Save()
            Catch ex As Exception
                MessageBox.Show(Me, "Could not save the settings:" & vbCrLf & ex.Message,
                                "Options", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return   ' keep the window open
            End Try

            Me.DialogResult = DialogResult.OK
        End Sub

    End Class

End Namespace
