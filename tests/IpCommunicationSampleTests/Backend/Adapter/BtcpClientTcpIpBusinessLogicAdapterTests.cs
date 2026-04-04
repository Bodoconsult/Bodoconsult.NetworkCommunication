// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

namespace IpCommunicationSampleTests.Backend.Adapter;

[TestFixture]
internal class BtcpClientTcpIpBusinessLogicAdapterTests
{
    private readonly IAppLoggerProxy _logger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly IIpDevice _device = TestDataHelper.CreateSimpleDevice();
    private readonly FakeBusinessTransactionManager _businessTransactionManager = new();

    public BtcpClientTcpIpBusinessLogicAdapterTests()
    {
        _device.LoadCommAdapter(new FakeIpCommunicationAdapter());
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _logger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new ClientInboundBtcpMessageToBtRequestDataConverter(_logger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new ClientInboundBtcpMessageToBtReplyConverter(_logger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new ClientBtRequestDataToOutboundBtcpMessageConverter(_logger);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new ClientBtReplyToOutboundDataMessageConverter(_logger);

        // Act  
        var adapter = new BtcpClientTcpIpBusinessLogicAdapter(_device, _businessTransactionManager,
            inboundDataMessageToBtRequestConverter, inboundDataMessageToBtReplyConverter,
            outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(adapter.AppLoggerProxy, Is.EqualTo(_logger));
            Assert.That(adapter.InboundDataMessageToBtReplyConverter,
                Is.EqualTo(inboundDataMessageToBtReplyConverter));
            Assert.That(adapter.InboundDataMessageToBtRequestConverter,
                Is.EqualTo(inboundDataMessageToBtRequestConverter));
            Assert.That(adapter.OutboundBtReplyToOutboundDataMessageConverter,
                Is.EqualTo(outboundBtReplyDataMessageConverter));
            Assert.That(adapter.OutboundBtRequestToOutboundDataMessageConverter,
                Is.EqualTo(outboundBtRequestToOutboundDataMessageConverter));
        }
    }

    [Test]
    public void DefaultReceiveMessage_ValidMessage_CorrectBusinessTransactionCalled()
    {
        // Arrange 
        const int transactionId = 202;
        var transactionUid = Guid.NewGuid();

        _businessTransactionManager.RequestedTransactionId = 0;
        Assert.That(_businessTransactionManager.RequestedTransactionId, Is.Zero);

        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new ClientInboundBtcpMessageToBtRequestDataConverter(_logger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new ClientInboundBtcpMessageToBtReplyConverter(_logger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new ClientBtRequestDataToOutboundBtcpMessageConverter(_logger);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new ClientBtReplyToOutboundDataMessageConverter(_logger);

        var adapter = new BtcpClientTcpIpBusinessLogicAdapter(_device, _businessTransactionManager,
            inboundDataMessageToBtRequestConverter, inboundDataMessageToBtReplyConverter,
            outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);

        var msg = new BtcpRequestInboundDataMessage(transactionId, transactionUid);

        // Act  
        adapter.DefaultReceiveMessage(msg);

        // Assert
        Wait.Until(() => _businessTransactionManager.RequestedTransactionId > 0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_businessTransactionManager.RequestedTransactionId, Is.EqualTo(transactionId));
        }
    }

}