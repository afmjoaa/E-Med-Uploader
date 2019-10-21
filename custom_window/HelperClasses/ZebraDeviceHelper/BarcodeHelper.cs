using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using custom_window.HelperClasses.DataModels;
using custom_window.Pages;
using Interop.CoreScanner;

namespace custom_window.HelperClasses
{
    public class BarcodeHelper
    {
        private static BarcodeHelper _instance = null;
        private string _lastBarcodeString = null;
        private bool isCancelled = false;

        private static CCoreScanner cCoreScannerClass;


        public delegate void BarcodeEvent(string barcodeString);

        public BarcodeEvent onBarcodeEvent;

        private BarcodeHelper()
        {
            try
            {
                //Instantiate CoreScanner Class
                cCoreScannerClass = new CCoreScanner();
                //Call Open API
                short[] scannerTypes = new short[1]; //Scanner Types you are interested in
                scannerTypes[0] = 1; // 1 for all scanner types
                short numberOfScannerTypes = 1; // Size of the scannerTypes array
                int status; // Extended API return code
                cCoreScannerClass.Open(0, scannerTypes, numberOfScannerTypes, out status);

                Debug.WriteLine("Status is:" + status);

                // Subscribe for barcode events in cCoreScannerClass
                cCoreScannerClass.BarcodeEvent += new
                    _ICoreScannerEvents_BarcodeEventEventHandler(OnBarcodeEvent);
                DocCapMessage ms;
                // Let's subscribe for events
                int opcode = 1001; // Method for Subscribe events
                string outXML; // XML Output
                string inXML = "<inArgs>" +
                               "<cmdArgs>" +
                               "<arg-int>1</arg-int>" + // Number of events you want to subscribe
                               "<arg-int>1</arg-int>" + // Comma separated event IDs
                               "</cmdArgs>" +
                               "</inArgs>";
                cCoreScannerClass.ExecCommand(opcode, ref inXML, out outXML, out status);
            }
            catch (Exception exp)
            {
                Console.WriteLine("Something wrong please check... " + exp.Message);
            }
        }

        public static BarcodeHelper GetInstance()
        {
            Debug.WriteLine("instance of barcode reader");
            return _instance ?? (_instance = new BarcodeHelper());
        }

        public void startListening()
        {
        }


        public void stopListening()
        {
            isCancelled = true;
        }

        void OnBarcodeEvent(short eventType, ref string pscanData)
        {
            Debug.WriteLine("Barcode scanned: (event): " + pscanData);
            XDocument doc;
            using (var s = new StringReader(pscanData))
            {
                doc = XDocument.Load(s);
            }

            var rawHex = GetRawInnerValue(doc.ToString());
            var refinedString = HexToAsciiString(rawHex);
        }

        private string HexToAsciiString(string barcodeRawHexString)
        {
            var splitted = barcodeRawHexString.Split(' ');
            var ret = "";

            foreach (var hx in splitted)
            {
                var ui = Convert.ToUInt32(hx, 16);
                if (ui >= 32 && ui <= 126)
                {
                    var tempChar = Convert.ToChar(ui);
                    // if (char.IsLetterOrDigit(tempChar) || tempChar == ' ' || tempChar == '.') ret += tempChar;
                    ret += tempChar;
                }
            }

            return ret;
        }

        private string GetRawInnerValue(string myXML)
        {
            var doc = new XmlDocument();
            doc.LoadXml(myXML);
            XmlNodeList parentNode = doc.GetElementsByTagName("rawdata");
            return parentNode[0].InnerText;
        }

        public void GetPatientInfoFromBarcode(ref string patientName, ref string newNidNumber, ref string oldNidNumber,
            ref string dateOfbirth)
        {
            //TODO
        }

        // private Patient 
    }
}