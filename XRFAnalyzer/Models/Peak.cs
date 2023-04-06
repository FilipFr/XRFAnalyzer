using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal partial class Peak : ObservableObject
    {
        [ObservableProperty]
        private double _height;
        [ObservableProperty]
        private int _apexChannel;
        [ObservableProperty]
        private int _apexEnergy;
        [ObservableProperty]
        private Tuple<int, int> _channelRange = new (0, 0);
        [ObservableProperty]
        private Tuple<double, double> _energyRange = new (0, 0);
        [ObservableProperty]
        private double _grossArea;
        [ObservableProperty]
        private double _netArea;
        [ObservableProperty]
        private bool canBeSumPeak = false;

        public static List<Peak> GetPeaksFromSpectrum(List<double> values, List<Tuple<int,int>> rois) 
        {
            List<Peak> peaks = new();
            foreach(var roi in rois) 
            {
                Peak toAdd = new Peak();
                toAdd.Height = values.Skip(roi.Item1).Take(roi.Item2 - roi.Item1).Max();
                toAdd.ApexChannel = values.ToList().IndexOf(toAdd.Height);
                toAdd.ChannelRange = roi;
                peaks.Add(toAdd);
            }
            return peaks;
        }

        public static Peak GetPeakFromRoi(List<double> values, Tuple<int,int> roi) 
        {
            Peak peak = new Peak();
            if (roi.Item2 > roi.Item1)
            {
                peak.Height = values.Skip(roi.Item1).Take(roi.Item2 - roi.Item1).Max();
                peak.ApexChannel = values.ToList().IndexOf(peak.Height);
                peak.ChannelRange = roi;
            }
            return peak;
        }

        public void CalculateAreas(List<double> counts, List<double> correctedCounts) 
        {
            GrossArea = 0;
            NetArea = 0;
            for (int i = ChannelRange.Item1; i <= ChannelRange.Item2; i++) 
            {
                GrossArea += counts[i];
                NetArea += correctedCounts[i];
            }
        }

        public void DetermineIfSumPeak(List<Peak> peaks) 
        {
            foreach(Peak peak in peaks)
            {
                if (peak.ChannelRange.Item1 < 2 * this.ApexChannel && peak.ChannelRange.Item2 > 2* this.ApexChannel) 
                {
                    peak.CanBeSumPeak = true;
                }
            }
        }
    }
}
