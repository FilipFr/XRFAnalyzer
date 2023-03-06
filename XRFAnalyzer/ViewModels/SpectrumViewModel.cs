using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XRFAnalyzer.Models;
using XRFAnalyzer.ViewModels.Commands;
using XRFAnalyzer.ViewModels.Commands.SpectrumCommands;

namespace XRFAnalyzer.ViewModels
{
    internal class SpectrumViewModel : BaseViewModel
    {
        public LoadSpectrumCommand LoadSpectrumCommand { get; set; }
        public SpectrumViewModel()
        {
            _spectrum = new Spectrum();
            LoadSpectrumCommand = new LoadSpectrumCommand(_spectrum);
        }
        private Spectrum _spectrum = new();

        public Spectrum Spectrum
        {
            get { return _spectrum; }
            set
            {
                _spectrum = value;
                OnPropertyChanged();
            }
        }
        
    }
}
