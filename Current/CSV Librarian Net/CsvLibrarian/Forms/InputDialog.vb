Imports System.Drawing
Imports System.Windows.Forms
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>A glyph icon shown on an <see cref="InputDialog"/> (Segoe Fluent Icons, light blue).</summary>
    Public Enum InputIcon
        None
        Find      ' E721 — magnifier
    End Enum

    ''' <summary>
    ''' Dark-themed single-line text prompt — the themed replacement for VB's InputBox.
    ''' Optionally shows a light-blue glyph icon. <see cref="Prompt"/> returns the entered
    ''' text on OK, or Nothing on Cancel / Esc / close. Extend by adding to <see cref="InputIcon"/>
    ''' and <see cref="GlyphFor"/>.
    ''' </summary>
    Public Class InputDialog
        Inherits Form

        Private ReadOnly _iconLabel As New Label()
        Private ReadOnly _prompt As New Label()
        Private ReadOnly _box As New TextBox()
        Private ReadOnly _okButton As New Button()
        Private ReadOnly _cancelButton As New Button()

        Private Sub New(title As String, promptText As String, defaultText As String,
                        icon As InputIcon, okText As String, cancelText As String)
            Font = New Font("Segoe UI", 9.0F)
            Text = title
            FormBorderStyle = FormBorderStyle.FixedDialog
            StartPosition = FormStartPosition.CenterParent
            MinimizeBox = False
            MaximizeBox = False
            ShowInTaskbar = False
            BackColor = DarkTheme.FormBackground
            ForeColor = DarkTheme.TextNormal

            Const pad As Integer = 16, contentW As Integer = 380
            Const btnH As Integer = 30, gap As Integer = 8, iconSize As Integer = 32, iconGap As Integer = 12
            Dim glyph = GlyphFor(icon)
            Dim iconOffset = If(glyph IsNot Nothing, iconSize + iconGap, 0)
            Dim contentLeft = pad + iconOffset

            _prompt.AutoSize = True
            _prompt.MaximumSize = New Size(contentW, 0)
            _prompt.ForeColor = DarkTheme.TextNormal
            _prompt.Location = New Point(contentLeft, pad)
            _prompt.Text = promptText
            Controls.Add(_prompt)
            Dim promptH = If(String.IsNullOrEmpty(promptText), 0, _prompt.PreferredSize.Height)

            Dim tbTop = pad + promptH + If(promptH > 0, 8, 0)
            _box.BorderStyle = BorderStyle.FixedSingle
            _box.BackColor = DarkTheme.RowPrimary
            _box.ForeColor = DarkTheme.TextNormal
            _box.Text = If(defaultText, "")
            _box.SetBounds(contentLeft, tbTop, contentW, 24)
            _box.SelectAll()
            Controls.Add(_box)

            If glyph IsNot Nothing Then
                Dim iconTop = pad + Math.Max(0, ((_box.Bottom - pad) - iconSize) \ 2)
                _iconLabel.AutoSize = False
                _iconLabel.Size = New Size(iconSize, iconSize)
                _iconLabel.Location = New Point(pad, iconTop)
                _iconLabel.TextAlign = ContentAlignment.MiddleCenter
                _iconLabel.Font = DarkTheme.GlyphFont(18.0F, FontStyle.Bold)
                _iconLabel.ForeColor = DarkTheme.IconBlue
                _iconLabel.Text = glyph
                Controls.Add(_iconLabel)
            End If

            ClientSize = New Size(contentLeft + contentW + pad, _box.Bottom + 16 + btnH + pad)

            ' Buttons bottom-right: [Cancel] [OK].
            Dim btnY = ClientSize.Height - btnH - pad
            _okButton.Text = okText
            _okButton.DialogResult = DialogResult.OK
            Dim okW = Math.Max(96, TextRenderer.MeasureText(okText, Font).Width + 28)
            _okButton.SetBounds(ClientSize.Width - pad - okW, btnY, okW, btnH)
            _cancelButton.Text = cancelText
            _cancelButton.DialogResult = DialogResult.Cancel
            Dim cancelW = Math.Max(96, TextRenderer.MeasureText(cancelText, Font).Width + 28)
            _cancelButton.SetBounds(_okButton.Left - gap - cancelW, btnY, cancelW, btnH)
            DarkTheme.StyleFlatButtons(_okButton, _cancelButton)
            Controls.Add(_okButton)
            Controls.Add(_cancelButton)

            AcceptButton = _okButton
            CancelButton = _cancelButton
        End Sub

        Private Shared Function GlyphFor(icon As InputIcon) As String
            Select Case icon
                Case InputIcon.Find : Return ChrW(&HE721).ToString()
                Case Else : Return Nothing
            End Select
        End Function

        ''' <summary>Show the prompt; returns the entered text on OK, or Nothing on Cancel.</summary>
        Public Shared Function Prompt(owner As IWin32Window, title As String, promptText As String,
                                      Optional defaultText As String = "",
                                      Optional icon As InputIcon = InputIcon.None,
                                      Optional okText As String = "OK",
                                      Optional cancelText As String = "Cancel") As String
            Using dlg As New InputDialog(title, promptText, defaultText, icon, okText, cancelText)
                If dlg.ShowDialog(owner) = DialogResult.OK Then Return dlg._box.Text
                Return Nothing
            End Using
        End Function

    End Class

End Namespace
