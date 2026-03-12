// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Factories;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class StateMachineStateFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var factory = new StateMachineStateFactory();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.Zero);
        }
    }

    [Test]
    public void LoadContext_ValidSetup_ContextLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        // Act  
        factory.LoadContext(device);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.CurrentContext, Is.Not.Null);
            Assert.That(factory.CurrentContext, Is.EqualTo(device));
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.Zero);
        }
    }

    [Test]
    public void RegisterConfiguration_ValidNoActionStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();
        factory.LoadContext(device);

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState, new DeviceReadyStateBuilder())
        {
            CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.CurrentContext, Is.Not.Null);
            Assert.That(factory.CurrentContext, Is.EqualTo(device));
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.EqualTo(device));
        }
    }

    [Test]
    public void RegisterConfiguration_ValidOrderBasedActionStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();
        factory.LoadContext(device);

        var config = new OrderBasedActionStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
        {
            PrepareOrdersForStateMachineStateDelegate = DelegateHelper.PrepareOrdersForStateMachineStateDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.CurrentContext, Is.Not.Null);
            Assert.That(factory.CurrentContext, Is.EqualTo(device));
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.EqualTo(device));
        }
    }

    [Test]
    public void RegisterConfiguration_ValidJobStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();
        factory.LoadContext(device);

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
        {
            PrepareOrdersForStateMachineStateDelegate = DelegateHelper .PrepareOrdersForStateMachineStateDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.CurrentContext, Is.Not.Null);
            Assert.That(factory.CurrentContext, Is.EqualTo(device));
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.EqualTo(device));
        }
    }

    [Test]
    public void RegisterConfiguration_ValidOrderlessActionStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();
        factory.LoadContext(device);

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, new DeviceOfflineStateBuilder())
        {
            ExecuteActionForStateDelegate = DelegateHelper.ExecuteActionForStateDelegate
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.CurrentContext, Is.Not.Null);
            Assert.That(factory.CurrentContext, Is.EqualTo(device));
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.EqualTo(device));
        }
    }
}