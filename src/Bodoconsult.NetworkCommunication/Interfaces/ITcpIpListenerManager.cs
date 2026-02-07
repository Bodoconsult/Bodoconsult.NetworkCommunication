// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Interfaces;

// https://csharp-networking.com/chapter03/

/// <summary>
/// Manages on a TCP/IP server the listener instances to avoid unnecessary thread creation for listeners on the same port
/// </summary>
public interface ITcpIpListenerManager: IDisposable
{
    /// <summary>
    /// Is the listener blocking? See Socket.Blocking for more details
    /// </summary>
    bool Blocking { get; set; }

    /// <summary>
    /// Is the listener working without delay? See Socket.NoDelay for more details
    /// </summary>
    bool NoDelay { get; set; }

    /// <summary>
    /// Keep alive time in ms. Only on Windows OS
    /// </summary>
    int KeepAliveTime { get; set; }

    /// <summary>
    /// Keep alive interval ms. Only used on Windows OS
    /// </summary>
    int KeepAliveInterval { get; set; }

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite
    /// </summary>
    int SendTimeout { get; set; }

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite
    /// </summary>
    int ReceiveTimeout { get; set; }

    /// <summary>
    /// The maximum length of the pending connections queue
    /// </summary>
    int ListenBacklog { get; set; }

    /// <summary>
    /// Readonly list of all current sockets
    /// </summary>
    List<KeyValuePair<int, Socket>> CurrentSockets { get; }

    /// <summary>
    /// Readonly list of all current consumers
    /// </summary>
    List<KeyValuePair<Socket, List<ClientConnectionAcceptedDelegate>>> CurrentConsumers { get; }

    /// <summary>
    /// Register a listener for a certain port on the local machine
    /// </summary>
    /// <param name="port">Local port to listen on</param>
    /// <param name="acceptDelegate">Delegate fired when a TCP/IP conenction was accepted</param>
    /// <returns>Listener socket</returns>
    Socket RegisterListener(int port, ClientConnectionAcceptedDelegate acceptDelegate);

    /// <summary>
    /// Unregister a listener on a port
    /// </summary>
    /// <param name="port">Port to unregister</param>
    /// <param name="acceptDelegate">Accept delegate to unregister</param>
    void UnregisterListener(int port, ClientConnectionAcceptedDelegate acceptDelegate);

    /// <summary>
    /// Unregister a listener on a port
    /// </summary>
    /// <param name="listener">Listener to unregister</param>
    /// <param name="acceptDelegate">Accept delegate to unregister</param>
    void UnregisterListener(Socket listener, ClientConnectionAcceptedDelegate acceptDelegate);

    /// <summary>
    /// Find a listener by the port it is listening on
    /// </summary>
    /// <param name="port">Local port to listen on</param>
    /// <returns>Listener socket or null if found none</returns>
    Socket GetListener(int port);
}