// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Numerics;
using FftSharp;
using ScottPlot;
using SampleData = FftSharp.SampleData;

namespace IpBackend.Bll.BusinessLogic.Fft
{
    public class FftManager
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="spectrum">Current spectrum data. Length must be a power of 2.</param>
        public FftManager(Complex[] spectrum)
        {
            Spectrum = spectrum;
        }

        /// <summary>
        /// Sample rate in hz
        /// </summary>
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


        public void SaveAsPng()
        {
            // sample audio with tones at 2, 10, and 20 kHz plus white noise
            double[] signal = SampleData.SampleAudio1();
            int sampleRate = 48_000;

            // calculate the power spectral density using FFT
            Complex[] spectrum = FFT.Forward(signal);
            double[] psd = FFT.Power(spectrum);
            double[] freq = FFT.FrequencyScale(psd.Length, sampleRate);

            // plot the sample audio
            Plot plt = new();
            plt.Add.ScatterLine(freq, psd);
            plt.YLabel("Power (dB)");
            plt.XLabel("Frequency (Hz)");

            try
            {

                plt.SavePng(@"C:\temp\periodogram.png", 500, 200);

                //var xml = plt.GetSvgXml(1024, 768);

                //Debug.Print(xml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
