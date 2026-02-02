using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_IP_RESERVATION_V4 structure defines a client IP reservation. This structure extends an IP reservation by including the type of client (DHCP or BOOTP) holding the reservation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpIpReservationV4 : IDisposable
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the reserved IP address.
    /// </summary>
    public readonly DhcpIpAddress ReservedIpAddress;
    /// <summary>
    /// DHCP_CLIENT_UID structure that contains the hardware address (MAC address) of the DHCPv4 client that holds this reservation.
    /// </summary>
    private readonly IntPtr ReservedForClientPointer;
    /// <summary>
    /// Value that specifies the DHCPv4 reserved client type.
    /// </summary>
    public readonly DhcpClientType AllowedClientTypes;

    /// <summary>
    /// DHCP_CLIENT_UID structure that contains the hardware address (MAC address) of the DHCPv4 client that holds this reservation.
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
