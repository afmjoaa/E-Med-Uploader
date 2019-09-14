using System.Security;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using custom_window.Core;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>

    public partial class LoginPage : BasePage<LoginViewModel>, IHavePassword
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        #region iconColorFix

        private void Phone_number_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("phone_Num_ico");
            phoneNumberIcon.Foreground = Brushes.OrangeRed;
        }

        private void Phone_number_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("phone_Num_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("password_ico");
            phoneNumberIcon.Foreground = Brushes.OrangeRed;
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("password_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;
        }

        #endregion

        public SecureString SecurePassword => password.SecurePassword;
    }
}