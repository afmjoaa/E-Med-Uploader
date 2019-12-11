using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;

namespace custom_window.Core
{
    public class RegisterViewModel : BaseViewModel
    {
        //public ICommand SigninCommand { get; set; }
        public bool SigninIsRunning { get; set; }

        public RegisterViewModel()
        {
            //SigninCommand = new RelayParameterizedCommand(async (parameter) => await SigninAsync(parameter));
            

        }

        /*private async Task SigninAsync(object parameter)
        {
            await RunCommand(() => this.SigninIsRunning, async() =>
            {
                await Task.Delay(50);

                IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Home);
            });
        }*/
    }
}
