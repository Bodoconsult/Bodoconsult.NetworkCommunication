// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Builders;

[TestFixture]
internal class DeviceOnlineStateBuilderTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new DeviceOnlineStateBuilder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(string.IsNullOrEmpty(builder.StateName), Is.False);
            Assert.That(builder.StateId, Is.Not.Zero);
        }
    }

    [Test]
    public void BuildState_WrongStatenameInConfig_ThrowsException()
    {
        // Arrange 
        var builder = new DeviceOnlineStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState);

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
        var builder = new DeviceOnlineStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState);

        // Act  
        var state = builder.BuildState(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));
            Assert.That(state.Id, Is.EqualTo(DefaultStateIds.DeviceOnlineState));
            Assert.That(state.InitialBusinessSubState, Is.EqualTo(DefaultBusinessSubStates.TryToConnect));
            Assert.That(state.InitialDeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOnline));
        }
    }

    [Test]
    public void SetInitalStates_ValidSetup_StatesSet()
    {
        // Arrange 
        var builder = new DeviceOnlineStateBuilder();

        var device = TestDataHelper.CreateStateMachineDevice();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState)
        {
            CurrentContext = device
        };

        var state = builder.BuildState(config);

        // Act  
        Assert.DoesNotThrow(() =>
        {
            state.SetInitalStates();
        });

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.DeviceState, Is.EqualTo(state.InitialDeviceState));
            Assert.That(device.BusinessSubState, Is.EqualTo(state.InitialBusinessSubState));
        }
    }


    [Test]
    public void InitiateState_ValidSetup_DoesNotThrow()
    {
        // Arrange 
        var builder = new DeviceOnlineStateBuilder();

        var device = TestDataHelper.CreateStateMachineDevice();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState)
        {
            CurrentContext = device
        };

        var state = builder.BuildState(config);

        // Act  
        Assert.DoesNotThrow(() =>
        {
            state.InitiateState();
        });

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));
            Assert.That(state.Id, Is.EqualTo(DefaultStateIds.DeviceOnlineState));
        }
    }
}