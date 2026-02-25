// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class DummyDataBlockCodecTests
{
    [Test]
    public void DecodeDataBlock_ValidData_ReturnsDataBlock()
    {
        // Arrange 
        var codec = new DummyDataBlockCodec();
        var data = new byte[] { 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62 }.AsMemory();

        // Act  
        var db = codec.DecodeDataBlock(data);

        // Assert
        Assert.That(db, Is.Not.Null);
        Assert.That(db.DataBlockType, Is.EqualTo('x'));
        Assert.That(db.Data.IsEqualTo(data[1..]));
    }

    [Test]
    public void EncodeDataBlock_ValidData_ReturnsDataBlock()
    {
        // Arrange 
        var codec = new DummyDataBlockCodec();
        var data = new byte[] { 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62 }.AsMemory();

        var dataBlock = new DummyOutboundDatablock
        {
            Data = data,
            DataBlockType = 'x'
        };

        var list = new List<byte>();
        Assert.That(list.Count, Is.EqualTo(0));

        // Act  
        codec.EncodeDataBlock(list, dataBlock);

        // Assert
        Assert.That(list.Count, Is.EqualTo(data.Length + 1));
    }
}