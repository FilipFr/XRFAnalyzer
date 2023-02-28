using System;
using System.Collections.Generic;
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

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            CultureInfo culture = 
                Strings.Culture.TwoLetterISOLanguageName.Equals("sk", StringComparison.InvariantCultureIgnoreCase) ?
                new CultureInfo("en-US") : new CultureInfo("sk");
            LocalizationResourceManager.Instance.SetCulture(culture);
        }
    }
}
