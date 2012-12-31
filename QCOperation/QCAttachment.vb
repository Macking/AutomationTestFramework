'This Class can get/upload file to QC server
'Using QC OTA COM component
'Author Name: Macking
'Create Date: 2010-05-24

Imports TDAPIOLELib

''' <summary>
''' QCAttachment can provide the function of Download and Upload file(s) from QC
''' <para>If the project control by version, the update test attachment will be failure </para>
''' </summary>
''' <remarks>
''' Sample:
'''<para>QCOperation.QCAttachment taa = new QCOperation.QCAttachment();</para> 
'''<para>ArrayList attachments = new ArrayList();</para> 
''' <para>bool upSuccess;</para>
'''<para>//attachments will save the download files path</para>
'''<para>//Download</para>
'''<para>//Type value: it can be TEST, RUN, STEP or others</para>
'''<para>//Id value: TEST's id, RUN's id , and so on</para>
'''<para>//Specific file full name or wildcard with extention name</para>
'''<para>//File out put path</para>
'''<para>attachments = (ArrayList)taa.DownloadAttachment(qcOnline.getTDConn(), "Test", "1406", "Query_1406.xml", "C:\\temp");</para>
'''<para>attachments = (ArrayList)taa.DownloadAttachment(qcOnline.getTDConn(), "Test", "1406", "*.JPG", "C:\\temp");</para>
'''<para>//Upload</para>
'''<para>upSuccess = (bool)taa.UploadAttachment(qcOnline.getTDConn(), "RUN", "1406", "test.cs", "C:\\temp");</para>
''' </remarks>

