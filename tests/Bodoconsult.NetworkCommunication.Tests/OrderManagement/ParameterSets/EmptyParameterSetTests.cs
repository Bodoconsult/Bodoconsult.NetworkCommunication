// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using NUnit.Framework.Internal.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.ParameterSets;

[TestFixture]
internal class EmptyParameterSetTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var ps = new EmptyParameterSet();

        // Assert
        Assert.That(ps, Is.Not.Null);
        Assert.That(ps.GetType().GetInterfaces().Contains(typeof(IParameterSet)));
    }

    [Test]
    public void IsValid_ValidSetup_ReturnsTrue()
    {
        // Arrange 
        var ps = new EmptyParameterSet();

        // Act  
        var result = ps.IsValid;

        // Assert
        Assert.That(result.Any(), Is.False);
    }

    [Test]
    public void LoadOrder_TestOrder_OrderLoaded()
    {
        // Arrange 
        var ps = new EmptyParameterSet();

        var dateTimeService = TestDataHelper.AppDateService;
        var benchLogger = TestDataHelper.GetFakeAppBenchProxy();

        // Act  
        IOrder order = new TestOrder(ps, dateTimeService, benchLogger);

        // Assert
        Assert.That(order, Is.Not.Null);
        Assert.That(ps.CurrentOrder, Is.Not.Null);
    }

    [Test]
    public void ToDataBlock_ValidSetup_DataFilled()
    {
        // Arrange 
        var ps = new EmptyParameterSet();

        // Act  
        ps.ToDataBlock();

        // Assert
        Assert.That(ps.Data.Length, Is.EqualTo(0));
    }
}