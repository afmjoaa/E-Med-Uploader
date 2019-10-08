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
        private Dictionary<string, Watcher> _watchers = new Dictionary<string, Watcher>();


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
            // var w = new Watcher(path);
            // new Thread(w.watch).Start(); 

            if (_watchers.ContainsKey(path) && _watchers[path].IsWatching)
            {
                Debug.WriteLine("Already watching this directory.");
                return;
            }

            Debug.WriteLine("started watching: " + path);
            var wt = GetWatcher(path);
            new Thread(wt.watch).Start();
        }

        private void PauseFolderWatching()
        {
            GetWatcher(path).Dispose();
            _watchers.Remove(path);
        }

        private void DeleteFolderWatching()
        {
            GetWatcher(path).Dispose();
            _watchers.Remove(path);


            var itemIndex = 0;

            foreach (var currentItem in FolderListVm.Instance.myItem)
            {
                if (currentItem.path == path)
                {
                    break;
                }

                itemIndex++;
            }

            FolderListVm.Instance.myItem.RemoveAt(itemIndex);

        }

        private Watcher GetWatcher(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (_watchers.ContainsKey(path)) return _watchers[path];
            var ret = new Watcher(path);
            _watchers.Add(path, ret);
            return ret;
        }
    }
}