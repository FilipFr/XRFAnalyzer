using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRFAnalyzer.Resources.Localization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XRFAnalyzer.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
        [ObservableProperty]
        private ObservableObject _viewM;

        public MainViewModel() 
        {
            ViewM = new SpectrumViewModel();
        }
    }
}
