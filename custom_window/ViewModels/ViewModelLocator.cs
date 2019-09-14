using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using custom_window.Core;

namespace custom_window
{
    /// <summary>
    /// locates viewModels from ioc to bind in xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public properties

        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();
        public static ApplicationViewModel ApplicationViewModel => IoC.Get<ApplicationViewModel>();

        #endregion

    }
}
