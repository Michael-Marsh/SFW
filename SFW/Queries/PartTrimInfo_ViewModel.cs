﻿using SFW.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace SFW.Queries
{
    public class PartTrimInfo_ViewModel : ViewModelBase
    {
        #region Properties

        public string SkuNumber { get; set; }
        public string SetupNumber { get; set; }
        public string Revision { get; set; }
        public string FirstGuideBarLoc { get; set; }
        public string SecondGuideBarLoc { get; set; }
        public string CenterGuideBarLoc { get; set; }
        public string SlideRailLoc { get; set; }
        public string VGuideBar { get; set; }
        public string CrossBar { get; set; }
        public string ModTemplate { get; set; }
        public string CenterGuideBar { get; set; }
        public string RecessBar { get; set; }
        public string Notes { get; set; }
        RelayCommand _setupCmd;

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PartTrimInfo_ViewModel()
        { }

        /// <summary>
        /// Overridden Constructor
        /// Used to prepopulate the TrimmingInfo list
        /// </summary>
        /// <param name="trimInfo">TrimmingInfo list</param>
        public PartTrimInfo_ViewModel(List<string> trimInfo)
        {
            SkuNumber = trimInfo[0];
            SetupNumber = trimInfo[1];
            Revision = trimInfo[2];
            FirstGuideBarLoc = trimInfo[3];
            SecondGuideBarLoc = trimInfo[4];
            CenterGuideBarLoc = trimInfo[5];
            SlideRailLoc = trimInfo[6];
            VGuideBar = trimInfo[7];
            CrossBar = trimInfo[8];
            ModTemplate = trimInfo[9];
            CenterGuideBar = trimInfo[10];
            RecessBar = trimInfo[11];
            Notes = trimInfo.Count > 12 ? trimInfo[12] : string.Empty;
        }

        #region Open Setup Print ICommand

        public ICommand OpenSetupICommand
        {
            get
            {
                if (_setupCmd == null)
                {
                    _setupCmd = new RelayCommand(SetupExecute, SetupCanExecute);
                }
                return _setupCmd;
            }
        }

        private void SetupExecute(object parameter)
        {
            try
            {
                var _tool = string.Empty;
                if (parameter.ToString().Contains("*"))
                {
                    var _type = parameter.ToString().Split('*');
                    switch (_type[1])
                    {
                        case "V":
                            _tool = VGuideBar;
                            break;
                        case "C":
                            _tool = CenterGuideBar;
                            break;
                        case "R":
                            _tool = RecessBar;
                            break;
                        case "M":
                            _tool = ModTemplate;
                            break;
                        case "X":
                            _tool = CrossBar;
                            break;
                    }
                }
                else
                {
                    _tool = SetupNumber;
                }
                Process.Start($"{App.GlobalConfig.First(o => o.Site == App.Facility).PartPrint}{ _tool}.pdf");
            }
            catch (Win32Exception)
            {
                System.Windows.MessageBox.Show("The set up or tooling print you have selected does not exist.\nPlease contact engineering for assistance.", "File not Found", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Please contact IT.\n\n{ex.Message}", "Unhandled Exception", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        private bool SetupCanExecute(object parameter) => true;

        #endregion
    }
}
