// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SUBNET_ELEMENT_DATA_V5 structure defines an element that describes a feature or restriction of a subnet. Together, a set of elements describes the set of IP addresses served for a subnet by DHCP or BOOTP. DHCP_SUBNET_ELEMENT_DATA_V5 specifically allows for the definition of BOOTP-served addresses.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSubnetElementDataV5Managed : IDisposable
{
    /// <summary>
    /// DHCP_SUBNET_ELEMENT_TYPE enumeration value describing the type of element in the subsequent field.
    /// </summary>
    public readonly DhcpSubnetElementType ElementType;

    private readonly IntPtr ElementPointer;

    public DhcpSubnetElementDataV5Managed(DhcpSubnetElementType elementType, DhcpBootpIpRange element)
    {
        if (elementType != DhcpSubnetElementType.DhcpIpRangesDhcpOnly &&
            elementType != DhcpSubnetElementType.DhcpIpRangesDhcpBootp &&
            elementType != DhcpSubnetElementType.DhcpIpRangesBootpOnly)
        {
            throw new ArgumentOutOfRangeException(nameof(elementType));
        }

        ElementType = elementType;
        ElementPointer = Marshal.AllocHGlobal(Marshal.SizeOf(element));
        Marshal.StructureToPtr(element, ElementPointer, false);
    }

    public DhcpSubnetElementDataV5Managed(DhcpSubnetElementType elementType, DhcpIpReservationV4Managed element)
    {
        if (elementType != DhcpSubnetElementType.DhcpReservedIps)
            throw new ArgumentOutOfRangeException(nameof(elementType));

        ElementType = elementType;
        ElementPointer = Marshal.AllocHGlobal(Marshal.SizeOf(element));
        Marshal.StructureToPtr(element, ElementPointer, false);
    }

    public DhcpSubnetElementDataV5Managed(DhcpSubnetElementType elementType, DhcpIpRange element)
    {
        if (elementType != DhcpSubnetElementType.DhcpExcludedIpRanges)
            throw new ArgumentOutOfRangeException(nameof(elementType));

        ElementType = elementType;
        ElementPointer = Marshal.AllocHGlobal(Marshal.SizeOf(element));
        Marshal.StructureToPtr(element, ElementPointer, false);
    }

    public void Dispose()
    {
        if (ElementPointer != IntPtr.Zero)
        {
            switch (ElementType)
            {
                case DhcpSubnetElementType.DhcpReservedIps:
                    var reservedIp = ElementPointer.MarshalToStructure<DhcpIpReservationV4Managed>();
                    reservedIp.Dispose();
                    break;
            }

            Marshal.FreeHGlobal(ElementPointer);
        }
    }
}
