﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SFW.Converters
{
    public class SiteToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "User")
            {
                return App.SiteNumber == 2 && (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (parameter.ToString() == "Input")
            {
                return App.SiteNumber == 2 && !(bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (parameter.ToString() == "Ref")
            {
                if (App.SiteNumber == 2)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            else if (string.IsNullOrEmpty(parameter?.ToString()))
            {
                switch (value.ToString())
                {
                    case "2":
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
            else
            {
                switch (value.ToString())
                {
                    case "1":
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Collapsed;
        }
    }
}
