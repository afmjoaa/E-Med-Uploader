using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.DataModels;
using custom_window.HelperClasses.ZebraDeviceHelper;
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

        #region constructor and init

        public Home()
        {
            InitializeComponent();

            fpDeviceHelper = FingerprintHelper.GetInstance();
            fpDeviceHelper.onCaptureCallBackEvents += OnFingerprintCaptured;

            _cfService = CloudFirestoreService.GetInstance();

            bcHelper = BarcodeHelper.GetInstance();
            bcHelper.modifiedModifiedBarcodeEvent += OnModifiedBarcodeEvent;
            bcHelper.mobileModifiedBarcodeEvent += OnMobileModifiedBarcodeEvent;

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

        private async void OnModifiedBarcodeEvent(string patientname, string oldnid, string newnid,
            string dateofbirth, string present, string permanent, string votingArea, string issueDate, string myPk,
            string mySignature)
        {
            if (IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible) return;
            //search by nid 
            var cancellationToken = new CancellationToken();
            //newNid search first
            var foundPatient = await _cfService.FindPatientBy("new_nid", newnid, cancellationToken);
            if (foundPatient != null)
            {
                ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");
                //associate with fileUploader
                IoC.Get<PatientInfoCheckViewModel>().selectPatient(foundPatient.id, foundPatient.name);

                IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
                //update view
                IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                    foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                    foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                    foundPatient.display_pic,
                    foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates.Count < 4);

                IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.ExistingPatientInfo);
            }
            else
            {
                ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the nid no");
                //update the new registration window
                IoC.Get<PatientInfoCheckViewModel>().SetWindowData(patientname, "",
                    "", dateofbirth, permanent,
                    present, votingArea, issueDate,
                    "",
                    oldnid, newnid, false);
                //showing the window now
                IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.NewPatientRegistration);
            }
        }

        private async void OnMobileModifiedBarcodeEvent(string patientUid)
        {
            //on mobile patient screen scan...
            //retrieve the patient
            var foundPatient = await _cfService.RetrivePatientByUid(patientUid);
            //null check is not null existing acknowledge
            if (foundPatient != null)
            {
                ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");
                //associate with fileUploader
                IoC.Get<PatientInfoCheckViewModel>().selectPatient(foundPatient.id, foundPatient.name);

                //IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;


                //update view
                IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                    foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                    foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                    foundPatient.display_pic,
                    foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates.Count < 4);
                IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.ExistingPatientInfo);
            }
            else
            {
                ToastClass.NotifyMin("Not identifiable", "Sorry this is not a patient QR code..");
            }

        }

        private async void OnFingerprintCaptured(string templateString, byte[] templateBlob)
        {
            if (IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible) return;
            //search database
            var foundPatient = await _cfService.FindPatientByFingerprint(templateString);
            //found
            if (foundPatient != null)
            {
                //Found
                IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;

                //associate with fileUploader
                IoC.Get<PatientInfoCheckViewModel>().selectPatient(foundPatient.id, foundPatient.name);

                //notify
                //Console.WriteLine("Welcome " + foundPatient.name, "We have Identified you!");
                ToastClass.NotifyMin("Welcome " + foundPatient.name, "We have Identified you!");

                //update info in existing window
                IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                    foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                    foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                    foundPatient.display_pic,
                    foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates.Count < 4);

                IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.ExistingPatientInfo);
            }
            else
            {
                //Not Found
                ToastClass.NotifyMin("Not identifiable", "No patient found associated with the fingerPrint ");
                IoC.Get<PatientInfoCheckViewModel>().SetWindowData("", "",
                    "", "", "",
                    "", "", "",
                    "",
                    "", "", false);
                IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.NewPatientRegistration);
            }
        }

        private void NewPatientRegister_OnClick(object sender, RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.NewPatientRegistration);
        }

        private async void CriteriaSearchBtn_OnClick(object sender, RoutedEventArgs e)
        {
            string phone = "By Phone Number";
            string mail = "By Phone Number";
            string newNid = "By Phone Number";

            ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, true);
            CriteriaSearchBtn.IsEnabled = false;


            if (string.IsNullOrEmpty(SearchField.Text) || string.IsNullOrEmpty(SearchCriteria.Text))
            {
                await IoC.UI.ShowMessage(new DialogViewModel()
                {
                    Title = "Error Information",
                    Message = "Search Field or Criteria can't be empty",
                    OkText = "Okay"
                });
                ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, false);
                CriteriaSearchBtn.IsEnabled = true;

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
                    //associate with fileUploader
                    IoC.Get<PatientInfoCheckViewModel>().selectPatient(foundPatient.id, foundPatient.name);

                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;

                    //update view
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates.Count < 4);


                    IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.ExistingPatientInfo);
                }
                else
                {
                    ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the phone number");
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData("", "",
                        "", "", "",
                        "", "", "",
                        "",
                        "", "", false);
                    IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.NewPatientRegistration);
                }

                CriteriaSearchBtn.IsEnabled = true;
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
                    //associate with fileUploader
                    IoC.Get<PatientInfoCheckViewModel>().selectPatient(foundPatient.id, foundPatient.name);

                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
                    //update view
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates.Count < 4);

                    IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.ExistingPatientInfo);
                }
                else
                {
                    ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the email");
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData("", "",
                        "", "", "",
                        "", "", "",
                        "",
                        "", "", false);
                    IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.NewPatientRegistration);
                }

                CriteriaSearchBtn.IsEnabled = true;
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
                    //associate with fileUploader
                    IoC.Get<PatientInfoCheckViewModel>().selectPatient(foundPatient.id, foundPatient.name);

                    IoC.Get<PatientInfoCheckViewModel>().CurrentContent = ContentType.ExistingPatientInfo;
                    //update view
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData(foundPatient.name, foundPatient.phone,
                        foundPatient.email, foundPatient.birth, foundPatient.permanent_address,
                        foundPatient.present_address, foundPatient.voting_area, foundPatient.issue_date,
                        foundPatient.display_pic,
                        foundPatient.old_nid, foundPatient.new_nid, foundPatient.fingerprint_templates.Count < 4);

                    IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.ExistingPatientInfo);
                }
                else
                {
                    ToastClass.NotifyMin("Not identifiable", "Sorry no patient found associated with the nid no");
                    IoC.Get<PatientInfoCheckViewModel>().SetWindowData("", "",
                        "", "", "",
                        "", "", "",
                        "",
                        "", "", false);
                    IoC.Get<PatientInfoCheckViewModel>().ShowNewOrOld(ContentType.NewPatientRegistration);
                }

                CriteriaSearchBtn.IsEnabled = true;
                ButtonProgressAssist.SetIsIndicatorVisible(CriteriaSearchBtn, false);
            }
        }

        #region unused
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

        #endregion

        private void DiscardSelectedPatientBtnClicked(object sender, RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().selectPatient(null, null);
        }
    }
}
/*fp_textblock.Dispatcher?.Invoke(() =>
{
    fp_textblock.Height = 500;
    fp_textblock.Width = 400;
    fp_textblock.Selection.Text = "\ntemplate: " + templateString;
    Clipboard.SetText(templateString);
});
//this code save fingerPrint in the clipboard
*/
