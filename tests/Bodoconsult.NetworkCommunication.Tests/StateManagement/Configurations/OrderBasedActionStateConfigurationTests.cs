// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Configurations;

[TestFixture]
internal class OrderBasedActionStateConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const string stateName = "Blubb";

        var builder = new DeviceOfflineStateBuilder();

        // Act  
        var config = new OrderBasedActionStateConfiguration(stateName, builder)
        {
            CurrentContext = TestDataHelper.CreateStateMachineDevice(), 
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.StateName, Is.EqualTo(stateName));
            Assert.That(config.CurrentContext, Is.Not.Null);
            Assert.That(config.OrderFinishedSucessfullyDelegate, Is.Null);
            Assert.That(config.OrderFinishedUnsucessfullyDelegate, Is.Null);
            Assert.That(config.PrepareOrdersForStateMachineStateDelegate, Is.Null);
            Assert.That(config.StateBuilderBuilder, Is.Not.Null);
        }
    }
}