using System.Windows;
using System.Windows.Controls;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for PhoneSearch.xaml
    /// </summary>
    public partial class ExistingPatientInfo : UserControl
    {
        public ExistingPatientInfo()
        {
            InitializeComponent();
        }

        private void skipBtnClicked(object sender, RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().selectedPatientId = null;
            IoC.Get<PatientInfoCheckViewModel>().selectedPatientName = "No patient is selected";
            IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
            IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
        }

        private void acknowledgeBtnClicked(object sender, RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
            IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
        }
    }
}