using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for ActionListView.xaml
    /// </summary>
    public partial class FolderListView : UserControl
    {
        public List<FolderItemVm> MyFolderItemList { get; set; }

        public FolderListView()
        {
            MyFolderItemList = new List<FolderItemVm>
            {
                new FolderItemVm()
                {
                    path = "hello World"
                }
            };

            InitializeComponent();
        }
    }
}