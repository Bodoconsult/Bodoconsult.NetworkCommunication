// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SUBNET_ELEMENT_DATA structure defines an element that describes a feature or restriction of a subnet. Together, a set of elements describes the set of IP addresses served for a subnet by DHCP.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSubnetElementData : IDisposable
{
    /// <summary>
    /// DHCP_SUBNET_ELEMENT_TYPE enumeration value describing the type of element in the subsequent field.
    /// </summary>
    public readonly DhcpSubnetElementType ElementType;

    private readonly IntPtr ElementPointer;

    /// <summary>
    /// DHCP_IP_RANGE structure that contains the set of DHCP-served IP addresses. This member is present if ElementType is set to DhcpIpRanges.
    /// </summary>
    /// <returns></returns>
    public DhcpIpRange ReadIpRange() => ElementPointer.MarshalToStructure<DhcpIpRange>();

    /// <summary>
    /// DHCP_HOST_INFO structure that contains the IP addresses of secondary DHCP servers available on the subnet. This member is present if ElementType is set to DhcpSecondaryHosts.
    /// </summary>
    /// <returns></returns>
    public DhcpHostInfo ReadSecondaryHost() => ElementPointer.MarshalToStructure<DhcpHostInfo>();

    /// <summary>
    /// DHCP_IP_RESERVATION structure that contains the set of reserved IP addresses for the subnet. This member is present if ElementType is set to DhcpExcludedIpRanges.
    /// </summary>
    /// <returns></returns>
    public DhcpIpReservation ReadReservedIp() => ElementPointer.MarshalToStructure<DhcpIpReservation>();

    /// <summary>
    /// DHCP_IP_CLUSTER structure that contains the set of clusters within the subnet. This member is present if ElementType is set to DhcpIpUsedClusters.
    /// </summary>
    /// <returns></returns>
    public DhcpIpCluster ReadIpUsedCluster() => ElementPointer.MarshalToStructure<DhcpIpCluster>();

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
