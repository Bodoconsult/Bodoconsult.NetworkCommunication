// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.App.Abstractions.NetworkCommands;

[TestFixture]
internal class NetworkCommandTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        const string cmd = "log";

        // Act  
        var networkCommand = new NetworkCommand(cmd);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(networkCommand.Command, Is.EqualTo(cmd));
            Assert.That(networkCommand.Parameters, Is.Not.Null);
        }
    }
}