// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Base class for <see cref="IWaitStateManager"/> impls
/// </summary>
public abstract class BaseWaitStateManager : IWaitStateManager
{
    protected readonly IProducerConsumerQueue<InboundHandshakeMessage> ReceivedHandshakes = new ProducerConsumerQueue<InboundHandshakeMessage>();
    protected readonly List<SendPacketProcess> WaitStates = new();

    private readonly object _waitStateLock = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseWaitStateManager(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;

        ReceivedHandshakes.ConsumerTaskDelegate = ConsumerTaskDelegate;
        ReceivedHandshakes.StartConsumer();
    }


    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Timeout for the state
    /// </summary>
    public int Timeout { get; protected set; }

    /// <summary>
    /// The last handshake block code sent. Default: 254 meaning nothing sent
    /// </summary>
    public byte LastHandshakeSendBlockCode { get; set; } = 254;

    /// <summary>
    /// Method used to bind to a delegate for receiving handshakes
    /// </summary>
    /// <param name="msg">Data message</param>
    public virtual void OnHandshakeReceived(IInboundHandShakeDataMessage msg)
    {

        Debug.Print("Handshake reached wait state manager 1");

        if (msg is not InboundHandshakeMessage handshake)
        {
            // Do nothing
            return;
        }

        RaiseHandshakeReceivedDelegate?.Invoke(msg);

        try
        {

            LastHandshakeSendBlockCode = 254;

            // Put the handshake in the processing queue
            ReceivedHandshakes.Enqueue(handshake);

            DataMessagingConfig.MonitorLogger.LogDebug($"Handshake {handshake.HandshakeMessageType} registered");

            LastMessageTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
        catch (Exception e)
        {
            DataMessagingConfig.MonitorLogger.LogError($"Handshake {handshake.HandshakeMessageType} received but handling error", e);
        }
    }

    /// <summary>
    /// The timestamp of the last message received
    /// </summary>
    public long LastMessageTimeStamp { get; set; }

    /// <summary>
    /// Register a wait for ACK state to be handled
    /// </summary>
    /// <param name="state">State to be registered for ACK handling</param>
    public void RegisterWaitState(SendPacketProcess state)
    {
        state.Timeout = Timeout;
        state.UnregisterWaitStateDelegate = UnregisterWaitState;
        lock (_waitStateLock)
        {
            WaitStates.Add(state);
        }
        //Smddevice.MonitorLogger.LogInformation("WaitState registered");
        Debug.Print($"WSM: WaitState registered: {Count}");

    }

    /// <summary>
    /// Delegate for unregistering a wait state from a <see cref="IWaitStateManager"/> implementation
    /// </summary>
    /// <param name="state">wait state to unregister</param>
    public void UnregisterWaitState(SendPacketProcess state)
    {
        lock (_waitStateLock)
        {
            if (WaitStates.Contains(state))
            {
                WaitStates.Remove(state);
            }
        }
    }

    /// <summary>
    /// Delegate called if <see cref="IWaitStateManager.OnHandshakeReceived"/> is starting to process a received handshake message
    /// </summary>
    public RaiseDataMessageHandshakeReceivedDelegate RaiseHandshakeReceivedDelegate { get; set; }

    /// <summary>
    /// Current number of wait states registered
    /// </summary>
    public int Count
    {
        get
        {
            lock (_waitStateLock)
            {
                return WaitStates.Count;
            }
        }
    }

    private void ConsumerTaskDelegate(InboundHandshakeMessage handshake)
    {
        // Get the counters at method start to avoid later change resulting from new input
        var waitStateCount = Count;

        Debug.Print($"Handshake {handshake.HandshakeMessageType} reached wait state manager 2: ({waitStateCount} states waiting)");

        List<SendPacketProcess> waitStates;

        lock (_waitStateLock)
        {
            waitStates = WaitStates.ToList();
        }

        // Check the handshake with all open waitstates
        foreach (var waitState in waitStates)
        {
            waitState.HandshakeReceived(handshake);
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        lock (_waitStateLock)
        {
            WaitStates.Clear();
        }

        ReceivedHandshakes.Dispose();
    }
}