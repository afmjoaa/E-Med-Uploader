using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace custom_window
{
    public class SideListDm : SideListVm
    {
        public static SideListDm Instance => new SideListDm();

        public SideListDm()
        {
            Items = new List<SideItemVm>
            {
                new SideItemVm
                {
                    Name = "Home",
                    IsSelected = true,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/SideMenu/home.png"))
                },
                new SideItemVm
                {
                    Name = "History",
                    IsSelected = false,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/SideMenu/history.png"))
                },
                new SideItemVm
                {
                    Name = "Statistics",
                    IsSelected = false,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/SideMenu/state.png"))
                },
                new SideItemVm
                {
                    Name = "Settings",
                    IsSelected = false,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/SideMenu/setting.png"))
                },
                new SideItemVm
                {
                    Name = "About",
                    IsSelected = false,
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/SideMenu/info.png"))
                },
            };
        }
    }
}