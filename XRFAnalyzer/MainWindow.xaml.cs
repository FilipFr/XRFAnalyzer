using Grpc.Net.Client;
using Microsoft.Win32;
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
            SpectrumWpfPlot.Plot.Style(figureBackground: System.Drawing.ColorTranslator.FromHtml("#DDDDDD"));
            SpectrumWpfPlot.Plot.Style(dataBackground: System.Drawing.ColorTranslator.FromHtml("#F5F5F5"));
            SpectrumWpfPlot.Plot.Style(grid: System.Drawing.ColorTranslator.FromHtml("#D8E1EB"));
            SpectrumWpfPlot.Plot.YLabel(LocalizationResourceManager["SpectrumWpfPlotYLabel"].ToString());
            SpectrumWpfPlot.Plot.XLabel(LocalizationResourceManager["SpectrumWpfPlotXLabel"].ToString());
            SpectrumWpfPlot.Plot.SetAxisLimits(0, 2048, 0, 10000);
        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo culture = 
                Strings.Culture.TwoLetterISOLanguageName.Equals("sk", StringComparison.InvariantCultureIgnoreCase) ?
                new CultureInfo("en-US") : new CultureInfo("sk");
            LocalizationResourceManager.Instance.SetCulture(culture);
            SpectrumWpfPlot.Plot.YLabel(LocalizationResourceManager["SpectrumWpfPlotYLabel"].ToString());
            SpectrumWpfPlot.Plot.XLabel(LocalizationResourceManager["SpectrumWpfPlotXLabel"].ToString());
            SpectrumWpfPlot.Render();
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
    }
}
