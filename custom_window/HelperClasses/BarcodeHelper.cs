using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using CoreScanner;

namespace custom_window.HelperClasses
{
    public class BarcodeHelper
    {
        private static BarcodeHelper _instance = null;
        private string _lastBarcodeString = null;
        public bool isCancelled = false;

        public delegate void BarcodeEvent(string barcodeString);

        public BarcodeEvent onBarcodeEvent;

        private BarcodeHelper()
        {
        }

        public static BarcodeHelper GetInstance()
        {
            return _instance ?? (_instance = new BarcodeHelper());
        }

        public void startListening()
        {
            var thread = new Thread(workBg) {IsBackground = true};
            thread.Start();
        }

        public void workBg()
        {

            //Instantiate CoreScanner Class
            var cCoreScannerClass = new CCoreScannerClass();
            //Call Open API
            short[] scannerTypes = new short[1]; // Scanner Types you are interested in
            scannerTypes[0] = 1; // 1 for all scanner types
            short numberOfScannerTypes = 1; // Size of the scannerTypes array
            int status; // Extended API return code
            cCoreScannerClass.Open(0, scannerTypes, numberOfScannerTypes, out status);
           // Let's beep the beeper
            int opcode = 6000; // Method for Beep the beeper
            string outXML; // Output
            string inXML = "<inArgs>" +
                           "<scannerID>1</scannerID>" + // The scanner you need to beep
                           "<cmdArgs>" +
                           "<arg-int>3</arg-int>" + // 4 high short beep pattern
                           "</cmdArgs>" +
                           "</inArgs>";
            cCoreScannerClass.ExecCommand(opcode, ref inXML, out outXML, out status);

            while (!isCancelled)
            {
                
                // Invoke((Action)(() => { string cb = Clipboard.GetText(); }));

                string cb = null;
                RunAsSTAThread(() =>
                {
                    cb = Clipboard.GetText();
                    Debug.WriteLine(cb);
                });

                if (!string.IsNullOrWhiteSpace(cb))
                {
                    if (cb.StartsWith("<]?NM") || cb.StartsWith("<]?NM"))
                    {
                        bool f = true;
                        var cnts = new string[] {"NW", "OL", "BR", "PE", "VA", "DT", "PK"};
                        foreach (var item in cnts)
                        {
                            if (!cb.Contains(item))
                            {
                                f = false;
                                break;
                            }
                        }

                        if (!f) continue;
                        // check if new data is scanned
                        if (cb != _lastBarcodeString)
                        {
                            onBarcodeEvent.Invoke(cb);
                        }
                    }
                }

                Thread.Sleep(200);
            }
        }

        public void RunAsSTAThread(Action goForIt)
        {
            AutoResetEvent @event = new AutoResetEvent(false);
            Thread thread = new Thread(
                () =>
                {
                    goForIt();
                    @event.Set();
                });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            @event.WaitOne();
        }

        public void stopListening()
        {
            isCancelled = true;
        }
    }
}