'This Class can connect QC server
'Using QC OTA COM component
'Author Name: Macking
'Create Date: 2010-05-24

Imports TDAPIOLELib

''' <summary>
''' QCConnection can provide the function of (dis)connection QC
''' </summary>
''' <remarks>
''' Sample:
''' <para>QCOperation.QCConnection conn = new QCOperation.QCConnection();</para>
''' </remarks>

Public Class QCConnection

    Dim isConnectToQC As Boolean
    Dim isConnectProject As Boolean
    Dim tdConn As TDConnection

End Class
