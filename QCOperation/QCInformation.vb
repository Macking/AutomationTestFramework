'This Class can write value or create run to QC server
'Author Name: Macking
'Create Date: 2010-06-07

Imports TDAPIOLELib

''' <summary>
''' QCInformation can provide the function of write information to QC, such as Test Run Step, Test Run Summary
''' </summary>
''' <remarks>
''' Sample:
''' <para>TSTest instance;</para>
''' <para>//generate the run first</para>
''' <para>RunFactory runFact = instance.RunFactory as RunFactory;</para>
''' <para>DateTime now = TestUtility.GetCurrentTime();</para>
''' <para>Run instanceRun = runFact.AddItem("Run_" + now.ToShortDateString() +</para>
''' <para>"_" + now.ToShortTimeString()) as Run;</para>
''' <para>QCOperation.QCInformation info = new QCOperation.QCInformation();</para>
''' <para>string runID = instanceRun.ID as string;</para>
''' <para>//Initial the start status</para>
''' <para>info.SetTestRunStatus(tdConn, runID, "Not Completed");</para>
''' <para>//Add the run steps</para>
''' <para>info.SetTestRunStep(tdConn, runID, "Step 1", "Passed");</para>
''' <para>info.SetTestRunStep(tdConn, runID, "Step 2", "Failed");</para>
''' <para>//Update the end status</para>
''' <para>info.SetTestRunStatus(tdConn, runID, "Failed");</para>
''' <para>//When finish the test, record the summary in instance of testset</para>
''' <para>string instanceID = instance.ID as string;</para>
''' <para>info.SetTestInstanceSummary(tdConn, instanceID, "TC_USER_02", "01");</para>
''' <para>info.SetTestInstanceSummary(tdConn, instanceID, "TC_USER_03", "02");</para>
''' </remarks>

Public Class QCInformation

    ''' <summary>
    ''' Write the test instance summary information
    ''' <para>Such as NumOfPass and NumOfTotal</para>
    ''' </summary>
    ''' <param name="TdConn">TDConnection TdConn, should be using OTA initial the connection</param>
    ''' <param name="Id">String Id, Test Instance's id value</param>
    ''' <param name="FieldName">String FieldName, The field you want to add value</param>
    ''' <param name="FieldValue">String FieldValue, The value you want to add field</param>
    ''' <returns>The return is Ture or False</returns>
    ''' <remarks></remarks>
    Public Function SetTestInstanceSummary(ByVal TdConn As TDConnection, _
                                           ByVal Id As String, _
                                           ByVal FieldName As String, _
                                           ByVal FieldValue As String) As Boolean
        Dim Result As Boolean
        Dim instance As TSTest

        Try
            instance = TdConn.TSTestFactory.item(Id)
            instance.Field(FieldName) = FieldValue
            instance.Post()
            instance.Refresh()
            Result = True
        Catch
            Result = False
        End Try
        Return Result
    End Function

    ''' <summary>
    ''' Add the test run of status to a related test instance
    ''' <para>It will be shown as Pass or Fail</para>
    ''' </summary>
    ''' <param name="TdConn">TDConnection TdConn, should be using OTA initial the connection</param>
    ''' <param name="Id">String Id, Test RUN's id value</param>
    ''' <param name="Status">String Status, Which you want to set status to QC in this test run
    ''' <para>it can be only given as: "Not Completed","Passed","Failed","N/A", and other value will be assign to "No Run" </para></param>
    ''' <returns>The return is Ture or False</returns>
    ''' <remarks></remarks>
    Public Function SetTestRunStatus(ByVal TdConn As TDConnection, _
                                     ByVal Id As String, _
                                     ByVal Status As String) As Boolean
        Dim Result As Boolean
        Dim CurrRun As Run
        Try
            CurrRun = TdConn.RunFactory.item(Id)
            CurrRun.Status = Status
            CurrRun.Post()
            CurrRun.Refresh()
            Result = True
        Catch
            Result = False
        End Try
    End Function

    ''' <summary>
    ''' Add the test steps to test run
    ''' <para>You can add steps as you want</para>
    ''' </summary>
    ''' <param name="TdConn">TDConnection TdConn, should be using OTA initial the connection</param>
    ''' <param name="Id">String Id, Test RUN's id value</param>
    ''' <param name="StepName">String StepName, The name of step you can set</param>
    ''' <param name="StepStatus">String StepStatus, The status of step you set
    ''' <para>it can be only given as: "Not Completed","Passed","Failed","N/A", and other value will be assign to "No Run" </para></param>
    ''' <returns>The return is Ture or False</returns>
    ''' <remarks></remarks>
    Public Function SetTestRunStep(ByVal TdConn As TDConnection, _
                                   ByVal Id As String, _
                                   ByVal StepName As String, _
                                   ByVal StepStatus As String) As Boolean
        Dim Result As Boolean
        Dim CurrRun As Run
        Dim CurrStep As StepFactory
        Dim stepDone As [Step]

        Try
            CurrRun = TdConn.RunFactory.item(Id)
            CurrStep = CurrRun.StepFactory
            stepDone = CurrStep.AddItem(StepName)
            stepDone.Status = StepStatus
            stepDone.Post()
            stepDone.Refresh()
            CurrRun.Post()
            CurrRun.Refresh()

            Result = True
        Catch
            Result = False
        End Try
    End Function

End Class
