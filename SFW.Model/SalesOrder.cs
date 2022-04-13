﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

//Created by Michael Marsh 5-4-18

namespace SFW.Model
{
    public class SalesOrder : ModelBase
    {
        #region Properties

        public string SalesNumber { get; set; }
        public string PartNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPart { get; set; }
        public int LineNumber { get; set; }
        public int LineTotalNumber { get; set; }
        public int LineBaseQuantity { get; set; }
        public int LineBalQuantity { get; set; }
        public string LineDesc { get; set; }
        public string LineNotes { get; set; }
        public bool LoadPattern { get; set; }
        public string InternalComments { get; set; }
        public string SpecialInstructions { get; set; }
        public bool IsExpedited { get; set; }
        public string ShipName { get; set; }
        public string[] ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShipCountry { get; set; }
        public bool IsBackOrder { get { return LineBalQuantity < LineBaseQuantity; } }
        public DateTime CommitDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool IsLate { get { return CommitDate > DateTime.Today; } }
        public IList<SalesOrder> LineList { get; set; }
        public string CreditStatus { get; set; }
        public string CreditApprover { get; set; }
        public DateTime CreditDate { get; set; }
        public int CreditLimit { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditShippedBalance { get; set; }
        public decimal CreditAllocatedBalance { get; set; }
        public decimal CurrentCreditLimit { get; set; }
        public decimal OrderBalance { get; set; }
        public bool IsStagged { get; set; }

        #endregion

        /// <summary>
        /// SalesOrder Object Default Constructor
        /// </summary>
        public SalesOrder()
        { }

        /// <summary>
        /// Sales Order Object constructor
        /// Will create a new SalesOrder Object based on a DataRow from any DataTable Object
        /// </summary>
        /// <param name="dRow">DataRow with the item array values for the sales order</param>
        /// <param name="sqlCon">Sql Connection to use</param>
        public SalesOrder(DataRow dRow)
        {
            SalesNumber = dRow.Field<string>("SoNbr");
            PartNumber = dRow.Field<string>("PartNbr");
            CustomerNumber = dRow.Field<string>("CustNbr");
            CustomerName = dRow.Field<string>("CustName");
            CustomerPart = dRow.Field<string>("CustPartNbr");
            LineNumber = dRow.Field<int>("LineNbr");
            LineTotalNumber = dRow.Field<int>("LineCount");
            LineBalQuantity = dRow.Field<int>("BalQty");
            LineBaseQuantity = dRow.Field<int>("BaseQty");
            LineDesc = dRow.Field<string>("Description");
            LoadPattern = dRow.Field<string>("LoadPattern").ToUpper() == "PLASTIC";
            InternalComments = GetNotes(SalesNumber, 'C');
            SpecialInstructions = GetNotes(SalesNumber, 'I');
            IsExpedited = dRow.Field<int>("IsExpedited") > 0;
            ShipName = dRow.Field<string>("ShipName");
            ShipAddress = new string[2];
            ShipAddress[0] = dRow.Field<string>("ShipAddr1");
            ShipAddress[1] = dRow.Field<string>("ShipAddr2");
            ShipCity = dRow.Field<string>("ShipCity");
            ShipState = dRow.Field<string>("ShipState");
            ShipZip = dRow.Field<string>("ShipZip");
            ShipCountry = dRow.Field<string>("ShipCountry");
            CommitDate = dRow.Field<DateTime>("ShipDate");
            RequestDate = dRow.Field<DateTime>("ReqDate");
            DeliveryDate = dRow.Field<DateTime>("DelDate");
            CreditStatus = dRow.Field<string>("CredStatus");
            CreditApprover = dRow.Field<string>("CredApprover");
            CreditDate = dRow.Field<DateTime>("CredDate") == new DateTime(1999, 01, 01) ? DateTime.MinValue : dRow.Field<DateTime>("CredDate");
            CreditLimit = dRow.Field<int>("AR_Limit");
            CreditBalance = dRow.Field<decimal>("AR_Bal");
            CreditShippedBalance = dRow.Field<decimal>("AR_SBal");
            CreditAllocatedBalance = dRow.Field<decimal>("AR_ABal");
            CurrentCreditLimit = dRow.Field<decimal>("AR_Credit");
            OrderBalance = dRow.Field<decimal>("AR_OrdBal");
            IsStagged = dRow.Field<string>("IsStagged") == "T";
        }

        #region Data Access

        /// <summary>
        /// Retrieve a DataTable with all the data relevent to a schedule
        /// </summary>
        /// <param name="sqlCon">Sql Connection to use</param>
        /// <returns>DataTable with the schedule data results</returns>
        public static DataTable GetScheduleData(SqlConnection sqlCon)
        {
            using (var _tempTable = new DataTable())
            {
                if (sqlCon != null && sqlCon.State != ConnectionState.Closed && sqlCon.State != ConnectionState.Broken)
                {
                    try
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(@"SELECT
                                                                                b.[ID]
	                                                                            ,a.[So_Nbr] as 'SoNbr'
	                                                                            ,CAST(SUBSTRING(b.[ID], CHARINDEX('*', b.[ID], 0) + 1, LEN(b.[ID])) as int) as 'LineNbr'
	                                                                            ,(SELECT COUNT(ac.[ID]) FROM [dbo].[SOD-INIT] ac WHERE ac.[ID] LIKE CONCAT(a.[So_Nbr], '*%')) as 'LineCount'
	                                                                            ,b.[Part_Wo_Gl] as 'PartNbr'
                                                                                ,CAST(b.[Ln_Bal_Qty] as int) as 'BalQty'
	                                                                            ,CAST(b.[Ln_Del_Qty] as int) as 'BaseQty'
	                                                                            ,b.[Um_Base] as 'Uom'
	                                                                            ,ISNULL(b.[D_esc], (SELECT aa.[Description] FROM [dbo].[IM-INIT] aa WHERE aa.[Part_Number] = b.[Part_Wo_Gl])) as 'Description'
                                                                                ,a.[Cust_Nbr] as 'CustNbr'
	                                                                            ,(SELECT ab.[Name] FROM [dbo].[CM-INIT] ab WHERE ab.[Cust_Nbr] = a.[Cust_Nbr]) as 'CustName'
                                                                                ,CONCAT((SELECT ab.[Name] FROM [dbo].[CM-INIT] ab WHERE ab.[Cust_Nbr] = a.[Cust_Nbr]),' (', a.[Cust_Nbr], ')') as 'FullCustName'
                                                                                ,b.[Cust_Part_Nbr] as 'CustPartNbr'
	                                                                            ,RTRIM(a.[Credit_Code]) as 'CredStatus'
                                                                                ,a.[Credit_Chk] as 'CredApprover'
	                                                                            ,CAST(ISNULL(a.[Credit_Date], '1999-01-01') as date) as 'CredDate'
	                                                                            ,CASE WHEN a.[Ord_Type] = 'FSE' OR a.[Ord_Type] LIKE '%DE' THEN 'EOP' ELSE a.[Ord_Type] END as 'Type'
	                                                                            ,CAST(CASE WHEN a.[Jump_Reason] IS NULL THEN 0 ELSE 1 END as int) as 'IsExpedited'
	                                                                            ,a.[Ship_To_Name] as 'ShipName'
	                                                                            ,a.[Ship_To_Addr1] as 'ShipAddr1'
	                                                                            ,a.[Ship_To_Addr2] as 'ShipAddr2'
	                                                                            ,a.[Ship_To_City] as 'ShipCity'
	                                                                            ,a.[Ship_To_State] as 'ShipState'
	                                                                            ,a.[Ship_To_Zip] as 'ShipZip'
	                                                                            ,ISNULL(a.[Ship_To_Country], 'US') as 'ShipCountry'
	                                                                            ,CAST(a.[Date_Added] as date) as 'DateAdded'
	                                                                            ,CAST(a.[Delivery_Date] as date) as 'DelDate'
	                                                                            ,CAST(a.[Requested_Date] as date) as 'ReqDate'
	                                                                            ,CAST(a.[Commit_Ship_Date] as date) as 'ShipDate'
                                                                                ,(SELECT ISNULL(aa.[Load_Pattern], '') FROM [dbo].[CM-INIT] aa WHERE aa.[Cust_Nbr] = a.[Cust_Nbr]) AS 'LoadPattern'
                                                                                ,CASE WHEN b.[Make_To_Order] = 'Y' THEN 1
		                                                                            WHEN b.[Make_To_Order] = 'N' OR d.[Wp_Nbr] IS NOT NULL THEN 0
		                                                                            ELSE -1 END as 'MTO'
                                                                                ,CASE WHEN CAST(b.[Ln_Del_Qty] as int) - CAST(b.[Ln_Bal_Qty] as int) = 0 THEN 0 ELSE 1 END AS 'IsBackOrder'
                                                                                ,ISNULL(c.[Ar_Credit_Limit], 0) as 'AR_Limit'
	                                                                            ,c.[Balance] as 'AR_Bal'
	                                                                            ,c.[Ship_Bal] as 'AR_SBal'
	                                                                            ,c.[Alloc_Bal] as 'AR_ABal'
	                                                                            ,ISNULL(c.[Ar_Credit_Limit] - (c.[Balance] + c.[Ship_Bal] + c.[Alloc_Bal]), 0.00) as 'AR_Credit'
	                                                                            ,a.[Order_Bal_Ext_Price] as 'AR_OrdBal'
                                                                                ,CASE WHEN d.[Wp_Nbr] IS NOT NULL THEN 1 ELSE 0 END as 'IsWOLinked'
                                                                                ,CASE WHEN CAST(b.[Ln_Bal_Qty] as int) <= (SELECT ab.[Qty_On_Hand] FROM [dbo].[IPL-INIT] ab WHERE ab.[Part_Nbr] = b.[Part_Wo_Gl]) OR CAST(a.[Commit_Ship_Date] as date) >= CAST(GETDATE() as date)
		                                                                            THEN 1
		                                                                            ELSE 0
	                                                                            END as 'CanShip'
                                                                                ,b.[User_Def_1] as 'IsStagged'
                                                                            FROM
	                                                                            [dbo].[SOH-INIT] a
                                                                            RIGHT JOIN
	                                                                            [dbo].[SOD-INIT] b ON SUBSTRING(b.[ID], 0, CHARINDEX('*', b.[ID], 0)) = a.[So_Nbr]
                                                                            RIGHT JOIN
	                                                                            [dbo].[CM-INIT] c ON c.[Cust_Nbr] = a.[Cust_Nbr]
                                                                            LEFT JOIN
	                                                                            [dbo].[WP-INIT] d ON d.[So_Reference] = CONCAT(b.[ID], '*1')
                                                                            WHERE
	                                                                            a.[Order_Status] IS NULL AND b.[Comp] = 'O' AND (b.[Part_Wo_Gl] IS NOT NULL OR b.[D_esc] IS NOT NULL) AND a.[So_Nbr] IS NOT NULL AND b.[Part_Wo_Gl] != '1010199'
                                                                            ORDER BY
                                                                                a.[Commit_Ship_Date], b.[ID] ASC", sqlCon))
                        {
                            adapter.Fill(_tempTable);
                            //TODO: need to get this to a table for return.
                            return _tempTable.AsEnumerable()
                                .GroupBy(r => r.Field<string>("ID"))
                                .Select(g => g.First())
                                .CopyToDataTable();
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        throw sqlEx;
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

        /// <summary>
        /// Get the sales order internal comments table
        /// </summary>
        /// <param name="sqlCon">Sql Connection to use</param>
        public static DataTable GetNotesTable(SqlConnection sqlCon)
        {
            if (sqlCon != null && sqlCon.State != ConnectionState.Closed && sqlCon.State != ConnectionState.Broken)
            {
                var _conString = sqlCon.Database.Contains("WCCO") ?
                //WCCO Query
                @"SELECT        sohSI.[So_Nbr] AS 'SalesID', sohSI.[Special_Instructions] AS 'Comments', 'I' AS 'Type'
                    FROM            [dbo].[SOH-INIT-Special_Instructions] sohSI RIGHT JOIN
                                             [dbo].[SOH-INIT] soh ON soh.[So_Nbr] = sohSI.[So_Nbr] AND soh.[Order_Status] IS NULL
                    WHERE        sohSI.[Special_Instructions] IS NOT NULL
                    UNION
                    SELECT        sohIC.[So_Nbr] AS 'SalesID', REPLACE(sohIC.[Internal_Comments], '""', ' ') AS 'Comments', 'C' AS 'Type'
                    FROM            [dbo].[SOH-INIT_Internal_Comments] sohIC RIGHT JOIN
                                             [dbo].[SOH-INIT] soh ON soh.[So_Nbr] = sohIC.[So_Nbr] AND soh.[Order_Status] IS NULL
                    WHERE        sohIC.[Internal_Comments] LIKE '%bag%'" :
                 //CSI Query
                 @"SELECT        sohIC.So_Nbr AS SalesID, REPLACE(sohIC.Internal_Comments, '""', ' ') AS Comments, 'C' AS Type
                    FROM            dbo.[SOH-INIT_Internal_Comments] AS sohIC RIGHT OUTER JOIN
                                             dbo.[SOH-INIT] AS soh ON soh.So_Nbr = sohIC.So_Nbr AND soh.Order_Status IS NULL
                    WHERE        (sohIC.Internal_Comments LIKE '%bag%')";
                try
                {
                    using (DataTable dt = new DataTable())
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter($@"USE {sqlCon.Database}; {_conString}", sqlCon))
                        {
                            adapter.Fill(dt);
                        }
                        return dt;
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw sqlEx;
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

        #endregion

        /// <summary>
        /// Get the sales order internal comments
        /// </summary>
        /// <param name="soNumber">Sales Order to get the internal comments from</param>
        /// <param name="type">Type of note I = Special Instructions, C = Internal Comments</param>
        /// <returns>Internal comments in a string</returns>
        public static string GetNotes(string soNumber, char type)
        {
            var _note = string.Empty;
            var _rows = MasterDataSet.Tables["SoNotes"].Select($"[SalesID] = '{soNumber}' AND [Type] = '{type}'");
            if (_rows.Length > 0)
            {
                foreach (var _row in _rows)
                {
                    _note += $"{_row.Field<string>("Comments")}\n";
                }
                return string.IsNullOrEmpty(_note) ? null : _note?.Trim('\n');
            }
            return null;
        }

        /// <summary>
        /// Get a list of the differnt types of sales orders
        /// </summary>
        /// <returns>list of sales order types as IList<string></returns>
        public static IList<string> GetOrderTypeList()
        {
            var _rtnList = new List<string>();
            foreach (DataRow _row in MasterDataSet.Tables["SalesMaster"].DefaultView.ToTable(true, "Type").Rows)
            {
                _rtnList.Add(_row.Field<string>("Type"));
            }
            return _rtnList;
        }

        /// <summary>
        /// Get a list of all the line items on a sales order
        /// </summary>
        /// <param name="soNbr">Sales order number to search</param>
        /// <param name="lineNbr">Optional: Any line number not to include in the list</param>
        /// <returns>list of sales order line items as IList<string></returns>
        public static IList<SalesOrder> GetLineList(string soNbr)
        {
            var _rtnList = new List<SalesOrder>();
            var _rows = MasterDataSet.Tables["SalesMaster"].Select($"[SoNbr] = '{soNbr}'");
            if (_rows.Length > 0)
            {
                foreach (var _row in _rows)
                {
                    _rtnList.Add(new SalesOrder
                                    {
                                        LineBalQuantity = _row.Field<int>("BalQty")
                                        ,LineDesc = _row.Field<string>("Description")
                                        ,LineNumber = _row.Field<int>("LineNbr")
                                        ,PartNumber = _row.Field<string>("PartNbr")
                                        ,LineBaseQuantity = _row.Field<int>("BaseQty")
                                        ,LineNotes = _row.Field<string>("Uom")
                                        ,IsStagged = _row.Field<int>("IsBackOrder") == 1
                                    });
                }
            }
            return (from c in _rtnList
                   orderby c.LineNumber
                   select c).ToList();
            /*if (!string.IsNullOrEmpty(soNbr))
            {
                var _condString = string.Empty;
                foreach (int i in lineNbr)
                {
                    _condString += i != 0 ? $" AND a.[ID] != CONCAT('{soNbr}', '*', '{i}')" : "";
                }
                var _rtnList = new List<SalesOrder>();
                if (sqlCon != null && sqlCon.State != ConnectionState.Closed && sqlCon.State != ConnectionState.Broken)
                {
                    try
                    {
                        using (SqlCommand _cmd = new SqlCommand(@"SELECT
	                                                                SUBSTRING(a.[ID], CHARINDEX('*', a.[ID],0) + 1, LEN(a.[ID])) as 'LineNbr'
	                                                                ,a.[Part_Wo_Gl] as 'PartNbr'
	                                                                ,(SELECT aa.[Description] FROM [dbo].[IM-INIT] aa WHERE aa.[Part_Number] = a.[Part_Wo_Gl]) as 'PartDesc'
	                                                                ,a.[Ln_Bal_Qty] as 'BalQty'
	                                                                ,a.[Ln_Del_Qty] as 'BaseQty'
	                                                                ,a.[Um_Base] as 'Uom'
                                                                    ,CASE WHEN CAST(ISNULL((SELECT SUM(aa.[Oh_Qty_By_Loc]) FROM [dbo].[IPL-INIT_Location_Data] aa WHERE aa.[ID1] = a.[Part_Wo_Gl] 
		                                                                AND aa.[Loc_Pick_Avail_Flag] = 'Y'), 0) as int) >= CAST(a.[Ln_Bal_Qty] as int)
		                                                                THEN 0
		                                                                ELSE 1
	                                                                END as 'IsBackOrder'
                                                                FROM [dbo].[SOD-INIT] a
                                                                WHERE a.[ID] LIKE CONCAT(@p1, '*%')", sqlCon))
                        {
                            if (!string.IsNullOrEmpty(_condString))
                            {
                                _cmd.CommandText += _condString;
                            }
                            _cmd.CommandText += " ORDER BY a.[ID] ASC";
                            _cmd.Parameters.AddWithValue("p1", soNbr);
                            using (SqlDataReader reader = _cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _rtnList.Add(new SalesOrder
                                    {
                                        LineBalQuantity = reader.SafeGetInt32("BalQty")
                                        ,LineDesc = reader.SafeGetString("PartDesc")
                                        ,LineNumber = reader.SafeGetInt32("LineNbr")
                                        ,PartNumber = reader.SafeGetString("PartNbr")
                                        ,LineBaseQuantity = reader.SafeGetInt32("BaseQty")
                                        ,LineNotes = reader.SafeGetString("Uom")
                                        ,IsStagged = reader.SafeGetInt32("IsBackOrder") == 1
                                    });
                                }
                            }
                        }
                        return _rtnList;
                    }
                    catch (SqlException sqlEx)
                    {
                        throw sqlEx;
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
            return null;*/
        }
    }
}
