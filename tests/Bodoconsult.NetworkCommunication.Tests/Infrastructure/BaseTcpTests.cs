// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace Bodoconsult.NetworkCommunication.Tests.Infrastructure;

/// <summary>
/// Base class for TCP/IP communication related tests
/// </summary>
public class BaseTcpTests : ITcpTests
{
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
    public TcpTestServer Server { get; set; }

    /// <summary>
    /// Current IP address to use
    /// </summary>
    public IPAddress IpAddress { get; set; }


    /// <summary>
    /// Current socket proxy to use
    /// </summary>
    public ISocketProxy Socket { get; set; }

    /// <summary>
    /// Device communication data
    /// </summary>
    public IIpDataMessagingConfig DataMessagingConfig { get; set; }

    /// <summary>
    /// General log file
    /// </summary>
    public IAppLoggerProxy Logger { get; set; } = TestDataHelper.GetFakeAppLoggerProxy();

    /// <summary>
    ///  Bind the delegates for testing
    /// </summary>
    protected void BindDelegates()
    {
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

    #region Event catcher methods

    protected void OnHandshakeReceivedDelegate(IDataMessage handshake)
    {
        MessageCounter++;
        IsUpdateModeReceived = true;
    }

    protected void OnCorruptedMessage(byte messageBlockAndRc, string reason)
    {
        IsCorruptedMessageFired = true;
    }
    protected void OnNotExpectedMessageReceivedEvent(IDataMessage message)
    {
        IsOnNotExpectedMessageReceivedFired = true;
    }
    protected void OnRaiseRequestComDevCloseEvent(string requestSource)
    {
        IsComDevCloseFired = true;
    }
    protected void OnRaiseDataMessageReceivedEvent(IDataMessage message)
    {
        MessageCounter++;
        IsMessageReceivedFired = true;
    }
    protected void OnRaiseDataMessageNotSentEvent(ReadOnlyMemory<byte> message, string reason)
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