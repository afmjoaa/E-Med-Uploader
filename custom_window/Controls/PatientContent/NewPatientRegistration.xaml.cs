using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.MailAuthService;
using custom_window.HelperClasses.ZebraDeviceHelper;
using Firebase.Auth;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Console = System.Console;
using Exception = System.Exception;
using Patient = custom_window.HelperClasses.DataModels.Patient;

namespace custom_window.Controls.PatientContent
{
    /// <summary>
    /// Interaction logic for NoAcc.xaml
    /// </summary>
    public partial class NewPatientRegistration : UserControl
    {
        private BarcodeHelper _bar;
        private FingerprintHelper _fp;
        private OpenFileDialog op;
        private FirebaseAuthLink authLink = null;
        public List<string> fingerPrints { get; set; }

        public string uploadPatientDisplayPicUrl { get; set; } = null;
        public string pk { get; set; } 
        public string signature { get; set; } 

        public NewPatientRegistration()
        {
            InitializeComponent();
            _bar = BarcodeHelper.GetInstance();
            _fp = FingerprintHelper.GetInstance();

            //delegate for display pic and fingerprints

            _bar.modifiedModifiedBarcodeEvent += OnModifiedModifiedBarcodeEvent;
            _fp.onCaptureCallBackEventsTwo += OnCaptureCallBackEvents;
            fingerPrints = new List<string>();
        }





        private async Task SetFingerAndDisplayPic(string displypic, List<string> fingerlist)
        {
            //set the display pic 
            if (string.IsNullOrEmpty(displypic))
            {
                patientImage.Source =
                    new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/patientDemo.png"));
            }
            else
            {
                var imgUrl = new Uri(displypic);
                // or you can download it Async won't block your UI
                var imageData = await new WebClient().DownloadDataTaskAsync(imgUrl);

                var bitmapImage = new BitmapImage {CacheOption = BitmapCacheOption.OnLoad};
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.EndInit();
                patientImage.Source = bitmapImage;
            }


            //set the finger print 
            fingerPrints = new List<string>();
            lt.IsChecked = false;
            li.IsChecked = false;
            rt.IsChecked = false;
            ri.IsChecked = false;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateOne = true;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateTwo = true;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateThree = true;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateFour = true;


            if (fingerPrints.Count == 0)
            {
                fingerPrints.Add(fingerlist[0]);
                lt.IsChecked = true;
                IoC.Get<PatientInfoCheckViewModel>().visibleStateOne = false;
            }
        }

        private void NullifyAllFinger()
        {
            fingerPrints = new List<string>();
            lt.IsChecked = false;
            li.IsChecked = false;
            rt.IsChecked = false;
            ri.IsChecked = false;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateOne = true;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateTwo = true;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateThree = true;
            IoC.Get<PatientInfoCheckViewModel>().visibleStateFour = true;
        }

