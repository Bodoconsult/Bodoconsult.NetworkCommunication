// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DigitalTwins;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.App.Abstractions.Helpers;
using Bodoconsult.App.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DigitalTwins
{
    [TestFixture]
    internal class SfxpDigitalTwinMessageFactoryTests
    {
        [Test]
        public void GenerateMessages_ValidSetup_ReturnsMessagesAsExpected()
        {
            // Arrange 
            var dt = new SfxpDigitalTwinMessageFactory();

            // Act  
            var result = dt.GenerateMessages();

            // Assert
            Assert.That(result.Count, Is.EqualTo(dt.NumberOfMessagesCreated));

            for (var index = 0; index < result.Count; index++)
            {
                var message = result[index].ToArray();
                Debug.Print($"{message.Length} bytes");
                Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(message));
                WriteToFile(index, message);
            }
        }

        [Explicit]
        [Test]
        public void Test()
        {
            // Arrange 
            var data = ResourceHelper.GetByteResource("Bodoconsult.NetworkCommunication.Tests.Resources.msg.bin");

            // Act  
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] != 0x9)
                {
                    continue;
                }
                var section = data.AsSpan(i, 40);
                Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(section.ToArray()));
            }

            // Assert
        }


        private void WriteToFile(int index, byte[] message)
        {
            var filePath = $"C:\\temp\\sfx{index}.bin";

            File.WriteAllBytes(filePath, message);
        }
    }
}
