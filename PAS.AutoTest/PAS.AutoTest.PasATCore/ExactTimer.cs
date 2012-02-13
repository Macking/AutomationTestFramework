using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace PAS.AutoTest.PasATCore
{
    /// <summary>
    /// Notice:: If using this class when your laptop's power is plug in, the result would not exactly. The result deponds on the CPU's frequency.
    /// Using System.Diagnostics.Stopwatch would more exactly. 
    /// </summary>
    public class ExactTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpm_Frequency);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        private long mStart = 0;
        private long mEnd = 0;
        private long mFreq;

        public ExactTimer()
        {
            int attemp = 0;
            while (QueryPerformanceFrequency(out mFreq) == false && attemp < 100)
            {
                attemp++;
            }
        }

        public void Start()
        {
            System.Threading.Thread.Sleep(0);
            QueryPerformanceCounter(out mStart);
        }

        public void Stop()
        {
            QueryPerformanceCounter(out mEnd);
        }

        public double Duration
        {
            get
            {
                return (mEnd - mStart)*1000/mFreq;
            }
        }
    }
}
