// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Builders;

[TestFixture]
internal class DeviceReadyStateBuilderTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new DeviceReadyStateBuilder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(string.IsNullOrEmpty(builder.StateName), Is.False);
            Assert.That(builder.StateId, Is.EqualTo(DefaultStateIds.DeviceReadyState));
            Assert.That(builder.StateName, Is.EqualTo(DefaultStateNames.DeviceReadyState));
        }
    }

    [Test]
    public void BuildState_WrongStatenameInConfig_ThrowsException()
    {
        // Arrange 
        var builder = new DeviceReadyStateBuilder();

        var config = new NoActionStateConfiguration(DefaultStateNames.DeviceOnlineState, builder);

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

        // Act  
        var state = builder.BuildState(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state.CurrentContext, Is.EqualTo(device));
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));

            Assert.That(state.HandleAsyncMessageDelegate, Is.Not.Null);
            Assert.That(state.HandleComDevCloseDelegate, Is.Not.Null);
            Assert.That(state.HandleErrorMessageDelegate, Is.Not.Null);
            Assert.That(state.HandleRegularStateRequestAnswerDelegate, Is.Not.Null);
            Assert.That(state.PrepareRegularStateRequestDelegate, Is.Not.Null);

            Assert.That(state.AllowedNextStates, Does.Contain(DefaultStateNames.DeviceOfflineState));
            Assert.That(state.AllowedNextStates, Does.Contain(DefaultStateNames.DeviceStartStreamingState));
            Assert.That(state.AllowedNextStates, Does.Contain(DefaultStateNames.DeviceStartSnapshotState));
        }
    }
}