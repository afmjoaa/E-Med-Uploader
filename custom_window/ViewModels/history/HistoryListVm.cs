using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using custom_window.Core;

namespace custom_window
{
    public class HistoryListVm : BaseViewModel
    {
//        public List<HistoryItemVm> Items { get; set; }
        public ObservableCollection<HistoryItemVm> Items { get; set; }
    }
}