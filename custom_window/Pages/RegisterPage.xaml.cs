using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ControlzEx.Standard;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;
using Firebase.Database;
using Google.Cloud.Firestore;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class RegisterPage : BasePage<RegisterViewModel>
    {
        private FileUploadService _fileUploadService = null;
        private CloudFirestoreService _cloudFirestoreService = null;
        private bool imageUploadFlag = false;

        public RegisterPage()
        {
            InitializeComponent();
            _cloudFirestoreService = CloudFirestoreService.GetInstance();
            _fileUploadService = FileUploadService.GetInstance();
        }

        #region textViewColorFix

        private void Hospital_name_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("hospital_name_icon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void Hospital_name_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("hospital_name_icon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void Contact_number_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var contactNameIcon = (PackIcon)this.FindName("contact_number_icon");
            contactNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void Contact_number_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var contactNameIcon = (PackIcon)this.FindName("contact_number_icon");
            contactNameIcon.Foreground = Brushes.Gray;
        }


        private void Hos_reg_number_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosRegNumIcon = (PackIcon)this.FindName("hos_reg_number_icon");
            hosRegNumIcon.Foreground = Brushes.OrangeRed;
        }

        private void Hos_reg_number_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosRegNumIcon = (PackIcon)this.FindName("hos_reg_number_icon");
            hosRegNumIcon.Foreground = Brushes.Gray;
        }

        #endregion


        private async void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(PreceedBtn, true);
            PreceedBtn.IsEnabled = false;
            //check validation here 
            if (string.IsNullOrEmpty(hospital_name.Text) || string.IsNullOrEmpty(contact_number.Text) || string.IsNullOrEmpty(hos_reg_number.Text))
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "No Field Can be empty !!",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(PreceedBtn, false);
                PreceedBtn.IsEnabled = true;
                return;
            }else if (contact_number.Text.Length != 11) {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "Phone number must be 11 digit...",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(PreceedBtn, false);
                PreceedBtn.IsEnabled = true;
                return;
            }else if (!imageUploadFlag)
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Upload Information",
                    Message = "Please upload a hospital logo...",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(PreceedBtn, false);
                PreceedBtn.IsEnabled = true;
                return;
            }

            //update user 
            Properties.Settings.Default.displayName = hospital_name.Text;
            Properties.Settings.Default.phoneNumber = contact_number.Text;
            Properties.Settings.Default.Save();

            //update user in auth

            await _cloudFirestoreService.authProvider.UpdateProfileAsync(Properties.Settings.Default.token,
                hospital_name.Text, Properties.Settings.Default.photoUrl);

            //uploading the hospital registration data
            var hospital = new Hospital
            {
                hospitalName = hospital_name.Text,
                registrationNumber = hos_reg_number.Text,
                mobileNumber = contact_number.Text,
                email = Properties.Settings.Default.email,
                photoUrl = Properties.Settings.Default.photoUrl,
                uid = Properties.Settings.Default.localId,
                isEmailVerified = Properties.Settings.Default.isEmailverified,
                firebaseAuthToken = Properties.Settings.Default.token,
                location = new GeoPoint()

            };

            try
            {
                await _cloudFirestoreService.AddHospital(hospital);
                ButtonProgressAssist.SetIsIndicatorVisible(PreceedBtn, false);
                PreceedBtn.IsEnabled = true;
                SideMenuVm.Instance.UpdateSideMenu();
                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
            }
            catch (Exception exception)
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = exception.Message,
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(PreceedBtn, false);
                PreceedBtn.IsEnabled = true;

            }
            
        }

        private void Skip_Button_Click(object sender, RoutedEventArgs e)
        {
            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
        }

        private void UploadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            
            if (op.ShowDialog() == true)
            {
                HospitalImage.ImageSource = new BitmapImage(new Uri(op.FileName));
                imageUploadFlag = true;
            }

            _fileUploadService.UploadHospitalDisplayPic(op.OpenFile(), Properties.Settings.Default.localId);
            
            //upload the file and update the user and local setting
        }
    }
}