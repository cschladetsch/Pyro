﻿namespace Pyro
{
    using System;

    /// <summary>
    /// A simple low-pass filter.
    /// </summary>
    public class FilterButterworth
    {
        public enum PassType
        {
            Highpass,
            Lowpass
        }

        private readonly float c, a1, a2, a3, b1, b2;

        private readonly float frequency;
        private readonly PassType passType;

        /// <summary>
        /// rez amount, from sqrt(2) to ~ 0.1
        /// </summary>
        private readonly float resonance;
        private readonly int sampleRate;

        /// <summary>
        /// Array of input values, latest are in front
        /// </summary>
        private readonly float[] inputHistory = new float[2];

        /// <summary>
        /// Array of output values, latest are in front
        /// </summary>
        private readonly float[] outputHistory = new float[3];

        public FilterButterworth(float frequency, int sampleRate, PassType passType,
            float resonance = 1.414213562373095f)
        {
            this.resonance = resonance;
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.passType = passType;

            switch (passType)
            {
                case PassType.Lowpass:
                    c = 1.0f / (float) Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = 2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (1.0f - c * c) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
                case PassType.Highpass:
                    c = (float) Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0f / (1.0f + resonance * c + c * c);
                    a2 = -2f * a1;
                    a3 = a1;
                    b1 = 2.0f * (c * c - 1.0f) * a1;
                    b2 = (1.0f - resonance * c + c * c) * a1;
                    break;
            }
        }

        public float Value 
            => outputHistory[0];

        public void Update(float newInput)
        {
            var newOutput = a1 * newInput + a2 * inputHistory[0] + a3 * inputHistory[1] - b1 * outputHistory[0] -
                            b2 * outputHistory[1];

            inputHistory[1] = inputHistory[0];
            inputHistory[0] = newInput;

            outputHistory[2] = outputHistory[1];
            outputHistory[1] = outputHistory[0];
            outputHistory[0] = newOutput;
        }
    }
}