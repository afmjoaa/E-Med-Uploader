using System;
using System.Windows.Media.Imaging;

namespace custom_window.ViewModels.side.Design
{
    public class SideItemDm : SideItemVm
    {
        public static SideItemDm Instance => new SideItemDm();

        public SideItemDm()
        {
            Name = "Home";
            IsSelected = true;
            ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/SideMenu/home.png"));
        }
    }
}
