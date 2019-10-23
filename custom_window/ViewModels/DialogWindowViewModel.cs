using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using custom_window.Core;

namespace custom_window
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