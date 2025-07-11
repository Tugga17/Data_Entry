Public Class MetricsMassProperties
    Private _contact As String
    Private _lexicon As Integer
    Private _inverse As String
    'Private _targets As Boolean
    Private _active As Boolean
    Private _benchmarkID As String
    Private _confidential As Boolean
    Private _composite As Boolean
    Private _incentive As Boolean
    Private _target As Boolean
    Private _primaryData As String
    Private _backupData As String
    Private _commentData As String
    Private _commentNotRequired As Boolean
    Private _ManagedBy As Integer
    Private _MetricOwner As String
    Private _MODcontact As Boolean = False
    Private _MODlexicon As Boolean = False
    Private _MODinverse As Boolean = False
    Private _MODtargets As Boolean = False
    Private _MODactive As Boolean = False
    Private _MODbenchmarkID As Boolean = False
    Private _MODconfidential As Boolean = False
    Private _MODComposite As Boolean = False
    Private _MODIncentive As Boolean = False
    Private _MODTarget As Boolean = False
    Private _formatID As Integer
    Private _MODformatID As Boolean
    Private _MODprimary As Boolean = False
    Private _ModBackup As Boolean = False
    Private _MODComment As Boolean = False
    Private _MODcommentNotRequired As Boolean
    Private _MODManagedBy As Boolean = False
    Private _MODMetricOwner As String

    Public ReadOnly Property HasEdits() As Boolean
        Get
            Return (_MODcommentNotRequired Or _MODconfidential Or _MODIncentive Or _MODTarget Or _MODcontact Or _MODlexicon Or _MODinverse Or _MODactive Or _MODManagedBy Or _MODprimary Or _ModBackup Or _MODMetricOwner Or _MODformatID Or _MODComment)
        End Get
    End Property
    Public Property MODActive() As Boolean
        Get
            Return _MODactive
        End Get
        Set(ByVal value As Boolean)
            _MODactive = value
        End Set
    End Property
    Public Property MODPMContactID() As Boolean
        Get
            Return _MODcontact
        End Get
        Set(ByVal value As Boolean)
            _MODcontact = value
        End Set
    End Property
    Public Property MODLexiconID() As Boolean
        Get
            Return _MODlexicon
        End Get
        Set(ByVal value As Boolean)
            _MODlexicon = value
        End Set
    End Property

    Public Property MODInverseScale() As Boolean
        Get
            Return _MODinverse
        End Get
        Set(ByVal value As Boolean)
            _MODinverse = value
        End Set
    End Property
    Public Property MODConfidentialMeasure() As Boolean
        Get
            Return _MODconfidential
        End Get
        Set(ByVal value As Boolean)
            _MODconfidential = value
        End Set
    End Property
    Public Property MODComposite() As Boolean
        Get
            Return _MODComposite
        End Get
        Set(ByVal value As Boolean)
            _MODComposite = value
        End Set
    End Property
    Public Property MODIncentive() As Boolean
        Get
            Return _MODIncentive
        End Get
        Set(ByVal value As Boolean)
            _MODIncentive = value
        End Set
    End Property
    Public Property MODTarget() As Boolean
        Get
            Return _MODTarget
        End Get
        Set(ByVal value As Boolean)
            _MODTarget = value
        End Set
    End Property
    Public Property MODPrimaryDataProvider() As Boolean
        Get
            Return _MODprimary
        End Get
        Set(ByVal value As Boolean)
            _MODprimary = value
        End Set
    End Property
    Public Property MODBackupDataProvider() As Boolean
        Get
            Return _ModBackup
        End Get
        Set(ByVal value As Boolean)
            _ModBackup = value
        End Set
    End Property
    Public Property MODCommentNotRequired() As Boolean
        Get
            Return _MODcommentNotRequired
        End Get
        Set(ByVal value As Boolean)
            _MODcommentNotRequired = value
        End Set
    End Property
    Public Property MODManagedBy() As Boolean
        Get
            Return _MODManagedBy
        End Get
        Set(ByVal value As Boolean)
            _MODManagedBy = value
        End Set
    End Property
    Public Property MODMetricOwner() As Boolean
        Get
            Return _MODMetricOwner
        End Get
        Set(ByVal value As Boolean)
            _MODMetricOwner = value
        End Set
    End Property
    Public Property MODCommentDataProvider() As Boolean
        Get
            Return _MODComment
        End Get
        Set(ByVal value As Boolean)
            _MODComment = value
        End Set
    End Property
    'Public Property MODIncentive() As Boolean
    '    Get
    '        Return _MODincentive
    '    End Get
    '    Set(ByVal value As Boolean)
    '        _MODincentive = value
    '    End Set
    'End Property
    Public Property MODFormatID() As Boolean
        Get
            Return _MODformatID
        End Get
        Set(ByVal value As Boolean)
            _MODformatID = value
        End Set
    End Property
    Public Property FormatID() As Integer
        Get
            Return _formatID
        End Get
        Set(ByVal value As Integer)
            _formatID = value
        End Set
    End Property
    Public Property Active() As Boolean
        Get
            Return _active
        End Get
        Set(ByVal value As Boolean)
            _active = value
        End Set
    End Property
    Public Property PMContactID() As String
        Get
            Return _contact
        End Get
        Set(ByVal value As String)
            _contact = value
        End Set
    End Property
    Public Property LexiconID() As Integer
        Get
            Return _lexicon
        End Get
        Set(ByVal value As Integer)
            _lexicon = value
        End Set
    End Property

    Public Property InverseScale() As String
        Get
            Return _inverse
        End Get
        Set(ByVal value As String)
            _inverse = value
        End Set
    End Property
    Public Property ConfidentialMeasure() As Boolean
        Get
            Return _confidential
        End Get
        Set(ByVal value As Boolean)
            _confidential = value
        End Set
    End Property
    Public Property Composite() As Boolean
        Get
            Return _composite
        End Get
        Set(ByVal value As Boolean)
            _composite = value
        End Set
    End Property
    Public Property Incentive() As Boolean
        Get
            Return _incentive
        End Get
        Set(ByVal value As Boolean)
            _incentive = value
        End Set
    End Property
    Public Property Target() As Boolean
        Get
            Return _target
        End Get
        Set(ByVal value As Boolean)
            _target = value
        End Set
    End Property
    Public Property PrimaryDataProvider() As String
        Get
            Return _primaryData
        End Get
        Set(ByVal value As String)
            _primaryData = value
        End Set
    End Property
    Public Property BackupDataProvider() As String
        Get
            Return _backupData
        End Get
        Set(ByVal value As String)
            _backupData = value
        End Set
    End Property
    Public Property CommentDataProvider() As String
        Get
            Return _commentData
        End Get
        Set(ByVal value As String)
            _commentData = value
        End Set
    End Property
    Public Property CommentNotRequired() As Boolean
        Get
            Return _commentNotRequired
        End Get
        Set(ByVal value As Boolean)
            _commentNotRequired = value
        End Set
    End Property
    Public Property ManagedBy() As String
        Get
            Return _ManagedBy
        End Get
        Set(ByVal value As String)
            _ManagedBy = value
        End Set
    End Property
    Public Property MetricOwner() As String
        Get
            Return _MetricOwner
        End Get
        Set(ByVal value As String)
            _MetricOwner = value
        End Set
    End Property
End Class
