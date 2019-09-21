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
        }

        private async Task RegisterAsync()
        {
            /*IoC.Get<ApplicationViewModel>().SideMenuVisible ^= true;
            return;*/

            //todo got to the register page
            IoC.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.Home;
            //((WindowViewModel) ((MainWindow) Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Register;
            await Task.Delay(1);
        }

        #endregion

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task LoginAsync(object parameter)
        {
            await RunCommand(() => this.LoginIsRunning, async () =>
            {
                await Task.Delay(50);

                var phoneNumber = this.PhoneNumber;

                // IMPORTANT: Never store unsecure password in variable like this
                var pass = (parameter as IHavePassword).SecurePassword.Unsecure();

                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Register);
            });
        }
    }
}