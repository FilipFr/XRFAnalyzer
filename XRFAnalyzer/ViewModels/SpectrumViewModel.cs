using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XRFAnalyzer.Models;
using XRFAnalyzer.ViewModels.Commands;

namespace XRFAnalyzer.ViewModels
{
    internal partial class SpectrumViewModel : ObservableObject
    {
        [ObservableProperty]
        private Spectrum _spectrum;
        [ObservableProperty]
        private string _currentFile;
        [ObservableProperty]
        private List<int> _counts;
        [ObservableProperty]
        private List<Tuple<int,int>> _peaks;
        [ObservableProperty]
        private Dictionary<double, double> _calibrationPoints;
        [ObservableProperty]
        private bool _isLoaded;
        [ObservableProperty]
        private bool _isCalibrated;


        public SpectrumViewModel()
        {
            Spectrum = new Spectrum();
            CurrentFile = "";
            Counts = Spectrum.Counts;
            Peaks = Spectrum.Peaks;
            CalibrationPoints = Spectrum.CalibrationPoints;
            IsLoaded = false;
            IsCalibrated = false;
            Load = new Command(() => LoadSpectrum());
        }



        public ICommand Load { get; set; }
            
        private void LoadSpectrum() 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "mca files (*.mca)|*.mca|All files (*.*)|*.*";

            string message = "";
            if (Spectrum != null && openFileDialog.ShowDialog() == true)
            {
                bool result = Spectrum.TryParseMca(openFileDialog.FileName, out message);
                if(result) 
                {
                    this.Counts = new (Spectrum.Counts);
                    this.CalibrationPoints = new (Spectrum.CalibrationPoints);
                    this.Peaks = new (Spectrum.Peaks);
                    IsLoaded = true;
                } 
                else 
                {
                    MessageBox.Show(message);
                }
            }
        }
    }
}
