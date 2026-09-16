Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Namespace Theme

    ''' <summary>
    ''' Native helpers for dark UI touches that WinForms doesn't expose as properties.
    ''' </summary>
    Friend Module NativeDark

        <DllImport("uxtheme.dll", ExactSpelling:=True, CharSet:=CharSet.Unicode)>
        Private Function SetWindowTheme(hWnd As IntPtr, pszSubAppName As String, pszSubIdList As String) As Integer
        End Function

        ''' <summary>
        ''' Give a control the Windows "dark" scrollbar style (dark track, grey thumb) —
        ''' Windows 10 1809+. No effect on older Windows. Best-effort; the control must
        ''' already have a window handle.
        ''' </summary>
        Friend Sub ApplyDarkScrollbars(ctrl As Control)
            If ctrl Is Nothing OrElse Not ctrl.IsHandleCreated Then Return
            Try
                SetWindowTheme(ctrl.Handle, "DarkMode_Explorer", Nothing)
            Catch
                ' Not available on this OS — leave the default scrollbars.
            End Try
        End Sub

        ''' <summary>
        ''' Turn on double buffering for a control (e.g. a DataGridView) via its protected
        ''' <c>DoubleBuffered</c> property, so repaints on scroll / row insert don't flicker
        ''' or lag on large tables. Reflection is used because the property isn't public;
        ''' this survives designer regeneration (no need to subclass the control).
        ''' </summary>
        Friend Sub EnableDoubleBuffer(ctrl As Control)
            If ctrl Is Nothing Then Return
            Try
                Dim prop = GetType(Control).GetProperty("DoubleBuffered",
                    BindingFlags.Instance Or BindingFlags.NonPublic)
                prop?.SetValue(ctrl, True, Nothing)
            Catch
                ' Best-effort — a partial-trust or restricted host may block reflection.
            End Try
        End Sub

    End Module

End Namespace
