using System;
using System.Windows;
using custom_window.Core;
using custom_window.HelperClasses;

namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Home : BasePage<HomeViewModel>
    {
        private FingerprintHelper fingerPrintDevice = null;
        private string mString;
        private byte[] mStored = null;

        public Home()
        {
            InitializeComponent();
            fingerPrintDevice = new FingerprintHelper();
            fingerPrintDevice.onCaptureCallBackEvents += OnCapture;
        }

        private void InitButton(object sender, RoutedEventArgs e)
        {
            fingerPrintDevice.InitDevice();
            fingerPrintDevice.OpenDevice();
        }

        public void OnCapture(string base64, byte[] template)
        {
            if (mStored == null)
            {
                mString = base64;
                mStored = template;
            }

            var t = new ToastClass();
            // t.ShowNotification("Captured fingerprint", "your fingerprint is captured!\n"+base64, 10);

            //            Console.WriteLine("ev: "+base64);


            fp_textblock.Dispatcher.Invoke(() =>
            {
                fp_textblock.Height = 500;
                fp_textblock.Width = 400;

                fp_textblock.Selection.Text = "\ntemplate:\n" + base64;
            });


            if (mStored != null)
            {
                byte[] blob1 = Convert.FromBase64String(mString);
                byte[] blob2 = Convert.FromBase64String(base64);

                var ret = fingerPrintDevice.CompareFingerPrint(blob1, blob2);
                Console.WriteLine("Match template 1 vs template 2 score=" + ret + "!\n");

                var tt = new ToastClass();
                tt.ShowNotification("Score: ", ret.ToString(), 0);
            }
        }
    }
}