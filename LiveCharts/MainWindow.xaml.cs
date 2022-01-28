using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using InteractiveDataDisplay.WPF;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ScottPlot;
using ScottPlot.Drawing;

namespace LiveCharts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            LiveChartsCore.LiveCharts.HasMapFor<Point>((point, chartPoint) =>
            {
                chartPoint.IsNull = false;
                chartPoint.PrimaryValue = point.Y;
                chartPoint.SecondaryValue = point.X;
            });

            InitializeComponent();

            var viewModel = (MainWindowViewModel)DataContext;

            #region ScottPlot line
            //var activeSeries = viewModel.MultiDynamicLine;
            //viewModel.SeriesUpdated += (_, series) => UpdateChart(series);

            //void UpdateChart(ObservableCollection<ISeries> series)
            //{
            //    ObservableCollection<ISeries> lines = activeSeries;
            //    if (!ReferenceEquals(series, lines))
            //        return;
            //    Chart.Plot.Clear();
            //    for (int i = 0; i < lines.Count; i++)
            //    {
            //        var linePoints = lines[i].Values!.Cast<Point>().ToArray();
            //        Chart.Plot.AddScatter(linePoints.Select(pt => pt.X).ToArray(), linePoints.Select(pt => pt.Y).ToArray());
            //    }

            //    Chart.Render();
            //}

            //UpdateChart(activeSeries);
            #endregion

            #region ScottPlot heatmap
            //Chart.Plot.AddHeatmap(viewModel.Intensities, null, lockScales: false);
            //Chart.Configuration.Zoom = false;
            //Chart.Configuration.ScrollWheelZoom = false;
            //Chart.Configuration.LeftClickDragPan = false;
            //Chart.Plot.Margins(0, 0);
            //Chart.Refresh();
            #endregion

            #region InteractiveDataDisplay lines
            //ObservableCollection<ISeries> activeSeries = viewModel.MultiDynamicLine;
            //viewModel.SeriesUpdated += (_, series) => UpdateChart(series);

            //void UpdateChart(ObservableCollection<ISeries> series)
            //{
            //    if (!ReferenceEquals(series, activeSeries))
            //        return;

            //    Canvas.Children.Clear();
            //    for (int i = 0; i < series.Count; i++)
            //    {
            //        var line = new LineGraph { Points = new PointCollection(series[i].Values!.Cast<Point>()) };
            //        Canvas.Children.Add(line);
            //    }
            //}

            //UpdateChart(activeSeries);
            #endregion

            #region InteractiveDataDisplay heatmap
            //HeatMap.Plot(viewModel.Intensities, viewModel.Arguments, viewModel.Arguments);
            #endregion

            #region OxyPlot line
            //ObservableCollection<ISeries> activeSeries = viewModel.SingleDynamicLine;
            //var model = new PlotModel();
            //viewModel.SeriesUpdated += (_, series) => UpdateChart(series);
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            //UpdateChart(activeSeries);
            //Plot.Model = model;

            //void UpdateChart(ObservableCollection<ISeries> series)
            //{
            //    if (!ReferenceEquals(series, activeSeries))
            //        return;

            //    model.Series.Clear();
            //    for (int i = 0; i < series.Count; i++)
            //    {
            //        IEnumerable<Point> linePoints = series[i].Values!.Cast<Point>();
            //        var line = new LineSeries();
            //        foreach (Point linePoint in linePoints)
            //            line.Points.Add(new DataPoint(linePoint.X, linePoint.Y));

            //        model.Series.Add(line);
            //    }

            //    model.InvalidatePlot(true);
            //}
            #endregion

            #region OxyPlot heatmap
            //double[,] heatValues = viewModel.Intensities;
            //var model = new PlotModel();
            //model.Axes.Add(new LinearColorAxis() { Position = AxisPosition.Right, Palette = OxyPalettes.Cool(64) });
            //model.Series.Add(new HeatMapSeries() { X0 = 0, X1 = heatValues.GetLength(0), Y0 = 0, Y1 = heatValues.GetLength(1), Data = heatValues });
            //Plot.Model = model;
            #endregion
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            ((MainWindowViewModel)DataContext).Dispose();
        }
    }
}
