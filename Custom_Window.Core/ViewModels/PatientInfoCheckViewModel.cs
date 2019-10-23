using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace custom_window.Core
{
    public class PatientInfoCheckViewModel : BaseViewModel
    {
        #region public commands

        public ICommand closePatientInfoCommand { get; set; }

        #endregion

        /// <summary>
        /// true if patient info layout is to be shown
        /// </summary>
        public bool PatientInfoCheckVisible { get; set; } = false;

        #region PatientInfoRegion

        public string pName { get; set; }
        public string pOldNid { get; set; }
        public string pNewNid { get; set; }
        public string pBirth { get; set; }
        public string pPhone { get; set; }
        public string pEmail { get; set; }
        public string pAddress { get; set; }

        #endregion

        #region constructor

        public PatientInfoCheckViewModel()
        {
            //creating commands
            closePatientInfoCommand = new RelayCommand(close);
        }

        #endregion


        public void close()
        {
            IoC.Get<PatientInfoCheckViewModel>().PatientInfoCheckVisible = false;
        }

        public void GetPatientDataFromBarcodeWindow(ref string name, ref string oldNid, ref string newNid,
            ref string birth, ref string email, ref string address)
        {
        }

        public void SetWindowData(string name, string oldNid, string newNid, string birth, string email, string address)
        {
            pName = name;
            pOldNid = oldNid;
            pNewNid = newNid;
            pBirth = birth;
            pEmail = email;
            pAddress = address;
        }
    }
}