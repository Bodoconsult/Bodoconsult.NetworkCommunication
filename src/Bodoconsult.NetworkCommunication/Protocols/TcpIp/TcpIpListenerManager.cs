// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Collections.Concurrent;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net;
using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Helpers;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

public class TcpIpListenerManager : ITcpIpListenerManager
{
    private readonly ConcurrentDictionary<int, Socket> _currentSockets = new();

    private readonly ConcurrentDictionary<Socket, List<ClientConnectionAcceptedDelegate>> _currentConsumers = new();

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _currentConsumers.Clear();

        foreach (var listener in _currentSockets.Values)
        {
            try
            {
                listener.Shutdown(SocketShutdown.Both);
                listener.Close(5000);
            }
            catch //(Exception e)
            {
                // Do nothing
            }

            try
            {
                listener.Dispose();
            }
            catch //(Exception e)
            {
                // Do nothing
            }
        }

        _currentSockets.Clear();
    }

    /// <summary>
    /// The maximum length of the pending connections queue. Default: 1
    /// </summary>
    public int ListenBacklog { get; set; } = 1;

    /// <summary>
    /// Readonly list of all current sockets
    /// </summary>
    public List<KeyValuePair<int, Socket>> CurrentSockets => _currentSockets.ToList();

    /// <summary>
    /// Readonly list of all current consumers
    /// </summary>
    public List<KeyValuePair<Socket, List<ClientConnectionAcceptedDelegate>>> CurrentConsumers => _currentConsumers.ToList();

    /// <summary>
    /// Is the listener blocking? See Socket.Blocking for more details. Default: false
    /// </summary>
    public bool Blocking { get; set; } = false;

    /// <summary>
    /// Is the listener working without delay? See Socket.NoDelay for more details. Default: true
    /// </summary>
    public bool NoDelay { get; set; } = true;

    /// <summary>
    /// Keep alive time in ms. Only used on Windows OS. Default: 7200000
    /// </summary>
    public int KeepAliveTime { get; set; } = 7200000;

    /// <summary>
    /// Keep alive interval ms. Only used on Windows OS. Default: 1000
    /// </summary>
    public int KeepAliveInterval { get; set; } = 1000;

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;

    /// <summary>
    /// Register a listener for a certain port on the local machine
    /// </summary>
    /// <param name="port">Local port to listen on</param>
    /// <param name="acceptDelegate">Delegate fired when a TCP/IP conenction was accepted</param>
    /// <returns>Listener socket</returns>
    public Socket RegisterListener(int port, ClientConnectionAcceptedDelegate acceptDelegate)
    {
        // Check if listener is registered already
        if (_currentSockets.TryGetValue(port, out var listener))
        {
            // Now register a new consumer for the listener
            RegisterConsumer(listener, acceptDelegate);
            return listener;
        }

        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout,
            NoDelay = NoDelay,
            Blocking = Blocking
        };

        // ToDo: RL: make property for that
        listener.ExclusiveAddressUse = false;
        listener.SetSocketKeepAliveValues(KeepAliveTime, KeepAliveInterval);

        // Bind listener to all IP addresses of the local network adapter
        EndPoint ep = new IPEndPoint(IPAddress.Any, port);
        listener.Bind(ep);

        // Now start listening
        listener.Listen(ListenBacklog);

        // Now begin
        listener.BeginAccept(AcceptCallback, listener);

        // Keep the listener instance
        _currentSockets.TryAdd(port, listener);

        // Now register a new consumer for the listener
        RegisterConsumer(listener, acceptDelegate);
        return listener;
    }

    private void RegisterConsumer(Socket listener, ClientConnectionAcceptedDelegate acceptDelegate)
    {
        if (!_currentConsumers.TryGetValue(listener, out var consumers))
        {
            consumers = [acceptDelegate];
            _currentConsumers.TryAdd(listener, consumers);
        }

        if (consumers.Contains(acceptDelegate))
        {
            return;
        }

        consumers.Add(acceptDelegate);
    }

    /// <summary>
    /// Unregister a listener on a port
    /// </summary>
    /// <param name="listener">Listener to unregister</param>
    /// <param name="acceptDelegate">Accept delegate to unregister</param>
    public void UnregisterListener(Socket listener, ClientConnectionAcceptedDelegate acceptDelegate)
    {
        if (!_currentConsumers.TryGetValue(listener, out var consumers))
        {
            return;
        }

        if (consumers?.Contains(acceptDelegate) ?? false)
        {
            consumers.Remove(acceptDelegate);
        }

        if (consumers?.Count > 0)
        {
            return;
        }

        _ = _currentConsumers.TryRemove(listener, out _);


        var item = _currentSockets.FirstOrDefault(xx => xx.Value == listener);

        if (item.Key == 0)
        {
            return;
        }

        _ = _currentSockets.TryRemove(item.Key, out _);
    }

    /// <summary>
    /// Find a listener by the port it is listening on
    /// </summary>
    /// <param name="port">Local port to listen on</param>
    /// <returns>Listener socket or null if found none</returns>
    public Socket GetListener(int port)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Callback for accepting new connection
    /// </summary>
    /// <param name="ar"></param>
    private void AcceptCallback(IAsyncResult ar)
    {
        // Get the socket that handles the client request
        var listener = (Socket)ar.AsyncState;

        if (listener == null)
        {
            return;
        }

        // Get the client socket
        var clientSocket = listener.EndAccept(ar);

        // Now deliver the client socket to the consumer
        if (!_currentConsumers.TryGetValue(listener, out var consumers))
        {
            return;
        }

        foreach (var consumer in consumers)
        {
            if (consumer.Invoke(clientSocket))
            {
                return;
            }
        }
    }

    /// <summary>
    /// Unregister a listener on a port
    /// </summary>
    /// <param name="port">Port to unregister</param>
    /// <param name="acceptDelegate">Accept delegate to unregister</param>
    public void UnregisterListener(int port, ClientConnectionAcceptedDelegate acceptDelegate)
    {
        if (!_currentSockets.TryGetValue(port, out var listener))
        {
            return;
        }

        UnregisterListener(listener, acceptDelegate);
    }
}