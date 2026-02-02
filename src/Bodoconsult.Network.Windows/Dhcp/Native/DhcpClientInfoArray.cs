// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpClientInfoArray : IDisposable
{
    /// <summary>
    /// Specifies the number of elements present in Clients.
    /// </summary>
    public readonly int NumElements;
    /// <summary>
    /// Pointer to a list of DHCP_CLIENT_INFO structures that contain information on specific DHCP subnet clients.).
    /// </summary>
    private readonly IntPtr ClientsPointer;

    /// <summary>
    /// Pointer to a list of DHCP_CLIENT_INFO structures that contain information on specific DHCP subnet clients.).
    /// </summary>
    public IEnumerable<ClientTuple> Clients
    {
        get
        {
            if (NumElements == 0 || ClientsPointer == IntPtr.Zero)
                yield break;

            var iter = ClientsPointer;
            for (var i = 0; i < NumElements; i++)
            {
                var clientPtr = Marshal.ReadIntPtr(iter);
                yield return new ClientTuple()
                {
                    Pointer = clientPtr,
                    Value = clientPtr.MarshalToStructure<DhcpClientInfo>()
                };
                iter += IntPtr.Size;
            }
        }
    }

    public void Dispose()
    {
        foreach (var client in Clients)
        {
            client.Value.Dispose();
            Api.FreePointer(client.Pointer);
        }

        Api.FreePointer(ClientsPointer);
    }

    internal struct ClientTuple
    {
        public IntPtr Pointer;
        public DhcpClientInfo Value;
    }
}
