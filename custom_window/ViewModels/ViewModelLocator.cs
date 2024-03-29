﻿using custom_window.Core;

namespace custom_window.ViewModels
{
    /// <summary>
    /// locates viewModels from ioc to bind in xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public properties

        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();
        public static ApplicationViewModel ApplicationViewModel => IoC.Get<ApplicationViewModel>();
        public static PatientInfoCheckViewModel PatientInfoCheckViewModel => IoC.Get<PatientInfoCheckViewModel>();

        #endregion
    }
}