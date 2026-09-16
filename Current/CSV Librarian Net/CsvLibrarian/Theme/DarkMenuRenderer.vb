Imports System.Drawing
Imports System.Windows.Forms

Namespace Theme

    ''' <summary>Dark colour table for menus / dropdowns / context menus. Literal
    ''' colours (no Palette abstraction) matching the app's Altium-style dark scheme.</summary>
    Friend Class DarkMenuColors
        Inherits ProfessionalColorTable

        Private Shared ReadOnly Bar As Color = Color.FromArgb(45, 45, 48)
        Private Shared ReadOnly DropDown As Color = Color.FromArgb(45, 45, 48)
        Private Shared ReadOnly Hover As Color = Color.FromArgb(61, 92, 135)
        Private Shared ReadOnly BorderC As Color = Color.FromArgb(62, 62, 66)
        Private Shared ReadOnly Sep As Color = Color.FromArgb(70, 70, 74)

        Public Overrides ReadOnly Property MenuStripGradientBegin As Color
            Get
                Return Bar
            End Get
        End Property
        Public Overrides ReadOnly Property MenuStripGradientEnd As Color
            Get
                Return Bar
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
            Get
                Return DropDown
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
            Get
                Return DropDown
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
            Get
                Return DropDown
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
            Get
                Return DropDown
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelected As Color
            Get
                Return Hover
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
            Get
                Return Hover
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
            Get
                Return Hover
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
            Get
                Return DropDown
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
            Get
                Return DropDown
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemBorder As Color
            Get
                Return Hover
            End Get
        End Property
        Public Overrides ReadOnly Property MenuBorder As Color
            Get
                Return BorderC
            End Get
        End Property
        Public Overrides ReadOnly Property SeparatorDark As Color
            Get
                Return Sep
            End Get
        End Property
        Public Overrides ReadOnly Property SeparatorLight As Color
            Get
                Return Sep
            End Get
        End Property
        Public Overrides ReadOnly Property CheckBackground As Color
            Get
                Return Hover
            End Get
        End Property
        Public Overrides ReadOnly Property CheckSelectedBackground As Color
            Get
                Return Hover
            End Get
        End Property
    End Class

    ''' <summary>ToolStrip renderer that paints menus dark with light text and a blue
    ''' hover/selection. Assign via <c>ToolStripManager.Renderer</c> to cover the menu
    ''' bar, all dropdowns, and context menus.</summary>
    Friend Class DarkMenuRenderer
        Inherits ToolStripProfessionalRenderer

        Public Sub New()
            MyBase.New(New DarkMenuColors())
            Me.RoundedEdges = False
        End Sub

        Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
            If Not e.Item.Enabled Then
                e.TextColor = Color.FromArgb(120, 120, 124)
            ElseIf e.Item.Selected OrElse e.Item.Pressed Then
                e.TextColor = Color.White
            Else
                e.TextColor = Color.FromArgb(220, 220, 220)
            End If
            MyBase.OnRenderItemText(e)
        End Sub

        Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
            e.ArrowColor = Color.FromArgb(220, 220, 220)
            MyBase.OnRenderArrow(e)
        End Sub
    End Class

End Namespace
