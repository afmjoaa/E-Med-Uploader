using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using custom_window.Core;

namespace custom_window
{
    public class SideItemVm : BaseViewModel
    {
        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public BitmapImage ImageSource { get; set; }

        
    }
}
