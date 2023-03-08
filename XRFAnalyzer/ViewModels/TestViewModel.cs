using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRFAnalyzer.Models;

namespace XRFAnalyzer.ViewModels
{
    internal class TestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TestModel Test
        {
            get { return Test; }
            set
            {
                Test = value;
                OnPropertyChanged(nameof(Test));
            }
        }
    }
}
