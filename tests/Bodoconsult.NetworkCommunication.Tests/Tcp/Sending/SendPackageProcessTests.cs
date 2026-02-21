// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.TestData;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Sending;

public class SendPackageProcessDuplexIoTests
{

    private readonly IAppLoggerProxy _logger = new AppLoggerProxy(new FakeLoggerFactory(), new LogDataFactory());

    [OneTimeTearDown]
    public void CleanUp()
    {
        _logger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var p = new SendPacketProcess();

        // Assert
        Assert.That(p.DataMessagingConfig, Is.Null);
        Assert.That(p.Message ,Is.Null);
        Assert.That(p.DuplexIo, Is.Null);

        Assert.That(p.TotalTimeout, Is.GreaterThan(p.Timeout));
    }

    [Test]
    public void LoadDependencies_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataMessagingConfig dataMessagingConfig = new DefaultDataMessagingConfig();
        IOutboundDataMessage message = new RawOutboundDataMessage();
        IDuplexIo duplexIo = new FakeHandshakeDuplexIo(dataMessagingConfig);

        var p = new SendPacketProcess();

        // Act  
        p.LoadDependencies(duplexIo, message, dataMessagingConfig);

        // Assert
        Assert.That(p.DataMessagingConfig, Is.Not.Null);
        Assert.That(p.Message, Is.Not.Null);
        Assert.That(p.DuplexIo, Is.Not.Null);
    }


    [Test]
    public void RegisterWaitState_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataMessagingConfig dataMessagingConfig = new DefaultDataMessagingConfig();

        dataMessagingConfig.DataMessageProcessingPackage = new DummyDataMessageProcessingPackage(dataMessagingConfig);
            
        IOutboundDataMessage message = new RawOutboundDataMessage();
        IDuplexIo duplexIo = new FakeHandshakeDuplexIo(dataMessagingConfig);

        var p = new SendPacketProcess();

        p.LoadDependencies(duplexIo, message, dataMessagingConfig);

        // Act  
        p.RegisterWaitState();

        // Assert
        Assert.That(p.DataMessagingConfig, Is.Not.Null);
        Assert.That(p.Message, Is.Not.Null);
        Assert.That(p.DuplexIo, Is.Not.Null);
        Assert.That(dataMessagingConfig.DataMessageProcessingPackage.WaitStateManager.Count, Is.Not.EqualTo(0));

    }

    [Test]
    public void Execute_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataMessagingConfig dataMessagingConfig = new DefaultDataMessagingConfig();
        dataMessagingConfig.CheckIfDeviceIsReadyDelegate = () => true;
        dataMessagingConfig.AppLogger = _logger;
        dataMessagingConfig.MonitorLogger = _logger;

        dataMessagingConfig.DataMessageProcessingPackage = new DummyDataMessageProcessingPackage(dataMessagingConfig);


        IOutboundDataMessage message = new RawOutboundDataMessage();
        IDuplexIo duplexIo = new FakeHandshakeDuplexIo(dataMessagingConfig);

        var p = new SendPacketProcess();

        p.LoadDependencies(duplexIo, message, dataMessagingConfig);
        p.RegisterWaitState();

        // Act  
        p.Execute();

        // Assert
        Assert.That(p.ProcessExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));

    }

    [Test]
    public void Execute_ValidSetupNoHandshakeReceived_PropsSetCorrectly()
    {
        // Arrange 
        IDataMessagingConfig dataMessagingConfig = new DefaultDataMessagingConfig();
        dataMessagingConfig.CheckIfDeviceIsReadyDelegate = () => true;
        dataMessagingConfig.AppLogger = _logger;
        dataMessagingConfig.MonitorLogger = _logger;

        dataMessagingConfig.DataMessageProcessingPackage = new DummyDataMessageProcessingPackage(dataMessagingConfig);


        IOutboundDataMessage message = new RawOutboundDataMessage();

        var duplexIo = new FakeHandshakeDuplexIo(dataMessagingConfig);
        duplexIo.NumberOfTriesTheHandshakeIsReceived = int.MaxValue;


        var p = new SendPacketProcess();

        p.LoadDependencies(duplexIo, message, dataMessagingConfig);
        p.RegisterWaitState();

        // Act  
        p.Execute();

        // Assert
        Assert.That(p.ProcessExecutionResult, Is.SameAs(OrderExecutionResultState.Timeout));
        Assert.That(dataMessagingConfig.DataMessageProcessingPackage.WaitStateManager.Count, Is.EqualTo(0));
    }


    [Test]
    public void Execute_ValidSetupHandshakeReceivedAfter2ndTry_PropsSetCorrectly()
    {
        // Arrange 
        IDataMessagingConfig dataMessagingConfig = new DefaultDataMessagingConfig();
        dataMessagingConfig.CheckIfDeviceIsReadyDelegate = () => true;
        dataMessagingConfig.AppLogger = _logger;
        dataMessagingConfig.MonitorLogger = _logger;

        dataMessagingConfig.DataMessageProcessingPackage = new DummyDataMessageProcessingPackage(dataMessagingConfig);


        IOutboundDataMessage message = new RawOutboundDataMessage();

        var duplexIo = new FakeHandshakeDuplexIo(dataMessagingConfig);
        duplexIo.NumberOfTriesTheHandshakeIsReceived = 2;


        var p = new SendPacketProcess();

        p.LoadDependencies(duplexIo, message, dataMessagingConfig);
        p.RegisterWaitState();

        // Act  
        p.Execute();

        // Assert
        Assert.That(p.ProcessExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));

    }

    [Test]
    public void Execute_ValidSetupDelayedHandshake_PropsSetCorrectly()
    {
        // Arrange 
        IDataMessagingConfig dataMessagingConfig = new DefaultDataMessagingConfig();
        dataMessagingConfig.CheckIfDeviceIsReadyDelegate = () => true;
        dataMessagingConfig.AppLogger = _logger;
        dataMessagingConfig.MonitorLogger = _logger;

        dataMessagingConfig.DataMessageProcessingPackage = new DummyDataMessageProcessingPackage(dataMessagingConfig);


        IOutboundDataMessage message = new RawOutboundDataMessage();
        var duplexIo = new FakeHandshakeDuplexIo(dataMessagingConfig);
        duplexIo.ReceiverDelay = 2000;

        var p = new SendPacketProcess();

        p.LoadDependencies(duplexIo, message, dataMessagingConfig);
        p.RegisterWaitState();

        // Act  
        p.Execute();

        // Assert
        Assert.That(p.ProcessExecutionResult, Is.SameAs(OrderExecutionResultState.Successful));

    }

}