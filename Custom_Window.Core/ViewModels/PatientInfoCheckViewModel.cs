using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace custom_window.Core
{
    public class PatientInfoCheckViewModel : BaseViewModel
    {
        #region public commands

        public ICommand closePatientInfoCommand { get; set; } = null;

        #endregion

        /// <summary>
        /// true if patient info layout is to be shown
        /// </summary>
        public bool PatientInfoCheckVisible { get; set; } = false;


        #region selectedPatient

        public string selectedPatientId { get; set; }

        public string selectedPatientName { get; set; } = "No patient is selected";

        public bool DiscardSelectedPatientVisible { get; set; } = false;

        #endregion

        public void selectPatient(string patientId, string patientName)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                selectedPatientName = "No patient is selected";
                selectedPatientId = null;
                DiscardSelectedPatientVisible = false;
            }
            else
            {
                selectedPatientName = patientName + " is selected";
                selectedPatientId = patientId;
                DiscardSelectedPatientVisible = true;

            }
        }

        /// <summary>
        /// content will be served  based on the patient account type
        /// three values can be possible acc == account
        /// 1. noAcc 2. existingAccWithNID 3. existingAccWithoutNID
        /// </summary>
        public ContentType CurrentContent { get; set; } = ContentType.NewPatientRegistration;

        #region PatientInfoRegion

        public bool visibleStateOne { get; set; } = true;
        public bool visibleStateTwo { get; set; } = true;
        public bool visibleStateThree { get; set; } = true;
        public bool visibleStateFour { get; set; } = true;


        public bool isCheckOne { get; set; } = false;
        public bool isCheckTwo { get; set; } = false;
        public bool isCheckThree { get; set; } = false;
        public bool isCheckFour { get; set; } = false;

        public String name { get; set; }
        public String phone { get; set; }
        public String email { get; set; }
        public String birth { get; set; }
        public String permanent_address { get; set; }
        public String present_address { get; set; }
        public String voting_area { get; set; }
        public String issue_date { get; set; }
        public String old_nid { get; set; }
        public String new_nid { get; set; }
        public bool addFinger { get; set; }

        //last two are not useful
        public String display_pic { get; set; }

        public BitmapImage PatiemtImage { get; set; } =new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/patientDemo.png"));
        public List<string> FingerPrintList { get; set; } = new List<string>();

        #endregion

        #region constructor

        public PatientInfoCheckViewModel()
        {
            //creating commands
            closePatientInfoCommand = new RelayCommand(HidePatientInfo);
        }

        #endregion


        public void SetWindowData(string mName, string mPhone, string mEmail, string mBirth, string mPermanent_address,
            string mPresent_address,
            string mVoting_area, string mIssue_date, string mDisplay_pic, string mOld_nid, string mNew_nid,
            bool mAddFinger)
        {
            name = mName;
            phone = mPhone;
            email = mEmail;
            birth = mBirth;
            permanent_address = mPermanent_address;
            present_address = mPresent_address;
            voting_area = mVoting_area;
            issue_date = mIssue_date;
            old_nid = mOld_nid;
            new_nid = mNew_nid;
            addFinger = mAddFinger;
            display_pic = mDisplay_pic;
            if (!string.IsNullOrEmpty(display_pic))
            {
                try
                {
                    SetDisplayPic(mDisplay_pic);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            nullFingerPrints();
        }


        private async Task SetDisplayPic(string displypic)
        {
            //set the display pic 
            if (string.IsNullOrEmpty(displypic))
            {
                PatiemtImage = new BitmapImage(new Uri("pack://application:,,,/Images/BackGround/patientDemo.png"));
            }
            else
            {
                var imgUrl = new Uri(displypic);
                // or you can download it Async won't block your UI
                var imageData = await new WebClient().DownloadDataTaskAsync(imgUrl);

                var bitmapImage = new BitmapImage { CacheOption = BitmapCacheOption.OnLoad };
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                Dispatcher.CurrentDispatcher.Invoke(() => PatiemtImage = bitmapImage);
                
            }
        }


        public void NullWindowData()
        {
            name = "";
            phone = "";
            email = "";
            birth = "";
            permanent_address = "";
            present_address = "";
            voting_area = "";
            issue_date = "";
            old_nid = "";
            new_nid = "";
            display_pic = "";
            addFinger = false;
            try
            {
                SetDisplayPic(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
            nullFingerPrints();
        }

        private void nullFingerPrints()
        {
            //do what need to be done to make the binding work
            //set the finger print 
            isCheckOne = false;
            isCheckTwo = false;
            isCheckThree = false;
            isCheckFour = false;
            visibleStateOne = true;
            visibleStateTwo = true;
            visibleStateThree = true;
            visibleStateFour = true;
        }


        public void ShowNewOrOld(ContentType pageName)
        {
            IoC.Get<ApplicationViewModel>().CurrentWindowVisible = false;
            IoC.Get<ApplicationViewModel>().SideMenuVisible = false;
            this.PatientInfoCheckVisible = true;
            this.CurrentContent = pageName;
        }

        public void HidePatientInfo()
        {
            IoC.Get<ApplicationViewModel>().CurrentWindowVisible = true;
            IoC.Get<ApplicationViewModel>().SideMenuVisible = true;
            this.PatientInfoCheckVisible = false;
        }
    }
}