using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRFAnalyzer.Models.DTOs
{
    internal partial class FindPeaksDTO : ObservableObject
    {
        [ObservableProperty]
        private List<int>? _counts;
        [ObservableProperty]
        private int _height;
        [ObservableProperty]
        private int _threshold;
        [ObservableProperty]
        private int _distance;
        [ObservableProperty]
        private int _prominence;
        [ObservableProperty]
        private int _width;
        [ObservableProperty]
        private int _wLen;
        [ObservableProperty]
        private double _relHeight;
        [ObservableProperty]
        private int _plateauSize;

        public FindPeaksDTO() 
        { 
            Counts = new(); 
        }

    }
}
