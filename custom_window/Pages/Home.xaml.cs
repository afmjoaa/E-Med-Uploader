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

        //fp width 288, height:375
        private CloudFirestoreService _cfService = null;

        public Home()
        {
            InitializeComponent();
            fpDeviceHelper = new FingerprintHelper();
            fpDeviceHelper.onCaptureCallBackEvents += OnFingerprintCaptured;
            _cfService = CloudFirestoreService.GetInstance();
        }

        private void InitButton(object sender, RoutedEventArgs e)
        {
            fpDeviceHelper.InitDevice();
            fpDeviceHelper.OpenDevice();
        }

        private async void OnFingerprintCaptured(string templateString, byte[] templateBlob)
        {
            fp_textblock.Dispatcher?.Invoke(() =>
            {
                fp_textblock.Height = 500;
                fp_textblock.Width = 400;
                fp_textblock.Selection.Text = "\ntemplate: " + templateString;
            });
            // add or check for existing patient having this fingerprint 
            var matchedPatient = await FindPatientByFingerprint(templateString);

            if (matchedPatient != null)
            {
                ToastClass.NotifyMin("Welcome " + matchedPatient.patient_name, "We have Identified you!");
            }
        }

        private async Task<Patient> FindPatientByFingerprint(string template)
        {
            if (_cfService == null) _cfService = CloudFirestoreService.GetInstance();

            Patient ret = null;
            var patients = await _cfService.getAllPatients();

            var score = 0;
            string bestTemplate = null;

            var blob1 = Convert.FromBase64String(template);
            byte[] blob2 = null;

            foreach (var patient in patients)
            {
                if (string.IsNullOrEmpty(patient.patient_fingerprint_template_right_thumb)) continue;
                blob2 = Convert.FromBase64String(patient.patient_fingerprint_template_right_thumb);
                var cScore = fpDeviceHelper.CompareFingerPrint(blob1, blob2);
                if (cScore > score && cScore > 60)
                {
                    score = cScore;
                    ret = patient;
                    bestTemplate = patient.patient_fingerprint_template_right_thumb;
                }
            }

            return ret;
        }
    }
}