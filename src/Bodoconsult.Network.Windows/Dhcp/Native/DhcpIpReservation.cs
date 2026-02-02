// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_IP_RESERVATION structure defines a client IP reservation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpIpReservation : IDisposable
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the reserved IP address.
    /// </summary>
    public readonly DhcpIpAddress ReservedIpAddress;
    /// <summary>
    /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
    /// </summary>
    private readonly IntPtr ReservedForClientPointer;

    /// <summary>
    /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
    /// </summary>
    public DhcpClientUid ReservedForClient => ReservedForClientPointer.MarshalToStructure<DhcpClientUid>();

    public void Dispose()
    {
        if (ReservedForClientPointer != IntPtr.Zero)
        {
            ReservedForClient.Dispose();
            Api.FreePointer(ReservedForClientPointer);
        }
    }
}
