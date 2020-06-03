﻿using SFW.Helpers;
using SFW.Model;
using System.Linq;
using System.Windows.Input;

namespace SFW.Reports
{
    public class PressShift_ViewModel : ViewModelBase
    {
        #region Properties

        public Press_ShiftReport PSReport { get; set; }
        public string ReportAction { get; set; }
        public bool CanEdit { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psReport"></param>
        public PressShift_ViewModel(Press_ShiftReport psReport)
        {
            PSReport = psReport;
            ReportAction = psReport.ReportID != null && psReport.ReportID > 0 ? "Update" : "Submit";
            CanEdit = psReport.ReportStatus == "O";
        }

        /// <summary>
        /// After a report submit or update allows the press report parent to update this child component
        /// </summary>
        /// <param name="reportID">Report unique ID</param>
        /// <param name="rAction">Report action text</param>
        public void UpdateView(int reportID, string rAction)
        {
            if (reportID > 0)
            {
                PSReport.ReportID = reportID;
                OnPropertyChanged(nameof(PSReport));
                ReportAction = rAction;
                OnPropertyChanged(nameof(ReportAction));
            }
            if (reportID == -1)
            {
                System.Windows.MessageBox.Show("This report is a duplicate and will not be submitted.\nPlease call IT if you feel you reached this message in error.", "Duplicate Report", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
    }
}
