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

        public static List<Peak> GetPeaksFromSpectrum(Spectrum spectrum, List<Tuple<int,int>> rois) 
        {
            List<Peak> peaks = new();
            foreach(var roi in rois) 
            {
                Peak toAdd = new Peak();
                toAdd.Height = spectrum.Counts.Skip(roi.Item1).Take(roi.Item2 - roi.Item1).Max();
                toAdd.ApexChannel = spectrum.Counts.ToList().IndexOf(toAdd.Height);
                toAdd.ChannelRange = roi;
                peaks.Add(toAdd);
            }
            return peaks;
        }

        public static Peak GetPeakFromRoi(Spectrum spectrum, Tuple<int,int> roi) 
        {
            Peak peak = new Peak();
            if (roi.Item2 > roi.Item1)
            {
                peak.Height = spectrum.Counts.Skip(roi.Item1).Take(roi.Item2 - roi.Item1).Max();
                peak.ApexChannel = spectrum.Counts.ToList().IndexOf(peak.Height);
                peak.ChannelRange = roi;
            }
            return peak;
        }
    }
}
