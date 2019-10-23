using System.Threading.Tasks;
using System.Windows;
using custom_window.Core;
using Custom_Window.Core.ViewModels.Dialogs;
using MaterialDesignThemes.Wpf;

namespace custom_window
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
    }
}