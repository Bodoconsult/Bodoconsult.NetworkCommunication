// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Net.Sockets;
using System.Text;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// State object for receiving data from remote device.
/// </summary>
public class StateObject
{
    // Client socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 256;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new();
}