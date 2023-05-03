using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRFAnalyzer.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XRFAnalyzer.Models;
using XRFAnalyzer.Models.DTOs;
using System.Windows;
using Newtonsoft.Json;
using Google.Protobuf.Collections;

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

        public RelayCommand SwitchToSourceSpectrumViewModelCommand { get; set; }
        public RelayCommand SwitchToCharacteristicSpectrumViewModelCommand { get; set; }
        public RelayCommand QuantitativeAnalysisCommand { get; set; }

        public MainViewModel() 
        {
            ElementData = new Models.Static.ElementData();
            DetectorData = new Models.Static.DetectorData();
            CharacteristicSpectrumViewModel = new SpectrumViewModel(ElementData);
            ViewM = CharacteristicSpectrumViewModel;
            SourceSpectrumViewModel = new SpectrumViewModel(ElementData);
            SwitchToSourceSpectrumViewModelCommand = new RelayCommand(SwitchToSourceSpectrumViewModel);
            SwitchToCharacteristicSpectrumViewModelCommand = new RelayCommand(SwitchToCharacteristicSpectrumViewModel);
            QuantitativeAnalysisCommand = new RelayCommand(QuantitativeAnalysis);
        }

        private void SwitchToSourceSpectrumViewModel() 
        {
            if (SourceSpectrumViewModel != null && ViewM != SourceSpectrumViewModel) 
            {
                ViewM = SourceSpectrumViewModel;
            }
        }

        private void SwitchToCharacteristicSpectrumViewModel() 
        {
            if (CharacteristicSpectrumViewModel != null && ViewM != CharacteristicSpectrumViewModel) 
            {
                ViewM = CharacteristicSpectrumViewModel;
            }
        }

        private void QuantitativeAnalysis() 
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

            var reply = CharacteristicSpectrumViewModel.client.QuantificationMessage(new QuantificationRequest
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
                }
            }
            MessageBox.Show(report.ToString());
            
        }

    }
}
