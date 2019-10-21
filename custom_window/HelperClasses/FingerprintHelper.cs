using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using libzkfpcsharp; // ZK Fingerprint Device library

namespace custom_window.HelperClasses
{
    class FingerprintHelper : zkfp2
    {
        private static FingerprintHelper instance = null;

        private FingerprintHelper()
        {
        }

        public static FingerprintHelper GetInstance()
        {
            if (instance != null) return instance;
            instance = new FingerprintHelper();
            return instance;
        }

        public delegate void OnCaptureCallBack(string strTemplate, byte[] template);

        public OnCaptureCallBack onCaptureCallBackEvents;

        IntPtr mDevHandle = IntPtr.Zero;
        IntPtr mDBHandle = IntPtr.Zero;
        IntPtr FormHandle = IntPtr.Zero;
        bool bIsTimeToDie = false;
        bool IsRegister = false;
        bool bIdentify = true;
        byte[] FPBuffer;
        int RegisterCount = 0;
        const int REGISTER_FINGER_COUNT = 3;

        byte[][] RegTmps = new byte[3][];
        byte[] RegTmp = new byte[2048];
        byte[] CapTmp = new byte[2048];

        int cbCapTmp = 2048;
        int cbRegTmp = 0;
        int iFid = 1;

        private int mfpWidth = 0;
        private int mfpHeight = 0;
        private int mfpDpi = 0;

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        public void InitDevice()
        {
            int ret = zkfperrdef.ZKFP_ERR_OK;
            if ((ret = zkfp2.Init()) == zkfperrdef.ZKFP_ERR_OK)
            {
                int nCount = zkfp2.GetDeviceCount();
                if (nCount > 0)
                {
                    Console.WriteLine("conntected!");
                    ///OpenDevice();
                }
                else
                {
                    zkfp2.Terminate();
                    Console.WriteLine("No device connected!");
                }
            }
            else
            {
                Console.WriteLine("No Device Connected");
            }
        }

        public void OpenDevice()
        {
            int ret = zkfp.ZKFP_ERR_OK;
            if (IntPtr.Zero == (mDevHandle = zkfp2.OpenDevice(0)))
            {
                Console.WriteLine("OpenDevice fail");
                return;
            }

            if (IntPtr.Zero == (mDBHandle = zkfp2.DBInit()))
            {
                Console.WriteLine("Init DB fail");
                zkfp2.CloseDevice(mDevHandle);
                mDevHandle = IntPtr.Zero;
                return;
            }

            RegisterCount = 0;
            cbRegTmp = 0;
            iFid = 1;
            for (int i = 0; i < 3; i++)
            {
                RegTmps[i] = new byte[2048];
            }

            byte[] paramValue = new byte[4];
            int size = 4;
            zkfp2.GetParameters(mDevHandle, 1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            zkfp2.GetParameters(mDevHandle, 2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            size = 4;
            zkfp2.GetParameters(mDevHandle, 3, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpDpi);

            Console.WriteLine("reader parameter, image width:" + mfpWidth + ", height:" + mfpHeight + ", dpi:" +
                              mfpDpi + "\n");

            Thread captureThread = new Thread(DoCapture) {IsBackground = true};
            captureThread.Start();
            bIsTimeToDie = false;
            Console.WriteLine("Open success\n");
        }

        private void DoCapture()
        {
            while (!bIsTimeToDie)
            {
                cbCapTmp = 2048;
                int ret = zkfp2.AcquireFingerprint(mDevHandle, FPBuffer, CapTmp, ref cbCapTmp);
                if (ret == zkfp.ZKFP_ERR_OK)
                {
//                    SendMessage(FormHandle, MESSAGE_CAPTURED_OK, IntPtr.Zero, IntPtr.Zero);
                    Console.WriteLine("Fingerprint captured!!");
                    // var get = ImageFromRawBgraArray(FPBuffer, mfpWidth, mfpHeight);

/*        
                    var r = new Random();
                    var fileName = "file" + r.Next() + ".bmp";
                    get.Save(fileName);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    string path = Directory.GetCurrentDirectory();
                    bitmap.UriSource = new Uri(path + "\\" + fileName);

                    bitmap.EndInit();
                    bitmap.Freeze();
*/
                    var templateInBase64 = zkfp2.BlobToBase64(CapTmp, cbCapTmp);

                    onCaptureCallBackEvents.Invoke(templateInBase64, CapTmp);

                    //      Console.WriteLine(templateInBase64);
//                    Dispatcher?.Invoke(() => { img_view.Source = bitmap; }, DispatcherPriority.Normal);
                }

                Thread.Sleep(200);
            }
        }

        public Image ImageFromRawBgraArray(byte[] arr, int width, int height)
        {
            var output = new Bitmap(width, height);
            var rect = new System.Drawing.Rectangle(0, 0, width, height);
            var bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
            var ptr = bmpData.Scan0;
            Marshal.Copy(arr, 0, ptr, arr.Length);
            output.UnlockBits(bmpData);
            return output;
        }

        public int CompareFingerPrint(byte[] stored, byte[] candidate)
        {
            return zkfp2.DBMatch(mDBHandle, candidate, stored);
        }

        public void CloseDevice()
        {
            zkfp2.Terminate();
        }
    }
}