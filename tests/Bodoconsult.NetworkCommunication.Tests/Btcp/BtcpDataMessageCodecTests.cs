// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.ClientNotifications;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.ClientNotifications.Notifications;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Tests.Fakes.Converters;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;
using System.Text;
using Bodoconsult.App.BusinessTransactions;

namespace Bodoconsult.NetworkCommunication.Tests.Btcp;

[TestFixture]
internal class BtcpDataMessageCodecTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        // Act  
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Assert
        Assert.That(codec.DataBlockCodingProcessor, Is.Not.Null);
    }

    [Test]
    public void DecodeDataMessage_ValidInputWithDataBlockReply_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x0, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64,
            0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35,
            0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x4,
            // Identifier byte
            0x78,
            // Default reply
            0x32, 0x7c, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x7c, 0x42, 0x6c, 0x61, 0x62, 0x62, 0x7c,
            // Payload
            0x42, 0x6c, 0x69, 0x70, 0x70,
            0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);

            Assert.That(result.DataMessage, Is.Not.Null);
            var btcpMsg = (BtcpReplyInboundDataMessage?)result.DataMessage;

            Assert.That(btcpMsg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg);
            Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(btcpMsg.DataBlock, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg.DataBlock);
            Assert.That(btcpMsg.DataBlock.Data.Length, Is.EqualTo(19));

            Assert.That(btcpMsg.ErrorCode, Is.Not.Zero);
            Assert.That(btcpMsg.InfoMessage, Is.EqualTo("Blubb"));
            Assert.That(btcpMsg.ErrorMessage, Is.EqualTo("Blabb"));
            Assert.That(btcpMsg.Payload.Length, Is.Not.Zero);
            Assert.That(btcpMsg.RawMessageData.Length, Is.Not.Zero);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidInputWithDataBlockRequest_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x1, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64, 0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35, 0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x4, 0x78, 0x75, 0x62, 0x62, 0x3, 0x2, 0x2, 0x4, 0x6b, 0x75, 0x62, 0x62, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);

            var btcpMsg = (BtcpRequestInboundDataMessage?)result.DataMessage;

            Assert.That(btcpMsg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg);
            Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(btcpMsg.DataBlock, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg.DataBlock);
            Assert.That(btcpMsg.DataBlock.Data.Length, Is.EqualTo(11));
            Assert.That(btcpMsg.RawMessageData.Length, Is.Not.Zero);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidInputNoDataBlockWithEotReply_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x0, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64, 0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35, 0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x4, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);

            var btcpMsg = (BtcpReplyInboundDataMessage?)result.DataMessage;

            Assert.That(btcpMsg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg);
            Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(btcpMsg.DataBlock, Is.Null);
            Assert.That(btcpMsg.RawMessageData.Length, Is.Not.Zero);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidInputNoDataBlockWithEotRequest_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x1, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64, 0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35, 0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x4, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);

            var btcpMsg = (BtcpRequestInboundDataMessage?)result.DataMessage;

            Assert.That(btcpMsg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg);
            Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(btcpMsg.DataBlock, Is.Null);
        }
    }

    [Explicit]
    [Test]
    public void DummyTest()
    {
        //var uid = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e");

        //var bytes = Encoding.UTF8.GetBytes(uid.ToString());
        var bytes = Encoding.UTF8.GetBytes("Blipp");

        Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(bytes));

        Assert.Pass();
    }


    [Test]
    public void DecodeDataMessage_ValidInputNoDataBlockNoEotReply_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x0, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64, 0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35, 0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x4, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);

            var btcpMsg = (BtcpReplyInboundDataMessage?)result.DataMessage;

            Assert.That(btcpMsg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg);
            Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(btcpMsg.DataBlock, Is.Null);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidInputNoDataBlockNoEotRequest_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x1, 0x31, 0x4, 0x30, 0x66, 0x38, 0x66, 0x61, 0x64, 0x35, 0x62, 0x2d, 0x64, 0x39, 0x63, 0x62, 0x2d, 0x34, 0x36, 0x39, 0x66, 0x2d, 0x61, 0x31, 0x36, 0x35, 0x2d, 0x37, 0x30, 0x38, 0x36, 0x37, 0x37, 0x32, 0x38, 0x39, 0x35, 0x30, 0x65, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);

            var btcpMsg = (BtcpRequestInboundDataMessage?)result.DataMessage;

            Assert.That(btcpMsg, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(btcpMsg);
            Assert.That(btcpMsg.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(btcpMsg.DataBlock, Is.Null);
        }
    }

    [Test]
    public void DecodeDataMessage_InvalidInput_MessageNotDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x31 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result);
            Assert.That(result.ErrorCode, Is.Not.Zero);
            Assert.That(result.DataMessage, Is.Null);
        }
    }

    [Test]
    public void DecodeDataMessage_ValidInputRealWorld1_MessageDecoded()
    {
        // Arrange 
        var msg = new byte[] { 0x2, 0x0, 0x32, 0x30, 0x31, 0x4, 0x35, 0x65, 0x31, 0x37, 0x34, 0x65, 0x62, 0x65, 0x2d, 0x30, 0x63, 0x64, 0x30, 0x2d, 0x34, 0x36, 0x33, 0x65, 0x2d, 0x61, 0x32, 0x64, 0x31, 0x2d, 0x39, 0x34, 0x31, 0x66, 0x66, 0x39, 0x34, 0x65, 0x65, 0x66, 0x61, 0x65, 0x4, 0x30, 0x7c, 0x7c, 0x7c, 0x78, 0x30, 0x7c, 0x7c, 0x3 };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.DecodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(result);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(result.DataMessage, Is.Not.Null);
        }
    }

    //[Test]
    //public void EncodeDataMessage_ValidInput_MessageEncoded()
    //{
    //    // Arrange 
    //    var transactionId = 1;

    //    var data = new byte[] { 0x75, 0x62, 0x62, 0x6b, 0x75, 0x62, 0x62 };

    //    var dataBlock = new DummyOutboundDatablock
    //    {
    //        Data = data,
    //        DataBlockType = 'x'
    //    };

    //    var msg = new BtcpOutboundDataMessage(transactionId)
    //    {
    //        DataBlock = dataBlock
    //    };

    //    Assert.That(msg.RawMessageData.Length, Is.Zero);

    //    var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
    //    dataBlockCodingProcessor.LoadDataBlockCodecs('x', new DummyDataBlockCodec());
    //    var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

    //    // Act  
    //    var result = codec.EncodeDataMessage(msg);

    //    // Assert
    //    Assert.That(result, Is.Not.Null);
    //    Assert.That(result.ErrorCode, Is.Zero);
    //    Assert.That(msg.RawMessageData.Length, Is.Not.Zero);

    //    Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
    //    Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x31));
    //    Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(DeviceCommunicationBasics.Eot));

    //    Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
    //}

    [Test]
    public void EncodeDataMessage_ValidInputReply_MessageEncoded()
    {
        // Arrange 
        var transactionId = 1;
        var transactionUid = Guid.NewGuid();

        var data = new byte[] { 0x75, 0x62, 0x62, 0x6b, 0x75, 0x62, 0x62 };

        var dataBlock = new BasicOutboundDatablock
        {
            Data = data,
            DataBlockType = 'x'
        };

        var msg = new BtcpReplyOutboundDataMessage(transactionId, transactionUid)
        {
            DataBlock = dataBlock
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
            Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x0));
            Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(0x31));
            Assert.That(msg.RawMessageData.Span[3], Is.EqualTo(DeviceCommunicationBasics.Eot));

            Assert.That(msg.RawMessageData.Span[40], Is.EqualTo(DeviceCommunicationBasics.Eot));
            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidInputRequest_MessageEncoded()
    {
        // Arrange 
        var transactionId = 1;
        var transactionUid = Guid.NewGuid();

        var data = new byte[] { 0x75, 0x62, 0x62, 0x6b, 0x75, 0x62, 0x62 };

        var dataBlock = new BasicOutboundDatablock
        {
            Data = data,
            DataBlockType = 'x'
        };

        var msg = new BtcpRequestOutboundDataMessage(transactionId, transactionUid)
        {
            DataBlock = dataBlock
        };

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
            Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x1));
            Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(0x31));
            Assert.That(msg.RawMessageData.Span[3], Is.EqualTo(DeviceCommunicationBasics.Eot));

            Assert.That(msg.RawMessageData.Span[40], Is.EqualTo(DeviceCommunicationBasics.Eot));
            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidInputNoDataBlockReply_MessageEncoded()
    {
        // Arrange 
        var transactionId = 1;
        var transactionUid = Guid.NewGuid();

        var msg = new BtcpReplyOutboundDataMessage(transactionId, transactionUid);

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
            Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x0));
            Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(0x31));
            Assert.That(msg.RawMessageData.Span[3], Is.EqualTo(DeviceCommunicationBasics.Eot));

            Assert.That(msg.RawMessageData.Span[40], Is.EqualTo(DeviceCommunicationBasics.Eot));
            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
        }
    }

    [Test]
    public void EncodeDataMessage_ValidInputNoDataBlockRequest_MessageEncoded()
    {
        // Arrange 
        var transactionId = 1;
        var transactionUid = Guid.NewGuid();

        var msg = new BtcpRequestOutboundDataMessage(transactionId, transactionUid);

        Assert.That(msg.RawMessageData.Length, Is.Zero);

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());
        var codec = new BtcpDataMessageCodec(dataBlockCodingProcessor);

        // Act  
        var result = codec.EncodeDataMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ErrorCode, Is.Zero);
            Assert.That(msg.RawMessageData.Length, Is.Not.Zero);

            Assert.That(msg.RawMessageData.Span[0], Is.EqualTo(DeviceCommunicationBasics.Stx));
            Assert.That(msg.RawMessageData.Span[1], Is.EqualTo(0x1));
            Assert.That(msg.RawMessageData.Span[2], Is.EqualTo(0x31));
            Assert.That(msg.RawMessageData.Span[3], Is.EqualTo(DeviceCommunicationBasics.Eot));

            Assert.That(msg.RawMessageData.Span[msg.RawMessageData.Length - 1], Is.EqualTo(DeviceCommunicationBasics.Etx));
        }
    }
}

internal class BtcpIpDeviceClientTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.Logger;

    [OneTimeTearDown]
    public void Cleanup()
    {
        _appLogger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();

        // Act  
        var client = new BtcpIpDeviceClient(device, _appLogger);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.Device, Is.EqualTo(device));
            Assert.That(client.AllowedNotifications, Is.Not.Null);
            Assert.That(client.AllowedNotifications.Count, Is.Not.Zero);
            Assert.That(client.AllowedNotifications.Contains(nameof(StateMachineStateNotification)), Is.True);
        }
    }

    [Test]
    public void LoadClientManager_ValidSetup_ClientManagerLoaded()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService();
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);

        // Act  
        client.LoadClientManager(clientManager);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(client.ClientManager, Is.EqualTo(clientManager));
        }
    }

    [Test]
    public void CheckNotification_ValidNotification_ReturnsTrue()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService();
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);
        client.LoadClientManager(clientManager);

        var noti = new StateMachineStateNotification
        {
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Happens normally in client manager
        noti.NotificationObjectToSend = clientMessagingService.Convert(noti);

        // Act  
        var result = client.CheckNotification(noti);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void DoNotifyClient_ValidNotification_ReturnsTrue()
    {
        // Arrange 
        var commAdapter = new FakeIpCommunicationAdapter();
        var device = TestDataHelper.CreateSimpleDevice();
        device.LoadCommAdapter(commAdapter);

        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new TestInboundBtcpMessageToBtRequestDataConverter(_appLogger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new TestInboundBtcpMessageToBtReplyConverter(_appLogger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new TestBtRequestDataToOutboundBtcpMessageConverter(_appLogger);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new TestBtReplyToOutboundDataMessageConverter(_appLogger);
        var btm = new FakeBusinessTransactionManager();

        IDeviceBusinessLogicAdapter adapter = new TestBtcpClientTcpIpBusinessLogicAdapter(device, btm, inboundDataMessageToBtRequestConverter,
            inboundDataMessageToBtReplyConverter, outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);
        device.LoadDeviceBusinessLogicAdapter(adapter);

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService();
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);
        client.LoadClientManager(clientManager);

        var noti = new StateMachineStateNotification
        {
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Happens normally in client manager
        noti.NotificationObjectToSend = clientMessagingService.Convert(noti);

        var result = client.CheckNotification(noti);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            Assert.DoesNotThrow(() =>
            {
                client.DoNotifyClient(noti);
            });

            // Assert
            Assert.That(result, Is.True);
            Assert.That(commAdapter.WasSent, Is.True);
        }
    }

    [Test]
    public void ClientManager_DoNotifyAllClients_ValidNotification_ReturnsTrue()
    {
        // Arrange 
        var commAdapter = new FakeIpCommunicationAdapter();
        var device = TestDataHelper.CreateSimpleDevice();
        device.LoadCommAdapter(commAdapter);

        IInboundMessageToBtRequestDataConverter inboundDataMessageToBtRequestConverter = new TestInboundBtcpMessageToBtRequestDataConverter(_appLogger);
        IInboundDataMessageToBtReplyConverter inboundDataMessageToBtReplyConverter = new TestInboundBtcpMessageToBtReplyConverter(_appLogger);
        IBtRequestDataToOutboundDataMessageConverter outboundBtRequestToOutboundDataMessageConverter = new TestBtRequestDataToOutboundBtcpMessageConverter(_appLogger);
        IBtReplyToOutboundDataMessageConverter outboundBtReplyDataMessageConverter = new TestBtReplyToOutboundDataMessageConverter(_appLogger);
        var btm = new FakeBusinessTransactionManager();

        IDeviceBusinessLogicAdapter adapter = new TestBtcpClientTcpIpBusinessLogicAdapter(device, btm, inboundDataMessageToBtRequestConverter,
            inboundDataMessageToBtReplyConverter, outboundBtRequestToOutboundDataMessageConverter, outboundBtReplyDataMessageConverter);
        device.LoadDeviceBusinessLogicAdapter(adapter);

        IClientNotificationLicenseManager licenseManager = new DummyClientNotificationLicenseManager();
        IClientMessagingService clientMessagingService = new BasicBtcpNetworkingClientMessagingService();
        IClientManager clientManager = new ClientManager(licenseManager, _appLogger, clientMessagingService);

        var client = new BtcpIpDeviceClient(device, _appLogger);

        clientManager.AddClient(client);
        client.LoadClientManager(clientManager);

        var noti = new StateMachineStateNotification
        {
            DeviceStateId = 1,
            DeviceStateName = "Blubb",
            BusinessStateId = 2,
            BusinessStateName = "Blabb",
            BusinessSubstateId = 3,
            BusinessSubstateName = "Blobb"
        };

        // Happens normally in client manager
        noti.NotificationObjectToSend = clientMessagingService.Convert(noti);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            Assert.DoesNotThrow(() =>
            {
                clientManager.DoNotifyAllClients(noti);
            });

            // Assert
            Wait.Until(() => commAdapter.WasSent);
            Assert.That(commAdapter.WasSent, Is.True);
        }
    }
}