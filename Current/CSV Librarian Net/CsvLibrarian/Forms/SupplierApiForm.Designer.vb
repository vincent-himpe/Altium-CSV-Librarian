Namespace Forms

    Partial Class SupplierApiForm
        Inherits System.Windows.Forms.Form

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            grpDigikey = New GroupBox()
            lblDigikeyClientId = New Label()
            txtDigikeyClientId = New TextBox()
            lblDigikeyAccessToken = New Label()
            txtDigikeyAccessToken = New TextBox()
            grpMouser = New GroupBox()
            lblMouserApiKey = New Label()
            txtMouserApiKey = New TextBox()
            grpLcsc = New GroupBox()
            lblLcscApiKey = New Label()
            txtLcscApiKey = New TextBox()
            lblLcscApiSecret = New Label()
            txtLcscApiSecret = New TextBox()
            grpTme = New GroupBox()
            lblTmeApiToken = New Label()
            txtTmeApiToken = New TextBox()
            okBtn = New Button()
            cancelBtn = New Button()
            grpDigikey.SuspendLayout()
            grpMouser.SuspendLayout()
            grpLcsc.SuspendLayout()
            grpTme.SuspendLayout()
            SuspendLayout()
            '
            ' grpDigikey
            '
            grpDigikey.Controls.Add(lblDigikeyClientId)
            grpDigikey.Controls.Add(txtDigikeyClientId)
            grpDigikey.Controls.Add(lblDigikeyAccessToken)
            grpDigikey.Controls.Add(txtDigikeyAccessToken)
            grpDigikey.Location = New Point(12, 12)
            grpDigikey.Name = "grpDigikey"
            grpDigikey.Size = New Size(356, 92)
            grpDigikey.TabIndex = 0
            grpDigikey.TabStop = False
            grpDigikey.Text = "Digikey"
            '
            ' lblDigikeyClientId
            '
            lblDigikeyClientId.AutoSize = True
            lblDigikeyClientId.Location = New Point(14, 29)
            lblDigikeyClientId.Name = "lblDigikeyClientId"
            lblDigikeyClientId.Size = New Size(55, 15)
            lblDigikeyClientId.TabIndex = 0
            lblDigikeyClientId.Text = "Client ID"
            '
            ' txtDigikeyClientId
            '
            txtDigikeyClientId.Location = New Point(110, 26)
            txtDigikeyClientId.Name = "txtDigikeyClientId"
            txtDigikeyClientId.Size = New Size(232, 23)
            txtDigikeyClientId.TabIndex = 1
            '
            ' lblDigikeyAccessToken
            '
            lblDigikeyAccessToken.AutoSize = True
            lblDigikeyAccessToken.Location = New Point(14, 61)
            lblDigikeyAccessToken.Name = "lblDigikeyAccessToken"
            lblDigikeyAccessToken.Size = New Size(80, 15)
            lblDigikeyAccessToken.TabIndex = 2
            lblDigikeyAccessToken.Text = "Access Token"
            '
            ' txtDigikeyAccessToken
            '
            txtDigikeyAccessToken.Location = New Point(110, 58)
            txtDigikeyAccessToken.Name = "txtDigikeyAccessToken"
            txtDigikeyAccessToken.Size = New Size(232, 23)
            txtDigikeyAccessToken.TabIndex = 3
            '
            ' grpMouser
            '
            grpMouser.Controls.Add(lblMouserApiKey)
            grpMouser.Controls.Add(txtMouserApiKey)
            grpMouser.Location = New Point(12, 112)
            grpMouser.Name = "grpMouser"
            grpMouser.Size = New Size(356, 62)
            grpMouser.TabIndex = 1
            grpMouser.TabStop = False
            grpMouser.Text = "Mouser"
            '
            ' lblMouserApiKey
            '
            lblMouserApiKey.AutoSize = True
            lblMouserApiKey.Location = New Point(14, 29)
            lblMouserApiKey.Name = "lblMouserApiKey"
            lblMouserApiKey.Size = New Size(48, 15)
            lblMouserApiKey.TabIndex = 0
            lblMouserApiKey.Text = "API Key"
            '
            ' txtMouserApiKey
            '
            txtMouserApiKey.Location = New Point(110, 26)
            txtMouserApiKey.Name = "txtMouserApiKey"
            txtMouserApiKey.Size = New Size(232, 23)
            txtMouserApiKey.TabIndex = 1
            '
            ' grpLcsc
            '
            grpLcsc.Controls.Add(lblLcscApiKey)
            grpLcsc.Controls.Add(txtLcscApiKey)
            grpLcsc.Controls.Add(lblLcscApiSecret)
            grpLcsc.Controls.Add(txtLcscApiSecret)
            grpLcsc.Location = New Point(12, 182)
            grpLcsc.Name = "grpLcsc"
            grpLcsc.Size = New Size(356, 92)
            grpLcsc.TabIndex = 2
            grpLcsc.TabStop = False
            grpLcsc.Text = "LCSC"
            '
            ' lblLcscApiKey
            '
            lblLcscApiKey.AutoSize = True
            lblLcscApiKey.Location = New Point(14, 29)
            lblLcscApiKey.Name = "lblLcscApiKey"
            lblLcscApiKey.Size = New Size(48, 15)
            lblLcscApiKey.TabIndex = 0
            lblLcscApiKey.Text = "API Key"
            '
            ' txtLcscApiKey
            '
            txtLcscApiKey.Location = New Point(110, 26)
            txtLcscApiKey.Name = "txtLcscApiKey"
            txtLcscApiKey.Size = New Size(232, 23)
            txtLcscApiKey.TabIndex = 1
            '
            ' lblLcscApiSecret
            '
            lblLcscApiSecret.AutoSize = True
            lblLcscApiSecret.Location = New Point(14, 61)
            lblLcscApiSecret.Name = "lblLcscApiSecret"
            lblLcscApiSecret.Size = New Size(62, 15)
            lblLcscApiSecret.TabIndex = 2
            lblLcscApiSecret.Text = "API Secret"
            '
            ' txtLcscApiSecret
            '
            txtLcscApiSecret.Location = New Point(110, 58)
            txtLcscApiSecret.Name = "txtLcscApiSecret"
            txtLcscApiSecret.Size = New Size(232, 23)
            txtLcscApiSecret.TabIndex = 3
            '
            ' grpTme
            '
            grpTme.Controls.Add(lblTmeApiToken)
            grpTme.Controls.Add(txtTmeApiToken)
            grpTme.Location = New Point(12, 282)
            grpTme.Name = "grpTme"
            grpTme.Size = New Size(356, 62)
            grpTme.TabIndex = 3
            grpTme.TabStop = False
            grpTme.Text = "TME"
            '
            ' lblTmeApiToken
            '
            lblTmeApiToken.AutoSize = True
            lblTmeApiToken.Location = New Point(14, 29)
            lblTmeApiToken.Name = "lblTmeApiToken"
            lblTmeApiToken.Size = New Size(60, 15)
            lblTmeApiToken.TabIndex = 0
            lblTmeApiToken.Text = "API Token"
            '
            ' txtTmeApiToken
            '
            txtTmeApiToken.Location = New Point(110, 26)
            txtTmeApiToken.Name = "txtTmeApiToken"
            txtTmeApiToken.Size = New Size(232, 23)
            txtTmeApiToken.TabIndex = 1
            '
            ' okBtn
            '
            okBtn.Location = New Point(196, 356)
            okBtn.Name = "okBtn"
            okBtn.Size = New Size(84, 28)
            okBtn.TabIndex = 4
            okBtn.Text = "OK"
            okBtn.UseVisualStyleBackColor = True
            '
            ' cancelBtn
            '
            cancelBtn.DialogResult = DialogResult.Cancel
            cancelBtn.Location = New Point(284, 356)
            cancelBtn.Name = "cancelBtn"
            cancelBtn.Size = New Size(84, 28)
            cancelBtn.TabIndex = 5
            cancelBtn.Text = "Cancel"
            cancelBtn.UseVisualStyleBackColor = True
            '
            ' SupplierApiForm
            '
            AcceptButton = okBtn
            CancelButton = cancelBtn
            ClientSize = New Size(380, 398)
            Controls.Add(grpDigikey)
            Controls.Add(grpMouser)
            Controls.Add(grpLcsc)
            Controls.Add(grpTme)
            Controls.Add(okBtn)
            Controls.Add(cancelBtn)
            FormBorderStyle = FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            Name = "SupplierApiForm"
            StartPosition = FormStartPosition.CenterParent
            Text = "Supplier API Integration"
            grpDigikey.ResumeLayout(False)
            grpDigikey.PerformLayout()
            grpMouser.ResumeLayout(False)
            grpMouser.PerformLayout()
            grpLcsc.ResumeLayout(False)
            grpLcsc.PerformLayout()
            grpTme.ResumeLayout(False)
            grpTme.PerformLayout()
            ResumeLayout(False)
        End Sub

        Friend WithEvents grpDigikey As System.Windows.Forms.GroupBox
        Friend WithEvents lblDigikeyClientId As System.Windows.Forms.Label
        Friend WithEvents txtDigikeyClientId As System.Windows.Forms.TextBox
        Friend WithEvents lblDigikeyAccessToken As System.Windows.Forms.Label
        Friend WithEvents txtDigikeyAccessToken As System.Windows.Forms.TextBox
        Friend WithEvents grpMouser As System.Windows.Forms.GroupBox
        Friend WithEvents lblMouserApiKey As System.Windows.Forms.Label
        Friend WithEvents txtMouserApiKey As System.Windows.Forms.TextBox
        Friend WithEvents grpLcsc As System.Windows.Forms.GroupBox
        Friend WithEvents lblLcscApiKey As System.Windows.Forms.Label
        Friend WithEvents txtLcscApiKey As System.Windows.Forms.TextBox
        Friend WithEvents lblLcscApiSecret As System.Windows.Forms.Label
        Friend WithEvents txtLcscApiSecret As System.Windows.Forms.TextBox
        Friend WithEvents grpTme As System.Windows.Forms.GroupBox
        Friend WithEvents lblTmeApiToken As System.Windows.Forms.Label
        Friend WithEvents txtTmeApiToken As System.Windows.Forms.TextBox
        Friend WithEvents okBtn As System.Windows.Forms.Button
        Friend WithEvents cancelBtn As System.Windows.Forms.Button
    End Class

End Namespace
