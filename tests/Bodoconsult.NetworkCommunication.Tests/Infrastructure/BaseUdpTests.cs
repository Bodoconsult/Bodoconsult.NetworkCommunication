// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace Bodoconsult.NetworkCommunication.Tests.Infrastructure;

/// <summary>
/// Base class for UDP communication related tests
/// </summary>
public class BaseUdpTests : IUdpTests
{
    /// <summary>
    /// Holds the duplex IO channel implementation (see <see cref="IDuplexIo"/>) to use
    /// </summary>
    protected IDuplexIo? DuplexIo;

    /// <summary>
    /// Events for checking is an event was fired 
    /// </summary>
    protected bool IsMessageReceivedFired;
    protected bool IsMessageNotReceivedFired;
    protected bool IsComDevCloseFired;
    protected bool IsCorruptedMessageFired;
    protected bool IsOnNotExpectedMessageReceivedFired;
    protected bool IsUpdateModeReceived;
    protected bool DuplexIsInProgressFired;
    protected bool DuplexSetInProgressFired;
    protected bool IsDataMessageSentFired;
    protected bool IsDataMessageNotSentFired;

    /// <summary>
    /// Number of messages received or sent
    /// </summary>
    protected int MessageCounter;

    /// <summary>
    /// Current TCP/IP server to send data to the socket
    /// </summary>
    public IUdpDevice? RemoteUdpDevice { get; set; }

    /// <summary>
    /// Current IP address to use
    /// </summary>
    public IPAddress? IpAddress { get; set; }

    /// <summary>
    /// Current socket proxy to use
    /// </summary>
    public ISocketProxy? Socket { get; set; }

    /// <summary>
    /// Device communication data
    /// </summary>
    public IIpDataMessagingConfig? DataMessagingConfig { get; set; }

    /// <summary>
    /// General log file
    /// </summary>
    public IAppLoggerProxy Logger { get; set; } = TestDataHelper.Logger;

