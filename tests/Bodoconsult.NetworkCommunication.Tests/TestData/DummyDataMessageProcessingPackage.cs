// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.Interfaces;
using Moq;

namespace Bodoconsult.NetworkCommunication.Tests.TestData;

/// <summary>
/// Dummy implementatioon of <see cref="IDataMessageProcessingPackage"/>
/// </summary>
public class DummyDataMessageProcessingPackage : IDataMessageProcessingPackage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    public DummyDataMessageProcessingPackage(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;
        WaitStateManager = new DefaultWaitStateManager(dataMessagingConfig);
        HandshakeDataMessageValidator = new DummyHandshakeValidator();
        DataMessageSplitter = new Mock<IDataMessageSplitter>().Object;
        DataMessageCodingProcessor = new Mock<IDataMessageCodingProcessor>().Object;
        DataMessageProcessor = new Mock<IDataMessageProcessor>().Object;
        DataBlockCodingProcessor = new Mock<IDataBlockCodingProcessor>().Object;
        DataMessageValidator = new Mock<IDataMessageValidator>().Object;
        DataMessageHandshakeFactory = new Mock<IDataMessageHandshakeFactory>().Object;
        OutboundDataMessageFactory = new Mock<IOutboundDataMessageFactory>().Object;
    }

    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Current data message splitter
    /// </summary>
    public IDataMessageSplitter DataMessageSplitter { get; set; }

    /// <summary>
    /// Current data message coding processor
    /// </summary>
    public IDataMessageCodingProcessor DataMessageCodingProcessor { get; set; }

    /// <summary>
    /// Current data message processor for internal forwarding of the received messages
    /// </summary>
    public IDataMessageProcessor DataMessageProcessor { get; set; }

    /// <summary>
    /// Current <see cref="IDataBlockCodingProcessor"/> instance
    /// </summary>
    public IDataBlockCodingProcessor DataBlockCodingProcessor { get; set; }

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
    public IDataMessageValidator DataMessageValidator { get; set; }

    /// <summary>
    /// Factory for creation of handshakes to be sent for received messages
    /// </summary>
    public IDataMessageHandshakeFactory DataMessageHandshakeFactory { get; set; }

    /// <summary>
    /// Factory for outbound data messages
    /// </summary>
    public IOutboundDataMessageFactory OutboundDataMessageFactory { get; set; }

    public void LoadCustomDataBlockCodecs()
    {
        throw new NotImplementedException();
    }
}