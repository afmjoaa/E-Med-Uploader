using System.Collections.ObjectModel;
using custom_window.Core;

namespace custom_window.ViewModels.history
{
    public class HistoryListVm : BaseViewModel
    {
//        public List<HistoryItemVm> Items { get; set; }
        public ObservableCollection<HistoryItemVm> Items { get; set; }
    }
}