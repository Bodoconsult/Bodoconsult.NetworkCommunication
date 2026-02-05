// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;

namespace Bodoconsult.NetworkCommunication.Tests.Sdcp;

[TestFixture]
internal class DefaultDataBlockCodingProcessorTests
{
    [Test]
    public void Ctor_ValidSetup_PropSetCorrectly()
    {
        // Arrange 

        // Act  
        var dbcp = new DefaultDataBlockCodingProcessor();

        // Assert
        var dbc = dbcp.GetDatablockCodecCanBeNull('x');

        Assert.That(dbc, Is.Null);
    }

    [Test]
    public void LoadDataBlockCodecs_ValidSetup_CodecLoaded()
    {
        // Arrange 
        var dbcp = new DefaultDataBlockCodingProcessor();

        // Act  
        dbcp.LoadDataBlockCodecs('x', new SdcpDummyDataBlockCodec());

        // Assert
        var dbc = dbcp.GetDatablockCodecCanBeNull('x');

        Assert.That(dbc, Is.Not.Null);
    }

    [Test]
    public void FromBytesToDataBlock_ValidSetup_CodecLoaded()
    {
        // Arrange 
        var dbcp = new DefaultDataBlockCodingProcessor();
        dbcp.LoadDataBlockCodecs('x', new SdcpDummyDataBlockCodec());

        var data = new byte[] { 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62 }.AsMemory();

        // Act  
        var db = dbcp.FromBytesToDataBlock(data);

        // Assert
        Assert.That(db, Is.Not.Null);
        Assert.That(db.DataBlockType, Is.EqualTo('x'));
        Assert.That(db.Data.IsEqualTo(data[1..]));
    }

    [Test]
    public void FromDataBlockToBytes_ValidSetup_CodecLoaded()
    {
        // Arrange 
        var dbcp = new DefaultDataBlockCodingProcessor();
        dbcp.LoadDataBlockCodecs('x', new SdcpDummyDataBlockCodec());

        var data = new byte[] { 0x78, 0x42, 0x6c, 0x75, 0x62, 0x62 }.AsMemory();
        var db = dbcp.FromBytesToDataBlock(data);

        var list = new List<byte>();
        Assert.That(list.Count, Is.EqualTo(0));

        // Act  
        dbcp.FromDataBlockToBytes(list, db);

        // Assert
        Assert.That(list.Count, Is.EqualTo(data.Length));
    }
}