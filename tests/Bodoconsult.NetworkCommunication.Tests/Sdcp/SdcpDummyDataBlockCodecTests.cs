// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class SdcpDummyDataBlockCodecTests
{
    [Test]
    public void DecodeDataBlock_ValidData_ReturnsDataBlock()
    {
        // Arrange 
        var codec = new SdcpDummyDataBlockCodec();
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
        var codec = new SdcpDummyDataBlockCodec();
        var data = new byte[] { 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62 }.AsMemory();

        var db = codec.DecodeDataBlock(data);

        var list = new List<byte>();
        Assert.That(list.Count, Is.EqualTo(0));

        // Act  
        codec.EncodeDataBlock(list, db);

        // Assert
        Assert.That(list.Count, Is.EqualTo(data.Length));
    }
}