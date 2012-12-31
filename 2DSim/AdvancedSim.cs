/* This class for simulating 2D Viewer
 * main function is opening SOCKET and receive/send message to PAS
 * Author Name: Macking
 * Create Date: 2010-06-12
 * */

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace TwoDSim
{
    /// <summary>
    /// Class for AdvanceSimulator
    /// </summary>
    /// <remarks>
    /// Example:
    /// <para>int port = 2010;</para>
    /// <para>TwoDSim.AdvanceSimulator si = new TwoDSim.AdvanceSimulator(port);</para>
    /// <para>//Start 2DViewer</para>
    /// <para>si.StartSimulater("2DViewer;openObjcet");</para>
    /// <para>string sRet = "";</para>
    /// <para>sRet = si.StopSimulator(60000);</para>
    /// <para>//Begin another communication</para>
    /// <para>si.StartSimulater("close");</para>
    /// <para>sRet = si.StopSimulator(60000);</para>
    /// </remarks>  
    public class AdvanceSimulator
    {
        private Thread sTh;
        //private ThreadStart threadDelegate;
        private AutoResetEvent simEvent;
        private string simRetStr;

        private ThreadForAdvanceSim tsListen;

        //public AdvanceSimulator(int portNum, string returnValue)
        /// <summary>
        /// Constructor for Simulator
        /// </summary>
        /// <param name="portNum">The socket number you want to listen</param>
        public AdvanceSimulator(int portNum)
        {
            //tsListen = new ThreadForAdvanceSim(portNum, returnValue, new SimCallbackAdvance(setBack));
            tsListen = new ThreadForAdvanceSim(portNum, new SimCallbackAdvance(setReturnString));
        }

        /// <summary>
        /// Start a AdvanceSimulator to receive SOCKET message
        /// </summary>
        /// <param name="retMessage">The string which you want the AdvanceSimulator sent back to PAS</param>
        /// <returns></returns>
        public int StartSimulater(string retMessage)
        {
            simEvent = new AutoResetEvent(false);
            try
            {                
                Thread th = new Thread(tsListen.ThreadListen);
                th.Start(retMessage);
                sTh = th;
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e);
                return -1;
            }
        }

        private void WaitSimFinish(object stateInfo)
        {
            //sTh.();      
            sTh.Join();
            ((AutoResetEvent)stateInfo).Set();
            //return true;
        }

        private void setReturnString(string retStr)
        {
            simRetStr = retStr;
            //return retStr;     
        }

        /// <summary>
        /// Stop a AdvanceSimulator and get the PAS send message to AdvanceSimulator
        /// </summary>
        /// <param name="timeOut">The time out you set(millisecond). If the AdvanceSimulator didn't receive any message from PAS, it should be exit when the time out</param>
        /// <returns>The PAS send to AdvanceSimulator's message</returns>
        public string StopSimulator(int timeOut)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WaitSimFinish), simEvent);
            if (simEvent.WaitOne(timeOut, false))
                return simRetStr;
            else
            {
                //When time out
                sTh.Abort();
                sTh.Join(10);
                tsListen.PortClose();
                return "";
            }

            //sTh.Join(timeOut);

            //sTh.Abort(); 

            //return simRetStr;           
        }
    }
    /// <summary>
    /// Just for call back, return the thread received message
    /// </summary>
    /// <param name="retMessage"></param>
    delegate void SimCallbackAdvance(string retMessage);

    class ThreadForAdvanceSim
    {
        //private string echoValue;
        private int listionPort;
        private SimCallbackAdvance retCallback;
        private TcpListener tcpListener;
        private TcpClient tcpClient;

        //public ThreadForAdvanceSim(int port,string returnValue, SimCallbackAdvance paCB)
        public ThreadForAdvanceSim(int port, SimCallbackAdvance paCB)
        {
            listionPort = port;
            // echoValue = returnValue;
            retCallback = paCB;

            //string server = null;
            //server = Dns.GetHostName();

            //IPHostEntry heserver = Dns.GetHostEntry(server);
            //foreach (IPAddress localAddr in heserver.AddressList)
            //{
            //  tcpListener = new TcpListener(localAddr, port);
            //  tcpClient = new TcpClient();
            //}
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpClient = new TcpClient();
            //IPAddress localAddr = IPAddress.Parse("10.112.37.135");
            //  IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            //tcpListener = new TcpListener(localAddr, port);
            //tcpClient = new TcpClient();
        }

        public void ThreadListen(object retMessage)
        {
            try
            {
                string TempCommand = (string)retMessage;
                string[] responseAndCommand = TempCommand.Split(';');
                string response = responseAndCommand[0];
                string command = responseAndCommand[1];
                tcpListener.Start();
                string result = "";
                while (true)
                {
                    //Console.WriteLine(DateTime.Now.TimeOfDay.ToString());
                    tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream ns = tcpClient.GetStream();
                    StringBuilder sb = new StringBuilder();

                    int res = 0;
                    int oneBt;
                    do
                    {
                        oneBt = ns.ReadByte();
                        sb.AppendFormat("{0}", (char)oneBt);
                        res++;
                        //receive '\r' & '\n' will end
                        if (oneBt == 10 || oneBt == 13)
                            break;
                    } while (oneBt != -1);

                    byte[] feedback = Encoding.ASCII.GetBytes(response);
                    ns.Write(feedback, 0, feedback.Length);

                    ns.Close();
                    tcpClient.Close();
                    result = sb.ToString().TrimEnd();
                    string subRet = result;
                    if (subRet.Length != 0)
                    {
                        int indexKey = subRet.IndexOf(" ");
                        if (indexKey != -1)
                        {
                            subRet = subRet.Substring(0, indexKey);
                            result = result.Substring(indexKey + 1);
                        }
                        if (subRet == (string)command)
                            break;
                    }
                }

                if (retCallback != null)
                    retCallback(result);
                tcpListener.Stop();
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread Abort");
            }
        } //End of ThreadListen

        public void PortClose()
        {
            tcpClient.Close();
            tcpListener.Stop();
        } //End of PortClose
    }
}
