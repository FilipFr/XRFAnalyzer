using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class SpectrumPoint
    {
        public int Channel { get; set; }
        public int Count { get; set; }
        public double Energy { get; set; }     // Photon energy of characteristic radiation in kEv
        public double LogCount { get { return double.IsInfinity(Math.Log10(this.Count)) ? 0 : Math.Log10(this.Count); } }

        public SpectrumPoint(int channel, int count)
        {
            Channel = channel;
            Count = count;
        }

        public void SetEnergy(double slope, double intercept)
        {
            this.Energy = slope * this.Count + intercept;
        }

        public static List<SpectrumPoint> GetPointsFromIntegers(List<int> data)
        {
            List<SpectrumPoint> points = new();
            for (int i = 0; i < data.Count; i++) 
            {
                points.Add(new SpectrumPoint(i, data[i]));
            }
            return points;
        }
    }
}
