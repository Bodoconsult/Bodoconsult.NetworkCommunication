// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions.NetworkCommands;

namespace Bodoconsult.NetworkCommunication.Tests.App.Abstractions.NetworkCommands;

[TestFixture]
internal class TncpCommandBuilderTests
{
    [Test]
    public void BuildIt_CommandWithoutParameters_ReturnsValidString()
    {
        // Arrange 
        const string cmd = "log";

        var builder = new TncpCommandBuilder();

        // Act  
        var result = builder.BuildIt(cmd, null);

        // Assert
        Assert.That(result, Is.EqualTo(cmd));
    }

    [Test]
    public void BuildIt_CommandWithOneParameter_ReturnsValidString()
    {
        // Arrange 
        const string cmd = "log";

        var param1 = new NetworkCommandParameter { Name = "Parameter0", Value = "Value1" };

        var parameters = new List<NetworkCommandParameter>
        {
            param1
        };

        var builder = new TncpCommandBuilder();

        // Act  
        var result = builder.BuildIt(cmd, parameters);

        // Assert
        Assert.That(result, Is.EqualTo($"{cmd},{param1.Value}"));
    }

    [Test]
    public void BuildIt_CommandWithTwoParametersLastEmpty_ReturnsValidString()
    {
        // Arrange 
        const string cmd = "log";

        var param1 = new NetworkCommandParameter { Name = "Parameter0", Value = "Value1" };
        var param2 = new NetworkCommandParameter { Name = "Parameter0", Value = string.Empty};

        var parameters = new List<NetworkCommandParameter>
        {
            param1,
            param2
        };

        var builder = new TncpCommandBuilder();

        // Act  
        var result = builder.BuildIt(cmd, parameters);

        // Assert
        Assert.That(result, Is.EqualTo($"{cmd},{param1.Value},"));
    }

    [Test]
    public void BuildIt_CommandWithTwoParametersFirstEmpty_ReturnsValidString()
    {
        // Arrange 
        const string cmd = "log";

        var param1 = new NetworkCommandParameter { Name = "Parameter0", Value = string.Empty };
        var param2 = new NetworkCommandParameter { Name = "Parameter0", Value = "Value1" };

        var parameters = new List<NetworkCommandParameter>
        {
            param1,
            param2
        };

        var builder = new TncpCommandBuilder();

        // Act  
        var result = builder.BuildIt(cmd, parameters);

        // Assert
        Assert.That(result, Is.EqualTo($"{cmd},,{param2.Value}"));
    }

    [Test]
    public void BuildIt_CommandWithTwoParameters_ReturnsValidString()
    {
        // Arrange 
        const string cmd = "log";

        var param1 = new NetworkCommandParameter { Name = "Parameter0", Value = "Value1" };
        var param2 = new NetworkCommandParameter { Name = "Parameter0", Value = "Value2" };

        var parameters = new List<NetworkCommandParameter>
        {
            param1,
            param2
        };

        var builder = new TncpCommandBuilder();

        // Act  
        var result = builder.BuildIt(cmd, parameters);

        // Assert
        Assert.That(result, Is.EqualTo($"{cmd},{param1.Value},{param2.Value}"));
    }
}