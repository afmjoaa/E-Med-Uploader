using System;
using Firebase.Auth;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using custom_window.Core;
using System.Threading.Tasks;
using System.Windows.Input;
using custom_window.HelperClasses;
using custom_window.HelperClasses.ApplicationScope;
using custom_window.HelperClasses.AuthHelper;
using custom_window.HelperClasses.DataModels;
using custom_window.HelperClasses.MailAuthService;
using custom_window.ViewModels.side;
using Hospital = custom_window.HelperClasses.DataModels.Hospital;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : BasePage<LoginViewModel>, IHavePassword
    {
        #region Init

        private CloudFirestoreService cfService = null;

        #endregion

        #region Constructor

        public LoginPage()
        {
            InitializeComponent();
            cfService = CloudFirestoreService.GetInstance();
        }

        #endregion

        #region relay functions

        private async void google_OnClick(object sender, RoutedEventArgs e)
        {
            var Toast = new ToastClass();
            Toast.ShowNotification("Twitter Authentication", "Twitter authentication is coming soon...", 200);

        }

        private void fb_OnClick(object sender, RoutedEventArgs e)
        {
            //testing the patient slide in the main application
            /*IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
            IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;*/
            var Toast = new ToastClass();
            Toast.ShowNotification("Facebook Authentication", "Facebook authentication is coming soon...", 200);
        }

        private void twitter_OnClick(object sender, RoutedEventArgs e)
        {
            var Toast = new ToastClass();
            Toast.ShowNotification("Twitter Authentication", "Twitter authentication is coming soon...", 200);
        }

        #endregion

        #region iconColorFix

        private void Phone_number_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("phone_Num_ico");
            phoneNumberIcon.Foreground = Brushes.DodgerBlue;
        }

        private void Phone_number_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("phone_Num_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("password_ico");
            phoneNumberIcon.Foreground = Brushes.DodgerBlue;
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("password_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;
        }

        private void Code_GotFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("code_ico");
            phoneNumberIcon.Foreground = Brushes.DodgerBlue;
        }

        private void Code_LostFocus(object sender, RoutedEventArgs e)
        {
            var phoneNumberIcon = (PackIcon) this.FindName("code_ico");
            phoneNumberIcon.Foreground = Brushes.Gray;
        }

        #endregion

        public SecureString SecurePassword => password.SecurePassword;


        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(loginButton, true);
            loginButton.IsEnabled = false;

            var emailText = phone_number.Text;
            var pass = password.Password;

            //validate the email pass
            if (string.IsNullOrEmpty(emailText) || string.IsNullOrEmpty(pass))
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "Email or Password file is empty !!",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
                loginButton.IsEnabled = true;
                return;
            }

            //sign in flow
            try
            {
                FirebaseAuthLink authLink = await cfService.authProvider.SignInWithEmailAndPasswordAsync(emailText, pass);

                //save to the local cache
                AuthHelper.SaveUserToSettings(authLink);
                Properties.Settings.Default.isLogedIn = true;
                Properties.Settings.Default.Save();

                //TODO check if user Registered or not
                CancellationToken cancellationToken = new CancellationToken(false);
                Hospital hospital = await ApplicationData.Instance.GetUserHospital(cancellationToken);
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
                loginButton.IsEnabled = true;
                if (hospital != null)
                {
                    SideMenuVm.Instance.UpdateSideMenu();
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
                }
                else
                {
                    IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Register);
                }
            }
            catch (FirebaseAuthException exception)
            {
                string reason = exception.Reason.ToString();
                reason = string.Concat(reason.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = reason,
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
                loginButton.IsEnabled = true;
            }
        }


        private async void SignUpBtnOnClick(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(signUpBtn, true);
            signUpBtn.IsEnabled = false;
            //get the password and email here
            var emailText = phone_number.Text;
            var mPassword = password.Password;
            var confirmPass = code.Password;

            if (string.IsNullOrEmpty(emailText) || string.IsNullOrEmpty(mPassword) || string.IsNullOrEmpty(confirmPass))
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "No Field Can be empty !!",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(signUpBtn, false);
                signUpBtn.IsEnabled = true;
                return;
            }
            else if (mPassword != confirmPass)
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "Passwords doesn't match..",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(signUpBtn, false);
                signUpBtn.IsEnabled = true;
                return;
            }

            //testing my code here 
            try
            {
                FirebaseAuthLink newUser =
                    await cfService.authProvider.CreateUserWithEmailAndPasswordAsync(emailText, mPassword, "", true);

                
                AuthHelper.SaveUserToSettings(newUser);
                Properties.Settings.Default.isLogedIn = true;
                Properties.Settings.Default.Save();

                ButtonProgressAssist.SetIsIndicatorVisible(signUpBtn, false);
                signUpBtn.IsEnabled = true;
                //now send to register page ...
                //don't need to check as only new user possible
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Register);
            }
            catch (FirebaseAuthException exception)
            {
                string reason = exception.Reason.ToString();
                reason = string.Concat(reason.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = reason,
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(signUpBtn, false);
                signUpBtn.IsEnabled = true;
            }
        }

        private async void ForgetPassBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //show new dialogBox with email field...
            await IoC.UI.ShowForgetPassBlock(new ForgetPassViewModel()
            {
                Title = "Forget Password",
                OkText = "Okay",
                PreRetrievalMessage = "Please Provide your Email address so that we can send reset password mail...",
                PreRetrievalBtnText = "Send Mail"
            });
        }


        #region justUIFuncitions

        private void New_User_Button_Click(object sender, RoutedEventArgs e)
        {
            /*hidding login items*/
            newUserBtn.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Collapsed;
            forgetPassBtn.Visibility = Visibility.Collapsed;

            /*showing register items*/
            haveActBtn.Visibility = Visibility.Visible;
            signUpBtn.Visibility = Visibility.Visible;
            emailBlock.Visibility = Visibility.Visible;
            codeBlock.Visibility = Visibility.Visible;
            passBlock.Visibility = Visibility.Visible;
        }

        private void Have_Account_Button_Click(object sender, RoutedEventArgs e)
        {
            /*hidding register items*/
            haveActBtn.Visibility = Visibility.Collapsed;
            signUpBtn.Visibility = Visibility.Collapsed;
            codeBlock.Visibility = Visibility.Collapsed;

            /*showing login items*/
            passBlock.Visibility = Visibility.Visible;
            emailBlock.Visibility = Visibility.Visible;
            newUserBtn.Visibility = Visibility.Visible;
            loginButton.Visibility = Visibility.Visible;
            forgetPassBtn.Visibility = Visibility.Visible;
        }

        #endregion
    }
}

//authProvider.Dispose();//removes the app configuration
//two way from two library admin and authentication
/*FirebaseAuth one = FirebaseAuth.DefaultInstance;
one.GetUserAsync()
authProvider.GetUserAsync()*/