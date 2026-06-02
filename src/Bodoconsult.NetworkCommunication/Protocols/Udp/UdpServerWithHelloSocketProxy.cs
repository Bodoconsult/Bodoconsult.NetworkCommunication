// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.Udp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast server  using a HELLO message sent by the client to the server to set up the connection.
/// The content of the HELLO message does not care, but it has to be sent as first message from client to server
/// </summary>
public class UdpServerWithHelloSocketProxy : BaseUpdSocketProxy
{
    private readonly IDatagramPipeline _pipeline;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? SendEndPoint;

    private bool _isBound;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    public UdpServerWithHelloSocketProxy(IAppLoggerProxy logger) : base(logger, new DatagramPipeline())
    {
        _pipeline = (IDatagramPipeline)ReceiverPipeline;
        _pipeline.BufferSize = MaxPacketSize;
    }

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
                return UdpClient?.Client.Available ?? 0;
            }
            catch
            {
                return 0;
            }
        }
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
        UdpClient = null;
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

                // The following three lines allow multiple clients on the same PC
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpClient.Client.Blocking = false;

                EndPoint = new IPEndPoint(IPAddress.Any, Port);
                _isBound = true;
            }
            catch (Exception e)
            {
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
                var result = 0;
                var buffer = _pipeline.GetBuffer();
                try
                {
                    var udpResult = await UdpClient.ReceiveAsync(CancellationTokenSource.Token).AsTask();
                    result = udpResult.Buffer.Length;
                    if (result != 0)
                    {
                        udpResult.Buffer.CopyTo(buffer.Memory);

                        _pipeline.AddMemory(buffer, udpResult.Buffer.Length);
                    }
                    else
                    {
                        _pipeline.ReleaseBuffer(buffer);
                    }
                }
                catch (OperationCanceledException)
                {
                    _pipeline.ReleaseBuffer(buffer);
                    break;
                }
                catch (SocketException se)
                {
                    _pipeline.ReleaseBuffer(buffer);
                    Logger.LogError($"{LoggerId}Receiving failed: socket error {se.ErrorCode}: {se.Message}");
                }
                catch (Exception e)
                {
                    _pipeline.ReleaseBuffer(buffer);
                    Logger.LogError($"{LoggerId}Receiving failed ", e);
                }

                // var result = await Socket.ReceiveAsync(buffer, SocketFlags.None, CancellationTokenSource.Token);

                if (result == 0)
                {
                    await Task.Delay(5);
                }

                //Debug.Print($"{LoggerId}Received {result} byte");

                //AsyncHelper.FireAndForget(() =>
                //{
                //    try
                //    {
                //        SocketReceivedDataDelegate.Invoke(buffer[..result]);
                //    }
                //    catch (Exception e)
                //    {
                //        Logger.LogError($"{LoggerId}Forwarding received data failed", e);
                //    }
                //});
            }
        }
        catch (OperationCanceledException)
        {

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
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Byte array to send</param>
    public override async Task<int> Send(byte[] bytesToSend)
    {
        if (UdpClient == null || SendEndPoint == null || Equals(SendEndPoint.Address, IPAddress.Any))
        {
            return 0;
        }

        try
        {
            var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint, CancellationTokenSource.Token);
            Logger.LogInformation($"{LoggerId}sent {result} bytes");
            return result;
        }
        catch (SocketException se)
        {
            Logger.LogError($"{LoggerId}Sending failed: socket error {se.ErrorCode}: {se.Message}");
            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Sending failed", e);
            return 0;
        }
    }

    /// <summary>
    /// Send bytes
    /// </summary>
    /// <param name="bytesToSend">Data to send</param>
    public override async Task<int> Send(ReadOnlyMemory<byte> bytesToSend)
    {
        if (UdpClient == null || SendEndPoint == null || Equals(SendEndPoint.Address, IPAddress.Any))
        {
            Logger.LogWarning($"{LoggerId}UdpClient is null or SendEndPoint is null or address IPAddress.Any. No client request before?");
            return 0;
        }

        try
        {
            var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint, CancellationTokenSource.Token).AsTask();
            Logger.LogInformation($"{LoggerId}sent {result} bytes");
            return result;
        }
        catch (SocketException se)
        {
            Logger.LogError($"{LoggerId}Sending failed: socket error {se.ErrorCode}: {se.Message}");
            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Sending failed", e);
            return 0;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        CancellationTokenSource.Cancel();
        IsDisposed = true;

        if (UdpClient == null)
        {
            return;
        }

        UdpClient.Client.Shutdown(SocketShutdown.Both);
        UdpClient.Close();
        UdpClient.Dispose();
        UdpClient = null;
    }
}