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
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for UDP unicast server only sending to one client. No receiving of data
/// </summary>
public class UdpServerSocketProxy : BaseUpdSocketProxy
{
    //private readonly byte[] _tmp = new byte[1];
    private bool _isBound;

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    protected IPEndPoint? EndPoint;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    public UdpServerSocketProxy(IAppLoggerProxy logger) : base(logger, new DatagramPipeline())
    { }

    /// <summary>
    /// Endpoint for listening
    /// </summary>
    public IPEndPoint? SendEndPoint { get; set; }

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
                //Trace.TraceInformation($"Bytes available: {UdpClient?.Client.Available ?? 0}");
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
                UdpClient = new UdpClient();

                // The following three lines allow multiple clients on
                // the same PC
                UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpClient.Client.Blocking = false;
                UdpClient.Client.DontFragment = true;
                // No delay settings possible here

                EndPoint = new IPEndPoint(IpAddress, Port);
                SendEndPoint = EndPoint;
                Logger.LogInformation($"{LoggerId}sending to {IpAddress}:{Port}");

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
    public override Task ReceiverLoop(AutoResetEvent waitForLoopStarted)
    {
        waitForLoopStarted.Set();
        return Task.CompletedTask;
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
        if (UdpClient == null || SendEndPoint == null)
        {
            Logger.LogWarning($"{LoggerId}UdpClient is null or SendEndPoint is null or address IPAddress.Any. No client request before?");
            return 0;
        }

        try
        {
            var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint, CancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
            Logger.LogInformation($"{LoggerId}sent {result}B");
            return result;
        }
        catch (SocketException socketException)
        {
            if (socketException.ErrorCode != 10054)
            {
                Logger.LogError($"{LoggerId}Sending failed", socketException);
            }
            else
            {
                Logger.LogDebug($"{LoggerId}No connection");
            }

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
            var result = await UdpClient.SendAsync(bytesToSend, SendEndPoint, CancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
//#if DEBUG
//            var s = ArrayHelper.GetStringFromArrayCsharpStyle(bytesToSend, false);
//            Debug.Print($"\r\n{s}\r\n");
//#endif
            Logger.LogInformation($"{LoggerId}sent {result}B");
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