// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessagingProcessors;

[TestFixture]
internal class SortableDataMessageProcessorTests
{
    private bool _wasDataMessageFired;

    private readonly IAppLoggerProxy _logger = TestDataHelper.GetFakeAppLoggerProxy();

    [OneTimeTearDown]
    public void CleanUp()
    {
        _logger.Dispose();
    }


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
        config.DataMessageProcessingPackage.DataMessageSorter = new DefaultInboundDataMessageSorter(_logger);

        // Act  
        var proc = new SortableDataMessageProcessor(config);

        // Assert
        Assert.That(proc.Config, Is.EqualTo(config));
    }

    [Test]
    public void ProcessMessage_ValidHandshake_Processed()
    {
        // Arrange 
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.DataMessageProcessingPackage.DataMessageSorter = new DefaultInboundDataMessageSorter(_logger);
        config.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var proc = new SortableDataMessageProcessor(config);

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
        config.DataMessageProcessingPackage.DataMessageSorter = new DefaultInboundDataMessageSorter(_logger);
        config.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;

        var proc = new SortableDataMessageProcessor(config);

        var msg = new SdcpSortableInboundDataMessage();

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