using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models
{
    internal partial class SpectrumState : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoaded = false;
        [ObservableProperty]
        private bool _isCalibrated = false;
    }
}
