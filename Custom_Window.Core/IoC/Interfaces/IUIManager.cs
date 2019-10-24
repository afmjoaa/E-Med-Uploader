using System.Threading.Tasks;

namespace custom_window.Core
{
    public interface IUIManager
    {
        /// <summary>
        /// displays a simple msg box to the user
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task ShowMessage(DialogViewModel viewModel);

        Task ShowChangePassBlock(ChangePassViewModel viewModel);
    }
}
