// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Bodoconsult.App.Helpers;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

public class TcpIpListenerManager : ITcpIpListenerManager
{
    private readonly ConcurrentDictionary<Socket, ListenerData> _listeners = new();

    //private readonly ConcurrentDictionary<int, Socket> _currentSockets = new();
    //private readonly ConcurrentDictionary<Socket, List<ClientConnectionAcceptedDelegate>> _currentConsumers = new();


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        ClearAll();
    }

    /// <summary>
    /// Clear all loaded values. Mainly for testing
    /// </summary>
    public void ClearAll()
    {
        foreach (var listener in _listeners.Values)
        {
            listener.AcceptCts?.Cancel();

            try
            {
                if (listener.Listener != null)
                {
                    listener.Listener.Shutdown(SocketShutdown.Both);
                    listener.Listener.Close(5000);
                }

            }
            catch //(Exception e)
            {
                // Do nothing
            }

            try
            {
                listener.Listener?.Dispose();
            }
            catch //(Exception e)
            {
                // Do nothing
            }
        }

        _listeners.Clear();
    }

    /// <summary>
    /// The maximum length of the pending connections queue. Default: 1
    /// </summary>
    public int ListenBacklog { get; set; } = 1;

    /// <summary>
    /// Readonly list of all current sockets
    /// </summary>
    public List<KeyValuePair<Socket, ListenerData>> CurrentListeners => _listeners.ToList();

    /// <summary>
    /// Exclusive address use?
    /// </summary>
    public bool ExclusiveAddressUse { get; set; } = true;

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
        ListenerData data;

        var kvp = _listeners.FirstOrDefault(x => x.Value.Port == port);

        // Check if listener is registered already
        if (kvp.Key != null)
        {
            data = kvp.Value;

            // Now register a new consumer for the listener
            RegisterConsumer(data, acceptDelegate);
            return data.Listener;
        }

        data = new ListenerData
        {
            Port = port
        };


        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            ReceiveTimeout = ReceiveTimeout,
            SendTimeout = SendTimeout,
            NoDelay = NoDelay,
            Blocking = Blocking
        };

        data.Listener = listener;
        if (!_listeners.TryAdd(listener, data))
        {
            throw new ArgumentException("Adding listener data failed!");
        }

        // ToDo: RL: make property for that
        listener.ExclusiveAddressUse = false;
        listener.SetSocketKeepAliveValues(KeepAliveTime, KeepAliveInterval);

        // Bind listener to all IP addresses of the local network adapter
        EndPoint ep = new IPEndPoint(IPAddress.Any, port);
        listener.Bind(ep);

        // Now start listening
        listener.Listen(ListenBacklog);

        // Now begin
        data.AcceptCts = new CancellationTokenSource();
        data.AcceptTask = WaitForAccept(data.Listener, data.AcceptCts.Token);

        AsyncHelper.FireAndForget(() =>
        {
            data.AcceptTask.GetAwaiter().GetResult();
        });


        //// Keep the listener instance
        //_currentSockets.TryAdd(port, listener);

        // Now register a new consumer for the listener
        RegisterConsumer(data, acceptDelegate);
        return listener;
    }

    private async Task WaitForAccept(Socket listener, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var clientSocket = await listener.AcceptAsync(token);

            // Now deliver the client socket to the consumer
            if (!_listeners.TryGetValue(listener, out var data))
            {
                return;
            }

            foreach (var consumer in data.CurrentConsumers)
            {
                if (consumer.Invoke(clientSocket))
                {
                    return;
                }
            }
        }
    }

    private static void RegisterConsumer(ListenerData data, ClientConnectionAcceptedDelegate acceptDelegate)
    {
        if (data.CurrentConsumers.Contains(acceptDelegate))
        {
            return;
        }

        data.CurrentConsumers.Add(acceptDelegate);
    }

    /// <summary>
    /// Unregister a listener on a port
    /// </summary>
    /// <param name="listener">Listener to unregister</param>
    /// <param name="acceptDelegate">Accept delegate to unregister</param>
    public void UnregisterListener(Socket listener, ClientConnectionAcceptedDelegate acceptDelegate)
    {
        if (!_listeners.TryGetValue(listener, out var data))
        {
            return;
        }

        if (data.CurrentConsumers.Contains(acceptDelegate))
        {
            data.CurrentConsumers.Remove(acceptDelegate);
        }

        if (data.CurrentConsumers?.Count > 0)
        {
            return;
        }

        data.AcceptCts?.Cancel();

        if (data.Listener != null)
        {
            try
            {
                data.Listener.Shutdown(SocketShutdown.Both);
            }
            catch //(Exception e)
            {
                // Do nothing
            }

            data.Listener.Close(1000);
            data.Listener.Dispose();
        }

        _ = _listeners.TryRemove(listener, out _);
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
        if (!_listeners.TryGetValue(listener, out var data))
        {
            return;
        }

        foreach (var consumer in data.CurrentConsumers)
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
        var kvp = _listeners.FirstOrDefault(x => x.Value.Port == port);

        // Check if listener is registered already
        if (kvp.Key == null)
        {
            return;
        }

        UnregisterListener(kvp.Value.Listener, acceptDelegate);
    }
}