// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_BINARY_DATA structure defines an opaque blob of binary data.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpBinaryDataManaged : IDisposable
{
    /// <summary>
    /// Specifies the size of Data, in bytes.
    /// </summary>
    public readonly int DataLength;

    /// <summary>
    /// Pointer to an opaque blob of byte (binary) data.
    /// </summary>
    private readonly IntPtr DataPointer;

    public DhcpBinaryDataManaged(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            DataLength = 0;
            DataPointer = IntPtr.Zero;
        }
        else
        {
            DataLength = data.Length;
            DataPointer = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, DataPointer, data.Length);
        }
    }

    public DhcpBinaryDataManaged(ulong hwAddr1, ulong hwAddr2, int dataLength)
    {
        if (dataLength == 0)
        {
            DataLength = 0;
            DataPointer = IntPtr.Zero;
        }
        else
        {
            DataLength = dataLength;
            DataPointer = Marshal.AllocHGlobal(dataLength > 8 ? 16 : 8);

            hwAddr1 = BitHelper.HostToNetworkOrder(hwAddr1);
            Marshal.WriteInt64(DataPointer, (long)hwAddr1);

            if (dataLength > 8)
            {
                hwAddr2 = BitHelper.HostToNetworkOrder(hwAddr2);
                Marshal.WriteInt64(DataPointer + 8, (long)hwAddr2);
            }
        }
    }

    public void Dispose()
    {
        if (DataPointer != IntPtr.Zero)
            Marshal.FreeHGlobal(DataPointer);
    }
}
