Imports Microsoft.Office.Interop

Public Class myExcel
    Implements IDisposable
    Private _wbList As New List(Of Excel.Workbook)
    Private _wsList As New List(Of Excel.Worksheet)
    Private WithEvents _ap As New Excel.Application
    Public ReadOnly Property ExcelApplication() As Excel.Application
        Get
            Return _ap
        End Get
    End Property


    Private disposedValue As Boolean = False        ' To detect redundant calls

    Private Sub TearDownExcel()
        Try
            For Each workbook As Excel.Workbook In _wbList
                For Each worksheet As Excel.Worksheet In _wsList
                    NAR(worksheet)
                Next
                workbook.Close(False)
                NAR(workbook)
            Next
            _ap.Workbooks.Close()
            NAR(_ap.Workbooks)
            'quit and dispose app
            _ap.Quit()
            NAR(_ap)
            _wbList.Clear()
            _wsList.Clear()
            'VERY IMPORTANT
            GC.Collect()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub RegisterWorkBooks(ByVal Wb As Excel.Workbook, ByVal Sh As Object) Handles _ap.WorkbookNewSheet
        _wbList.Add(Wb)
    End Sub


    Private Sub NAR(ByVal o As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(o)
        Catch
        Finally
            o = Nothing
        End Try
    End Sub

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.TearDownExcel()
            End If

            ' TODO: free shared unmanaged resources
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
