// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Protocols.TcpIp;

/// <summary>
/// Current asynchronous implementation of <see cref="ISocketProxy"/> for TCP
/// </summary>
public class TcpIpServerSocketProxy : BaseTcpIpSocketProxy
{
    //private readonly byte[] _tmp = new byte[1];
    private Socket? _listener;
    private bool _isBound;

    /// <summary>
    /// Default ctor
    /// </summary>
    public TcpIpServerSocketProxy(ITcpIpListenerManager tcpIpListenerManager, IAppLoggerProxy logger) : base(logger)
    {
        TcpIpListenerManager = tcpIpListenerManager;
    }

    /// <summary>
    /// Current <see cref="ITcpIpListenerManager"/> instance
    /// </summary>
    public ITcpIpListenerManager TcpIpListenerManager { get; }

    /// <summary>
    /// Is the socket connected
    /// </summary>
    public override bool Connected => _isBound;
    //{
    //    get
    //    {
    //        // Replacement for Socket.Connected. See sample at the end of https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.connected?redirectedfrom=MSDN&view=net-7.0#System_Net_Sockets_Socket_Connected

    //        try
    //        {
    //            if (_listener == null)
    //            {
    //                return false;
    //            }

    //            // This is how you can determine whether a socket is still connected.
    //            var blockingState = _listener.Blocking;
    //            try
    //            {
    //                _listener.Blocking = false;
    //                _listener.Send(_tmp, 0, 0);
    //                //Console.WriteLine("Connected!");
    //            }
    //            catch (SocketException e)
    //            {
    //                // 10035 == WSAEWOULDBLOCK
    //                if (e.NativeErrorCode.Equals(10035))
    //                {
    //                    // Still connected, but the send would block;
    //                }
    //                else
    //                {
    //                    // Disconnected
    //                    return false;
    //                }
    //            }
    //            finally
    //            {
    //                _listener.Blocking = blockingState;
    //            }

    //            return true;
    //        }
    //        catch //(Exception e)
    //        {
    //            return false;
    //        }
    //    }
    //}

    /// <summary>
    /// The number of bytes available to read
    /// </summary>
    public override int BytesAvailable
    {
        get
        {
            try
            {
                return Socket is not { Connected: true } ? 0 : Socket.Available;
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
    public override async Task<int> Send(byte[] bytesToSend)
    {
        if (Socket is not { Connected: true })
        {
            return 0;
        }

        try
        {
            var result = await Socket.SendAsync(bytesToSend, CancellationTokenSource.Token);
            Logger.LogInformation($"{LoggerId}sent {result} bytes");
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
        if (Socket is not { Connected: true })
        {
            return 0;
        }

        try
        {
            var result = await Socket.SendAsync(bytesToSend, CancellationTokenSource.Token).AsTask();
            Logger.LogInformation($"{LoggerId}sent {result} bytes");
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
    /// Poll data
    /// </summary>
    /// <returns>True, if data can be read, else false</returns>
    public override bool Poll()
    {
        try
        {
            return _listener != null && _listener.Poll(1, SelectMode.SelectRead);
        }
        catch (Exception e)
        {
            Logger.LogError($"{LoggerId}Polling failed", e);
            return false;
        }
    }

    /// <summary>
    /// Close the socket
    /// </summary>
    public override void Close()
    {
        _isBound = false;
        CancellationTokenSource.Cancel(false);
        CancellationTokenSource = new CancellationTokenSource();

        if (_listener != null)
        {
            TcpIpListenerManager.UnregisterListener(_listener, AcceptDelegate);
        }
        if (Socket == null)
        {
            return;
        }
        Socket.Shutdown(SocketShutdown.Both);
        Socket.Close();
    }

    /// <summary>
    /// Connect to an IP endpoint
    /// </summary>
    public override async Task Connect()
    {
        await Task.Run(() =>
        {
            try
            {
                if (Socket != null)
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                    Socket?.Dispose();

                    CancellationTokenSource = new CancellationTokenSource();
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{LoggerId}{e}");
                // Do nothing
            }
            finally
            {
                Socket = null;
            }


            _listener = TcpIpListenerManager.RegisterListener(Port, AcceptDelegate);
            Logger.LogInformation($"{LoggerId}bound to IPAddress.Any:{Port}");
        });
    }

    private Task<bool> AcceptDelegate(Socket clientSocket)
    {
        var task = Task.Run(() =>
        {
            // ToDo: check remote IP address
            Socket = clientSocket;

            _isBound = Socket != null;

            StartReceiverLoop();

            return true;
        });

        return task;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        CancellationTokenSource.Cancel(false);

        if (_listener != null)
        {
            TcpIpListenerManager.UnregisterListener(_listener, AcceptDelegate);
        }

        IsDisposed = true;
        Socket?.Close();
        Socket?.Dispose();
        Socket = null;
    }
}