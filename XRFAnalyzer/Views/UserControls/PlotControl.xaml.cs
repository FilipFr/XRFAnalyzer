using Google.Protobuf.Collections;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.SnapLogic;
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
using XRFAnalyzer.Models;
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

        public static readonly DependencyProperty XLabelAltProperty = DependencyProperty.Register(
            "XLabelAlt",
            typeof(string),
            typeof(PlotControl),
            new PropertyMetadata("Energy", new PropertyChangedCallback(XLabelAltChanged)));
        public string XLabelAlt
        {
            get { return (string)GetValue(XLabelAltProperty); }
            set { SetValue(XLabelAltProperty, value); }
        }
        private static void XLabelAltChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
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
            typeof(List<double>),
            typeof(PlotControl),
            new PropertyMetadata(null, new PropertyChangedCallback(CountsChanged)));
        public List<double> Counts
        {
            get { return (List<double>)GetValue(CountsProperty); }
            set { SetValue(CountsProperty, value); }
        }
        private static void CountsChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            double[] values = b.Counts.Select(x => (double)x).ToArray();
            if (Enumerable.SequenceEqual(b.Counts, b.CorrectedCounts)) 
            {
                b.UpdateSignalPlot();
            }
            else 
            {
                b.UpdateSignalPlot(false);
            }
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

        public static readonly DependencyProperty LineEnergiesProperty = DependencyProperty.Register(
            "LineEnergies",
            typeof(List<string>),
            typeof(PlotControl),
            new PropertyMetadata(new List<string>(), new PropertyChangedCallback(LineEnergiesChanged)));
        public List<string> LineEnergies
        {
            get { return (List<string>)GetValue(LineEnergiesProperty); }
            set { SetValue(LineEnergiesProperty, value); }
        }
        private static void LineEnergiesChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot();
        }

        public static readonly DependencyProperty CorrectedCountsProperty = DependencyProperty.Register(
            "CorrectedCounts",
            typeof(List<double>),
            typeof(PlotControl),
            new PropertyMetadata(new List<double>(), new PropertyChangedCallback(CorrectedCountsChanged)));
        public List<double> CorrectedCounts
        {
            get { return (List<double>)GetValue(CorrectedCountsProperty); }
            set { SetValue(CorrectedCountsProperty, value); }
        }
        private static void CorrectedCountsChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
           b.UpdateSignalPlot(true);
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

        public static readonly DependencyProperty IsXAxisUnitToggledProperty = DependencyProperty.Register(
            "IsXAxisUnitToggled",
            typeof(bool),
            typeof(PlotControl),
            new PropertyMetadata(false, new PropertyChangedCallback(IsXAxisUnitToggledChanged)));
        public bool IsXAxisUnitToggled
        {
            get { return (bool)GetValue(IsXAxisUnitToggledProperty); }
            set { SetValue(IsXAxisUnitToggledProperty, value); }
        }
        private static void IsXAxisUnitToggledChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot(false);
        }

        public static readonly DependencyProperty CalibrationCurveSlopeProperty = DependencyProperty.Register(
            "CalibrationCurveSlope",
            typeof(double),
            typeof(PlotControl),
            new PropertyMetadata(Double.MaxValue, new PropertyChangedCallback(CalibrationCurveSlopeChanged)));
        public double CalibrationCurveSlope 
        {
            get { return (double)GetValue(CalibrationCurveSlopeProperty); }
            set { SetValue(CalibrationCurveSlopeProperty, value); }
        }
        private static void CalibrationCurveSlopeChanged(DependencyObject a, DependencyPropertyChangedEventArgs e) 
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot();
        }

        public static readonly DependencyProperty CalibrationCurveInterceptProperty = DependencyProperty.Register(
            "CalibrationCurveIntercept",
            typeof(double),
            typeof(PlotControl),
            new PropertyMetadata(Double.MaxValue, new PropertyChangedCallback(CalibrationCurveInterceptChanged)));
        public double CalibrationCurveIntercept
        {
            get { return (double)GetValue(CalibrationCurveInterceptProperty); }
            set { SetValue(CalibrationCurveInterceptProperty, value); }
        }
        private static void CalibrationCurveInterceptChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            PlotControl b = (PlotControl)a;
            b.UpdateSignalPlot();
        }

        public void UpdateSignalPlot(bool preserveAxisLimits = true)
        {
            var limits = SpectrumWpfPlot.Plot.GetAxisLimits();
            SpectrumWpfPlot.Plot.Clear();
            if (preserveAxisLimits) 
            {
                SpectrumWpfPlot.Plot.SetAxisLimits(limits);
            }

            List<double> background = new();
            if (CorrectedCounts != null && Counts.Count == CorrectedCounts.Count) 
            {
                for (int i  = 0; i < CorrectedCounts.Count; i++) 
                {
                    background.Add(Counts[i] - CorrectedCounts[i]);
                }
            }

            double[] values = (IsLogarithmicToggled) ?
                Counts.Select(y => ((Math.Log10(y) == double.NegativeInfinity || Math.Log10(y) < 1) ? 0 : Math.Log10(y))).ToArray() :
                Counts.Select(y => y).ToArray();
            double[] backgroundValues = (IsLogarithmicToggled) ?
                background.Select(y => ((Math.Log10(y) == double.NegativeInfinity || Math.Log10(y) < 1) ? 0 : Math.Log10(y))).ToArray() :
                background.Select(y => y).ToArray();

            double[] XValues = (CalibrationCurveSlope != Double.MaxValue && CalibrationCurveIntercept != Double.MaxValue) ?
                Enumerable.Range(0, values.Length).Select(x => (double)x).ToArray() :
                Enumerable.Range(0, values.Length).Select(x => x * CalibrationCurveSlope + CalibrationCurveIntercept).ToArray();

            var signalPlot = SpectrumWpfPlot.Plot.AddSignal(values);
            signalPlot.FillAboveAndBelow(System.Drawing.Color.FromArgb(255, 90, 180, 90), System.Drawing.Color.Red, 0.5);
            signalPlot.LineColor = System.Drawing.Color.FromArgb(255, 90, 180, 90);

            var backgroundPlot = SpectrumWpfPlot.Plot.AddSignal(backgroundValues);
            backgroundPlot.FillAboveAndBelow(System.Drawing.Color.Wheat, System.Drawing.Color.Red, 0.5);
            backgroundPlot.LineColor = System.Drawing.Color.FromArgb(255, 90, 180, 90);

            foreach (Tuple<int, int> peak in Peaks)
            {
                double[] Xs = Enumerable.Range(peak.Item1, peak.Item2 - peak.Item1).Select(x => (double)x).ToArray();
                //if (CalibrationCurveSlope != Double.MaxValue && CalibrationCurveIntercept != Double.MaxValue)
                //{
                //    Xs = Xs.Select(x => x * CalibrationCurveSlope + CalibrationCurveIntercept).ToArray();
                //}
                double[] Ys = values.Skip(peak.Item1).Take(peak.Item2 - peak.Item1).ToArray();
                if(Ys.Length == 0) 
                {
                    continue;
                }
                var newSignalXY = SpectrumWpfPlot.Plot.AddSignalXY(Xs, Ys);
                newSignalXY.Color = System.Drawing.Color.RebeccaPurple;
                newSignalXY.FillAboveAndBelow(System.Drawing.Color.Red, System.Drawing.Color.Purple, 0.5);
                if(SelectedPeakIndex >= 0 && SelectedPeakIndex < Peaks.Count && peak == Peaks[SelectedPeakIndex]) 
                {
                    newSignalXY.FillAboveAndBelow(System.Drawing.Color.Blue, System.Drawing.Color.Purple, 0.5);
                }
            }
            foreach (Tuple<int, int> foundRoi in FoundRois)
            {
                double[] Xs = Enumerable.Range(foundRoi.Item1, foundRoi.Item2 - foundRoi.Item1).Select(x => (double)x).ToArray();
                //if (CalibrationCurveSlope != Double.MaxValue && CalibrationCurveIntercept !=  Double.MaxValue) 
                //{
                //    Xs = Xs.Select(x => x * CalibrationCurveSlope + CalibrationCurveIntercept).ToArray();
                //}
                double[] Ys = values.Skip(foundRoi.Item1).Take(foundRoi.Item2 - foundRoi.Item1).ToArray();
                var newSignalXY = SpectrumWpfPlot.Plot.AddSignalXY(Xs, Ys);
                newSignalXY.FillAboveAndBelow(System.Drawing.Color.RebeccaPurple, System.Drawing.Color.Purple, 0.5);
            }
            foreach (Tuple<double, string> lines in ParseLineEnergies()) 
            {
                var vline = SpectrumWpfPlot.Plot.AddVerticalLine(lines.Item1);
                vline.LineWidth = 2;
                vline.DragEnabled = false;
                vline.PositionLabel = true;
                vline.PositionLabelOppositeAxis = true;
                vline.PositionLabelBackground = vline.Color;
                vline.PositionFormatter = new Func<double, string> (x => lines.Item2);
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
            if (IsXAxisUnitToggled && CalibrationCurveSlope != Double.MaxValue && CalibrationCurveIntercept != Double.MaxValue) 
            {
                string energyTickLabels(double x) => $"{(x*CalibrationCurveSlope + CalibrationCurveIntercept):N2}";
                SpectrumWpfPlot.Plot.XAxis.TickLabelFormat(energyTickLabels);
                SpectrumWpfPlot.Plot.XLabel(XLabelAlt);
            }
            else
            {
                string energyTickLabels(double x) => x.ToString();
                SpectrumWpfPlot.Plot.XAxis.TickLabelFormat(energyTickLabels);
                SpectrumWpfPlot.Plot.XLabel(XLabel);
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

        public List<Tuple<double, string>> ParseLineEnergies() 
        {
            List<Tuple<double, string>> toReturn = new();
            foreach(string line in LineEnergies) 
            {
                string[] splitLine = line.Split();
                double item1 = Double.Parse(splitLine[0]);
                //if (IsXAxisUnitToggled) 
                //{
                //    item1 = item1 * CalibrationCurveSlope + CalibrationCurveIntercept;
                //}
                string item2 = splitLine[1] + " " + splitLine[2];
                Tuple<double, string> parsed = new(item1, item2);
                toReturn.Add(parsed);
            }
            return toReturn;
        }
    }
}
