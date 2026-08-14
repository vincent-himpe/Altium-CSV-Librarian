Imports System.Drawing
Imports System.Windows.Forms

Namespace Theme

    ''' <summary>Factory helpers for consistently styled dark-theme controls.</summary>
    Public Module UiFactory

        Public Enum ButtonKind
            Accent
            Ghost
            Success
            Danger
        End Enum

        Public Function MakeButton(text As String, kind As ButtonKind) As Button
            Dim b As New Button With {
                .Text = text,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .Margin = New Padding(3, 6, 3, 6),
                .Padding = New Padding(10, 5, 10, 5)
            }
            StyleButton(b, kind)
            Return b
        End Function

        ''' <summary>
        ''' Apply the dark-theme appearance to an existing button (e.g. one created
        ''' by the WinForms designer). Kept separate from creation so designer-built
        ''' controls can be themed at runtime.
        ''' </summary>
        Public Sub StyleButton(b As Button, kind As ButtonKind)
            b.FlatStyle = FlatStyle.Flat
            b.UseVisualStyleBackColor = False
            b.Cursor = Cursors.Hand
            b.FlatAppearance.BorderSize = 1

            Select Case kind
                Case ButtonKind.Accent
                    b.BackColor = Palette.Accent
                    b.ForeColor = Color.White
                    b.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
                    b.FlatAppearance.BorderColor = Palette.Accent
                    b.FlatAppearance.MouseOverBackColor = Palette.AccentDark
                    b.FlatAppearance.MouseDownBackColor = Palette.AccentDark
                Case ButtonKind.Success
                    b.BackColor = Palette.ColorFromHex("#1a3a2a")
                    b.ForeColor = Palette.Success
                    b.Font = New Font("Segoe UI", 9.0F)
                    b.FlatAppearance.BorderColor = Palette.Border
                    b.FlatAppearance.MouseOverBackColor = Palette.ColorFromHex("#1f4a35")
                Case ButtonKind.Danger
                    b.BackColor = Palette.Surface
                    b.ForeColor = Palette.Danger
                    b.Font = New Font("Segoe UI", 9.0F)
                    b.FlatAppearance.BorderColor = Palette.Border
                    b.FlatAppearance.MouseOverBackColor = Palette.ColorFromHex("#2a1a1a")
                Case Else ' Ghost
                    b.BackColor = Palette.Surface
                    b.ForeColor = Palette.TextMuted
                    b.Font = New Font("Segoe UI", 9.0F)
                    b.FlatAppearance.BorderColor = Palette.Border
                    b.FlatAppearance.MouseOverBackColor = Palette.Panel
            End Select
        End Sub

        Public Function MakeLabel(text As String, fore As Color, Optional bold As Boolean = False,
                                  Optional size As Single = 9.0F,
                                  Optional family As String = "Segoe UI") As Label
            Return New Label With {
                .Text = text,
                .AutoSize = True,
                .ForeColor = fore,
                .BackColor = Color.Transparent,
                .Font = New Font(family, size, If(bold, FontStyle.Bold, FontStyle.Regular)),
                .Margin = New Padding(3)
            }
        End Function

        ''' <summary>Style a TextBox for the dark theme.</summary>
        Public Sub StyleTextBox(tb As TextBox)
            tb.BackColor = Palette.Panel
            tb.ForeColor = Palette.TextColor
            tb.BorderStyle = BorderStyle.FixedSingle
            tb.Font = New Font("Consolas", 9.5F)
        End Sub

    End Module

End Namespace
