// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Builders.BasicTests;

[TestFixture]
internal class BaseJobStateMachineStateBuilderTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new DeviceStartStreamingStateBuilder();

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
        var builder = new DeviceStartStreamingStateBuilder();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceOnlineState);

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
        var builder = new DeviceStartStreamingStateBuilder();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState)
        {
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
            PrepareOrdersForStateMachineStateDelegate = DelegateHelper.PrepareOrdersForStateMachineStateDelegate
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