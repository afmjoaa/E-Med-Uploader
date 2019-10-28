using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;
using MaterialDesignThemes.Wpf;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Home : BasePage<HomeViewModel>
    {
        #region Init

        private FingerprintHelper fpDeviceHelper = null;
        private BarcodeHelper bcHelper = null;

        private FileUploadService _fileUploadService = null;

        //fp width 288, height:375
        private CloudFirestoreService _cfService = null;

        #endregion

        #region tempPatient

        private Patient temporaryPatient = null;

        #endregion

        public Home()
        {
            InitializeComponent();
            fpDeviceHelper = FingerprintHelper.GetInstance();
            fpDeviceHelper.onCaptureCallBackEvents += OnFingerprintCaptured;

            _cfService = CloudFirestoreService.GetInstance();

            bcHelper = BarcodeHelper.GetInstance();
            bcHelper.modifiedModifiedBarcodeEvent += OnModifiedBarcodeEvent;

            _fileUploadService = FileUploadService.GetInstance();
            _fileUploadService.OnProgressUpdated += OnFileUploadProgressUpdated;
        }

        #region design functions

        private void Combo_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("Combo_icon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void Combo_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("Combo_icon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void identification_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("identification_icon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void identification_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon)this.FindName("identification_icon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        #endregion

        private void OnFileUploadProgressUpdated(int percentage)
        {
            upload_progress.Dispatcher?.Invoke(new Action(() => { upload_progress.Value = percentage; }));
        }

        private void InitButton(object sender, RoutedEventArgs e)
        {
            fpDeviceHelper.InitDevice();
            fpDeviceHelper.OpenDevice();
            bcHelper.InitDevice();
        }

        private async void OnModifiedBarcodeEvent(string patientName, string oldNid, string newNid, string dateOfBirth)
        {
            ToastClass.NotifyMin("New Barcode!", patientName + "\n" + oldNid + "\n" + newNid + "\n" + dateOfBirth);
            // wait for fingerprint and find the existing user or create new user using data from barcode
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

                IoC.Get<PatientInfoCheckViewModel>().SetWindowData(matchedPatient.patient_name,
                    matchedPatient.patient_old_nid, matchedPatient.patient_new_nid, matchedPatient.patient_birth,
                    matchedPatient.patient_email, matchedPatient.patient_address, matchedPatient.patient_phone);

                IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
            }
            else
            {
                Debug.WriteLine("No such user having this fingerprint adding new user");
                var pat = new Patient();
                pat.patient_fingerprint_template_right_thumb = templateString;
                var r = new Random();
                pat.patient_id = "pt_" + r.Next().ToString();
                Debug.WriteLine("Receptionist might ask about your contact info to complete registration!");

                // var patInfoPrompt = Prompt.ShowDialog("Enter Patient Name:", "Patient's' Info");

                // pat.patient_name = patInfoPrompt;

                var patId = await _cfService.AddPatient(pat);
                Debug.WriteLine("new patient added: " + patId + "\n\n");

                FileUploadService.CurrentPatient = pat;
            }
        }

        private void Fix_Button_Click(object sender, RoutedEventArgs e)
        {
            Close_Devices_ButtonClick(null, null);
            InitButton(null, null);
        }

        private void Close_Devices_ButtonClick(object sender, RoutedEventArgs e)
        {
            bcHelper.CloseDevice();
            fpDeviceHelper.CloseDevice();
        }

        private void NewPatientRegister_OnClick(object sender, RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.NewPatientRegistration;
            IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
        }

        private void CriteriaSearchBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //first search the database find the user 
            //if found then show 
            //if not found then show the newPatient register
            IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
            IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
        }
    }
}