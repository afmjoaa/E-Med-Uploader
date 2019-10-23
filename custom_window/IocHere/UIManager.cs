using System.Threading.Tasks;
using System.Windows;
using custom_window.Core;
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


        public Task ShowChangePassBlock(DialogViewModel viewModel)
        {
            return new ChangePassMessageBox().ShowDialog(viewModel);
        }
    }
}