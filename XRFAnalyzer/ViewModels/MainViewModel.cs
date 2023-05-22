using System;
using System.Collections.Generic;
using System.Linq;
using XRFAnalyzer.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XRFAnalyzer.Models;
using XRFAnalyzer.Models.DTOs;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace XRFAnalyzer.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
        [ObservableProperty]
        private ObservableObject _viewM;
        [ObservableProperty]
        private Models.Static.ElementData _elementData;
        [ObservableProperty]
        private Models.Static.DetectorData _detectorData;
        [ObservableProperty]
        private SpectrumViewModel _sourceSpectrumViewModel;
        [ObservableProperty]
        private SpectrumViewModel _characteristicSpectrumViewModel;
        [ObservableProperty]
        private List<double> _results = new();
        [ObservableProperty]
        private List<string> _symbols = new();
        [ObservableProperty]
        private double _energy = new();

        public RelayCommand SwitchSpectrumViewModelCommand { get; set; }
        public RelayCommand SwitchToCharacteristicSpectrumViewModelCommand { get; set; }
        public RelayCommand SwitchToSourceSpectrumViewModelCommand { get; set; }
        public RelayCommand QuantitativeAnalysisCommand { get; set; }
        public RelayCommand MonoenergeticQuantitativeAnalysisCommand { get; set; }
        public RelayCommand ShowAboutCommand { get; set; }

        public MainViewModel() 
        {
            ElementData = new Models.Static.ElementData();
            DetectorData = new Models.Static.DetectorData();
            CharacteristicSpectrumViewModel = new SpectrumViewModel(ElementData);
            ViewM = CharacteristicSpectrumViewModel;
            SourceSpectrumViewModel = new SpectrumViewModel(ElementData);
            SwitchSpectrumViewModelCommand = new RelayCommand(SwitchSpectrumViewModel, CanSwitchSpectrumViewModel);
            SwitchToCharacteristicSpectrumViewModelCommand = new RelayCommand(SwitchToCharacteristicSpectrumViewModel);
            SwitchToSourceSpectrumViewModelCommand = new RelayCommand(SwitchToSourceSpectrumViewModel);
            QuantitativeAnalysisCommand = new RelayCommand(QuantitativeAnalysis);
            MonoenergeticQuantitativeAnalysisCommand = new RelayCommand(MonoenergeticQuantitativeAnalysis);
            ShowAboutCommand = new RelayCommand(ShowAbout);
        }

        private void SwitchSpectrumViewModel() 
        {
            if(ViewM != null && ViewM != SourceSpectrumViewModel) 
            {
                ViewM = SourceSpectrumViewModel;
            } else if (ViewM != null && ViewM != CharacteristicSpectrumViewModel) 
            {
                ViewM = CharacteristicSpectrumViewModel;
            }
        }

        private void SwitchToCharacteristicSpectrumViewModel()
        {
            if (ViewM != null && ViewM != CharacteristicSpectrumViewModel)
            {
                ViewM = CharacteristicSpectrumViewModel;
            }
        }

        private void SwitchToSourceSpectrumViewModel() 
        {
            if (ViewM != null && ViewM != SourceSpectrumViewModel)
            {
                ViewM = SourceSpectrumViewModel;
            }
        }


        private bool CanSwitchSpectrumViewModel() 
        {
            return true;
        }


        private async void QuantitativeAnalysis() 
        {
            QuantificationDTO dto = new();

            dto.PCounts = SourceSpectrumViewModel.Counts;
            dto.IntervalsPerChannel = 10;
            dto.PSlope = SourceSpectrumViewModel.CalibrationCurveSlope; 
            dto.PIntercept = SourceSpectrumViewModel.CalibrationCurveIntercept;
            dto.DetectorEnergies = DetectorData.SelectedDetector.Energies;
            dto.DetectorEfficiencies = DetectorData.SelectedDetector.TotalAttenuations;
            
            List<int> elements = new List<int>();
            List<Peak> sortedPeaks = (List<Peak>)CharacteristicSpectrumViewModel.Peaks.OrderByDescending(x=>x.NetArea).ToList();
            foreach (Peak peak in sortedPeaks) 
            {
                if (peak.CurrentEmissionLine != null)
                {    
                    if (elements.Contains(peak.CurrentEmissionLine.Number)) 
                    {
                        continue;
                    }
                    Symbols.Add(peak.CurrentEmissionLine.ElementSymbol);
                    elements.Add(peak.CurrentEmissionLine.Number);
                    dto.PeakAreas.Add(peak.NetArea);
                    dto.PeakEnergies.Add(peak.CurrentEmissionLine.Energy);
                    dto.Yields.Add(peak.CurrentEmissionLine.Yield);
                    dto.Probabilities.Add(peak.CurrentEmissionLine.TransitionProbability);
                    dto.JumpRatios.Add(peak.CurrentEmissionLine.JumpRatio);
                }
            }
            foreach (int value in elements) 
            {
                foreach (Element element in ElementData.Data) 
                {
                    if (value == element.Number) 
                    {
                        dto.CoefficientEnergies.Add(element.MassCoefficientEnergies);
                        dto.AbsorptionData.Add(element.MassAbsorptionCoefficients);
                        dto.AttenuationData.Add(element.MassAttenuationCoefficients);
                    }
                }
            }

            List<NumericalData> coeffEnergies = new();
            foreach(IEnumerable<double> values in dto.CoefficientEnergies ) 
            {
                coeffEnergies.Add(new NumericalData { Data = { values } });
            }
            List<NumericalData> coeffAbsorption = new();
            foreach (IEnumerable<double> values in dto.AbsorptionData)
            {
                coeffAbsorption.Add(new NumericalData { Data = { values } });
            }
            List<NumericalData> coeffAttenuation = new();
            foreach (IEnumerable<double> values in dto.AttenuationData)
            {
                coeffAttenuation.Add(new NumericalData { Data = { values } });
            }
            if (dto.ValidationMessage() != "")
            {
                MessageBox.Show(dto.ValidationMessage());
                return;
            }
            var reply = await CharacteristicSpectrumViewModel.client.QuantificationMessageAsync(new QuantificationRequest
            {
                PCounts = { dto.PCounts },
                IntervalsPerChannel = dto.IntervalsPerChannel,
                PSlope = dto.PSlope,
                PIntercept = dto.PIntercept,
                PeakAreas = { dto.PeakAreas },
                PeakEnergies = { dto.PeakEnergies },
                DetectorEnergies = { dto.DetectorEnergies },
                DetectorEfficiencies = { dto.DetectorEfficiencies },
                Yields = { dto.Yields },
                Probabilities = { dto.Probabilities },
                JumpRatios = { dto.JumpRatios },
                CoefficientEnergies = { coeffEnergies },
                AbsorptionData = { coeffAbsorption },
                AttenuationData = { coeffAttenuation },

            });
            string report = "";
            if (elements.Count == reply.Concentrations.Count) 
            {
                for (int i=0; i<elements.Count; i++) 
                {
                    var symbol = ElementData.Data.FirstOrDefault(x => x.Number == elements[i]);
                    report += symbol +": " + reply.Concentrations[i].ToString() + "\n";
                    this.Results.Add(reply.Concentrations[i]);
                }
            }
            ExportResults();
            MessageBox.Show(report.ToString());
        }

        private void ShowAbout() 
        {
            MessageBox.Show("XRFAnalyzer v0.1");
        }

        private async void MonoenergeticQuantitativeAnalysis()
        {
            QuantificationDTO dto = new();
            Symbols = new();
            dto.PCounts = new List<double>(){ Energy, Energy};
            dto.IntervalsPerChannel = 1;
            dto.PSlope = 1;
            dto.PIntercept = 0;
            dto.DetectorEnergies = DetectorData.SelectedDetector.Energies;
            dto.DetectorEfficiencies = DetectorData.SelectedDetector.TotalAttenuations;

            List<int> elements = new List<int>();
            List<Peak> sortedPeaks = (List<Peak>)CharacteristicSpectrumViewModel.Peaks.OrderByDescending(x => x.NetArea).ToList();
            foreach (Peak peak in sortedPeaks)
            {
                if (peak.CurrentEmissionLine != null)
                {
                    if (elements.Contains(peak.CurrentEmissionLine.Number))
                    {
                        continue;
                    }
                    Symbols.Add(peak.CurrentEmissionLine.ElementSymbol);
                    elements.Add(peak.CurrentEmissionLine.Number);
                    dto.PeakAreas.Add(peak.NetArea);
                    dto.PeakEnergies.Add(peak.CurrentEmissionLine.Energy);
                    dto.Yields.Add(peak.CurrentEmissionLine.Yield);
                    dto.Probabilities.Add(peak.CurrentEmissionLine.TransitionProbability);
                    dto.JumpRatios.Add(peak.CurrentEmissionLine.JumpRatio);
                }
            }
            foreach (int value in elements)
            {
                foreach (Element element in ElementData.Data)
                {
                    if (value == element.Number)
                    {
                        dto.CoefficientEnergies.Add(element.MassCoefficientEnergies);
                        dto.AbsorptionData.Add(element.MassAbsorptionCoefficients);
                        dto.AttenuationData.Add(element.MassAttenuationCoefficients);
                    }
                }
            }

            List<NumericalData> coeffEnergies = new();
            foreach (IEnumerable<double> values in dto.CoefficientEnergies)
            {
                coeffEnergies.Add(new NumericalData { Data = { values } });
            }
            List<NumericalData> coeffAbsorption = new();
            foreach (IEnumerable<double> values in dto.AbsorptionData)
            {
                coeffAbsorption.Add(new NumericalData { Data = { values } });
            }
            List<NumericalData> coeffAttenuation = new();
            foreach (IEnumerable<double> values in dto.AttenuationData)
            {
                coeffAttenuation.Add(new NumericalData { Data = { values } });
            }
            if (dto.ValidationMessage() != "")
            {
                MessageBox.Show(dto.ValidationMessage());
                return;
            }
            var reply = await CharacteristicSpectrumViewModel.client.QuantificationMessageAsync(new QuantificationRequest
            {
                PCounts = { dto.PCounts },
                IntervalsPerChannel = dto.IntervalsPerChannel,
                PSlope = dto.PSlope,
                PIntercept = dto.PIntercept,
                PeakAreas = { dto.PeakAreas },
                PeakEnergies = { dto.PeakEnergies },
                DetectorEnergies = { dto.DetectorEnergies },
                DetectorEfficiencies = { dto.DetectorEfficiencies },
                Yields = { dto.Yields },
                Probabilities = { dto.Probabilities },
                JumpRatios = { dto.JumpRatios },
                CoefficientEnergies = { coeffEnergies },
                AbsorptionData = { coeffAbsorption },
                AttenuationData = { coeffAttenuation },

            });
            string report = "";
            if (elements.Count == reply.Concentrations.Count)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    var symbol = ElementData.Data.FirstOrDefault(x => x.Number == elements[i]);
                    report += symbol + ": " + reply.Concentrations[i].ToString() + "\n";
                    this.Results.Add(reply.Concentrations[i]);
                }
            }
            ExportResults();
            MessageBox.Show(report.ToString());
        }

        private void ExportResults() 
        {
            string result =
                "ID".PadRight(20, ' ')
                + "Centroid (keV)".PadRight(20, ' ')
                + "FWHM (keV)".PadRight(20, ' ')
                + "Net area (counts)".PadRight(20, ' ')
                + "Gross area (counts)".PadRight(20, ' ')
                + "Line".PadRight(20, ' ')
                + "Line energy (keV)".PadRight(20, ' ')
                + "Mass concentration (%)\n";
            int counter = 0;
            for (int i = 0; i < this.CharacteristicSpectrumViewModel.Peaks.Count; i++) 
            {
                result += 
                    i.ToString().PadRight(20, ' ')
                    + (CharacteristicSpectrumViewModel.Peaks[i].Centroid*CharacteristicSpectrumViewModel.CalibrationCurveSlope + CharacteristicSpectrumViewModel.CalibrationCurveIntercept).ToString().PadRight(20, ' ') 
                    + CharacteristicSpectrumViewModel.Peaks[i].Fwhm.ToString().PadRight(20, ' ')
                    + CharacteristicSpectrumViewModel.Peaks[i].NetArea.ToString().PadRight(20, ' ')
                    + CharacteristicSpectrumViewModel.Peaks[i].GrossArea.ToString().PadRight(20, ' ')
                    + CharacteristicSpectrumViewModel.Peaks[i].CurrentEmissionLine.ToString().PadRight(20, ' ')
                    + CharacteristicSpectrumViewModel.Peaks[i].CurrentEmissionLine.Energy.ToString().PadRight(20, ' ')
                    ;
                for (int j = 0; j < Symbols.Count; j++) 
                {
                    if (CharacteristicSpectrumViewModel.Peaks[i].CurrentEmissionLine.ElementSymbol == Symbols[j]) 
                    {
                        result = result + Math.Round(Results[j]*100, 2).ToString();
                    }
                }
                result += "\n";
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, result);

        }

    }
}
