using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace custom_window.Core
{
    /// <summary>
    /// The View Model for a login screen
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The email of the user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool LoginIsRunning { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }
        public ICommand PatientInfoCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginViewModel()
        {
            // Create commands
            LoginCommand = new RelayParameterizedCommand(async (parameter) => await LoginAsync(parameter));
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
            PatientInfoCommand = new RelayCommand(async () => await PatientInfo());
        }

        private async Task PatientInfo()
        {
           //show a custom dialog 
           await IoC.UI.ShowMessage(new DialogViewModel
           {
               Title = "Patient Info Check",
               Message = "This is the testing message",
               OkText = "Ok"
           });
        }


        private async Task RegisterAsync()
        {
            //testing the patient slide in the main application
            IoC.Get<ApplicationViewModel>().PatientInfoCheckVisible = true;
        }

        #endregion

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task LoginAsync(object parameter)
        {
        }
    }
}