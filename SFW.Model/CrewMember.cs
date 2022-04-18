﻿using IBMU2.UODOTNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace SFW.Model
{
    public class CrewMember : ModelBase
    {
        #region Properties

        private string _idNbr;
        public string IdNumber
        {
            get { return _idNbr; }
            set { _idNbr = value; OnPropertyChanged(nameof(IdNumber)); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private bool _isDirect;
        public bool IsDirect
        {
            get
            { return _isDirect; }
            set
            { _isDirect = value; OnPropertyChanged(nameof(IsDirect)); }
        }

        private int _shift;
        public int Shift
        {
            get
            { return _shift; }
            set
            { _shift = value; OnPropertyChanged(nameof(Shift)); }
        }

        private string _shiftStart;
        public string ShiftStart
        {
            get
            { return _shiftStart; }
            set
            { _shiftStart = value; OnPropertyChanged(nameof(ShiftStart)); }
        }

        private string _shiftEnd;
        public string ShiftEnd
        {
            get
            { return _shiftEnd; }
            set
            { _shiftEnd = value; OnPropertyChanged(nameof(Shift)); }
        }

        private string _lastClock;
        public string LastClock
        {
            get
            { return _lastClock; }
            set
            {
                if (IsDirect && !_clockLoaded)
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        var _time = GetLastClockTime(IdNumber, Shift);
                        _time = string.IsNullOrEmpty(_time) || _time == "00:00" ? ShiftStart : _time;
                        if (DateTime.TryParse(_time, out DateTime dt))
                        {
                            if (DateTime.Now < dt && Shift != 3)
                            {
                                _time = string.Empty;
                            }
                        }
                        else
                        {
                            _time = string.Empty;
                        }
                        _lastClock = value = _time;
                        _clockLoaded = true;
                        OnPropertyChanged(nameof(LastClock));
                    });
                }
                _lastClock = value;
                OnPropertyChanged(nameof(LastClock));
            }
        }
        private bool _clockLoaded;

        public char ClockTran { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CrewMember()
        { }

        /// <summary>
        /// Overridden Constructor
        /// Load a crewmember object based on an employee ID
        /// </summary>
        /// <param name="idNbr">Crew member ID Number</param>
        public CrewMember(string idNbr)
        {
            var _rows = MasterDataSet.Tables["CREW"].Select($"[EmployeeID] = '{idNbr}'");
            if (_rows.Length > 0)
            {
                IdNumber = idNbr;
                Name = _rows.FirstOrDefault().Field<string>("DisplayName");
                IsDirect = _rows.FirstOrDefault().Field<int>("IsDirect") == 1;
                Shift = _rows.FirstOrDefault().Field<int>("Shift");
                ShiftStart = _rows.FirstOrDefault().Field<string>("ShiftStart");
                ShiftEnd = _rows.FirstOrDefault().Field<string>("ShiftEnd");
                LastClock = string.Empty;
            }
        }

        /// <summary>
        /// Overridden Constructor
        /// Load a crewmember object based on an employee name
        /// WARNING this will not work if the name is spelled incorrectly
        /// Best to just include all crew member ID's in the active directory
        /// </summary>
        /// <param name="firstName">Crew member first name</param>
        /// <param name="lastName">Crew member last name</param>
        public CrewMember(string firstName, string lastName)
        {
            var _rows = MasterDataSet.Tables["CREW"].Select($"[FirstName] = '{firstName}' AND [LastName] = '{lastName}'");
            if (_rows.Length > 0)
            {
                IdNumber = _rows.FirstOrDefault().Field<string>("EmployeeID");
                Name = _rows.FirstOrDefault().Field<string>("DisplayName");
                IsDirect = _rows.FirstOrDefault().Field<int>("IsDirect") == 1;
                Shift = _rows.FirstOrDefault().Field<int>("Shift");
                ShiftStart = _rows.FirstOrDefault().Field<string>("ShiftStart");
                ShiftEnd = _rows.FirstOrDefault().Field<string>("ShiftEnd");
                LastClock = string.Empty;
            }
        }

        #region Data Access

        /// <summary>
        /// Get a table of all BOM's for every SKU on file
        /// </summary>
        /// <param name="sqlCon">Sql Connection to use</param>
        /// <returns>DataTable of bill of materials</returns>
        public static DataTable GetCrewTable(SqlConnection sqlCon)
        {
            using (var _tempTable = new DataTable())
            {
                if (sqlCon != null && sqlCon.State != ConnectionState.Closed && sqlCon.State != ConnectionState.Broken)
                {
                    try
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter($@"USE {sqlCon.Database};
                                                                                SELECT
	                                                                                em.[Emp_No] as 'EmployeeID'
	                                                                                ,em.[First_Name] as 'FirstName'
	                                                                                ,em.[Last_Name] as 'LastName'
	                                                                                ,CONCAT(em.[First_Name], ' ', em.[Last_Name]) as 'DisplayName'
	                                                                                ,CASE WHEN em.[Dept] = '010' THEN 1 ELSE 0 END as 'IsDirect'
	                                                                                ,CASE WHEN em.[Shift] > 3
		                                                                                THEN 4
		                                                                                ELSE em.[Shift]
	                                                                                END as 'Shift'
	                                                                                ,CASE WHEN em.[Shift] = '1'
		                                                                                THEN '07:00'
		                                                                                WHEN em.[Shift] = '2'
		                                                                                THEN '15:00'
		                                                                                WHEN em.[Shift] = '3'
		                                                                                THEN '23:00'
		                                                                                ELSE '08:00'
	                                                                                END as 'ShiftStart'
	                                                                                ,CASE WHEN em.[Shift] = '1'
		                                                                                THEN '15:00'
		                                                                                WHEN em.[Shift] = '2'
		                                                                                THEN '23:00'
		                                                                                WHEN em.[Shift] = '3'
		                                                                                THEN '07:00'
		                                                                                ELSE '17:00'
	                                                                                END as 'ShiftEnd'

                                                                                FROM
	                                                                                [dbo].[EMPLOYEE_MASTER-INIT] em
                                                                                WHERE
	                                                                                [Pay_Status] = 'A'", sqlCon))
                        {
                            adapter.Fill(_tempTable);
                            return _tempTable;
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        throw new Exception(sqlEx.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else
                {
                    throw new Exception("A connection could not be made to pull accurate data, please contact your administrator");
                }
            }
        }

        #endregion

        /// <summary>
        /// Checks to see if a crew ID number is valid
        /// </summary>
        /// <param name="idNbr">Crew member ID</param>
        /// <returns>Crew member existance in the database</returns>
        public static bool IsCrewIDValid(string idNbr)
        {
            return MasterDataSet.Tables["CREW"].Select($"[EmployeeID] = '{idNbr}'").Length > 0;
        }

        /// <summary>
        /// Get a crew member's display name
        /// </summary>
        /// <param name="idNbr">Crew member's ID number</param>
        /// <returns>Crew member's display name</returns>
        public static string GetCrewDisplayName(string idNbr)
        {
            var _rows = MasterDataSet.Tables["CREW"].Select($"[EmployeeID] = '{idNbr}'");
            return _rows.Length > 0 ? _rows.FirstOrDefault().Field<string>("DisplayName") : null;
        }

        /// <summary>
        /// Get the shift start time for a user
        /// </summary>
        /// <param name="idNbr">User Id number</param>
        /// <returns>Shift start time as a string</returns>
        public static string GetShiftStartTime(string idNbr)
        {
            var _rows = MasterDataSet.Tables["CREW"].Select($"[EmployeeID] = '{idNbr}'");
            return _rows.Length > 0 ? _rows.FirstOrDefault().Field<string>("ShiftStart") : string.Empty;
        }

        /// <summary>
        /// Get the shift end time for a user
        /// </summary>
        /// <param name="idNbr">User Id number</param>
        /// <returns>Shift end time as a string</returns>
        public static string GetShiftEndTime(string idNbr)
        {
            var _rows = MasterDataSet.Tables["CREW"].Select($"[EmployeeID] = '{idNbr}'");
            return _rows.Length > 0 ? _rows.FirstOrDefault().Field<string>("ShiftEnd") : string.Empty;
        }

        /// <summary>
        /// Retreives the last labor clocked in time for the current user
        /// </summary>
        /// <param name="idNbr">User ID number</param>
        /// <param name="shift">User shift</param>
        /// <returns>Time as a string</returns>
        public static string GetLastClockTime(string idNbr, int shift)
        {
            try
            {
                var id2 = (DateTime.Now - Convert.ToDateTime("1967/12/31")).Days;
                if (shift == 3 && DateTime.Now.TimeOfDay > new TimeSpan(23,00,00))
                {
                    id2++;
                }
                using (UniSession uSession = UniObjects.OpenSession(WipReceipt.ErpCon[0], WipReceipt.ErpCon[1], WipReceipt.ErpCon[2], WipReceipt.ErpCon[3], WipReceipt.ErpCon[4]))
                {
                    try
                    {
                        var uResponse = string.Empty;
                        using (UniCommand uCmd = uSession.CreateUniCommand())
                        {
                            uCmd.Command = $"LIST LBR.DETAIL WITH @ID = \"{idNbr}*{id2}\" Latest_Time_Out";
                            uCmd.Execute();
                            uResponse = uCmd.Response;
                            if (!string.IsNullOrEmpty(uResponse))
                            {
                                //All the code below is to clean the UniCommand.Response
                                //The response returns identical to a M2k TCL response
                                var uResArray = uResponse.Split('\r');
                                var _sortList = new List<DateTime>();
                                var _listAdd = false;
                                foreach (var s in uResArray)
                                {
                                    if (s.Replace("\n", "").Contains($"records") || s.Replace("\n", "").Contains($"record"))
                                    {
                                        break;
                                    }
                                    if (s.Replace("\n", "~").Contains($"~{idNbr}*{id2}") || _listAdd)
                                    {
                                        _sortList.Add(Convert.ToDateTime(s.Substring(s.Length - 5).Trim()));
                                        _listAdd = true;
                                    }
                                }
                                if (_sortList.Count == 0)
                                {
                                    uResponse = GetShiftStartTime(idNbr);
                                }
                                else
                                {
                                    if (shift == 3 && !_sortList.Any(o => o.TimeOfDay < new TimeSpan(23, 00, 00)))
                                    {
                                        _sortList = _sortList.OrderBy(o => o.TimeOfDay).ToList();
                                    }
                                    else
                                    {
                                        _sortList = _sortList.OrderByDescending(o => o.TimeOfDay).ToList();
                                    }
                                    uResponse = _sortList[0].ToString("HH:mm");
                                }
                            }
                            else
                            {
                                uResponse = GetShiftStartTime(idNbr);
                            }
                        }
                        UniObjects.CloseSession(uSession);
                        return uResponse;
                    }
                    catch (Exception ex)
                    {
                        if (uSession != null)
                        {
                            UniObjects.CloseSession(uSession);
                        }
                        return ex.Message;
                    }
                }
            }
            catch
            {
                throw new Exception("A connection could not be made to pull accurate data, please contact your administrator");
            }
        }

        /// <summary>
        /// Get any crew members full professional name stored in the active directory
        /// </summary>
        /// <param name="domainName">Crew Member Domain Name</param>
        /// <returns>Full crew member name as string</returns>
        public static string GetCrewMemberFullName(string domainName)
        {
            using (PrincipalContext pContext = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal uPrincipal = UserPrincipal.FindByIdentity(pContext, domainName))
                {
                    return uPrincipal != null ? $"{uPrincipal.GivenName} {uPrincipal.Surname}" : domainName;
                }
            }
        }
    }
}
