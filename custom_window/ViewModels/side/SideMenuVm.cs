using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Media.Imaging;
using custom_window.Core;
using custom_window.HelperClasses.ApplicationScope;
using Hospital = custom_window.HelperClasses.DataModels.Hospital;

namespace custom_window.ViewModels.side
{
    public class SideMenuVm : BaseViewModel
    {

        public BitmapImage hospitalImage { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/add.png"));

        public string hospitalName { get; set; } = "Hospital Name";

        public string hospitalId { get; set; } = "Registration ID";

        #region constructor

        public SideMenuVm()
        {

        }

        private static SideMenuVm PrivateInstance { get; set; }
        private static readonly object padlock = new object();

        public static SideMenuVm Instance
        {
            get
            {
                lock (padlock)
                {
                    return PrivateInstance ?? (PrivateInstance = new SideMenuVm());
                }
            }
        }

        #endregion

        public async void UpdateSideMenu()
        {
            CancellationToken cancellationToken = new CancellationToken(false);
            Hospital hospital = await ApplicationData.Instance.GetUserHospital(cancellationToken);
            hospitalName = hospital.hospitalName;
            hospitalId = hospital.registrationNumber;
           
            if (string.IsNullOrEmpty(hospital.photoUrl))
            {
                hospitalImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/add.png"));
            }
            else
            {
                var imgUrl = new Uri(hospital.photoUrl);
                //var imageData = new WebClient().DownloadData("https://firebasestorage.googleapis.com/v0/b/emed-4490e.appspot.com/o/9ULIdiStlSMDeMi43FmYmqYL3Gp1?alt=media&token=414b2883-a57a-4993-a67c-dd9e32d7a005");

                // or you can download it Async won't block your UI
                var imageData = await new WebClient().DownloadDataTaskAsync(imgUrl);

                var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.EndInit();
                hospitalImage = bitmapImage;
            }
        }

    }
}