using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_BIND_ELEMENT_ARRAY structure defines an array of network binding elements used by a DHCP server.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpBindElementArray : IDisposable
{
    /// <summary>
    /// Specifies the number of network binding elements listed in Elements.
    /// </summary>
    public readonly int NumElements;
    /// <summary>
    /// Specifies an array of DHCP_BIND_ELEMENT structures
    /// </summary>
    private readonly IntPtr ElementsPointer;

    /// <summary>
    /// Specifies an array of DHCP_BIND_ELEMENT structures
    /// </summary>
    public IEnumerable<DhcpBindElement> Elements
    {
        get
        {
            if (NumElements == 0 || ElementsPointer == IntPtr.Zero)
                yield break;

            var iter = ElementsPointer;
            var size = Marshal.SizeOf(typeof(DhcpBindElement));
            for (var i = 0; i < NumElements; i++)
            {
                yield return iter.MarshalToStructure<DhcpBindElement>();
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
