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


        private HistoryListDm()
        {
            _cf = CloudFirestoreService.GetInstance();
            _cf.OnDbFileChanged += OnDbFileChanged;
            LoadList();
        }

        private void OnDbFileChanged(List<ReportFile> updatedfiles)
        {
            Application.Current.Dispatcher?.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    Items.Clear();
                    foreach (ReportFile rf in updatedfiles)
                    {
                        Items.Add(new HistoryItemVm()
                        {
                            Name = rf.file_name, Status = "Completed", RecieverID = rf.associated_patientId,
                            Date = rf.file_creation_date.ToString(), Url = rf.file_url
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
                        Items.Add(new HistoryItemVm()
                        {
                            Name = rf.file_name,
                            Status = "Completed",
                            RecieverID = rf.associated_patientId,
                            Date = rf.file_creation_date.ToString(),
                            Url = rf.file_url
                        });
                    }
                }
            }
        }
    }
}