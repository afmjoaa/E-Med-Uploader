using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using custom_window.Core;
using custom_window.HelperClasses;
namespace custom_window.Pages
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Home : BasePage<HomeViewModel>
    {

        private FingerprintHelper fingerPrintDevice = null ;

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

        public void OnCapture(string base64)
        {

            var t = new ToastClass();
            t.ShowNotification("Captured fingerprint", "your fingerprint is captured!\n"+base64, 10);

            Console.WriteLine("ev: "+base64);

           
            fp_textblock.Dispatcher.Invoke(() => {
                fp_textblock.Height = 500;
                fp_textblock.Width = 400;
                
                fp_textblock.Selection.Text = "\ntemplate:\n"+ base64;
            });


        }

      

    }
}