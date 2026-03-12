// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Builders.BasicTests;

[TestFixture]
internal class BaseOrderlessStateMachineStateBuilderTests
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
        var builder = new DeviceOfflineStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOfflineState, builder)
        {
            CurrentContext = TestDataHelper.CreateStateMachineDevice(),
        };

        // Act  
        var state = builder.BuildState(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));
        }
    }
}