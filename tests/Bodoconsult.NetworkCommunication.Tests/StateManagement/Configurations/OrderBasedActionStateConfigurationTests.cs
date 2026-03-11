// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Configurations;

[TestFixture]
internal class OrderBasedActionStateConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const string stateName = "Blubb";

        // Act  
        var config = new OrderBasedActionStateConfiguration(stateName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.StateName, Is.EqualTo(stateName));
            Assert.That(config.CurrentContext, Is.Null);
            Assert.That(config.OrderFinishedSucessfullyDelegate, Is.Null);
            Assert.That(config.OrderFinishedUnsucessfullyDelegate, Is.Null);
            Assert.That(config.PrepareOrdersForStateMachineStateDelegate, Is.Null);
            Assert.That(config.StateBuilderBuilder, Is.Null);
        }
    }
}