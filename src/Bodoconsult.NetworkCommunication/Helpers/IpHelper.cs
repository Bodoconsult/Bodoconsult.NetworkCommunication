// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Helpers;

/// <summary>
/// Helper class for TCP based / related methods
/// </summary>
public static class IpHelper
{
    /// <summary>
    /// Get the IP address of the local network interface
    /// </summary>
    /// <returns></returns>
    public static IPAddress GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork) // Nur IPv4-Adressen
            {
                return ip;
            }
        }

        throw new ArgumentException("NO IP address for TCP/IP v4 found");
    }

    /// <summary>
    /// Test if a TCP/IP connection to a certain IP host is possible for the requested port number
    /// </summary>
    /// <param name="ipAddress">Requested IP address</param>
    /// <param name="portNumber">Requested port number</param>
    /// <returns></returns>
    public static bool TestIpConnection(string ipAddress, int portNumber)
    {
        var success = false;

        var ipa = IPAddress.Parse(ipAddress);
        try
        {
            var sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(ipa, portNumber);
            if (sock.Connected)
            {
                success = true;
            }

            sock.Close();
        }
        catch //(System.Net.Sockets.SocketException ex)
        {
            success = false;
        }

        return success;
    }


    /// <summary>
    /// Test if a TCP/IP connection to a certain host is possible for the requested port number
    /// </summary>
    /// <param name="hostname">Requested host name</param>
    /// <param name="portNumber">Requested port number</param>
    /// <returns></returns>
    public static bool TestHostConnection(string hostname, int portNumber)
    {
        var success = false;

        var ipa = Dns.GetHostAddresses(hostname)[0];
        try
        {
            var sock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(ipa, portNumber);
            if (sock.Connected)
            {
                success = true;
            }

            sock.Close();
        }
        catch //(System.Net.Sockets.SocketException ex)
        {
            success = false;
        }

        return success;
    }

    /// <summary>
    /// Get the broadcast address
    /// </summary>
    /// <param name="address">IP address</param>
    /// <param name="subnetMask">subnet mask</param>
    /// <returns>IP address</returns>
    /// <exception cref="ArgumentException">Lengths of IP address and subnet mask do not match</exception>
    public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
    {
        var ipAdressBytes = address.GetAddressBytes();
        var subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
        {
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");
        }

        var broadcastAddress = new byte[ipAdressBytes.Length];
        for (var i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastAddress);
    }

    /// <summary>
    /// Get the network address
    /// </summary>
    /// <param name="address">IP address</param>
    /// <param name="subnetMask">subnet mask</param>
    /// <returns>IP address</returns>
    /// <exception cref="ArgumentException">Lengths of IP address and subnet mask do not match</exception>
    public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
    {
        var ipAdressBytes = address.GetAddressBytes();
        var subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
        {
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");
        }

        var broadcastAddress = new byte[ipAdressBytes.Length];
        for (var i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] & subnetMaskBytes[i]);
        }
        return new IPAddress(broadcastAddress);
    }

    /// <summary>
    /// Check if two address are in the same subnet
    /// </summary>
    /// <param name="address2">Address 2</param>
    /// <param name="address1">Address 1</param>
    /// <param name="subnetMask"></param>
    /// <returns></returns>
    public static bool IsInSameSubnet(this IPAddress address1, IPAddress address2, IPAddress subnetMask)
    {
        var network1 = address1.GetNetworkAddress(subnetMask);
        var network2 = address2.GetNetworkAddress(subnetMask);

        return network1.Equals(network2);
    }

    /// <summary>
    /// Check if a local port is available
    /// </summary>
    /// <param name="port">Local port to check</param>
    /// <returns>True if local port is free else false</returns>
    public static bool IsLocalPortAvailable(int port)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        foreach (var tcpi in tcpConnInfoArray)
        {
            if (tcpi.LocalEndPoint.Port == port)
            {
                return false;
            }
        }

        var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
        foreach (var endpoint in tcpListeners)
        {
            if (endpoint.Port == port)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a remote TCP port is open (accepting connections).
    /// </summary>
    /// <param name="ipAddress">Remote IP address (e.g., "192.168.1.1")</param>
    /// <param name="port">TCP port to check (0–65535)</param>
    /// <param name="timeoutMilliseconds">Connection timeout in ms (default: 5000 ms)</param>
    /// <returns>True if the port is open else false</returns>
    public static async Task<bool> IsRemotePortOpenAsync(
        string ipAddress,
        int port,
        int timeoutMilliseconds = 5000)
    {
        // Validate port range
        if (port is < 0 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), "IpHelper: Port must be between 0 and 65535.");
        }

        // Resolve IP address (handles hostnames like "example.com")
        if (!IPAddress.TryParse(ipAddress, out var ip))
        {
            var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
            ip = hostEntry.AddressList[0]; // Use the first IP (simplified)
        }

        using var client = new TcpClient();

        try
        {
            // Attempt connection with timeout
            var connectTask = client.ConnectAsync(ip, port);
            var timeoutTask = Task.Delay(timeoutMilliseconds);

            // Wait for either the connection to succeed or the timeout
            var completedTask = await Task.WhenAny(connectTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                // Timeout: Port is likely closed or unreachable
                return false;
            }

            // If connectTask completed, check for success
            await connectTask; // Throws if connection failed
            return true; // Connection succeeded: port is open
        }
        catch (SocketException ex)
        {
            // Handle specific socket errors
            switch (ex.SocketErrorCode)
            {
                case SocketError.ConnectionRefused:
                    // Server rejected connection (port closed)
                    return false;
                case SocketError.HostUnreachable:
                case SocketError.NetworkUnreachable:
                    // Network issue (e.g., no route to host)
                    return false;
                default:
                    // Unexpected error (e.g., firewall block)
                    Debug.Print($"IpHelper: Socket error: {ex.SocketErrorCode}");
                    return false;
            }
        }
        catch (Exception ex)
        {
            // Catch-all for unexpected errors (e.g., invalid IP)
            Debug.Print($"IpHelper: Error checking port: {ex.Message}");
            return false;
        }
    }
}