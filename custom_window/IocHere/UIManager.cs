using System.Threading.Tasks;
using custom_window.Core;
using custom_window.Dialogs;

namespace custom_window.IocHere
{
    /// <summary>
    /// showing a dialog message
    /// </summary>
    public class UIManager : IUIManager
    {
        public Task ShowMessage(DialogViewModel viewModel)
        {
            return  new DialogMessageBox().ShowDialog(viewModel);
        }


        public Task ShowChangePassBlock(ChangePassViewModel viewModel)
        {
            return new ChangePassMessageBox().ShowDialog(viewModel);
        }

        public Task ShowForgetPassBlock(ForgetPassViewModel viewModel)
        {
            return new ForgetPassDialog().ShowDialog(viewModel);
        }

        public Task ShowConfirmMessage(ConfirmDialogViewModel viewModel)
        {
            return new ConfirmDialog().ShowDialog(viewModel);

        }
    }
}