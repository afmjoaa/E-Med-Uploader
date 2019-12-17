using System.Windows;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.ViewModels.folder;

namespace custom_window.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogMessageBox.xaml
    /// </summary>
    public partial class ConfirmDialog : BaseDialogUserControl
    {
        public ConfirmDialog()
        {
            InitializeComponent();
        }

        private void YesBtnClicked(object sender, RoutedEventArgs e)
        {
            CloudFirestoreService.GetInstance().LogOut();

            // FileUploadService.GetInstance().Dispose();
            // close all devices & stop all services...
            FolderListVm.Instance.myItem.Clear();
            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
        }
    }
}
