// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_IP_ARRAY structure defines an array of IP addresses.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpIpArrayManaged : IDisposable
{
    /// <summary>
    /// Specifies the number of IP addresses in Elements.
    /// </summary>
    public readonly uint NumElements;
    /// <summary>
    /// Pointer to a list of DHCP_IP_ADDRESS values.
    /// </summary>
    private readonly IntPtr ElementsPointer;

    public IEnumerable<DhcpIpAddress> Elements
    {
        get
        {
            if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                yield break;

            var iter = ElementsPointer;
            var size = Marshal.SizeOf(typeof(DhcpIpAddress));
            for (var i = 0; i < NumElements; i++)
            {
                yield return iter.MarshalToStructure<DhcpIpAddress>();
                iter += size;
            }
        }
    }

    public DhcpIpArrayManaged(List<DhcpIpAddress> elements)
    {
        var elementSize = Marshal.SizeOf(typeof(DhcpIpAddress));
        var elementsPointer = Marshal.AllocHGlobal(elementSize * elements.Count);

        NumElements = (uint)elements.Count;
        ElementsPointer = elementsPointer;

        foreach (var element in elements)
        {
            Marshal.StructureToPtr(element, elementsPointer, true);
            elementsPointer += elementSize;
        }
    }

    public DhcpIpArrayManaged(DhcpIpAddress element)
    {
        var elementSize = Marshal.SizeOf(typeof(DhcpIpAddress));
        var elementsPointer = Marshal.AllocHGlobal(elementSize);

        NumElements = 1;
        ElementsPointer = elementsPointer;

        Marshal.StructureToPtr(element, elementsPointer, true);
    }

    public void Dispose()
    {
        if (ElementsPointer != IntPtr.Zero)
            Marshal.FreeHGlobal(ElementsPointer);
    }
}
