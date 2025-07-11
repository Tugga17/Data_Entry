Imports System.Configuration.ConfigurationManager
Public Class MyAppSettings
    'Public Shared Function GetReportingYear() As Integer
    '    Dim s As New My.MySettings
    '    Return s.ReportingYear
    'End Function
    Public Shared Function GetSharepointImportPathPowerOn() As String
        Return AppSettings("SharePointImportPowerOn")
    End Function
    Public Shared Function GetSharepointImportPathEnergyDelivery() As String
        Return AppSettings("SharePointImportEnergyDelivery")
    End Function
    Public Shared Function GetSharepointImportPathEpprentice() As String
        Return AppSettings("SharePointImportEpprentice")
    End Function
    Public Shared Function GetSharepointURL() As String
        Return AppSettings("SharepointURL")
    End Function
    Public Shared Function GetPowerOnDocumentLibrary() As String
        Return AppSettings("PowerOnDocumentLibrary")
    End Function
    Public Shared Function GetEnergyDeliveryDocumentLibrary() As String
        Return AppSettings("EnergyDeliveryDocumentLibrary")
    End Function
    Public Shared Function GetEpprenticeDocumentLibrary() As String
        Return AppSettings("EpprenticeDocumentLibrary")
    End Function
    Public Shared Function GetWebPath() As String
        Return AppSettings("WebPath")
    End Function
    Public Shared Function GetWebLink() As String
        Return AppSettings("WebLink")
    End Function
    Public Shared Function GetEpprenticePath() As String
        Return AppSettings("EpprenticePath")
    End Function
    Public Shared Function GetEpprenticeWebLink() As String
        Return AppSettings("EpprenticeWebLink")
    End Function
    Public Shared Function GetCustomerCountsPath() As String
        Return AppSettings("CustomerCountsPath")
    End Function
    Public Shared Function GetCCSCustomerCountsFile() As String
        Return AppSettings("CCSCustomerCountsFile")
    End Function
    Public Shared Function GetCISCustomerCountsFile() As String
        Return AppSettings("CISCustomerCountsFile")
    End Function
End Class
