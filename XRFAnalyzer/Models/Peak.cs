using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XRFAnalyzer.Models
{
    internal partial class Peak : ObservableObject
    {
        [ObservableProperty]
        private SpectrumPoint _highestPoint;
        [ObservableProperty]
        private SpectrumPoint _leftBasePoint;
        [ObservableProperty]
        private SpectrumPoint _rightBasePoint;
        [ObservableProperty]
        private double? _grossArea;
        [ObservableProperty]
        private double? _netArea;

        public Peak(SpectrumPoint highestPoint, SpectrumPoint leftBasePoint, SpectrumPoint rightBasePoint)
        {
            HighestPoint = highestPoint;
            LeftBasePoint = leftBasePoint;
            RightBasePoint = rightBasePoint;
        }

        public static Peak ConvertFromRoi(List<SpectrumPoint> list, int start, int end) 
        {
            SpectrumPoint leftBasePoint = list[start];
            SpectrumPoint rightBasePoint = list[end];
            SpectrumPoint highestPoint = list.Skip(start).Take(end-start+1).OrderByDescending(x => x.Count).First();
            return new Peak(highestPoint, leftBasePoint, rightBasePoint);
        }
    }
}
