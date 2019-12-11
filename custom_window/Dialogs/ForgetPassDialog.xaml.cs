using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using custom_window.HelperClasses;
using Firebase.Auth;
using Firebase.Database;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for DialogMessageBox.xaml
    /// </summary>
    public partial class ForgetPassDialog : BaseDialogUserControl
    {
        private CloudFirestoreService cfService = null;

        public ForgetPassDialog()
        {
            InitializeComponent();
            cfService = CloudFirestoreService.GetInstance();

        }

        #region iconColorFix

        private void Phone_number_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon)this.FindName("phone_Num_ico");
            phoneNumberIcon.Foreground = Brushes.OrangeRed;
        }

        private void Phone_number_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon)this.FindName("phone_Num_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;
        }
        #endregion

        private async void SendResetMailBtnClicked(object sender, RoutedEventArgs e)
        {
            //validate the mail
            if (string.IsNullOrEmpty(phone_number.Text))
            {
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = "Email field is empty...";
                return;
            }
            ButtonProgressAssist.SetIsIndicatorVisible(SendMailBtn, true);


            try
            {
                await cfService.authProvider.SendPasswordResetEmailAsync(phone_number.Text);
                MainMessage.Text = "Reset Password mail is send to your mail, Please check your email...";
                ButtonProgressAssist.SetIsIndicatorVisible(SendMailBtn, false);

                CloseBtn.Visibility = Visibility.Visible;
                SendMailBtn.Visibility = Visibility.Collapsed;

                ErrorText.Visibility = Visibility.Hidden;
                emailBlock.Visibility = Visibility.Collapsed;

            }
            catch (FirebaseAuthException exception)
            {
                ButtonProgressAssist.SetIsIndicatorVisible(SendMailBtn, false);
                string reason = exception.Reason.ToString();
                reason = string.Concat(reason.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = reason;
            }
        }
    }
}
