// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Builders;

[TestFixture]
internal class DeviceStartSnapshotStateBuilderTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new DeviceStartSnapshotStateBuilder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(string.IsNullOrEmpty(builder.StateName), Is.False);
            Assert.That(builder.StateName, Is.EqualTo(DefaultStateNames.DeviceStartSnapshotState));
            Assert.That(builder.StateId, Is.EqualTo(DefaultStateIds.DeviceStartSnapshotState));
        }
    }

    [Test]
    public void BuildState_WrongStatenameInConfig_ThrowsException()
    {
        // Arrange 
        var builder = new DeviceStartSnapshotStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState, builder);

        // Act and assert
        Assert.Throws<ArgumentException>(() =>
        {
            builder.BuildState(config);
        });
    }

    [Test]
    public void BuildState_ValidSetup_StateBuilded()
    {
        // Arrange
        var device = TestDataHelper.CreateStateMachineDevice();

        var builder = new DeviceStartSnapshotStateBuilder();

        var ps = new SdcpParameterSet();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartSnapshotState, builder)
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

        // Act  
        var state = (IOrderBasedActionStateMachineState)builder.BuildState(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.CurrentContext, Is.EqualTo(device));
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(DefaultStateIds.DeviceStartSnapshotState));

            Assert.That(state.OrderFinishedSucessfullyDelegate, Is.Not.Null);
            Assert.That(state.OrderFinishedUnsucessfullyDelegate, Is.Not.Null);

            Assert.That(state.HandleAsyncMessageDelegate, Is.Not.Null);
            Assert.That(state.HandleComDevCloseDelegate, Is.Not.Null);
            Assert.That(state.HandleErrorMessageDelegate, Is.Not.Null);
            Assert.That(state.HandleRegularStateRequestAnswerDelegate, Is.Not.Null);
            Assert.That(state.PrepareRegularStateRequestDelegate, Is.Not.Null);

            Assert.That(state.AllowedNextStates, Does.Contain(DefaultStateNames.DeviceOfflineState));
            Assert.That(state.AllowedNextStates, Does.Contain(DefaultStateNames.DeviceSnapshotState));
        }
    }

    [Test]
    public void InitiateState_ValidSetup_StateBuilded()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var ps = new TncpParameterSet();
        var ps2 = new TncpParameterSet();

        var builder = new DeviceStartSnapshotStateBuilder();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartSnapshotState, builder)
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

        var state = (IOrderBasedActionStateMachineState)builder.BuildState(config);

        // Act  
        state.InitiateState();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Orders.Count, Is.EqualTo(state.OrderConfigurations.Count));
        }
    }

    [Test]
    public void RunNextOrder_OneOrder_Successful()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var ps = new TncpParameterSet();

        var builder = new DeviceStartSnapshotStateBuilder();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartSnapshotState, builder)
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

        config.ParameterSets.Add(ps);

        var state = (IOrderBasedActionStateMachineState)builder.BuildState(config);
        state.InitiateState();
        device.SetFakeState(state);

        // Act  
        state.RunNextOrder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Orders.Count, Is.EqualTo(state.OrderConfigurations.Count));
            Assert.That(state.CurrentOrderIndex, Is.EqualTo(state.OrderConfigurations.Count));
        }
    }

    [Test]
    public void RunNextOrder_TwoOrders_Successful()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();

        var ps = new TncpParameterSet();
        var ps2 = new TncpParameterSet();
        
        var builder = new DeviceStartSnapshotStateBuilder();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartSnapshotState, builder)
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

        var state = (IOrderBasedActionStateMachineState)builder.BuildState(config);
        state.InitiateState();
        device.SetFakeState(state);

        // Act  
        state.RunNextOrder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.Orders.Count, Is.EqualTo(state.OrderConfigurations.Count));
            Assert.That(state.CurrentOrderIndex, Is.EqualTo(state.OrderConfigurations.Count));
        }
    }
}