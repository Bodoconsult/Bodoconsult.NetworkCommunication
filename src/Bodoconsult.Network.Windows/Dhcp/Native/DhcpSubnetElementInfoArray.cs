// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SUBNET_ELEMENT_INFO_ARRAY structure defines an array of subnet element data.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSubnetElementInfoArray : IDisposable
{
    /// <summary>
    /// Specifies the number of elements in Elements.
    /// </summary>
    public readonly int NumElements;

    /// <summary>
    /// Pointer to a list of DHCP_SUBNET_ELEMENT_DATA structures that contain the data for the corresponding subnet elements.
    /// </summary>
    private readonly IntPtr ElementsPointer;

    /// <summary>
    /// Pointer to a list of DHCP_SUBNET_ELEMENT_DATA structures that contain the data for the corresponding subnet elements.
    /// </summary>
    public IEnumerable<DhcpSubnetElementData> Elements
    {
        get
        {
            if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                yield break;

            var iter = ElementsPointer;
            var size = Marshal.SizeOf(typeof(DhcpSubnetElementData));
            for (var i = 0; i < NumElements; i++)
            {
                yield return iter.MarshalToStructure<DhcpSubnetElementData>();
                iter += size;
            }
        }
    }

    public void Dispose()
    {
        foreach (var element in Elements)
            element.Dispose();

        Api.FreePointer(ElementsPointer);
    }
}
