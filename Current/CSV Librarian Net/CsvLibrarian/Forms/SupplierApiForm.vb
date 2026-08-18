Imports System.Windows.Forms
Imports CsvLibrarian.Services

Namespace Forms

    ''' <summary>
    ''' Editor for per-supplier API credentials (Digikey, Mouser, LCSC, TME), stored in
    ''' <see cref="AppSettings.SupplierApi"/> in the settings JSON. OK writes the values
    ''' back into the shared settings object and saves; Cancel leaves them unchanged.
    ''' Fixed-size dialog, standard Windows colour scheme.
    ''' </summary>
    Public Class SupplierApiForm

        Private ReadOnly _settings As AppSettings

        ''' <summary>Parameterless constructor for the Windows Forms designer.</summary>
        Public Sub New()
            InitializeComponent()
            _settings = New AppSettings()
        End Sub

        Public Sub New(settings As AppSettings)
            InitializeComponent()
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
                MessageBox.Show(Me, "Could not save the API settings:" & vbCrLf & ex.Message,
                                "Supplier API Integration", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return   ' keep the window open
            End Try

            Me.DialogResult = DialogResult.OK
        End Sub

    End Class

End Namespace
