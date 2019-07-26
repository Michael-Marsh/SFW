﻿using SFW.Commands;
using SFW.Helpers;
using SFW.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

//Created by Michael Marsh 10-23-18

namespace SFW.WIP
{
    public class ViewModel : ViewModelBase
    {
        #region Properties

        public WipReceipt WipRecord { get; set; }
        
        public int? WipQuantity
        {
            get { return WipRecord.WipQty; }
            set
            {
                if (value == 0 || value == null)
                {
                    foreach (var c in WipRecord.WipWorkOrder.Bom)
                    {
                        c.WipInfo.Clear();
                        c.WipInfo.Add(new CompWipInfo(!string.IsNullOrEmpty(c.BackflushLoc), c.CompNumber, c.CompUom));
                    }
                }
                else if (WipRecord.WipQty != value)
                {
                    if (value == -1)
                    {
                        value = WipRecord.WipQty;
                    }
                    foreach (var c in WipRecord.WipWorkOrder.Bom)
                    {
                        var _qty = Convert.ToInt32(value);
                        if (WipRecord.IsMulti)
                        {
                            _qty *= Convert.ToInt32(RollQuantity);
                        }
                        if (WipRecord.IsScrap == Model.Enumerations.Complete.Y)
                        {
                            _qty += Convert.ToInt32(ScrapQuantity);
                        }
                        c.UpdateWipInfo(_qty);
                    }
                }
                WipRecord.WipQty = value;
                OnPropertyChanged(nameof(WipQuantity));
                OnPropertyChanged(nameof(WipRecord));
            }
        }

        public int? ScrapQuantity
        {
            get { return WipRecord.ScrapQty; }
            set
            {
                WipRecord.ScrapQty = value;
                OnPropertyChanged(nameof(ScrapQuantity));
                WipQuantity = -1;
            }
        }

        public Model.Enumerations.Complete Scrap
        {
            get { return WipRecord.IsScrap; }
            set
            {
                WipRecord.IsScrap = value;
                OnPropertyChanged(nameof(Scrap));
                ScrapQuantity = null;
                WipRecord.ScrapReason = null;
                WipRecord.ScrapReference = SelectedReason = null;
                WipQuantity = -1;
            }
        }

        public int? RollQuantity
        {
            get { return WipRecord.RollQty; }
            set
            {
                WipRecord.RollQty = value;
                OnPropertyChanged(nameof(RollQuantity));
                WipQuantity = -1;
            }
        }

        public bool Multi
        {
            get { return WipRecord.IsMulti; }
            set
            {
                WipRecord.IsMulti = value;
                OnPropertyChanged(nameof(Multi));
                if (!value)
                {
                    RollQuantity = null;
                }
            }
        }

        public bool HasCrew { get { return WipRecord.CrewList != null; } }

        private int? tQty;
        public int? TQty
        {
            get { return tQty; }
            set { tQty = value; OnPropertyChanged(nameof(TQty)); }
        }

        private bool vLoc;
        public bool ValidLocation
        {
            get { return vLoc; }
            set { vLoc = value; OnPropertyChanged(nameof(ValidLocation)); }
        }

        public bool IsLotTrace
        {
            get { return WipRecord.IsLotTracable || WipRecord.WipWorkOrder.Bom.Count(o => o.IsLotTrace) > 0; }
        }

        public ObservableCollection<string> ScrapReasonCollection { get; set; }
        public string SelectedReason
        {
            get { return WipRecord.ScrapReason; }
            set { WipRecord.ScrapReason = value; OnPropertyChanged(nameof(SelectedReason)); }
        }

        RelayCommand _wip;
        RelayCommand _mPrint;
        RelayCommand _removeCrew;
        RelayCommand _removeComp;

        #endregion

        /// <summary>
        /// WIP ViewModel Default Constructor
        /// </summary>
        public ViewModel(WorkOrder woObject)
        {
            WipRecord = new WipReceipt(CurrentUser.FirstName, CurrentUser.LastName, woObject, App.AppSqlCon);
            if (ScrapReasonCollection == null)
            {
                var _tempList = Enum.GetValues(typeof(M2kClient.AdjustCode)).Cast<M2kClient.AdjustCode>().Where(o => o != M2kClient.AdjustCode.CC);
                var _descList = new List<string>();
                foreach (var e in _tempList)
                {
                    _descList.Add(e.GetDescription());
                }
                ScrapReasonCollection = new ObservableCollection<string>(_descList);
            }
        }

