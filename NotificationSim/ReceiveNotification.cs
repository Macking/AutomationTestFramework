using System;
using System.Collections.Generic;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace NotificationSim
{
  public class ReceiveNotification
  {
    private Session session;
    private Connection connection;
    private int ReceivedNumber;
    private bool isFinished;
    private string stopWithMessage;
    private List<string> ReceivedMessages;
    private List<string> ListionTopics;

    public ReceiveNotification()
    {
      init("localhost",61616);
    }

    public ReceiveNotification(string host, int port)
    {
      init(host,port);
    }

    private void init(string host, int port)
    {
      ReceivedNumber = 0;
      isFinished = false;
      ReceivedMessages = new List<string>();
      ListionTopics = new List<string>();
      
      Uri uriSample = new Uri("activemq:tcp://" + host + ":" + port);
      ConnectionFactory confSample = new ConnectionFactory(uriSample);
      connection = (Connection)confSample.CreateConnection();
      connection.Start();
      session = (Session)connection.CreateSession();
    }

    public int getNotificationNumber()
    {
      if (isFinished)
        return ReceivedNumber;
      else
        return -1;
    }

    public List<string> getNotificationContent()
    {
      if (isFinished)
        return ReceivedMessages;
      else
        return new List<string>();
    }

    public void startListon(List<string> topics, string stopTopic, string stopContent)
    {
      foreach (string topic in topics)
      {
        ListionTopics.Add(topic);
      }
      ListionTopics.Add(stopTopic);
      stopWithMessage = stopContent;
      System.Threading.Thread tx = new System.Threading.Thread(startListonMessage);
      tx.Start();
    }

    private void startListonMessage()
    {
      foreach (string topic in ListionTopics)
      {
        subscribe(topic, OnMessage);
      }      
    }

    private void OnMessage(IMessage message)
    {
      ReceivedNumber++;
      Apache.NMS.ActiveMQ.Commands.ActiveMQTextMessage outputMsg = (Apache.NMS.ActiveMQ.Commands.ActiveMQTextMessage)message;
      //Console.WriteLine("Message Content:" + outputMsg.Text);
      if (stopWithMessage != null && stopWithMessage == outputMsg.Text)
      {
        session.Close();
        connection.Close();
        isFinished = true;
      }
      else
      {
        ReceivedMessages.Add(outputMsg.Text);
      }
      //mcontent = "No: " + ReceivedNumber + " ::message: " + outputMsg.Text;
      //Console.WriteLine(mcontent);
    }

    private void subscribe(string messageType, MessageListener msgListener)
    {
      try
      {
        IDestination destination = session.GetTopic(messageType);
        MessageConsumer consumer = (MessageConsumer)session.CreateConsumer(destination);
        consumer.Listener += new MessageListener(OnMessage);
      }
      catch (Exception e) { }
    }


  }
}
