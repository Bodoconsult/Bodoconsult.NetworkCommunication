// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

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
        var builder = new DeviceStartStreamingStateBuilder();

        var config = new OrderlessActionStateConfiguration(DefaultStateNames.DeviceOnlineState);

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

        var config = new OrderBasedActionStateConfiguration(DefaultStateNames.DeviceStartStreamingState)
            {
                OrderFinishedSucessfullyDelegate = OrderFinishedSucessfullyDelegate,
                OrderFinishedUnsucessfullyDelegate = OrderFinishedUnsucessfullyDelegate,
                PrepareOrdersForStateMachineStateDelegate = PrepareOrdersForStateMachineStateDelegate
            };

        // Act  
        var state = (IOrderBasedActionStateMachineState)builder.BuildState(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));
            Assert.That(state.PrepareOrdersForStateMachineStateDelegate, Is.Not.Null);
            Assert.That(state.OrderFinishedSucessfullyDelegate, Is.Not.Null);
            Assert.That(state.OrderFinishedUnsucessfullyDelegate, Is.Not.Null);
        }
    }

    private List<IOrder> PrepareOrdersForStateMachineStateDelegate()
    {
        return [];
    }

    private void OrderFinishedUnsucessfullyDelegate(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    private void OrderFinishedSucessfullyDelegate(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }
}