using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
namespace Margatsni.Controls.Converters
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture.TwoLetterISOLanguageName);

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture.TwoLetterISOLanguageName);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool va = (value is bool && (bool)value);
            var p = parameter as string;
            bool vap = false;
            if (!string.IsNullOrEmpty(p))
            {
                if (p.ToLower() == "false"){
                    vap = true;
                }
            }

            va ^= vap;
         
            var re = (va) ? Visibility.Visible : Visibility.Collapsed;
            return re;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool re = value is Visibility && (Visibility)value == Visibility.Visible;
            if (parameter is bool)
            {
                re ^= (bool)parameter;
            }

            return re;
        }
    }
}
