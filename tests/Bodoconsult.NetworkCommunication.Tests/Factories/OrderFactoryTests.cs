// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class OrderFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var factory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        // Assert
        var data = factory.CurrentConfigurations;
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.Zero);
    }

    [Test]
    public void RegisterConfiguration_ValidSetup_ConfigRegistered()
    {
        // Arrange 
        const string configName = "TestConfig";
        var factory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        var builder = new SdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration(configName, BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = DelegateHelper.HandleRequestAnswerOnSuccessDelegate,
            CreateParameterSetDelegate = () => new SdcpParameterSet()
        };

        // Act  
        factory.RegisterConfiguration(config);

        // Assert
        var data = factory.CurrentConfigurations;
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.EqualTo(1));
    }


    [Test]
    public void RegisterConfiguration_NoCreateParameterSetDelegate_ThrowsException()
    {
        // Arrange 
        const string configName = "TestConfig";
        var factory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        var builder = new SdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration(configName, BuiltinOrders.SdcpOrder, builder)
        {
          //  Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = DelegateHelper.HandleRequestAnswerOnSuccessDelegate
        };

        // Act  
        Assert.Throws<ArgumentNullException>(() =>
        {
            factory.RegisterConfiguration(config);
        });
    }

    [Test]
    public void GetConfiguration_ValidSetup_ReturnsConfig()
    {
        // Arrange 
        const string configName = "SdcpOrderConfiguration";
        var factory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        var config = new SdcpOrderConfiguration
        {
        //    Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = DelegateHelper.HandleRequestAnswerOnSuccessDelegate,
            CreateParameterSetDelegate = () => new SdcpParameterSet()
        };

        factory.RegisterConfiguration(config);

        // Act  
        var result = factory.GetConfiguration(configName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetConfiguration_WrongConfigName_ReturnsConfig()
    {
        // Arrange 
        const string configName = "TestConfig";
        var factory = new OrderFactory(TestDataHelper.DefaultOrderIdGenerator);

        var builder = new SdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration(configName, BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = DelegateHelper.HandleRequestAnswerOnSuccessDelegate,
            CreateParameterSetDelegate = () => new SdcpParameterSet()
        };

        factory.RegisterConfiguration(config);

        // Act  
        var result = factory.GetConfiguration("Blubb");

        // Assert
        Assert.That(result, Is.Null);
    }
}