// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Bodoconsult.NetworkCommunication.Testing;

/// <summary>
/// Base class for UDP client or server implementations
/// </summary>
public abstract class UdpBase : IDisposable
{
    private Thread _thread;
    private bool _isDisposed;

    protected bool IsServer;



    /// <summary>
    /// Current local device listener
    /// </summary>
    protected UdpClient Listener;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint EndPoint;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint SendEndPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="ipAddress">IP address of the server</param>
    /// <param name="port">Port the server listens on</param>
    /// <param name="clientPort">Port the client listens on or 0 (then the same port as for the server is used). Setting clientPort is required normally only if UDP server and client are installed on the same machine!</param>
    protected UdpBase(IPAddress ipAddress, int port, int clientPort = 0)
    {
        Listener = new UdpClient();
        Listener.ExclusiveAddressUse = false;
        Listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Listener.Client.ReceiveTimeout = ReceiveTimeout;
        Listener.Client.SendTimeout = SendTimeout;
        //Listener.Client.NoDelay = true;


        IpAddress = ipAddress;
        Port = port;
        ClientPort = clientPort == 0 ? port : clientPort;
    }

    /// <summary>
    /// IP address of the server
    /// </summary>
    public IPAddress IpAddress { get; }

    /// <summary>
    /// Port the server listens on
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Port the client listens on or 0 (then the same port as for the server is used)
    /// </summary>
    public int ClientPort { get; }

    /// <summary>
    /// Send timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int SendTimeout { get; set; } = 10000;

    /// <summary>
    /// Receive timeout in milliseconds. -1 means infinite.
    /// </summary>
    public int ReceiveTimeout { get; set; } = 10000;

    /// <summary>
    /// Current cancellation token
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>
    /// All received messages
    /// </summary>
    public List<ReadOnlyMemory<byte>> ReceivedMessages { get; } = new();

    /// <summary>
    /// Start the client
    /// </summary>
    public void Start()
    {
        _thread = new Thread(WaitForMessages);
        _thread.Start();
    }


    private void WaitForMessages()
    {
        while (!CancellationTokenSource.Token.IsCancellationRequested)
        {
            if (CancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            if (Listener.Available <= 0)
            {
                //Thread.Sleep(50);
                continue;
            }
            var bytes = Listener.Receive(ref EndPoint);

            Debug.Print($"{GetType().Name}: received multicast from {EndPoint}:");
            Debug.Print($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");

            ReceivedMessages.Add(bytes.AsMemory());

            if (!IsServer)
            {
                Send(bytes);
            }
        }
    }

    //public async Task<Received> Receive()
    //{
    //    var result = await Listener.ReceiveAsync();
    //    return new Received()
    //    {
    //        Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
    //        Sender = result.RemoteEndPoint
    //    };
    //}

    /// <summary>
    /// Send byte array to the client
    /// </summary>
    /// <param name="data">Byte array to send</param>
    public virtual void Send(byte[] data)
    {
        if (_isDisposed)
        {
            return;
        }

        Debug.Print(GetType().Name);


        var result = Listener.Send(data, SendEndPoint);
        Debug.Print($"{GetType().Name}: sent {result} byte(s)!");

    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _isDisposed = true;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        CancellationTokenSource.Cancel();

        try
        {
            Listener?.Close();
            Listener?.Dispose();
        }
        catch
        {
            // Do nothing
        }
    }
}