using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRFAnalyzer.Models;
using XRFAnalyzer.Models.Static;

namespace XRFAnalyzer.ViewModels
{
    internal partial class AnalysisViewModel : ObservableObject
    {
        private List<Peak> selectedPeaks;
        private List<double> _primarySpectrumCounts;
        private List<double> _intervalsPerChannel;
        private List<double> _intervalsPerPeak;

    }
}
