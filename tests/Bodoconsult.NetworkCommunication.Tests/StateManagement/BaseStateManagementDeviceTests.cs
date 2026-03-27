// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement;

[TestFixture]
internal class BaseStateManagementDeviceTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var device = TestDataHelper.CreateStateMachineDevice();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Null);
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOffline));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.NotSet));
            Assert.That(device.ClientNotificationManager, Is.Not.Null);
            Assert.That(device.CommunicationAdapter, Is.Not.Null);
            Assert.That(device.OrderManager, Is.Not.Null);
            Assert.That(device.OrderProcessor, Is.Not.Null);
            Assert.That(device.DataMessagingConfig, Is.Not.Null);
            Assert.That(device.StateMachineStateFactory, Is.Null);
        }
    }

    [Test]
    public void SetBusinessSubState_ValidSetup_BusinessSubstateSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        // Act  
        device.SetBusinessSubState(DefaultBusinessSubStates.Connected);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Null);
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOffline));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.Connected));
        }
    }

    [Test]
    public void SetStates_ValidSetup_StatesSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        // Act  

        // Act  
        device.SetStates(DefaultDeviceStates.DeviceStateOnline, DefaultBusinessSubStates.Connected);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Null);
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOnline));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.Connected));
        }
    }

    [Test]
    public void RequestState_OrderlessActionStateConfiguration_StatesSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceOfflineStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, builder)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
        };

        var newState = builder.BuildState(config);

        // Act  
        device.RequestState(newState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(device.CurrentState);
            Assert.That(device.CurrentState.Name, Is.EqualTo(newState.Name));
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOffline));

            // Business substate not that important here. Depending on runtime we may have 2 possible values
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.PingingTower).Or.EqualTo(DefaultBusinessSubStates.NotSet));
        }
    }

    [Test]
    public void RequestState_OrderBasedActionStateConfiguration_StatesSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var ps = new TncpParameterSet();
        var ps2 = new TncpParameterSet();

        var builder = new DeviceStartStreamingStateBuilder();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, builder)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
        };

        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");
        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");

        config.ParameterSets.Add(ps);
        config.ParameterSets.Add(ps2);

        var newState = builder.BuildState(config);

        // Act  
        device.RequestState(newState);

        var os = (IOrderBasedActionStateMachineState)newState;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(device.CurrentState);
            Assert.That(device.CurrentState.Name, Is.EqualTo(newState.Name));
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOnline));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.TryToConnect));

            Assert.That(os.Orders.Count, Is.EqualTo(os.OrderConfigurations.Count));
        }
    }

    [Test]
    public void RequestState_NoActionStateConfiguration_StatesSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceReadyStateBuilder();

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState, builder)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
        };

        var newState = builder.BuildState(config);

        // Act  
        device.RequestState(newState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(device.CurrentState);
            Assert.That(device.CurrentState.Name, Is.EqualTo(newState.Name));
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateReady));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.NotSet));
        }
    }

    [Test]
    public void RegisterJobState_OrderBasedActionStateConfiguration_StateAddedToJobstates()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceStartStreamingStateBuilder();

        var ps = new SdcpParameterSet();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, builder)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            
        };

        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");
        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");

        config.ParameterSets.Add(ps);
        config.ParameterSets.Add(ps);

        var newState = (IJobStateMachineState)builder.BuildState(config);

        // Act  
        device.RegisterJobState(newState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.JobStates.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void RequestNewDeviceState_ValidSetup_DeviceStateSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var deviceState = DefaultDeviceStates.DeviceStateReady;

        // Act  
        device.RequestNewDeviceState(deviceState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.DeviceState, Is.EqualTo(deviceState));
        }
    }

    [Test]
    public void SaveJobState_OrderBasedActionStateConfiguration_StateSaved()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceStartStreamingStateBuilder();

        var ps = new SdcpParameterSet();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, builder)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
        };

        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");
        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");

        config.ParameterSets.Add(ps);
        config.ParameterSets.Add(ps);

        var newState = (IJobStateMachineState)builder.BuildState(config);

        // Act  
        device.SaveJobState(newState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.SavedJobState, Is.EqualTo(newState));
        }
    }

    [Test]
    public void RestoreSavedJobState_OrderBasedActionStateConfiguration_StateSaved()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceStartStreamingStateBuilder();

        var ps = new SdcpParameterSet();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, builder)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
        };

        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");
        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");

        config.ParameterSets.Add(ps);
        config.ParameterSets.Add(ps);

        var newState = (IJobStateMachineState)builder.BuildState(config);

        device.SaveJobState(newState);

        Assert.That(device.CurrentState, Is.Null);

        // Act  
        device.RestoreSavedJobState();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.EqualTo(newState));
        }
    }

}