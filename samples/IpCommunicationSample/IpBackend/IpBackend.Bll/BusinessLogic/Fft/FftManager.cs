// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using FftSharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IpBackend.Bll.BusinessLogic.Fft
{
    public class FftManager
    {
        /// <summary>
        /// Returns true if the given number is evenly divisible by a power of 2
        /// </summary>
        public static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0 && x > 0;
        }

        public static uint LastPowerOf2SmallerThanNumber(uint number)
        {
            var nn = (uint)(0x8000_0000ul >> BitOperations.LeadingZeroCount(number));

            return nn;
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="spectrum">Current spectrum data. Length must be a power of 2.</param>
        public FftManager(Complex[] spectrum)
        {
            Spectrum = spectrum;
        }

        public int SampleRate { get; set; } = 48_000;

        /// <summary>
        /// Current spectrum data. Length must be a power of 2.
        /// </summary>
        public Complex[] Spectrum { get; }

        /// <summary>
        /// Power spectrum density (PSD)
        /// </summary>
        public double[] Psd { get; private set; } = [];

        /// <summary>
        /// Frequency scale
        /// </summary>
        public double[] FrequencyScale { get; private set; } = [];

        /// <summary>
        /// Calculate the power spectral density using FFT
        /// </summary>
        public void CalculatePsd()
        {
            FFT.Forward(Spectrum);
            Psd = FFT.Power(Spectrum);
        }

        /// <summary>
        /// Calculate frequency scale
        /// </summary>
        public void CalculateFrequencyScale()
        {
            FrequencyScale = FFT.FrequencyScale(Psd.Length, SampleRate);
        }
    }
}
