using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SUBNET_ELEMENT_DATA_V5 structure defines an element that describes a feature or restriction of a subnet. Together, a set of elements describes the set of IP addresses served for a subnet by DHCP or BOOTP. DHCP_SUBNET_ELEMENT_DATA_V5 specifically allows for the definition of BOOTP-served addresses.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSubnetElementDataV5 : IDisposable
{
    /// <summary>
    /// DHCP_SUBNET_ELEMENT_TYPE enumeration value describing the type of element in the subsequent field.
    /// </summary>
    public readonly DhcpSubnetElementType ElementType;

    private readonly IntPtr ElementPointer;

    /// <summary>
    /// DHCP_BOOTP_IP_RANGE structure that contains the set of BOOTP-served IP addresses. This member is present if ElementType is set to DhcpIpRangesBootpOnly.
    /// </summary>
    /// <returns></returns>
    public DhcpBootpIpRange ReadBootpIpRange() => ElementPointer.MarshalToStructure<DhcpBootpIpRange>();

    /// <summary>
    /// DHCP_HOST_INFO structure that contains the IP addresses of secondary DHCP servers available on the subnet. This member is present if ElementType is set to DhcpSecondaryHosts.
    /// </summary>
    /// <returns></returns>
    public DhcpHostInfo ReadSecondaryHost() => ElementPointer.MarshalToStructure<DhcpHostInfo>();

    /// <summary>
    /// DHCP_IP_RESERVATION_V4 structure that contains the set of reserved IP addresses for the subnet. This member is present if ElementType is set to DhcpReservedIps.
    /// </summary>
    /// <returns></returns>
    public DhcpIpReservationV4 ReadReservedIp() => ElementPointer.MarshalToStructure<DhcpIpReservationV4>();

    /// <summary>
    /// DHCP_IP_RANGE structure that contains a range of IP addresses. This member is present if ElementType is set to DhcpIpRanges or DhcpExcludedIpRanges.
    /// </summary>
    /// <returns></returns>
    public DhcpIpRange ReadIpRange() => ElementPointer.MarshalToStructure<DhcpIpRange>();

    public void Dispose()
    {
        if (ElementPointer != IntPtr.Zero)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            switch (ElementType)
            {
                case DhcpSubnetElementType.DhcpSecondaryHosts:
                    ReadSecondaryHost().Dispose();
                    break;
                case DhcpSubnetElementType.DhcpReservedIps:
                    ReadReservedIp().Dispose();
                    break;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            Api.FreePointer(ElementPointer);
        }
    }
}
