// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// https://learn.microsoft.com/en-us/archive/blogs/knom/ip-address-calculations-with-c-subnetmasks-networks

using System.Net;

namespace Bodoconsult.NetworkCommunication.Helpers;

/// <summary>
/// Class representing a subnet mask
/// </summary>
public static class SubnetMask
{
    /// <summary>
    /// Class A subnet mask 255.0.0.0
    /// </summary>
    public static readonly IPAddress ClassA = IPAddress.Parse("255.0.0.0");

    /// <summary>
    /// Class B subnet mask 255.255.0.0"
    /// </summary>
    public static readonly IPAddress ClassB = IPAddress.Parse("255.255.0.0");

    /// <summary>
    /// Class C subnet mask 255.255.255.0
    /// </summary>
    public static readonly IPAddress ClassC = IPAddress.Parse("255.255.255.0");

    /// <summary>
    /// Create by host part bit length
    /// </summary>
    /// <param name="hostpartLength">Host part length</param>
    /// <returns><see cref="IPAddress"/> instance</returns>
    /// <exception cref="ArgumentException"></exception>
    public static IPAddress CreateByHostBitLength(int hostpartLength)
    {
        var netPartLength = 32 - hostpartLength;

        if (netPartLength < 2)
        {
            throw new ArgumentException("Number of hosts is to large for IPv4");
        }

        var binaryMask = new byte[4];

        for (var i = 0; i < 4; i++)
        {
            if (i * 8 + 8 <= netPartLength)
            {
                binaryMask[i] = 255;
            }
            else if (i * 8 > netPartLength)
            {
                binaryMask[i] = 0;
            }
            else
            {
                var oneLength = netPartLength - i * 8;
                var binaryDigit = string.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
                binaryMask[i] = Convert.ToByte(binaryDigit, 2);
            }
        }
        return new IPAddress(binaryMask);
    }

    /// <summary>
    /// Create IP address by net bit length
    /// </summary>
    /// <param name="netpartLength">Net part length</param>
    /// <returns><see cref="IPAddress"/> instance</returns>
    public static IPAddress CreateByNetBitLength(int netpartLength)
    {
        var hostPartLength = 32 - netpartLength;
        return CreateByHostBitLength(hostPartLength);
    }

    /// <summary>
    /// Create IP address by host number
    /// </summary>
    /// <param name="numberOfHosts">Number of hosts</param>
    /// <returns><see cref="IPAddress"/> instance</returns>
    public static IPAddress CreateByHostNumber(int numberOfHosts)
    {
        var maxNumber = numberOfHosts + 1;

        var b = Convert.ToString(maxNumber, 2);

        return CreateByHostBitLength(b.Length);
    }
}