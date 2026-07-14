// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Numerics;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using IpBackend.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSampleTests.Backend.BusinessLogic.Adapters
{
    [TestFixture]
    internal class SfxpIpDeviceUdpBusinessLogicAdapterTests
    {
        [Test]
        public void GetArray_ValidListOfChunks_ReturnsComplexArray()
        {
            // Arrange 
            var chunks = new List<DataChunk>
            {
                new()
                {
                    Channel = 0x1,
                    Data = new byte[] { 0xa, 0xb, 0xc, 0xa, 0xb, 0xc, 0xa, 0xb }
                },
                new()
                {
                    Channel = 0x2,
                    Data = new byte[] {  0xb, 0xc, 0xa, 0xb, 0xc, 0xa, 0xb, 0xc }
                },
                new()
                {
                    Channel = 0x3,
                    Data = new byte[] { 0xc, 0xa, 0xb, 0xc, 0xa, 0xb, 0xc, 0xa }
                }
            };

            // Act  
            var result = SfxpIpDeviceUdpBusinessLogicAdapter.GetArray(chunks);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Length, Is.EqualTo(chunks.Count * 8));

                //var span = result.AsSpan();

                //Assert.That(span[0], Is.EqualTo(0x1));
                //Assert.That(span[9], Is.EqualTo(0x2));
                //Assert.That(span[18], Is.EqualTo(0x3));

                //Assert.That(span[1], Is.EqualTo(0xa));
                //Assert.That(span[10], Is.EqualTo(0xb));
                //Assert.That(span[19], Is.EqualTo(0xc));
            }
        }

        [Test]
        public void GetComplex_ValidSetup0xf5_PropsSetCorrectly()
        {
            // Arrange 
            const byte b = 0xf5;

            // Act  
            var result = SfxpIpDeviceUdpBusinessLogicAdapter.GetComplex(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Real, Is.EqualTo(5d));
                Assert.That(result.Imaginary, Is.EqualTo(15d));
            }
        }

        [Test]
        public void GetComplex_ValidSetup0xa_PropsSetCorrectly()
        {
            // Arrange 
            const byte b = 0xa;

            // Act  
            var result = SfxpIpDeviceUdpBusinessLogicAdapter.GetComplex(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Real, Is.EqualTo(10d));
                Assert.That(result.Imaginary, Is.Zero);
            }
        }

        [Test]
        public void CheckListLengthForFft_ListLength1_RemovesNoItem()
        {
            // Arrange 
            var b = new List<Complex> { new(real: 42, imaginary: 12) };

            // Act  
            SfxpIpDeviceUdpBusinessLogicAdapter.CheckListLengthForFft(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(b.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void CheckListLengthForFft_ListLength2_RemovesNoItem()
        {
            // Arrange 
            var b = new List<Complex>
            {
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12)
            };

            // Act  
            SfxpIpDeviceUdpBusinessLogicAdapter.CheckListLengthForFft(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(b.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void CheckListLengthForFft_ListLength3_Removes1Item()
        {
            // Arrange 
            var b = new List<Complex>
            {
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12)
            };

            // Act  
            SfxpIpDeviceUdpBusinessLogicAdapter.CheckListLengthForFft(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(b.Count, Is.EqualTo(2));
            }
        }


        [Test]
        public void CheckListLengthForFft_ListLength4_RemovesNoItem()
        {
            // Arrange 
            var b = new List<Complex>
            {
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12)
            };

            // Act  
            SfxpIpDeviceUdpBusinessLogicAdapter.CheckListLengthForFft(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(b.Count, Is.EqualTo(4));
            }
        }

        [Test]
        public void CheckListLengthForFft_ListLength7_Removes3Items()
        {
            // Arrange 
            var b = new List<Complex>
            {
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12),
                new(real: 42, imaginary: 12)
            };

            // Act  
            SfxpIpDeviceUdpBusinessLogicAdapter.CheckListLengthForFft(b);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(b.Count, Is.EqualTo(4));
            }
        }

        // [Test]
        //public void GetComplex_ValidSetup0xa_PropsSetCorrectly()
        //{
        //    // Arrange 
        //    const byte b = 0xa;

        //    // Act  
        //    var result = SfxpIpDeviceUdpBusinessLogicAdapter.GetComplex(b);

        //    // Assert
        //    using (Assert.EnterMultipleScope())
        //    {
        //        Assert.That(result.Real, Is.EqualTo(10d));
        //        Assert.That(result.Imaginary, Is.Zero);
        //    }
        //}
    }
}
