﻿using System;
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
    /// Interaction logic for FindPeaksPage.xaml
    /// </summary>
    public partial class FindPeaksPage : Page
    {
        public FindPeaksPage()
        {
            InitializeComponent();
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {

        }
    }
}
