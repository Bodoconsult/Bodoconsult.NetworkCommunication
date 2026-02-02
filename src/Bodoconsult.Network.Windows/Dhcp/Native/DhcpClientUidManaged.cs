// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpClientUidManaged : IDisposable
{
    /// <summary>
    /// Specifies the size of Data, in bytes.
    /// </summary>
    public readonly int DataLength;

    /// <summary>
    /// Pointer to an opaque blob of byte (binary) data.
    /// </summary>
    private readonly IntPtr DataPointer;

    public DhcpClientUidManaged(byte[] data)
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

    public DhcpClientUidManaged(DhcpIpAddress data)
    {
        DataLength = Marshal.SizeOf(data);
        DataPointer = Marshal.AllocHGlobal(DataLength);
        Marshal.StructureToPtr(data, DataPointer, false);
    }

    public DhcpClientUidManaged(ulong bytes1, ulong bytes2, int dataLength)
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

            bytes1 = BitHelper.HostToNetworkOrder(bytes1);
            Marshal.WriteInt64(DataPointer, (long)bytes1);

            if (dataLength > 8)
            {
                bytes2 = BitHelper.HostToNetworkOrder(bytes2);
                Marshal.WriteInt64(DataPointer + 8, (long)bytes2);
            }

        }
    }

    public void Dispose()
    {
        if (DataPointer != IntPtr.Zero)
            Marshal.FreeHGlobal(DataPointer);
    }
}
