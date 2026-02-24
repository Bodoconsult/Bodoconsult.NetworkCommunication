// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessingPackage"/> for BTCP protocol
/// </summary>
public class BtcpDataMessageProcessingPackage : IDataMessageProcessingPackage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public BtcpDataMessageProcessingPackage(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;

        // *******************************
        // Now setup the dependent objects

        // 1. Message splitter
        DataMessageSplitter = new BtcpDataMessageSplitter();

        // 2. Codecs
        DataMessageCodingProcessor = new DefaultDataMessageCodingProcessor();
        LoadCodecs();

        // 3. Internal forwarding
        DataMessageProcessor = new DefaultDataMessageProcessor(dataMessagingConfig);

        // 4. Wait state handler
        WaitStateManager = new DefaultWaitStateManager(dataMessagingConfig);

        // 5. Handshake validator
        HandshakeDataMessageValidator = new BtcpHandshakeDataMessageValidator();

        // 6. Data message validator
        DataMessageValidator = new BtcpDataMessageValidator();

        // 7. Handshake creation factory
        DataMessageHandshakeFactory = new BtcpHandshakeFactory();
    }

    private void LoadCodecs()
    {
        var handShakeCodec = new BtcpHandshakeMessageCodec();
        DataMessageCodingProcessor.MessageCodecs.Add(handShakeCodec);

        DataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();

        LoadCustomDataBlockCodecs();

        var deviceMessageCodec = new BtcpDataMessageCodec(DataBlockCodingProcessor);
        DataMessageCodingProcessor.MessageCodecs.Add(deviceMessageCodec);

        var rawCodec = new RawDataMessageCodec();
        DataMessageCodingProcessor.MessageCodecs.Add(rawCodec);
    }

    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Current data message splitter
    /// </summary>
    public IDataMessageSplitter DataMessageSplitter { get; }

    /// <summary>
    /// Current data message coding processor
    /// </summary>
    public IDataMessageCodingProcessor DataMessageCodingProcessor { get; }

    /// <summary>
    /// Current data message processor for internal forwarding of the received messages
    /// </summary>
    public IDataMessageProcessor DataMessageProcessor { get; }

    /// <summary>
    /// Current <see cref="IDataBlockCodingProcessor"/> instance
    /// </summary>
    public IDataBlockCodingProcessor DataBlockCodingProcessor { get; protected set; }

    /// <summary>
    /// Current wait state manager
    /// </summary>
    public IWaitStateManager WaitStateManager { get; }

    /// <summary>
    /// Current validator impl for handshake messages
    /// </summary>
    public IHandshakeDataMessageValidator HandshakeDataMessageValidator { get; }

    /// <summary>
    /// Current validator impl for data messages
    /// </summary>
    public IDataMessageValidator DataMessageValidator { get; }

    /// <summary>
    /// Factory for creation of handshakes to be sent for received messages
    /// </summary>
    public IDataMessageHandshakeFactory DataMessageHandshakeFactory { get; }

    /// <summary>
    /// Load custom data block codecs. This method should be overwritten to load your app specific codecs
    /// </summary>
    public virtual void LoadCustomDataBlockCodecs()
    {
        // Load your datablock codes here
        DataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
    }
}