Public Class QCAttachment
    'It can provide to customer
    ' Private -> Public
    Private Function DownloadAttachment(ByVal tdConn As TDConnectionClass, _
                                       ByVal type As String, _
                                       ByVal OutPath As String, _
                                       ByVal id As String)
        Dim attachment As TDAPIOLELib.AttachmentFactory
        Dim result As System.Collections.ArrayList
        Dim attlist As TDAPIOLELib.List
        Dim i As Integer
        Dim attfile As TDAPIOLELib.Attachment

        Dim test As TDAPIOLELib.Test
        Dim rn As TDAPIOLELib.Run
        Dim stepFactory As TDAPIOLELib.StepFactory

        If Right(OutPath, 1) <> "\" Then
            OutPath = OutPath & "\"
        End If

        result = New System.Collections.ArrayList()
        rn = tdConn.RunFactory().Item(id)

        Select Case type.ToUpper()
            Case "TEST"
                test = tdConn.TestFactory().item(id)
                If test.HasAttachment Then
                    attachment = test.Attachments
                    attlist = attachment.NewList("")

                    For i = 1 To attlist.Count
                        attfile = attlist(i)
                        attfile.Load(True, OutPath)
                        result.Add(attfile.FileName)
                    Next
                End If
            Case "RUN"
                If rn.HasAttachment Then
                    attachment = rn.Attachments
                    attlist = attachment.NewList("")

                    For i = 1 To attlist.Count
                        attfile = attlist(i)
                        attfile.Load(True, OutPath)
                        result.Add(attfile.FileName)
                    Next
                End If
            Case "STEP"
                stepFactory = rn.StepFactory
                Dim ls As TDAPIOLELib.List
                ls = stepFactory.NewList("")
                Dim st As TDAPIOLELib.Step
                For Each st In ls
                    If st.HasAttachment Then
                        attachment = st.Attachments()
                        attlist = attachment.NewList("")
                        For i = 1 To attlist.Count
                            attfile = attlist(i)
                            attfile.Load(True, OutPath)
                            result.Add(st.Name + "|" + attfile.FileName)
                        Next
                    End If
                Next
        End Select
        DownloadAttachment = result
    End Function

    'Intenal function
    Private Function GetAttachmentFromTest(ByVal tdConn As TDConnectionClass, _
                                         ByVal id As String, _
                                         ByVal fileName As String, _
                                         ByVal outPath As String)
        Dim result As System.Collections.ArrayList
        Dim test As TDAPIOLELib.Test
        Dim attachFact As AttachmentFactory
        Dim attachmentList As List
        Dim attachment As Attachment
        Dim LongFileName As String
        Dim FileNameNoExtention As String
        Dim FileExtention As String
        Dim attachmentServerPath As String
        Dim Pos As Integer
        Dim extendedStorage As ExtendedStorage
        'Dim s1, s2

        result = New System.Collections.ArrayList()

        test = tdConn.TestFactory().item(id)

        attachFact = test.Attachments
        attachmentList = attachFact.NewList("SELECT * FROM CROS_REF")

        FileNameNoExtention = Mid(fileName, 1, InStrRev(fileName, ".", -1, 1) - 1)
        FileExtention = Mid(fileName, InStrRev(fileName, ".", -1, 1) + 1)

        For Each attachment In attachmentList
            If FileNameNoExtention <> "*" Then
                If StrComp(attachment.Name(1), fileName, CompareMethod.Text) <> False Then
                    Continue For
                End If
            Else
                If StrComp(Right(attachment.Name(1), 3), FileExtention, CompareMethod.Text) <> False Then
                    Continue For
                End If
            End If
            LongFileName = attachment.Name
            Pos = InStr(1, attachment.ServerFileName, attachment.Name, CompareMethod.Text)
            attachmentServerPath = Left(attachment.ServerFileName, Pos - 1)
            If StrComp(attachmentServerPath, "") = 0 Then
                result.Add("")
                GetAttachmentFromTest = result
                Exit Function
            End If
            If Right(outPath, 1) <> "\" Then
                outPath = outPath & "\"
            End If
            extendedStorage = test.ExtendedStorage
            extendedStorage.ServerPath = attachmentServerPath
            extendedStorage.ClientPath = outPath
            extendedStorage.Load(LongFileName, True)
            result.Add(outPath & LongFileName)
        Next
        GetAttachmentFromTest = result
    End Function
    'Intenal function
    Private Function GetAttachmentServerPath(ByVal TestObject, ByVal FileName, ByRef LongFileName)
        Dim AttachmentFactory
        Dim AttachmentList
        Dim Attachment
        Dim Pos
        AttachmentFactory = TestObject.Attachments
        AttachmentList = AttachmentFactory.NewList("SELECT * FROM CROS_REF")

        For Each Attachment In AttachmentList
            If StrComp(Attachment.Name(1), FileName, 1) = False Then
                LongFileName = Attachment.Name
                Pos = InStr(1, Attachment.ServerFileName, Attachment.Name, 1)
                GetAttachmentServerPath = Left(Attachment.ServerFileName, Pos - 1)
                Exit Function
            End If
        Next
        GetAttachmentServerPath = ""
    End Function
    'Intenal function
    Private Function GetAttachmentFromTestObject(ByVal TestObject, ByVal FileName, ByVal OutPath)
        Dim MyPath
        Dim LongFileName
        Dim ExtendedStorage As ExtendedStorage
        LongFileName = ""
        MyPath = GetAttachmentServerPath(TestObject, FileName, LongFileName)

        If StrComp(MyPath, "") = 0 Then
            GetAttachmentFromTestObject = ""
            Exit Function
        End If

        If Right(OutPath, 1) <> "\" Then
            OutPath = OutPath & "\"
        End If

        ' Load the attachment using the extended storage object
        ExtendedStorage = TestObject.ExtendedStorage
        ExtendedStorage.ServerPath = MyPath
        ExtendedStorage.ClientPath = OutPath
        ExtendedStorage.Load(LongFileName, True)

        GetAttachmentFromTestObject = OutPath & LongFileName
    End Function

    ' Get an attachment from another test
    ' It can be open to the customer
    ' Private -> Public
    Private Function GetAttachmentFromTestName(ByVal tdConn As TDConnectionClass, _
                                              ByVal TestName As String, _
                                              ByVal FileName As String, _
                                              ByVal OutPath As String)
        Dim TestList As List
        Dim TestInstance As Test
        Dim Result As String

        Result = ""
        TestList = tdConn.TestFactory.NewList("SELECT * FROM TEST WHERE TS_NAME = '" & TestName & "'")

        For Each TestInstance In TestList
            Result = GetAttachmentFromTestObject(TestInstance, FileName, OutPath)
        Next
        GetAttachmentFromTestName = Result
    End Function

    ''' <summary>
    ''' Attach an attachment file to run, if the filename already exist, the upload will over the old one
    ''' </summary>
    ''' <param name="tdConn">The td conn.</param>
    ''' <param name="Id">The id.</param>
    ''' <param name="FileName">Name of the file.</param>
    ''' <param name="FilePath">The file path.</param>
    ''' <returns></returns>
    Private Function AttachFileToRun(ByVal tdConn As TDConnectionClass, _
                                         ByVal Id As String, _
                                         ByVal FileName As String, _
                                         ByVal FilePath As String)
        Dim Result As Boolean
        Dim rn As TDAPIOLELib.Run
        Dim Attachment As AttachmentFactory
        Dim att As Attachment
        Dim extStr As ExtendedStorage
        Dim attachmentList As List
        Dim countNum As Integer
        Result = False
        Try
            rn = tdConn.RunFactory.Item(Id)

            Attachment = rn.Attachments

            'Already exist, should update this attachment
            attachmentList = Attachment.NewList("SELECT * FROM CROS_REF")
            countNum = attachmentList.Count
            For Each att In attachmentList
                If StrComp(att.Name(1), FileName, CompareMethod.Text) = False Then
                    extStr = att.AttachmentStorage
                    extStr.ClientPath = FilePath
                    extStr.Save(FileName, True)
                    att.Post()
                    att.Refresh()
                    Result = True
                    Continue For
                End If
                countNum -= 1
            Next

            'Not exist, add this attachment directly
            If (countNum = 0) Then
                att = Attachment.AddItem(FileName)
                extStr = att.AttachmentStorage
                extStr.ClientPath = FilePath
                extStr.Save(FileName, True)
                att.Post()
                att.Refresh()
                Result = True
            End If

        Catch
            Result = False
        End Try
        AttachFileToRun = Result
    End Function


    ''' <summary>
    ''' Attach an attachment file to test, if the filename already exist, the upload will over the old one
    ''' </summary>
    ''' <param name="tdConn">The td conn.</param>
    ''' <param name="Id">The id.</param>
    ''' <param name="FileName">Name of the file.</param>
    ''' <param name="FilePath">The file path.</param>
    ''' <returns></returns>
    Private Function AttachFileToTest(ByVal tdConn As TDConnectionClass, _
                                         ByVal Id As String, _
                                         ByVal FileName As String, _
                                         ByVal FilePath As String)
        Dim Result As Boolean
        Dim test As TDAPIOLELib.Test
        Dim Attachment As AttachmentFactory
        Dim attachmentList As List
        Dim att As Attachment
        Dim extStr As ExtendedStorage
        Dim countNum As Integer

        Result = False
        Try
            test = tdConn.TestFactory().item(Id)
            Attachment = test.Attachments

            attachmentList = Attachment.NewList("SELECT * FROM CROS_REF")
            countNum = attachmentList.Count
            For Each att In attachmentList
                If StrComp(att.Name(1), FileName, CompareMethod.Text) = False Then
                    extStr = att.AttachmentStorage
                    extStr.ClientPath = FilePath
                    extStr.Save(FileName, True)
                    att.Post()
                    att.Refresh()
                    Result = True
                    Continue For
                End If
                countNum -= 1
            Next

            'Not exist, add this attachment directly
            If (countNum = 0) Then
                att = Attachment.AddItem(FileName)
                extStr = att.AttachmentStorage
                extStr.ClientPath = FilePath
                extStr.Save(FileName, True)
                att.Post()
                att.Refresh()
                Result = True
            End If
        Catch
            Result = False
        End Try

        AttachFileToTest = Result
    End Function

    ''' <summary>
    ''' Removes the attachment from test.
    ''' </summary>
    ''' <param name="tdConn">The td conn.</param>
    ''' <param name="Id">The id.</param>
    ''' <returns></returns>
    Private Function RemoveAttachmentFromTest(ByVal tdConn As TDConnectionClass, _
                                         ByVal Id As String)
        Dim Result As Boolean
        Dim test As TDAPIOLELib.Test
        Dim Attachment As AttachmentFactory
        Dim attachmentList As List
        Dim att As Attachment

        Result = False
        Try
            test = tdConn.TestFactory().item(Id)
            Attachment = test.Attachments

            attachmentList = Attachment.NewList("SELECT * FROM CROS_REF")

            For Each att In attachmentList 'Remove the attachment one by one
                Attachment.RemoveItem(att.ID)
            Next

            Result = True

        Catch
            Result = False
        End Try

        RemoveAttachmentFromTest = Result
    End Function

    ''' <summary>
    ''' Upload attachment to QC
    ''' <para>Only implement the RUN and TEST type</para>
    ''' </summary>
    ''' <param name="TdConn">TDConnectionClass TdConn, should be using OTA initial the connection</param>
    ''' <param name="Type">String Type, It's value can be TEST, RUN, STEP or others</param>
    ''' <param name="Id">String Id, Match Type's id value</param>
    ''' <param name="FileName">String FileName, The file name you want to upload</param>
    ''' <param name="UploadPath">String UploadPath, The local path which your upload file located</param>
    ''' <returns>The return is Ture or False</returns>
    ''' <remarks></remarks>
    Public Function UploadAttachment(ByVal TdConn As TDConnectionClass, _
                                    ByVal Type As String, _
                                    ByVal Id As String, _
                                    ByVal FileName As String, _
                                    ByVal UploadPath As String) As Boolean
        Dim Result As Boolean
        Dim FullFileName As String
        Result = False

        If Right(UploadPath, 1) <> "\" Then
            UploadPath = UploadPath & "\"
        End If

        FullFileName = UploadPath & FileName

        If My.Computer.FileSystem.FileExists(FullFileName) Then

            Select Case Type.ToUpper()
                Case "RUN"
                    Result = AttachFileToRun(TdConn, Id, FileName, UploadPath)
                Case "TEST"
                    Result = AttachFileToTest(TdConn, Id, FileName, UploadPath)
            End Select
        End If
        UploadAttachment = Result

    End Function

    ''' <summary>
    ''' Removes the attachment from QC, currently only support remove from test case.
    ''' </summary>
    ''' <param name="TdConn">The td conn.</param>
    ''' <param name="Type">The type.</param>
    ''' <param name="Id">The id.</param>
    ''' <returns></returns>
    Public Function RemoveAttachment(ByVal TdConn As TDConnectionClass, _
                                    ByVal Type As String, _
                                    ByVal Id As String) As Boolean
        Dim Result As Boolean
        Result = False

        Select Case Type.ToUpper()
            'Case "RUN"
            'Result = False ' Not supported yet
            Case "TEST"
                Result = RemoveAttachmentFromTest(TdConn, Id)
        End Select

        RemoveAttachment = Result

    End Function

    ''' <summary>
    ''' Download attachment from QC
    ''' <para>Only implement the TEST type</para>
    ''' </summary>
    ''' <param name="TdConn">TDConnectionClass TdConn, should be using OTA initial the connection</param>
    ''' <param name="Type">String Type, It's value can be TEST, RUN, STEP or others</param>
    ''' <param name="Id">String Id, Match Type's id value</param>
    ''' <param name="FileName">String FileName, The file name you want to download</param>
    ''' <param name="DownloadPath">String DownloadPath, The local path that you want to save the file(s)</param>
    ''' <returns>StringArray, the download file(s) local path array </returns>
    ''' <remarks></remarks>
    Public Function DownloadAttachment(ByVal TdConn As TDConnectionClass, _
                                       ByVal Type As String, _
                                       ByVal Id As String, _
                                       ByVal FileName As String, _
                                       ByVal DownloadPath As String) As ArrayList

        Dim attachment As TDAPIOLELib.AttachmentFactory
        Dim result As System.Collections.ArrayList
        Dim attlist As TDAPIOLELib.List
        Dim i As Integer
        Dim attfile As TDAPIOLELib.Attachment

        'Dim test As TDAPIOLELib.Test
        Dim rn As TDAPIOLELib.Run
        Dim stepFactory As TDAPIOLELib.StepFactory

        If Right(DownloadPath, 1) <> "\" Then
            DownloadPath = DownloadPath & "\"
        End If

        result = New System.Collections.ArrayList()
        rn = TdConn.RunFactory().Item(Id)

        Select Case Type.ToUpper()
            Case "TEST"
                result = GetAttachmentFromTest(TdConn, Id, FileName, DownloadPath)
            Case "RUN"
                If rn.HasAttachment Then
                    attachment = rn.Attachments
                    attlist = attachment.NewList("")

                    For i = 1 To attlist.Count
                        attfile = attlist(i)
                        attfile.Load(True, DownloadPath)
                        result.Add(attfile.FileName)
                    Next
                End If
            Case "STEP"
                stepFactory = rn.StepFactory
                Dim ls As TDAPIOLELib.List
                ls = stepFactory.NewList("")
                Dim st As TDAPIOLELib.Step
                For Each st In ls
                    If st.HasAttachment Then
                        attachment = st.Attachments()
                        attlist = attachment.NewList("")
                        For i = 1 To attlist.Count
                            attfile = attlist(i)
                            attfile.Load(True, DownloadPath)
                            result.Add(st.Name + "|" + attfile.FileName)
                        Next
                    End If
                Next
        End Select
        DownloadAttachment = result
    End Function

End Class