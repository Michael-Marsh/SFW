﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

//Created by Michael Marsh 5-1-18

namespace SFW.Converters
{
    public sealed class CountToVisibility : IValueConverter, IMultiValueConverter
    {
        #region IValueConverter Implementation

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                var _value = 0;
                var _param = 0;
                if (int.TryParse(parameter.ToString(), out _param) && int.TryParse(value.ToString(), out _value))
                {
                    return _value >= _param ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (int.TryParse(value.ToString(), out _value))
                {
                    return _value >= 1 ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
            else if (value != null)
            {
                if (int.TryParse(value.ToString(), out int i))
                {
                    return i >= 1 ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Collapsed;
        }

        #endregion

        #region IMultiValueConverter Implementation

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter?.ToString() == "LotList")
            {
               return System.Convert.ToInt32(values[0]) != 0 && System.Convert.ToInt32(values[1]) > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            var _test = false;
            var _count = 0;
            if (values != null)
            {
                foreach (var i in values)
                {
                    if (System.Convert.ToInt32(i) > 0 && _count == 0)
                    {
                        _test = true;
                    }
                    else if(System.Convert.ToInt32(i) > 0)
                    {
                        _test = false;
                    }
                    else if (System.Convert.ToInt32(i) == 0 && _count == 0)
                    {
                        _test = false;
                    }
                    _count++;
                }
                return _test ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var o = new object[targetTypes.Length];
            var _count = 0;
            foreach (var t in targetTypes)
            {
                o[_count] = Visibility.Visible;
                _count++;
            }
            return o;
        }

        #endregion
    }
}
