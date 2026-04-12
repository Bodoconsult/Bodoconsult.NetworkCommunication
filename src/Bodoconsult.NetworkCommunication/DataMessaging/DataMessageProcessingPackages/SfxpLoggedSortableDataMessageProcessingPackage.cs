// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;
using Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;
using Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessingPackage"/> for SFXP protocol on client side with message sorting and logging
/// </summary>
public class SfxpLoggedSortableDataMessageProcessingPackage : IDataMessageProcessingPackage
{
    /// <summary>
    /// Default ctor
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SfxpLoggedSortableDataMessageProcessingPackage(IDataMessagingConfig dataMessagingConfig)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        DataMessagingConfig = dataMessagingConfig;
        DataMessagingConfig.DataMessageProcessingPackage = this;

        // *******************************
        // Now setup the dependent objects

        // 0. Data sorter
        DataMessageSorter = new DefaultInboundDataMessageSorter();

        // 1. Message splitter
        DataMessageSplitter = new UdpDatagramDataMessageSplitter();

        // 2. Codecs
        DataMessageCodingProcessor = new DefaultDataMessageCodingProcessor();
        DataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        LoadCodecs();

        // 3. Internal forwarding
        DataMessageProcessor = new LoggedSortableDataMessageProcessor(dataMessagingConfig);

        // 4. Wait state handler
        WaitStateManager = new DefaultWaitStateManager(dataMessagingConfig);

        // 5. Handshake validator
        HandshakeDataMessageValidator = new SfxpHandshakeDataMessageValidator();

        // 6. Data message validator
        DataMessageValidator = new SfxpDataMessageValidator();

        // 7. Handshake creation factory
        DataMessageHandshakeFactory = new DoNotSendHandshakeFactory();

        // 8. Outbound data message factory
        OutboundDataMessageFactory = new FakeOutboundDataMessageFactory();
    }

    private void LoadCodecs()
    {
        DataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();

        LoadCustomDataBlockCodecs();

        var deviceMessageCodec = new SfxpDataMessageCodec(DataBlockCodingProcessor);
        DataMessageCodingProcessor.MessageCodecs.Add(deviceMessageCodec);

        // Needed for client hello
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
    /// Current data loggers. May contain zero or more loggers. Each message can be logged only by one logger (or none)
    /// </summary>
    public List<IInboundDataLogger> DataLoggers { get; } = [];

    /// <summary>
    /// Current data message sorter instance to use or null
    /// </summary>
    public IInboundDataMessageSorter? DataMessageSorter { get; set; }

    /// <summary>
    /// Load custom data block codecs. This method should be overwritten to load your app specific codecs
    /// </summary>
    public virtual void LoadCustomDataBlockCodecs()
    {
        // Load your datablock codes here
        DataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
    }
}