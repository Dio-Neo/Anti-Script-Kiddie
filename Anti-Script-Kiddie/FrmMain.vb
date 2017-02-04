#Region "Imports Namespace"
Imports System.Management
Imports System.Security.Cryptography.X509Certificates
Imports Shell32
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions
Imports System.Diagnostics
#End Region

Public Class FrmMain
#Region "Variables"
    Const Quote As String = """"
    Private _mCSV As New CSVData
    Private _vtColumn As Integer = 0
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
        LvStartup.View = View.Details
        LvStartup.FullRowSelect = True
        LvStartup.MultiSelect = False
        LvStartup.Columns.Add("Cpation", 200)
        LvStartup.Columns.Add("Image Path", 500)
        LvStartup.Columns.Add("Signed", 100)
        LvResult.View = View.Details
        LvResult.FullRowSelect = True
        LvResult.MultiSelect = False
        LvResult.Columns.Add("Cpation", 200)
        LvResult.Columns.Add("Image Path", 500)
        LvResult.Columns.Add("Signed", 100)
        Width = LvStartup.Columns(0).Width + LvStartup.Columns(1).Width + LvStartup.Columns(2).Width + 50
    End Sub
#End Region

#Region "CSV"
    Private Sub LoadCSV(fileName As String, Optional viewOption As String = Nothing)
        On Error Resume Next
        _mCSV.Separator = ","
        _mCSV.TextQualifier = """"
        _mCSV.LoadCSV(fileName)
        LvResult.Clear()
        Dim dc As DataColumn
        Dim dr As DataRow
        Dim lvi As ListViewItem
        Dim idx As Integer
        For Each dc In _mCSV.CSVDataSet.Tables(0).Columns
            LvResult.Columns.Add(dc.ColumnName, 100, HorizontalAlignment.Left)
        Next
        For Each dr In _mCSV.CSVDataSet.Tables(0).Rows
            lvi = LvResult.Items.Add(dr(0))
            For idx = 1 To _mCSV.CSVDataSet.Tables(0).Columns.Count - 1
                If (dr(idx) = String.Empty) Then
                    lvi.SubItems.Add("Unknown")
                    lvi.BackColor = Color.Pink
                Else
                    lvi.SubItems.Add(dr(idx))
                End If
            Next
        Next
        Dim c As Integer = LvResult.Columns.Count, lvc As Integer = LvResult.Items.Count
        For i = 0 To c - 1
            LvResult.Columns(i).Text = LvResult.Items(0).SubItems(i).Text
            If LvResult.Items(0).SubItems(i).Text = "VT detection" Then
                _vtColumn = i
            End If
        Next
        LvResult.Items(0).Remove()
        'For i = 0 To lvc - 1
        '    If LvResult.Items(i).SubItems(ent).Text = "Unknown" Then
        '        LvResult.Items(i).BackColor = Color.Pink
        '    End If
        'Next

    End Sub

    Private Sub SaveCSV(fileName As String, pathToSave As String)
        On Error Resume Next
        File.WriteAllText(pathToSave, fileName)
    End Sub
#End Region

#Region "Core"
    Private Sub GetStartup()
        LvStartup.Items.Clear()
        Dim Managements As New ManagementClass("Win32_StartupCommand")
        Dim ManagementObjectCollections As ManagementObjectCollection = Managements.GetInstances()
        For Each mItem In ManagementObjectCollections
            Dim strStartupArray() As String = {mItem("Caption").ToString, GetPath(mItem("Command").ToString),
                                GetSignature(GetPath(mItem("Command").ToString))}

            LvStartup.Items.Add(New ListViewItem(strStartupArray))
            If (strStartupArray(2) = "Unsigned") Then
                LvStartup.Items(LvStartup.Items.Count - 1).BackColor = Color.Pink
                Dim strResultArray() As String = {mItem("Caption").ToString, GetPath(mItem("Command").ToString),
                                GetSignature(GetPath(mItem("Command").ToString))}
                LvResult.Items.Add(New ListViewItem(strResultArray))
                LvResult.Items(LvResult.Items.Count - 1).BackColor = Color.Red
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

    Private Function GetThreatInfo(Path As String) As String
        Return String.Empty
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

