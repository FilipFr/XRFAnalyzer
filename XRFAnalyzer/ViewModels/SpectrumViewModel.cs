using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XRFAnalyzer.Models;
using XRFAnalyzer.ViewModels.Commands;

namespace XRFAnalyzer.ViewModels
{
    internal class SpectrumViewModel : BaseViewModel
    {
        public Load LoadSpectrum { get; set; }
        public SpectrumViewModel()
        {
            _spectrum = new Spectrum();
            LoadSpectrum = new Load(this);
        }
        private Spectrum _spectrum;

        public Spectrum Spectrum
        {
            get { return _spectrum; }
            set
            {
                _spectrum = value;
                OnPropertyChanged(nameof(Spectrum));
            }
        }

        public class Load : Command 
        {
            private SpectrumViewModel Vm;

            public Load (SpectrumViewModel vm) 
            {
                Vm = vm;
            }

            public override bool CanExecute(object? parameter)
            {
                return true;
            }
                
            public override void Execute(object? parameter)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "mca files (*.mca)|*.mca|All files (*.*)|*.*";

                string message = "";
                Spectrum lol = new Spectrum();
                if (lol != null && openFileDialog.ShowDialog() == true)
                {
                    lol.TryParseMca(openFileDialog.FileName, out message);
                    MessageBox.Show(message + " " + lol.Tests);
                    Vm.Spectrum = lol;
                }
            }
        }
    }
}
