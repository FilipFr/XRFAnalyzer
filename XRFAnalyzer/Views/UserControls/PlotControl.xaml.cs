using Google.Protobuf.Collections;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRFAnalyzer.Views.Pages;

namespace XRFAnalyzer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for PlotControl.xaml
    /// </summary>
    public partial class PlotControl : UserControl
    {
        public PlotControl()
        {
            InitializeComponent();
            SpectrumWpfPlot.Plot.Style(figureBackground: System.Drawing.ColorTranslator.FromHtml("#DDDDDD"));
            SpectrumWpfPlot.Plot.Style(dataBackground: System.Drawing.ColorTranslator.FromHtml("#F5F5F5"));
            SpectrumWpfPlot.Plot.Style(grid: System.Drawing.ColorTranslator.FromHtml("#D8E1EB"));
            SpectrumWpfPlot.Plot.YLabel(YLabel);
            SpectrumWpfPlot.Plot.XLabel(XLabel);
            SpectrumWpfPlot.Plot.SetAxisLimits(0, 2048, 0, 10000);
            SpectrumWpfPlot.Render();
            SpectrumWpfPlot.Plot.XAxis2.Layout(padding: 0, minimumSize: 8, maximumSize: 8);
        }

        public static readonly DependencyProperty YLabelProperty = DependencyProperty.Register(
            "YLabel",
            typeof(string),
            typeof(PlotControl),
            new PropertyMetadata("Counts", new PropertyChangedCallback(YLabelChanged)));
        public string YLabel
        {
            get { return (string)GetValue(YLabelProperty); }
            set { SetValue(YLabelProperty, value); }
        }
        private static void YLabelChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.SpectrumWpfPlot.Plot.YLabel(b.YLabel);
            b.SpectrumWpfPlot.Plot.Render();
        }

        public static readonly DependencyProperty XLabelProperty = DependencyProperty.Register(
            "XLabel",
            typeof(string),
            typeof(PlotControl),
            new PropertyMetadata("Channel", new PropertyChangedCallback(XLabelChanged)));
        public string XLabel
        {
            get { return (string)GetValue(XLabelProperty); }
            set { SetValue(XLabelProperty, value); }
        }
        private static void XLabelChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.SpectrumWpfPlot.Plot.XLabel(b.XLabel);
            b.SpectrumWpfPlot.Plot.Render();
        }

        public static readonly DependencyProperty SelectedPeakIndexProperty = DependencyProperty.Register(
            "SelectedPeakIndex",
            typeof(int),
            typeof(PlotControl),
            new PropertyMetadata(-1, new PropertyChangedCallback(SelectedPeakIndexChanged)));
        public int SelectedPeakIndex
        {
            get { return (int)GetValue(SelectedPeakIndexProperty); }
            set { SetValue(SelectedPeakIndexProperty, value); }
        }
        private static void SelectedPeakIndexChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.SelectedPeakIndex = b.SelectedPeakIndex;
            b.UpdateSignalPlot();
        }

        public static readonly DependencyProperty CountsProperty = DependencyProperty.Register(
            "Counts",
            typeof(List<int>),
            typeof(PlotControl),
            new PropertyMetadata(null, new PropertyChangedCallback(CountsChanged)));
        public List<int> Counts
        {
            get { return (List<int>)GetValue(CountsProperty); }
            set { SetValue(CountsProperty, value); }
        }
        private static void CountsChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            double[] values = b.Counts.Select(x => (double)x).ToArray();
            b.UpdateSignalPlot(false);
        }

        public static readonly DependencyProperty PeaksProperty = DependencyProperty.Register(
            "Peaks",
            typeof(List<Tuple<int, int>>),
            typeof(PlotControl),
            new PropertyMetadata(new List<Tuple<int, int>>(), new PropertyChangedCallback(PeaksChanged)));
        public List<Tuple<int, int>> Peaks
        {
            get { return (List<Tuple<int, int>>)GetValue(PeaksProperty); }
            set { SetValue(PeaksProperty, value); }
        }
        private static void PeaksChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot();
        }

        public static readonly DependencyProperty FoundRoisProperty = DependencyProperty.Register(
            "FoundRois",
            typeof(List<Tuple<int, int>>),
            typeof(PlotControl),
            new PropertyMetadata(new List<Tuple<int, int>>(), new PropertyChangedCallback(FoundRoisChanged)));
        public List<Tuple<int, int>> FoundRois
        {
            get { return (List<Tuple<int, int>>)GetValue(FoundRoisProperty); }
            set { SetValue(FoundRoisProperty, value); }
        }
        private static void FoundRoisChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot();
        }

        public static readonly DependencyProperty IsLogarithmicToggledProperty = DependencyProperty.Register(
            "IsLogarithmicToggled",
            typeof(bool),
            typeof(PlotControl),
            new PropertyMetadata(false, new PropertyChangedCallback(IsLogarithmicToggledChanged)));
        public bool IsLogarithmicToggled
        {
            get { return (bool)GetValue(IsLogarithmicToggledProperty); }
            set { SetValue(IsLogarithmicToggledProperty, value); }
        }
        private static void IsLogarithmicToggledChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot(false);
        }

        public void UpdateSignalPlot(bool preserveAxisLimits = true)
        {
            var limits = SpectrumWpfPlot.Plot.GetAxisLimits();
            SpectrumWpfPlot.Plot.Clear();
            if (preserveAxisLimits) 
            {
                SpectrumWpfPlot.Plot.SetAxisLimits(limits);
            }
            

            double[] values = (IsLogarithmicToggled) ?
                Counts.Select(y => (Math.Log10(y) == double.NegativeInfinity ? 0 : Math.Log10(y))).ToArray() :
                Counts.Select(x => (double)x).ToArray();

            var signalPlot = SpectrumWpfPlot.Plot.AddSignal(values);
            signalPlot.FillAboveAndBelow(System.Drawing.Color.FromArgb(255, 90, 180, 90), System.Drawing.Color.Red, 0.5);
            signalPlot.LineColor = System.Drawing.Color.FromArgb(255, 90, 180, 90);
            foreach (Tuple<int, int> peak in Peaks)
            {
                double[] Xs = Enumerable.Range(peak.Item1, peak.Item2 - peak.Item1).Select(x => (double)x).ToArray();
                double[] Ys = values.Skip(peak.Item1).Take(peak.Item2 - peak.Item1).ToArray();
                var newSignalXY = SpectrumWpfPlot.Plot.AddSignalXY(Xs, Ys);
                newSignalXY.Color = System.Drawing.Color.RebeccaPurple;
                newSignalXY.FillAboveAndBelow(System.Drawing.Color.Red, System.Drawing.Color.Purple, 0.5);
                if(SelectedPeakIndex >= 0 && SelectedPeakIndex < Peaks.Count && peak == Peaks[SelectedPeakIndex]) 
                {
                    newSignalXY.FillAboveAndBelow(System.Drawing.Color.Blue, System.Drawing.Color.Purple, 0.5);
                }
                foreach(Tuple<int, int> foundRoi in FoundRois) 
                {
                    Xs = Enumerable.Range(foundRoi.Item1, foundRoi.Item2 - foundRoi.Item1).Select(x => (double)x).ToArray();
                    Ys = values.Skip(foundRoi.Item1).Take(foundRoi.Item2 - foundRoi.Item1).ToArray();
                    newSignalXY = SpectrumWpfPlot.Plot.AddSignalXY(Xs, Ys);
                    newSignalXY.FillAboveAndBelow(System.Drawing.Color.RebeccaPurple, System.Drawing.Color.Purple, 0.5);
                }
            }
            if (IsLogarithmicToggled) 
            {
                string logTickLabels(double y) => Math.Pow(10, y).ToString();
                SpectrumWpfPlot.Plot.YAxis.TickLabelFormat(logTickLabels);
                SpectrumWpfPlot.Plot.YAxis.MinorLogScale(true);
            } 
            else
            {
                string logTickLabels(double y) => y.ToString();
                SpectrumWpfPlot.Plot.YAxis.TickLabelFormat(logTickLabels);
                SpectrumWpfPlot.Plot.YAxis.MinorLogScale(false);
            }
            SpectrumWpfPlot.Plot.Render();
            SpectrumWpfPlot.Refresh();
        }


        private void SpectrumWpfPlot_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (double x, double y) = this.SpectrumWpfPlot.GetMouseCoordinates();
            double roundedX = Math.Round(x);
            double roundedY = Math.Round(y);
            int counter = 0;
            foreach(Tuple<int,int> peak in Peaks) 
            {
                if (roundedX <= peak.Item2  && roundedX >= peak.Item1) 
                {
                    if(roundedY <= Counts[(int)roundedX]) 
                    {
                        SelectedPeakIndex = counter;
                        
                        break;
                    }
                }
                else 
                {
                    SelectedPeakIndex = -1;
                }
                counter++;
            }
            
            UpdateSignalPlot();
            
        }
    }
}
