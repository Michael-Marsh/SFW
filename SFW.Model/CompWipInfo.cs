﻿using SFW.Model.Enumerations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SFW.Model
{
    public class CompWipInfo : INotifyPropertyChanged
    {
        #region Properties

        private string lotNbr;
        public string LotNbr
        {
            get { return lotNbr; }
            set { lotNbr = value; OnPropertyChanged(nameof(LotNbr)); OnPropertyChanged(nameof(ValidLot)); }
        }

        public bool ValidLot { get; set; }

        private int? lotQty;
        public int? LotQty
        {
            get { return lotQty; }
            set { lotQty = value; OnPropertyChanged(nameof(LotQty)); }
        }

        private string rcptLoc;
        public string RcptLoc
        {
            get { return rcptLoc; }
            set { rcptLoc = value; OnPropertyChanged(nameof(RcptLoc)); }
        }

        private bool rollStatus;
        public bool RollStatus
        {
            get { return rollStatus; }
            set { rollStatus = value; OnPropertyChanged(nameof(RollStatus)); }
        }

        public bool QtyLock { get; set; }
        public string PartNbr { get; set; }
        public bool IsBackFlush { get; set; }
        public int BaseQty { get; set; }
        public string Uom { get; set; }
        public int OnHandQty { get; set; }

        private int ohCalc;
        public int OnHandCalc
        {
            get { return ohCalc; }
            set { ohCalc = value; OnPropertyChanged(nameof(OnHandCalc)); }
        }

        private Complete isScrap;
        public Complete IsScrap
        {
            get { return isScrap; }
            set
            {
                isScrap = value;
                if (value == Complete.N)
                {
                    ScrapCollection.Clear();
                }
                else
                {
                    ScrapCollection.Add(new WipReceipt.Scrap { ID = $"0*{PartNbr}*{LotNbr}"});
                }
                OnPropertyChanged(nameof(IsScrap));
            }
        }
        public ObservableCollection<WipReceipt.Scrap> ScrapCollection { get; set; }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Reflects changes from the ViewModel properties to the View
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        /// <summary>
        /// Defualt Constructor
        /// </summary>
        /// <param name="hasBFLoc">Does the component have a default backflush location</param>
        /// <param name="partNbr">Part Number of the component</param>
        /// <param name="uom">Part Unit of Measure of the component</param>
        public CompWipInfo(bool hasBFLoc, string partNbr, string uom)
        {
            IsBackFlush = hasBFLoc;
            PartNbr = partNbr;
            Uom = uom;
            QtyLock = false;
            ScrapCollection = new ObservableCollection<WipReceipt.Scrap>(new List<WipReceipt.Scrap>());
            IsScrap = Complete.N;
            ValidLot = false;
        }
    }
}
