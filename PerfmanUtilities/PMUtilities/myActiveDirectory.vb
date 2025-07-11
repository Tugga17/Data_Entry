Imports System.DirectoryServices
Imports System.Text

Public Class myActiveDirectory

    Private DomainNameValue As String
    Private GroupNameValue As String
    Public Sub New(ByVal DomainName As String, ByVal GroupName As String)

        DomainNameValue = DomainName
        GroupNameValue = GroupName

    End Sub



    Public Function ReturnUsersDT() As DataTable

        Dim strDirEntryPath As String
        strDirEntryPath = "WinNT://" + DomainNameValue + "/" + GroupNameValue + ",group"
        Dim users As Object
        Dim group As New DirectoryEntry(strDirEntryPath)

        users = group.Invoke("members")

        Dim ADuser As Object

        Dim ActiveDirTable As DataTable
        ActiveDirTable = New DataTable("UserList")
        Dim UserID As DataColumn = New DataColumn("UserID")
        Dim UserName As DataColumn = New DataColumn("UserName")
        UserID.DataType = System.Type.GetType("System.String")
        UserName.DataType = System.Type.GetType("System.String")
        ActiveDirTable.Columns.Add(UserID)
        ActiveDirTable.Columns.Add(UserName)

        For Each ADuser In CType(users, IEnumerable)

            Dim userEntry As New System.DirectoryServices.DirectoryEntry(ADuser)
            Dim fullName As String = userEntry.Name
            Dim myNewRow As DataRow
            myNewRow = ActiveDirTable.NewRow()
            myNewRow("UserID") = userEntry.Name
            myNewRow("UserName") = fullName
            ActiveDirTable.Rows.Add(myNewRow)

        Next
        Return ActiveDirTable

    End Function

    Public Function ReturnUserList() As List(Of String)
        Dim strDirEntryPath As String
        strDirEntryPath = "WinNT://" + DomainNameValue + "/" + GroupNameValue + ",group"
        Dim users As Object
        Dim group As New DirectoryEntry(strDirEntryPath)
        Dim list As New List(Of String)

        users = group.Invoke("members")
        For Each ADuser As Object In CType(users, IEnumerable)
            Dim userEntry As New System.DirectoryServices.DirectoryEntry(ADuser)
            list.Add(userEntry.Name)
        Next
        Return list
    End Function

    Public Function ReturnUsersAL(ByVal appendDomain As Boolean) As String

        Dim strDirEntryPath As String
        strDirEntryPath = "WinNT://" + DomainNameValue + "/" + GroupNameValue + ",group"
        Dim users As Object
        Dim group As New DirectoryEntry(strDirEntryPath)

        users = group.Invoke("members")

        Dim ADuser As Object
        Dim tmpUserList As New StringBuilder

        For Each ADuser In CType(users, IEnumerable)

            Dim userEntry As New System.DirectoryServices.DirectoryEntry(ADuser)

            If appendDomain Then
                tmpUserList.Append(userEntry.Name & "@" & DomainNameValue & ";")
            Else
                tmpUserList.Append(userEntry.Name & ";")
            End If
        Next

        Return tmpUserList.ToString

    End Function

    Public Function GetEmail(ByVal userID As String) As String
        Dim dsDirectoryEntry As DirectoryEntry
        Dim dsSearch As DirectorySearcher
        Dim result As SearchResultCollection

        Dim aName As String = userID
        dsDirectoryEntry = New DirectoryEntry("WinNT://" + DomainNameValue + "/")
        dsSearch = New DirectorySearcher(dsDirectoryEntry)
        With dsSearch
            .Filter = "(samAccountName=" & aName & ")"
            .PropertyNamesOnly = True
            .PropertiesToLoad.Add("mail")
            result = .FindAll()
        End With

        If result.Count = 1 Then
            Return "the email for " & aName & " is " & result.ToString
        Else
            Return String.Empty
        End If

    End Function

    Public Function GetUserInfo(ByVal username As String, ByVal pwd As String) As String

        Dim strRealName As String = ""
        If username = "" Or username = Nothing Then

            strRealName = "Invalid Signature"

        Else

            pwd = Nothing ' works better if pwd is nothing 
            Dim domain As String = DomainNameValue
            Dim path As String = "LDAP://" + domain
            Dim domainAndUsername As String = domain + "\" + username
            Dim entry As DirectoryEntry = New DirectoryEntry(path, domainAndUsername, pwd)
            Dim Searcher As DirectorySearcher = New DirectorySearcher(entry)
            Dim result As System.DirectoryServices.SearchResult
            Searcher.Filter = ("(anr=" & username & ")")
            result = Searcher.FindOne()
            If Not IsNothing(result) Then

                strRealName = result.Properties("givenName")(0).ToString() & " " & result.Properties("sn")(0).ToString()

            End If

        End If
        Return strRealName

    End Function

End Class
