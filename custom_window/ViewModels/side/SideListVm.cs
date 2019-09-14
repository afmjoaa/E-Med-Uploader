using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using custom_window.Core;

namespace custom_window
{
    public class SideListVm :BaseViewModel
    {
       public List<SideItemVm> Items { get; set; }
        
    }
}
