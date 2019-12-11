using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Console = System.Console;

namespace custom_window
{
    /// <summary>
    /// Interaction logic for NoAcc.xaml
    /// </summary>
    public partial class NewPatientRegistration : UserControl
    {
        private BarcodeHelper _bar;
        private FingerprintHelper _fp;
        private OpenFileDialog op;
        public List<string> fingerPrints { get; set; }

        public NewPatientRegistration()
        {
            InitializeComponent();
            _bar = BarcodeHelper.GetInstance();
            _fp = FingerprintHelper.GetInstance();

            _bar.modifiedModifiedBarcodeEvent += OnModifiedModifiedBarcodeEvent;
            _fp.onCaptureCallBackEvents+= OnCaptureCallBackEvents;
             fingerPrints = new List<string>();
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
            else
            {
                var isMatchedAny = false;
                foreach (var print in fingerPrints.ToList())
                {
                    var isFingerPrintMatch = FingerprintHelper.GetInstance().isFingerPrintMatch(print,templateString);
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
        }

        private void OnModifiedModifiedBarcodeEvent(string patientname, string oldnid, string newnid,
            string dateofbirth)
        {
            vName.Dispatcher.Invoke(new System.Action(()=> {
                vName.Text = patientname;
            }));
            // get data for registration   
            ToastClass.NotifyMin("New data for fingerprint", "getting ready");
        }

        private async  void Register_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, true);
            errorText.Visibility = Visibility.Hidden;
            //validate data
            if (!validateData())
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = "No field can be empty...";
                ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                return;
            }else if (validateData() && fingerPrints.Count < 4)//four finger print data
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = "Please add all four fingerPrints...";
                ButtonProgressAssist.SetIsIndicatorVisible(RegisterAndSelectBtn, false);
                return;
            }

            // check if user data doesn't conflict

            //sign in anonomously

            //save to database

        }

        private void Skip_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = false;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { 
            
             op = new OpenFileDialog();
             op.Title = "Select a picture";
             op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png";

             if (op.ShowDialog() == true)
             {
                 patientImage.Source = new BitmapImage(new Uri(op.FileName));
             }

             //upload file by this
             //FileUploadService.GetInstance().UploadPatientDisplayPic(op.OpenFile(), patientId:);
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
    }
}