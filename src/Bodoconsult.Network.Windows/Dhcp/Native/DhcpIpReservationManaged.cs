// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_IP_RESERVATION structure defines a client IP reservation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpIpReservationManaged : IDisposable
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the reserved IP address.
    /// </summary>
    public readonly DhcpIpAddress ReservedIpAddress;
    /// <summary>
    /// DHCP_CLIENT_UID structure that contains information on the client holding this IP reservation.
    /// </summary>
    private readonly IntPtr ReservedForClientPointer;

    public DhcpIpReservationManaged(DhcpIpAddress reservedIpAddress, DhcpClientUidManaged reservedForClient)
    {
        ReservedIpAddress = reservedIpAddress;
        ReservedForClientPointer = Marshal.AllocHGlobal(Marshal.SizeOf(reservedForClient));
        Marshal.StructureToPtr(reservedForClient, ReservedForClientPointer, false);
    }

    public DhcpIpReservationManaged(DhcpServerIpAddress address, DhcpServerHardwareAddress reservedForClient)
        : this(reservedIpAddress: address.ToNativeAsNetwork(),
            reservedForClient: reservedForClient.ToNativeClientUid())
    { }

    public void Dispose()
    {
        if (ReservedForClientPointer != IntPtr.Zero)
        {
            var reservedForClient = ReservedForClientPointer.MarshalToStructure<DhcpClientUidManaged>();
            reservedForClient.Dispose();

            Marshal.FreeHGlobal(ReservedForClientPointer);
        }
    }
}