        private void OnCaptureCallBackEvents(string templateString, byte[] template)
        {
            Console.WriteLine("fingerPrint joaa");
            if (fingerPrints.Count == 0)
            {
                fingerPrints.Add(templateString);
                if (Dispatcher != null)
                    Dispatcher.Invoke(() =>
                    {
                        lt.IsChecked = true;
                        IoC.Get<PatientInfoCheckViewModel>().visibleStateOne = false;
                    });
            }
            else if (fingerPrints.Count < 4)
            {
                var isMatchedAny = false;
                foreach (var print in fingerPrints.ToList())
                {
                    var isFingerPrintMatch = FingerprintHelper.GetInstance().isFingerPrintMatch(print, templateString);
                    if (isFingerPrintMatch)
                    {
                        isMatchedAny = true;
                    }
                }

                if (!isMatchedAny)
                {
                    fingerPrints.Add(templateString);
                    if (fingerPrints.Count == 2)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            li.IsChecked = true;
                            IoC.Get<PatientInfoCheckViewModel>().visibleStateTwo = false;
                        });
                    }
                    else if (fingerPrints.Count == 3)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            rt.IsChecked = true;
                            IoC.Get<PatientInfoCheckViewModel>().visibleStateThree = false;
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ri.IsChecked = true;
                            IoC.Get<PatientInfoCheckViewModel>().visibleStateFour = false;
                        });
                    }
                }
                else
                {
                    if (Dispatcher != null)
                        Dispatcher.Invoke(() =>
                        {
                            errorText.Visibility = Visibility.Visible;
                            errorText.Text = "Same fingerprint given twice";
                        });
                    Console.WriteLine("Same fingerprint given twice");
                }
            }
            else
            {
                if (Dispatcher != null)
                    Dispatcher.Invoke(() =>
                    {
                        errorText.Visibility = Visibility.Visible;
                        errorText.Text = "Already 4 fingerprint is taken...";
                    });
            }
        }


        private void OnModifiedModifiedBarcodeEvent(string patientname, string oldnid, string newnid,
            string dateofbirth, string present, string permanent, string votingArea, string issueDate, string myPk,
            string mySignature)
        {
            //update the view here 
            if (Dispatcher != null)
                Dispatcher.Invoke(() =>
                {
                    vName.Text = patientname;
                    vOldNid.Text = oldnid;
                    vNewNid.Text = newnid;
                    vBirth.Text = dateofbirth;
                    vParmanent.Text = permanent;
                    vPresent.Text = present;
                    vVotingArea.Text = votingArea;
                    vIssueDate.Text = issueDate;
                    this.pk = myPk;
                    this.signature = mySignature;
                });
        }

        private async void Register_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, true);
            errorText.Visibility = Visibility.Hidden;
            RegisterAndSelectBtn.IsEnabled = false;
            //validate data
            if (!validateData())
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = "No field can be empty...";
                ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                RegisterAndSelectBtn.IsEnabled = true;
                return;
            }
            else if (validateData() && fingerPrints.Count < 4) //four finger print data
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = "Please add all four fingerPrints...";
                ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                RegisterAndSelectBtn.IsEnabled = true;
                return;
            }

            // check if user data doesn't conflict
            var vPhoneText = vPhone.Text;
            var vEmailText = vEmail.Text;
            var vNidText = vNewNid.Text;

            var cancellationToken = new CancellationToken();
            var cancellationTokenOne = new CancellationToken();
            var cancellationTokenTwo = new CancellationToken();

            try
            {
                var patientOne = await CloudFirestoreService.GetInstance()
                                .FindPatientBy("phone", vPhoneText, cancellationToken);
                if (patientOne != null)
                {
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Already " + patientOne.name + " is associated with the phone number";
                    ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                    RegisterAndSelectBtn.IsEnabled = true;
                    return;
                }
            }
            catch (Exception exception)
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = exception.Message;
                Console.WriteLine(exception);
            }

            try
            {
                var patientTwo = await CloudFirestoreService.GetInstance()
                    .FindPatientBy("email", vEmailText, cancellationTokenOne);
                if (patientTwo != null)
                {
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Already " + patientTwo.name + " is associated with the email address";
                    ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                    RegisterAndSelectBtn.IsEnabled = true;
                    return;
                }

               
            }
            catch (Exception exception)
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = exception.Message;
                Console.WriteLine(exception);
            }

            try
            {
                var patientThree = await CloudFirestoreService.GetInstance()
                    .FindPatientBy("new_nid", vNidText, cancellationTokenTwo);
                if (patientThree != null)
                {
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Already "+ patientThree.name +" is associated with the Nid number";
                    ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                    RegisterAndSelectBtn.IsEnabled = true;
                    return;
                }
            }
            catch (Exception exception)
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = exception.Message;
                Console.WriteLine(exception);
            }

            try
            {
                foreach (var print in fingerPrints.ToList())
                {
                    var PatientFour = await CloudFirestoreService.GetInstance().FindPatientByFingerprint(print);
                    if (PatientFour != null)
                    {
                        errorText.Visibility = Visibility.Visible;
                        errorText.Text = "One of the fingerprint is associated with a patient...";
                        ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                        RegisterAndSelectBtn.IsEnabled = true;
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = exception.Message;
                Console.WriteLine(exception);
            }

            //sign in email pass
            try
            {
                if (authLink != null)
                {
                    authLink = await CloudFirestoreService.GetInstance().authProvider
                        .SignInWithEmailAndPasswordAsync(vEmail.Text,
                            "123456");

                }
                else if (authLink == null && !string.IsNullOrEmpty(vEmail.Text))
                {
                    authLink = await CloudFirestoreService.GetInstance().authProvider
                        .CreateUserWithEmailAndPasswordAsync(vEmail.Text,
                            "123456");
                }

                if (authLink == null)
                {
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Fetal Auth-link is null..";
                    return;
                }

                //save to database
                Patient uploadingPatient = new Patient();

                uploadingPatient.id = authLink.User.LocalId;
                uploadingPatient.FederatedId = authLink.User.FederatedId;
                uploadingPatient.firebaseAuthToken = authLink.FirebaseToken;
                uploadingPatient.firebaseRefreshToken = authLink.RefreshToken;
                uploadingPatient.name = vName.Text;
                uploadingPatient.phone = vPhone.Text;
                uploadingPatient.email = vEmail.Text;
                uploadingPatient.birth = vBirth.Text;
                uploadingPatient.permanent_address = vParmanent.Text;
                uploadingPatient.present_address = vPresent.Text;
                uploadingPatient.voting_area = vVotingArea.Text;
                uploadingPatient.issue_date = vIssueDate.Text;
                uploadingPatient.display_pic = vIssueDate.Text;
                uploadingPatient.pk = this.pk;
                uploadingPatient.signature = this.signature;
                uploadingPatient.display_pic = uploadPatientDisplayPicUrl;
                uploadingPatient.fingerprint_templates = fingerPrints;
                uploadingPatient.old_nid = vOldNid.Text;
                uploadingPatient.new_nid = vNewNid.Text;

                await CloudFirestoreService.GetInstance().AddPatient(uploadingPatient);

                RegistrationCodeSender.GetInstance().SendCodeToEmail(vEmail.Text);
                ToastClass.NotifyMin("Patient registered", "Patient is registered and selected for file upload...");

                ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                RegisterAndSelectBtn.IsEnabled = true;

                IoC.Get<PatientInfoCheckViewModel>().selectPatient(authLink.User.LocalId, vName.Text);

                IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
                NullifyAllFinger();
                authLink = null;
                errorText.Text = "";
                errorText.Visibility = Visibility.Collapsed;
                patientImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/patientDemo.png"));

               
                IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
            }
            catch (FirebaseAuthException exception)
            {
                string reason = exception.Reason.ToString();
                reason = string.Concat(reason.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                errorText.Visibility = Visibility.Visible;
                errorText.Text = reason;
                ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                RegisterAndSelectBtn.IsEnabled = true;
            }

            //wait for patient...
        }

        private void Skip_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().selectPatient(null, null);

            IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
            NullifyAllFinger();
            authLink = null;
            errorText.Text = "";
            errorText.Visibility = Visibility.Collapsed;
            patientImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/patientDemo.png"));
            IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
        }

        private async void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {

                patientImage.Source = new BitmapImage(new Uri(op.FileName));
                if (authLink != null)
                {
                    uploadPatientDisplayPicUrl = await FileUploadService.GetInstance()
                        .UploadPatientDisplayPic(op.OpenFile(), authLink.User.LocalId);

                }
                else if (authLink == null && !string.IsNullOrEmpty(vEmail.Text))
                {

                    try
                    {
                        authLink = await CloudFirestoreService.GetInstance().authProvider
                            .CreateUserWithEmailAndPasswordAsync(vEmail.Text,
                                "123456");

                        uploadPatientDisplayPicUrl = await FileUploadService.GetInstance()
                            .UploadPatientDisplayPic(op.OpenFile(), authLink.User.LocalId);
                    }
                    catch (FirebaseAuthException exception)
                    {
                        string reason = exception.Reason.ToString();
                        reason = string.Concat(reason.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                        errorText.Visibility = Visibility.Visible;
                        errorText.Text = reason;
                        Console.WriteLine(exception);
                    }
                   
                }
               
            }

            //upload file by this
        }

        private bool validateData()
        {
            if (string.IsNullOrEmpty(vName.Text) ||
                string.IsNullOrEmpty(vBirth.Text) ||
                string.IsNullOrEmpty(vEmail.Text) ||
                string.IsNullOrEmpty(vIssueDate.Text) ||
                string.IsNullOrEmpty(vNewNid.Text) ||
                string.IsNullOrEmpty(vOldNid.Text) ||
                string.IsNullOrEmpty(vParmanent.Text) ||
                string.IsNullOrEmpty(vPresent.Text) ||
                string.IsNullOrEmpty(vPhone.Text) ||
                string.IsNullOrEmpty(vVotingArea.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void Lt_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fingerPrints.Count >= 1)
                {
                    fingerPrints.RemoveAt(0);
                }

                IoC.Get<PatientInfoCheckViewModel>().visibleStateOne = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Li_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fingerPrints.Count >= 2)
                {
                    fingerPrints.RemoveAt(1);
                }

                IoC.Get<PatientInfoCheckViewModel>().visibleStateTwo = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Rt_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fingerPrints.Count >= 3)
                {
                    fingerPrints.RemoveAt(2);
                }

                IoC.Get<PatientInfoCheckViewModel>().visibleStateThree = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Ri_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fingerPrints.Count >= 4)
                {
                    fingerPrints.RemoveAt(3);
                }

                IoC.Get<PatientInfoCheckViewModel>().visibleStateFour = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void DiscardFingerPrintClicked(object sender, RoutedEventArgs e)
        {
            NullifyAllFinger();
            fingerPrints = new List<string>();
        }
    }
}

