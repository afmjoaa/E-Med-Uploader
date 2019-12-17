using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window.ViewModels.folder
{
    public class FolderItemVm : BaseViewModel
    {
        public string path { get; set; }

        public bool playBtnEnable { get; set; }
        public bool pauseBtnEnable { get; set; }


        public static Dictionary<string, WatcherService> _watchers = new Dictionary<string, WatcherService>();


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

            //initialize play in from the caller after initialize the path from Mysetting.cs
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void PlayFolderWatching()
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

            playBtnEnable = false;
            pauseBtnEnable = true;
            var itemIndex = 0;

            if (Properties.Settings.Default.watchFolder != null && Properties.Settings.Default.watchFolderState != null)
            {
                foreach (var currentItem in Properties.Settings.Default.watchFolder)
                {
                    if (currentItem == path)
                    {
                        Properties.Settings.Default.watchFolderState.RemoveAt(itemIndex);
                        Properties.Settings.Default.watchFolderState.Insert(itemIndex, "true");
                        Properties.Settings.Default.Save();
                        break;
                    }

                    itemIndex++;
                }
            }
        }

        private void PauseFolderWatching()
        {
            GetWatcher(path).Dispose();
            _watchers.Remove(path);

            playBtnEnable = true;
            pauseBtnEnable = false;

            var itemIndex = 0;
            if (Properties.Settings.Default.watchFolder != null && Properties.Settings.Default.watchFolderState != null)
            {
                foreach (var currentItem in Properties.Settings.Default.watchFolder)
                {
                    if (currentItem == path)
                    {
                        Properties.Settings.Default.watchFolderState.RemoveAt(itemIndex);
                        Properties.Settings.Default.watchFolderState.Insert(itemIndex, "false");
                        Properties.Settings.Default.Save();
                        break;
                    }

                    itemIndex++;
                }
            }
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
                    FolderListVm.Instance.myItem.RemoveAt(itemIndex);
                    break;
                }

                itemIndex++;
            }

            //delete from the properties
            itemIndex = 0;
            if (Properties.Settings.Default.watchFolder != null && Properties.Settings.Default.watchFolderState != null)
            {
                foreach (var currentItem in Properties.Settings.Default.watchFolder)
                {
                    if (currentItem == path)
                    {
                        Properties.Settings.Default.watchFolder.RemoveAt(itemIndex);
                        Properties.Settings.Default.watchFolderState.RemoveAt(itemIndex);
                        Properties.Settings.Default.Save();
                        break;
                    }

                    itemIndex++;
                }
            }
        }

        private WatcherService GetWatcher(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (_watchers.ContainsKey(path)) return _watchers[path];
            var ret = new WatcherService(path);
            _watchers.Add(path, ret);
            return ret;
        }

        public static void DisposeAllWatchers()
        {
            foreach (var ww in _watchers)
            {
                ww.Value.Dispose();
            }
        }
    }
}