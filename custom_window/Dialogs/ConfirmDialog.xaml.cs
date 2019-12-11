using System.Windows;
using System.Windows.Controls;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window
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
