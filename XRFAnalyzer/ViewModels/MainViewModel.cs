using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRFAnalyzer.Resources.Localization;

namespace XRFAnalyzer.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
        public SpectrumViewModel SpectrumViewModel => new SpectrumViewModel();
    }
}
