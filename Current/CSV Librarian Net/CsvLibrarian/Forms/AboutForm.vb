Namespace Forms

    ''' <summary>Standard Windows "About" dialog.</summary>
    Public Class AboutForm

        Public Sub New()
            InitializeComponent()
            lblAppTitle.Text = "Altium CSV Librarian V " & AppInfo.Version
            lblAbout.Text = AppInfo.AboutString
        End Sub

    End Class

End Namespace
