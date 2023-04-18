using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRFAnalyzer.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace XRFAnalyzer.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
        [ObservableProperty]
        private ObservableObject _viewM;
        [ObservableProperty]
        private ObservableObject _sourceSpectrumViewModel;
        [ObservableProperty]
        private ObservableObject _characteristicSpectrumViewModel;

        public RelayCommand SwitchToSourceSpectrumViewModelCommand { get; set; }
        public RelayCommand SwitchToCharacteristicSpectrumViewModelCommand { get; set; }

        public MainViewModel() 
        {
            CharacteristicSpectrumViewModel = new SpectrumViewModel();
            ViewM = CharacteristicSpectrumViewModel;
            SourceSpectrumViewModel = new SpectrumViewModel();
            SwitchToSourceSpectrumViewModelCommand = new RelayCommand(SwitchToSourceSpectrumViewModel);
            SwitchToCharacteristicSpectrumViewModelCommand = new RelayCommand(SwitchToCharacteristicSpectrumViewModel);
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

    }
}
