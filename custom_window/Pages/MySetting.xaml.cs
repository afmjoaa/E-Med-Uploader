using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using custom_window.Core;
using custom_window.HelperClasses;

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
//            FolderListVm.Instance.myItem.Add(new FolderItemVm() {path = "hello World"});

            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

                var files = Directory.GetFiles(fbd.SelectedPath);

//                    System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                toast.ShowNotification("Selected Folder", fbd.SelectedPath, 200);

                var tempitems = FolderListVm.Instance.myItem;
                bool alreadyAdded = false;
                foreach (var it in tempitems)
                {
                    if (it.path == fbd.SelectedPath)
                    {
                        // dont add..
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                    FolderListVm.Instance.myItem.Add(new FolderItemVm() {path = fbd.SelectedPath});
            }
        }

        private void LogOutButtonClick(object sender, MouseButtonEventArgs e)
        {
            CloudFirestoreService.GetInstance().LogOut();
            // FileUploadService.GetInstance().Dispose();
            // close all devices & stop all services...

            FolderListVm.Instance.myItem.Clear();

            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
            IoC.Get<ApplicationViewModel>().SideMenuVisible = false;
        }

        private void ResetPassword_Button_Click(object sender, MouseButtonEventArgs e)
        {
            if (CloudFirestoreService.GetInstance()._isLoggedIn)
            {
//                CloudFirestoreService.GetInstance().ResetPassword(); TODO
            }
            else
            {
                ToastClass.NotifyMin("Fatal Error", "something very bad happened..");
            }
        }
    }
}