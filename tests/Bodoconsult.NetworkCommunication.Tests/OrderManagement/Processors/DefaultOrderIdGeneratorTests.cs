// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class DefaultOrderIdGeneratorTests
{
    [Test]
    public void NextId_ValidSetup_IdGenerated()
    {
        // Arrange 
        var generator = new DefaultOrderIdGenerator(TestDataHelper.AppDateService);

        // Act  
        var id = generator.NextId();

        // Assert
        Assert.That(id, Is.Not.Zero);
    }
}