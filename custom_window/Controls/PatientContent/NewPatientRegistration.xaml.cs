using System.Windows.Controls;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for NoAcc.xaml
    /// </summary>
    public partial class NewPatientRegistration : UserControl
    {
        private BarcodeHelper _bar;
        private FingerprintHelper _fp;

        public NewPatientRegistration()
        {
            InitializeComponent();
            _bar = BarcodeHelper.GetInstance();
            _fp = FingerprintHelper.GetInstance();

            _bar.modifiedModifiedBarcodeEvent += OnModifiedModifiedBarcodeEvent;
        }

        private void OnModifiedModifiedBarcodeEvent(string patientname, string oldnid, string newnid,
            string dateofbirth)
        {
            reg_name_tb.Dispatcher.Invoke(new System.Action(()=> {
                reg_name_tb.Text = patientname;
            }));
            // get data for registration   
            ToastClass.NotifyMin("New data for fingerprint", "getting ready");
        }

        private void Register_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
           // _bar.modifiedModifiedBarcodeEvent -= OnModifiedModifiedBarcodeEvent;
        }

        private void Skip_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
           // _bar.modifiedModifiedBarcodeEvent -= OnModifiedModifiedBarcodeEvent;
        }
    }
}