// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Bodoconsult.NetworkCommunication.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.HelperTests;

[TestFixture]
internal class BitHelperTests
{
    [Test]
    public void FromInt64ToBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const long value = 1;

        // Act  
        var result = BitHelper.FromInt64ToBigEndian(value);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            // Little endian
            Assert.That(result[0], Is.Zero);
            Assert.That(result[1], Is.Zero);
            Assert.That(result[2], Is.Zero);
            Assert.That(result[3], Is.Zero);

            // Big endian
            Assert.That(result[4], Is.Zero);
            Assert.That(result[5], Is.Zero);
            Assert.That(result[6], Is.Zero);
            Assert.That(result[7], Is.EqualTo(1));
        }
    }

    [Test]
    public void FromInt64ToBigEndian_ValidValueMinus1_ReturnsArray()
    {
        // Arrange 
        const long value = -2;

        // Act  
        var result = BitHelper.FromInt64ToBigEndian(value);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            // Little endian
            Assert.That(result[0], Is.EqualTo(255));
            Assert.That(result[1], Is.EqualTo(255));
            Assert.That(result[2], Is.EqualTo(255));
            Assert.That(result[3], Is.EqualTo(255));

            // Big endian
            Assert.That(result[4], Is.EqualTo(255));
            Assert.That(result[5], Is.EqualTo(255));
            Assert.That(result[6], Is.EqualTo(255));
            Assert.That(result[7], Is.EqualTo(254));
        }
    }

    [Test]
    public void FromInt32ToBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const int value = 1;

        // Act  
        var result = BitHelper.FromInt32ToBigEndian(value);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            // Little endian
            Assert.That(result[0], Is.Zero);
            Assert.That(result[1], Is.Zero);

            // Big endian
            Assert.That(result[2], Is.Zero);
            Assert.That(result[3], Is.EqualTo(1));
        }
    }

    [Test]
    public void FromInt16ToBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const short value = 1;

        // Act  
        var result = BitHelper.FromInt16ToBigEndian(value);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            // Little endian
            Assert.That(result[0], Is.Zero);

            // Big endian
            Assert.That(result[1], Is.EqualTo(1));
        }
    }

    [Test]
    public void FromDoubleToBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const double value = 1;

        // Act  
        var result = BitHelper.FromDoubleToBigEndian(value);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Length, Is.Not.Zero);
            Assert.That(result.Length, Is.EqualTo(8));
        }
    }

    [Test]
    public void FromSingleToBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const float value = 1;

        // Act  
        var result = BitHelper.FromSingleToBigEndian(value);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Length, Is.Not.Zero);
            Assert.That(result.Length, Is.EqualTo(4));
        }
    }

    [Test]
    public void ToInt64FromBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const long value = 1;
        var array = BitHelper.FromInt64ToBigEndian(value);

        // Act  
        var result = BitHelper.ToInt64FromBigEndian(array);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void ToInt32FromBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const int value = 1;
        var array = BitHelper.FromInt32ToBigEndian(value);

        // Act  
        var result = BitHelper.ToInt32FromBigEndian(array);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void ToInt16FromBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const short value = 1;
        var array = BitHelper.FromInt16ToBigEndian(value);

        // Act  
        var result = BitHelper.ToInt16FromBigEndian(array);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void ToDoubleFromBigEndian_ValidValue1_ReturnsArray()
    {
        // Arrange 
        const short value = 1;
        var array = BitHelper.FromDoubleToBigEndian(value);

        // Act  
        var result = BitHelper.ToDoubleFromBigEndian(array);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void ToSingleFromBigEndian_ValidArray_ReturnsArray()
    {
        // Arrange 
        const float value = 1;
        //var array = new byte[]{0x0, 0x0, 0x0, 0x1};
        var array = BitHelper.FromSingleToBigEndian(value);

        // Act  
        var result = BitHelper.ToSingleFromBigEndian(array);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(value));
        }
    }

    [Test]
    public void Test()
    {
        const long value = 0xb9;

        var result1 = value & 0x0f;

        Assert.That(result1, Is.EqualTo(0x9));

        var result2 = value & 0xf0;

        Assert.That(result2, Is.EqualTo(0xb0));

        var result3 = value >> 4;

        Assert.That(result3, Is.EqualTo(0xb));
    }
}