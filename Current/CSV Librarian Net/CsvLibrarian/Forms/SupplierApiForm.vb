Imports System.Drawing
Imports System.Windows.Forms
Imports CsvLibrarian.Services
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>
    ''' Editor for per-supplier API credentials (Digikey, Mouser, LCSC, TME), stored in
    ''' <see cref="AppSettings.SupplierApi"/> in the settings JSON. OK writes the values
    ''' back into the shared settings object and saves; Cancel leaves them unchanged.
    ''' Fixed-size dialog, dark colour scheme.
    ''' </summary>
    Public Class SupplierApiForm

        Private ReadOnly _settings As AppSettings

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
            StyleDark()
            _settings = New AppSettings()
        End Sub

        Public Sub New(settings As AppSettings)
            InitializeComponent()
            StyleDark()
            _settings = settings
            If _settings.SupplierApi Is Nothing Then _settings.SupplierApi = New SupplierApiSettings()

            Dim api = _settings.SupplierApi
            txtDigikeyClientId.Text = If(api.DigikeyClientId, "")
            txtDigikeyAccessToken.Text = If(api.DigikeyAccessToken, "")
            txtMouserApiKey.Text = If(api.MouserApiKey, "")
            txtLcscApiKey.Text = If(api.LcscApiKey, "")
            txtLcscApiSecret.Text = If(api.LcscApiSecret, "")
            txtTmeApiToken.Text = If(api.TmeApiToken, "")
        End Sub

        ''' <summary>Dark group captions + text fields + flat dark buttons.</summary>
        Private Sub StyleDark()
            For Each g As GroupBox In {grpDigikey, grpMouser, grpLcsc, grpTme}
                g.ForeColor = DarkTheme.TextNormal
            Next
            For Each t As TextBox In {txtDigikeyClientId, txtDigikeyAccessToken, txtMouserApiKey,
                                      txtLcscApiKey, txtLcscApiSecret, txtTmeApiToken}
                t.BackColor = DarkTheme.RowPrimary
                t.ForeColor = DarkTheme.TextNormal
                t.BorderStyle = BorderStyle.FixedSingle
            Next
            DarkTheme.StyleFlatButtons(okBtn, cancelBtn)
        End Sub

        Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
            Dim api = _settings.SupplierApi
            api.DigikeyClientId = txtDigikeyClientId.Text.Trim()
            api.DigikeyAccessToken = txtDigikeyAccessToken.Text.Trim()
            api.MouserApiKey = txtMouserApiKey.Text.Trim()
            api.LcscApiKey = txtLcscApiKey.Text.Trim()
            api.LcscApiSecret = txtLcscApiSecret.Text.Trim()
            api.TmeApiToken = txtTmeApiToken.Text.Trim()

            Try
                _settings.Save()
            Catch ex As Exception
                ConfirmDialog.Notify(Me, "Supplier API Integration",
                    "Could not save the API settings:" & vbCrLf & ex.Message, icon:=DialogIcon.Error)
                Return   ' keep the window open
            End Try

            Me.DialogResult = DialogResult.OK
        End Sub

    End Class

End Namespace
