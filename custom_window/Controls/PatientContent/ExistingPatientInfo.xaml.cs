using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses;
using custom_window.HelperClasses.ApplicationScope;
using MaterialDesignThemes.Wpf;
using Oracle.ManagedDataAccess.Client;
using Hospital = custom_window.HelperClasses.DataModels.Hospital;

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

        #region garbage

        private void voucher_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("voucherIcon");
            hosNameIcon.Foreground = Brushes.DodgerBlue;
        }

        private void voucher_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var hosNameIcon = (PackIcon) this.FindName("voucherIcon");
            hosNameIcon.Foreground = Brushes.Gray;
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

        #endregion

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
                            errorText.Foreground = new SolidColorBrush(Colors.Red);
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
                        errorText.Foreground = new SolidColorBrush(Colors.Red);
                        errorText.Visibility = Visibility.Visible;
                        errorText.Text = "Already 4 fingerprint is taken...";
                    });
            }
        }

        private void skipBtnClicked(object sender, RoutedEventArgs e)
        {
            errorText.Text = "";
            errorText.Visibility = Visibility.Hidden;
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
            errorText.Text = "";
            errorText.Visibility = Visibility.Hidden;
            //IoC.Get<PatientInfoCheckViewModel>().NullWindowData();
            IoC.Get<PatientInfoCheckViewModel>().HidePatientInfo();
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
            errorText.Text = "";
            errorText.Visibility = Visibility.Hidden;

            //validate finger print 
            if (fingerPrints.Count < 4) //four finger print data
            {
                errorText.Foreground = new SolidColorBrush(Colors.Red);
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
                        errorText.Foreground = new SolidColorBrush(Colors.Red);
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
                errorText.Foreground = new SolidColorBrush(Colors.Red);
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
                errorText.Text = "";
                errorText.Visibility = Visibility.Hidden;
                ButtonProgressAssist.SetIsIndicatorVisible(UpdateFingerprintBtn, false);
                UpdateFingerprintBtn.IsEnabled = true;
                UpdateFingerprintBtn.Visibility = Visibility.Collapsed;
                AddFingerPrintSection.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                errorText.Foreground = new SolidColorBrush(Colors.Red);
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


        private async Task SetDisplayPic(string displypic)
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
        }


        private void generateVoucherBtnClicked(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            var voucher = "v" + random.Next(1000000, 9999999).ToString();
            voucherID.Text = voucher;
        }

        public Boolean validateSimulation()
        {
            var patientInfoCheckViewModel = IoC.Get<PatientInfoCheckViewModel>();

            if (toggleBili.IsChecked != null && toggleTsh.IsChecked != null && toggleCbc.IsChecked != null)
            {
                bool toggleBiliIsChecked = (bool) toggleBili.IsChecked;
                bool toggleTshIsChecked = (bool) toggleTsh.IsChecked;
                bool toggleCbcIsChecked = (bool) toggleCbc.IsChecked;

                if (string.IsNullOrEmpty(patientName.Text) ||
                    string.IsNullOrEmpty(voucherID.Text) ||
                    (!toggleBiliIsChecked && !toggleTshIsChecked && !toggleCbcIsChecked))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private async void SimulateReportRequest_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(simulateReportRequest, true);

            if (validateSimulation())
            {
                errorText.Visibility = Visibility.Hidden;
                errorText.Text = "";
                //TODO sumon vai database ae voucher vhorbe
                var pat = IoC.Get<PatientInfoCheckViewModel>();


                CancellationToken cancellationToken = new CancellationToken(false);
                Hospital hospital = await ApplicationData.Instance.GetUserHospital(cancellationToken);
                var hospitalName = hospital.hospitalName;
                var hospitalId = hospital.registrationNumber;

                var birthYear = pat.birth.Substring(0, 4);
                var today = DateTime.Today;
                var age = 24;
                var testType = "sb";
                try
                {
                    age = today.Year - Int32.Parse(birthYear);
                }
                catch (Exception err)
                {
                    age = 24;
                }
                // Go back to the year the person was born in case of a leap year
                //if (birthdate.Date > today.AddYears(-age)) age--;

                if (toggleBili.IsChecked != null && toggleTsh.IsChecked != null && toggleCbc.IsChecked != null)
                {
                    bool toggleBiliIsChecked = (bool) toggleBili.IsChecked;
                    bool toggleTshIsChecked = (bool) toggleTsh.IsChecked;
                    bool toggleCbcIsChecked = (bool) toggleCbc.IsChecked;

                    if (toggleBiliIsChecked && toggleTshIsChecked && toggleCbcIsChecked)
                        testType = "sb,tsh,cbc";
                    else if (toggleBiliIsChecked && toggleTshIsChecked)
                        testType = "sb,tsh";
                    else if (toggleTshIsChecked && toggleCbcIsChecked)
                        testType = "tsh,cbc";
                    else if (toggleBiliIsChecked && toggleCbcIsChecked)
                        testType = "sb,cbc";
                    else if (toggleBiliIsChecked)
                        testType = "sb";
                    else if (toggleTshIsChecked)
                        testType = "tsh";
                    else if (toggleCbcIsChecked)
                        testType = "cbc";
                    else
                        testType = "";
                }

                /*'{testType}'*/
                /*specimen null*/
                string sql =
                    $"INSERT INTO PATIENT_INFO VALUES('{voucherID.Text}','{hospitalName}','{pat.name}','{pat.phone}','Male',{age.ToString()},'Self','Blood',null, null,'bm123', null,'{pat.email}')";


                OracleConnection con = new OracleConnection();
                // create connection string using builder
                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();

                ocsb.UserID = "appdev";
                ocsb.Password = "pass";
                ocsb.DataSource = "localhost:1521/orcl";

                // connect
                con.ConnectionString = ocsb.ConnectionString;
                con.Open();
                Console.WriteLine("Connection established (" + con.ServerVersion + ")");
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                var executeNonQuery = cmd.ExecuteNonQuery();

                Console.WriteLine("Connection executed (" + executeNonQuery + ")");

                // Close and Dispose OracleConnection
                con.Close();
                con.Dispose();

                errorText.Foreground = new SolidColorBrush(Colors.Green);
                errorText.Visibility = Visibility.Visible;
                errorText.Text = voucherID.Text + " is added to Local Database";
            }
            else
            {
                errorText.Foreground = new SolidColorBrush(Colors.Red);
                errorText.Visibility = Visibility.Visible;
                errorText.Text = "Either Patient, voucher or report type isn't selected...";
            }

            ButtonProgressAssist.SetIsIndicatorVisible(simulateReportRequest, false);
        }

        private void SimulateReportCreation_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(simulateReportCreation, true);

            string reportSQL = $"insert into tsh values('r123456','{voucherID.Text}','3.00,2.00,4.00')";

            OracleConnection con = new OracleConnection();
            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.UserID = "appdev";
            ocsb.Password = "pass";
            ocsb.DataSource = "localhost:1521/orcl";
            con.ConnectionString = ocsb.ConnectionString;
            con.Open();
            OracleCommand cmd = con.CreateCommand();
           

            if (toggleBili.IsChecked != null && toggleTsh.IsChecked != null && toggleCbc.IsChecked != null)
            {
                bool toggleBiliIsChecked = (bool)toggleBili.IsChecked;
                bool toggleTshIsChecked = (bool)toggleTsh.IsChecked;
                bool toggleCbcIsChecked = (bool)toggleCbc.IsChecked;

                if (toggleBiliIsChecked && toggleTshIsChecked && toggleCbcIsChecked)
                {
                    //tsh
                    reportSQL = $"INSERT INTO tsh VALUES('r123456','{voucherID.Text}','3.00,2.00,1.00')"; 
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    //bilirubin
                    reportSQL = $"INSERT INTO bilirubin VALUES('r123456','{voucherID.Text}','1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    //cbc
                    reportSQL = $"INSERT INTO cbc VALUES('r123456','{voucherID.Text}','10.00,9.00,8.00,7.00,6.00,5.00,4.00,3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();


                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Bilirubin, TSH & CBC report is added to Local Database";
                }
                else if (toggleBiliIsChecked && toggleTshIsChecked)
                {
                    //tsh
                    reportSQL = $"INSERT INTO tsh VALUES('r123456','{voucherID.Text}','3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    //bilirubin
                    reportSQL = $"INSERT INTO bilirubin VALUES('r123456','{voucherID.Text}','1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();


                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Bilirubin & TSH report is added to Local Database";
                }
                else if (toggleTshIsChecked && toggleCbcIsChecked)
                {
                    //tsh
                    reportSQL = $"INSERT INTO tsh VALUES('r123456','{voucherID.Text}','3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();


                    //cbc
                    reportSQL = $"INSERT INTO cbc VALUES('r123456','{voucherID.Text}','10.00,9.00,8.00,7.00,6.00,5.00,4.00,3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Bilirubin & CBC report is added to Local Database";
                }
                else if (toggleBiliIsChecked && toggleCbcIsChecked)
                {
                    //bilirubin
                    reportSQL = $"INSERT INTO bilirubin VALUES('r123456','{voucherID.Text}','1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    //cbc
                    reportSQL = $"INSERT INTO cbc VALUES('r123456','{voucherID.Text}','10.00,9.00,8.00,7.00,6.00,5.00,4.00,3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "TSH & CBC report is added to Local Database";
                }
                else if (toggleBiliIsChecked)
                {
                    //bilirubin
                    reportSQL = $"INSERT INTO bilirubin VALUES('r123456','{voucherID.Text}','1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "Bilirubin report is added to Local Database";
                }
                else if (toggleTshIsChecked)
                {
                    //tsh
                    reportSQL = $"INSERT INTO tsh VALUES('r123456','{voucherID.Text}','3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "TSH report is added to Local Database";
                }
                else if (toggleCbcIsChecked)
                {
                    //cbc
                    reportSQL = $"INSERT INTO cbc VALUES('r123456','{voucherID.Text}','10.00,9.00,8.00,7.00,6.00,5.00,4.00,3.00,2.00,1.00')";
                    cmd.CommandText = reportSQL;
                    cmd.ExecuteNonQuery();

                    errorText.Foreground = new SolidColorBrush(Colors.Green);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "CBC report is added to Local Database";
                }
                else
                {
                    errorText.Foreground = new SolidColorBrush(Colors.Red);
                    errorText.Visibility = Visibility.Visible;
                    errorText.Text = "No report is selected";
                }
            }

            //cmd.CommandText = reportSQL;
            //cmd.ExecuteNonQuery();
            //Console.WriteLine("Connection executed (" + executeNonQuery + ")");

            con.Close();
            con.Dispose();
            ButtonProgressAssist.SetIsIndicatorVisible(simulateReportCreation, false);
        }
    }
}