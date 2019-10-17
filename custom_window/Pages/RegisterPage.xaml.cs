using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Standard;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;
using MaterialDesignThemes.Wpf;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class RegisterPage : BasePage<RegisterViewModel>
    {
        private CloudFirestoreService _cloudFirestoreService = null;

        public RegisterPage()
        {
            InitializeComponent();
            _cloudFirestoreService = CloudFirestoreService.GetInstance();
        }

        private void Hospital_name_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("hospital_name_icon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void Hospital_name_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("hospital_name_icon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void Hospital_id_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosIdIcon = (PackIcon) this.FindName("hospital_id_icon");
            hosIdIcon.Foreground = Brushes.OrangeRed;
        }

        private void Hospital_id_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosIdIcon = (PackIcon) this.FindName("hospital_id_icon");
            hosIdIcon.Foreground = Brushes.Gray;
        }

        private void Contact_number_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var contactNameIcon = (PackIcon) this.FindName("contact_number_icon");
            contactNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void Contact_number_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var contactNameIcon = (PackIcon) this.FindName("contact_number_icon");
            contactNameIcon.Foreground = Brushes.Gray;
        }

        private void Contact_mail_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var contactMailIcon = (PackIcon) this.FindName("contact_mail_icon");
            contactMailIcon.Foreground = Brushes.OrangeRed;
        }

        private void Contact_mail_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var contactMailIcon = (PackIcon) this.FindName("contact_mail_icon");
            contactMailIcon.Foreground = Brushes.Gray;
        }

        private void Hos_reg_number_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosRegNumIcon = (PackIcon) this.FindName("hos_reg_number_icon");
            hosRegNumIcon.Foreground = Brushes.OrangeRed;
        }

        private void Hos_reg_number_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosRegNumIcon = (PackIcon) this.FindName("hos_reg_number_icon");
            hosRegNumIcon.Foreground = Brushes.Gray;
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            //proceed...
            var hospital = new Hospital
            {
                hospital_name = hospital_name.Text,
                hospital_id = hospital_id.Text,
                hospital_email = contact_mail.Text,
                hospital_phone_number = contact_number.Text,
                hospital_registration_num = hos_reg_number.Text,
                hospital_pass = "123",
                //ho
            };
            _cloudFirestoreService.AddHospital(hospital);
            Debug.WriteLine(hospital_name.Text);
            ToastClass.NotifyMin("Successfully Registered!!", "You may Login now using default password: 123 :(");
        }

        private void Skip_Button_Click(object sender, RoutedEventArgs e)
        {
            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Login);
        }
    }
}