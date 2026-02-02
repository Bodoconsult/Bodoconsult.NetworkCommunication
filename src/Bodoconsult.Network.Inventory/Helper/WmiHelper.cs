using System;
using System.Management;

namespace Bodoconsult.Inventory.Helper;

/// <summary>
/// Helping functionality for WMI access to remote computers
/// </summary>
public class WmiHelper
{

    /// <summary>
    /// Get connection options for WMI access depending on Ip and domain
    /// </summary>
    /// <param name="ip">IP address to connect to</param>
    /// <param name="localIp">Local IP address</param>
    /// <param name="domain">Domain name</param>
    /// <param name="username">User name</param>
    /// <param name="password">Password</param>
    /// <returns></returns>
    public static ConnectionOptions GetWmiConnectionOptions(string ip, string localIp, string domain, string username, string password)
    {
        ConnectionOptions connectionOptions;

        if (ip == localIp)
        {
            connectionOptions = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                EnablePrivileges = true,
                Timeout = new TimeSpan(0, 0, 2, 0)
            };
        }
        else
        {
            if (string.IsNullOrEmpty(domain))
            {
                connectionOptions = new ConnectionOptions
                {
                    Username = username,
                    Password = PasswordHelper.DecryptPassword(password),
                    Timeout = new TimeSpan(0, 0, 2, 0),
                    EnablePrivileges = true
                };
            }
            else
            {
                connectionOptions = new ConnectionOptions
                {
                    Authority = $"NTLMDOMAIN:{domain}",
                    Username = username,
                    Password = PasswordHelper.DecryptPassword(password),
                    Timeout = new TimeSpan(0, 0, 2, 0),
                    EnablePrivileges = true
                };
            }
        }

        return connectionOptions;
    }
}