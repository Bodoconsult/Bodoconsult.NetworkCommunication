// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

// https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
// https://dev.to/chakewitz/c-networking-raw-sockets-tcp-and-udp-programming-46oc
// https://learn.microsoft.com/de-de/dotnet/framework/network-programming/using-udp-services
// https://enclave.io/high-performance-udp-sockets-net6/

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast
/// </summary>
public class UdpClientSocketProxy : BaseUpdSocketProxy
{
    private readonly IDatagramPipeline _pipeline;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    public UdpClientSocketProxy(IAppLoggerProxy logger) : base(logger, new DatagramPipeline())
    {
        _pipeline = (IDatagramPipeline)ReceiverPipeline;
        _pipeline.BufferSize = MaxPacketSize;
    }

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    public IPEndPoint? SendEndPoint { get; set; }

    private bool _isBound;

    /// <summary>
    /// Current socket (only for testing purposes, do not access directly in production code)
    /// </summary>
    public UdpClient? UdpClient { get; protected set; }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public override bool Connected => _isBound;

    /// <summary>
    /// The number of bytes available to read
    /// </summary>
    public override int BytesAvailable
    {
        get
        {
            try
            {
                return UdpClient?.Available ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override Task<int> Send(byte[] bytesToSend)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        _isBound = false;
        CancellationTokenSource.Cancel();

        if (UdpClient == null)
        {
            return;
        }

        UdpClient.Client.Shutdown(SocketShutdown.Both);
        UdpClient.Close();
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public override async Task Connect()
    {
        ArgumentNullException.ThrowIfNull(IpAddress);

        await Task.Run(() =>
        {
            try
            {
                if (UdpClient != null)
                {
                    UdpClient.Client.Shutdown(SocketShutdown.Both);
                    UdpClient.Close();
                    UdpClient.Dispose();

                    CancellationTokenSource = new CancellationTokenSource();
                }
            }
            catch // (Exception ex)
            {
                // Do nothing
            }
            finally
            {
                UdpClient = null;
            }

            try
            {
                UdpClient = new UdpClient(Port);
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, ReceiveBufferSize);
                UdpClient.Client.Blocking = false;

                EndPoint = new IPEndPoint(IpAddress, Port);
                SendEndPoint = EndPoint;

                //UdpClient.Connect(EndPoint);
                Logger.LogInformation($"{LoggerId}connected to IPAddress.Any:{Port}");
                _isBound = true;
            }
            catch (SocketException e)
            {
                Logger.LogError($"{LoggerId}Connecting failed: socket error {e.SocketErrorCode}");
            }
            catch (Exception e)
            {
                // ToDo: add logging
                Logger.LogError($"{LoggerId}{e}");
                _isBound = false;
                throw;
            }
        });
    }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    public override void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate)
    {
        SocketReceivedDataDelegate = socketReceivedDataDelegate;

        AutoResetEvent wait = new(false);

        // Start receive loop now
        Task.Run(async () =>
        {
            await ReceiverLoop(wait);
        });

        wait.WaitOne(100);
    }

    /// <summary>
    /// Run the receiver loop
    /// </summary>
    /// <param name="waitForLoopStarted"></param>
    /// <returns></returns>
    public override async Task ReceiverLoop(AutoResetEvent waitForLoopStarted)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(UdpClient);
            ArgumentNullException.ThrowIfNull(SocketReceivedDataDelegate);

            Logger.LogInformation($"{LoggerId}ReceiverLoop started");

            waitForLoopStarted.Set();

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var udpResult = await UdpClient.ReceiveAsync(CancellationTokenSource.Token);
                    SendEndPoint = udpResult.RemoteEndPoint;
                    _pipeline.AddMemory(udpResult);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SocketException se)
                {
                    Logger.LogError($"{LoggerId}Receiving failed: socket error {se.ErrorCode}: {se.Message}");
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogError($"{LoggerId}Receiving failed", e);
                }
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (SocketException se)
        {
            Logger.LogError($"{LoggerId}Receiver loop failed: socket error {se.ErrorCode}: {se.Message}");
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Receiver loop failed", e);
        }
    }

    /// <summary>
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    public override bool Poll()
    {
        try
        {
            return UdpClient != null && UdpClient.Client.Poll(1, SelectMode.SelectRead);
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Polling failed", e);
            return false;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        CancellationTokenSource.Cancel();
        IsDisposed = true;
        UdpClient?.Close();
        UdpClient?.Dispose();
        UdpClient = null;
        _isBound = false;
    }
}