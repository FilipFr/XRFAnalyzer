using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
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
    /// Interaction logic for LinearRegressionPlotControl.xaml
    /// </summary>
    public partial class LinearRegressionPlotControl : UserControl
    {
        public LinearRegressionPlotControl()
        {
            InitializeComponent();
            LinearRegressionWpfPlot.Plot.YLabel(YLabels);
            LinearRegressionWpfPlot.Plot.XLabel(XLabels);
        }

        public static readonly DependencyProperty YLabelsProperty = DependencyProperty.Register(
            "YLabels",
            typeof(string),
            typeof(LinearRegressionPlotControl),
            new PropertyMetadata("Counts", new PropertyChangedCallback(YLabelsChanged)));

        public string YLabels
        {
            get { return (string)GetValue(YLabelsProperty); }
            set { SetValue(YLabelsProperty, value); }
        }

        private static void YLabelsChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            LinearRegressionPlotControl b = (LinearRegressionPlotControl)a;
            b.LinearRegressionWpfPlot.Plot.YLabel(b.YLabels);
            b.LinearRegressionWpfPlot.Plot.Render();
        }

        public static readonly DependencyProperty XLabelsProperty = DependencyProperty.Register(
            "XLabels",
            typeof(string),
            typeof(LinearRegressionPlotControl),
            new PropertyMetadata("Channel", new PropertyChangedCallback(XLabelsChanged)));
        public string XLabels
        {
            get { return (string)GetValue(XLabelsProperty); }
            set { SetValue(XLabelsProperty, value); }
        }
        private static void XLabelsChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            LinearRegressionPlotControl b = (LinearRegressionPlotControl)a;
            b.LinearRegressionWpfPlot.Plot.XLabel(b.XLabels);
            b.LinearRegressionWpfPlot.Plot.Render();
        }

        public static readonly DependencyProperty CalibrationPointsProperty = DependencyProperty.Register(
            "CalibrationPoints",
            typeof(ObservableCollection<Tuple<int, double>>),
            typeof(LinearRegressionPlotControl),
            new PropertyMetadata(new ObservableCollection<Tuple<int, double>>(), new PropertyChangedCallback(CalibrationPointsChanged)));
        public ObservableCollection<Tuple<int, double>> CalibrationPoints
        {
            get { return (ObservableCollection<Tuple<int, double>>)GetValue(CalibrationPointsProperty); }
            set { SetValue(CalibrationPointsProperty, value); }
        }
        private static void CalibrationPointsChanged(DependencyObject a, DependencyPropertyChangedEventArgs e)
        {
            var action = new NotifyCollectionChangedEventHandler(
            (o, args) =>
            {
                LinearRegressionPlotControl b = (LinearRegressionPlotControl)a;
                double[] Xs = Array.Empty<double>();
                double[] Ys = Array.Empty<double>();
                double X1 = 0;
                double X2 = 0;
                if (b.CalibrationPoints.Count > 1)
                {
                    foreach (Tuple<int, double> kvp in b.CalibrationPoints)
                    {
                        Xs = Xs.Append(kvp.Item1).ToArray();
                        Ys = Ys.Append(kvp.Item2).ToArray();
                        X1 = Xs[0];
                        X2 = Xs[Xs.Length - 1];
                    }
                    var model = new ScottPlot.Statistics.LinearRegressionLine(Xs, Ys);
                    b.LinearRegressionWpfPlot.Plot.Clear();
                    b.LinearRegressionWpfPlot.Plot.Title("Linear Regression\n" +
                        $"Y = {model.slope:0.0000}x + {model.offset:0.0} " +
                        $"(R² = {model.rSquared:0.0000})");
                    b.LinearRegressionWpfPlot.Plot.AddScatter(Xs, Ys, lineWidth: 0);
                    b.LinearRegressionWpfPlot.Plot.AddLine(model.slope, model.offset, (X1, X2), lineWidth: 2);
                    if(!Double.IsNaN(model.slope) && !Double.IsNaN(model.offset) && !Double.IsNaN(model.rSquared)) 
                    {
                        b.LinearRegressionWpfPlot.Refresh();
                        b.LinearRegressionWpfPlot.Plot.Render();
                    }
                    else 
                    {
                        b.LinearRegressionWpfPlot.Plot.Clear();
                    }
                }
                else { b.LinearRegressionWpfPlot.Plot.Clear(); }
            });
            if (e.OldValue != null)
            {
                var coll = (INotifyCollectionChanged)e.OldValue;
                coll.CollectionChanged -= action;
            }

            if (e.NewValue != null)
            {
                var coll = (ObservableCollection<Tuple<int, double>>)e.NewValue;
                coll.CollectionChanged += action;
            }
        }
    }
}
