using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Threading;

using PAS.AutoTest.PasATCore;
using PAS.AutoTest.TestData;

namespace PAS.AutoTest.ScriptRunner
{
    public class ScriptRunner
    {
        private string mLastOutput = @"LastOutput.xml";
        private object syncObject = new object();
        private Thread mScriptRunner;
        private Thread mSynchronize ;
        private OutputData mOutput = new OutputData();
        private bool mIsRunComplete = false;
        private bool mIsTimeout = false;

    //add by macking 2011/08/19 for get the runner return from scripts
    private string mLastRunnerReturn;


        /// <summary>
        /// run the script
        /// </summary>
        /// <param name="scriptFilePath">script file path</param>
        /// <param name="timeOut">time out setting, measured by second</param>
        /// <returns></returns>
        public ExecuteResult Run(string scriptFilePath, string inputDataFilePath, int timeOut)
        {
            ExecuteResult er = new ExecuteResult();
            this.mIsRunComplete = false;
            this.mIsTimeout = false;

            try
            {
                StreamReader sr = new StreamReader(scriptFilePath);
                this.ExecuteScript(sr.ReadToEnd(), inputDataFilePath, timeOut);

                //while (!this.mIsRunComplete)
                //    Thread.Sleep(100);

                if (!mIsTimeout)
                {
                    if (LastOutput.Summary.Failed > 0)                //failed checkpoints exists
                        er.Result = TestResult.Fail;
                    else if (LastOutput.Summary.Passed == 0)    // no pass no failed
                        er.Result = TestResult.Done;
                    else                                                              // all other condition
                        er.Result = TestResult.Pass;
                }
                else
                {
                    er.Result = TestResult.Fail;
                    er.Message = "Wait script done time out.";
                }
                er.Output = this.LastOutput;
            }
            catch (Exception ex)
            {
                er.Result = TestResult.Incomplete;
                er.Output = this.LastOutput;
                er.Message = ex.Message;
            }

            return er;
        }

        private void RunScriptSync(object runner)
        {
            MethodInfo objMI = runner.GetType().GetMethod("Run");
            objMI.Invoke(runner, null);
            this.ScriptDone();
        }

        private void TimerStart(object timeout)
        {
            Thread.Sleep((int)timeout * 1000);
            this.TimeOut();
        }

        private void ScriptDone()
        {
            lock (this.syncObject)
            {
                if (this.mSynchronize != null)
                {
                    this.mSynchronize.Abort();
                    this.mSynchronize = new Thread(this.TimerStart);
                    this.mIsRunComplete = true;
                    this.mIsTimeout = false;
                }
            }
        }

        private void TimeOut()
        {
            lock (this.syncObject)
            {
                if (this.mScriptRunner != null)
                {
                    this.mScriptRunner.Abort();
                    this.mScriptRunner = new Thread(this.RunScriptSync);
                    this.mIsRunComplete = true;
                    this.mIsTimeout = true;
                }
            }
        }



        /// <summary>
        /// Enter your script within this method. 
        /// </summary>
        public void ExecuteScript(string script, string inputDataPath, int timout)
        {
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();

            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Drawing.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Xml.dll");
            objCompilerParameters.ReferencedAssemblies.Add("PAS.AutoTest.PasATCore.dll");
            objCompilerParameters.ReferencedAssemblies.Add("PAS.AutoTest.ScriptRunner.dll");
            objCompilerParameters.ReferencedAssemblies.Add("PAS.AutoTest.TestData.dll");
      
            objCompilerParameters.ReferencedAssemblies.Add("KDIS7ATCore.dll");
            objCompilerParameters.ReferencedAssemblies.Add("2DSim.dll");
      objCompilerParameters.ReferencedAssemblies.Add("NotificationSim.dll");

            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            CompilerResults cr = objICodeCompiler.CompileAssemblyFromSource(objCompilerParameters, this.GenerateScript(script, inputDataPath));

            if (cr.Errors.HasErrors)
            {
                MessageBox.Show("Compile Error: ");
                foreach (CompilerError err in cr.Errors)
                {
                    MessageBox.Show(err.ErrorText);
                }
            }
            else
            {
                Assembly objAssembly = cr.CompiledAssembly;
                object  objRunner = objAssembly.CreateInstance("CaseRunner.Runner");

                //mScriptRunner = new Thread(RunScriptSync);
                //mSynchronize = new Thread(TimerStart);

                //this.mScriptRunner.Start(objRunner);
                //this.mSynchronize.Start(timout);

                MethodInfo objMI = objRunner.GetType().GetMethod("Run");
                objMI.Invoke(objRunner, null);

        //object runStatus = objRunner.GetType().GetMethod("getRunStatus").Invoke(objRunner, null);
        //if (runStatus)
        //{
        mLastRunnerReturn = (string)objRunner.GetType().GetMethod("getRunReturn").Invoke(objRunner, null);
        mOutput.ConvertFromString(mLastRunnerReturn);
        //}
        //else
        //{
        //  mLastRunnerReturn = string.Empty;
        //}
        //Console.Out.WriteLine("OK");
            }
        }



        private string GenerateScript(string script, string inputDataPath)
        {
            string result = new StreamReader("ScriptTemplate.txt").ReadToEnd();

            result = result.Replace("@@INPUT@@", "@\"" + inputDataPath + "\"");
            result = result.Replace("@@RUN@@", script);

            return result;


        }

        public OutputData LastOutput
        {
            get
            {
        //modify by macking 2011/08/19 for get the runner return from scripts
        //return new OutputData(this.mLastOutput);
        //return new OutputData(mLastRunnerReturn);
        return mOutput;
            }
        }
    }

    public class ExecuteResult
    {
        public TestResult Result;
        public OutputData Output = null;
        public string Message = string.Empty;
    }
}
