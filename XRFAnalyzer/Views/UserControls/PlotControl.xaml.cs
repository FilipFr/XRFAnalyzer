using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            peakPlots = new();
            SpectrumWpfPlot.Plot.Style(figureBackground: System.Drawing.ColorTranslator.FromHtml("#DDDDDD"));
            SpectrumWpfPlot.Plot.Style(dataBackground: System.Drawing.ColorTranslator.FromHtml("#F5F5F5"));
            SpectrumWpfPlot.Plot.Style(grid: System.Drawing.ColorTranslator.FromHtml("#D8E1EB"));
            SpectrumWpfPlot.Plot.YLabel(YLabel);
            SpectrumWpfPlot.Plot.XLabel(XLabel);
            SpectrumWpfPlot.Plot.SetAxisLimits(0, 2048, 0, 10000);
            SpectrumWpfPlot.Render();
            SpectrumWpfPlot.Plot.XAxis2.Layout(padding: 0, minimumSize: 8, maximumSize: 8);
        }

        private List<SignalPlotXY> peakPlots;

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
            b.UpdateSignalPlot(values, false);
        }

        public static readonly DependencyProperty PeaksProperty = DependencyProperty.Register(
            "Peaks",
            typeof(List<Tuple<int, int>>),
            typeof(PlotControl),
            new PropertyMetadata(null, new PropertyChangedCallback(PeaksChanged)));
        public List<Tuple<int, int>> Peaks
        {
            get { return (List<Tuple<int, int>>)GetValue(PeaksProperty); }
            set { SetValue(PeaksProperty, value); }
        }
        private static void PeaksChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            
            double[] values = b.Counts.Select(x => (double)x).ToArray();
            foreach (Tuple<int, int> peak in b.Peaks)
            {
                double[] peakValues = values.Skip(peak.Item1).Take(peak.Item2 -  peak.Item1).ToArray();
                
                SignalPlotXY peakPlot = new SignalPlotXY();
                
                peakPlot.Xs = Enumerable.Range(peak.Item1, peak.Item2 - peak.Item1).Select(Convert.ToDouble).ToArray();
                peakPlot.Ys = peakValues;
                b.peakPlots.Add(peakPlot);
            }
            b.UpdateSignalPlot(values, false);
        }
        
        public void UpdateSignalPlot(double[] values, bool isLogarithmicScale) 
        {
            var signalPlot = SpectrumWpfPlot.Plot.AddSignal(values);
            signalPlot.FillAboveAndBelow(System.Drawing.Color.FromArgb(255, 90, 180, 90), System.Drawing.Color.Red, 0.5);
            signalPlot.LineColor = System.Drawing.Color.FromArgb(255, 90, 180, 90);
            foreach (SignalPlotXY plot in peakPlots)
            {
                var newSignalXY = SpectrumWpfPlot.Plot.AddSignalXY(plot.Xs, plot.Ys);
                newSignalXY.Color = System.Drawing.Color.RebeccaPurple;
                newSignalXY.FillAboveAndBelow(System.Drawing.Color.Red, System.Drawing.Color.Purple, 0.5);

            }
            if (isLogarithmicScale) 
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
            SpectrumWpfPlot.Plot.AxisAuto();
            SpectrumWpfPlot.Plot.Render();
            SpectrumWpfPlot.Refresh();
        }
    }
}
