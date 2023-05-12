using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using ScottPlot.MarkerShapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

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
        private Tuple<int, int> _channelRange = new(0, 0);
        [ObservableProperty]
        private EnergyRangeTuple _energyRange = new(0, 0);
        [ObservableProperty]
        private double _grossArea;
        [ObservableProperty]
        private double _netArea;
        [ObservableProperty]
        private bool canBeSumPeak = false;
        [ObservableProperty]
        private ObservableCollection<EmissionLine> potentialEmissionLines = new();
        [ObservableProperty]
        private EmissionLine? currentEmissionLine = null;
        [ObservableProperty]
        private double _centroid;
        [ObservableProperty]
        private double _fwhm;
        [ObservableProperty]
        private double _fwhm_channels;


        public static List<Peak> GetPeaksFromSpectrum(List<double> values, List<Tuple<int, int>> rois)
        {
            List<Peak> peaks = new();
            foreach (var roi in rois)
            {
                Peak toAdd = new Peak();
                toAdd.Height = values.Skip(roi.Item1).Take(roi.Item2 - roi.Item1).Max();
                toAdd.ApexChannel = values.ToList().IndexOf(toAdd.Height);
                toAdd.ChannelRange = roi;
                toAdd.GaussianCentroidCalculation(toAdd.ApexChannel, values, roi);
                peaks.Add(toAdd);
            }
            return peaks;
        }

        public static Peak GetPeakFromRoi(List<double> values, Tuple<int, int> roi)
        {
            Peak peak = new Peak();
            if (roi.Item2 > roi.Item1)
            {
                peak.Height = values.Skip(roi.Item1).Take(roi.Item2 - roi.Item1).Max();
                peak.ApexChannel = values.ToList().IndexOf(peak.Height);
                peak.ChannelRange = roi;
                peak.GaussianCentroidCalculation(peak.ApexChannel, values, roi);

            }
            return peak;
        }

        public void CalculateAreas(List<double> counts, List<double> correctedCounts)
        {
            GrossArea = 0;
            NetArea = 0;
            CalculateFwkm(correctedCounts, 0.5);
            for (int i = ChannelRange.Item1; i <= ChannelRange.Item2; i++)
            {
                GrossArea += counts[i];
                NetArea += correctedCounts[i];
            }

        }

        public void DetermineIfSumPeak(List<Peak> peaks)
        {
            foreach (Peak peak in peaks)
            {
                if (peak.ChannelRange.Item1 < 2 * this.ApexChannel && peak.ChannelRange.Item2 > 2 * this.ApexChannel)
                {
                    peak.CanBeSumPeak = true;
                }
            }
        }

        public void FindPotentialEmissionLines(List<Element> elements, ObservableCollection<CalibrationRow> calibrationRows, double range)
        {
            PotentialEmissionLines.Clear();
            foreach (CalibrationRow row in calibrationRows)
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

        public void SortPotentialEmissionLines(List<Peak> peaks, ObservableCollection<CalibrationRow> calibrationRows)
        {
            ObservableCollection<EmissionLine> sortedLines = new();

            SortByIdentifiedElements(peaks);
            SortByCurrentElements(peaks);
            SortByCalibrationPoints(calibrationRows);
            
  
        }

        public static List<EmissionLine> GetCurrentEmissionLines(List<Peak> peaks)
        {
            List<EmissionLine> currentLines = new();
            foreach (Peak peak in peaks)
            {
                if (peak.CurrentEmissionLine != null)
                {
                    currentLines.Add(peak.CurrentEmissionLine);
                }
            }
            return currentLines;
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

        public void CalculateFwkm(List<double> counts, double relativeHeight)
        {
            try
            {
                if (relativeHeight <= 0 || relativeHeight >= 1)
                {
                    throw new ArgumentException("k must be from interval (0, 1)");
                }
                int x1 = -1;
                int x3 = -1;
                double y1 = -1;
                double y2 = -1;
                double y3 = -1;
                double y4 = -1;

                double widthLevel = relativeHeight * this.Height;

                for (int i = this.ChannelRange.Item1; i < this.ChannelRange.Item2 - 1; i++)
                {
                    if (counts[i] < widthLevel && widthLevel < counts[i + 1])
                    {
                        x1 = i;
                        y1 = counts[i];
                        y2 = counts[i + 1];
                    }
                    if (x1 != -1 && counts[i] > widthLevel && widthLevel > counts[i + 1])
                    {
                        x3 = i;
                        y3 = counts[i];
                        y4 = counts[i + 1];
                    }
                }
                this.Fwhm_channels = x3 - x1 + ((y3 - widthLevel) / (y3 - y4) - (widthLevel - y1) / (y2 - y1));
            }
            catch
            {
                this.Fwhm_channels = -1;
            }
        }

        public void CalculateCentroid()
        {

        }

        public double FiveChannelCentroidCalculation(int middleChannel, List<double> counts)
        {
            try
            {
                if (counts.Count != 5 || middleChannel < 0)
                {
                    throw new Exception("Conditions for using FiveChannelCentroidCalculator not met.");
                }
                double centroid = 0;
                centroid = middleChannel +
                    (counts[3] * (counts[2] - counts[0]) - counts[1] * (counts[2] - counts[4])) /
                    (counts[3] * (counts[2] - counts[0]) + counts[1] * (counts[2] - counts[4]));
                return centroid;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void GaussianCentroidCalculation(int middleChannel, List<double> counts, Tuple<int, int> borders)
        {
            try
            {
                double[] yValues = GetFiveValuesAroundMaximum(middleChannel, counts, borders).ToArray();
                if (yValues.Length < 0)
                {
                    throw new Exception("GaussianCentroidCalculation failed.");
                }
                double[] xValues = { middleChannel - 2, middleChannel - 1, middleChannel, middleChannel + 1, middleChannel + 2 };
                ValueTuple<double, double, double> vt = MathNet.Numerics.Fit.Curve(xValues, yValues, gaussian, counts[middleChannel], middleChannel, 1);
                this.Centroid = vt.Item2;
            }
            catch
            {
                this.Centroid = -1;
            }
        }

        private static List<double> GetFiveValuesAroundMaximum(int maximumChannel, List<double> counts, Tuple<int, int> borders)
        {
            try
            {
                if (maximumChannel + 2 > borders.Item2 || maximumChannel - 2 < borders.Item1)
                {
                    throw new Exception("Channel containing the maximum value is too close to the border.");
                }
                return counts.Skip(maximumChannel - 2).Take(5).ToList();
            }
            catch (Exception ex)
            {
                return new List<double>();
            }
        }
        private static double Gauss(double a, double b, double c, double x)
        {
            return a * Math.Pow(Math.E, (-1 * Math.Pow((x - b), 2) / (2 * Math.Pow(c, 2))));
        }
        private static readonly Func<double, double, double, double, double> gaussian = Gauss;

        private void SortByIdentifiedElements(List<Peak> peaks)
        {
            ObservableCollection<EmissionLine> sortedLines = new();

            foreach (EmissionLine line in this.PotentialEmissionLines)
            {
                int counter = 0;
                foreach (Peak peak in peaks)
                {
                    foreach (EmissionLine potentialLine in peak.PotentialEmissionLines)
                    {
                        
                        if (peak != this && line.Number == potentialLine.Number)
                        {
                            sortedLines.Add(line);

                        }
                        counter++;
                    }
                    counter = 0;
                }
            }
            foreach (EmissionLine line in this.PotentialEmissionLines)
            {
                if (!sortedLines.Contains(line))
                {
                    sortedLines.Add(line);
                }
            }
            this.PotentialEmissionLines = sortedLines;
        }

        private void SortByCurrentElements(List<Peak> peaks)
        {
            ObservableCollection<EmissionLine> sortedLines = new();

            foreach (EmissionLine line in this.PotentialEmissionLines)
            {
                foreach (Peak peak in peaks)
                {
                    foreach (EmissionLine potentialLine in peak.PotentialEmissionLines)
                    {

                        if (peak != this && line.Number == potentialLine.Number)
                        {
                            sortedLines.Add(line);

                        }
                        break;
                    }
                }
            }
            foreach (EmissionLine line in this.PotentialEmissionLines)
            {
                if (!sortedLines.Contains(line))
                {
                    sortedLines.Add(line);
                }
            }
            this.PotentialEmissionLines = sortedLines;
        }

        private void SortByCalibrationPoints(ObservableCollection<CalibrationRow> calibrationRows) 
        {
            ObservableCollection<EmissionLine> sortedLines = new();
            foreach (EmissionLine line in this.PotentialEmissionLines)
            {
                foreach (CalibrationRow row in calibrationRows)
                {
                    if (row.EmissionLine != null && line.Energy == row.EmissionLine.Energy)
                    {
                        sortedLines.Add(line);
                    }
                }
            }
            foreach (EmissionLine line in this.PotentialEmissionLines)
            {
                if (!sortedLines.Contains(line))
                {
                    sortedLines.Add(line);
                }
            }
            this.PotentialEmissionLines =  sortedLines;
        }

    }
}