#Region "External Class"
Public Class CSVData
    Implements IDisposable

    Dim dsCSV As DataSet
    Dim mSeparator As Char = ","
    Dim mTextQualifier As Char = """"
    Dim mData() As String
    Dim mHeader As Boolean

    Private regQuote As New Regex("^(\x22)(.*)(\x22)(\s*,)(.*)$", RegexOptions.IgnoreCase + RegexOptions.RightToLeft)
    Private regNormal As New Regex("^([^,]*)(\s*,)(.*)$", RegexOptions.IgnoreCase + RegexOptions.RightToLeft)
    Private regQuoteLast As New Regex("^(\x22)([\x22*]{2,})(\x22)$", RegexOptions.IgnoreCase)
    Private regNormalLast As New Regex("^.*$", RegexOptions.IgnoreCase)

    Protected Disposed As Boolean

#Region " Load CSV "
    '
    ' Load CSV
    '
    Public Sub LoadCSV(ByVal CSVFile As String)
        LoadCSV(CSVFile, False)
    End Sub

    '
    ' Load CSV - Has Header
    '
    Public Sub LoadCSV(ByVal CSVFile As String, ByVal HasHeader As Boolean)
        On Error Resume Next
        mHeader = HasHeader
        SetupRegEx()

        If File.Exists(CSVFile) = False Then
            Throw New Exception(CSVFile & " does not exist.")
        End If

        If Not dsCSV Is Nothing Then
            dsCSV.Clear()
            dsCSV.Tables.Clear()
            dsCSV.Dispose()
            dsCSV = Nothing
        End If

        dsCSV = New DataSet("CSV")
        dsCSV.Tables.Add("CSVData")

        Dim sr As New StreamReader(CSVFile)
        Dim idx As Integer
        Dim bFirstLine As Boolean = True
        Dim dr As DataRow

        Do While sr.Peek > -1
            ProcessLine(sr.ReadLine())

            '
            ' Create Columns
            '
            If bFirstLine = True Then
                For idx = 0 To mData.GetUpperBound(0)
                    If mHeader = True Then
                        dsCSV.Tables("CSVData").Columns.Add(mData(idx), GetType(String))
                    Else
                        dsCSV.Tables("CSVData").Columns.Add("Column" & idx, GetType(String))
                    End If
                Next
            End If

            '
            ' Add Data
            '
            If Not (bFirstLine = True And mHeader = True) Then
                dr = dsCSV.Tables("CSVData").NewRow()

                For idx = 0 To mData.GetUpperBound(0)
                    dr(idx) = mData(idx)
                Next

                dsCSV.Tables("CSVData").Rows.Add(dr)
                dsCSV.AcceptChanges()
            End If

            bFirstLine = False
        Loop

        sr.Close()
    End Sub

    '
    ' Load CSV with custom separator
    '
    Public Sub LoadCSV(ByVal CSVFile As String, ByVal Separator As Char)
        LoadCSV(CSVFile, Separator, False)
    End Sub

    '
    ' Load CSV with custom separator and Has Header
    '
    Public Sub LoadCSV(ByVal CSVFile As String, ByVal Separator As Char, ByVal HasHeader As Boolean)
        mSeparator = Separator
        Try
            LoadCSV(CSVFile, HasHeader)
        Catch ex As Exception
            Throw New Exception("CSV Error", ex)
        End Try
    End Sub

    '
    ' Load CSV with custom separator and text qualifier
    '
    Public Sub LoadCSV(ByVal CSVFile As String, ByVal Separator As Char, ByVal TxtQualifier As Char)
        LoadCSV(CSVFile, Separator, TxtQualifier, False)
    End Sub

    '
    ' Load CSV with custom separator and text qualifier
    '
    Public Sub LoadCSV(ByVal CSVFile As String, ByVal Separator As Char, ByVal TxtQualifier As Char, ByVal HasHeader As Boolean)
        mSeparator = Separator
        mTextQualifier = TxtQualifier
        Try
            LoadCSV(CSVFile, HasHeader)
        Catch ex As Exception
            Throw New Exception("CSV Error", ex)
        End Try
    End Sub
#End Region
#Region " Process Line "
    '
    ' Process Line
    '
    Private Sub ProcessLine(ByVal sLine As String)
        Dim sData As String
        Dim m As Match
        Dim idx As Integer
        Dim mc As MatchCollection

        Erase mData
        sLine = sLine.Replace(ControlChars.Tab, "    ") 'Replace tab with 4 spaces
        sLine = sLine.Trim

        Do While sLine.Length > 0
            sData = ""

            If regQuote.IsMatch(sLine) Then
                mc = regQuote.Matches(sLine)
                '
                ' "text",<rest of the line>
                '
                m = regQuote.Match(sLine)
                sData = m.Groups(2).Value
                sLine = m.Groups(5).Value
            ElseIf regQuoteLast.IsMatch(sLine) Then
                '
                ' "text"
                '
                m = regQuoteLast.Match(sLine)
                sData = m.Groups(2).Value
                sLine = ""
            ElseIf regNormal.IsMatch(sLine) Then
                '
                ' text,<rest of the line>
                '
                m = regNormal.Match(sLine)
                sData = m.Groups(1).Value
                sLine = m.Groups(3).Value
            ElseIf regNormalLast.IsMatch(sLine) Then
                '
                ' text
                '
                m = regNormalLast.Match(sLine)
                sData = m.Groups(0).Value
                sLine = ""
            Else
                '
                ' ERROR!!!!!
                '
                sData = ""
                sLine = ""
            End If

            sData = sData.Trim
            sLine = sLine.Trim

            If mData Is Nothing Then
                ReDim mData(0)
                idx = 0
            Else
                idx = mData.GetUpperBound(0) + 1
                ReDim Preserve mData(idx)
            End If

            mData(idx) = sData
        Loop
    End Sub
#End Region
#Region " Regular Expressions "
    '
    ' Set up Regular Expressions
    '
    Private Sub SetupRegEx()
        Dim sQuote As String = "^(%Q)(.*)(%Q)(\s*%S)(.*)$"
        Dim sNormal As String = "^([^%S]*)(\s*%S)(.*)$"
        Dim sQuoteLast As String = "^(%Q)(.*)(%Q$)"
        Dim sNormalLast As String = "^.*$"
        Dim sSep As String
        Dim sQual As String

        If Not regQuote Is Nothing Then regQuote = Nothing
        If Not regNormal Is Nothing Then regNormal = Nothing
        If Not regQuoteLast Is Nothing Then regQuoteLast = Nothing
        If Not regNormalLast Is Nothing Then regNormalLast = Nothing

        sSep = mSeparator
        sQual = mTextQualifier

        If InStr(".$^{[(|)]}*+?\", sSep) > 0 Then sSep = "\" & sSep
        If InStr(".$^{[(|)]}*+?\", sQual) > 0 Then sQual = "\" & sQual

        sQuote = sQuote.Replace("%S", sSep)
        sQuote = sQuote.Replace("%Q", sQual)
        sNormal = sNormal.Replace("%S", sSep)
        sQuoteLast = sQuoteLast.Replace("%Q", sQual)

        regQuote = New Regex(sQuote, RegexOptions.IgnoreCase + RegexOptions.RightToLeft)
        regNormal = New Regex(sNormal, RegexOptions.IgnoreCase + RegexOptions.RightToLeft)
        regQuoteLast = New Regex(sQuoteLast, RegexOptions.IgnoreCase + RegexOptions.RightToLeft)
        regNormalLast = New Regex(sNormalLast, RegexOptions.IgnoreCase + RegexOptions.RightToLeft)
    End Sub
#End Region
#Region " Save As "
    '
    ' Save data as XML
    '
    Public Sub SaveAsXML(ByVal sXMLFile As String)
        If dsCSV Is Nothing Then Exit Sub
        dsCSV.WriteXml(sXMLFile)
    End Sub

    '
    ' Save data as CSV
    '
    Public Sub SaveAsCSV(ByVal sCSVFile As String)
        If dsCSV Is Nothing Then Exit Sub

        Dim dr As DataRow
        Dim sLine As String
        Dim sw As New StreamWriter(sCSVFile)
        Dim iCol As Integer

        For Each dr In dsCSV.Tables("CSVData").Rows
            sLine = ""
            For iCol = 0 To dsCSV.Tables("CSVData").Columns.Count - 1
                If sLine.Length > 0 Then sLine &= mSeparator
                If Not dr(iCol) Is DBNull.Value Then
                    If InStr(dr(iCol), mSeparator) > 0 Then
                        sLine &= mTextQualifier & dr(iCol) & mTextQualifier
                    Else
                        sLine &= dr(iCol)
                    End If
                End If
            Next

            sw.WriteLine(sLine)
        Next

        sw.Flush()
        sw.Close()
        sw = Nothing
    End Sub
#End Region
#Region " Properties "
    '
    ' Separator Property
    '
    Public Property Separator() As Char
        Get
            Return mSeparator
        End Get
        Set(ByVal Value As Char)
            mSeparator = Value
            SetupRegEx()
        End Set
    End Property

    '
    ' Qualifier Property
    '
    Public Property TextQualifier() As Char
        Get
            Return mTextQualifier
        End Get
        Set(ByVal Value As Char)
            mTextQualifier = Value
            SetupRegEx()
        End Set
    End Property

    '
    ' Dataset Property
    '
    Public ReadOnly Property CSVDataSet() As DataSet
        Get
            Return dsCSV
        End Get
    End Property
#End Region
#Region " Dispose and Finalize "
    '
    ' Dispose
    '
    Public Sub Dispose() Implements System.IDisposable.Dispose
        Dispose(True)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Disposed Then Exit Sub

        If disposing Then
            Disposed = True

            GC.SuppressFinalize(Me)
        End If

        If Not dsCSV Is Nothing Then
            dsCSV.Clear()
            dsCSV.Tables.Clear()
            dsCSV.Dispose()
            dsCSV = Nothing
        End If
    End Sub

    '
    ' Finalize
    '
    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub
#End Region
End Class

#End Region