using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace nc_sim
{
    class nc_sim
    {
        static void Main(string[] args)
        {
            string mode = "";
            string content = "";
            string address = "localhost";
            string port = "10000";
            string response = "0,OK\n";
            bool multiport = false;
            foreach (string ar in args)
            {
                mode = ar.Substring(1, 1);
                content = ar.Substring(2);
                switch (mode)
                {
                    case "l":
                        address = content;
                        break;
                    case "p":
                        port = content;
                        break;
                    case "a":
                        multiport = true;
                        break;
                    default:
                        break;
                }
            }
            try
            {
                TcpListener tcpListener;
                if (address == "127.0.0.1")
                {
                    if (multiport)
                    {
                        int n = 1;
                        tcpListener = new TcpListener(IPAddress.Loopback, int.Parse(port));
                        do
                        {
                            TcpListener tcpListenerMulti = new TcpListener(IPAddress.Loopback, int.Parse(port) + n);
                            tcpListenerMulti.Start();
                            n++;
                        } while (n < 100);
                    }
                    else
                    {
                        Console.WriteLine("Listen on 127.0.0.1:" + port);
                        tcpListener = new TcpListener(IPAddress.Loopback, int.Parse(port));
                    }
                }
                else
                {
                    if (multiport)
                    {
                        int n = 1;
                        tcpListener = new TcpListener(IPAddress.Any, int.Parse(port));
                        do
                        {
                            TcpListener tcpListenerMulti = new TcpListener(IPAddress.Any, int.Parse(port) + n);
                            tcpListenerMulti.Start();
                            n++;
                        } while (n < 100);
                    }
                    else
                    {
                        tcpListener = new TcpListener(IPAddress.Any, int.Parse(port));
                    }

                }
                tcpListener.Start();
                TcpClient tcpClient = new TcpClient();

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
                        if (oneBt == 10 || oneBt == 13)
                            break;
                    } while (oneBt != -1);
                    byte[] feedback = Encoding.ASCII.GetBytes(response);
                    ns.Write(feedback, 0, feedback.Length);
                    ns.Close();
                    //tcpClient.Close();
                    Console.WriteLine(sb.ToString().TrimEnd());
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
        }
    }
}
