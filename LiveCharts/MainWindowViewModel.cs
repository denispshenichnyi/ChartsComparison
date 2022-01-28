using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveCharts
{
    public class MainWindowViewModel : IDisposable
    {
        private const int PointsCount = 10_000;
        private const int MultiSeriesCount = 10;
        private const int HeatCount = 1_000;

        private readonly List<DataGenerator> _dataGenerators = new();

        public MainWindowViewModel()
        {
            SingleLine = CreateLines(1, false);
            MultiLine = CreateLines(MultiSeriesCount, false);
            SingleDynamicLine = CreateLines(1, true);
            MultiDynamicLine = CreateLines(MultiSeriesCount, true);

            Intensities = CreateIntensities();
            Arguments = Enumerable.Range(0, HeatCount).ToArray();
        }

        public ObservableCollection<ISeries> SingleLine { get; }
        public ObservableCollection<ISeries> MultiLine { get; }
        public ObservableCollection<ISeries> SingleDynamicLine { get; }
        public ObservableCollection<ISeries> MultiDynamicLine { get; }
        public double[,] Intensities { get; }
        public int[] Arguments { get; }

        public event EventHandler<ObservableCollection<ISeries>> SeriesUpdated;

        private ObservableCollection<ISeries> CreateLines(int count, bool isDynamicData)
        {
            int pointsPerSeries = PointsCount / count;
            var generator = new DataGenerator(pointsPerSeries, count);
            generator.GenerateInitialData();
            ObservableCollection<ISeries> data = CreateLinesCore(generator.DataSource.ToArray());
            if (isDynamicData)
            {
                generator.Start();
                generator.DataChanged += (s, _) =>
                {
                    ObservableCollection<ISeries> newData = CreateLinesCore(((DataGenerator)s)!.DataSource.ToArray());
                    for (var i = 0; i < newData.Count; i++)
                        data[i].Values = newData[i].Values;
                    SeriesUpdated?.Invoke(this, data);
                };
            }

            _dataGenerators.Add(generator);

            return data;
        }

        private ObservableCollection<ISeries> CreateLinesCore(ObservableCollection<Point>[] dataSource)
        {
            var result = new ObservableCollection<ISeries>();
            for (int i = 0; i < dataSource.Length; i++)
            {
                var line = new LineSeries<Point>
                {
                    Fill = null,
                    AnimationsSpeed = null,
                    GeometrySize = 0
                };
                var values = new Point[dataSource[i].Count];
                for (int j = 0; j < values.Length; j++)
                    values[j] = dataSource[i][j];

                line.Values = values;
                result.Add(line);
            }

            return result;
        }

        private double[,] CreateIntensities()
        {
            var result = new double[HeatCount, HeatCount];
            for (int i = 0; i < HeatCount; i++)
            for (int j = 0; j < HeatCount; j++)
                result[i, j] = i * j % 100;

            return result;
        }

        public void Dispose()
        {
            foreach (DataGenerator dataGenerator in _dataGenerators)
                dataGenerator.Stop();
            _dataGenerators.Clear();
        }
    }
}
