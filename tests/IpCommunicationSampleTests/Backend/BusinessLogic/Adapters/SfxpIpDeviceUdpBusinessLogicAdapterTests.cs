// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpBackend.Bll.BusinessLogic.Adapters;
using IpBackend.Bll.ClientNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IpCommunicationSampleTests.Backend.BusinessLogic.Adapters
{
    [TestFixture]
    internal class SfxpIpDeviceUdpBusinessLogicAdapterTests
    {
        [Test]
        public void Ctor_ValidSetup_PropsSetCorrectly()
        {
            // Arrange 
            var chunks = new List<DataChunk>
            {
                new()
                {
                    Channel = 0x1,
                    Data = new byte[] { 0xa, 0xb, 0xc, 0xa, 0xb, 0xc, 0xa, 0xb }
                },
                new()
                {
                    Channel = 0x2,
                    Data = new byte[] {  0xb, 0xc, 0xa, 0xb, 0xc, 0xa, 0xb, 0xc }
                },
                new()
                {
                    Channel = 0x3,
                    Data = new byte[] { 0xc, 0xa, 0xb, 0xc, 0xa, 0xb, 0xc, 0xa }
                }
            };

            // Act  
            var result = SfxpIpDeviceUdpBusinessLogicAdapter.GetArray(chunks);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Length, Is.EqualTo(chunks.Count * 9));

                var span = result.AsSpan();

                Assert.That(span[0], Is.EqualTo(0x1));
                Assert.That(span[9], Is.EqualTo(0x2));
                Assert.That(span[18], Is.EqualTo(0x3));

                Assert.That(span[1], Is.EqualTo(0xa));
                Assert.That(span[10], Is.EqualTo(0xb));
                Assert.That(span[19], Is.EqualTo(0xc));
            }
        }
    }
}
