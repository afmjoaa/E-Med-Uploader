using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using custom_window.Core;
using custom_window.HelperClasses;
using Google.Cloud.Firestore;
using LiveCharts;
using LiveCharts.Wpf;
using DataFormats = System.Windows.Forms.DataFormats;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Statistics : BasePage<StateViewModel>
    {
        private CloudFirestoreService _cf = null;

        public Statistics()
        {
            InitializeComponent();
            _cf = CloudFirestoreService.GetInstance();


            //add the created one here...


            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Uploaded Reports",
                    Values = new ChartValues<double> {0, 0, 0},
                    LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                    PointGeometrySize = 8,
                    PointForeground = Brushes.AntiqueWhite,
                    Fill = new LinearGradientBrush(Colors.LightSalmon, Colors.Transparent, new Point(0, 0),
                        new Point(0, 1)),
                    Stroke = Brushes.LightSalmon,
                    StrokeThickness = 2
                }
            };

            /*|| x.LastWriteTime == today*/
            var defaultWatchFolder = Properties.Settings.Default.watchFolder;
            var totalCreated = 0;
            var yesterdayCreated = 0;
            var beforeYesterdayCreated = 0;
            if (defaultWatchFolder != null && defaultWatchFolder.Count != 0)
            {
                DateTime today = DateTime.Now.Date;
                DateTime yesterdayDate = today.AddDays(-1);

                foreach (var folder in defaultWatchFolder)
                {
                    //getting the today count
                    FileInfo[] todaysFiles = new DirectoryInfo(folder)
                        .EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                        .Select(x =>
                        {
                            x.Refresh();
                            return x;
                        })
                        .Where(x => (x.CreationTime.Date - today).Hours <= 24)
                        .ToArray();
                    totalCreated = totalCreated + todaysFiles.Length;

                    //getting the yesterday files
                    FileInfo[] yesterdaysFiles = new DirectoryInfo(folder)
                        .EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                        .Select(x =>
                        {
                            x.Refresh();
                            return x;
                        })
                        .Where(x => (x.CreationTime.Date - yesterdayDate).Hours <= 48 &&
                                    (x.CreationTime.Date - yesterdayDate).Hours > 24)
                        .ToArray();
                    yesterdayCreated = yesterdayCreated + yesterdaysFiles.Length;


                    int fCount = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly).Length;
                    beforeYesterdayCreated =
                        beforeYesterdayCreated + fCount - todaysFiles.Length - yesterdaysFiles.Length;
                }
            }

            SeriesCollection.Add(new LineSeries
            {
                Title = "Created Report",
                Values = new ChartValues<double> {totalCreated, yesterdayCreated, beforeYesterdayCreated},
                LineSmoothness = 1, //0: straight lines, 1: really smooth lines
                PointGeometrySize = 8,
                PointForeground = Brushes.LightCyan,
                Fill = new LinearGradientBrush(Colors.LightBlue, Colors.Transparent, new Point(0, 0),
                    new Point(0, 1)),
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2
            });

            Labels = new[] {"Today", "Yesterday", "Before yesterday"};
            YFormatter = value => value.ToString("F1");

            //modifying any series values will also animate and update the chart
            //SeriesCollection[3].Values.Add(5d);
            DataContext = this;

            LoadList();

            //this.SeriesCollection[0].Values.RemoveAt(1);
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public async Task LoadList()
        {
            var returnMap = new Dictionary<string, int>();
            var allFiles = await _cf.GetUploadedFilesForState();
            Double todayCount = 0;
            Double yesterdayCount = 0;
            Double beforeYesterdayCount = 0;
            var today = Timestamp.GetCurrentTimestamp().ToDateTime();

            if (allFiles != null)
            {
                if (allFiles.Count > 0)
                {
                    foreach (ReportFile rf in allFiles)
                    {
                        var totalHours = (today - rf.file_creation_date.ToDateTime()).Hours;
                        if (totalHours <= 24)
                        {
                            todayCount = todayCount + 1;
                        }
                        else if (totalHours <= 48 && totalHours > 24)
                        {
                            yesterdayCount = yesterdayCount + 1;
                        }
                        else
                        {
                            beforeYesterdayCount = beforeYesterdayCount + 1;
                        }
                    }
                }
            }

            this.SeriesCollection[0].Values.Insert(0, todayCount);
            this.SeriesCollection[0].Values.Insert(1, yesterdayCount);
            this.SeriesCollection[0].Values.Insert(2, beforeYesterdayCount);
            this.SeriesCollection[0].Values.Remove(0d);
            this.SeriesCollection[0].Values.Remove(0d);
            this.SeriesCollection[0].Values.Remove(0d);

            /*for (int i = 0; i < this.SeriesCollection[0].Values.Count +1; i++)
            {
                if (i >= 3)
                {
                    this.SeriesCollection[0].Values.Remove(0);
                }
            }*/
        }
    }
}

/*int fCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;*/