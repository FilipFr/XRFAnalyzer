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
                    case "NavigationButton1":
                        NavigationService.GetNavigationService(this).Navigate(new Peaks());
                        break;
                    case "NavigationButton2":
                        NavigationService.GetNavigationService(this).Navigate(new Calibration());
                        break;
                    case "NavigationButton3":
                        NavigationService.GetNavigationService(this).Navigate(new Background());
                        break;
                    case "NavigationButton4":
                        NavigationService.GetNavigationService(this).Navigate(new SumPeaks());
                        break;
                    case "NavigationButton5":
                        NavigationService.GetNavigationService(this).Navigate(new QualitativeAnalysis());
                        break;
                    case "NavigationButton6":
                        NavigationService.GetNavigationService(this).Navigate(new QuantitativeAnalysis());
                        break;

                }
            }
        }
    }
}
