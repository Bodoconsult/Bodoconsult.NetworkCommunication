// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessagingProcessors;

[TestFixture]
internal class DefaultDataMessageProcessorTests
{
    private bool _wasDataMessageFired;

    [SetUp]
    public void Setup()
    {
        _wasDataMessageFired = false;
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);

        // Act  
        var proc = new DefaultDataMessageProcessor(config);

        // Assert
        Assert.That(proc.Config, Is.EqualTo(config));
    }

    [Test]
    public void ProcessMessage_ValidHandshake_Processed()
    {
        // Arrange 
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var proc = new DefaultDataMessageProcessor(config);

        var msg = new InboundHandshakeMessage();

        // Act  
        Assert.DoesNotThrow(() =>
        {
            proc.ProcessMessage(msg);
        });

        // Assert
        Assert.That(proc.Config, Is.EqualTo(config));
    }

    [Test]
    public void ProcessMessage_ValidDataMessage_DelegateFired()
    {
        // Arrange 
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var proc = new DefaultDataMessageProcessor(config);

        var msg = new SdcpInboundDataMessage();

        // Act  
        proc.ProcessMessage(msg);

        // Assert
        Wait.Until(() => _wasDataMessageFired);
        Assert.That(_wasDataMessageFired, Is.True);
    }

    private void RaiseCommLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        _wasDataMessageFired = true;
    }
}