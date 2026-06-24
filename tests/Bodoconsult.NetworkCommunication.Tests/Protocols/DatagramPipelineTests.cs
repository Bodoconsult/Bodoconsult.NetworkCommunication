// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Protocols;

namespace Bodoconsult.NetworkCommunication.Tests.Protocols;

[TestFixture]
internal class DatagramPipelineTests
{
    private bool _isReceived;

    [SetUp]
    public void Setup()
    {
        _isReceived = false;
    }

    [Test]
    public void Ctor_ValidSetup_PropSetCorrectly()
    {
        // Arrange 

        // Act  
        var pipe = new DatagramPipeline();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pipe.BufferSize, Is.Not.Zero);
            Assert.That(pipe.SocketReceivedDataDelegate, Is.Null);
        }
    }

    [Test]
    public void AddMemory_OneDatablock_DoesNotThrow()
    {
        // Arrange 
        var pipe = new DatagramPipeline();
        pipe.StartReceiverLoop(SocketReceivedData);

        var data = new byte[] { 0x0, 0x1 };

        // Act and assert 
        Assert.DoesNotThrow(() =>
        {
            pipe.AddMemory(data);
        });

        // Assert
        Wait.Until(() => _isReceived);
        Assert.That(_isReceived, Is.True);
    }

    private void SocketReceivedData(Memory<byte> data)
    {
        _isReceived = true;
    }
}