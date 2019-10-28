using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for ChangePassMessageBox.xaml
    /// </summary>
    public partial class ChangePassMessageBox : BaseDialogUserControl
    {
        public ChangePassMessageBox()
        {
            InitializeComponent();
        }

        private void CurrentPass_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("CurrentPassIcon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void CurrentPass_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("CurrentPassIcon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void NewPass_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("NewPassIcon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }
        private void NewPass_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("NewPassIcon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void ConfirmPass_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("ConfirmPassIcon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void ConfirmPass_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("ConfirmPassIcon");
            hosNameIcon.Foreground = Brushes.Gray;
        }
    }
}