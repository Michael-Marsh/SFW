﻿using SFW.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

//Created by Michael Marsh 5-3-18

namespace SFW.Converters
{
    public class WorkCenterNameConverter : IValueConverter
    {
        #region Properties

        public static List<Machine> MachineList { get; private set; }

        #endregion

        #region IValueConverter Implementation

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? $"{MachineList.FirstOrDefault(o => o.MachineNumber == value.ToString()).MachineName} ({value.ToString()})" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion

        /// <summary>
        /// Work Center Name Converter Default Constructor
        /// </summary>
        /// <param name="workOrderList"></param>
        public WorkCenterNameConverter()
        {
            if (MachineList == null)
            {
                MachineList = Machine.GetMachineList(App.AppSqlCon);
            }
        }
    }
}