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
        private List<double>? _counts;
        [ObservableProperty]
        private int _height;

        [ObservableProperty]
        private int _distance;
        [ObservableProperty]
        private int _prominence;

        [ObservableProperty]
        private int _wLen = 10;


        public FindPeaksDTO() 
        { 
            Counts = new(); 
        }

    }
}
