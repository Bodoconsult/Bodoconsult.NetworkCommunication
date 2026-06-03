// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.OrderBuilders;

[TestFixture]
internal class TncpOrderBuilderTests : OrderBuilderTestsBase
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var builder = new TncpOrderBuilder();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.OrderTypeName, Is.EqualTo(BuiltinOrders.TncpOrder));
            Assert.That(builder.ParameterSetType, Is.EqualTo(typeof(TncpParameterSet)));
        }
    }

    [Test]
    public void CreateOrder_ValidSetup_OrderCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new TncpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.TncpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        // Act  
        var order = builder.CreateOrder(config, 1);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(order, Is.Not.Null);
            Assert.That(order.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.ParameterSet?.CurrentOrder, Is.EqualTo(order));

            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));

            var rs = (IDeviceRequestSpec)order.RequestSpecs.First();
            Assert.That(rs.RequestAnswerSteps.Count, Is.EqualTo(1));

            var ras = rs.RequestAnswerSteps.First();
            Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new TncpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        // Act  
        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(requestSpec, Is.Not.Null);
            Assert.That(requestSpec.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateNoAnswerDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new TncpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            const string name = "RequestSpec";
            var requestSpec = builder.CreateNoAnswerDeviceRequestSpec(order, name, HandleRequestAnswerOnSuccessDelegate);

            // Assert
            Assert.That(requestSpec, Is.Not.Null);
            Assert.That(requestSpec.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateNoHandshakeNoAnswerDeviceRequestSpec_ValidSetup_RequestSpecCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new TncpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        // Act  
        using (Assert.EnterMultipleScope())
        {
            const string name = "RequestSpec";
            var requestSpec = builder.CreateNoHandshakeNoAnswerDeviceRequestSpec(order, name, HandleRequestAnswerOnSuccessDelegate);

            // Assert
            Assert.That(requestSpec, Is.Not.Null);
            Assert.That(requestSpec.ParameterSet, Is.EqualTo(ps));
            Assert.That(order.RequestSpecs.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateDeviceRequestAnswerStep_ValidSetup_RequestAnswerStepCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new TncpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        // Act  
        var rasName = "AnswerStep1";
        var ras = builder.CreateDeviceRequestAnswerStep(requestSpec, rasName);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ras, Is.Not.Null);
            Assert.That(ras.DeviceRequestSpec, Is.EqualTo(requestSpec));
            Assert.That(requestSpec.RequestAnswerSteps.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CreateRequestAnswerStep_ValidSetup_RequestAnswerStepCreated()
    {
        // Arrange 
        var ps = new TncpParameterSet();
        var builder = new TncpOrderBuilder();

        var order = new OmOrder(1, "Test", ps);

        Assert.That(order.RequestSpecs.Count, Is.Zero);

        var name = "RequestSpec";
        var requestSpec = builder.CreateDeviceRequestSpec(order, name);

        var rasName = "AnswerStep1";
        var ras = builder.CreateDeviceRequestAnswerStep(requestSpec, rasName);

        Assert.That(ras.AllowedRequestAnswers.Count, Is.Zero);

        // Act  
        var raName = "Test";
        var ra = builder.CreateRequestAnswer(ras, raName, CheckReceivedMessageDelegate, HandleRequestAnswerOnSuccessDelegate);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ra.Name, Is.EqualTo(raName));
            Assert.That(ra.CheckReceivedMessageDelegate, Is.Not.Null);
            Assert.That(ras.AllowedRequestAnswers.Count, Is.EqualTo(1));
        }
    }

    [Test]
    public void CheckReceivedMessageDelegate_ValidSetupNoEnd_ReturnsTrue()
    {
        // Arrange 

        IRequestAnswer requestAnswer = new RequestAnswer(true, null, "Test", BtcpOrderBuilder.CheckReceivedMessageDelegate);
        var sentMessage = new TncpOutboundDataMessage
        {
            TelnetCommand = "Blubb"
        };

        var replyMessage = new TncpInboundDataMessage
        {
            TelnetCommand = "<BEGIN>Blubb"
        };

        var errors = new List<string>();

        // Act  
        var result = TncpOrderBuilder.CheckReceivedMessageDelegate(requestAnswer, sentMessage, replyMessage, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(errors.Count, Is.Zero);
        }
    }

    [Test]
    public void CheckReceivedMessageDelegate_ValidSetup_ReturnsTrue()
    {
        // Arrange 

        IRequestAnswer requestAnswer = new RequestAnswer(true, null, "Test", BtcpOrderBuilder.CheckReceivedMessageDelegate);
        var sentMessage = new TncpOutboundDataMessage
        {
            TelnetCommand = "Blubb"
        };

        var replyMessage = new TncpInboundDataMessage
        {
            TelnetCommand = "<BEGIN>Blubb"
        };

        var errors = new List<string>();

        // Act  
        var result = TncpOrderBuilder.CheckReceivedMessageDelegate(requestAnswer, sentMessage, replyMessage, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(errors.Count, Is.Zero);
        }
    }

    [Test]
    public void CheckReceivedMessageDelegate_ValidCommandContainsSpaces_ReturnsTrue()
    {
        // Arrange 

        IRequestAnswer requestAnswer = new RequestAnswer(true, null, "Test", BtcpOrderBuilder.CheckReceivedMessageDelegate);
        var sentMessage = new TncpOutboundDataMessage
        {
            TelnetCommand = "Blubb"
        };

        var replyMessage = new TncpInboundDataMessage
        {
            TelnetCommand = "<BEGIN> Blubb"
        };

        var errors = new List<string>();

        // Act  
        var result = TncpOrderBuilder.CheckReceivedMessageDelegate(requestAnswer, sentMessage, replyMessage, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(errors.Count, Is.Zero);
        }
    }

    [Test]
    public void CheckReceivedMessageDelegate_ValidCommandWithError_ReturnsFalse()
    {
        // Arrange 

        IRequestAnswer requestAnswer = new RequestAnswer(true, null, "Test", BtcpOrderBuilder.CheckReceivedMessageDelegate);
        var sentMessage = new TncpOutboundDataMessage
        {
            TelnetCommand = "Blubb"
        };

        var replyMessage = new TncpInboundDataMessage
        {
            TelnetCommand = "<BEGIN>Blubb",
            TelnetAdditionalInfo = "<ERROR>Invalid command"
        };

        var errors = new List<string>();

        // Act  
        var result = TncpOrderBuilder.CheckReceivedMessageDelegate(requestAnswer, sentMessage, replyMessage, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
            Assert.That(errors.Count, Is.Not.Zero);
        }
    }

    [Test]
    public void CheckReceivedMessageDelegate_ValidCommandWithConfig_ReturnsTrue()
    {
        // Arrange 

        IRequestAnswer requestAnswer = new RequestAnswer(true, null, "Test", BtcpOrderBuilder.CheckReceivedMessageDelegate);
        var sentMessage = new TncpOutboundDataMessage
        {
            TelnetCommand = "Blubb"
        };

        var replyMessage = new TncpInboundDataMessage
        {
            TelnetCommand = "<BEGIN>Blubb",
            TelnetAdditionalInfo = "<CONFIG>Invalid command"
        };

        var errors = new List<string>();

        // Act  
        var result = TncpOrderBuilder.CheckReceivedMessageDelegate(requestAnswer, sentMessage, replyMessage, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(errors.Count, Is.Zero);
        }
    }

    [Test]
    public void CheckReceivedMessageDelegate_ValidCommandRealWorld_ReturnsTrue()
    {
        // Arrange 

        IRequestAnswer requestAnswer = new RequestAnswer(true, null, "Test", BtcpOrderBuilder.CheckReceivedMessageDelegate);

        var outData = new byte[]
        {
            0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6f, 0x72, 0x64, 0x65, 0x72, 0x2c, 0x33,
            0xa
        };

        var dataBlockCodingProcessor = new DefaultDataBlockCodingProcessor();
        dataBlockCodingProcessor.LoadDataBlockCodecs('x', new BasicDataBlockCodec());

        var codec = new TncpDataMessageCodec(dataBlockCodingProcessor);

        var sentMessage = new TncpOutboundDataMessage
        {
            TelnetCommand = "set,stream,order,3\n"
        };



        var codecResult = codec.DecodeDataMessage(outData);
        Assert.That(codecResult.ErrorCode, Is.Zero);

        var inData = new byte[]
            {
                0x3c, 0x42, 0x45, 0x47, 0x49, 0x4e, 0x3e, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c,
                0x6f, 0x72, 0x64, 0x65, 0x72, 0x2c, 0x33, 0xa, 0x3c, 0x45, 0x4e, 0x44, 0x3e, 0xa
        };

        var replyMessage1 = codec.DecodeDataMessage(inData);

        var errors = new List<string>();

        if (replyMessage1.DataMessage is not IInboundDataMessage replyMessage)
        {
            throw new ArgumentException("replyMessage1.DataMessage is not IInboundDataMessage");
        }

        // Act  
        var result = TncpOrderBuilder.CheckReceivedMessageDelegate(requestAnswer, sentMessage, replyMessage, errors);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(errors.Count, Is.Zero);
        }
    }
}