    /// <summary>
    ///  Bind the delegates for testing
    /// </summary>
    protected void BindDelegates()
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);
        DataMessagingConfig.SocketProxy = Socket;
        //DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager.RaiseHandshakeReceivedDelegate = OnHandshakeReceivedDelegate;
        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = OnRaiseDataMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotReceivedDelegate = OnRaiseDataMessagingConfigMessageNotReceivedEvent;
        DataMessagingConfig.RaiseComDevCloseRequestDelegate = OnRaiseRequestComDevCloseEvent;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigCorruptedMessageDelegate = OnCorruptedMessage;
        DataMessagingConfig.RaiseDataMessageNotSentDelegate = OnRaiseDataMessageNotSentEvent;
        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageSentDelegate = OnRaiseDataMessagingConfigMessageSentEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotSentDelegate = OnRaiseDataMessagingConfigMessageNotSentEvent;
    }

    /// <summary>
    /// Send a message with the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="message">Current message to send</param>
    public virtual void Send(IOutboundDataMessage message)
    {
        ArgumentNullException.ThrowIfNull(DuplexIo);

        DuplexIo.StartCommunication().Wait();

        DuplexIo.SendMessage(message).Wait();

        var task = Task.Run(() =>
        {
            var i = 0;
            while (i < 200)
            {
                AsyncHelper.Delay(5);
                i++;
            }

        });
        task.Wait();

        DuplexIo.StopCommunication().Wait();
    }


    public virtual void SendDataFromRemoteToLocal(List<byte[]> data, int expectedCount)
    {
        // Arrange
        ArgumentNullException.ThrowIfNull(DuplexIo);
        ArgumentNullException.ThrowIfNull(RemoteUdpDevice);

        DuplexIo.StartCommunication().Wait();

        // Now send messages
        var cts = new CancellationTokenSource(5000);
        Task.Run(() =>
        {
            foreach (var message in data)
            {
                RemoteUdpDevice.Send(message);

                if (cts.Token.IsCancellationRequested)
                {
                    break;
                }
            }

            Wait.Until(() => MessageCounter >= expectedCount);

            Debug.Print($"Waited for {MessageCounter} messages!");

            cts.Cancel();
        });


        var tcs1 = new TaskCompletionSource<bool>();
        var t1 = tcs1.Task;

        // Start a background task that will complete tcs1.Task
        Task.Run(() =>
        {
            while (true)
            {
                if (MessageCounter >= expectedCount)
                {
                    tcs1.SetResult(true);
                    Debug.Print($"Exit waiting with {MessageCounter} messages!");
                    return;
                }

                if (!cts.Token.IsCancellationRequested)
                {
                    continue;
                }

                Debug.Print($"Exit: {RemoteUdpDevice.ReceivedMessages.Count}");
                break;
            }

            tcs1.SetResult(false);
        });


        var result = t1.GetAwaiter().GetResult();

        // Start a background task that will complete tcs1.Task

        // Act
        DuplexIo.StopCommunication().Wait(1000);

        Assert.That(result);

        Debug.Print("Process done");
    }

    public virtual void SendDataFromLocalToRemote(List<IOutboundDataMessage> data, int expectedCount,
        CancellationToken? cancellationToken = null)
    {
        // Arrange
        ArgumentNullException.ThrowIfNull(DuplexIo);
        ArgumentNullException.ThrowIfNull(RemoteUdpDevice);

        DuplexIo.StartCommunication().Wait();

        //// Send a recognition message to the server
        //var msg = new SdcpOutboundDataMessage
        //{
        //    WaitForAcknowledgement = false,
        //    DataBlock = new BasicOutboundDatablock
        //    {
        //        Data = new Memory<byte>([0x0, 0x1, 0x2, 0x3, 0x4]),
        //        DataBlockType = 'x'
        //    },
        //    RawMessageData = new Memory<byte>([0x78, 0x0, 0x1, 0x2, 0x3, 0x4])
        //};

        //RemoteUdpDevice.Send(msg.RawMessageData.ToArray());

        var cts = new CancellationTokenSource();

        // Now send messages
        var tcs1 = new TaskCompletionSource<bool>();
        var t1 = tcs1.Task;

        // Start a background task that will complete tcs1.Task
        var t2 = Task.Run(() =>
        {
            try
            {
                while (true)
                {
                    if (RemoteUdpDevice.ReceivedMessages.Count >= expectedCount)
                    {
                        Debug.Print("Successful received  messages");
                        tcs1.SetResult(true);
                        return;
                    }

                    if (!cts.Token.IsCancellationRequested &&
                        !(cancellationToken?.IsCancellationRequested ?? false))
                    {
                        continue;
                    }

                    Debug.Print($"Exit: {RemoteUdpDevice.ReceivedMessages.Count}");
                    break;
                }

                tcs1.SetResult(false);
            }
            catch (Exception e)
            {
                Debug.Print($"Exit2: {RemoteUdpDevice.ReceivedMessages.Count}: {e}");
                tcs1.SetResult(false);
            }
        });

        // Send now
        Task.Run(async () =>
        {
            try
            {
                string s;
                for (var index = 0; index < data.Count; index++)
                {
                    var message = data[index];

                    await DuplexIo.SendMessage(message);
                    s = $"UdpServer: send message {index}";
                    Debug.Print(s);

                    if (cancellationToken?.IsCancellationRequested ?? false)
                    {
                        await cts.CancelAsync();
                        return;
                    }
                }

                s = $"Left send loop: CancelRequest: {cts.Token.IsCancellationRequested}";
                Debug.Print(s);

                //Trace.Assert(!cts.Token.IsCancellationRequested);

                Wait.Until(() => RemoteUdpDevice.ReceivedMessages.Count >= expectedCount, 10000);

                s = $"Waited for messages received: {RemoteUdpDevice.ReceivedMessages.Count} / {expectedCount}";
                Debug.Print(s);

                //Trace.Assert(RemoteUdpDevice.ReceivedMessages.Count >= expectedCount);

                await RemoteUdpDevice.CancellationTokenSource.CancelAsync();

                await cts.CancelAsync();
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        });

        t2.Wait(10000);
        var result = t1.GetAwaiter().GetResult();

        // Start a background task that will complete tcs1.Task

        // Act
        DuplexIo.StopCommunication().Wait(1000);

        Assert.That(result);

        Debug.Print("Process done");
    }

    protected void RunBasicReceiveTests(List<byte[]> data, int expectedCount)
    {
        // Arrange and act
        SendDataFromRemoteToLocal(data, expectedCount);

        // Assert
        if (expectedCount == 0)
        {
            Assert.That(MessageCounter, Is.EqualTo(expectedCount));
        }
        else
        {
            Assert.That(MessageCounter, Is.Not.Zero);
        }

        Assert.That(MessageCounter, Is.EqualTo(expectedCount));
    }

    protected void RunBasicSendTests(List<IOutboundDataMessage> data, int expectedCount)
    {
        // Arrange and act
        SendDataFromLocalToRemote(data, expectedCount, null);

        // Assert
        if (expectedCount == 0)
        {
            Assert.That(MessageCounter, Is.EqualTo(expectedCount));
        }
        else
        {
            Assert.That(MessageCounter, Is.Not.Zero);
        }

        Assert.That(MessageCounter, Is.EqualTo(expectedCount));
    }


    #region Event catcher methods


    protected void OnHandshakeReceivedDelegate(IInboundDataMessage handshake)
    {
        MessageCounter++;
        IsUpdateModeReceived = true;
    }


    protected void OnCorruptedMessage(byte messageBlockAndRc, string reason)
    {
        IsCorruptedMessageFired = true;
    }

    protected void OnNotExpectedMessageReceivedEvent(IInboundDataMessage message)
    {
        IsOnNotExpectedMessageReceivedFired = true;
    }

    protected void OnRaiseRequestComDevCloseEvent(string requestSource)
    {
        IsComDevCloseFired = true;
    }

    protected void OnRaiseDataMessageReceivedEvent(IInboundDataMessage message)
    {
        MessageCounter++;
        IsMessageReceivedFired = true;
    }

    protected void OnRaiseDataMessageNotSentEvent(ReadOnlyMemory<byte> message, string? reason)
    {
        IsDataMessageNotSentFired = true;
    }

    protected void OnRaiseDataMessageSentEvent(ReadOnlyMemory<byte> message)
    {
        IsDataMessageSentFired = true;
    }


    protected virtual void BaseReset()
    {
        MessageCounter = 0;
        IsMessageReceivedFired = false;
        IsMessageNotReceivedFired = false;
        IsComDevCloseFired = false;
        IsCorruptedMessageFired = false;
        IsOnNotExpectedMessageReceivedFired = false;
        IsUpdateModeReceived = false;
        DuplexIsInProgressFired = false;
        DuplexSetInProgressFired = false;
        IsDataMessageSentFired = false;
        IsDataMessageNotSentFired = false;
    }


    protected void SetNotInProgress()
    {
        DuplexSetInProgressFired = true;
    }

    protected bool WorkInProgress()
    {
        DuplexIsInProgressFired = true;
        return false;
    }



    #endregion

    /// <summary>
    /// Central exception handling for <see cref="IDuplexIo"/> implementations
    /// </summary>
    public virtual void CentralErrorHandling(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);

        string msg;
        var loggerId = $"{DataMessagingConfig.LoggerId}:";

        IsDataMessageNotSentFired = true;
        IsDataMessageSentFired = false;

        if (exception is SocketException)
        {
            msg = $"{loggerId}:SocketException: Requesting for communication closing.";
            IsComDevCloseFired = true;
        }
        else if (exception is ObjectDisposedException)
        {
            msg = $"{loggerId}ObjectDisposedException: Requesting for communication closing.";
            IsComDevCloseFired = true;
        }
        else if (exception is SecurityException)
        {
            msg = $"{loggerId}SecurityException: Requesting for communication closing";
            IsComDevCloseFired = true;
        }
        else
        {
            msg = $"{loggerId}Exception: {exception.Message}";
        }

        //Debug.Print(msg);
        Debug.Print(msg);

    }

}