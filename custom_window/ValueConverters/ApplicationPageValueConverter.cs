using custom_window.Pages;
using System;
using System.Diagnostics;
using System.Globalization;
using custom_window.Core;

namespace custom_window
{
    /// <summary>
    /// converts the Application to an actual view/page
    /// </summary>
    class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationPage)value)
            {
                case (ApplicationPage.Login):
                    return new LoginPage();

                case (ApplicationPage.Register):
                    return new RegisterPage();

                case (ApplicationPage.Home):
                    return new Home();

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
