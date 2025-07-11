Imports Telerik.WinControls
Imports Telerik.WinControls.UI
Imports Entergy.PerformanceManagement.PMDB
Imports Entergy.PerformanceManagement.PMDTO
Imports System.Configuration.ConfigurationManager
Imports Entergy.PerformanceManagement.PMUtilities
Public Class DataEntryGrid
    Inherits RadForm
    Private _presenter As New myPresenter
    Private Sub DataEntryGrid_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.SetUpMetricGrid() 'build the metric grid 

        'Dim SecUGDB As New SecurityUserGroupsDB
        'Dim SecUser As List(Of SecurityUsersGroupsDTO) = SecUGDB.GetSecurityUserGroups(Environment.UserName)
        Dim seclevel As String = String.Empty

        Dim SecUGDB As New SecurityUserGroupsDB
        Dim SecurityUserAdmin As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "Admin")
        Dim SecurityUserPWRUPM As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRUPM")
        Dim SecurityUserPWRSYS As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "PWRSYS")
        Dim SecurityUserDP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "DP")
        Dim SecurityUserBDP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "BDP")
        Dim SecurityUserCP As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "CP")
        Dim SecurityUserSysViewer As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "SysViewer")
        Dim SecurityUserRptViewer As Boolean = SecUGDB.IsUserInSecurityGroup(Environment.UserName, "RptViewer")

        If SecurityUserAdmin = True Or SecurityUserPWRUPM = True Then
            Me.LoadMetrics()
        ElseIf SecurityUserPWRSYS = True Then
            LoadMetricsBySYS()
        ElseIf SecurityUserDP = True And SecurityUserBDP = True And SecurityUserCP = True Then
            LoadMetricsByDP_BDP_CP()
        ElseIf SecurityUserDP = True And SecurityUserBDP = False And SecurityUserCP = True Then
            LoadMetricsByDP_CP()
        ElseIf SecurityUserDP = True And SecurityUserBDP = True And SecurityUserCP = False Then
            LoadMetricsByDP_BDP()
        ElseIf SecurityUserDP = True And SecurityUserBDP = False And SecurityUserCP = False Then
            Me.LoadMetricsByDP()
        ElseIf SecurityUserBDP = True And SecurityUserDP = False And SecurityUserCP = False Then
            Me.LoadMetricsByBDP()
        ElseIf SecurityUserCP = True Then
            Me.LoadMetricsByCP()
            Me.rmiCalComp.Visibility = ElementVisibility.Collapsed
            Me.rmiCalCompPE.Visibility = ElementVisibility.Collapsed
            Me.rmiCarryOver.Visibility = ElementVisibility.Collapsed
        ElseIf SecurityUserSysViewer = True Then

        ElseIf SecurityUserRptViewer = True Then

        End If
    End Sub

    Public Sub SetUpMetricGrid()
        rgvDEMetrics.MasterGridViewTemplate.AutoGenerateColumns = False
        rgvDEMetrics.MasterGridViewTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill
        rgvDEMetrics.MasterGridViewTemplate.AllowAddNewRow = False
        rgvDEMetrics.MasterGridViewTemplate.AllowEditRow = False
        rgvDEMetrics.MasterGridViewTemplate.EnableFiltering = True
        rgvDEMetrics.MasterGridViewTemplate.EnableGrouping = False
        rgvDEMetrics.EnableAlternatingRowColor = True

        rgvDEMetrics.Columns.Clear()
        Dim col As New GridViewTextBoxColumn("MetricID", "MetricID")
        col.Width = 75
        col.HeaderText = "Metric ID"
        col.AllowFiltering = True
        rgvDEMetrics.Columns.Add(col)

        Dim col2 As New GridViewTextBoxColumn("MetricName", "MetricName")
        col2.Width = 300
        col2.HeaderText = "Metric Name"
        rgvDEMetrics.Columns.Add(col2)

        Dim col3 As New GridViewTextBoxColumn("PMContactID", "PMContactID")
        col3.Width = 50
        col3.HeaderText = "Contact"
        rgvDEMetrics.Columns.Add(col3)

        Dim col4 As New GridViewTextBoxColumn("MetricOwnerUserID", "MetricOwnerUserID")
        col4.Width = 50
        col4.HeaderText = "Owner"
        rgvDEMetrics.Columns.Add(col4)

        Dim col5 As New GridViewCheckBoxColumn("Active", "Active")
        col5.Width = 50
        col5.HeaderText = "Active"
        rgvDEMetrics.Columns.Add(col5)

        Dim col8 As New GridViewCheckBoxColumn("CompositeMetric", "CompositeMetric")
        col8.Width = 60
        col8.HeaderText = "Composite"
        col8.AllowFiltering = True
        rgvDEMetrics.Columns.Add(col8)

        Dim col9 As New GridViewTextBoxColumn("ManagedByDesc", "ManagedByDesc")
        col9.Width = 75
        col9.HeaderText = "Managed By"
        col9.AllowFiltering = True
        rgvDEMetrics.Columns.Add(col9)

        Dim col10 As New GridViewCheckBoxColumn("IncentiveMeasure", "IncentiveMeasure")
        col10.Width = 50
        col10.HeaderText = "Incentive"
        rgvDEMetrics.Columns.Add(col10)
    End Sub
    Private Sub LoadMetricsByBDP()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsByBDP(Environment.UserName)
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetricsByCP()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsByCP(Environment.UserName)
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetricsByDP()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsByDP(Environment.UserName)
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetricsByDP_BDP()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsByDP_BDP(Environment.UserName)
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetricsByDP_CP()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsByDP_CP(Environment.UserName)
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetricsByDP_BDP_CP()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsByDP_BDP_CP(Environment.UserName)
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetricsBySYS()
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetricsBySYS()
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetrics()
        Dim db As New PMMetricsDB
        Dim dbT As New PMTargetTrackingDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetrics()
        _presenter.LoadGridView(rgvDEMetrics, list)
    End Sub
    Private Sub LoadMetric(ByVal CID As Integer)
        Dim db As New PMMetricsDB
        Dim list As List(Of MetricDTO) = db.GetUPMMetrics(CID)
        _presenter.LoadGridView(rgvDEMetrics, list)
        '_metric.MetricData.
    End Sub

    Private Sub rgvDEMetrics_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rgvDEMetrics.DoubleClick
        If rgvDEMetrics.SelectedRows.Count = 0 Or rgvDEMetrics.SelectedRows.Count > 1 Then Exit Sub
        'Dim f As New DataEntry
        'If f.ShowDialog() = Windows.Forms.DialogResult.OK Then
        Dim edit As New DataEntry
        edit.Metric = rgvDEMetrics.SelectedRows(0).DataBoundItem
        edit.ShowDialog()
        'Me.LoadMetrics()
        'End If
    End Sub

    Private Sub rmiCalComp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rmiCalComp.Click
        Dim f As New PMActualsCriteria
        Dim db As New CompositeMetricDB
        Dim order As List(Of Integer) = db.GetCompositeCalculationOrder

        f.EnableOpCo = False
        If f.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim month = f.ReportCriteria.ReportingMonth
            Dim year = f.ReportCriteria.ReportingYear
            For Each cID As Integer In order
                For Each item As GridViewRowInfo In rgvDEMetrics.SelectedRows
                    Dim chart As MetricDTO = item.DataBoundItem
                    If chart.CompositeMetric And chart.CID = cID Then
                        Dim c As New CompositeWorker(chart.CID)
                        Try
                            c.ComputeActuals(month, year)
                            RadMessageBox.Show("Computations Complete.")
                        Catch ex As Exception
                            RadMessageBox.Show(ex.Message)
                        End Try
                    End If
                Next
            Next

        End If
    End Sub

    Private Sub rmiCalCompPE_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rmiCalCompPE.Click
        Dim f As New PMActualsCriteria
        Dim db As New CompositeMetricDB
        Dim order As List(Of Integer) = db.GetCompositeCalculationOrder

        f.EnableOpCo = False
        If f.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim month = f.ReportCriteria.ReportingMonth
            Dim year = f.ReportCriteria.ReportingYear
            For Each cID As Integer In order
                For Each item As GridViewRowInfo In rgvDEMetrics.SelectedRows
                    Dim chart As MetricDTO = item.DataBoundItem
                    If chart.CompositeMetric And chart.CID = cID Then
                        Dim c As New CompositeWorker(chart.CID)
                        Try
                            c.ComputePEActuals(month, year)
                            RadMessageBox.Show("PE Computations Complete.")
                        Catch ex As Exception
                            RadMessageBox.Show(ex.Message)
                        End Try
                    End If
                Next
            Next

        End If
    End Sub

    Private Sub rmiCarryOver_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rmiCarryOver.Click
        If rgvDEMetrics.SelectedRows.Count = 0 Then Exit Sub
        Try
            Me.Cursor = Cursors.WaitCursor
            Dim yearform As New PMEditReportingYear
            yearform.InformationLabel = "Carry Points over from year: "
            If yearform.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Dim year As Integer = yearform.ReportingYear
                Dim db As New PMPointsDB

                RadMessageBox.Show("<html><size=9>This process will not overwrite any points that were previously carried over.</html>")

                For Each item As GridViewRowInfo In rgvDEMetrics.SelectedRows
                    Dim metric As MetricDTO = item.DataBoundItem
                    db.CarryOverPoints(metric.CID, year)
                Next
                RadMessageBox.Show("Points carried over successfully!")
            End If
        Catch ex As Exception
            Throw
        Finally
            Me.Cursor = Cursors.Arrow
        End Try
    End Sub
End Class