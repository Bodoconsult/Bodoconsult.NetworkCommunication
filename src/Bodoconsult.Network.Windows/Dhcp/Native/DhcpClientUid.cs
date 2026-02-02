// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpClientUid : IDisposable
{
    /// <summary>
    /// Specifies the size of Data, in bytes.
    /// </summary>
    public readonly int DataLength;

    /// <summary>
    /// Pointer to an opaque blob of byte (binary) data.
    /// </summary>
    private readonly IntPtr DataPointer;

    /// <summary>
    /// Blob of byte (binary) data.
    /// </summary>
    public byte[] Data
    {
        get
        {
            var blob = new byte[DataLength];

            if (DataLength != 0)
                Marshal.Copy(DataPointer, blob, 0, DataLength);

            return blob;
        }
    }

    public DhcpIpAddress ClientIpAddress
    {
        get
        {
            if (DataLength < 4)
                throw new ArgumentOutOfRangeException(nameof(DataLength));

            return (DhcpIpAddress)Marshal.ReadInt32(DataPointer);
        }
    }

    public DhcpServerHardwareAddress ClientHardwareAddress
    {
        get
        {
            if (DataLength < 5)
                throw new ArgumentOutOfRangeException(nameof(DataLength));

            return DhcpServerHardwareAddress.FromNative(DhcpServerHardwareType.Ethernet, DataPointer + 5, DataLength - 5);
        }
    }

    public void Dispose()
    {
        Api.FreePointer(DataPointer);
    }
}
