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
  class Program
  {
    static void Main(string[] args)
    {
      int port = 10000;
      string sRet = "";
      //simulator si = new simulator(port, "2DViewer");
      simulator si = new simulator(port);

      ////Minimize or Restore
      //si.StartSimulater("min");
      //sRet = si.StopSimulator(120000);
      //Console.WriteLine("Get from thread:{0}", sRet);

      ////Get Configuration
      //si.StartSimulater("CgkJPHRyb3BoeSAgdHlwZT0icGFzX3NldF9jb25maWciPgoJCQkgIDxjb25maWc+CgkJCQkgICA8IS0tYXBwX2ljb24gb3B0aW9uYWwsIHZhbHVlIGlzIGxvY2FsIHBhdGgtLT4KCQkJICAJICA8cGFyYW1ldGVyIGtleT0iYXBwX2ljb24iIHZhbHVlPSJEOlx0ZXN0XGljb24uanBnIi8+CgkJCQkgIDwhLS1tb2RlIG9wdGlvbmFsICB2YWx1ZSByYW5nZVtsaWdodCxwYXRpZW50XS0tPgoJCQkJICA8cGFyYW1ldGVyIGtleT0ibW9kZSIgdmFsdWU9InBhdGllbnQiLz4KCgkJCSAgPC9jb25maWc+CgkJPC90cm9waHk+CgkJCgk=");
      //sRet = si.StopSimulator(30000);
      //Console.WriteLine("Get from thread:{0}", sRet);

      ////Close or Set Configuration
      //si.StartSimulater("close");
      //sRet = si.StopSimulator(30000);
      //Console.WriteLine("Get from thread:{0}", sRet);

      while (true)
      {
      //Start 2DView
        si.StartSimulater("0,2DViewer");
      //...
      sRet = si.StopSimulator(120000);
      Console.WriteLine("Get from thread:{0}", sRet);

      Console.WriteLine("==================");
      }
      si.StartSimulater("0,2DViewer");
      sRet = si.StopSimulator(300000);
      Console.WriteLine("Get from thread:{0}", sRet);
      Console.WriteLine("==================");

      si.StartSimulater("0,2DViewer");
      sRet = si.StopSimulator(300000);
      Console.WriteLine("Get from thread:{0}", sRet);
      Console.WriteLine("==================");

      si.StartSimulater("0,2DViewer");
      sRet = si.StopSimulator(300000);
      Console.WriteLine("Get from thread:{0}", sRet);
      Console.WriteLine("==================");

      si.StartSimulater("0,2DViewer");
      sRet = si.StopSimulator(300000);
      Console.WriteLine("Get from thread:{0}", sRet);
      Console.WriteLine("==================");

      //si = new simulator("test");
      //si.StartSimulater();

      //sRet = si.getSimBack(120000);
      //Console.WriteLine("Get from thread:{0}", sRet);




      //Get Configuration
      //si.StartSimulater("CgkJPHRyb3BoeSAgdHlwZT0icGFzX3NldF9jb25maWciPgoJCSAgPGNvbmZpZz4KCQkJICAgPCEtLXRpdGxlIG9wdGlvbmFsLS0+CgkJCSAgIDxwYXJhbWV0ZXIga2V5PSJ0aXRsZSIgdmFsdWU9InRlc3QiLz4KCQkJICAgPCEtLWFwcF9pY29uIG9wdGlvbmFsLS0+CgkJCSAgIDxwYXJhbWV0ZXIga2V5PSJhcHBfaWNvbiIgdmFsdWU9IkQ6XHRlc3RcaWNvbi5qcGciLz4KCQkJICAgPCEtLWFwcF9uYW1lIG9wdGlvbmFsLS0+CgkJCSAgIDxwYXJhbWV0ZXIga2V5PSJhcHBfbmFtZSIgdmFsdWU9ImRwbXMiLz4KCQkJICAgPCEtLW1vZGUgb3B0aW9uYWwgIHZhbHVlIHJhbmdlW2xpZ2h0LHBhdGllbnRdLS0+CgkJCSAgIDxwYXJhbWV0ZXIga2V5PSJtb2RlIiB2YWx1ZT0icGF0aWVudCIvPgoJCQkgICA8IS0tc3RhdGUgb3B0aW9uYWwgIHZhbHVlIHJhbmdlW3Jlc3RvcmUsbWluaW1pemVdLS0+CgkJCSAgIDxwYXJhbWV0ZXIga2V5PSJzdGF0ZSIgdmFsdWU9Im1pbmltaXplZCIvPgoJCQkgICA8IS0tdG9vdGhfbm90YXRpb24gb3B0aW9uYWwgIHZhbHVlIHJhbmdlW2V1cm9wZWFuLGFtZXJpY2FuLHVua25vd25dLS0+CgkJCSAgIDxwYXJhbWV0ZXIga2V5PSJ0b290aF9ub3RhdGlvbiIgdmFsdWU9ImV1cm9wZWFuIi8+CgkJICA8L2NvbmZpZz4KCQk8L3Ryb3BoeT4KCQkKCQ==");
      //sRet = si.StopSimulator(30000);
      //Console.WriteLine("Get from thread:{0}", sRet);
      
      Console.Read();
    }
  }

  /// <summary>
  /// Class for simulator
  /// </summary>
  /// <remarks>
  /// Example:
  /// <para>int port = 2010;</para>
  /// <para>TwoDSim.simulator si = new TwoDSim.simulator((port);</para>
  /// <para>//Start 2DViewer</para>
  /// <para>si.StartSimulater("2DViewer");</para>
  /// <para>string sRet = "";</para>
  /// <para>sRet = si.StopSimulator(60000);</para>
  /// <para>//Begin another communication</para>
  /// <para>si.StartSimulater("close");</para>
  /// <para>sRet = si.StopSimulator(60000);</para>
  /// </remarks>  
  public class simulator
  {
    private Thread sTh;
    //private ThreadStart threadDelegate;
    private AutoResetEvent simEvent;
    private string simRetStr;

    private ThreadForPAS tsListen;

    //public simulator(int portNum, string returnValue)
    /// <summary>
    /// Constructor for Simulator
    /// </summary>
    /// <param name="portNum">The socket number you want to listen</param>
    public simulator(int portNum)
    {
      //tsListen = new ThreadForPAS(portNum, returnValue, new SimCallback(setBack));
      tsListen = new ThreadForPAS(portNum, new SimCallback(setReturnString));
    }

    /// <summary>
    /// Start a simulator to receive SOCKET message
    /// </summary>
    /// <param name="retMessage">The string which you want the simulator sent back to PAS</param>
    /// <returns></returns>
    public int StartSimulater(string retMessage)
    {
      simEvent = new AutoResetEvent(false);
      try
      {        
       // threadDelegate = new ThreadStart(tsListen.ThreadListen);
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
    /// Stop a simulator and get the PAS send message to simulator
    /// </summary>
    /// <param name="timeOut">The time out you set(millisecond). If the simulator didn't receive any message from PAS, it should be exit when the time out</param>
    /// <returns>The PAS send to simulator's message</returns>
    public string StopSimulator(int timeOut)
    {
      ThreadPool.QueueUserWorkItem(new WaitCallback(WaitSimFinish), simEvent);
      if (simEvent.WaitOne(timeOut))
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
  delegate void SimCallback(string retMessage);

  class ThreadForPAS
  {
    //private string echoValue;
    private int listionPort;
    private SimCallback retCallback;
    private TcpListener tcpListener;
    private TcpClient tcpClient;

    //public ThreadForPAS(int port,string returnValue, SimCallback paCB)
    public ThreadForPAS(int port, SimCallback paCB)
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
      //Int32 port = listionPort;
      try
      {        
        //tcpListener.Server.ReceiveTimeout = 1;

        tcpListener.Start();
      
        byte[] readStr = new byte[1024];
        //while (true)
        //{
        
        tcpClient = tcpListener.AcceptTcpClient();
        NetworkStream ns = tcpClient.GetStream();
        StringBuilder sb = new StringBuilder();
        string result = "";
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
          // res = ns.Read(readStr, 0, readStr.Length);
          // sb.AppendFormat("{0}", Encoding.ASCII.GetString(readStr, 0, res));
        } //while (ns.DataAvailable);
        while (oneBt != -1);

        byte[] feedback = Encoding.ASCII.GetBytes((string)retMessage);
        ns.Write(feedback, 0, feedback.Length);

        Debug.Print("2D Simulator: Recv from PAS <{0}> byte", res);
        Console.WriteLine("Recv from PAS:{0} byte", res);
        Debug.Print("2D Simulator: Content <{0}>", sb);
        Console.WriteLine("Content: {0}", sb);
        //Console.Read();
        ns.Close();
        tcpClient.Close();
        result = sb.ToString().TrimEnd();
        //}

        if (retCallback != null)
          retCallback(result);
        tcpListener.Stop();
      }
      catch (ThreadAbortException e)
      {
        //Console.WriteLine("{0}", e);
        Console.WriteLine("Thread Abort");
        Debug.Print("Thread Abort");
        //tcpClient.Close();
        //tcpListener.Stop();
      }      
    } //End of ThreadListen

    public void PortClose()
    {
      tcpClient.Close();
      tcpListener.Stop();
    } //End of PortClose
  } 
}
