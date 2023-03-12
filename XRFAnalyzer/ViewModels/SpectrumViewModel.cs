using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XRFAnalyzer.Models;
using XRFAnalyzer.Models.DTOs;
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
        private List<Tuple<int, int>> _peaks;
        [ObservableProperty]
        private ObservableCollection<CalibrationRow> _calibrationRows;
        [ObservableProperty]
        private bool _isLoaded;
        [ObservableProperty]
        private bool _isCalibrated;
        [ObservableProperty]
        private bool _isLogarithmicToggled;
        [ObservableProperty]
        private List<Element> _elements;
        [ObservableProperty]
        private CalibrationRow _currentCalibrationRow;
        [ObservableProperty]
        private int _maxChannel;
        [ObservableProperty]
        private FindPeaksDTO _findPeaksDTO;

        public SpectrumViewModel()
        {
            Spectrum = new Spectrum();
            CurrentFile = "";
            Counts = Spectrum.Counts;
            Peaks = Spectrum.Peaks;
            CalibrationRows = new();
            CurrentCalibrationRow = new();
            IsLoaded = false;
            IsCalibrated = false;
            IsLogarithmicToggled = false;
            Load = new Command(() => LoadSpectrum());
            AddCalibrationPointCommand = new Command(() => AddCalibrationPoint());
            Elements = GetElementsData();
            MaxChannel = GetMaxChannel();
            FindPeaksDTO = new();
        }



        public ICommand Load { get; set; }
        public ICommand AddCalibrationPointCommand { get; set; }

        
        private void LoadSpectrum()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "mca files (*.mca)|*.mca|All files (*.*)|*.*";

            string message = "";
            if (Spectrum != null && openFileDialog.ShowDialog() == true)
            {
                bool result = Spectrum.TryParseMca(openFileDialog.FileName, out message);
                if (result)
                {
                    this.Counts = new(Spectrum.Counts);
                    MaxChannel = GetMaxChannel();
                    foreach(int channel in Spectrum.CalibrationPoints.Keys) 
                    {
                        this.CalibrationRows.Add(new (channel, Spectrum.CalibrationPoints[channel]));
                    }
                    this.Peaks = new(Spectrum.Peaks);
                    IsLoaded = true;
                }
                else
                {
                    MessageBox.Show(message);
                }
            }
        }

        private void AddCalibrationPoint() 
        {
            if(CurrentCalibrationRow != null && 
                CurrentCalibrationRow.Channel != null &&
                CurrentCalibrationRow.Energy != null) 
            {
                CalibrationRows.Add(CurrentCalibrationRow);
                CurrentCalibrationRow = new();
            }
        }

        public partial class CalibrationRow : ObservableObject
        {
            [ObservableProperty]
            private int _channel;
            [ObservableProperty]
            private double _energy;
            [ObservableProperty]
            private Element? _element;
       
            private EmissionLine? _emissionLine;

            public EmissionLine EmissionLine 
            {
                get { return _emissionLine; }
                set { _emissionLine = value;
                    OnPropertyChanged();
                    if (EmissionLine != null)
                    {
                        Energy = Math.Round(_emissionLine.Energy, 4);
                    }
                } 
            }

            public CalibrationRow() { }

            public CalibrationRow(int channel, double energy)  
            {
                Channel = channel;
                Energy = energy;
            }
        }
        
        private List<Element> GetElementsData() {
            List<EmissionLine>? lines = JsonConvert.DeserializeObject<List<EmissionLine>>(File.ReadAllText("Resources\\Data\\elements_lines.json"));
            List<Element>? elements = JsonConvert.DeserializeObject<List<Element>>(File.ReadAllText("Resources\\Data\\elements_info.json"));
            List<Element> toReturn = new List<Element>();
            foreach (Element element in elements)
            {
                element.PopulateEmissionLines(lines);
                if (element.EmissionLines.Count > 0) 
                {
                    toReturn.Add(element);
                }
            }
            return toReturn;
        }

        public int GetMaxChannel() 
        {
            if(Counts.Count > 0) 
            {
                return Counts.Count - 1;
            }
            return 0;
        }
    }
}
