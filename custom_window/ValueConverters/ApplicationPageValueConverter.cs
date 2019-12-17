using System;
using System.Diagnostics;
using System.Globalization;
using custom_window.Core;
using custom_window.Pages;

namespace custom_window.ValueConverters
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

                case (ApplicationPage.History):
                    return new History();

                case (ApplicationPage.Statistics):
                    return new Statistics();

                case (ApplicationPage.About):
                    return new About();

                case (ApplicationPage.Settings):
                    return new MySetting();

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