        /// <summary>
        /// Validates that the LotQty for the components equal the main part Wip quantity
        /// </summary>
        /// <returns>Validation response as bool</returns>
        private bool ValidateComponents()
        {
            var _validLoc = true;
            var _validQty = true;
            var _validScrap = true;
            foreach (var c in WipRecord.WipWorkOrder.Bom.Where(o => o.IsLotTrace))
            {
                if (string.IsNullOrEmpty(c.BackflushLoc))
                {
                    _validLoc = c.WipInfo.Count(o => !string.IsNullOrEmpty(o.LotNbr)) == c.WipInfo.Count(o => !string.IsNullOrEmpty(o.RcptLoc));
                }
                else
                {
                    _validLoc = c.WipInfo.Where(o => !o.ValidLot && !string.IsNullOrEmpty(o.LotNbr)).Count() == 0;
                }
                if (WipRecord.IsScrap == Model.Enumerations.Complete.Y)
                {
                    _validQty = Math.Round(Convert.ToDouble(WipRecord.WipQty + WipRecord.ScrapQty) * c.AssemblyQty, 0) == c.WipInfo.Where(o => !string.IsNullOrEmpty(o.LotNbr)).Sum(o => o.LotQty);
                }
                else
                {
                    _validQty = Math.Round(Convert.ToDouble(WipRecord.WipQty) * c.AssemblyQty, 0) == c.WipInfo.Where(o => !string.IsNullOrEmpty(o.LotNbr)).Sum(o => o.LotQty);
                }
                _validScrap = c.WipInfo.Count(o => o.ScrapQty > 0) == c.WipInfo.Count(o => !string.IsNullOrEmpty(o.ScrapReason)) 
                    && c.WipInfo.Count(o => o.ScrapReason == "QSC") == c.WipInfo.Count(o => !string.IsNullOrEmpty(o.ScrapReference));
                if (!_validLoc || !_validQty)
                {
                    return false;
                }
            }
            return true;
        }

        #region Process Wip ICommand

        public ICommand WipICommand
        {
            get
            {
                if (_wip == null)
                {
                    _wip = new RelayCommand(WipExecute, WipCanExecute);
                }
                return _wip;
            }
        }

