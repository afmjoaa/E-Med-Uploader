using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window
{
    public class HistoryItemVm : BaseViewModel
    {
       
        public string Name { get; set; }
        public string ReportType { get; set; }
        public string RecieverID { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Size { get; set; }

        public BitmapImage FileTypeImageSource { get; set; }

        public ICommand OpenClickedItemCommand { get; set; }

        public HistoryItemVm()
        {
            //create commands
            OpenClickedItemCommand = new RelayCommand(OpenItem);
        }

        private void OpenItem()
        {
            var Toast = new ToastClass();
            switch (Name)
            {
                case "Home":
                    //SideListDm.Instance.Items.ForEach((value) => { value.IsSelected = value.Name == "Home"; });
                    //IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
                    Toast.ShowNotification("Name", Name, 300);
                    break;
                default:
                    //IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
                    Toast.ShowNotification("default", "default Clicked", 300);
                    break;
            }
        }
    }
}
