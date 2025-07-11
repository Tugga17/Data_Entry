Imports System.Net
Imports Microsoft.SharePoint.Client
Imports System.Data.SqlClient
Imports Entergy.PerformanceManagement.PMDB
Imports Telerik.WinControls


Public Class SharepointUtilities
    Public Sub UploadDocumentToSharepoint(ByVal localFile As String, ByVal remoteFile As String, ByVal spURL As String, ByVal docLib As String)
        '// Read in the local file
        Dim r As Byte()
        Dim Strm As System.IO.FileStream = New System.IO.FileStream(localFile, System.IO.FileMode.Open, System.IO.FileAccess.Read)
        Dim reader As System.IO.BinaryReader = New System.IO.BinaryReader(Strm)
        Dim filecontents As Byte() = reader.ReadBytes(CInt(Strm.Length))
        reader.Close()
        Strm.Close()
        Dim sRemoteFileURL As String

        Dim NC As System.Net.NetworkCredential = System.Net.CredentialCache.DefaultNetworkCredentials

        sRemoteFileURL = spURL & "/" & docLib & "/" & Trim(LTrim(RTrim(remoteFile)))
        sRemoteFileURL = Replace(sRemoteFileURL, " ", "%20")
        sRemoteFileURL = Replace(sRemoteFileURL, "\", "/")

        Dim m_WC As WebClient = New WebClient
        m_WC.Credentials = NC
        Try
            r = m_WC.UploadData(sRemoteFileURL, "PUT", filecontents)
        Catch ex As Exception
            RadMessageBox.Show("Could not write file " + localFile + " to " + sRemoteFileURL + " to Sharepoint. Please copy by hand or see if directory exists in Sharepoint and try again. " + ex.Message)
        End Try


    End Sub
    Dim connString As String = "Server=prfdatad;Database=Perfmgr;User ID=perfmgr_dev;Password=perfmgr;Trusted_Connection=no"

    Public Sub UpdateSharepointScorecardList()
        Dim spList As List(Of ScorecardsDTO) = Me.GetSharepointScorecardList
        Dim dbList As List(Of ScorecardsDTO) = Me.GetDatabaseScorecardList

        For Each dbItem As ScorecardsDTO In dbList
            Dim found As Boolean = False
            For Each spItem As ScorecardsDTO In spList
                If dbItem.sharepointID = spItem.sharepointID Then
                    found = True
                    Me.UpdateSharepointScorecardList(dbItem)
                End If
            Next
            If Not found Then
                dbItem.sharepointID = Me.InsertSharepointScorecardList(dbItem)
                Me.UpdateDatabaseScorecardList(dbItem)
            End If
        Next
        RadMessageBox.Show("List Updated Sucessfully")
    End Sub

    Public Function GetSharepointScorecardList() As List(Of ScorecardsDTO)
        Dim ret As New List(Of ScorecardsDTO)
        Dim context As New ClientContext("https://myentergy.entergy.com/sites/UPM")
        Dim list As List = context.Web.Lists.GetByTitle("AvailableMyScorecards")
        Dim query As New CamlQuery
        query.ViewXml = "<View/>"
        Dim items As ListItemCollection = list.GetItems(query)
        context.Load(list)
        context.Load(items)
        context.ExecuteQuery()

        For Each item As ListItem In items
            Dim dto As New ScorecardsDTO
            dto.scorecardName = item("Title")
            dto.sharepointID = item("ID")
            dto.Active = item("Active")
            ret.Add(dto)
        Next
        Return ret
    End Function

    Public Sub UpdateSharepointScorecardList(ByVal data As ScorecardsDTO)
        Dim context As New ClientContext("https://myentergy.entergy.com/sites/UPM")
        Dim list As List = context.Web.Lists.GetByTitle("AvailableMyScorecards")
        Dim query As New CamlQuery
        query.ViewXml = "<View><Query><Where><Contains><FieldRef Name='ID' /><Value Type='Number'>" + data.sharepointID.ToString + "</Value></Contains></Where></Query></View>"
        Dim items As ListItemCollection = list.GetItems(query)
        context.Load(list)
        context.Load(items)
        context.ExecuteQuery()

        For Each item As ListItem In items
            item("Title") = data.scorecardName
            item("Active") = data.Active
            item.Update()
            context.ExecuteQuery()
        Next
    End Sub

    Public Function InsertSharepointScorecardList(ByVal data As ScorecardsDTO) As Integer

        Dim ret As New List(Of ScorecardsDTO)
        Dim context As New ClientContext("https://myentergy.entergy.com/sites/UPM")
        Dim list As List = context.Web.Lists.GetByTitle("AvailableMyScorecards")
        Dim param As New ListItemCreationInformation

        Dim itemAdd As ListItem = list.AddItem(param)
        itemAdd("Title") = data.scorecardName
        itemAdd("Active") = data.Active

        itemAdd.Update()

        context.ExecuteQuery()

        Return itemAdd.Id

    End Function

    Public Function GetDatabaseScorecardList() As List(Of ScorecardsDTO)
        Dim ret As List(Of ScorecardsDTO)
        Dim db As New PMScorecardsDB
        ret = db.GetAllScorecards

        Return ret
    End Function
    Public Sub UpdateDatabaseScorecardList(ByVal dto As ScorecardsDTO)
        Dim db As New PMScorecardsDB
        db.SaveScorecards(dto)

    End Sub
End Class
