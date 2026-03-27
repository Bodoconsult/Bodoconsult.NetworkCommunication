// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic.Converters;

namespace IpCommunicationSampleTests.Backend.Converters
{
    [TestFixture]
    internal class ClientBtReplyToOutboundDataMessageConverterTests
    {
        private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();

        [OneTimeTearDown]
        public void Cleanup()
        {
            _appLogger.Dispose();
        }

        [Test]
        public void Ctor_ValidSetup_PropsSetCorrectly()
        {
            
            // Arrange 

            // Act  
            var converter = new ClientBtReplyToOutboundDataMessageConverter(_appLogger);

            // Assert
            Assert.That(converter.AppLogger, Is.EqualTo(_appLogger));
        }

        [Test]
        public void MapToOutboundDataMessage_ValidReplyErrorCode0_PropsSetCorrectly()
        {

            // Arrange 
            var converter = new ClientBtReplyToOutboundDataMessageConverter(_appLogger);

            var reply = new DefaultBusinessTransactionReply
            {
                RequestData = new EmptyBusinessTransactionRequestData()
                {
                    TransactionId = 100
                }
            };

            // Act  
            var msg = converter.MapToOutboundDataMessage(reply);

            // Assert
            Assert.That(msg, Is.Not.Null);
            Assert.That(msg.DataBlock, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(msg.DataBlock);

            var payload = msg.DataBlock.Data;

            Assert.That(payload.Length, Is.Not.Zero);

            var dataString = Encoding.UTF8.GetString(payload.Span);

            Assert.That(dataString, Is.EqualTo("0||"));
        }

    }
}
