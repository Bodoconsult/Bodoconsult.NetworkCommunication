// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Builders;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.StateManagement.Configurations;

[TestFixture]
internal class NoActionStateConfigurationTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const string stateName = "Blubb";

        var builder = new DeviceReadyStateBuilder();

        // Act  
        var config = new NoActionStateConfiguration(stateName, builder)
        {
            CurrentContext = TestDataHelper.CreateStateMachineDevice(),
        };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.StateName, Is.EqualTo(stateName));
            Assert.That(config.CurrentContext, Is.Not.Null);
            Assert.That(config.CheckJobstatesActionForStateDelegate, Is.Null);
            Assert.That(config.StateBuilderBuilder, Is.Not.Null);
        }
    }
}