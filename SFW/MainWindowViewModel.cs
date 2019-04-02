﻿using SFW.Controls;
using SFW.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

//Created by Michael Marsh 4-19-18

namespace SFW
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties

        private static List<Machine> _mList;
        public static List<Machine> MachineList
        {
            get { return _mList; }
            set { _mList = value; StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MachineList))); }
        }

        private static Machine mach;
        public static Machine SelectedMachine
        {
            get { return mach; }
            set
            {
                if (value == null)
                {
                    value = MachineList.FirstOrDefault(o => o.MachineName == "All");
                }
                else if (mach != value && !IsChanging)
                {
                    IsChanging = true;
                    if (value.MachineGroup != SelectedMachineGroup)
                    {
                        SelectedMachineGroup = value.MachineGroup;
                    }
                    var _filter = value.MachineName == "All" ? "" : $"MachineName = '{value.MachineName}'";
                    var _dock = App.SiteNumber == 0 ? WorkSpaceDock.CsiDock : WorkSpaceDock.WccoDock;
                    _dock.Dispatcher.BeginInvoke(new Action(() => 
                    {
                        if (((Schedule.ViewModel)((Schedule.View)_dock.Children[0]).DataContext).ScheduleView != null)
                        {
                            if (((DataView)((Schedule.ViewModel)((Schedule.View)_dock.Children[0]).DataContext).ScheduleView.SourceCollection).Table.Select($"MachineNumber = '{value.MachineNumber}'").Length == 0)
                            {
                                ((ShopRoute.ViewModel)((ShopRoute.View)_dock.Children[1]).DataContext).ShopOrder = new WorkOrder();
                            }
                            ((DataView)((Schedule.ViewModel)((Schedule.View)_dock.Children[0]).DataContext).ScheduleView.SourceCollection).RowFilter = _filter;
                        }
                        IsChanging = false;
                    }));
                }
                mach = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(SelectedMachine)));
            }
        }

        private static List<string> _mGrpList;
        public static List<string> MachineGroupList
        {
            get { return _mGrpList; }
            set { _mGrpList = value; StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MachineGroupList))); }
        }

        private static string machGrp;
        public static string SelectedMachineGroup
        {
            get { return machGrp; }
            set
            {
                if (machGrp != value && !IsChanging)
                {
                    IsChanging = true;
                    var _temp = value == "All" ? "" : $"MachineGroup = '{value}'";
                    var _tempDock = App.SiteNumber == 0 ? WorkSpaceDock.CsiDock : WorkSpaceDock.WccoDock;
                    if (((Schedule.ViewModel)((Schedule.View)_tempDock.Children[0]).DataContext).ScheduleView != null)
                    {
                        ((DataView)((Schedule.ViewModel)((Schedule.View)_tempDock.Children[0]).DataContext).ScheduleView.SourceCollection).RowFilter = _temp;
                    }
                    SelectedMachine = MachineList.FirstOrDefault(o => o.MachineName == "All");
                    IsChanging = false;
                }
                machGrp = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(SelectedMachineGroup)));
            }
        }

        public static string CurrentSite
        {
            get
            { return App.Site; }
            set
            {
                value = App.Site;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentSite)));
            }
        }

        public static int CurrentSiteNbr
        {
            get
            { return App.SiteNumber; }
            set
            {
                value = App.SiteNumber;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(CurrentSiteNbr)));
            }
        }

        private static bool IsChanging;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public event EventHandler CanExecuteChanged;

        #endregion

        /// <summary>
        /// Main Window ViewModel Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            IsChanging = false;
            new WorkSpaceDock();
        }

        /// <summary>
        /// Updates all the static properties in the MainWindow View components when other views require new data
        /// </summary>
        public void UpdateProperties()
        {
            MachineList = Machine.GetMachineList(App.AppSqlCon, true);
            SelectedMachine = MachineList.First();
            MachineGroupList = Machine.GetMachineGroupList(App.AppSqlCon, true);
            SelectedMachineGroup = MachineGroupList.First();
            CurrentSite = App.Site;
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MachineList)));
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(MachineGroupList)));
        }
    }
}
