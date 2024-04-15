using System;
using System.Collections.Generic;

namespace VMMS
{
    public class ObjProduct
    {
        /// <summary>  
        /// 备件对象类
        /// </summary>
        public int ProductID { get; set; }
        public Guid ProductGUID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string MnemonicCode { get; set; }//助记码
        public string Barcode { get; set; }
        public Guid UnitGUID { get; set; }
        public Guid TypeGUID { get; set; }
        public int PropertyID { get; set; }
        public decimal Price { get; set; }
        public string Remark { get; set; }
        public bool DeleteMark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
        public IList<ObjModel> ListModel { get; set; }///Model明细数据集合属性
        //明细数据属性
        public Guid DetailGUID { get; set; }//明细数据行GUID
        public int DetailSN { get; set; }//明细序号
        public Guid BillGUID { get; set; }//出入库单据Guid
        public Guid DebitLocationGUID { get; set; } //入库库位
        public decimal DebitNumber { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal DebitCost { get; set; }//入库单位成本
        public Guid CreditLocationGUID { get; set; } //出库库位
        public decimal CreditNumber { get; set; }//出库数量
        public decimal CreditAmount { get; set; }//出库金额
        public decimal CreditCost { get; set; }//出库单位成本
        public decimal SalesNumber { get; set; }//销售数量
        public decimal SalesPrice { get; set; }//销售单价
        public decimal SalesAmount { get; set; }//出库单位成本
        public decimal DiffAmount { get; set; }//折扣金额
        public decimal ChargeAmount { get; set; }//收款金额
        public decimal ProfitAmount { get; set; }//利润金额
        //余额表属性
        public DateTime CurrentDate { get; set; }
        public int BalanceYear { get; set; }
        public int BalanceMonth { get; set; }
        public decimal BeginNumber { get; set; }//期初余额
        public decimal BeginAmount { get; set; }//期初余额
        public decimal EndNumber { get; set; }//期末余额
        public decimal EndAmount { get; set; }//期末余额
        //public decimal Cost { get; set; }//期末结转单位成本
        public bool IsBalance { get; set; }//上期期末余额、转为本期期初余额
        //库存表属性
        public int InventoryID { get; set; } //库存表ID
        public Guid LocationGUID { get; set; } //库位
        public decimal InventoryCost { get; set; }//库存成本
        public decimal InventoryNumber { get; set; }//库存量
        public decimal InventoryAmount { get; set; }//库存额
        public int StatusID { get; set; }
        //查询条件
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public Guid CustomerGUID { get; set; }
        public Guid ModelGUID { get; set; }
        public int TypeID { get; set; }//单据类别
        //显示属性
        public string BillCode { get; set; }
        public DateTime BillDate { get; set; }
        public Guid UserGUID { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LicensePlate { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}