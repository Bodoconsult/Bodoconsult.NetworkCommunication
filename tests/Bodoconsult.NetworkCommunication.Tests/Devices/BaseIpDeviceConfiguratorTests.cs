// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Devices;

[TestFixture]
internal class BaseIpDeviceConfiguratorTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var config = new TestIpDeviceConfigurator();

        // Assert
        Assert.That(config.DataMessagingConfig, Is.Null);
        Assert.That(config.Device, Is.Null);
    }

    [Test]
    public void CreateMessagingConfig_ValidSetup_ThrowsException()
    {
        // Arrange 
        var config = new TestIpDeviceConfigurator();
        IDataMessageProcessingPackageFactory messageProcessingPackageFactory = new TncpDataMessageProcessingPackageFactory();

        // Act  
        Assert.Throws<NotSupportedException>(() =>
        {
            config.CreateMessagingConfig("TestDevice", "127.0.0.1", 9000, messageProcessingPackageFactory);
        });

        // Assert
        Assert.That(config.DataMessagingConfig, Is.Null);
        Assert.That(config.Device, Is.Null);
    }

    [Test]
    public void CreateDevice_ValidSetup_ThrowsException()
    {
        // Arrange 
        var config = new TestIpDeviceConfigurator();

        IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new TestIpDeviceAdapterFactory();

        // Act  
        Assert.Throws<NotSupportedException>(() =>
        {
            config.CreateDevice(businessLogicAdapterFactory);
        });

        // Assert
        Assert.That(config.DataMessagingConfig, Is.Null);
        Assert.That(config.Device, Is.Null);
    }

    // ToDo MOQ this test
    //[Test]
    //public void CreateMessagingConfig_ValidSetup_ThrowsException()
    //{
    //    // Arrange 
    //    var config = new TestIpDeviceConfigurator();

    //    IDeviceBusinessLogicAdapterFactory businessLogicAdapterFactory = new T;
    //    IOrderManagerFactory orderManagerFactory;

    //    // Act  
    //    Assert.Throws<NotSupportedException>(() =>
    //    {

    //        config.ConfigureOrderManagementAndStateManagement(businessLogicAdapterFactory, orderManagerFactory);
    //    });

    //    // Assert
    //    Assert.That(config.DataMessagingConfig, Is.Null);
    //    Assert.That(config.Device, Is.Null);
    //}

    [Test]
    public void GetDevice_ValidSetup_ThrowsException()
    {
        // Arrange 
        var config = new TestIpDeviceConfigurator();

        // Act  
        Assert.Throws<ArgumentNullException>(() =>
        {
            config.GetDevice();
        });

        // Assert
        Assert.That(config.DataMessagingConfig, Is.Null);
        Assert.That(config.Device, Is.Null);
    }
}