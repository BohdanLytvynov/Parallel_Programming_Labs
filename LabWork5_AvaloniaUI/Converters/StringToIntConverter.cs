using Avalonia.Data.Converters;
using Avalonia.Data;
using System.Globalization;
using System;

namespace LabWork5_AvaloniaUI.Converters
{
    /// <summary>
    /// Converts string to int
    /// </summary>
    internal class StringToIntConverter : IValueConverter
    {
        //ViewModel -> View // string
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        // View -> ViewModel // int?
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? s = value as string;

            if (string.IsNullOrWhiteSpace(s))
            {
                return null;//This will cause Required to work
            }

            //Try to parse
            if (int.TryParse(s, out int result))
            {
                return result;
            }
            //Binding error, Avalonia will think as invalid input but without exceptions
            return new BindingNotification(new InvalidCastException("Input number!"), BindingErrorType.Error);
        }
    }
}
