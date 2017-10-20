namespace eVision.eCrypt.KeyGenerator.Helpers.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value.Equals(parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value.Equals(true) ? parameter : Binding.DoNothing;
    }
}
