using System.Windows.Input;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.ViewModels.side.Design;

namespace custom_window.ViewModels.side
{
    public class SideItemVm : BaseViewModel
    {
       
        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public BitmapImage ImageSource { get; set; }

        public ICommand OpenClickedPageCommand { get; set; }

        public SideItemVm()
        {
            //create commands
            
            OpenClickedPageCommand = new RelayCommand(OpenPage);
        }

        private void OpenPage()
        {
            var Toast = new ToastClass();
            switch (Name)
            {
                case "Home":
                    SideListDm.Instance.Items.ForEach((value) => { value.IsSelected = value.Name == "Home"; });
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
                    break;
                case "History":
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.History);
                    SideListDm.Instance.Items.ForEach((value) => { value.IsSelected = value.Name == "History"; });
                    break;
                case "Statistics":
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Statistics);

                    SideListDm.Instance.Items.ForEach((value) => { value.IsSelected = value.Name == "Statistics"; });
                    break;
                case "Settings":
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Settings);

                    SideListDm.Instance.Items.ForEach((value) => { value.IsSelected = value.Name == "Settings"; });
                    break;
                case "About":
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.About);
                    SideListDm.Instance.Items.ForEach((value) => { value.IsSelected = value.Name == "About"; });
                    break;
                default:
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
                    Toast.ShowNotification("default", "default Clicked", 300);
                    break;
            }
        }
    }
}
