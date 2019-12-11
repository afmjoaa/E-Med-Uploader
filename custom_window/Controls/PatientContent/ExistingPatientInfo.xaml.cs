using System.Windows;
using System.Windows.Controls;
using custom_window.Core;

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
            IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = false;
        }
    }
}