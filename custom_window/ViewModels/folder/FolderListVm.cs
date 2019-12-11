using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using custom_window.Core;

namespace custom_window
{
    public class FolderListVm : BaseViewModel
    {
        private static FolderListVm instance = null;
        public ObservableCollection<FolderItemVm> myItem { get; set; }

        #region constructor

        public FolderListVm()
        {
            myItem = new ObservableCollection<FolderItemVm>();
        }

        public static FolderListVm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FolderListVm();
                }

                return instance;
            }
        }

        #endregion

        public List<FolderItemVm> Items { get; set; }
    }
}