// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Helpers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.App.Abstractions.Extensions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;

namespace Bodoconsult.NetworkCommunication.Tests.Sfxp
{
    [TestFixture]
    internal class PacketwiseDataMessageSplitterTests
    {
        [Test]
        public void TryReadCommand_OneCommand_ReturnsCommand()
        {
            // Arrange 
            var expectedResult = new ReadOnlySequence<byte>([0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3]);

            var msg = new byte[] { 0x2, 0x42, 0x6c, 0x75, 0x62, 0x62, 0x3 };

            var buffer = new ReadOnlySequence<byte>(msg);

            var splitter = new PacketwiseDataMessageSplitter();

            // Act  
            var result = splitter.TryReadCommand(ref buffer, out var command);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.True);
                Assert.That(command.Length, Is.Not.Zero);

                Assert.That(command.IsEqualTo(expectedResult));
                Assert.That(command.Length, Is.EqualTo(msg.Length));
            }
        }
    }
}
