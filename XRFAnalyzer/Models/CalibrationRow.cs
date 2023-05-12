using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Grpc.Core.Metadata;

namespace XRFAnalyzer.Models
{
    internal partial class CalibrationRow : ObservableObject
    {
        [ObservableProperty]
        private double _channel;
        [ObservableProperty]
        private double _energy;
        [ObservableProperty]
        private Element? _element;

        private EmissionLine? _emissionLine;

        public EmissionLine EmissionLine
        {
            get { return _emissionLine; }
            set
            {
                _emissionLine = value;
                OnPropertyChanged();
                if (EmissionLine != null)
                {
                    Energy = Math.Round(_emissionLine.Energy, 4);
                }
            }
        }

        public CalibrationRow() { }

        public CalibrationRow(double channel, double energy)
        {
            Channel = channel;
            Energy = energy;
        }
    }
}
