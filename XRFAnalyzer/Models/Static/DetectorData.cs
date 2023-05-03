using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models.Static
{
    internal partial class DetectorData : ObservableObject
    {
        [ObservableProperty]
        private List<Detector> _data = new();
        [ObservableProperty]
        private Detector _selectedDetector = new();


        public DetectorData() 
        {
            Data = Detector.LoadData("Resources\\Data\\detector_efficiency.json");
        }
    }
}
