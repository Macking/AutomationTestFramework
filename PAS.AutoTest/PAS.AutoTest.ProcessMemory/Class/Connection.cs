using System;
using System.Management;
using System.Collections.Generic;
using System.Text;

namespace PAS.AutoTest.ProcessMemory
{
  class Connection
  {
    ManagementScope connectionScope;
    ConnectionOptions options;

    #region "properties"
    public ManagementScope GetConnectionScope
    {
      get { return connectionScope; }
    }
    public ConnectionOptions GetOptions
    {
      get { return options; }
    }
    #endregion

    #region "static helpers"
    public static ConnectionOptions SetConnectionOptions()
    {
      ConnectionOptions options = new ConnectionOptions();
      options.Impersonation = ImpersonationLevel.Impersonate;
      options.Authentication = AuthenticationLevel.Default;
      options.EnablePrivileges = true;
      return options;
    }

    public static ManagementScope SetConnectionScope(string machineName,
                                               ConnectionOptions options)
    {
      ManagementScope connectScope = new ManagementScope();
      connectScope.Path = new ManagementPath(@"\\" + machineName + @"\root\CIMV2");
      connectScope.Options = options;

      try
      {
        connectScope.Connect();
      }
      catch (ManagementException e)
      {
        Console.WriteLine("An Error Occurred: " + e.Message.ToString());
      }
      return connectScope;
    }
    #endregion

    #region "constructors"
    public Connection()
    {
      EstablishConnection(null, null, null, Environment.MachineName);
    }

    public Connection(string userName,
                      string password,
                      string domain,
                      string machineName)
    {
      EstablishConnection(userName, password, domain, machineName);
    }
    #endregion

    #region "private helpers"
    private void EstablishConnection(string userName, string password, string domain, string machineName)
    {
      options = Connection.SetConnectionOptions();
      if (domain != null || userName != null)
      {
        options.Username = domain + "\\" + userName;
        options.Password = password;
      }
      connectionScope = Connection.SetConnectionScope(machineName, options);
    }
    #endregion
  }
}
