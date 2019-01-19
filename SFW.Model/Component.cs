﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SFW.Model
{
    public class Component
    {
        #region Properties

        public string CompNumber { get; set; }
        public string CompDescription { get; set; }
        public int CurrentOnHand { get; set; }
        public int RequiredQty { get; set; }
        public double AssemblyQty { get; set; }
        public int IssuedQty { get; set; }
        public string CompMasterPrint { get; set; }
        public string CompUom { get; set; }
        public List<Lot> LotList { get; set; }
        public List<Lot> NonLotList { get; set; }
        public List<Lot> DedicatedLotList { get; set; }
        public string InventoryType { get; set; }
        public BindingList<CompWipInfo> WipInfo { get; set; }
        public bool IsLotTrace { get; set; }
        public static bool WipInfoUpdating { get; set; }

        #endregion

        /// <summary>
        /// Component Default Constructor 
        /// </summary>
        public Component()
        { }

        /// <summary>
        /// Retrieve a list of components for a work order
        /// </summary>
        /// <param name="woNbr">Work Order Number</param>
        /// <param name="balQty">Balance quantity left on the work order</param>
        /// <param name="sqlCon">Sql Connection to use</param>
        /// <returns>List of Component objects</returns>
        public static List<Component> GetComponentList(string woNbr, int balQty, SqlConnection sqlCon)
        {
            var _tempList = new List<Component>();
            WipInfoUpdating = false;
            if (sqlCon != null || sqlCon.State != ConnectionState.Closed || sqlCon.State != ConnectionState.Broken)
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand($@"USE {sqlCon.Database}; SELECT
	                                                            SUBSTRING(a.[ID], CHARINDEX('*', a.[ID], 0) + 1, LEN(a.[ID])) as 'Component', a.[Qty_Per_Assy] as 'Qty Per', a.[Qty_Reqd] as 'Req Qty',
	                                                            b.[Qty_On_Hand] as 'On Hand',
	                                                            c.[Description], c.[Drawing_Nbrs], c.[Um], c.[Inventory_Type], c.[Lot_Trace]
                                                            FROM
	                                                            [dbo].[PL-INIT] a
                                                            RIGHT JOIN
	                                                            [dbo].[IPL-INIT] b ON b.[Part_Nbr] = SUBSTRING(a.[ID], CHARINDEX('*', a.[ID], 0) + 1, LEN(a.[ID]))
                                                            RIGHT JOIN
	                                                            [dbo].[IM-INIT] c ON c.[Part_Number] = SUBSTRING(a.[ID], CHARINDEX('*', a.[ID], 0) + 1, LEN(a.[ID]))
                                                            WHERE
	                                                            a.[ID] LIKE CONCAT(@p1, '%');", sqlCon))
                    {
                        cmd.Parameters.AddWithValue("p1", woNbr);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    _tempList.Add(new Component
                                    {
                                        CompNumber = reader.SafeGetString("Component"),
                                        AssemblyQty = reader.SafeGetDouble("Qty Per"),
                                        RequiredQty = reader.SafeGetInt32("Req Qty"),
                                        CurrentOnHand = reader.SafeGetInt32("On Hand"),
                                        CompDescription = reader.SafeGetString("Description"),
                                        IssuedQty = Convert.ToInt32(Math.Round(reader.SafeGetDouble("Qty Per") * balQty, 0, MidpointRounding.AwayFromZero)),
                                        CompMasterPrint = reader.SafeGetString("Drawing_Nbrs"),
                                        CompUom = reader.SafeGetString("Um"),
                                        InventoryType = reader.SafeGetString("Inventory_Type"),
                                        IsLotTrace = reader.SafeGetString("Lot_Trace") == "T",
                                        LotList = reader.IsDBNull(0)
                                            ? new List<Lot>()
                                            : Lot.GetOnHandLotList(reader.SafeGetString("Component"), sqlCon),
                                        DedicatedLotList = reader.IsDBNull(0)
                                            ? new List<Lot>()
                                            : Lot.GetDedicatedLotList(reader.SafeGetString("Component"), woNbr, sqlCon),
                                        WipInfo = new BindingList<CompWipInfo>()
                                    });
                                    _tempList[_tempList.Count - 1].WipInfo.AddNew();
                                    _tempList[_tempList.Count - 1].WipInfo.ListChanged += WipInfo_ListChanged;
                                    _tempList[_tempList.Count - 1].NonLotList = _tempList[_tempList.Count - 1].LotList.Count == 0 && !reader.IsDBNull(0)
                                        ? Lot.GetOnHandNonLotList(reader.SafeGetString("Component"), sqlCon)
                                        : new List<Lot>();
                                }
                            }
                        }
                    }
                    return _tempList;
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

        /// <summary>
        /// Happens when an item is added or changed in the WipInfo Binding List property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void WipInfo_ListChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType == ListChangedType.ItemChanged && !WipInfoUpdating)
            {
                WipInfoUpdating = true;
                var _tempList = (BindingList<CompWipInfo>)sender;
                if (e.PropertyDescriptor.DisplayName == "LotNbr" && _tempList.Count > 1)
                {
                    var _tempQty = _tempList.Sum(o => o.LotQty);
                    foreach (CompWipInfo cWI in _tempList)
                    {
                        cWI.LotQty = _tempQty / _tempList.Count;
                    }
                }
                if (!string.IsNullOrEmpty(_tempList[e.NewIndex].LotNbr) && _tempList[e.NewIndex].LotQty > 0)
                {
                    ((BindingList<CompWipInfo>)sender).AddNew();
                }
                WipInfoUpdating = false;
            }
        }
    }

    public static class CompExtensions
    {
        public static void UpdateWipInfo(this Component comp, int wipQty)
        {
            var _tempD = Convert.ToDouble(wipQty);
            if (comp.WipInfo.Count == 1)
            {
                comp.WipInfo[0].LotQty = Convert.ToInt32(Math.Round(comp.AssemblyQty * _tempD, 0));
            }
            else
            {
                var _oddHandle = 0;
                foreach(var item in comp.WipInfo)
                {
                    item.LotQty = Convert.ToInt32(Math.Round(comp.AssemblyQty * _tempD, 0));
                    if (_oddHandle > 0 && item.LotQty + _oddHandle > wipQty)
                    {
                        item.LotQty = _oddHandle - wipQty;
                    }
                    _oddHandle += Convert.ToInt32(item.LotQty);
                }
            }
        }
    }
}
