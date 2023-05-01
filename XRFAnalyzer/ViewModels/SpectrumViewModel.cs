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
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.Input;
using MathNet.Numerics;
using Google.Protobuf.Collections;
using XRFAnalyzer.Models.Static;

namespace XRFAnalyzer.ViewModels
{
    internal partial class SpectrumViewModel : ObservableObject
    {
        GrpcChannel channel;
        XRFAnalyzerService.XRFAnalyzerServiceClient client;
        ElementData elementDataReference;

        [ObservableProperty]
        private Spectrum _spectrum;
        [ObservableProperty]
        private string _currentFile;
        [ObservableProperty]
        private List<double> _counts;
        [ObservableProperty]
        private List<Tuple<int, int>> _rois;
        [ObservableProperty]
        private List<Peak> _peaks = new();
        [ObservableProperty]
        private bool _isLoaded;
        [ObservableProperty]
        private bool _isCalibrated;
        [ObservableProperty]
        private bool _isLogarithmicToggled;
        [ObservableProperty]
        private bool _isXAxisUnitToggled;
        [ObservableProperty]
        private bool _isPeakSelected;
        [ObservableProperty]
        private List<Detector> _detectors;
        [ObservableProperty]
        private List<DetectorGrouped> _detectorsGrouped;
        [ObservableProperty]
        private DetectorGrouped _currentDetector;
        [ObservableProperty]
        private ObservableCollection<CalibrationRow> _calibrationRows;
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
        [ObservableProperty]
        private List<Tuple<int, int>> _foundRois;
        [ObservableProperty]
        private ObservableCollection<Tuple<int, double>> _calibrationPoints;
        [ObservableProperty]
        private List<double> _correctedCounts;
        [ObservableProperty]
        private bool _isBackgroundRemoved = false;
        [ObservableProperty]
        private ObservableCollection<Peak> _sumPeaks;
        [ObservableProperty]
        private double _calibrationCurveSlope = Double.MaxValue;
        [ObservableProperty]
        private double _calibrationCurveIntercept = Double.MaxValue;
        [ObservableProperty]
        private Yields _yields = new();
        [ObservableProperty]
        private JumpRatios _jumpRatioss = new(); 
        [ObservableProperty]
        private MassCoefficients _massCoefficientss = new();

        private int _calibrationSwitch;
        public int CalibrationSwitch 
        {
            get { return _calibrationSwitch; }
            set 
            { 
                _calibrationSwitch = value;
                OnPropertyChanged();
                GetCalibrationCurveParameters();
                if (CalibrationRows != null && CalibrationRows.Count > 1)
                {
                    CalibrationPoints = new();
                    foreach (var row in CalibrationRows) 
                    {
                        _calibrationPoints.Add(new(row.Channel, row.Energy));
                    }
                }
            }
        }
        
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
        [ObservableProperty]
        private BackgroundDTO _backgroundDTO;


