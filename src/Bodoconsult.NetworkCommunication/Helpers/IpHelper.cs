// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
    /// Check if a port is available
    /// </summary>
    /// <param name="port">Port to check</param>
    /// <returns>True if port is free else false</returns>
    public static bool IsPortAvailable(int port)
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
}