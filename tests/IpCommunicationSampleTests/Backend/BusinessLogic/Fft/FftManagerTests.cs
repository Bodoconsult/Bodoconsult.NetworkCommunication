// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpBackend.Bll.BusinessLogic.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpBackend.Bll.BusinessLogic.Fft;

namespace IpCommunicationSampleTests.Backend.BusinessLogic.Fft
{
    [TestFixture]
    internal class FftManagerTests
    {
        [Test]
        public void Ctor_ValidSetup_PropsSetCorrectly()
        {
            // Arrange 
            System.Numerics.Complex[] buffer =
            [
                new(real: 42, imaginary: 12),
                new(real: 96, imaginary: 34),
                new(real: 13, imaginary: 56),
                new(real: 99, imaginary: 78)
            ];

            // Act  
            var result = new FftManager(buffer);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Spectrum, Is.EqualTo(buffer));
                Assert.That(result.Psd.Length, Is.Zero);
                Assert.That(result.FrequencyScale.Length, Is.Zero);
            }
        }

        [Test]
        public void CalculatePsd_ValidData_PsdSet()
        {
            // Arrange 
            System.Numerics.Complex[] buffer =
            [
                new(real: 42, imaginary: 12),
                new(real: 96, imaginary: 34),
                new(real: 13, imaginary: 56),
                new(real: 99, imaginary: 78)
            ];

            var result = new FftManager(buffer);

            // Act  
            result.CalculatePsd();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Spectrum, Is.EqualTo(buffer));
                Assert.That(result.Psd.Length, Is.Not.Zero);
                Assert.That(result.FrequencyScale.Length, Is.Zero);
            }
        }

        [Test]
        public void CalculateFrequencyScale_ValidData_FreqSet()
        {
            // Arrange 
            System.Numerics.Complex[] buffer =
            [
                new(real: 42, imaginary: 12),
                new(real: 96, imaginary: 34),
                new(real: 13, imaginary: 56),
                new(real: 99, imaginary: 78),
                new(real: 42, imaginary: 12),
                new(real: 96, imaginary: 34),
                new(real: 13, imaginary: 56),
                new(real: 99, imaginary: 78),
                new(real: 42, imaginary: 12),
                new(real: 86, imaginary: 34),
                new(real: 18, imaginary: 56),
                new(real: 99, imaginary: 78),
                new(real: 42, imaginary: 12),
                new(real: 86, imaginary: 30),
                new(real: 18, imaginary: 56),
                new(real: 99, imaginary: 70)
            ];

            var result = new FftManager(buffer);
            result.CalculatePsd();

            // Act  
            result.CalculateFrequencyScale();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Spectrum, Is.EqualTo(buffer));
                Assert.That(result.Psd.Length, Is.Not.Zero);
                Assert.That(result.FrequencyScale.Length, Is.Not.Zero);
            }
        }

        [Test]
        public void LastPowerOf2SmallerThanNumber_1_Returns0()
        {
            // Arrange 
            const uint value = 1;

            // Act  
            var result = FftManager.LastPowerOf2SmallerThanNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void LastPowerOf2SmallerThanNumber_2_Returns1()
        {
            // Arrange 
            const uint value = 2;

            // Act  
            var result = FftManager.LastPowerOf2SmallerThanNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void LastPowerOf2SmallerThanNumber_3_Returns1()
        {
            // Arrange 
            const uint value = 3;

            // Act  
            var result = FftManager.LastPowerOf2SmallerThanNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void LastPowerOf2SmallerThanNumber_4_Returns1()
        {
            // Arrange 
            const uint value = 4;

            // Act  
            var result = FftManager.LastPowerOf2SmallerThanNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public void LastPowerOf2SmallerThanNumber_7_Returns1()
        {
            // Arrange 
            const uint value = 7;

            // Act  
            var result = FftManager.LastPowerOf2SmallerThanNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(4));
        }

        public void LastPowerOf2SmallerThanNumber_18_Returns1()
        {
            // Arrange 
            const uint value = 18;

            // Act  
            var result = FftManager.LastPowerOf2SmallerThanNumber(value);

            // Assert
            Assert.That(result, Is.EqualTo(16));
        }
    }
}
