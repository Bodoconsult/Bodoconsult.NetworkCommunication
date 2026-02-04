// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class EdcpDataMessageSplitterTests
{
    [Test]
    public void TryReadCommand_2ValidCommands_ReturnsCommand()
    {
        // Arrange 
        var splitter = new EdcpDataMessageSplitter();
        var msg = new byte[] { 0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3, 0x2, 0x42, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        var expectedResult = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3]);

        var buffer = new ReadOnlySequence<byte>(msg);

        //var txt = "Blubb";

        //var a = Encoding.UTF8.GetBytes(txt);

        //Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(a));

        // Act  
        var result = splitter.TryReadCommand(ref buffer, out var command);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(command.Length, Is.Not.EqualTo(0));

        Assert.That(command.IsEqualTo(expectedResult));
    }

    [Test]
    public void TryReadCommand_1validCommand_ReturnsCommand()
    {
        // Arrange 
        var splitter = new EdcpDataMessageSplitter();
        var msg = new byte[] { 0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x2, 0x42, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        var expectedResult = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x2, 0x42, 0x6b, 0x75, 0x62, 0x62, 0x3]);

        var buffer = new ReadOnlySequence<byte>(msg);

        // Act  
        var result = splitter.TryReadCommand(ref buffer, out var command);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(command.Length, Is.Not.EqualTo(0));

        Assert.That(command.IsEqualTo(expectedResult));
    }
}