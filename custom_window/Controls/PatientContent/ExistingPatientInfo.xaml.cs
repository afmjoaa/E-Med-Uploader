using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using custom_window.Core;
using custom_window.HelperClasses;
using MaterialDesignThemes.Wpf;

namespace custom_window.Controls.PatientContent
{
    /// <summary>
    /// Interaction logic for PhoneSearch.xaml
    /// </summary>
    public partial class ExistingPatientInfo : UserControl
    {
        public List<string> fingerPrints { get; set; }
        private FingerprintHelper _fp;

        public ExistingPatientInfo()
        {
            InitializeComponent();
            _fp = FingerprintHelper.GetInstance();

            _fp.onCaptureCallBackEventsThree += OnCaptureCallBackEvents;
            fingerPrints = new List<string>();
        }

        private void OnCaptureCallBackEvents(string templateString, byte[] template)
        {
            if (AddFingerPrintSection.Visibility != Visibility.Visible) return;
            

            Console.WriteLine("existing window Joaa");
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

        private void skipBtnClicked(object sender, RoutedEventArgs e)
        {
            errorText.Visibility = Visibility.Collapsed;
            IoC.Get<PatientInfoCheckViewModel>().selectPatient(null, null);

            if (IoC.Get<PatientInfoCheckViewModel>().addFinger)
            {
                AddFingerprintBtn.Visibility = Visibility.Visible;
            }
            else
            {
                AddFingerprintBtn.Visibility = Visibility.Collapsed;
            }

            IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
            NullifyAllFinger();

            AddFingerPrintSection.Visibility = Visibility.Collapsed;
            UpdateFingerprintBtn.Visibility = Visibility.Collapsed;

            IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
        }

        private void acknowledgeBtnClicked(object sender, RoutedEventArgs e)
        {
            errorText.Visibility = Visibility.Collapsed;
            IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
            IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
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

        private async void UpdateFingerPrintBtnClicked(object sender, RoutedEventArgs e)
        {
            UpdateFingerprintBtn.IsEnabled = false;
            ButtonProgressAssist.SetIsIndicatorVisible(UpdateFingerprintBtn, true);
            errorText.Visibility = Visibility.Collapsed;

            //validate finger print 
            if (fingerPrints.Count < 4) //four finger print data
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = "Please add all four fingerPrints...";
                ButtonProgressAssist.SetIsIndicatorVisible(UpdateFingerprintBtn, false);
                UpdateFingerprintBtn.IsEnabled = true;
                return;
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
                        ButtonProgressAssist.SetIsIndicatorVisible(UpdateFingerprintBtn, false);
                        UpdateFingerprintBtn.IsEnabled = true;
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


            //update to firebase patient 
            try
            {
                await CloudFirestoreService.GetInstance()
                    .UpdatePatient(IoC.Get<PatientInfoCheckViewModel>().selectedPatientId, "fingerprint_templates",
                        fingerPrints);

                //show toast 
                ToastClass.NotifyMin("Updated",
                    "Dear " + IoC.Get<PatientInfoCheckViewModel>().name + "your fingerprint have been updated..");

                //hide update section
                errorText.Visibility = Visibility.Collapsed;
                ButtonProgressAssist.SetIsIndicatorVisible(UpdateFingerprintBtn, false);
                UpdateFingerprintBtn.IsEnabled = true;
                UpdateFingerprintBtn.Visibility = Visibility.Collapsed;
                AddFingerPrintSection.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                errorText.Visibility = Visibility.Visible;
                errorText.Text = exception.Message;
                ButtonProgressAssist.SetIsIndicatorVisible(UpdateFingerprintBtn, false);
                UpdateFingerprintBtn.IsEnabled = true;
                Console.WriteLine(exception);
            }
        }

        private void AddFingerPrintBtnClicked(object sender, RoutedEventArgs e)
        {
            AddFingerprintBtn.Visibility = Visibility.Collapsed;
            UpdateFingerprintBtn.Visibility = Visibility.Visible;
            AddFingerPrintSection.Visibility = Visibility.Visible;
        }
    }
}