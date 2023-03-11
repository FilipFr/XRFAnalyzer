﻿using Grpc.Net.Client;
using Microsoft.Win32;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRFAnalyzer.Resources.Localization;
using XRFAnalyzer.ViewModels;

namespace XRFAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo culture = 
                Strings.Culture.TwoLetterISOLanguageName.Equals("sk", StringComparison.InvariantCultureIgnoreCase) ?
                new CultureInfo("en-US") : new CultureInfo("sk");
            LocalizationResourceManager.Instance.SetCulture(culture);

            PlotControl.SpectrumWpfPlot.Render();
        }

        private void Load_Click(object sender, RoutedEventArgs e) 
        {
            //using var channel = GrpcChannel.ForAddress("http://localhost:50051");
            //var client = new msg.msgClient(channel);
            //var reply = client.TransferMessage(new SomethingRequest { Msg = "oplan" });
            //MessageBox.Show(reply.ToString());
            
            
        }

        private void NavigationFrame_InheritDataContext(object sender, NavigationEventArgs e)
        {
            FrameworkElement ?content = NavigationFrame.Content as FrameworkElement;
            if (content != null) 
            {
                content.DataContext = DataContext;
            }
            
        }

        private void NavigationFrame_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement ?content = NavigationFrame.Content as FrameworkElement;
            if (content != null)
            {
                content.DataContext = DataContext;
            }
        }

        private void LogarithmicScaleToggle_Toggled(object sender, RoutedEventArgs e) 
        {
            if (sender == null) { return; }
            if((sender as MahApps.Metro.Controls.ToggleSwitch).IsOn) 
            {
                PlotControl.SpectrumWpfPlot.Plot.Clear();
                double[] logYs = PlotControl.Counts.Select(y => (Math.Log10(y) == double.NegativeInfinity ? 0 : Math.Log10(y))).ToArray();
                PlotControl.UpdateSignalPlot(logYs, true);
                PlotControl.SpectrumWpfPlot.Refresh();
            }
            else 
            {
                PlotControl.SpectrumWpfPlot.Plot.Clear();
                double[] values = PlotControl.Counts.Select(x => (double)x).ToArray();
                PlotControl.UpdateSignalPlot(values, false);
                PlotControl.SpectrumWpfPlot.Refresh();
                
            }
        }
    }
}
