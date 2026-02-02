// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;

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
        var ipHost = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddr = ipHost.AddressList[0];
        return ipAddr;
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
            var sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
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
            var sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork,
                System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
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
            broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
        }
        return new IPAddress(broadcastAddress);
    }

    public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
    {
        var network1 = address.GetNetworkAddress(subnetMask);
        var network2 = address2.GetNetworkAddress(subnetMask);

        return network1.Equals(network2);
    }
}