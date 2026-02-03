// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.App.Helpers;
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
    /// Current listener
    /// </summary>
    protected UdpClient Listener;

    /// <summary>
    /// Endpoint
    /// </summary>
    protected IPEndPoint EndPoint;

    /// <summary>
    /// Endpoint
    /// </summary>
    /// <param name="endPoint"></param>
    protected UdpBase(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
        Listener = new UdpClient();
    }

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

        if (Listener.Client.IsBound)
        {
            var result = Listener.Send(data);
            Debug.Print($"{GetType().Name}: sent {result} byte(s)!");
        }
        else
        {
            var result = Listener.Send(data, EndPoint);
            Debug.Print($"{GetType().Name}: sent {result} byte(s)!");
        }
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