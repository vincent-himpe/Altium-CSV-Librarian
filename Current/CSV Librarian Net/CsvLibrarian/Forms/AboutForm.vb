Imports System.Drawing
Imports System.Windows.Forms
Imports CsvLibrarian.Theme

Namespace Forms

    ''' <summary>Dark-themed "About" dialog.</summary>
    Public Class AboutForm

        Public Sub New()
            InitializeComponent()
            lblAppTitle.Text = "Altium CSV Librarian V " & AppInfo.Version
            lblAbout.Text = AppInfo.AboutString

            DarkTheme.StyleFlatButtons(btnOk)   ' visual-styled buttons ignore BackColor
        End Sub

    End Class

End Namespace
