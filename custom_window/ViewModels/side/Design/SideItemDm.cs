using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace custom_window
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
