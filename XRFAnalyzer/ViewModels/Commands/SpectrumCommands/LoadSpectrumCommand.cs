using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XRFAnalyzer.Models;


namespace XRFAnalyzer.ViewModels.Commands.SpectrumCommands
{
    internal class LoadSpectrumCommand : Command
    {
        private Spectrum _spectrum;
        public LoadSpectrumCommand(Spectrum spectrum)
        {
            _spectrum = spectrum;
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

            _spectrum = new Spectrum();
            string message = "";
            if (openFileDialog.ShowDialog() == true)
            {
                _ = Models.Spectrum.TryParseMca(openFileDialog.FileName, ref _spectrum, out message);
                MessageBox.Show(message + " " + _spectrum.Points[0].Count);
            }
        }
    }
}