        public SpectrumViewModel(ElementData elementData)
        {
            channel = GrpcChannel.ForAddress("http://localhost:50051");
            client = new XRFAnalyzerService.XRFAnalyzerServiceClient(channel);
            elementDataReference = elementData;
            Spectrum = new Spectrum();
            CurrentFile = "No data";
            Counts = Spectrum.Counts;
            Rois = Spectrum.Peaks;
            CalibrationRows = new();
            SumPeaks = new();
            CalibrationRows.CollectionChanged += OnCollectionChanged;
            SumPeaks.CollectionChanged += OnSumPeaksDeleted;
            CurrentCalibrationRow = new();
            IsLoaded = false;
            IsCalibrated = false;
            IsLogarithmicToggled = false;
            Load = new Command(() => LoadSpectrum());
            AddCalibrationPointCommand = new Command(() => AddCalibrationPoint());
            RemoveSelectedPeakCommand = new Command(() => RemoveSelectedPeak());
            AddPeakCommand = new Command(() => AddPeak());
            GetFindPeaksMessageCommand = new Command(() => GetFindPeaksMessage());
            AddFoundPeaksCommand = new Command(() => AddFoundPeaks());
            GetCorrectedCountsCommand = new Command(() => GetBackgroundMessage());
            RemoveBackgroundCommand = new RelayCommand(RemoveBackground, CanRemoveBackground);
            UndoBackgroundRemovalCommand = new RelayCommand(UndoBackgroundRemoval, CanUndoBackgroundRemoval);
            ShowCommand = new Command(() => Show());
            SelectedPeakIndex = -1;
            Detectors = Detector.LoadData("Resources\\Data\\detector_efficiency.json");
            Yields.Deserialize("Resources\\Data\\fluorescent_yield.json");
            JumpRatioss.Deserialize("Resources\\Data\\jump_ratio.json");
            MassCoefficientss.Deserialize("Resources\\Data\\xray_mass_ceoficient.json");
            DetectorsGrouped = new();
            DetectorsGrouped = DetectorGrouped.GroupDetectorsByName(Detectors);
            CurrentDetector = new();
            MaxChannel = GetMaxChannel();
            FindPeaksDTO = new();
            BackgroundDTO = new();
            FoundRois = new();
            CalibrationPoints = new();
            CorrectedCounts = new();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalibrationPoints.Clear();
            foreach (var row in CalibrationRows)
            {
                CalibrationPoints.Add(new(row.Channel, row.Energy));
            }
            GetCalibrationCurveParameters();
            OnPropertyChanged(nameof(CalibrationPoints));
        }

        private void OnSumPeaksDeleted(object sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < Peaks.Count; i++) 
            {
                if (Peaks[i].CanBeSumPeak) 
                {
                    if (!SumPeaks.Any(peak => Peaks[i] == peak)) 
                    {
                        Peaks.RemoveAt(i);
                        Rois.RemoveAt(i);
                        int temp = SelectedPeakIndex;
                        SelectedPeakIndex = Int32.MaxValue;
                        SelectedPeakIndex = temp;
                        return;
                    }
                }
            }
        }

        private void RemoveSelectedPeak()
        {
            if(SelectedPeakIndex > -1 && Peaks != null && SelectedPeakIndex < Peaks.Count) 
            {
                Peaks.RemoveAt(SelectedPeakIndex);
                Rois.RemoveAt(SelectedPeakIndex);
                GetSumPeaks();
                SelectedPeakIndex = -1;
            }
            
        }
        private void AddPeak()
        {
            if (RoiRightBoundary > RoiLeftBoundary)
            {
                Tuple<int,int> roiToAdd = new Tuple<int,int>(RoiLeftBoundary,RoiRightBoundary);
                Peaks.Add(Peak.GetPeakFromRoi(Counts, roiToAdd));
                Rois.Add(roiToAdd);
                GetSumPeaks();
                CalculatePeakAreas();
                CalibratePeaks();
                SelectedPeakIndex = Peaks.Count - 1;
            }
            else 
            {
                MessageBox.Show("Error: Left base channel cannot be greater or equal to right base channel");
            }
        }

        public ICommand Load { get; set; }
        public ICommand AddCalibrationPointCommand { get; set; }
        public ICommand RemoveSelectedPeakCommand { get; set; }
        public ICommand AddPeakCommand { get; set; }
        public ICommand GetFindPeaksMessageCommand { get; set; }
        public ICommand AddFoundPeaksCommand { get; set; }
        public ICommand GetCorrectedCountsCommand { get; set; }
        public RelayCommand RemoveBackgroundCommand { get; set; }
        public RelayCommand UndoBackgroundRemovalCommand { get; set; }
        public ICommand ShowCommand { get; set; }

        
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
                    CalibrationPoints.Clear();
                    CalibrationRows.Clear();
                    MaxChannel = GetMaxChannel();
                    FindPeaksDTO.Counts = Spectrum.Counts;
                    BackgroundDTO.Counts = Spectrum.Counts;
                    GetBackgroundMessage();
                    foreach(int channel in Spectrum.CalibrationPoints.Keys) 
                    {
                        this.CalibrationRows.Add(new (channel, Spectrum.CalibrationPoints[channel]));
                    }
                    GetCalibrationCurveParameters();
                    this.Rois = new(Spectrum.Peaks);
                    this.Peaks = Peak.GetPeaksFromSpectrum(Counts, Rois);
                    GetSumPeaks();
                    CalculatePeakAreas();
                    CalibratePeaks();
                    IsLoaded = true;
                    IsBackgroundRemoved = false;
                    RemoveBackgroundCommand.NotifyCanExecuteChanged();
                    UndoBackgroundRemovalCommand.NotifyCanExecuteChanged();
                    CurrentFile = openFileDialog.FileName.Split("\\").Last();
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

