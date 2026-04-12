// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

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
    public IAppLoggerProxy Logger { get; set; } = TestDataHelper.GetFakeAppLoggerProxy();

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

        // Send a recognition message to the server
        var msg = new RawOutboundDataMessage
        {
            RawMessageData = new Memory<byte>([0x1])
        };

        var result2 = DuplexIo.SendMessageDirect(msg).GetAwaiter().GetResult();
        Assert.That(result2.ProcessExecutionResult, Is.EqualTo(OrderExecutionResultState.Successful));

        // Now send messages
        foreach (var message in data)
        {
            RemoteUdpDevice.Send(message);
        }

        var tcs1 = new TaskCompletionSource<bool>();
        var t1 = tcs1.Task;

        // Start a background task that will complete tcs1.Task
        Task.Factory.StartNew(() =>
        {
            var cts = new CancellationTokenSource(5000);
            while (!cts.IsCancellationRequested)
            {
                if (MessageCounter >= expectedCount)
                {
                    tcs1.SetResult(true);
                    return;
                }
                Task.Delay(10, cts.Token);
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

    public virtual void SendDataFromLocalToRemote(List<IOutboundDataMessage> data, int expectedCount)
    {
        // Arrange
        ArgumentNullException.ThrowIfNull(DuplexIo);
        ArgumentNullException.ThrowIfNull(RemoteUdpDevice);

        DuplexIo.StartCommunication().Wait();

        // Send a recognition message to the server
        var msg = new SdcpOutboundDataMessage
        {
            WaitForAcknowledgement = false,
            DataBlock = new BasicOutboundDatablock
            {
                Data = new Memory<byte>([0x0, 0x1, 0x2, 0x3, 0x4]),
                DataBlockType = 'x'
            },
            RawMessageData = new Memory<byte>([0x78, 0x0, 0x1, 0x2, 0x3, 0x4])
        };

        RemoteUdpDevice.Send(msg.RawMessageData.ToArray());

        // Now send messages
        var tcs1 = new TaskCompletionSource<bool>();
        var t1 = tcs1.Task;

        // Start a background task that will complete tcs1.Task
        Task.Factory.StartNew(() =>
        {
            var cts = new CancellationTokenSource(50000);
            while (!cts.IsCancellationRequested)
            {
                if (RemoteUdpDevice.ReceivedMessages.Count >= expectedCount)
                {
                    tcs1.SetResult(true);
                    return;
                }
                Task.Delay(15, cts.Token);
            }

            tcs1.SetResult(false);
        });

        // Send now
        AsyncHelper.FireAndForget(() =>
        {
            try
            {
                foreach (var message in data)
                {
                    DuplexIo.SendMessage(message);
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                throw;
            }
        });

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
        SendDataFromLocalToRemote(data, expectedCount);

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
            msg = $"{DataMessagingConfig.LoggerId}:SocketException: Requesting for communication closing.";
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