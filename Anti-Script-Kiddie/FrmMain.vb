Imports System.Management
Imports System.Security.Cryptography.X509Certificates
Imports Shell32
Imports System.IO
Imports System.Diagnostics

Public Class FrmMain
#Region "Variables"
    Const Quote As String = """"
    Private _AllUsersStartup As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\"
    Private _CurrentUserStartup As String = Environment.GetFolderPath(Environment.SpecialFolder.Programs) &
        "\Startup\"
#End Region

#Region "Entry Point"
    Sub New()
        InitializeComponent()
        LvSet()
    End Sub

    Private Sub LvSet()
        LV.View = View.Details
        LV.FullRowSelect = True
        LV.MultiSelect = False
        LV.Columns.Add("Cpation", 200)
        LV.Columns.Add("Image Path", 500)
        LV.Columns.Add("Signed", 100)
        Width = LV.Columns(0).Width + LV.Columns(1).Width + LV.Columns(2).Width + 50
    End Sub
#End Region

#Region "Core"
    Private Sub GetStartup()
        LV.Items.Clear()
        Dim Managements As New ManagementClass("Win32_StartupCommand")
        Dim ManagementObjectCollections As ManagementObjectCollection = Managements.GetInstances()
        For Each mItem In ManagementObjectCollections
            Dim strArray() As String = {mItem("Caption").ToString, GetPath(mItem("Command").ToString),
                                GetSignature(GetPath(mItem("Command").ToString))}

            LV.Items.Add(New ListViewItem(strArray))
            If (strArray(2) = "Unsigned") Then
                LV.Items(LV.Items.Count - 1).BackColor = Color.Pink
            End If
        Next
    End Sub

    Private Function GetPath(Command As String) As String
        If (Command.IndexOf(".exe")) > 0 Then
            Command = Command.Replace(Quote, String.Empty)
            Command = Command.Substring(0, Command.IndexOf(".exe") + 1)
            Return Command + "exe"

        ElseIf (Command.IndexOf(".lnk") > 0) Then

            If (File.Exists(_AllUsersStartup & Command)) Then
                Command = _AllUsersStartup & Command
                Return GetShortcutPath(Command)
            Else
                Command = _CurrentUserStartup & "\" & Command
                Return GetShortcutPath(Command)
            End If

        Else
            Command = Command.Replace(Quote, String.Empty)
            Command = Command.Replace("/", String.Empty)
            Command = Command.Replace("-", String.Empty)
            Return Command
        End If

    End Function

    Private Function GetShortcutPath(Command As String)
        Dim directoryPath As String = Path.GetDirectoryName(Command)
        Dim fileName As String = Path.GetFileName(Command)
        Dim s As New Shell()
        Dim folder As Folder = s.NameSpace(directoryPath)
        Dim folderItem As FolderItem = folder.ParseName(fileName)
        If (folderItem IsNot Nothing) Then
            Dim link As ShellLinkObject = DirectCast(folderItem.GetLink, ShellLinkObject)
            Return link.Path
        End If
        Return String.Empty
    End Function

    Private Function GetSignature(Path As String) As String
        Dim theCertificate As X509Certificate2
        Dim chainIsValid As Boolean = False
        Dim theCertificateChain = New X509Chain()

        Try
            Dim theSigner As X509Certificate = X509Certificate.CreateFromSignedFile(Path)
            theCertificate = New X509Certificate2(theSigner)
        Catch ex As Exception
            Return "Unsigned"
        End Try

        theCertificateChain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot
        theCertificateChain.ChainPolicy.RevocationMode = X509RevocationMode.Online
        theCertificateChain.ChainPolicy.UrlRetrievalTimeout = New TimeSpan(0, 1, 0)
        theCertificateChain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag
        chainIsValid = theCertificateChain.Build(theCertificate)

        If chainIsValid Then
            Return "Signed"
        Else
            Return "Unsigned"
        End If
    End Function
#End Region

#Region "Threat Control"
    Private Function ProcessKill(processName As String) As Boolean
        Try

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
#End Region

#Region "UI Events"
    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles BtnScan.Click
        GetStartup()
    End Sub
#End Region

End Class
