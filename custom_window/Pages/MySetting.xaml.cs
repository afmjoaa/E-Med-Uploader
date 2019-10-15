using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class MySetting : BasePage<SettingViewModel>
    {
        public MySetting()
        {
            InitializeComponent();
        }

        private void AddNewFolderToWatch(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var toast = new ToastClass();
//            FolderListVm.Instance.myItem.Add(new FolderItemVm() {path = "hello World"});

            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

                var files = Directory.GetFiles(fbd.SelectedPath);

//                    System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                toast.ShowNotification("Selected Folder", fbd.SelectedPath, 200);
                FolderListVm.Instance.myItem.Add(new FolderItemVm() {path = fbd.SelectedPath});
            }
        }
    }
}