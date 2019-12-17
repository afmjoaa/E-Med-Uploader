using System;
using System.Diagnostics;
using System.Globalization;
using custom_window.Controls.PatientContent;
using custom_window.Core;

namespace custom_window.ValueConverters
{
    /// <summary>
    /// converts the content to control for patientInfoCheck control
    /// </summary>
    class ContentValueConverter : BaseValueConverter<ContentValueConverter>
    {
        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ContentType)value)
            {
                case (ContentType.NewPatientRegistration):
                    return new NewPatientRegistration();

                case (ContentType.ExistingPatientInfo):
                    return new ExistingPatientInfo();

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
