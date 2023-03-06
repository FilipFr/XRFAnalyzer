using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal class Peak
    {
        public SpectrumPoint HighestPoint { get; set; }

        public SpectrumPoint LeftBasePoint { get; set; }

        public SpectrumPoint RightBasePoint { get; set; }

        public double? GrossArea { get; set; }

        public double? NetArea { get; set; }

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