        private void GetCalibrationCurveParameters() 
        {
            if (CalibrationRows.Count > 1) 
            {
                double[] xData = CalibrationRows.Select(x => (double)x.Channel).ToArray();
                double[] yData = CalibrationRows.Select(x => x.Energy).ToArray();
                (CalibrationCurveIntercept, CalibrationCurveSlope) = Fit.Line(xData, yData);
                IsCalibrated = true;
                CalibratePeaks();
            } 
            else 
            {
                (CalibrationCurveIntercept, CalibrationCurveSlope) = (Double.MaxValue, Double.MaxValue);
                IsCalibrated = false;
                CalibratePeaks();
            }
        }

        public partial class DetectorGrouped : ObservableObject
        {
            public string Name { get; set; } = "";
            public List<Detector> Configurations { get; set; } = new();
            public string SelectedWindow { get; set; } = "";

            public DetectorGrouped() 
            {
            }

            public static List<DetectorGrouped> GroupDetectorsByName(List<Detector> detectors) 
            {
                var grouped = detectors.GroupBy(x =>  x.Name).OrderBy(x=> x.Key);
                List<DetectorGrouped> toReturn = new();
                foreach(var detector in grouped) 
                {
                    DetectorGrouped item = new();
                    item.Name = detector.Key;
                    foreach(var config in detector) 
                    {
                        item.Configurations.Add(config);
                    }
                    toReturn.Add(item);
                }
                return toReturn;
            }
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
                Counts = { FindPeaksDTO.Counts },
                Height = FindPeaksDTO.Height,
                Threshold = FindPeaksDTO.Threshold,
                Distance = FindPeaksDTO.Distance,
                Prominence = FindPeaksDTO.Prominence,
                Width = FindPeaksDTO.Width,
                Wlen = FindPeaksDTO.WLen,
                RelHeight = FindPeaksDTO.RelHeight,
                PlateauSize = FindPeaksDTO.PlateauSize
            });
            if (reply != null
                && reply.Peaks != null
                && reply.LeftBases != null
                && reply.RightBases != null
                && reply.Peaks.Count > 0
                && reply.RightBases.Count == reply.LeftBases.Count
                && reply.Peaks.Count == reply.LeftBases.Count) 
            {
                FoundRois = new();
                List<Tuple<int,int>> roisToAdd = new();
                for (int i = 0; i < reply.Peaks.Count; i++) 
                {
                    roisToAdd.Add(new(reply.LeftBases[i], reply.RightBases[i]));
                    SelectedPeakIndex = Peaks.Count;
                }
                SelectedPeakIndex = -1;
                FoundRois = roisToAdd;
            }
        }

        private void GetBackgroundMessage() 
        {
            var reply = client.BackgroundMessage(new BackgroundRequest
            {
                Counts = { BackgroundDTO.Counts },
                Lambda = BackgroundDTO.Lambda,
                Iterations = BackgroundDTO.IterationCount
            });
            if (reply != null) 
            {
                List<double> CorrCounts = new();
                for (int i = 0; i < reply.CorrectedCounts.Count; i++) 
                {
                    double toAdd = (reply.CorrectedCounts[i] < 0) ? 0 : reply.CorrectedCounts[i];
                    CorrCounts.Add(toAdd);
                }
                CorrectedCounts = new(CorrCounts.Select(x => x).ToList());
                RemoveBackgroundCommand.NotifyCanExecuteChanged();
                UndoBackgroundRemovalCommand.NotifyCanExecuteChanged();
            }
        }

        public void AddFoundPeaks() 
        {
            if (FoundRois == null || FoundRois.Count == 0) 
            {
                return;
            }
            foreach (Tuple<int,int> roi in FoundRois) 
            {
                if (!Rois.Contains(roi)) 
                { 
                    Rois.Add(roi);
                    Peaks.Add(Peak.GetPeakFromRoi(Counts, roi));
                    CalculatePeakAreas();
                    CalibratePeaks();
                }
            }
            FoundRois.Clear();
            SelectedPeakIndex = -999;
            SelectedPeakIndex = -1;
        }

        private void CalculatePeakAreas() 
        {
            foreach(Peak peak in Peaks) 
            {
                peak.CalculateAreas(Counts, CorrectedCounts);
            }
        }

        private void RemoveBackground() 
        {
            Counts = new(CorrectedCounts);
            IsBackgroundRemoved = true;
            Peaks = Peak.GetPeaksFromSpectrum(Counts, Rois);
            CalculatePeakAreas();
            RemoveBackgroundCommand.NotifyCanExecuteChanged();
            UndoBackgroundRemovalCommand.NotifyCanExecuteChanged();
        }

        private bool CanRemoveBackground() 
        {
            if(Counts == null || Counts.Count == 0) 
            {
                return false;
            }
            if (CorrectedCounts == null || CorrectedCounts.Count == 0)
            {
                return false;
            }
            if (Counts.Count != CorrectedCounts.Count) 
            {
                return false;
            }
            return !IsBackgroundRemoved;
        }

        private void UndoBackgroundRemoval() 
        {
            Counts = new(Spectrum.Counts);
            GetBackgroundMessage();
            IsBackgroundRemoved = false;
            Peaks = Peak.GetPeaksFromSpectrum(Counts, Rois);
            CalculatePeakAreas();
            RemoveBackgroundCommand.NotifyCanExecuteChanged();
            UndoBackgroundRemovalCommand.NotifyCanExecuteChanged();

        }

        private bool CanUndoBackgroundRemoval()
        {
            if (Counts == null || Counts.Count == 0)
            {
                return false;
            }
            if (CorrectedCounts == null || CorrectedCounts.Count == 0)
            {
                return false;
            }
            if (Counts.Count != CorrectedCounts.Count)
            {
                return false;
            }
            return IsBackgroundRemoved;
        }

        private void GetSumPeaks()
        {
            SumPeaks = new();
            SumPeaks.CollectionChanged += OnSumPeaksDeleted;
            foreach (Peak peak in Peaks)
            {
                peak.DetermineIfSumPeak(Peaks);
                if (peak.CanBeSumPeak)
                {
                    SumPeaks.Add(peak);
                }
            }
        }


        private void Show() 
        {
            foreach(Peak peak in Peaks) 
            {
                if (peak.ConfirmedEmissionLine == null)
                {
                    MessageBox.Show(peak.ApexChannel.ToString());
                }
            }
        }

        private void CalibratePeaks() 
        {
            if (CalibrationCurveSlope != Double.MaxValue && CalibrationCurveIntercept != Double.MaxValue)
            {
                foreach (Peak peak in Peaks)
                {
                    peak.ApexEnergy = peak.ApexChannel * CalibrationCurveSlope + CalibrationCurveIntercept;
                    peak.EnergyRange = new(peak.ChannelRange.Item1 * CalibrationCurveSlope + CalibrationCurveIntercept,
                        peak.ChannelRange.Item2 * CalibrationCurveSlope + CalibrationCurveIntercept);
                    peak.FindPotentialEmissionLines(elementDataReference.Data, CalibrationRows, 0.05);
                }
            }
            else
            {
                foreach (Peak peak in Peaks)
                {
                    peak.ApexEnergy = 0;
                    peak.EnergyRange = new(0,0);
                }
            }
        }
    }
}
