﻿using M2kClient;
using SFW.Controls;
using SFW.Queries;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

//Created by Michael Marsh 4-19-18

namespace SFW.Commands
{
    public class ViewLoad : ICommand
    {
        public static IList<int> HistoryList { get; set; }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// View Load Execution
        /// </summary>
        /// <param name="parameter">View to Load</param>
        public void Execute(object parameter)
        {
            if (HistoryList == null)
            {
                HistoryList = new List<int>();
            }
            try
            {
                var _temp = App.AppSqlCon.Database;
                var _wo = new object();
                if (parameter.GetType() == typeof(Model.WorkOrder) || parameter.ToString().Length > 3)
                {
                    _wo = parameter;
                    parameter = 3;
                }
                var _view = int.TryParse(parameter.ToString(), out int i) ? i : App.SiteNumber;
                var _viewModel = new object();
                _viewModel = null;
                //Handling the back function
                if (_view == -1)
                {
                    _view = App.SiteNumber;
                    if (HistoryList.Count > 0 && HistoryList.Count - 1 > 0)
                    {
                        HistoryList.RemoveAt(HistoryList.Count - 1);
                        _view = HistoryList[HistoryList.Count - 1];
                    }
                }
                if (_view == App.SiteNumber)
                {
                    HistoryList.Clear();
                }
                switch (_view)
                {
                    //The part information command calls can either send a work order object, part number or a null variable.  Need to handle each case
                    case 3:
                        if (_wo != null)
                        {
                            _viewModel = parameter.GetType() == typeof(Model.WorkOrder) ? new PartInfo_ViewModel((Model.WorkOrder)_wo) : new PartInfo_ViewModel(_wo.ToString());
                        }
                        break;
                    //Handles the refresh schedule calls
                    case -2:
                        if (!RefreshTimer.IsRefreshing)
                        {
                            RefreshTimer.RefreshTimerTick();
                        }
                        else
                        {
                            MessageBox.Show("The work load is currently refreshing.");
                        }
                        break;
                    //Currently the closed work orders are site agnostic so need to load the viewmodel at the time of the call
                    case 6:
                        _viewModel= new Schedule.Closed.ViewModel();
                        break;
                    //Each site has its own build out for the database change
                    case 0:
                        if (!App.DatabaseChange("CSI_MAIN"))
                        {
                            App.DatabaseChange(_temp);
                            return;
                        }
                        App.ErpCon.DatabaseChange(Database.CSI);
                        break;
                    case 1:
                        if (!App.DatabaseChange("WCCO_MAIN"))
                        {
                            App.DatabaseChange(_temp);
                            return;
                        }
                        App.ErpCon.DatabaseChange(Database.WCCO);
                        break;
                }
                if(_view != -2)
                {
                    HistoryList.Add(_view);
                    WorkSpaceDock.SwitchView(_view, _viewModel);
                }
            }
            catch (Exception)
            {

            }
        }
        public bool CanExecute(object parameter) => true;
    }
}
