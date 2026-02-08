// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Listener information
/// </summary>
public class ListenerData
{
    /// <summary>
    /// Port number the listener is listening on
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    ///  Current listener instance
    /// </summary>
    public Socket Listener { get; set; }

    /// <summary>
    /// Current consumers
    /// </summary>
    public List<ClientConnectionAcceptedDelegate> CurrentConsumers { get; } = new();

    /// <summary>
    /// <see cref="CancellationTokenSource"/> instance used to end the connection accept task
    /// </summary>
    public CancellationTokenSource AcceptCts { get; set; }

    /// <summary>
    /// Task waiting for connection accepts
    /// </summary>
    public Task AcceptTask { get; set; }

}