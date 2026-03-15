// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions.NetworkCommands;

namespace Bodoconsult.NetworkCommunication.Tests.App.Abstractions.NetworkCommands;

[TestFixture]
internal class TncpCommandParserTests
{
    [Test]
    public void Parse_ValidCommandWithOneComma_ReturnsNetworkCommand()
    {
        // Arrange 
        const string cmd = "log";
        const string param1 = "chstat";

        var parser = new TncpCommandParser();

        // Act  
        var result = parser.Parse($"{cmd},{param1}");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Command, Is.EqualTo(cmd));
            Assert.That(result.Parameters, Is.Not.Null);
            Assert.That(result.Parameters.Count, Is.EqualTo(1));
            Assert.That(result.Parameters[0].Value, Is.EqualTo(param1));
        }
    }

    [Test]
    public void Parse_ValidCommandWithTwoCommas_ReturnsNetworkCommand()
    {
        // Arrange 
        const string cmd = "log";
        const string param1 = "chstat";
        const string param2 = "blubb";

        var parser = new TncpCommandParser();

        // Act  
        var result = parser.Parse($"{cmd},{param1},{param2}");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Command, Is.EqualTo(cmd));
            Assert.That(result.Parameters, Is.Not.Null);
            Assert.That(result.Parameters.Count, Is.EqualTo(2));
            Assert.That(result.Parameters[0].Value, Is.EqualTo(param1));
            Assert.That(result.Parameters[1].Value, Is.EqualTo(param2));
        }
    }

    [Test]
    public void Parse_ValidCommandWithNoComma_ReturnsNetworkCommand()
    {
        // Arrange 
        const string cmd = "log";

        var parser = new TncpCommandParser();

        // Act  
        var result = parser.Parse(cmd);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Command, Is.EqualTo(cmd));
            Assert.That(result.Parameters, Is.Not.Null);
            Assert.That(result.Parameters.Count, Is.Zero);
        }
    }

    [Test]
    public void Parse_EmptyCommand_ThrowsException()
    {
        // Arrange 
        var cmd = string.Empty;

        var parser = new TncpCommandParser();

        // Act  
        Assert.Throws<ArgumentException>(() =>
        {
            parser.Parse(cmd);
        });
    }

    [Test]
    public void Parse_CommandWithFirstTokenEmpty_ThrowsException()
    {
        // Arrange 
        const string cmd = ",chstat";

        var parser = new TncpCommandParser();

        // Act  
        Assert.Throws<ArgumentException>(() =>
        {
            parser.Parse(cmd);
        });
    }
}