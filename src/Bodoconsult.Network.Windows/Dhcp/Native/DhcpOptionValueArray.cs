// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_OPTION_VALUE_ARRAY structure defines a list of DHCP option values (just the option data with associated ID tags).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpOptionValueArray : IDisposable
{
    /// <summary>
    /// Specifies the number of option values listed in Values.
    /// </summary>
    public readonly int NumElements;
    /// <summary>
    /// Pointer to a list of DHCP_OPTION_VALUE structures containing DHCP option values.
    /// </summary>
    private readonly IntPtr ValuesPointer;
    /// <summary>
    /// Pointer to a list of DHCP_OPTION_VALUE structures containing DHCP option values.
    /// </summary>
    public IEnumerable<DhcpOptionValue> Values
    {
        get
        {
            if (NumElements == 0 || ValuesPointer == IntPtr.Zero)
                yield break;

            var iter = ValuesPointer;
            var size = Marshal.SizeOf(typeof(DhcpOptionValue));
            for (var i = 0; i < NumElements; i++)
            {
                yield return iter.MarshalToStructure<DhcpOptionValue>();
                iter += size;
            }
        }
    }

    public void Dispose()
    {
        foreach (var value in Values)
            value.Dispose();

        Api.FreePointer(ValuesPointer);
    }
}
