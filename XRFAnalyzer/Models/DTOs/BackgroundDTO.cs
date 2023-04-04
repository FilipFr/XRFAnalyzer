using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models.DTOs
{
    internal partial class BackgroundDTO : ObservableObject
    {
        [ObservableProperty]
        private List<double>? _counts;
        [ObservableProperty]
        private int _lambda = 100;
        [ObservableProperty]
        private int _iterationCount = 15;

        public BackgroundDTO() 
        {
            Counts = new();
        }
    }
}
