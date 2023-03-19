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
        private List<int>? _counts;
        [ObservableProperty]
        private int _lambda;
        [ObservableProperty]
        private int _iterationCount;

        public BackgroundDTO() 
        {
            _counts = new List<int>();
        }
    }
}
