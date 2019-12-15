using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;

namespace custom_window
{
    public sealed class HistoryListDm : HistoryListVm
    {
        private static HistoryListDm PrivateInstance { get; set; }
        private static readonly object padlock = new object();
        private CloudFirestoreService _cf = null;

        public static HistoryListDm Instance
        {
            get
            {
                lock (padlock)
                {
                    return PrivateInstance ?? (PrivateInstance = new HistoryListDm());
                }
            }
        }

        #region constructor

        private HistoryListDm()
        {
            _cf = CloudFirestoreService.GetInstance();
            _cf.OnDbFileChanged += OnDbFileChanged;
            LoadList();
        }

        #endregion


        private void OnDbFileChanged(List<ReportFile> updatedfiles)
        {
            Application.Current.Dispatcher?.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    Items.Clear();
                    foreach (ReportFile rf in updatedfiles)
                    {
                        var bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/unknown.png"));

                        if (rf.file_type == ".txt" || rf.file_type == ".doc" || rf.file_type == "docx")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/text.png"));
                        }
                        else if (rf.file_type == ".pptx")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/pptx.png"));
                        }
                        else if (rf.file_type == ".cs" || rf.file_type == ".java" || rf.file_type == ".cpp" || rf.file_type == ".c" || rf.file_type == ".class")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/code.png"));
                        }
                        else if (rf.file_type == ".jpeg" || rf.file_type == ".png" || rf.file_type == ".jpg" || rf.file_type == ".svg" || rf.file_type == ".ico")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/image.png"));
                        }
                        else if (rf.file_type == ".pdf" )
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/pdf.png"));
                        }
                        else if (rf.file_type == ".aif" || rf.file_type == ".mp3" || rf.file_type == ".wav" || rf.file_type == ".wma" || rf.file_type == ".mid")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/mp3.png"));
                        }
                        else if (rf.file_type == ".h264" || rf.file_type == ".3gp" || rf.file_type == ".m4v" || rf.file_type == ".mp4" || rf.file_type == ".mkv")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/mp4.png"));
                        }

                        var dateTime = rf.file_creation_date.ToDateTime();

                        Items.Add(new HistoryItemVm()
                        {
                            Name = rf.file_name,
                            Status = "Completed",
                            ReportType = rf.file_type,
                            RecieverID = rf.associated_patientId,
                            Date = dateTime.ToLongDateString() +" Time:- "+ dateTime.ToLongTimeString(),
                            Url = rf.file_url,
                            FileTypeImageSource = bitmapImage,
                            Size = "Unknown"
                        });
                    }
                }));
        }

        public async void LoadList()
        {
            Items = new ObservableCollection<HistoryItemVm>();

            var allFiles = await _cf.GetUploadedFiles();
            if (allFiles != null)
            {
                if (allFiles.Count > 0)
                {
                    foreach (ReportFile rf in allFiles)
                    {
                        var bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/unknown.png"));

                        if (rf.file_type == ".txt" || rf.file_type == ".doc" || rf.file_type == "docx")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/text.png"));
                        }
                        else if (rf.file_type == ".pptx")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/pptx.png"));
                        }
                        else if (rf.file_type == ".cs" || rf.file_type == ".java" || rf.file_type == ".cpp" || rf.file_type == ".c" || rf.file_type == ".class")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/code.png"));
                        }
                        else if (rf.file_type == ".jpeg" || rf.file_type == ".png" || rf.file_type == ".jpg" || rf.file_type == ".svg" || rf.file_type == ".ico")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/image.png"));
                        }
                        else if (rf.file_type == ".pdf")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/pdf.png"));
                        }
                        else if (rf.file_type == ".aif" || rf.file_type == ".mp3" || rf.file_type == ".wav" || rf.file_type == ".wma" || rf.file_type == ".mid")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/mp3.png"));
                        }
                        else if (rf.file_type == ".h264" || rf.file_type == ".3gp" || rf.file_type == ".m4v" || rf.file_type == ".mp4" || rf.file_type == ".mkv")
                        {
                            bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/mp4.png"));
                        }
                        var dateTime = rf.file_creation_date.ToDateTime();

                        Items.Add(new HistoryItemVm()
                        {
                            Name = rf.file_name,
                            Status = "Completed",
                            RecieverID = rf.associated_patientId,
                            Date = dateTime.ToLongDateString() + " Time:- " + dateTime.ToLongTimeString(),
                            Url = rf.file_url,
                            FileTypeImageSource = bitmapImage,
                            Size = "Unknown",
                            ReportType = rf.file_type
                        });
                    }
                }
            }
        }
    }
}