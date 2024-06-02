using System.Globalization;

namespace Client.Converters;

class CompareTwoStringConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            string value1 = values[0].ToString()??"";
            string value2 = values[1].ToString() ?? "";
            if ((value1 == value2) && !string.IsNullOrEmpty(value1))
                return true;
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
