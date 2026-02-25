// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement;

[TestFixture]
internal class BaseRequestAnswerTests
{
    [Test]
    public void Ctor_DummyRequestAnswer_PropsSetCorrectly()
    {
        // Arrange 
        

        // Act  
        var ra = new DummyRequestAnswer();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ra.ReceivedMessage, Is.Null);
            Assert.That(ra.DataBlockType, Is.Null);
            Assert.That(ra.HasDatablock, Is.False);
            Assert.That(ra.WasReceived, Is.False);
            Assert.That(ra.HandleRequestAnswerOnSuccessDelegate, Is.Null);
            Assert.That(ra.HandleUnexpectedRequestAnswerDelegate, Is.Null);
        }
    }

    [Test]
    public void CheckReceivedMessage_DummyRequestAnswer_ThrowsException()
    {
        // Arrange 
        var ra = new DummyRequestAnswer();

        // Act and assert
        Assert.Throws<NotSupportedException>(() =>
        {
            ra.CheckReceivedMessage(null, null, new List<string>());
        });
    }
}