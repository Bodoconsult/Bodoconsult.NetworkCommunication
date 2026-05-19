// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageProcessingPackages;

[TestFixture]
internal class TncpDataMessageProcessingPackageFactoryTests
{

    [Test]
    public void CreateInstance_ValidSetup_ReturnsPackage()
    {
        // Arrange 
        var factory = new TncpDataMessageProcessingPackageFactory();
        var config = TestDataHelper.GetDataMessagingConfig();

        // Act  
        factory.CreateInstance(config);

        // Assert
        Assert.That(config.DataMessageProcessingPackage?.GetType(), Is.EqualTo(typeof(TncpDataMessageProcessingPackage)));
    }
}