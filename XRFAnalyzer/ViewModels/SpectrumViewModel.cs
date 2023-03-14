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
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XRFAnalyzer.Models;
using XRFAnalyzer.Models.DTOs;
using XRFAnalyzer.ViewModels.Commands;
using Grpc.Net.Client;
using Grpc.Core;

namespace XRFAnalyzer.ViewModels
{
    internal partial class SpectrumViewModel : ObservableObject
    {
        GrpcChannel channel;
        XRFAnalyzerService.XRFAnalyzerServiceClient client;

        [ObservableProperty]
        private Spectrum _spectrum;
        [ObservableProperty]
        private string _currentFile;
        [ObservableProperty]
        private List<int> _counts;
        [ObservableProperty]
        private List<Tuple<int, int>> _rois;
        [ObservableProperty]
        private List<Peak> _peaks;
        [ObservableProperty]
        private ObservableCollection<CalibrationRow> _calibrationRows;
        [ObservableProperty]
        private bool _isLoaded;
        [ObservableProperty]
        private bool _isCalibrated;
        [ObservableProperty]
        private bool _isLogarithmicToggled;
        [ObservableProperty]
        private bool _isPeakSelected;
        [ObservableProperty]
        private List<Element> _elements;
        [ObservableProperty]
        private CalibrationRow _currentCalibrationRow;
        [ObservableProperty]
        private int _maxChannel;
        [ObservableProperty]
        private Peak _selectedPeak;
        [ObservableProperty]
        private int _roiLeftBoundary = 0;
        [ObservableProperty]
        private int _roiRightBoundary = 0;

        
        private int _selectedPeakIndex;
        public int SelectedPeakIndex
        { 
            get { return _selectedPeakIndex; }
            set 
            { 
                _selectedPeakIndex = value;
                OnPropertyChanged();
                if (SelectedPeakIndex >= 0 && SelectedPeakIndex < Peaks.Count) 
                {
                    SelectedPeak = Peaks[SelectedPeakIndex];
                }
                if (SelectedPeakIndex == -1) 
                {
                    SelectedPeak = new Peak();
                    IsPeakSelected = false;
                }
            } }
        [ObservableProperty]
        private FindPeaksDTO _findPeaksDTO;


        public SpectrumViewModel()
        {
            channel = GrpcChannel.ForAddress("http://localhost:50051");
            client = new XRFAnalyzerService.XRFAnalyzerServiceClient(channel);
            Spectrum = new Spectrum();
            CurrentFile = "";
            Counts = Spectrum.Counts;
            Rois = Spectrum.Peaks;
            CalibrationRows = new();
            CurrentCalibrationRow = new();
            IsLoaded = false;
            IsCalibrated = false;
            IsLogarithmicToggled = false;
            Load = new Command(() => LoadSpectrum());
            AddCalibrationPointCommand = new Command(() => AddCalibrationPoint());
            RemoveSelectedPeakCommand = new Command(() => RemoveSelectedPeak());
            AddPeakCommand = new Command(() => AddPeak());
            GetFindPeaksMessageCommand = new Command(() => GetFindPeaksMessage());
            SelectedPeakIndex = -1;
            Elements = GetElementsData();
            MaxChannel = GetMaxChannel();
            FindPeaksDTO = new();
        }

        private void RemoveSelectedPeak()
        {
            if(SelectedPeakIndex > -1 && Peaks != null && SelectedPeakIndex < Peaks.Count) 
            {
                Peaks.RemoveAt(SelectedPeakIndex);
                Rois.RemoveAt(SelectedPeakIndex);
                SelectedPeakIndex = -1;
            }
            
        }
        private void AddPeak()
        {
            if (RoiRightBoundary > RoiLeftBoundary)
            {
                Tuple<int,int> roiToAdd = new Tuple<int,int>(RoiLeftBoundary,RoiRightBoundary);
                Peaks.Add(Peak.GetPeakFromRoi(Spectrum, roiToAdd));
                Rois.Add(roiToAdd);
                SelectedPeakIndex = Peaks.Count - 1;
            }
            else 
            {
                MessageBox.Show("Error: Peak should be wider than one channel.");
            }
        }

        public ICommand Load { get; set; }
        public ICommand AddCalibrationPointCommand { get; set; }
        public ICommand RemoveSelectedPeakCommand { get; set; }
        public ICommand AddPeakCommand { get; set; }
        public ICommand GetFindPeaksMessageCommand { get; set; }


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
                    FindPeaksDTO.Counts = new(Spectrum.Counts);
                    foreach(int channel in Spectrum.CalibrationPoints.Keys) 
                    {
                        this.CalibrationRows.Add(new (channel, Spectrum.CalibrationPoints[channel]));
                    }
                    this.Rois = new(Spectrum.Peaks);
                    this.Peaks = Peak.GetPeaksFromSpectrum(Spectrum, Rois);
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

        private void GetFindPeaksMessage() 
        {
            var reply = client.FindPeaksMessage(new FindPeaksRequest 
            { 
                Counts = { 1, 2, 3, 2, 1 }, 
                Height = 0, 
                Threshold = 0,
                Distance = 0,
                Prominence = 0,
                Width = 0,
                Wlen = 0,
                RelHeight = 0,
                PlateauSize = 0 
            });
            MessageBox.Show(reply.ToString());
        }
    }
}
