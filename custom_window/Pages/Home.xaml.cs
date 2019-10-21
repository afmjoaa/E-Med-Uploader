using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Home : BasePage<HomeViewModel>
    {
        private FingerprintHelper fpDeviceHelper = null;
        private BarcodeHelper bcHelper = null;

        //fp width 288, height:375
        private CloudFirestoreService _cfService = null;


        public Home()
        {
            InitializeComponent();
            fpDeviceHelper = FingerprintHelper.GetInstance();
            fpDeviceHelper.onCaptureCallBackEvents += OnFingerprintCaptured;
            _cfService = CloudFirestoreService.GetInstance();
            bcHelper = BarcodeHelper.GetInstance();
            bcHelper.onBarcodeEvent += OnBarcodeEvent;
        }

        [STAThread]
        private void InitButton(object sender, RoutedEventArgs e)
        {
            fpDeviceHelper.InitDevice();
            fpDeviceHelper.OpenDevice();
            bcHelper.startListening();
        }

        private void OnBarcodeEvent(string barcodestring)
        {
            Console.WriteLine("New Barcode found: " + barcodestring);
            ToastClass.NotifyMin("New Barcode!", barcodestring);
        }

        private async void OnFingerprintCaptured(string templateString, byte[] templateBlob)
        {
            fp_textblock.Dispatcher?.Invoke(() =>
            {
                fp_textblock.Height = 500;
                fp_textblock.Width = 400;
                fp_textblock.Selection.Text = "\ntemplate: " + templateString;
                Clipboard.SetText(templateString);
            });
            // add or check for existing patient having this fingerprint 
            var matchedPatient = await _cfService.FindPatientByFingerprint(templateString);
            if (matchedPatient != null)
            {
                FileUploadService.CurrentPatient = matchedPatient;
                ToastClass.NotifyMin("Welcome " + matchedPatient.patient_name, "We have Identified you!");
            }
            else
            {
                Debug.WriteLine("No such user having this fingerprint adding new user");
                var pat = new Patient();
                pat.patient_fingerprint_template_right_thumb = templateString;
                var r = new Random();
                pat.patient_id = "pt_" + r.Next().ToString();
                Debug.WriteLine("Receptionist might ask about your contact info to complete registration!");
                var patInfoPrompt = Prompt.ShowDialog("Enter Patient Name:", "Patient's' Info");

                pat.patient_name = patInfoPrompt;

                var patId = await _cfService.AddPatient(pat);
                Debug.WriteLine("new patient added: " + patId + "\n\n");

                FileUploadService.CurrentPatient = pat;
            }
        }
    }
}