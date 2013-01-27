using System;
using System.Windows.Data;

namespace TimeMe
{
    //Converter to generate the string
    public class FormatStringConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //append the string
            return string.Format(parameter.ToString(), value.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
