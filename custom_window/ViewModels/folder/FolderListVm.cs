using System.Collections.Generic;
using System.Collections.ObjectModel;
using custom_window.Core;

namespace custom_window.ViewModels.folder
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