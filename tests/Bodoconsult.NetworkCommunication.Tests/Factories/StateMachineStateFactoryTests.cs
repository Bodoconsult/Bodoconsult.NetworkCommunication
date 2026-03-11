// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
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
            Assert.That(factory.CurrentContext, Is.Null);
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

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState)
        {
            StateBuilderBuilder = new DeviceReadyStateBuilder(),
            CheckJobstatesActionForStateDelegate = CheckJobstatesActionForStateDelegate
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

        var config = new OrderBasedActionStateConfiguration(DefaultStateNames.DeviceStartStreamingState)
        {
            StateBuilderBuilder = new DeviceStartStreamingStateBuilder(),
            PrepareOrdersForStateMachineStateDelegate = PrepareOrdersForStateMachineStateDelegate,
            OrderFinishedUnsucessfullyDelegate = OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = OrderFinishedSucessfullyDelegate
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

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState)
        {
            StateBuilderBuilder = new DeviceStartStreamingStateBuilder(),
            PrepareOrdersForStateMachineStateDelegate = PrepareOrdersForStateMachineStateDelegate,
            OrderFinishedUnsucessfullyDelegate = OrderFinishedUnsucessfullyDelegate,
            OrderFinishedSucessfullyDelegate = OrderFinishedSucessfullyDelegate
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

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState)
        {
            StateBuilderBuilder = new DeviceOfflineStateBuilder(),
            ExecuteActionForStateDelegate = ExecuteActionForStateDelegate
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

    private void ExecuteActionForStateDelegate(IOrderlessActionStateMachineState state)
    {
        // Do nothing
    }

    private void OrderFinishedSucessfullyDelegate(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    private void OrderFinishedUnsucessfullyDelegate(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    private List<IOrder> PrepareOrdersForStateMachineStateDelegate()
    {
        return [];
    }

    private void CheckJobstatesActionForStateDelegate(INoActionStateMachineState state)
    {
        // Do nothing
    }

    [Test]
    public void RegisterConfiguration_NoStateBuilder_ThrowsException()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var factory = new StateMachineStateFactory();
        factory.LoadContext(device);

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState);

        // Act and assert 
        Assert.Throws<ArgumentNullException>(() =>
        {
            factory.RegisterConfiguration(config);
        });
    }
}