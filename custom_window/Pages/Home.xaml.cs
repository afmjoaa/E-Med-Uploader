using System;
using System.Diagnostics;
using System.Threading;
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

        #region constructor and init

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
            init();
        }

        private void init()
        {
            fpDeviceHelper.InitDevice();
            fpDeviceHelper.OpenDevice();
            bcHelper.InitDevice();
        }

        private void InitButton(object sender, RoutedEventArgs e)
        {
            fpDeviceHelper.InitDevice();
            fpDeviceHelper.OpenDevice();
            bcHelper.InitDevice();
        }

        #endregion

        #region editText color

        private void Combo_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("Combo_icon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void Combo_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("Combo_icon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        private void identification_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("identification_icon");
            hosNameIcon.Foreground = Brushes.OrangeRed;
        }

        private void identification_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("identification_icon");
            hosNameIcon.Foreground = Brushes.Gray;
        }

        #endregion

        private void OnFileUploadProgressUpdated(int percentage)
        {
            upload_progress.Dispatcher?.Invoke(new Action(() => { upload_progress.Value = percentage; }));
        }

        private async void OnModifiedBarcodeEvent(string patientName, string oldNid, string newNid, string dateOfBirth)
        {
            ToastClass.NotifyMin("New Barcode!", patientName + "\n" + oldNid + "\n" + newNid + "\n" + dateOfBirth);
            // wait for fingerprint and find the existing user or create new user using data from barcode
        }

        private async void OnFingerprintCaptured(string templateString, byte[] templateBlob)
        {
            /*fp_textblock.Dispatcher?.Invoke(() =>
            {
                fp_textblock.Height = 500;
                fp_textblock.Width = 400;
                fp_textblock.Selection.Text = "\ntemplate: " + templateString;
                Clipboard.SetText(templateString);
            });
            //this code save fingerPrint in the clipboard
            */

            //visible and new patient
            if (IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible &&
                IoC.Get<PatientInfoCheckViewModel>().CurrentContent == ContentType.NewPatientRegistration)
            {
                //inside register
            }
            //visible and existing patient
            else if (IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible &&
                     IoC.Get<PatientInfoCheckViewModel>().CurrentContent == ContentType.ExistingPatientInfo)
            {
                //don't do anything
            }
            //not visible
            else
            {
                //search database
                //1.found
                //2.not found
                // add or check for existing patient having this fingerprint 
                var foundPatient = await _cfService.FindPatientByFingerprint(templateString);
                if (foundPatient != null)
                {
                    //Found
                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;

                    FileUploadService.CurrentPatient = foundPatient;
                    ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");


                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates);

                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }
                else
                {
                    //Not Found
                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.NewPatientRegistration;
                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;

                    Debug.WriteLine("No such user having this fingerprint adding new user");
                    var pat = new Patient();
                    try
                    {
                        pat.fingerprint_templates[0] = templateString;
                        var r = new Random();
                        pat.id = "pt_" + r.Next().ToString();
                        Debug.WriteLine("Receptionist might ask about your contact info to complete registration!");


                        //updating patient info
                        var patId = await _cfService.AddPatient(pat);
                        //_cfService.authProvider.SignInAnonymouslyAsync();

                        Debug.WriteLine("new patient added: " + patId + "\n\n");
                        FileUploadService.CurrentPatient = pat;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
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

        private async void CriteriaSearchBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string phone = "By Phone Number";
            string mail = "By Phone Number";
            string newNid = "By Phone Number";

            ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, true);


            if (string.IsNullOrEmpty(SearchField.Text) || string.IsNullOrEmpty(SearchCriteria.Text))
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "Search Field or Criteria can't be empty",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, false);
                return;
            }


            //first search the database find the user 
            //if found then show 
            //if not found then show the newPatient register

            string value = SearchCriteria.Text;
            if (value == phone)
            {
                var cancellationToken = new CancellationToken();
                //phone search first
                var foundPatient = await _cfService.FindPatientBy("phone", SearchField.Text, cancellationToken);
                if (foundPatient != null)
                {
                    ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");

                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
                    //update view
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates);

                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }
                else
                {
                    ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the phone number");
                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.NewPatientRegistration;
                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }
                ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, false);
            }
            else if (value == mail)
            {
                var cancellationToken = new CancellationToken();
                //mail search first
                var foundPatient = await _cfService.FindPatientBy("email", SearchField.Text, cancellationToken);
                if (foundPatient != null)
                {
                    ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");

                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
                    //update view
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates);

                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }
                else
                {
                    ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the email");
                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.NewPatientRegistration;
                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }

                ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, false);

            }
            else
            {
                var cancellationToken = new CancellationToken();
                //newNid search first
                var foundPatient = await _cfService.FindPatientBy("new_nid", SearchField.Text, cancellationToken);
                if (foundPatient != null)
                {
                    ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");

                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
                    //update view
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates);

                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }
                else
                {
                    ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the nid no");
                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.NewPatientRegistration;
                    IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = true;
                }
                ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, false);
            }
        }
    }
}