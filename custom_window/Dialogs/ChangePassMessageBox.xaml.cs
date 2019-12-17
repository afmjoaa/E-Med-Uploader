using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using custom_window.HelperClasses;
using Firebase.Auth;
using MaterialDesignThemes.Wpf;

namespace custom_window.Dialogs
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
            var hosNameIcon = (PackIcon) this.FindName("CurrentPassIcon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void CurrentPass_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("CurrentPassIcon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void NewPass_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("NewPassIcon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void NewPass_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("NewPassIcon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void ConfirmPass_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("ConfirmPassIcon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void ConfirmPass_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("ConfirmPassIcon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private async void ResetPasswordBtnClicked(object sender, RoutedEventArgs e)
        {
            var currentPassText = CurrentPass.Text;
            var newPassText = NewPass.Text;
            var confirmPassText = ConfirmPass.Text;


            //FinishingBtn: FinishingText: ErrorText: restPassBtn: CancelBtn

            ButtonProgressAssist.SetIsIndicatorVisible(restPassBtn, true);
            restPassBtn.IsEnabled = false;
            ErrorText.Visibility = Visibility.Collapsed;

            //validation
            if (string.IsNullOrEmpty(currentPassText) || string.IsNullOrEmpty(newPassText) ||
                string.IsNullOrEmpty(confirmPassText))
            {
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = "No field can be empty...";
                ButtonProgressAssist.SetIsIndicatorVisible(restPassBtn, false);
                restPassBtn.IsEnabled = true;
                return;
            }
            else if (newPassText != confirmPassText)
            {
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = "New password and confirm password doesn't match...";
                ButtonProgressAssist.SetIsIndicatorVisible(restPassBtn, false);
                restPassBtn.IsEnabled = true;
                return;
            }

            //update pass
            try
            {
                await CloudFirestoreService.GetInstance().authProvider
                    .ChangeUserPassword(Properties.Settings.Default.federatedId, newPassText);
                ButtonProgressAssist.SetIsIndicatorVisible(restPassBtn, false);
                restPassBtn.IsEnabled = true;

                //hide all thing and show msg
                MainBody.Visibility = Visibility.Collapsed;

                FinishingText.Text =
                    $"Dear {Properties.Settings.Default.displayName} your password have been changed....";
                FinishingText.Visibility = Visibility.Visible;
                FinishingBtn.Visibility = Visibility.Visible;

                //this is not possible to get the idToken
                //don't do anything for now..........

            }
            catch (FirebaseAuthException exception)
            {
                string reason = exception.Reason.ToString();
                reason = string.Concat(reason.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = reason;
                ButtonProgressAssist.SetIsIndicatorVisible(restPassBtn, false);
                restPassBtn.IsEnabled = true;
            }
        }
    }
}