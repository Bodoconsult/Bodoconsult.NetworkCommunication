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
    public void GetBuffer_ValidSetup_ReturnsBuffer()
    {
        // Arrange 
        var pipe = new DatagramPipeline();

        // Act  
        var buffer = pipe.GetBuffer();

        // Assert
        Assert.That(buffer.Memory.Length, Is.GreaterThanOrEqualTo(pipe.BufferSize));

        buffer.Dispose();
    }

    [Test]
    public void ReleaseBuffer_ValidSetup_DoesNotThrow()
    {
        // Arrange 
        var pipe = new DatagramPipeline();

        var buffer = pipe.GetBuffer();

        // Act and assert 
        Assert.DoesNotThrow(() =>
        {
            pipe.ReleaseBuffer(buffer);
        });
    }

    [Test]
    public void AddMemory_OneDatablock_DoesNotThrow()
    {
        // Arrange 
        var pipe = new DatagramPipeline();
        pipe.StartReceiverLoop(SocketReceivedData);

        var buffer = pipe.GetBuffer();

        var data = new byte[] { 0x0, 0x1 };

        for (var i = 0; i < data.Length; i++)
        {
            buffer.Memory.Span[i] = data[i];
        }

        // Act and assert 
        Assert.DoesNotThrow(() =>
        {
            pipe.AddMemory(buffer, data.Length);
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

[TestFixture]
internal class StreamPipelineTests
{
    [Test]
    public void Ctor_ValidSetup_PropSetCorrectly()
    {
        // Arrange 

        // Act  
        var pipe = new StreamPipeline();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pipe.Buffer.Length, Is.Zero);
            Assert.That(pipe.BufferSize, Is.Not.Zero);
        }
    }

    [Test]
    public void GetBuffer_ValidSetup_ReturnsBuffer()
    {
        // Arrange 
        var pipe = new StreamPipeline();

        // Act  
        var buffer = pipe.GetBuffer();

        // Assert
        Assert.That(buffer.Memory.Length, Is.GreaterThanOrEqualTo(pipe.BufferSize));

        buffer.Dispose();
    }

    [Test]
    public void ReleaseBuffer_ValidSetup_DoesNotThrow()
    {
        // Arrange 
        var pipe = new StreamPipeline();

        var buffer = pipe.GetBuffer();


        // Act and assert 
        Assert.DoesNotThrow(() =>
        {
            pipe.ReleaseBuffer(buffer);
        });
    }

    [Test]
    public void AddMemory_OneDataBlock_DataAdded()
    {
        // Arrange 
        var pipe = new StreamPipeline();

        var buffer = pipe.GetBuffer();

        var data = new byte[] { 0x0, 0x1 };

        for (var i = 0; i < data.Length; i++)
        {
            buffer.Memory.Span[i] = data[i];
        }

        // Act  
        Assert.DoesNotThrow(() =>
        {
            pipe.AddMemory(buffer, data.Length);
        });

        // Assert
        Assert.That(pipe.Buffer.Length, Is.EqualTo(data.Length));
    }

    [Test]
    public void AddMemory_TwoDataBlocks_DataAdded()
    {
        // Arrange 
        var pipe = new StreamPipeline();
        const int numberOfBlocks = 3;
        var data = new byte[] { 0x0, 0x1 };

        for (var j = 0; j < numberOfBlocks; j++)
        {
            var buffer = pipe.GetBuffer();

            for (var i = 0; i < data.Length; i++)
            {
                buffer.Memory.Span[i] = data[i];
            }

            // Act  
            Assert.DoesNotThrow(() =>
            {
                pipe.AddMemory(buffer, data.Length);
            });
        }

        // Assert
        Assert.That(pipe.Buffer.Length, Is.EqualTo(numberOfBlocks * data.Length));
    }

    [Test]
    public void UseBuffer_ValidBuffer_DataAdded()
    {
        // Arrange 
        var pipe = new StreamPipeline();
        const int numberOfBlocks = 3;
        var data = new byte[] { 0x0, 0x1 };

        for (var j = 0; j < numberOfBlocks; j++)
        {
            var buffer = pipe.GetBuffer();

            for (var i = 0; i < data.Length; i++)
            {
                buffer.Memory.Span[i] = data[i];
            }


            Assert.DoesNotThrow(() =>
            {
                pipe.AddMemory(buffer, data.Length);
            });
        }

        Assert.That(pipe.Buffer.Length, Is.EqualTo(numberOfBlocks * data.Length));

        // Act
        pipe.Buffer = pipe.Buffer.Slice(data.Length);

        // Assert
        Assert.That(pipe.Buffer.Length, Is.EqualTo((numberOfBlocks - 1) * data.Length));
    }

    [Test]
    public void UseBuffer_ValidBuffer2_DataAdded()
    {
        // Arrange 
        var pipe = new StreamPipeline();
        const int numberOfBlocks = 3;
        var data = new byte[] { 0x0, 0x1 };

        for (var j = 0; j < numberOfBlocks; j++)
        {
            var buffer = pipe.GetBuffer();

            for (var i = 0; i < data.Length; i++)
            {
                buffer.Memory.Span[i] = data[i];
            }


            Assert.DoesNotThrow(() =>
            {
                pipe.AddMemory(buffer, data.Length);
            });
        }

        Assert.That(pipe.Buffer.Length, Is.EqualTo(numberOfBlocks * data.Length));

        // Act
        pipe.Buffer = pipe.Buffer.Slice(pipe.Buffer.Length);

        // Assert
        Assert.That(pipe.Buffer.Length, Is.Zero);
    }

    [Test]
    public void UseBuffer_ValidBuffer3_DataAdded()
    {
        // Arrange 
        var pipe = new StreamPipeline();
        const int numberOfBlocks = 3;
        var data = new byte[] { 0x0, 0x1 };

        for (var j = 0; j < numberOfBlocks; j++)
        {
            var buffer = pipe.GetBuffer();

            for (var i = 0; i < data.Length; i++)
            {
                buffer.Memory.Span[i] = data[i];
            }


            Assert.DoesNotThrow(() =>
            {
                pipe.AddMemory(buffer, data.Length);
            });
        }

        Assert.That(pipe.Buffer.Length, Is.EqualTo(numberOfBlocks * data.Length));

        // Act
        var buffer1 = pipe.Buffer;

        buffer1 = buffer1.Slice(pipe.Buffer.Length);

        pipe.MoveForward(buffer1.Length);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pipe.Buffer.Length, Is.Zero);
            Assert.That(buffer1.Length, Is.Zero);
        }
    }
}