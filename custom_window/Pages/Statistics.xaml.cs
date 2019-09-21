using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using custom_window.Core;
using LiveCharts;
using LiveCharts.Wpf;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Statistics : BasePage<StateViewModel>
    {
        public Statistics()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Uploaded Reports",
                    Values = new ChartValues<double> {4, 6, 5, 2, 4},
                    LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                    PointGeometrySize = 8,
                    PointForeground = Brushes.AntiqueWhite,
                    Fill = new LinearGradientBrush(Colors.LightPink, Colors.Transparent, new Point(0, 0),
                        new Point(0, 1)),
                    Stroke = Brushes.LightPink,
                    StrokeThickness = 2
                }
            };

            Labels = new[] {"Jan", "Feb", "Mar", "Apr", "May"};
            YFormatter = value => value.ToString("C");

            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Title = "Created Report",
                Values = new ChartValues<double> {5, 3, 2, 4, 5},
                LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                PointGeometrySize = 8,
                PointForeground = Brushes.LightCyan,
                Fill = new LinearGradientBrush(Colors.LightSkyBlue, Colors.Transparent, new Point(0, 0),
                    new Point(0, 1)),
                Stroke = Brushes.LightSkyBlue,
                StrokeThickness = 2
            });

            //modifying any series values will also animate and update the chart
            //SeriesCollection[3].Values.Add(5d);

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }


        public GridLength statHeight
        {
            get
            {
                return ((MainWindow) Application.Current.MainWindow).WindowState == WindowState.Maximized ? new GridLength(660) : new GridLength(440);
            }
        }
    }
}