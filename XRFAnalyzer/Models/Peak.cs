using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XRFAnalyzer.Models
{
    internal partial class Peak : ObservableObject
    {
        [ObservableProperty]
        private double _height;
        [ObservableProperty]
        private int _apexChannel;
        [ObservableProperty]
        private double _apexEnergy;
        [ObservableProperty]
        private Tuple<int, int> _channelRange = new (0, 0);
        [ObservableProperty]
        private EnergyRangeTuple _energyRange = new (0, 0);
        [ObservableProperty]
        private double _grossArea;
        [ObservableProperty]
        private double _netArea;
        [ObservableProperty]
        private bool canBeSumPeak = false;
        [ObservableProperty]
        private ObservableCollection<EmissionLine> potentialEmissionLines = new();
        [ObservableProperty]
        private EmissionLine? confirmedEmissionLine = null;

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

        public void FindPotentialEmissionLines(List<Element> elements, ObservableCollection<CalibrationRow> calibrationRows, double range) 
        {
            PotentialEmissionLines.Clear();
            foreach(CalibrationRow row in calibrationRows) 
            {
                if (row.EmissionLine != null && row.EmissionLine.Energy - range <= this.ApexEnergy && this.ApexEnergy <= row.EmissionLine.Energy + range) 
                {
                    potentialEmissionLines.Add(row.EmissionLine);
                    return;
                }
            }
            foreach (Element element in elements) 
            {
                foreach (EmissionLine line in element.EmissionLines)
                {
                    if (this.ApexEnergy != 0 && (line.Energy - range <= this.ApexEnergy && this.ApexEnergy <= line.Energy + range))
                    {
                        this.potentialEmissionLines.Add(line);
                    }
                }
            }
            this.potentialEmissionLines = new ObservableCollection<EmissionLine>(this.potentialEmissionLines.OrderBy(x => x.Line).ToList());
        }

        public partial class EnergyRangeTuple : ObservableObject 
        {
            [ObservableProperty]
            private double _item1;
            [ObservableProperty]
            private double _item2;

            public EnergyRangeTuple(double item1, double item2) 
            {
                this.Item1 = item1;
                this.Item2 = item2;
            }

            public override string ToString()
            {
                string toReturn = String.Format("({0:N2}, {1:N2})", this.Item1, this.Item2);
                return toReturn;
            }
        }
    }
}
