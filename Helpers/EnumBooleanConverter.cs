using System;
using System.Windows;
using System.Windows.Data;

namespace PDFRider
{
    /// <summary>
    /// This converter is used to data bind checked/uncheckd (bool) value of e.g. a radiobutton 
    /// to the corresponding value of an underlying enum.
    /// This class is from David Schmitt (http://stackoverflow.com/users/4918/david-schmitt)
    /// Posted at http://stackoverflow.com/questions/397556/wpf-how-to-bind-radiobuttons-to-an-enum
    /// </summary>
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
    }

}
