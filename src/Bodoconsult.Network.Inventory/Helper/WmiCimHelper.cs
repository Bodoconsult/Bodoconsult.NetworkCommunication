using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

namespace Bodoconsult.Inventory.Helper;

/// <summary>
/// Helper class for WMI access via Microsoft.Management.Infrastructure
/// </summary>
public class WmiCimHelper
{
    /// <summary>
    /// Get a CIM session for WMI access to computer
    /// </summary>
    /// <param name="domain">name of the domain</param>
    /// <param name="computer">host name or IP address</param>
    /// <param name="username">username to use for WMI access</param>
    /// <param name="password">password to use for WMI access</param>
    /// <returns></returns>
    public static CimSession GetSession(string domain, string computer, string username, string password)
    {
        var securepassword = new SecureString();
        foreach (var c in PasswordHelper.DecryptPassword(password))
        {
            securepassword.AppendChar(c);
        }

        //Debug.Print("'" + computer + "'");
        //Debug.Print("'"+domain+"'");
        //Debug.Print("'" + username+ "'");
        //Debug.Print("'" + password + "'");

        // create Credentials

        var credentials = new CimCredential(PasswordAuthenticationMechanism.Default,
            domain,
            username,
            securepassword);



        // create SessionOptions using Credentials
        var sessionOptions = new WSManSessionOptions();
        sessionOptions.AddDestinationCredentials(credentials);

        // create Session using computer, SessionOptions
        var session = CimSession.Create(computer.Trim(), sessionOptions);
        return session;
    }

    /// <summary>
    /// Execute a WQL query via CIM
    /// </summary>
    /// <param name="session"></param>
    /// <param name="namespacePath"></param>
    /// <param name="wql"></param>
    /// <returns></returns>
    public static IEnumerable<CimInstance> ExecuteQuery(CimSession session, string namespacePath, string wql)
    {
        try
        {
            return session?.QueryInstances(namespacePath, "WQL", wql).ToList();
        }
        catch (Exception e)
        {
            Debug.Print("ExecuteQuery:Error:{0}: {1}: {2}", namespacePath, wql, e.Message);
            return null;
        }
    }
}