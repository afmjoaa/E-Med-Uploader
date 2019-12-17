using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using custom_window.Core;
using custom_window.ViewModels;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new WindowViewModel(this);
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            //show overlay if we are not focused on the main window
            (DataContext as WindowViewModel).DimableOverlayVisible = true;
        }

        private void MainWindow_OnActivated(object sender, EventArgs e)
        {
            //hide overlay if we are focused on the main window 
            (DataContext as WindowViewModel).DimableOverlayVisible = false;

        }
    }
}
