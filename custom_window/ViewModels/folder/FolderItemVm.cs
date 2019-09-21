using System;
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

        private void PlayFolderWatching()
        {
            var Toast = new ToastClass();
//            Toast.ShowNotification("Watching started", path, 100);
        }

        private void PauseFolderWatching()
        {
            var Toast = new ToastClass();
//            Toast.ShowNotification("Watching Paused", path, 300);
        }

        private void DeleteFolderWatching()
        {
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
    }
}