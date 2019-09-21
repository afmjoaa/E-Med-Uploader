using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window
{
    public class FolderItemVm : BaseViewModel
    {
        public string path { get; set; }
        private Dictionary<string, Watcher> watchers = new Dictionary<string, Watcher>();


        public BitmapImage FolderTypeImageSource { get; set; }

        public ICommand ItemPlayCommand { get; set; }
        public ICommand ItemPauseCommand { get; set; }
        public ICommand ItemDeleteCommand { get; set; }

        public FolderItemVm()
        {
            //create commands
            ItemPlayCommand = new RelayCommand(PlayFolderWatching);
            ItemPauseCommand = new RelayCommand(PauseFolderWatching);
            ItemDeleteCommand = new RelayCommand(DeleteFolderWatching);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private async void PlayFolderWatching()
        {
            var Toast = new ToastClass();
           // Toast.ShowNotification("Watching started", path, 10);

            Debug.WriteLine("started");

            // var w = new Watcher(path);
            // new Thread(w.watch).Start(); 
            var wt = getWatcher(path);
            new Thread(wt.watch).Start();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine("changed");
        }

        private void PauseFolderWatching()
        {
            var Toast = new ToastClass();
//            Toast.ShowNotification("Watching Paused", path, 300);

            getWatcher(path).Dispose();
        }

        private void DeleteFolderWatching()
        {
            getWatcher(path).Dispose();
            var Toast = new ToastClass();
//            Toast.ShowNotification("Watching Deleted", path, 300);

            var idx = -1;

            for (var i = 0; i < FolderListVm.Instance.myItem.Count; i++)
            {
                if (FolderListVm.Instance.myItem[i].path == path)
                {
                    idx = i;
                    break;
                }
            }

            if (idx != -1)
            {
//                Toast.ShowNotification("Will delete:", idx.ToString(), 100);
                FolderListVm.Instance.myItem.RemoveAt(idx);
            }
        }


        private Watcher getWatcher(string path)
        {

            if (watchers.ContainsKey(path)) return watchers[path];

            var ret = new Watcher(path);
            watchers.Add(path,ret);
            return ret;
        }


    }
}