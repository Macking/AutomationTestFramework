Imports System.IO
Imports System.Xml.Serialization


Public Class TestConfigFile
    Private QCConnectField As QCConnectConfig

    Public Property QCConnect() As QCConnectConfig
        Get
            QCConnect = QCConnectField
        End Get
        Set(ByVal value As QCConnectConfig)
            QCConnectField = value
        End Set
    End Property

    Public Class QCConnectConfig
        Private ServerAddrField As String
        Private DomainField As String
        Private ProjectField As String
        Private LoginNameField As String
        Private PasswordField As String

        Public Property ServerAddr() As String
            Get
                ServerAddr = ServerAddrField
            End Get
            Set(ByVal value As String)
                ServerAddrField = value
            End Set
        End Property

        Public Property Domain() As String
            Get
                Domain = DomainField
            End Get
            Set(ByVal value As String)
                DomainField = value
            End Set
        End Property

        Public Property Project() As String
            Get
                Project = ProjectField
            End Get
            Set(ByVal value As String)
                ProjectField = value
            End Set
        End Property

        Public Property LoginName() As String
            Get
                LoginName = LoginNameField
            End Get
            Set(ByVal value As String)
                LoginNameField = value
            End Set
        End Property

        Public Property Password() As String
            Get
                Password = PasswordField
            End Get
            Set(ByVal value As String)
                PasswordField = value
            End Set
        End Property
    End Class

    Sub New()

    End Sub
    Sub New(ByVal configFile As String)
        Dim srFile As New StreamReader(configFile)
        Dim p2 As New TestConfigFile()
        Dim x As New XmlSerializer(p2.GetType)
        x.Deserialize(srFile)
        srFile.Close()

    End Sub

End Class
