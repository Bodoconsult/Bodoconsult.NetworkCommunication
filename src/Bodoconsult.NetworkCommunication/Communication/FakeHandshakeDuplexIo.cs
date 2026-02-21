// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// This class implements a <see cref="IDuplexIo"/> instance "receiving" a handshake on sending a message out
/// </summary>
public class FakeHandshakeDuplexIo : IDuplexIo
{

    private int _counter;

    public FakeHandshakeDuplexIo(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;
    }


    /// <summary>
    /// The number of tries the handshake is received
    /// </summary>
    public int NumberOfTriesTheHandshakeIsReceived { get; set; }

    /// <summary>
    /// The handshale message to be "received" on sending a message
    /// </summary>
    public IInboundHandShakeDataMessage HandShakeDataMessage { get; set; } = new InboundHandshakeMessage(MessageTypeEnum.Sent);

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Is the communication started?
    /// </summary>
    public bool IsCommunicationStarted { get; set; }

    /// <summary>
    /// The receiver part used of the duplex (bidirectional) comm channels
    /// </summary>
    public IDuplexIoReceiver Receiver { get; set; }

    /// <summary>
    /// The sender part used of the duplex (bidirectional) comm channels
    /// </summary>
    public IDuplexIoSender Sender { get; set; }

    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Is the current connection alive? True, if yes else false.
    /// </summary>
    public bool IsConnectionAlive { get; set; } = true;

    /// <summary>
    /// Delay between sending a message and receiving the handshake
    /// </summary>
    public int ReceiverDelay { get; set; }

    /// <summary>
    /// Send a message to the device.
    /// </summary>
    /// <param name="message">Current message to send</param>
    public Task<MessageSendingResult> SendMessage(IOutboundDataMessage message)
    {
        _counter++;
        if (_counter >= NumberOfTriesTheHandshakeIsReceived)
        {
            AsyncHelper.FireAndForget(() =>
            {
                if (ReceiverDelay > 0)
                {
                    Task.Delay(ReceiverDelay).Wait();
                }
                DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived(HandShakeDataMessage);
            });
        }
        var result = new MessageSendingResult(message, OrderExecutionResultState.Successful);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Send a message to the device directly. This method is intended for internal purposes only. Do NOT use directly. Use <see cref="IDuplexIo.SendMessage"/> instead. This method makes faking easier!
    /// </summary>
    /// <param name="message">Current message to send</param>
    public Task<MessageSendingResult> SendMessageInternal(IOutboundDataMessage message)
    {
        _counter++;
        if (_counter >= NumberOfTriesTheHandshakeIsReceived)
        {
            AsyncHelper.FireAndForget(() =>
            {
                if (ReceiverDelay > 0)
                {
                    Task.Delay(ReceiverDelay).Wait();
                }
                DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived(HandShakeDataMessage);
            });
        }
        var result = new MessageSendingResult(message, OrderExecutionResultState.Successful);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Start the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public Task StartCommunication()
    {
        // Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public Task StopCommunication()
    {
        // Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        // Do nothing
    }
}