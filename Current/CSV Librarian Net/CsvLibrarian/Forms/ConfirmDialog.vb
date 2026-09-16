Imports System.Drawing
Imports System.Windows.Forms
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>Severity glyph shown on a <see cref="ConfirmDialog"/> (standard Windows icons).</summary>
    Public Enum DialogIcon
        None
        Information
        Warning
        [Error]
        Question
    End Enum

    ''' <summary>
    ''' Dark-themed replacement for the light Windows <c>MessageBox</c>, used app-wide.
    '''  • <see cref="Notify"/> — single OK button (informational).
    '''  • <see cref="Ask"/> — two buttons; returns True on the affirmative.
    '''  • <see cref="Choose"/> — three buttons (e.g. Save / Discard / Cancel); returns
    '''    Yes / No / Cancel.
    ''' Shows an optional severity icon and sizes to its content (scrolls if very long).
    ''' </summary>
    Public Class ConfirmDialog
        Inherits Form

        Private ReadOnly _scroll As New Panel()
        Private ReadOnly _message As New Label()
        Private ReadOnly _iconBox As New PictureBox()

        ''' <summary>Buttons are laid out right-to-left in array order, so index 0 is the
        ''' rightmost (affirmative/default) button and the last is the Cancel/close button.</summary>
        Private Sub New(title As String, message As String, icon As DialogIcon,
                        texts As String(), results As DialogResult())
            Font = New Font("Segoe UI", 9.0F)
            Text = title
            FormBorderStyle = FormBorderStyle.FixedDialog
            StartPosition = FormStartPosition.CenterParent
            MinimizeBox = False
            MaximizeBox = False
            ShowInTaskbar = False
            BackColor = DarkTheme.FormBackground
            ForeColor = DarkTheme.TextNormal

            Const pad As Integer = 16, maxW As Integer = 560, maxContentH As Integer = 440
            Const btnH As Integer = 30, gap As Integer = 8, iconSize As Integer = 32, iconGap As Integer = 14

            Dim sysIcon = SystemIconFor(icon)
            Dim iconOffset = If(sysIcon IsNot Nothing, iconSize + iconGap, 0)

            ' Message: auto-size, wrapping at the available width; scrolls if very tall.
            _message.Font = Font
            _message.AutoSize = True
            _message.MaximumSize = New Size(maxW - 2 * pad - iconOffset, 0)
            _message.ForeColor = DarkTheme.TextNormal
            _message.Location = New Point(0, 0)
            _message.Text = message
            Dim msg = _message.PreferredSize

            Dim contentH = Math.Max(msg.Height, If(sysIcon IsNot Nothing, iconSize, 0))
            Dim shownH = Math.Min(contentH, maxContentH)
            Dim needScroll = contentH > maxContentH
            _scroll.AutoScroll = needScroll
            _scroll.Location = New Point(pad + iconOffset, pad)
            _scroll.Size = New Size(msg.Width + If(needScroll, SystemInformation.VerticalScrollBarWidth + 4, 0), shownH)
            _scroll.Controls.Add(_message)
            Controls.Add(_scroll)

            If sysIcon IsNot Nothing Then
                _iconBox.BackColor = DarkTheme.FormBackground
                _iconBox.Image = sysIcon.ToBitmap()
                _iconBox.SizeMode = PictureBoxSizeMode.CenterImage
                _iconBox.SetBounds(pad, pad, iconSize, iconSize)
                Controls.Add(_iconBox)
            End If

            Dim clientW = Math.Max(360, pad + iconOffset + _scroll.Width + pad)
            ClientSize = New Size(clientW, pad + shownH + pad + btnH + pad)

            ' Buttons: affirmative first (rightmost), laid out right-to-left.
            Dim btnY = ClientSize.Height - btnH - pad
            Dim x = ClientSize.Width - pad
            Dim firstBtn As Button = Nothing, lastBtn As Button = Nothing
            For i As Integer = 0 To texts.Length - 1
                Dim b As New Button() With {.Text = texts(i), .DialogResult = results(i)}
                Dim w = Math.Max(96, TextRenderer.MeasureText(texts(i), Font).Width + 28)
                b.SetBounds(x - w, btnY, w, btnH)
                x -= (w + gap)
                DarkTheme.StyleFlatButtons(b)
                Controls.Add(b)
                If firstBtn Is Nothing Then firstBtn = b
                lastBtn = b
            Next
            AcceptButton = firstBtn
            CancelButton = lastBtn
        End Sub

        Private Shared Function SystemIconFor(icon As DialogIcon) As Icon
            Select Case icon
                Case DialogIcon.Information : Return SystemIcons.Information
                Case DialogIcon.Warning : Return SystemIcons.Warning
                Case DialogIcon.[Error] : Return SystemIcons.Error
                Case DialogIcon.Question : Return SystemIcons.Question
                Case Else : Return Nothing
            End Select
        End Function

        ''' <summary>Two-button yes/no. Returns True only if the affirmative button was clicked.</summary>
        Public Shared Function Ask(owner As IWin32Window, title As String, message As String,
                                   Optional okText As String = "OK",
                                   Optional cancelText As String = "Cancel",
                                   Optional icon As DialogIcon = DialogIcon.Question) As Boolean
            Using dlg As New ConfirmDialog(title, message, icon,
                                           {okText, cancelText},
                                           {DialogResult.OK, DialogResult.Cancel})
                Return dlg.ShowDialog(owner) = DialogResult.OK
            End Using
        End Function

        ''' <summary>Single-OK informational dialog.</summary>
        Public Shared Sub Notify(owner As IWin32Window, title As String, message As String,
                                 Optional okText As String = "OK",
                                 Optional icon As DialogIcon = DialogIcon.Information)
            Using dlg As New ConfirmDialog(title, message, icon, {okText}, {DialogResult.OK})
                dlg.ShowDialog(owner)
            End Using
        End Sub

        ''' <summary>Three-button prompt (e.g. Save / Discard / Cancel). Returns
        ''' <see cref="DialogResult.Yes"/> (primary), <see cref="DialogResult.No"/> (secondary),
        ''' or <see cref="DialogResult.Cancel"/>.</summary>
        Public Shared Function Choose(owner As IWin32Window, title As String, message As String,
                                      yesText As String, noText As String,
                                      Optional cancelText As String = "Cancel",
                                      Optional icon As DialogIcon = DialogIcon.Warning) As DialogResult
            Using dlg As New ConfirmDialog(title, message, icon,
                                           {yesText, noText, cancelText},
                                           {DialogResult.Yes, DialogResult.No, DialogResult.Cancel})
                Return dlg.ShowDialog(owner)
            End Using
        End Function

    End Class

End Namespace
