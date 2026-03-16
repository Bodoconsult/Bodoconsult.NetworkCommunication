// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Factories;
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BtcpDataMessageProcessingPackage(IDataMessagingConfig dataMessagingConfig)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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

        // 8. Outbound data message factory
        OutboundDataMessageFactory = new BtcpOutboundDataMessageFactory();
    }

    private void LoadCodecs()
    {
        var handShakeCodec = new BtcpHandshakeMessageCodec();
        DataMessageCodingProcessor.MessageCodecs.Add(handShakeCodec);

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
    /// Factory for outbound data messages
    /// </summary>
    public IOutboundDataMessageFactory OutboundDataMessageFactory { get; }

    /// <summary>
    /// Load custom data block codecs. This method should be overwritten to load your app specific codecs
    /// </summary>
    public virtual void LoadCustomDataBlockCodecs()
    {
        // Load your datablock codes here
        DataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
    }
}

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessingPackage"/> for TNCP protocol
/// </summary>
public class TncpDataMessageProcessingPackage : IDataMessageProcessingPackage
{
    /// <summary>
    /// Default ctor
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TncpDataMessageProcessingPackage(IDataMessagingConfig dataMessagingConfig)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
        HandshakeDataMessageValidator = new TncpHandshakeDataMessageValidator();

        // 6. Data message validator
        DataMessageValidator = new TncpDataMessageValidator();

        // 7. Handshake creation factory
        DataMessageHandshakeFactory = new TncpHandshakeFactory();

        // 8. Outbound data message factory
        OutboundDataMessageFactory = new TncpOutboundDataMessageFactory();
    }

    private void LoadCodecs()
    {
        var handShakeCodec = new BtcpHandshakeMessageCodec();
        DataMessageCodingProcessor.MessageCodecs.Add(handShakeCodec);

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
    /// Factory for outbound data messages
    /// </summary>
    public IOutboundDataMessageFactory OutboundDataMessageFactory { get; }

    /// <summary>
    /// Load custom data block codecs. This method should be overwritten to load your app specific codecs
    /// </summary>
    public virtual void LoadCustomDataBlockCodecs()
    {
        // Load your datablock codes here
        DataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
    }
}