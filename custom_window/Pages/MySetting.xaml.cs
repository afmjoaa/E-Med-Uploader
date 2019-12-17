using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.ViewModels.folder;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class MySetting : BasePage<SettingViewModel>
    {
        public MySetting()
        {
            InitializeComponent();
        }

        private void AddNewFolderToWatch(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var toast = new ToastClass();

            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

                var files = Directory.GetFiles(fbd.SelectedPath);

                toast.ShowNotification("Selected Folder", fbd.SelectedPath, 200);

                var alreadyWatchingFolders = FolderListVm.Instance.myItem;
                bool alreadyAdded = false;
                foreach (var it in alreadyWatchingFolders)
                {
                    if (it.path == fbd.SelectedPath)
                    {
                        // dont add..
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                {
                    if (Properties.Settings.Default.watchFolder == null) { Properties.Settings.Default.watchFolder = new StringCollection(); }
                    
                    Properties.Settings.Default.watchFolder.Add(fbd.SelectedPath);
                    var indexOf = Properties.Settings.Default.watchFolder.IndexOf(fbd.SelectedPath);

                    if (Properties.Settings.Default.watchFolderState == null) { Properties.Settings.Default.watchFolderState = new StringCollection(); }
                        
                    Properties.Settings.Default.watchFolderState.Insert(indexOf, "true");
                    Properties.Settings.Default.Save();

                    var folderItemVm = new FolderItemVm()
                    {
                        path = fbd.SelectedPath,
                        playBtnEnable = false,
                        pauseBtnEnable = true
                    };
                    folderItemVm.PlayFolderWatching();
                    FolderListVm.Instance.myItem.Add(folderItemVm);
                }

            }
        }

        private async void LogOutButtonClick(object sender, MouseButtonEventArgs e)
        {
            await IoC.UI.ShowConfirmMessage(new ConfirmDialogViewModel()
            {
                Title = "Confirm Logout",
                Message = "Are your sure you wanna logout ?",
                yesText = "Yes",
                noText = "     Nope     ",
                dialogType = "logout_confirmation"
            });
        }

        private void ResetPassword_Button_Click(object sender, MouseButtonEventArgs e)
        {
            IoC.UI.ShowChangePassBlock(new ChangePassViewModel
            {
                Title = "Reset Password",
            });
        }
    }
}