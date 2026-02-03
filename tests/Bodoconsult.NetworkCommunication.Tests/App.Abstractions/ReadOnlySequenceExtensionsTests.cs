// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.App.Abstractions;

namespace Bodoconsult.NetworkCommunication.Tests.App.Abstractions;

[TestFixture]
internal class ReadOnlySequenceExtensionsTests
{
    [Test]
    public void IsEqual_EqualValues_ReturnsTrue()
    {
        // Arrange 
        var r1 = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3]);
        var r2 = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3]);

        // Act  
        var result = r1.IsEqualTo(r2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEqual_ValuesWithDifferentLength_ReturnsFalse()
    {
        // Arrange 
        var r1 = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62]);
        var r2 = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3]);

        // Act  
        var result = r1.IsEqualTo(r2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsEqual_ValuesWithDifferentContent_ReturnsFalse()
    {
        // Arrange 
        var r1 = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62]);
        var r2 = new ReadOnlySequence<byte>([0x2, 0x42, 0x6b, 0x75, 0x62, 0x62, 0x3]);

        // Act  
        var result = r1.IsEqualTo(r2);

        // Assert
        Assert.That(result, Is.False);
    }
}