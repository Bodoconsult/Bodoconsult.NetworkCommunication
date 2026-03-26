// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

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

        var config = new JobStateConfiguration(DefaultStateNames.DeviceOnlineState, builder);

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

        var ps = new TncpParameterSet();
        var ps2 = new TncpParameterSet();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, builder)
        {
            CurrentContext = TestDataHelper.CreateStateMachineDevice(),
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
        };

        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");
        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");

        config.ParameterSets.Add(ps);
        config.ParameterSets.Add(ps2);

        // Act  
        var state = builder.BuildState(config);

        var os = (IOrderBasedActionStateMachineState)state;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));

            Assert.That(os.Orders.Count, Is.Zero);
        }
    }

    [Test]
    public void InitiateState_ValidSetup_StateBuilded()
    {
        // Arrange 
        var builder = new DeviceStartStreamingStateBuilder();

        var ps = new TncpParameterSet();
        var ps2 = new TncpParameterSet();

        var config = new JobStateConfiguration(DefaultStateNames.DeviceStartStreamingState, builder)
        {
            CurrentContext = TestDataHelper.CreateStateMachineDevice(),
            OrderFinishedSucessfullyDelegate = DelegateHelper.OrderFinishedSucessfullyDelegate,
            OrderFinishedUnsucessfullyDelegate = DelegateHelper.OrderFinishedUnsucessfullyDelegate,
        };

        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");
        config.OrderConfigurations.Add($"{BuiltinOrders.TncpOrder}Configuration");

        config.ParameterSets.Add(ps);
        config.ParameterSets.Add(ps2);

        var state = builder.BuildState(config);

        var os = (IOrderBasedActionStateMachineState)state;

        // Act  
        state.InitiateState();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(state.Id, Is.EqualTo(builder.StateId));

            Assert.That(os.Orders.Count, Is.EqualTo(os.OrderConfigurations.Count));
        }
    }
}