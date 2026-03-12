// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Builders;

[TestFixture]
internal class DeviceOfflineStateBuilderTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new DeviceOfflineStateBuilder();

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
        var builder = new DeviceOfflineStateBuilder();

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

        // Act  
        var state = builder.BuildState(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));
            Assert.That(state.Id, Is.EqualTo(DefaultStateIds.DeviceOfflineState));
            Assert.That(state.CurrentContext, Is.EqualTo(device));

            Assert.That(state.HandleAsyncMessageDelegate, Is.Not.Null);
            Assert.That(state.HandleComDevCloseDelegate, Is.Not.Null);
            Assert.That(state.HandleErrorMessageDelegate, Is.Not.Null);
            Assert.That(state.HandleRegularStateRequestAnswerDelegate, Is.Not.Null);
            Assert.That(state.PrepareRegularStateRequestDelegate, Is.Not.Null);

            Assert.That(state.InitialBusinessSubState, Is.EqualTo(DefaultBusinessSubStates.NotSet));
            Assert.That(state.InitialDeviceState, Is.EqualTo(DefaultDeviceStates.DeviceStateOffline));
            Assert.That(state.AllowedNextStates.Contains(DefaultStateNames.DeviceOnlineState), Is.True);
        }
    }
}