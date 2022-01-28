using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LiveCharts
{
    public class DataGenerator
    {
        #region inner class
        private class RealTimeDataCollection : ObservableCollection<Point>
        {
            public void AddRange(IList<Point> items)
            {
                foreach (Point item in items)
                    Items.Add(item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, Items.Count - items.Count));
            }
            public void RemoveRangeAt(int startingIndex, int count)
            {
                var removedItems = new List<Point>(count);
                for (int i = 0; i < count; i++)
                {
                    removedItems.Add(Items[startingIndex]);
                    Items.RemoveAt(startingIndex);
                }
                if (count > 0)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, startingIndex));
            }
        }
        #endregion

        private const int DataGenerationIntervalMilliseconds = 15;

        private readonly int _pointsCount;
        private readonly List<Point>[] _buffer;
        private readonly object _sync = new();
        private readonly RealTimeDataCollection[] _dataSource;
        private readonly DispatcherTimer _timer;
        private bool _generatingEnabled;
        private Thread _generatingThread;
        private int _counter;

        public DataGenerator(int pointsCount, int seriesCount)
        {
            _pointsCount = pointsCount;

            _dataSource = new RealTimeDataCollection[seriesCount];
            for (int i = 0; i < seriesCount; i++)
                _dataSource[i] = new RealTimeDataCollection();

            _buffer = new List<Point>[seriesCount];
            for (int i = 0; i < seriesCount; i++)
                _buffer[i] = new List<Point>();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(DataGenerationIntervalMilliseconds), IsEnabled = false };
            _timer.Tick += OnTimerTick;
        }

        public IEnumerable<ObservableCollection<Point>> DataSource => this._dataSource;

        public event EventHandler DataChanged;

        public void GenerateInitialData()
        {
            for (int i = 0; i < _pointsCount - 1; i++)
            {
                _counter++;
                for (int j = 0; j < _dataSource.Length; j++)
                {
                    Point point = CreatePoint(j);
                    _dataSource[j].Add(point);
                }
            }
        }

        public void Start()
        {
            _generatingThread ??= new Thread(GeneratingLoop);
            _generatingEnabled = true;
            _generatingThread.Start();
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _generatingEnabled = false;
            _generatingThread?.Join();
            _generatingThread = null;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateDataSource();
        }

        private void UpdateDataSource()
        {
            lock (_sync)
                for (int i = 0; i < _dataSource.Length; i++)
                {
                    _dataSource[i].AddRange(_buffer[i]);
                    if (_dataSource[i].Count > _pointsCount)
                        _dataSource[i].RemoveRangeAt(0, _buffer[i].Count);
                    _buffer[i].Clear();
                }
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        private void AddPoint(int index)
        {
            Point point = CreatePoint(index);
            lock (_sync)
                _buffer[index].Add(point);
        }

        private Point CreatePoint(int index)
        {
            return new Point(_counter, _counter * (index + 1) + _counter % 10);
        }

        private void GeneratingLoop()
        {
            DateTime timeStamp = DateTime.Now;
            while (_generatingEnabled)
            {
                DateTime newTimeStamp = timeStamp.AddMilliseconds(DataGenerationIntervalMilliseconds);
                TimeSpan span = newTimeStamp - DateTime.Now;
                if (span.Ticks > 0)
                    Thread.Sleep((int)span.TotalMilliseconds);
                timeStamp = newTimeStamp;
                _counter++;
                for (int i = 0; i < _dataSource.Length; i++)
                    AddPoint(i);
            }
        }
    }
}