        private void WipExecute(object parameter)
        {
            if (WipReceipt.ValidLocation(WipRecord.ReceiptLocation, App.AppSqlCon))
            {
                var _machID = WipRecord.CrewList?.Count > 0 ? WorkOrder.GetAssignedMachineID(WipRecord.WipWorkOrder.OrderNumber, WipRecord.WipWorkOrder.Seq, App.AppSqlCon) : "";
                var _wipProc = M2kClient.M2kCommand.ProductionWip(WipRecord, WipRecord.CrewList?.Count > 0, App.ErpCon, WipRecord.IsLotTracable, _machID);
                if (_wipProc != null && _wipProc.First().Key > 0)
                {
                    WipRecord.WipLot.LotNumber = _wipProc.First().Value;
                    WipRecord.IsScrap = Model.Enumerations.Complete.N;
                    WipRecord.IsReclaim = Model.Enumerations.Complete.N;
                    WipRecord.IsMulti = false;
                    OnPropertyChanged(nameof(WipRecord));
                    TQty = WipQuantity;
                    WipQuantity = null;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You have entered an invalid receipt location.\nPlease double check your entery and try again."
                    ,"Invalid Location"
                    ,System.Windows.MessageBoxButton.OK
                    ,System.Windows.MessageBoxImage.Warning);
            }
        }
        private bool WipCanExecute(object parameter)
        {
            if (WipRecord != null)
            {
                var _baseValid = WipRecord.WipQty > 0 && !string.IsNullOrEmpty(WipRecord.ReceiptLocation) && ValidateComponents();
                var _multiValid = !WipRecord.IsMulti || (WipRecord.IsMulti && WipRecord.RollQty > 0);
                var _scrapValid = true;
                var _laborValid = WipRecord.CrewList.Where(o => DateTime.TryParse(o.LastClock, out var dt) && !string.IsNullOrEmpty(o.Name) && o.IsDirect).ToList().Count() == WipRecord.CrewList.Count(o => !string.IsNullOrEmpty(o.Name) && o.IsDirect);
                if (WipRecord.IsScrap == Model.Enumerations.Complete.Y)
                {
                    if (WipRecord.ScrapQty == 0)
                    {
                        _scrapValid = false;
                    }
                    else if (string.IsNullOrEmpty(WipRecord.ScrapReason))
                    {
                        _scrapValid = false;
                    }
                    else if (WipRecord.ScrapReason == "Quality Scrap")
                    {
                        if (string.IsNullOrEmpty(WipRecord.ScrapReference))
                        {
                            _scrapValid = false;
                        }
                    }
                }
                var _reclaimValid = WipRecord.IsReclaim == Model.Enumerations.Complete.N || (WipRecord.IsReclaim == Model.Enumerations.Complete.Y && WipRecord.ReclaimQty > 0);
                return _baseValid && _multiValid && _scrapValid && _reclaimValid && _laborValid;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Material Card Print ICommand

        public ICommand MPrintICommand
        {
            get
            {
                if (_mPrint == null)
                {
                    _mPrint = new RelayCommand(MPrintExecute, MPrintCanExecute);
                }
                return _mPrint;
            }
        }

        private void MPrintExecute(object parameter)
        {
            var _wQty = TQty == null || TQty == 0 ? Convert.ToInt32(WipRecord.WipQty) : Convert.ToInt32(TQty);
            TravelCard.Create("", "technology#1",
                WipRecord.WipWorkOrder.SkuNumber,
                WipRecord.WipLot.LotNumber,
                WipRecord.WipWorkOrder.SkuDescription,
                Sku.GetDiamondNumber(WipRecord.WipLot.LotNumber, App.AppSqlCon),
                _wQty,
                WipRecord.WipWorkOrder.Uom,
                Lot.GetAssociatedQIR(WipRecord.WipLot.LotNumber, App.AppSqlCon));
            switch(parameter.ToString())
            {
                case "T":
                    TravelCard.Display();
                    break;
                case "R":
                    TravelCard.DisplayReference();
                    break;
                    
            }
        }
        private bool MPrintCanExecute(object parameter) => true;

        #endregion

        #region Remove Crew List Item ICommand

        public ICommand RemoveCrewICommand
        {
            get
            {
                if (_removeCrew == null)
                {
                    _removeCrew = new RelayCommand(RemoveCrewExecute, RemoveCrewCanExecute);
                }
                return _removeCrew;
            }
        }

        private void RemoveCrewExecute(object parameter)
        {
            WipRecord.CrewList.Remove(WipRecord.CrewList.FirstOrDefault(c => c.IdNumber.ToString() == parameter.ToString()));
        }
        private bool RemoveCrewCanExecute(object parameter) => parameter != null && !string.IsNullOrEmpty(parameter.ToString());

        #endregion

        #region Remove Component List Item ICommand

        public ICommand RemoveCompICommand
        {
            get
            {
                if (_removeComp == null)
                {
                    _removeComp = new RelayCommand(RemoveCompExecute, RemoveCompCanExecute);
                }
                return _removeComp;
            }
        }

        private void RemoveCompExecute(object parameter)
        {
            foreach (var c in WipRecord.WipWorkOrder.Bom)
            {
                foreach (var w in c.WipInfo)
                {
                    if (w.LotNbr == parameter.ToString())
                    {
                        c.WipInfo.Remove(w);
                        return;
                    }
                }
            }
        }
        private bool RemoveCompCanExecute(object parameter) => parameter != null && !string.IsNullOrEmpty(parameter.ToString());

        #endregion

        /// <summary>
        /// Object disposal
        /// </summary>
        /// <param name="disposing">Called by the GC Finalizer</param>
        public override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                WipRecord = null;
                _wip = null;
                ((Schedule.ViewModel)Controls.WorkSpaceDock.WccoDock.GetChildOfType<Schedule.View>().DataContext).RefreshSchedule();
            }
        }
    }
}
