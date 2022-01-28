using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;

namespace LiveCharts
{
    public class MatrixToSeries : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double[,] matrix) || targetType != typeof(IEnumerable<ISeries>))
                return null;

            WeightedPoint[] heatValues = new WeightedPoint[matrix.Length];
            for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                heatValues[i * matrix.GetLength(0) + j] = new WeightedPoint(i, j, i * j % 100);

            var heatSeries = new HeatSeries<WeightedPoint>
            {
                PointPadding = new Padding(),
                Values = heatValues,
                ColorStops = new[] { 0, 0.5, 1 },
                HeatMap = new[]
                {
                    SKColors.Red.AsLvcColor(),
                    SKColors.Yellow.AsLvcColor(),
                    SKColors.Blue.AsLvcColor()
                }
            };
            return new ISeries[] { heatSeries };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
