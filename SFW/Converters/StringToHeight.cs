﻿using System;
using System.Windows.Data;

namespace SFW.Converters
{
    public sealed class StringToHeight : IValueConverter
    {
        #region IValueConverter Implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && int.TryParse(parameter.ToString(), out int hgt))
            {
                return !string.IsNullOrEmpty(value.ToString()) ? hgt : 0;
            }
            else
            {
                return 0;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }

        #endregion
    }
}
