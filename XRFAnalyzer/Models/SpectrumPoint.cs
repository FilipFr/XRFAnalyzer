using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XRFAnalyzer.Models
{
    internal partial class SpectrumPoint : ObservableObject
    {
        [ObservableProperty]
        private int _channel;
        [ObservableProperty]
        private int _count;
        [ObservableProperty]
        private double _energy; // Photon energy of characteristic radiation in kEv
        [ObservableProperty]
        private double _logCount;

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
