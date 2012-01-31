using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//using AutoTestComponent;

namespace AutoTestComponent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QCOTAClass qcOnline = new QCOTAClass();
        TestConfigFile tConfig = new TestConfigFile();
        TestConfigUtility t = new TestConfigUtility();
        private bool isAutoSelectDomain = false;
        private bool isAutoSelectProject = false;
        private string strDomainName = string.Empty;
        private string strProjectName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadConfig_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(configFilePath.Text))
            {
                
                //bool success = tConfig.Load(configFilePath.Text);
                
                bool success = t.Load(configFilePath.Text);
                if (success)
                {
                    tConfig = t.TestConfigString;
                    qcServerAddr.Text = tConfig.QCConnect.ServerAddr;

                    qcLoginName.Text = tConfig.QCConnect.LoginName;
                    qcLoginPassword.Text = tConfig.QCConnect.Password;
                    strDomainName = tConfig.QCConnect.Domain;
                    strProjectName = tConfig.QCConnect.Project;
                    
                    isAutoSelectDomain = true;
                    qcDomain.Items.Clear();
                    qcDomain.Items.Add(strDomainName);
                    //qcDomain.SelectedIndex = 0;
                    qcDomain.SelectedItem = strDomainName; 
                    isAutoSelectDomain = false;

                    isAutoSelectProject = true;
                    qcProject.Items.Clear();
                    qcProject.Items.Add(strProjectName);                    
                    //qcProject.SelectedIndex = 0;
                    qcProject.SelectedItem = strProjectName;
                    isAutoSelectProject = false;
                }
                else
                    MessageBox.Show("Can't serialize this XML file!");
            }
            else
            {
                MessageBox.Show("Can't load file!");
            }
            //FileStream fs = new FileStream();

        }

        private void saveConfig_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Save file!");
            t.TestConfigString.QCConnect.Domain = strDomainName;
            t.TestConfigString.QCConnect.Project = strProjectName;
            t.TestConfigString.QCConnect.ServerAddr = qcServerAddr.Text;
            t.TestConfigString.QCConnect.LoginName = qcLoginName.Text;
            t.TestConfigString.QCConnect.Password = qcLoginPassword.Text;

            if (System.IO.File.Exists(configFilePath.Text))
            {
                switch (MessageBox.Show("Do you want to overwrite the config file?", "Please confirm", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        //TestConfigFile tConfig = new TestConfigFile();
                        //tConfig = t.TestConfigString;
                        t.Save(configFilePath.Text);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                    default:
                        break;
                }
            }
            else
            {
                t.Save(configFilePath.Text);
            }
        }

        private void qcLogin_Click(object sender, RoutedEventArgs e)
        {
            if (qcLogin.Content.ToString() == "Login")
            {
                qcOnline.Login(qcServerAddr.Text,qcLoginName.Text,qcLoginPassword.Text);
                if(qcOnline.QCConnected)
                    qcLogin.Content = "Logout";
                
                List<string> domainlist = new List<string>();
                domainlist = qcOnline.getDomainList();

                isAutoSelectDomain = true;
                qcDomain.Items.Clear();
                isAutoSelectDomain = false;

                foreach (string d in domainlist)
                {
                    if (d == strDomainName)
                    {
                        qcDomain.SelectedItem = d;
                    }
                    qcDomain.Items.Add(d);
                }
            }
            else
            {
                qcOnline.Logout();
                if (!qcOnline.QCConnected)
                    qcLogin.Content = "Login";
                //string selectDomain = qcDomain.SelectedItem.ToString();
                isAutoSelectDomain = true;
                qcDomain.Items.Clear();
                qcDomain.Items.Add(strDomainName);
                //qcDomain.SelectedIndex = 0;
                qcDomain.SelectedItem = strDomainName;
                isAutoSelectDomain = false;

                isAutoSelectProject = true;
                qcProject.Items.Clear();
                qcProject.Items.Add(strProjectName);
                //qcProject.SelectedIndex = 0;
                qcProject.SelectedItem = strProjectName;
                isAutoSelectProject = false;
            }
        }

        private void qcConnect_Click(object sender, RoutedEventArgs e)
        {
            if (qcConnect.Content.ToString() == "Connect")
            {
                if (qcOnline.QCConnected)
                {
                    qcOnline.Connect(qcDomain.SelectedItem.ToString(), qcProject.SelectedItem.ToString());
                    if (qcOnline.ProjConnected)
                        qcConnect.Content = "DisConnect";
                }
                else
                {
                    qcOnline.Connect(qcServerAddr.Text,qcLoginName.Text,qcLoginPassword.Text,
                        qcDomain.SelectedItem.ToString(), qcProject.SelectedItem.ToString());
                    if (qcOnline.QCConnected && qcOnline.ProjConnected)
                        qcConnect.Content = "DisConnect";
                }
            }
            else
            {
                if (qcOnline.ProjConnected)
                {
                    qcOnline.DisConnect(true);
                    if (!qcOnline.ProjConnected && !qcOnline.QCConnected)
                        qcConnect.Content = "Connect";
                }
            }
        }
       
        private void qcDomain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isAutoSelectDomain)
            {                
                List<string> projectList = new List<string>();
                projectList = qcOnline.getProjectList(qcDomain.SelectedItem.ToString());
                
                isAutoSelectProject = true;
                qcProject.Items.Clear();
                isAutoSelectProject = false;

                foreach (string d in projectList)
                {
                    qcProject.Items.Add(d);
                    if (d == strProjectName)
                    {
                        isAutoSelectProject = true;
                        qcProject.SelectedItem = d;
                        isAutoSelectProject = false;
                    }
                }

                strDomainName = qcDomain.SelectedItem.ToString();
            }
        }

        private void qcProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isAutoSelectProject)
            {
                //strProjectName = ((ComboBox)sender).SelectedItem.ToString();
                strProjectName = qcProject.SelectedItem.ToString();
            }
        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
