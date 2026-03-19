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
    public void RegisterConfiguration_ValidNoActionStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState, new DeviceReadyStateBuilder())
        {
            CheckJobstatesActionForStateDelegate = DelegateHelper.DefaultCheckJobstatesActionForStateDelegate,
            CurrentContext = device
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.Null);
        }
    }

    [Test]
    public void RegisterConfiguration_ValidOrderBasedActionStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        var config = new OrderBasedActionStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
        {
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            CurrentContext = device
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.Null);
        }
    }

    [Test]
    public void RegisterConfiguration_ValidJobStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
        {
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            CurrentContext = device
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.Null);
        }
    }

    [Test]
    public void RegisterConfiguration_ValidOrderlessActionStateConfiguration_ConfigurationLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, new DeviceOfflineStateBuilder())
        {
            ExecuteActionForStateDelegate = DelegateHelper.ExecuteActionForStateDelegate,
            CurrentContext = device
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(factory.StateConfigurations, Is.Not.Null);
            Assert.That(factory.StateConfigurations.Count, Is.EqualTo(1));
            Assert.That(config.CurrentContext, Is.Null);
        }
    }

    [Test]
    public void CreateInstance_ValidOrderlessActionStateConfiguration_StateIsCreated()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, new DeviceOfflineStateBuilder())
        {
            ExecuteActionForStateDelegate = DelegateHelper.ExecuteActionForStateDelegate
        };

        factory.RegisterConfiguration(config);

        // Act  
        var state = factory.CreateInstance(device, DefaultStateNames.DeviceOfflineState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
        }
    }

    [Test]
    public void GetConfiguration_ValidOrderBasedActionStateConfiguration_ReturnsConfiguration()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();

        var config = new OrderBasedActionStateConfiguration(DefaultStateNames.DeviceStartStreamingState, new DeviceStartStreamingStateBuilder())
        {
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            CurrentContext = device
        };

        factory.RegisterConfiguration(config);

        // Act  
        var result = factory.GetConfiguration(config.StateName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(config));
        }
    }
}