// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;


namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class StateMachineOrderProcessorFactoryTests
{
    private readonly IAppDateService _dateTimeService = TestDataHelper.AppDateService;
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
    private readonly OrderPipeline _orderPipeline;

    private readonly IAppBenchProxy _benchLogger = TestDataHelper.GetFakeAppBenchProxy();

    private readonly IOrderManagementClientNotificationManager _notificationManager = new DoNothingOrderManagementClientNotificationManager();
    private readonly SyncOrderManager _syncManager;

    public StateMachineOrderProcessorFactoryTests()
    {
        var commAdapter = TestDataHelper.FakeIpCommunicationAdapter;
        var stepFactory = new RequestStepProcessorFactory();

        var device = TestDataHelper.CreateOrderManagementDevice();
        device.LoadCommAdapter(commAdapter);

        var orderProcessorFactory = new RequestProcessorFactory(stepFactory, device);

        _orderPipeline = new OrderPipeline(_dateTimeService, orderProcessorFactory, _appLogger, "Tower 000123: ");
        _syncManager = new SyncOrderManager();

        //var om = new FakeOrderManager();
        //device.LoadDeviceOrderManager(om);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _benchLogger.Dispose();
        _syncManager.Dispose();
        _orderPipeline.Dispose();
        _appLogger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var factory = new StateMachineOrderProcessorFactory(_dateTimeService, _syncManager, _notificationManager, _benchLogger);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.ClientNotificationManager, Is.EqualTo(_notificationManager));
            Assert.That(factory.SyncOrderManager, Is.EqualTo(_syncManager));
            Assert.That(factory.AppBenchProxy, Is.EqualTo(_benchLogger));
            Assert.That(factory.AppDateService, Is.EqualTo(_dateTimeService));
        }
    }

    [Test]
    public void CreateInstance_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var factory = new StateMachineOrderProcessorFactory(_dateTimeService, _syncManager, _notificationManager, _benchLogger);

        var device = TestDataHelper.CreateStateMachineDevice();

        // Act  
        var instance = factory.CreateInstance(device, _orderPipeline);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.GetType(), Is.EqualTo(typeof(StateMachineOrderProcessor)));
            Assert.That(instance.CurrentDevice, Is.EqualTo(device));
            Assert.That(instance.ClientNotificationManager, Is.EqualTo(_notificationManager));
        }
    }
}