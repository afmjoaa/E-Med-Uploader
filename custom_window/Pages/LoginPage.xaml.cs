using System;
using System.Diagnostics;
using System.Security;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using custom_window.Core;
using System.Threading.Tasks;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : BasePage<LoginViewModel>, IHavePassword
    {
        private CloudFirestoreService cfService = null;

        public LoginPage()
        {
            InitializeComponent();
            cfService = CloudFirestoreService.GetInstance();
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

        private void Code_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon)this.FindName("code_ico");
            phoneNumberIcon.Foreground = Brushes.OrangeRed;
        }

        private void Code_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon)this.FindName("code_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;

        }

        #endregion

        public SecureString SecurePassword => password.SecurePassword;

        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            // await Task.Delay(20); // 50
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndicatorVisible(loginButton, true);
            var phoneNumber = phone_number.Text;
            // IMPORTANT: Never store unsecure password in variable like this
            var pass = password.Password;

            if (!validatePhoneAndPass(phoneNumber, pass)) return;

            //check login details.
            // if no account then go to register page
            // else go to home page
            var (hospital, error_code) = await cfService.Login(phoneNumber, pass);
            if (error_code == Constants.NO_ERROR)
            {
                Debug.WriteLine("Login success!");
                ToastClass.NotifyMin("Login Success!", "Welcome " + hospital.hospital_name + " !!");
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
            }
            else if (error_code == Constants.BAD_PASS)
            {
                ToastClass.NotifyMin("Login Failed!", "Wrong Username/Password combination.");

                //IoC.Get<ApplicationViewModel>().GoToPage();
                return;
            }
            else if (error_code == Constants.BAD_USER)
            {
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Register);
            }

        }

        private bool validatePhoneAndPass(string phone, string pass)
        {
            return !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pass);
        }

        private void New_User_Button_Click(object sender, RoutedEventArgs e)
        {
            /*hidding login items*/
            newUserBtn.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Collapsed;
            passBlock.Visibility = Visibility.Collapsed;
            registerButton.Visibility = Visibility.Collapsed;
            resendCodeButton.Visibility = Visibility.Collapsed;
            codeBlock.Visibility = Visibility.Collapsed;


            /*showing register items*/
            haveActBtn.Visibility = Visibility.Visible;
            sendCodeButton.Visibility = Visibility.Visible;
            emailBlock.Visibility = Visibility.Visible;

        }

        private void Have_Account_Button_Click(object sender, RoutedEventArgs e)
        {
            /*hidding register items*/
            haveActBtn.Visibility = Visibility.Collapsed;
            sendCodeButton.Visibility = Visibility.Collapsed;
            registerButton.Visibility = Visibility.Collapsed;
            resendCodeButton.Visibility = Visibility.Collapsed;
            codeBlock.Visibility = Visibility.Collapsed;

            /*showing login items*/
            passBlock.Visibility = Visibility.Visible;
            emailBlock.Visibility = Visibility.Visible;
            newUserBtn.Visibility = Visibility.Visible;
            loginButton.Visibility = Visibility.Visible;

        }

        private void SendCodeButton_OnClick(object sender, RoutedEventArgs e)
        {
            /*hidding login items*/
            emailBlock.Visibility = Visibility.Collapsed;
            sendCodeButton.Visibility = Visibility.Collapsed;
            newUserBtn.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Collapsed;
            passBlock.Visibility = Visibility.Collapsed;

            /*showing register items*/

            haveActBtn.Visibility = Visibility.Visible;
            registerButton.Visibility = Visibility.Visible;
            haveActBtn.Visibility = Visibility.Visible;
            resendCodeButton.Visibility = Visibility.Visible;
            codeBlock.Visibility = Visibility.Visible;




            /*changing send code btn text*/
            //sendCodeButtonText.Text = "Resend Code";
        }


        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
           IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Register);
        }
    }
}