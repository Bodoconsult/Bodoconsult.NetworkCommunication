// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataSorters;

[TestFixture]
internal class DefaultInboundDataMessageSorterTests
{
    private readonly IAppLoggerProxy _logger = TestDataHelper.Logger;

    [OneTimeTearDown]
    public void CleanUp()
    {
        _logger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        // Assert
        Assert.That(sorter.LastMessageId, Is.EqualTo(ulong.MinValue));
    }

    [Test]
    public void AddMessage_MessageWithId0_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var msg = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        // Act  
        var result = sorter.AddMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg.OriginalMessageId));
            Assert.That(result.Contains(msg));
        }
    }

    [Test]
    public void AddMessage_MessageWithId1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var msg = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result = sorter.AddMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg.OriginalMessageId));
            Assert.That(result.Contains(msg));
        }
    }

    [Test]
    public void AddMessage_Message1Id0Message2Id1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg1.OriginalMessageId));
            Assert.That(result1.Contains(msg1));
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg2.OriginalMessageId));
            Assert.That(result2.Contains(msg2));
        }
    }

    [Test]
    public void AddMessage_Message1Id2Message2Id1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var msg0 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        var result0 = sorter.AddMessage(msg0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg0.OriginalMessageId));
            Assert.That(result0.Contains(msg0), Is.True);
        }

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 2
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Zero);
            Assert.That(result1.Contains(msg1), Is.False);
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Not.Zero);
            Assert.That(result2.Contains(msg1));
            Assert.That(result2.Contains(msg2));
        }
    }

    [Test]
    public void AddMessage_Message1Id3Message2Id2Message3Id1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var msg0 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        var result0 = sorter.AddMessage(msg0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg0.OriginalMessageId));
            Assert.That(result0.Contains(msg0), Is.True);
        }

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 3
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Zero);
            Assert.That(result1.Contains(msg1), Is.False);
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 2
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Zero);
            Assert.That(result2.Contains(msg2), Is.False);
        }

        var msg3 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result3 = sorter.AddMessage(msg3);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg1.OriginalMessageId));
            Assert.That(result3.Contains(msg1));
            Assert.That(result3.Contains(msg2));
            Assert.That(result3.Contains(msg3));
        }
    }

    [Test]
    public void AddMessage_TwoMessagesWithSameId_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var msg0 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        var result0 = sorter.AddMessage(msg0);

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 3
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg0.OriginalMessageId));
            Assert.That(result1.Contains(msg1), Is.False);
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 3
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg0.OriginalMessageId));
            Assert.That(result2.Contains(msg2), Is.False);
        }
    }

    [Explicit]
    [Test]
    public void AddMessage_MultipleMessagesRealWorld1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter(_logger);

        var fileContent = File.ReadAllText("C:\\Temp\\data.txt").Replace("\r\n", "\r", StringComparison.InvariantCultureIgnoreCase)
            .Replace(",", string.Empty, StringComparison.InvariantCultureIgnoreCase)
            .Replace(" US ", string.Empty, StringComparison.InvariantCultureIgnoreCase);

        var data = fileContent.Split('\r', StringSplitOptions.RemoveEmptyEntries);

        var numbers = new List<int>();

        foreach (var s in data)
        {
            var v = s.Split(':', StringSplitOptions.RemoveEmptyEntries);
            //Debug.Print(v[1]);
            numbers.Add(Convert.ToInt32(v[1]));

        }

        foreach (var number in numbers)
        {
            var msg1 = new SdcpSortableInboundDataMessage
            {
                OriginalMessageId = (ulong)number
            };

            var result = sorter.AddMessage(msg1);

            if (result.Count > 0)
            {
                Debug.Print($"OID {number}: returns {result.Count} messages");
            }
            //foreach (var msg in result)
            //{

            //}

            //using (Assert.EnterMultipleScope())
            //{
            //    Assert.That(sorter.LastMessageId, Is.EqualTo(msg1.OriginalMessageId));
            //    Assert.That(result1.Contains(msg1));
            //}
        }


    }
}