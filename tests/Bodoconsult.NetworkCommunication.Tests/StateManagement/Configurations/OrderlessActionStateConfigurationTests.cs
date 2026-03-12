// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Configurations;

[TestFixture]
internal class OrderlessActionStateConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const string stateName = "Blubb";

        var builder = new DeviceOfflineStateBuilder();

        // Act  
        var config = new OrderlessActionStateConfiguration(stateName, builder)
        {
            CurrentContext = TestDataHelper.CreateStateMachineDevice(),
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.StateName, Is.EqualTo(stateName));
            Assert.That(config.CurrentContext, Is.Not.Null);
            Assert.That(config.ExecuteActionForStateDelegate, Is.Null);
            Assert.That(config.StateBuilderBuilder, Is.Not.Null);
        }
    }
}