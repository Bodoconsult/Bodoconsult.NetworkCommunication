// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net.Sockets;
using System.Security;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for TCP/IP or UPP based <see cref="IDuplexIo"/> implementations
/// </summary>
public abstract class BaseDuplexIo : IDuplexIo
{
    private readonly ISendPacketProcessFactory _sendPacketProcessFactory;
        
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseDuplexIo(IDataMessagingConfig deviceCommSettings, ISendPacketProcessFactory sendPacketProcessFactory)
    {
        DataMessagingConfig = deviceCommSettings ?? throw new ArgumentNullException(nameof(deviceCommSettings));

        if (DataMessagingConfig.DataMessageProcessingPackage == null)
        {
            throw new ArgumentNullException(nameof(DataMessagingConfig.DataMessageProcessingPackage));
        }

        DataMessagingConfig.DuplexIoErrorHandlerDelegate = CentralErrorHandling;
        SocketProxy = DataMessagingConfig.SocketProxy;
        _sendPacketProcessFactory = sendPacketProcessFactory;
    }


    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get;  }

    /// <summary>
    /// Current socket to use
    /// </summary>
    public ISocketProxy SocketProxy { get; }

    /// <summary>
    /// Is the communication started?
    /// </summary>
    public bool IsCommunicationStarted { get; protected set; }

    /// <summary>
    /// The receiver part used of the duplex (bi-directional) comm channels
    /// </summary>
    public IDuplexIoReceiver Receiver { get; protected set; }

    /// <summary>
    /// The sender part used of the duplex (bi-directional) comm channels
    /// </summary>
    public IDuplexIoSender Sender { get; protected set; }

    /// <summary>
    /// Is the current connection alive? True, if yes else false.
    /// </summary>
    public bool IsConnectionAlive
    {
        get
        {
            if (SocketProxy == null || !SocketProxy.Connected)
            {
                return false;
            }
            var part1 = SocketProxy.Poll();
            var part2 = SocketProxy.BytesAvailable == 0;
            return !part1 || !part2;
        }
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public virtual async Task<MessageSendingResult> SendMessage(IDataMessage message)
    {

        if (message.MessageType == MessageTypeEnum.Received)
        {
            return await Task.Run(() => new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful));
        }
            
        if (message.WaitForAcknowledgement)
        {
            // Send and wait for handshake
            return await Task.Run(() => StartMessageSendingProcess(message));
        }

        // Send and do NOT wait for handshake
        return await Task.Run(async () =>
        {
            var result = await Sender.SendMessage(message);
            return result == 0 ? 
                new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful) : 
                new MessageSendingResult(message, OrderExecutionResultState.Successful);
        });
    }

    /// <summary>
    /// Send a message to the device directly. This method is intended for internal purposes only. Do NOT use directly. Use <see cref="IDuplexIo.SendMessage"/> instead. This method makes faking easier!
    /// </summary>
    /// <param name="message">Current message to send</param>
    public async Task<MessageSendingResult> SendMessageInternal(IDataMessage message)
    {
        var count = await Sender.SendMessage(message);
        return count == 0 ? new MessageSendingResult(message, OrderExecutionResultState.Unsuccessful) : new MessageSendingResult(message, OrderExecutionResultState.Successful);
    }

    /// <summary>
    /// Starts the message sending process either with waiting for a response from device or without it
    /// </summary>
    /// <param name="message">Current message to send</param>
    /// <returns>Result of the message sending process</returns>
    public MessageSendingResult StartMessageSendingProcess(IDataMessage message)
    {
        // New send process
        var currentSendPacketProcess = _sendPacketProcessFactory.CreateInstance(this,
            message,
            DataMessagingConfig);


        // Send the message and wait for ACK
        currentSendPacketProcess.Execute();

        // Result
        var result = new MessageSendingResult(message, currentSendPacketProcess.ProcessExecutionResult);
        currentSendPacketProcess.Dispose();
        return result;
    }

    /// <summary>
    /// Start the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public virtual Task StartCommunication()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Stop the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public virtual Task StopCommunication()
    {
        throw new NotSupportedException();
    }

    //public virtual async Task<byte[]> ReceiveFwUpdateMessageFromdevice()
    //{
    //    var bytes = new List<byte>();
    //    var receiveBuffer = new byte[1];
    //    var availableData = SocketProxy.BytesAvailable;
    //    if (availableData <= 0)
    //    {
    //        return Array.Empty<byte>();
    //    }
    //    var receivedByteCount = await SocketProxy.Receive(receiveBuffer);
    //    try
    //    {
    //        while (receivedByteCount > 0)
    //        {
    //            var receivedByte = receiveBuffer[0];
    //            bytes.Add(receivedByte);
    //            receivedByteCount = await SocketProxy.Receive(receiveBuffer);
    //        }
    //    }
    //    catch (SocketException exception)
    //    {
    //        var msg = $"{DataMessagingConfig.LoggerId}receiving firmware update message fails";
    //        DataMessagingConfig.AppLogger.LogError(msg, exception);
    //        DataMessagingConfig.MonitorLogger.LogError(msg, exception);
    //    }

    //    return bytes.ToArray();
    //}

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        Receiver?.UpdateDataMessageProcessingPackage();
        Sender?.UpdateDataMessageProcessingPackage();
    }

    /// <summary>
    /// Is the connection open?
    /// </summary>
    /// <returns>True if the conenction is open else false</returns>
    public bool IsConnected()
    {
        if (SocketProxy is not { Connected: true })
        {
            return false;
        }
        var part1 = SocketProxy.Poll();
        var part2 = SocketProxy.BytesAvailable == 0;
        return !part1 || !part2;
    }

    /// <summary>
    /// Central exception handling for <see cref="IDuplexIo"/> implementations
    /// </summary>
    public virtual void CentralErrorHandling(Exception exception)
    {
        string msg;

        if (exception is SocketException)
        {
            msg = $"{DataMessagingConfig.LoggerId}SocketException: Requesting for communication closing. {exception.StackTrace}";
            DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke(msg);
        }
        else if (exception is ObjectDisposedException)
        {
            msg = $"{DataMessagingConfig.LoggerId}ObjectDisposedException: Requesting for communication closing. {exception.StackTrace}";
            DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke(msg);
        }
        else if (exception is SecurityException)
        {
            msg = $"{DataMessagingConfig.LoggerId}SecurityException: Requesting for communication closing. {exception.StackTrace}";
            DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke(msg);
        }
        else
        {
            msg = $"{DataMessagingConfig.LoggerId}Exception: {exception.Message}: {exception.StackTrace}";
        }

        //Debug.Print(msg);
        DataMessagingConfig.AppLogger?.LogError(msg);
        DataMessagingConfig.MonitorLogger?.LogError(msg);
    }



    /// <summary>
    /// Current implementation of Dispose()
    /// </summary>
    /// <param name="disposing">Dispong required?</param>
    protected virtual Task Dispose(bool disposing)
    {
        throw new NotSupportedException();
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true).Wait(1000);
        GC.SuppressFinalize(this);
    }
}