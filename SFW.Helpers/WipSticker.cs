﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SFW.Helpers
{
    public class WipSticker
    {
        #region Properties

        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string ProductID { get; set; }
        public string ProductDescription { get; set; }
        public string ProductUnitOfMeasure { get; set; }
        public string ProductLot { get; set; }
        public string[] FabricLot { get; set; }
        public string[] CompoundLot { get; set; }
        public int LotQuantity { get; set; }
        public int LotWeight { get; set; }
        public string SalesOrderID { get; set; }
        public string PurchaseOrderID { get; set; }
        public string WorkOrderID { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public WipSticker()
        { }

        /// <summary>
        /// Overridden Constructor
        /// </summary>
        public WipSticker(string _cName, string _cID, string _pID, string _pDesc, string _pUom, string _pLot, string[] _fLot, string[] _cLot, int _lQty, int _lWgt, string _soID, string _purID, string _woID)
        {
            CustomerName = _cName;
            CustomerId = _cID;
            ProductID = _pID;
            ProductDescription = _pDesc;
            ProductUnitOfMeasure = _pUom;
            ProductLot = _pLot;
            FabricLot = _fLot;
            CompoundLot = _cLot;
            LotQuantity = _lQty;
            LotWeight = _lWgt;
            SalesOrderID = _soID;
            PurchaseOrderID = _purID;
            WorkOrderID = _woID;
        }

        /// <summary>
        /// Build a ZPL string for the WipSticker object
        /// </summary>
        /// <returns>ZPL string</returns>
        public string ToZPLString()
        {
            return $@"CT~~CD,~CC^~CT~
^XA
^MMT
^PW812
^LL0812
^LS0
^FO14,0^GFA,6360,6360,60,,::::::::::::::::U02jS03,U03jS018,Q04I038jR03C,O0KF03CjR0FE,N07LF3EgI03FEiK01FF,M03OFgI0IFiK07FF,M0PF8gG01IF8hQ07JFN0IF8,L03PFCgG01IFChQ07JFM03IF8,L0QFEgG03IFChQ07JFM03IF8,K03RFgG03IFChQ03JFM03IF8,K07LFC01IF8V01C003IFCgX038R03IFM03C7F8,K0LF8J0FFCV03C003IFCgX078R03IFO0FF8,J03KFCK03FEV07C001IFCgX0F8R03IFO0FF8,J07KFM07EV0FC001IF8gW01F8R03IFN01FF8,J0KFCM01FU03FCI0IFgX07F8R03IFN03FF8,I01KF8N0F8T07FCI03FEgX0FF8R03IFN03FF,I03JFEO038T0FFChI01FF8R03IFN0IF,I07JFCO018S01FFChI07FF8R03IFJ039C1IF,I07JF8P08S07FFChI0IF8R03IFI01F3JFE,I0KFgK0IFChH01IF8R03IFI07E7JFE,001JFEJ07FFEV01IFChH03IF8R03IFI0FCLF,001JFCI01JFCO03FEI07IFCT07FCL01FF8O07F8I0JF8J01FFCJ03IF001F9E7JF8,003JF8I07JFEI03IFC0IF800KFE0JF807IF81IFK01JFI07IF81IF001LFI0JFJ03IFJ01C3JFC,007JF8001LF8003IFC1IFC00KFE0JF807IF87IF8J07JFC007IF87IF801LF003JFCI03IFJ0180JFC,007JFI03FFC3FFC003IFC7IFE00KFE0JF807IF8JFCJ0KFE007IF8JFC01LF007KFI03IFJ01807IFE,00JFEI07FF81FFEI07FFC7JFI0IFC001IF800IF9JFEI01FF8IFI0IF9JFE001IF8001LF8003IFJ01803JF,00JFEI0IF00IFI03FFCKFI0IFCI0IF8007FF9JFEI03FF03FF8007FF9JFE001IF8003LFC003IFJ01801JF8,01JFC001IF007FF8003FFDKF800IFC001IF8007FFBKFI07FE03FFC007FFBKF001IF8007LFC003IFJ01C00JFC,01JFC003FFE007FFC003IFC3IF800IFCI0IF8007IF87IFI0FFE01FFC007IF87IF001IF800FF87IFE003IFJ01C00JFE,01JF8003FFE007FFC003IF81IF800IFC001IF8007IF03IF001FFE01FFE007IF03IF001IF800FC00IFE003IFJ01C007JF8,03JF8007FFE007FFE003IF00IFC00IFCI0IF8007FFE01IF001FFC01FFE007FFE01IF001IF801FI07IF003IFJ01C003JFC,03JF8007FFE003IF003IF00IFC00IFCI0IF8007FFE01IF803FFC01IF007FFE01IF801IF800EI03IF003IFK08001KF,03JFI0IFE003IF003IF007FFC00IFCI0IF8007FFC01IF803FFC01IF007FFC01IF801IF800CI03IF003IFO0KF8,07JFI0IFE003IF003FFE007FFC00IFCI0IF8007FFC00IF807FFC01IF807FFC00IF801IF8L01IF003IFO07JF9,07JFI0IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF807FFC00IF807FFC00IF801IF8L01IF803IFO01JFD8,07JF001IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF807FFC00IF807FFC00IF801IF8L01IF803IFP0JFCE,07IFE001IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF807FFC00IF807FFC00IF801IF8L01IF803IFP03IFCF,0JFE001IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF80IFC00IF807FFC00IF801IF8L01IF803IFP01IFCF,0JFE001IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFC00IF807FFC00IF801IF8J03F1IF803IFP01IFCF,0JFE001IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFC00IF807FFC00IF801IF8I03LF803IFQ0IFC78,0JFE003IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFC00IF807FFC00IF801IF8001MF803IFQ0IF878,0JFE003IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80OF807FFC00IF801IF8003MF803IFQ0IF878,0JFE003IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80OF807FFC00IF801IF8007FFE3IF803IFQ07FF078,0JFE003IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80OF807FFC00IF801IF800IF81IF803IFQ03FE078,0JFE003IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80OF807FFC00IF801IF801IF01IF803IFQ01FF078,0JFE003IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFCM07FFC00IF801IF803IF01IF803IFR0FF07C,0JFE001IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFCM07FFC00IF801IF803FFE01IF803IFR07F07C,0JFE001IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFCM07FFC00IF801IF803FFE01IF803IFQ013F87C,0JFE001IFC003IFC03FFE007FFC00IFCI0IF8007FFC00IF80IFCM07FFC00IF801IF807FFE01IF803IFQ039F87C,07IFE001IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF807FFEM07FFC00IF801IF807FFE01IF803IFQ07CF83C,07IFE001IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF807FFEM07FFC00IF801IF807FFE01IF803IFQ0F8F81C,07JF001IFC003IF803FFE007FFC00IFCI0IF8007FFC00IF807FFEM07FFC00IF801IF807FFE01IF803IFP01E1F004,07JFI0IFE003IF003FFE007FFC00IFCI0IF8007FFC00IF807IFM07FFC00IF801IF807FFE01IF803IFP03C3E,07JFI0IFE003IF003FFE007FFC00IFCI0IF8007FFC00IF803IFM07FFC00IF801IF807FFE01IF803IFP0787C,07JFI07FFE003IF003FFE007FFC00IFCI0IF8007FFC00IF803IF8I06007FFC00IF801IF807FFE01IF803IFP0F078,03JF8007FFE007FFE003FFE007FFC00IFCI0IF8007FFC00IF801IFCI0F007FFC01IF801IF807IF01IF803IFO01E0F,03JF8003FFE007FFE003FFE007FFC007FFC001IF8007FFC00IF801JF003F007FFC00IF801IF807IF81IF803IFO03E1E,03JF8003FFE007FFC003FFE007FFC007FFCI0IF8007FFC00IF800KFBFE007FFC01IF801IF803IFC3IF803IFQ03E,01JFC001IF007FF8003FFE007FFC007FFC001IF8007FFC00IF800MFE007FFC01IF800IF803NF803IFQ07C,01JFCI0IF00IF8003FFE007FFC007FFEI0IF8007FFC01IF8007LFC007FFC01IF800IFC03NF803IFQ07C,00JFEI07FF81IFI07IF007FFC003IFE01IF800IFE01IF8003LF800IFC01IF8007IFC1JFEIF803IF,00JFEI03FFC3FFE003JFE07IFE03IFE0KF87JFC1JFC01LF007JFC1JFC07IFC0JFCJF83JF8,007JFI01LFC003JFE07IFE01IFE0KF87JFC1JFC007JFC007JFC1JFC03IFC07IF8JF83JF8,007JF8I0LFI03JFE07IFE007FFE0KF87JFC1JFC001JFI07JFC1JFC01IFC03FFE0JF83JF83UF8,003JF8I03JFCP07IFE001FF00KF8M0JFCI07FFCI05J040JFC003FE001FF80JF83JF83UF8,003JFCJ0JF,001JFEK0FF,I0KFQ08,I0KF8O018,I07JFCO038,I03KFO078,I01KF8M01F,J0KFEM07E,J07KF8K01FC,J03LFK0FF8,J01MFI07FF,K07QFE,K03QF8,L0QF,L03OFC,M0OF,M01MF8,N03KF8,P0FFE,,::::^FS
^FT560,51^A0N,28,28^FB240,1,0,R^FH\^FD{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}^FS
^FT14,135^A0N,34,33^FH\^FDContiTech USA, Inc.^FS
^FT500,135^A0N,34,33^FH\^FB300,1,0,R^FD(817) 467-1200^FS
^FT14,177^A0N,34,33^FH\^FD1120 Commercial Blvd North, Arlington, Tx 76001^FS
^FO0,192^GB812,3,3^FS
^FT14,227^A0N,28,28^FH\^FDCustomer:^FS
^FT135,227^A0N,28,28^FH\^FD{CustomerId}^FS
^FT240,227^A0N,28,28^FB560,1,0,R^FH\^FD{CustomerName}^FS
^FT14,269^A0N,28,28^FH\^FDCustomer Order:^FS
^FT205,269^A0N,28,28^FH\^FD{PurchaseOrderID}^FS
^FO0,284^GB812,3,3^FS
^FT14,326^A0N,28,28^FH\^FDSales Order:^FS
^FT160,326^A0N,28,28^FH\^FD{SalesOrderID}^FS
^FT525,326^A0N,28,28^FB160,1,0,R^FH\^FDWork Order:^FS
^FT600,326^A0N,28,28^FB200,1,0,R^FH\^FD{WorkOrderID}^FS
^FO0,341^GB812,3,3^FS
^FT14,376^A0N,28,28^FH\^FDPart Number:^FS
^FT175,376^A0N,28,28^FH\^FD{ProductID}^FS
^FT14,418^A0N,28,28^FH\^FDDescription:^FS
^FT160,418^A0N,28,28^FH\^FD{ProductDescription}^FS
^FT14,460^A0N,28,28^FH\^FDLot:^FS
^FT65,460^A0N,28,28^FH\^FD{ProductLot}^FS
^FT555,460^A0N,28,28^FB130,1,0,R^FH\^FDQuantity:^FS
^FT600,460^A0N,28,28^FB200,1,0,R^FH\^FD{LotQuantity} {ProductUnitOfMeasure}^FS
^FO0,475^GB812,3,3^FS
^FT14,510^A0N,28,28^FH\^FDFabric Lot(s):^FS
^FT14,552^A0N,28,28^FH\^FD{FabricLot[0]}^FS
^FT14,594^A0N,28,28^FH\^FD{FabricLot[1]}^FS
^FT300,552^A0N,28,28^FH\^FD{FabricLot[2]}^FS
^FT300,594^A0N,28,28^FH\^FD{FabricLot[3]}^FS
^FO0,609^GB812,3,3^FS
^FT14,644^A0N,28,28^FH\^FDCompound Lot(s):^FS
^FT14,686^A0N,28,28^FH\^FD{CompoundLot[0]}^FS
^FT14,728^A0N,28,28^FH\^FD{CompoundLot[1]}^FS
^FT300,686^A0N,28,28^FH\^FD{CompoundLot[2]}^FS
^FT300,728^A0N,28,28^FH\^FD{CompoundLot[3]}^FS
^FO0,743^GB812,3,3^FS
^FT14,778^A0N,28,28^FH\^FDWeight:^FS
^FT110,778^A0N,28,28^FH\^FD{LotWeight} LB^FS
^PQ1,0,1,Y^XZ";
        }

        /// <summary>
        /// Print Label from an installed zebra printer
        /// </summary>
        /// <param name="_wipSticker"></param>
        /// <param name="_copies"></param>
        /// <returns>Completion as a read only dictionary (key: bool success or failure; value: error message)</returns>
        public IReadOnlyDictionary<bool, string> Print(int _copies)
        {
            var _result = new Dictionary<bool, string>();
            try
            {
                var _prtName = string.Empty;
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    if (printer.ToUpper().Contains("-ARZ") || printer.ToUpper().Contains("-WAZ"))
                    {
                        _prtName = printer;
                        break;
                    }
                }
                if (_prtName != string.Empty) 
                {
                    var _rtnValue = RawPrinter.SendStringToPrinter(_prtName, ToZPLString(), _copies);
                    _result.Add(_rtnValue.FirstOrDefault().Key, _rtnValue.FirstOrDefault().Value);
                    return _result;
                }
                else
                {
                    _result.Add(false, "No printer found.");
                    return _result;
                }
            }
            catch(Exception ex)
            {
                _result.Add(false, ex.Message);
                return _result;
            }
        }
    }
}
