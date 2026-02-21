// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.App.Helpers;
using System.Diagnostics;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Current implementation of <see cref="ISendPacketProcess"/> for sending a message to the device and waiting for response (ACK, NACK or CAN)
/// </summary>
public class SendPacketProcess : BaseSendPacketProcess
{

    private CancellationTokenSource _ctsWait;

    private TaskCompletionSource<IInboundHandShakeDataMessage> _taskCompletionSourceWait;
    private TaskCompletionSource<bool> _taskCompletionSourceSend;

    /// <summary>
    /// Delegate for unregistering a wait state from a <see cref="IWaitStateManager"/> implementation
    /// </summary>
    public UnregisterWaitStateDelegate UnregisterWaitStateDelegate { get; set; }

    /// <summary>
    /// Additional timeout for the time between starting timeout and sending message
    /// </summary>
    private const int AdditionalTimeout = 100;

    /// <summary>
    /// Execute the entry state
    /// </summary>
    /// <returns>True if a handshake was received else false</returns>
    protected override bool ExecuteSendAndWait()
    {
        // Prepare sending but wait with sending until waiting is started
        StartSendingMessage();

        // Start waiting for handshake now
        StartWaiting();

        return HasFinishedWithoutTimeout;
    }

    /// <summary>
    /// Prepare sending but wait with sending until waiting is started
    /// </summary>
    private void StartWaiting()
    {
        // Create cancellation token
        _ctsWait = new CancellationTokenSource(Timeout + AdditionalTimeout);
        _ctsWait.Token.Register(() =>
        {
            if (_taskCompletionSourceWait is not
                {
                    Task:
                    {
                        IsCompleted: false, IsCanceled: false, IsFaulted: false, IsCompletedSuccessfully: false
                    }
                })
            {
                return;
            }

            _taskCompletionSourceWait?.SetResult(null);

        });

        _taskCompletionSourceWait = new TaskCompletionSource<IInboundHandShakeDataMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        _taskCompletionSourceSend.SetResult(true);

        Debug.Print($"SSP:    start SEND waiting {DateTime.Now:O}");
        var taskResult = AsyncHelper.RunSync(() => _taskCompletionSourceWait.Task);
        Debug.Print($"SSP:    stop SEND waiting {DateTime.Now:O}");
        _taskCompletionSourceWait = null;

        if (taskResult == null)
        {
            ProcessExecutionResult = OrderExecutionResultState.Timeout;
            return;
        }

        DataMessagingConfig.DataMessageProcessingPackage.HandshakeDataMessageValidator.HandleHandshake(this, taskResult);
        ProcessDone();
    }

    private void StartSendingMessage()
    {
        // Create cancellation token
        _ctsWait = new CancellationTokenSource(AdditionalTimeout);
        _ctsWait.Token.Register(() =>
        {
            if (_taskCompletionSourceSend is not
                {
                    Task:
                    {
                        IsCompleted: false, IsCanceled: false, IsFaulted: false, IsCompletedSuccessfully: false
                    }
                })
            {
                return;
            }

            _taskCompletionSourceSend?.SetResult(false);

        });

        // Create the waiting task for sending (preparing waiting has to be done)
        _taskCompletionSourceSend = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        AsyncHelper.FireAndForget(() =>
        {
            try
            {
                // Now wait until preparing waiting is done
                var taskResult = AsyncHelper.RunSync(() => _taskCompletionSourceSend.Task);
                _taskCompletionSourceSend = null;

                if (!taskResult)
                {
                    return;
                }

                Task.Delay(50).Wait();

                SendMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
    }

    /// <summary>
    /// Receive a handshake message
    /// </summary>
    /// <param name="handshakeMessage">Current handshake message</param>
    public void HandshakeReceived(InboundHandshakeMessage handshakeMessage)
    {
        Debug.Print($"SPP: Handshake received {DateTime.Now:O}");
        //DataMessagingConfig.MonitorLogger?.LogDebug($"Message {Message.MessageId}: received handshake [{handshakeMessage.HandshakeMessageType:X2} {handshakeMessage.BlockAndRc:X2}]");
        DataMessagingConfig.MonitorLogger?.LogDebug($"Message {Message.MessageId}: received handshake [{handshakeMessage.HandshakeMessageType:X2}]");

        IInboundDataMessage rm = handshakeMessage;

        if (rm == null || _taskCompletionSourceWait == null)
        {
            return;
        }

        var valResult = DataMessagingConfig.DataMessageProcessingPackage.HandshakeDataMessageValidator
            .IsHandshakeForSentMessage(Message, rm);

        if (!valResult.IsMessageValid)
        {
            return;
        }

        // Stop the waiting task now
        if (_taskCompletionSourceWait is { Task: { IsCompleted: false, IsCanceled: false, IsFaulted: false, IsCompletedSuccessfully: false } })
        {
            _taskCompletionSourceWait.SetResult(handshakeMessage);
        }
    }

    /// <summary>
    /// Register a wait for ACK state to be handled
    /// </summary>
    public override void RegisterWaitState()
    {
        DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.RegisterWaitState(this);
    }

    /// <summary>
    /// Unregister a wait for ACK state to be handled
    /// </summary>
    public override void UnregisterWaitState()
    {
        // Unregister the wait state from the queue in wait state manager
        UnregisterWaitStateDelegate?.Invoke(this);
    }
}