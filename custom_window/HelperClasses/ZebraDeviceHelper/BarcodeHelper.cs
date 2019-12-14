using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using custom_window.HelperClasses.DataModels;
using custom_window.Pages;
using Interop.CoreScanner; // this is from 32bit installation location

namespace custom_window.HelperClasses
{
    public class BarcodeHelper
    {
        private static BarcodeHelper _instance = null;
        private string _lastBarcodeString = null;
        private bool isCancelled = false;

        private static CCoreScanner cCoreScannerClass;


        public delegate void ModifiedBarcodeEvent(string patientName, string oldNid,
            string newNid, string dateOfBirth, string present, string permanent,
            string votingArea, string issueDate, string pk, string signature
        );

        public delegate void MobileModifiedBarcodeEvent(string patientUid);

        public ModifiedBarcodeEvent modifiedModifiedBarcodeEvent;

        public MobileModifiedBarcodeEvent mobileModifiedBarcodeEvent;

        private BarcodeHelper()
        {
        }

        public static BarcodeHelper GetInstance()
        {
            return _instance ?? (_instance = new BarcodeHelper());
        }

        public void InitDevice()
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
                // Subscribe for barcode events in cCoreScannerClass
                cCoreScannerClass.BarcodeEvent += OnBarcodeEvent;
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

        public void CloseDevice()
        {
            int st;
            cCoreScannerClass.Close(0, out st);
            //handle status code.. or errors..
        }

        void OnBarcodeEvent(short eventType, ref string pscanData)
        {
            //  Debug.WriteLine("Barcode scanned: (event): " + pscanData);
            XDocument doc;
            using (var s = new StringReader(pscanData))
            {
                doc = XDocument.Load(s);
            }

            var rawHex = GetRawInnerValue(doc.ToString());
            var refinedString = HexToAsciiString(rawHex);

            if (refinedString.Length <= 28)
            {
                mobileModifiedBarcodeEvent.Invoke(refinedString);
            }
            else
            {
                var nidInfo = extractNidInfo(refinedString);
                modifiedModifiedBarcodeEvent.Invoke(nidInfo["name"],
                    nidInfo["oldNid"], nidInfo["newNid"], nidInfo["birthDate"], nidInfo["present"],
                    nidInfo["permanent"], nidInfo["votingArea"], nidInfo["issueDate"], nidInfo["pk"],
                    nidInfo["signature"]);
            }
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

        public void ExtractInfo(string mainStr, ref string patientName, ref string oldNid, ref string newNid,
            ref string dateOfBirth)
        {
            int ix = 0;
            for (; ix < mainStr.Length; ++ix)
            {
                if (mainStr[ix] == 'N' && mainStr[ix + 1] == 'M') break;
            }

            ix += 2;
            while (ix < mainStr.Length - 1)
            {
                if (mainStr[ix] == 'N' && mainStr[ix + 1] == 'W') break;
                patientName += mainStr[ix++];
            }

            ix += 2;
            while (ix < mainStr.Length - 1)
            {
                if (mainStr[ix] == 'O' && mainStr[ix + 1] == 'L') break;
                newNid += mainStr[ix++];
            }

            ix += 2;
            while (ix < mainStr.Length - 1)
            {
                if (mainStr[ix] == 'B' && mainStr[ix + 1] == 'R') break;
                if (char.IsDigit(mainStr[ix]))
                {
                    oldNid += mainStr[ix++];
                }
                else
                {
                    ix++;
                }
            }

            ix += 2;
            while (ix < mainStr.Length - 1)
            {
                if (mainStr[ix] == 'P' && mainStr[ix + 1] == 'E') break;
                dateOfBirth += mainStr[ix++];
            }
        }

        public Dictionary<string, string> extractNidInfo(string infoText)
        {
            var map = new Dictionary<string, string>();


            infoText = infoText.Substring(4, getLenthFromEndindex(4, infoText.Length - 1));
            int tempStart = infoText.IndexOf("NW");
            int tempStop = infoText.IndexOf("OL");


            map.Add("name", infoText.Substring(2, getLenthFromEndindex(2, tempStart)));
            map.Add("newNid", infoText.Substring(tempStart + 2,  getLenthFromEndindex(tempStart + 2, tempStop)));

            tempStart = infoText.IndexOf("OL");
            tempStop = infoText.IndexOf("BR");
            map.Add("oldNid", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStop)));

            tempStart = infoText.IndexOf("BR");
            tempStop = infoText.IndexOf("PE");
            map.Add("birthDate", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStart + 6)) + "/" +
                                 infoText.Substring(tempStart + 6, getLenthFromEndindex(tempStart + 6, tempStart + 8)) + "/" +
                                 infoText.Substring(tempStart + 8, getLenthFromEndindex(tempStart + 8, tempStop)));


            tempStart = infoText.IndexOf("PE");
            tempStop = infoText.IndexOf("PR");
            map.Add("present", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStop)));

            tempStart = infoText.IndexOf("PR");
            tempStop = infoText.IndexOf("VA");
            map.Add("permanent", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStop)));

            tempStart = infoText.IndexOf("VA");
            tempStop = infoText.IndexOf("DT");
            map.Add("votingArea", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStop)));

            tempStart = infoText.IndexOf("DT");
            tempStop = infoText.IndexOf("PK");
            map.Add("issueDate", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStart + 6)) + "/" +
                                 infoText.Substring(tempStart + 6, getLenthFromEndindex(tempStart + 6, tempStart + 8)) + "/" +
                                 infoText.Substring(tempStart + 8,getLenthFromEndindex(tempStart + 8, tempStop)));

            tempStart = infoText.IndexOf("PK");
            tempStop = infoText.IndexOf("SG");
            map.Add("pk", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStop)));

            tempStart = infoText.IndexOf("SG");
            tempStop = infoText.Length - 1;
            map.Add("signature", infoText.Substring(tempStart + 2, getLenthFromEndindex(tempStart + 2, tempStop)));
            return map;
        }

        public void waitForBarcode()
        {
        }

        public int getLenthFromEndindex(int startIndex, int endIndex)
        {
            return (endIndex - startIndex);
        }

        // private Patient 
    }
}