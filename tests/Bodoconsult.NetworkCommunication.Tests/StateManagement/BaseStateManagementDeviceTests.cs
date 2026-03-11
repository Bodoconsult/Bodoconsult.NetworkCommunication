// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.That(device.StateMachineStateFactory, Is.Not.Null);
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

        var builder = new DeviceOfflineStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate
        };

        // Act  
        var state = builder.BuildState(config);

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

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState)
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

        var builder = new DeviceStartStreamingStateBuilder();

        var config = new OrderBasedActionStateConfiguration(DefaultStateNames.DeviceStartStreamingState)
        {
            CurrentContext = device,
            HandleAsyncMessageDelegate = DelegateHelper.HandleAsyncMessageDelegate,
            HandleComDevCloseDelegate = DelegateHelper.HandleComDevCloseDelegate,
            HandleErrorMessageDelegate = DelegateHelper.HandleErrorMessageDelegate,
            HandleRegularStateRequestAnswerDelegate = DelegateHelper.HandleRegularStateRequestAnswerDelegate,
            PrepareRegularStateRequestDelegate = DelegateHelper.PrepareRegularStateRequestDelegate,
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            PrepareOrdersForStateMachineStateDelegate = DelegateHelper.PrepareOrdersForStateMachineStateDelegate
        };

        var newState = builder.BuildState(config);

        // Act  
        device.RequestState(newState);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.CurrentState, Is.Not.Null);
            Assert.That(device.CurrentState.Name, Is.EqualTo(newState.Name));
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOnline));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.TryToConnect));
        }
    }

    [Test]
    public void RequestState_NoActionStateConfiguration_StatesSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceReadyStateBuilder();

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceReadyState)
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
            Assert.That(device.CurrentState.Name, Is.EqualTo(newState.Name));
            Assert.That(device.DeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateReady));
            Assert.That(device.BusinessSubState, Is.EqualTo(DefaultBusinessSubStates.NotSet));
        }
    }

}