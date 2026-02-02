using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Bodoconsult.Inventory.Helper;

namespace Bodoconsult.Inventory.Handler;

public class WindowsServerHandler
{


    /// <summary>
    /// IP address of the item
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// Domain of the user to access WMI with
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Username of the user to access WMI with
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password of the user to access WMI with
    /// </summary>
    public string Password { get; set; }


    /// <summary>
    /// Represents the state of a request to the Server Manager.
    /// </summary>
    public enum ServerManagerRequestState : byte
    {
        /// <summary>
        /// The request is in progress.
        /// </summary>
        InProgress = 0,

        /// <summary>
        /// The request has completed.
        /// </summary>
        Completed = 1,

        /// <summary>
        /// The request has failed.
        /// </summary>
        Failed = 2,

    }


    /// <summary>
    /// Represents the installation state of a component (server role or feature).
    /// </summary>
    public enum ServerManagerInstalledState : byte
    {
        /// <summary>
        /// The component is not installed.
        /// </summary>
        NotInstalled = 0,

        /// <summary>
        /// The component is installed.
        /// </summary>
        Installed = 1,

        /// <summary>
        /// The component has been installed but requires a reboot to complete.
        /// </summary>
        RebootRequired = 3,

    }



    /// <summary>
    /// Queries the state of the Windows server components.
    /// </summary>
    public IList<string> GetInstalledRoles()
    {
        var roles = new List<string>();
        var localIp = NetworkHelper.GetLocalIpAddress();

        var options = WmiHelper.GetWmiConnectionOptions(Ip, localIp, Domain, Username, Password);
        var scope = new ManagementScope($@"\\{Ip}\root\Microsoft\Windows\ServerManager", options);
        var tasks = new ManagementClass(scope, new ManagementPath(@"MSFT_ServerManagerDeploymentTasks"), null);
        var requestGuid = new ManagementClass(scope, new ManagementPath(@"MSFT_ServerManagerRequestGuid"), null);
        requestGuid.Properties["HighHalf"].Value = 0;
        requestGuid.Properties["LowHalf"].Value = 0;
        var parameters = tasks.GetMethodParameters("GetServerComponentsAsync");
        parameters["RequestGuid"] = requestGuid;
        var results = tasks.InvokeMethod("GetServerComponentsAsync", parameters, null);
        var enumerationState = (ManagementBaseObject)results.Properties["EnumerationState"].Value;
        var requestState = (ServerManagerRequestState)enumerationState.Properties["RequestState"].Value;
        if (requestState != ServerManagerRequestState.Completed) { throw new InvalidOperationException(
            $"Could not determine the installed server components - state {requestState}."); }
        var serverComponents = (ManagementBaseObject[])results.Properties["ServerComponents"].Value;
        Log(serverComponents.Length.ToString());
        foreach (var serverComponent in serverComponents)
        {




            if ((byte) serverComponent.Properties["Installed"].Value == 1)
            {

                //foreach (var prop in serverComponent.Properties)
                //{
                //    if (prop.Value != null) Debug.Print(prop.Name.ToString() + ": " + prop.Value.ToString());
                //}

                if ( (byte)serverComponent.Properties["FeatureType"].Value == 0)
                {
                    roles.Add((string)serverComponent.Properties["Displayname"].Value);
                }

                    
            }

            //var displayName = (string)serverComponent.Properties["Displayname"].Value;
            //var installed = (ServerManagerInstalledState)serverComponent.Properties["Installed"].Value;
            //Log(displayName + " " + installed.ToString());
        }

        return roles;
    }


    /// <summary>
    /// Writes the specified message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void Log(string message)
    {
        Debug.Print(message + Environment.NewLine);
    }


}