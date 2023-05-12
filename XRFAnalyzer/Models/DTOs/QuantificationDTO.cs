using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XRFAnalyzer.Models.DTOs
{
    internal partial class QuantificationDTO : ObservableObject
    {
        [ObservableProperty]
        private List<double> _pCounts = new();
        [ObservableProperty]
        private int _intervalsPerChannel;
        [ObservableProperty]
        private double _pSlope;
        [ObservableProperty]
        private double _pIntercept;
        [ObservableProperty]
        private List<double> _peakAreas = new();
        [ObservableProperty]
        private List<double> _peakEnergies = new();
        [ObservableProperty]
        private List<double> _detectorEnergies = new();
        [ObservableProperty]
        private List<double> _detectorEfficiencies = new();
        [ObservableProperty]
        private List<double> _yields = new();
        [ObservableProperty]
        private List<double> _probabilities = new();
        [ObservableProperty]
        private List<double> _jumpRatios = new();
        [ObservableProperty]
        private List<List<double>> _coefficientEnergies = new();
        [ObservableProperty]
        private List<List<double>> _absorptionData = new();
        [ObservableProperty]
        private List<List<double>> _attenuationData = new();

        public string ValidationMessage()
        {
            string msg = "";
            if (this.PCounts.Count < 1)
                return "Cannot perform full analysis without primary spectrum.";
            if (this.PSlope == 0 || this.PSlope == Double.MaxValue) 
            {
                return "Cannot perform full analysis without calibration.";
            }
            if (this.PeakAreas.Count < 2) 
            {
                return "Analysis makes sense only for more than one identified peak";
            }
            if (this.DetectorEnergies.Count < 1) 
            {
                return "No detector selected.";
            }
            return msg;
        }
    }

    
}
