// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Sfxp;

[TestFixture]
internal class SfxpDataBlockCodecTests
{
    [Test]
    public void LoadStreamingConfig_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var codec = new SfxpDataBlockCodec();
        var mask = new byte[] { 0x0, 0x1, 0x2, 0x3, 0xC, 0xF };
        // Act  
        codec.LoadStreamingConfig(mask);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(codec.StreamingConfig.Length, Is.EqualTo(mask.Length - 1));
        }
    }

    [Test]
    public void LoadStreamingConfig_ValidSetupRealWorld_PropsSetCorrectly()
    {
        // Arrange 
        var codec = new SfxpDataBlockCodec();

        var mask = new byte[256];
        mask[254] = 0xc;
        mask[255] = 0xf;

        // Act  
        codec.LoadStreamingConfig(mask);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(codec.StreamingConfig.Length, Is.EqualTo(mask.Length - 1));
        }
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var codec = new SfxpDataBlockCodec();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(codec.StreamingConfig, Is.Not.Null);
            Assert.That(codec.StreamingConfig.Length, Is.Zero);
        }
    }

    [TestCase("0", TestName = "DecodeDataMessage_Sfx0_MessageDecoded")]
    [TestCase("1", TestName = "DecodeDataMessage_Sfx1_MessageDecoded")]
    [TestCase("2", TestName = "DecodeDataMessage_Sfx2_MessageDecoded")]
    [TestCase("3", TestName = "DecodeDataMessage_Sfx3_MessageDecoded")]
    [TestCase("4", TestName = "DecodeDataMessage_Sfx4_MessageDecoded")]
    [TestCase("5", TestName = "DecodeDataMessage_Sfx5_MessageDecoded")]
    [TestCase("6", TestName = "DecodeDataMessage_Sfx6_MessageDecoded")]
    [TestCase("7", TestName = "DecodeDataMessage_Sfx7_MessageDecoded")]
    [TestCase("8", TestName = "DecodeDataMessage_Sfx8_MessageDecoded")]
    [TestCase("9", TestName = "DecodeDataMessage_Sfx9_MessageDecoded")]
    public void DecodeDataMessage_Sfx_MessageDecoded(string number)
    {
        // Arrange 
        var data = new List<byte> { 0x73 };

        var msg = ResourceHelper.GetByteResource($"Bodoconsult.NetworkCommunication.Tests.Resources.sfx{number}.bin");
        data.AddRange(msg.AsMemory(7).Span);

        var codec = new SfxpDataBlockCodec();
        codec.LoadStreamingConfig([0x0, 0x1, 0x2, 0x3, 0xC, 0xF]);

        // Act  
        var result = codec.DecodeDataBlock(data.ToArray());

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);

            var db = (SfxpInboundDatablock)result;

            Assert.That(db, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(db);

            if (number == "0")
            {
                Assert.That(db.Data.Span[0], Is.EqualTo(DeviceCommunicationBasics.Null));
            }

            Assert.That(db.Data.Length, Is.EqualTo(data.Count - 1));

            Assert.That(db.DataChunks.Count, Is.Not.Zero);
            Assert.That(db.DataChunks.Any(x => x.Channel==0), Is.True);
            Assert.That(db.DataChunks.Any(x => x.Channel == 0xc), Is.True);
            Assert.That(db.DataChunks.Any(x => x.Channel ==  255), Is.False);

            // Return chunks to pool
            foreach (var chunk in db.DataChunks)
            {
                chunk.ReturnDataChunkDelegate?.Invoke(chunk);
            }
            db.DataChunks.Clear();
        }
    }

    [TestCase("0", TestName = "DecodeDataMessage_RealConfig_Sfx0_MessageDecoded")]
    [TestCase("1", TestName = "DecodeDataMessage_RealConfig_Sfx1_essageDecoded")]
    [TestCase("2", TestName = "DecodeDataMessage_RealConfig_Sfx2_MessageDecoded")]
    [TestCase("3", TestName = "DecodeDataMessage_RealConfig_Sfx3_MessageDecoded")]
    [TestCase("4", TestName = "DecodeDataMessage_RealConfig_Sfx4_MessageDecoded")]
    [TestCase("5", TestName = "DecodeDataMessage_RealConfig_Sfx5_MessageDecoded")]
    [TestCase("6", TestName = "DecodeDataMessage_RealConfig_Sfx6_MessageDecoded")]
    [TestCase("7", TestName = "DecodeDataMessage_RealConfig_Sfx7_MessageDecoded")]
    [TestCase("8", TestName = "DecodeDataMessage_RealConfig_Sfx8_MessageDecoded")]
    [TestCase("9", TestName = "DecodeDataMessage_RealConfig_Sfx9_MessageDecoded")]
    public void DecodeDataMessage_RealConfig_Sfx_MessageDecoded(string number)
    {
        // Arrange 
        var data = new List<byte> { 0x73 };

        var config = new byte[256];
        config[254] = 0xc;
        config[255] = 0xf;
        

        var msg = ResourceHelper.GetByteResource($"Bodoconsult.NetworkCommunication.Tests.Resources.sfx{number}.bin");
        data.AddRange(msg.AsMemory(7).Span);

        var codec = new SfxpDataBlockCodec();
        codec.LoadStreamingConfig(config);

        // Act  
        var result = codec.DecodeDataBlock(data.ToArray());

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);

            var db = (SfxpInboundDatablock)result;

            Assert.That(db, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(db);

            if (number == "0")
            {
                Assert.That(db.Data.Span[0], Is.EqualTo(DeviceCommunicationBasics.Null));
            }

            Assert.That(db.Data.Length, Is.EqualTo(data.Count - 1));

            Assert.That(db.DataChunks.Count, Is.Not.Zero);
            Assert.That(db.DataChunks.Any(x => x.Channel ==0 ), Is.True);
            Assert.That(db.DataChunks.Any(x => x.Channel == 0xc), Is.True);
            Assert.That(db.DataChunks.Any(x => x.Channel == 255), Is.False);


            // Return chunks to pool
            foreach (var chunk in db.DataChunks)
            {
                chunk.ReturnDataChunkDelegate?.Invoke(chunk);
            }
            db.DataChunks.Clear();
        }
    }

    [TestCase(0,new byte[]{}, TestName = "DecodeDataMessage_RealConfigRealData_Sfx0_MessageDecoded")]
    [TestCase(1,new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx1_essageDecoded")]
    [TestCase(2,new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx2_MessageDecoded")]
    [TestCase(3,new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx3_MessageDecoded")]
    [TestCase(4,new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx4_MessageDecoded")]
    [TestCase(5, new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx5_MessageDecoded")]
    [TestCase(6,new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx6_MessageDecoded")]
    [TestCase(7, new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx7_MessageDecoded")]
    [TestCase(8, new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx8_MessageDecoded")]
    [TestCase(9, new byte[] { }, TestName = "DecodeDataMessage_RealConfigRealData_Sfx9_MessageDecoded")]
    public void DecodeDataMessage_RealConfigRealData_Sfx_MessageDecoded(int counter, byte[] msg)
    {
        // Arrange 
        var data = new List<byte> { 0x73 };

        var config = new byte[254];
        config[252] = 0xc;
        config[253] = 0xf;

        data.AddRange(msg.AsMemory(7).Span);

        var codec = new SfxpDataBlockCodec();
        codec.LoadStreamingConfig(config);

        // Act  
        var result = codec.DecodeDataBlock(data.ToArray());

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);

            var db = (SfxpInboundDatablock)result;

            Assert.That(db, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(db);

            if (counter == 0)
            {
                Assert.That(db.Data.Span[0], Is.EqualTo(DeviceCommunicationBasics.Null));
            }

            Assert.That(db.Data.Length, Is.EqualTo(data.Count - 1));

            Assert.That(db.DataChunks.Count, Is.Not.Zero);
            Assert.That(db.DataChunks.Any(x => x.Channel == 0), Is.True);
            Assert.That(db.DataChunks.Any(x => x.Channel == 0xc), Is.True);
            Assert.That(db.DataChunks.Any(x => x.Channel == 255), Is.False);


            // Return chunks to pool
            foreach (var chunk in db.DataChunks)
            {
                chunk.ReturnDataChunkDelegate?.Invoke(chunk);
            }
            db.DataChunks.Clear();
        }
    }
}