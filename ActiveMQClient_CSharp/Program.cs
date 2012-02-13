using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace ActiveMQClient
{
    class Program
    {
        static void Main(string[] args)
        {
      /*
            //create the connection for activeMQ
      //string hostname = "10.112.39.102";
      string hostname = "localhost";
            string stompServerPort = "61616";
            Uri uriSample = new Uri("activemq:tcp://"+hostname+":"+stompServerPort);
            ConnectionFactory confSample = new ConnectionFactory(uriSample);
            Connection con1 =  (Connection)confSample.CreateConnection();
            con1.Start();

            //create session
            Session se1 = (Session)con1.CreateSession();

            //Notification of acquisition
      IDestination des = se1.GetTopic("topic.acquisitionCompleted");
            //IDestination des = se1.GetTopic("topic.acquisitionFailed");

            //Notification of image
            //IDestination des = se1.GetTopic("topic.imageCreated");
            //IDestination des = se1.GetTopic("topic.imageDeleted");

            //Notification of presentation State
            //IDestination des = se1.GetTopic("topic.presentationStateModified");
            //IDestination des = se1.GetTopic("topic.presentationStateDeleted");

            //Notification of analysis
            //IDestination des = se1.GetTopic("topic.analysisCreated");
            //IDestination des = se1.GetTopic("topic.analysisModified");
            //IDestination des = se1.GetTopic("topic.analysisModified");
            //IDestination des = se1.GetTopic("topic.analysisDeleted");

            //Notification of FMS
            //IDestination des = se1.GetTopic("topic.fmsCreated");
            //IDestination des = se1.GetTopic("topic.fmsModified");
            //IDestination des = se1.GetTopic("topic.fmsDeleted");

            //Notification of Volume
            //IDestination des = se1.GetTopic("topic.volumeCreated");

            //Notification of Report
            //IDestination des = se1.GetTopic("topic.reportCreated");
            //IDestination des = se1.GetTopic("topic.reportModified");
            //IDestination des = se1.GetTopic("topic.reportDeleted");

            //Notification of Patient
            //IDestination des = se1.GetTopic("topic.patientDeleted");

            //Notification of AppState change
            //IDestination des = se1.GetTopic("topic.appStateClosed");
            //IDestination des = se1.GetTopic("topic.appStateCreated");
            //IDestination des = se1.GetTopic("topic.appStateMinimized");
            //IDestination des = se1.GetTopic("topic.appStateRestored");
            //IDestination des = se1.GetTopic("tw");
            MessageConsumer consumerSample = (MessageConsumer)se1.CreateConsumer(des);

            //IMessage message = consumerSample.Receive(TimeSpan.FromMilliseconds(30000));
      consumerSample.Listener += new MessageListener(OnMessage);
      Console.ReadLine();
    }
    static void OnMessage(IMessage message)
    {
      Apache.NMS.ActiveMQ.Commands.ActiveMQTextMessage outputMsg = (Apache.NMS.ActiveMQ.Commands.ActiveMQTextMessage)message;
      Console.WriteLine("stop to check the output message: " + outputMsg.Text);

    }*/
      Config nc = new Config();
      if (nc.getHost() == null || nc.getPort() == 0)
        return;
      List<string> topics = nc.getTopics();
      StompConnection stomp = new StompConnection(nc.getHost(), nc.getPort());
      string logFile = string.Empty;
      if (nc.getLogPath() != null)
        logFile = nc.getLogPath();
      logFile = logFile.Substring(0, logFile.Length - 4);
      logFile = logFile + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".log";
      logFile = logFile.Replace("/", "_");
      logFile = logFile.Replace(":", "_");
      logFile = logFile.Replace(" ", "_");
      logFile = System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + "\\" + logFile;

      stomp.setLogpath(logFile);
      foreach (string topic in topics)
      {
        stomp.subscribe(topic, stomp.OnMessage);
      }
      Console.ReadLine();
    }

    class StompConnection
    {
      private static int count;
      private Session session;
      private string logPath = string.Empty;
      private string mcontent = string.Empty;
      public StompConnection(string host, int port)
      {
        Uri uriSample = new Uri("activemq:tcp://" + host + ":" + port);
        ConnectionFactory confSample = new ConnectionFactory(uriSample);
        Connection connection = (Connection)confSample.CreateConnection();
        connection.Start();
        //writeLog("Stomp Start:");
        //create session
        session = (Session)connection.CreateSession();
      }
            
      public void subscribe(string messageType, MessageListener msgListener)
      {
        try
        {
          IDestination destination = session.GetTopic(messageType);
          MessageConsumer consumer = (MessageConsumer)session.CreateConsumer(destination);
          consumer.Listener += new MessageListener(OnMessage);
        }
        catch (Exception e) { }
        }

      public void setLogpath(string path)
      {
        logPath = path;
      }

      public void OnMessage(IMessage message)
        {
        count++;
            Apache.NMS.ActiveMQ.Commands.ActiveMQTextMessage outputMsg = (Apache.NMS.ActiveMQ.Commands.ActiveMQTextMessage) message;
        mcontent = "No: " + count + " ::message: " + outputMsg.Text;
        Console.WriteLine(mcontent);
        writeLog(mcontent);
      }

      private void writeLog(string content)
      {
        if (logPath != null)
        {
          using (StreamWriter w = File.AppendText(logPath))
          {
            w.WriteLine("{0}", content);
            w.Flush();
            w.Close();
          }
        }
        else
          return;
      }
    }

    class Config
    {
      private XmlDocument doc;
      public Config()
      {
        doc = new XmlDocument();
        doc.Load(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + @"\NotificationConfig.xml");
      }

      public string getHost()
      {
        XmlNode xn = doc.SelectSingleNode("Config/Server/Host");
        string host = xn.InnerText.Trim();
        if (host != "")
          return host;
        else
          return null;
        }

      public int getPort()
      {
        XmlNode xn = doc.SelectSingleNode("Config/Server/Port");
        string port = xn.InnerText.Trim();
        if (port != "")
          return int.Parse(port);
        else
          return 0;
      }

      public List<string> getTopics()
      {
        List<string> topics = new List<string>();
        XmlNodeList xnl = doc.SelectNodes("Config/Topics/Name");
        foreach (XmlNode xn in xnl)
        {
          if (xn.Name.Trim() == "Name")
            topics.Add(xn.InnerText.Trim());
        }
        return topics;
      }

      public string getLogPath()
      {
        XmlNode xn = doc.SelectSingleNode("Config/Log/Enable");
        string enable = xn.InnerText.Trim();
        if (enable == "True")
        {
          xn = doc.SelectSingleNode("Config/Log/Path");
          return xn.InnerText.Trim();          
        }
        else
          return null;
      }
    }

    }
}
