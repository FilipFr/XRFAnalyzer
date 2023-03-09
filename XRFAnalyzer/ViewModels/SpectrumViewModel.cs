using CommunityToolkit.Mvvm.ComponentModel;
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
    internal partial class SpectrumViewModel : ObservableObject
    {
        [ObservableProperty]
        private Spectrum _spectr;

        public SpectrumViewModel()
        {
            _spectr = new Spectrum();
            Load = new Command(() => LoadSpectrum());
        }



        public ICommand Load { get; set; }
            
        private void LoadSpectrum() 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "mca files (*.mca)|*.mca|All files (*.*)|*.*";

            string message = "";
            if (Spectr != null && openFileDialog.ShowDialog() == true)
            {
                Spectr.TryParseMca(openFileDialog.FileName, out message);
                MessageBox.Show(message + " " + Spectr.Tests);
            }
        }
    }
}
