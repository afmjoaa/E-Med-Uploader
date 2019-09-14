using custom_window.Pages;
using System;
using System.Diagnostics;
using System.Globalization;
using custom_window.Core;
using Ninject;

namespace custom_window
{
    /// <summary>
    /// Converts a string name to a service pulled from IoC container
    /// not using this file
    /// </summary>
    class IoCConverter : BaseValueConverter<IoCConverter>
    {
        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case nameof(ApplicationViewModel):
                    return IoC.Get<ApplicationViewModel>();

                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
