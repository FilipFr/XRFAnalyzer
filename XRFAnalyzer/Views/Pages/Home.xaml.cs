using System;
using System.Collections.Generic;
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

namespace XRFAnalyzer.Views.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ?a = e.OriginalSource as Button;
            if (a != null)
            {
                switch (a.Name)
                {
                    case "NavigationButtonFindPeaks":
                        NavigationService.GetNavigationService(this).Navigate(new FindPeaksPage());
                        break;
                    case "NavigationButtonCalibration":
                        NavigationService.GetNavigationService(this).Navigate(new CalibrationPage());
                        break;
                    case "NavigationButtonBackground":
                        NavigationService.GetNavigationService(this).Navigate(new BackgroundPage());
                        break;
                    case "NavigationButtonSumPeaks":
                        NavigationService.GetNavigationService(this).Navigate(new SumPeaksPage());
                        break;
                    case "NavigationButtonQualitativeAnalysis":
                        NavigationService.GetNavigationService(this).Navigate(new QualitativeAnalysisPage());
                        break;
                    case "NavigationButtonQuantitativeAnalysis":
                        NavigationService.GetNavigationService(this).Navigate(new QuantitativeAnalysisPage());
                        break;

                }
            }
        }
    }
}
