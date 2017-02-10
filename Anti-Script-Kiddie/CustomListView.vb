Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Public Class CustomListView
    Inherits ListView
#Region "Interop-Defines"
    <DllImport("user32.dll")>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wPar As IntPtr, lPar As IntPtr) As IntPtr
    End Function

    ' ListView messages
    Private Const LVM_FIRST As Integer = &H1000
    Private Const LVM_GETCOLUMNORDERARRAY As Integer = (LVM_FIRST + 59)

    ' Windows Messages
    Private Const WM_PAINT As Integer = &HF
#End Region

    ''' <summary>
    ''' Structure to hold an embedded control's info
    ''' </summary>
    Private Structure EmbeddedControl
        Public Control As Control
        Public Column As Integer
        Public Row As Integer
        Public Dock As DockStyle
        Public Item As ListViewItem
    End Structure

    Private _embeddedControls As New ArrayList()

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Retrieve the order in which columns appear
    ''' </summary>
    ''' <returns>Current display order of column indices</returns>
    Protected Function GetColumnOrder() As Integer()
        Dim lPar As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(GetType(Integer)) * Columns.Count)

        Dim res As IntPtr = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, New IntPtr(Columns.Count), lPar)
        If res.ToInt32() = 0 Then
            ' Something went wrong
            Marshal.FreeHGlobal(lPar)
            Return Nothing
        End If

        Dim order As Integer() = New Integer(Columns.Count - 1) {}
        Marshal.Copy(lPar, order, 0, Columns.Count)

        Marshal.FreeHGlobal(lPar)

        Return order
    End Function

    ''' <summary>
    ''' Retrieve the bounds of a ListViewSubItem
    ''' </summary>
    ''' <param name="Item">The Item containing the SubItem</param>
    ''' <param name="SubItem">Index of the SubItem</param>
    ''' <returns>Subitem's bounds</returns>
    Protected Function GetSubItemBounds(Item As ListViewItem, SubItem As Integer) As Rectangle
        Dim subItemRect As Rectangle = Rectangle.Empty

        If Item Is Nothing Then
            Throw New ArgumentNullException("Item")
        End If

        Dim order As Integer() = GetColumnOrder()
        If order Is Nothing Then
            ' No Columns
            Return subItemRect
        End If

        If SubItem >= order.Length Then
            Throw New IndexOutOfRangeException("SubItem " + SubItem + " out of range")
        End If

        ' Retrieve the bounds of the entire ListViewItem (all subitems)
        Dim lviBounds As Rectangle = Item.GetBounds(ItemBoundsPortion.Entire)
        Dim subItemX As Integer = lviBounds.Left

        ' Calculate the X position of the SubItem.
        ' Because the columns can be reordered we have to use Columns[order[i]] instead of Columns[i] !
        Dim col As ColumnHeader
        Dim i As Integer
        For i = 0 To order.Length - 1
            col = Me.Columns(order(i))
            If col.Index = SubItem Then
                Exit For
            End If
            subItemX += col.Width
        Next

        subItemRect = New Rectangle(subItemX, lviBounds.Top, Me.Columns(order(i)).Width, lviBounds.Height)

        Return subItemRect
    End Function

    ''' <summary>
    ''' Adicionar um controle para o ListView
    ''' </summary>
    ''' <param name="Controle">Controle para ser Adicionado</param>
    ''' <param name="IndexColuna">Index da Coluna</param>
    ''' <param name="IndexItem">Index do Item</param>
    ''' <param name="Dock">Localização e redimensionar comportamento do controle integrado</param>
    Public Sub AdicionarControle(Controle As Control, IndexColuna As Integer, IndexItem As Integer, Dock As DockStyle)
        If Controle Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If IndexColuna >= Columns.Count OrElse IndexItem >= Items.Count Then
            Throw New ArgumentOutOfRangeException()
        End If

        Dim ec As EmbeddedControl
        ec.Control = Controle
        ec.Column = IndexColuna
        ec.Row = IndexItem
        ec.Dock = Dock
        ec.Item = Items(IndexItem)

        _embeddedControls.Add(ec)

        ' Add a Click event handler to select the ListView row when an embedded control is clicked
        AddHandler Controle.Click, New EventHandler(AddressOf _embeddedControl_Click)

        Me.Controls.Add(Controle)
    End Sub

    ''' <summary>
    ''' Remove a control from the ListView
    ''' </summary>
    ''' <param name="c">Control to be removed</param>
    Public Sub RemoveEmbeddedControl(c As Control)
        If c Is Nothing Then
            Throw New ArgumentNullException()
        End If

        For i As Integer = 0 To _embeddedControls.Count - 1
            Dim ec As EmbeddedControl = CType(_embeddedControls(i), EmbeddedControl)
            If ec.Control Is c Then
                RemoveHandler c.Click, New EventHandler(AddressOf _embeddedControl_Click)
                Me.Controls.Remove(c)
                _embeddedControls.RemoveAt(i)
                Return
            End If
        Next
        Throw New Exception("Control not found!")
    End Sub

    ''' <summary>
    ''' Retrieve the control embedded at a given location
    ''' </summary>
    ''' <param name="col">Index of Column</param>
    ''' <param name="row">Index of Row</param>
    ''' <returns>Control found at given location or null if none assigned.</returns>
    Public Function GetEmbeddedControl(col As Integer, row As Integer) As Control
        For Each ec As EmbeddedControl In _embeddedControls
            If ec.Row = row AndAlso ec.Column = col Then
                Return ec.Control
            End If
        Next

        Return Nothing
    End Function

    <DefaultValue(View.LargeIcon)>
    Public Shadows Property View() As View
        Get
            Return MyBase.View
        End Get
        Set(value As View)
            ' Embedded controls are rendered only when we're in Details mode
            For Each ec As EmbeddedControl In _embeddedControls
                ec.Control.Visible = (value = View.Details)
            Next

            MyBase.View = value
        End Set
    End Property

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_PAINT
                If View <> View.Details Then
                    Exit Select
                End If

                ' Calculate the position of all embedded controls
                For Each ec As EmbeddedControl In _embeddedControls
                    Dim rc As Rectangle = Me.GetSubItemBounds(ec.Item, ec.Column)

                    If (Me.HeaderStyle <> ColumnHeaderStyle.None) AndAlso (rc.Top < Me.Font.Height) Then
                        ' Control overlaps ColumnHeader
                        ec.Control.Visible = False
                        Continue For
                    Else
                        ec.Control.Visible = True
                    End If

                    Select Case ec.Dock
                        Case DockStyle.Fill
                            Exit Select
                        Case DockStyle.Top
                            rc.Height = ec.Control.Height
                            Exit Select
                        Case DockStyle.Left
                            rc.Width = ec.Control.Width
                            Exit Select
                        Case DockStyle.Bottom
                            rc.Offset(0, rc.Height - ec.Control.Height)
                            rc.Height = ec.Control.Height
                            Exit Select
                        Case DockStyle.Right
                            rc.Offset(rc.Width - ec.Control.Width, 0)
                            rc.Width = ec.Control.Width
                            Exit Select
                        Case DockStyle.None
                            rc.Size = ec.Control.Size
                            Exit Select
                    End Select

                    ' Set embedded control's bounds
                    ec.Control.Bounds = rc
                Next
                Exit Select
        End Select
        MyBase.WndProc(m)
    End Sub

    Private Sub _embeddedControl_Click(sender As Object, e As EventArgs)
        ' When a control is clicked the ListViewItem holding it is selected
        For Each ec As EmbeddedControl In _embeddedControls
            If ec.Control Is DirectCast(sender, Control) Then
                Me.SelectedItems.Clear()
                ec.Item.Selected = True
            End If
        Next
    End Sub
End Class

