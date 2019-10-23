using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace custom_window.Core
{
    public class PatientInfoCheckViewModel: BaseViewModel
    {
        #region public commands

        public ICommand closePatientInfoCommand { get; set; }


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
            IoC.Get<ApplicationViewModel>().PatientInfoCheckVisible = false;
        }
    }
}
