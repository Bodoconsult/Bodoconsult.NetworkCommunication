// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Numerics;
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
            Complex[] buffer =
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
            Complex[] buffer =
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
            Complex[] buffer =
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
        public void Test()
        {
            // Arrange 
            Complex[] buffer =
            [
                new(real: 42, imaginary: 12),
                new(real: 96, imaginary: 34),
                new(real: 13, imaginary: 56),
                new(real: 99, imaginary: 78)
            ];

            var result = new FftManager(buffer);

            result.CalculatePsd();

            // Act  
            result.SaveAsPng();

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Spectrum, Is.EqualTo(buffer));
                Assert.That(result.Psd.Length, Is.Not.Zero);
                Assert.That(result.FrequencyScale.Length, Is.Zero);
            }
        }
    }
}
