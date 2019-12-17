using System.Windows;
using System.Windows.Controls;

namespace custom_window.ViewModels
{
    /// <summary>
    /// The view model for the custom flat window  
    /// </summary>
    public class DialogWindowViewModel : WindowViewModel
    {
        #region public properties

        /// <summary>
        /// tittle on this dialog window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// the content to host inside the window 
        /// </summary>
        public Control Content { get; set; }

        #endregion

        #region contructor

        public DialogWindowViewModel(Window window) : base(window)
        {
            
        }

        #endregion
    }
}