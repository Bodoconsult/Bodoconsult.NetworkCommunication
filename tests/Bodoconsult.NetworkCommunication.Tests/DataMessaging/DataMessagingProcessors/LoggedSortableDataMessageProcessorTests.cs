// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessagingProcessors;

[TestFixture]
internal class LoggedSortableDataMessageProcessorTests
{
    private bool _wasDataMessageFired;

    private readonly IAppLoggerProxy _logger = TestDataHelper.Logger;

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
        var logger = new FakeInboundDataLogger();
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.DataMessageProcessingPackage.DataMessageSorter = new DefaultInboundDataMessageSorter(_logger);
        config.DataMessageProcessingPackage.DataLoggers.Add(logger);

        // Act  
        var proc = new LoggedSortableDataMessageProcessor(config);

        // Assert#
        using (Assert.EnterMultipleScope())
        {
            Assert.That(proc.Config, Is.EqualTo(config));
            Assert.That(logger.WasLogged, Is.False);
        }
    }

    [Test]
    public void ProcessMessage_ValidHandshake_Processed()
    {
        // Arrange 
        var logger = new FakeInboundDataLogger();
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.DataMessageProcessingPackage.DataMessageSorter = new DefaultInboundDataMessageSorter(_logger);
        config.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;
        config.DataMessageProcessingPackage.DataLoggers.Add(logger);

        var proc = new LoggedSortableDataMessageProcessor(config);

        var msg = new InboundHandshakeMessage();

        // Act  
        Assert.DoesNotThrow(() =>
        {
            proc.ProcessMessage(msg);
        });

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(proc.Config, Is.EqualTo(config));
            Assert.That(logger.WasLogged, Is.False);
        }
    }

    [Test]
    public void ProcessMessage_ValidDataMessage_DelegateFired()
    {
        // Arrange 
        var logger = new FakeInboundDataLogger();
        var config = TestDataHelper.GetDataMessagingConfig();
        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.DataMessageProcessingPackage.DataMessageSorter = new DefaultInboundDataMessageSorter(_logger);
        config.RaiseCommLayerDataMessageReceivedDelegate = RaiseCommLayerDataMessageReceivedDelegate;
        config.DataMessageProcessingPackage.DataLoggers.Add(logger);

        var proc = new LoggedSortableDataMessageProcessor(config);

        var msg = new SdcpSortableInboundDataMessage();

        // Act  
        proc.ProcessMessage(msg);

        // Assert
        Wait.Until(() => _wasDataMessageFired);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_wasDataMessageFired, Is.True);
            Assert.That(logger.WasLogged, Is.True);
        }
    }

    private void RaiseCommLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        _wasDataMessageFired = true;
    }
}