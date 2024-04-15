using System;
using System.Collections.Generic;

namespace VMMS
{
    /// <summary>
    /// 出入库单据对象
    /// </summary>
    public class ObjBill
    {
        public ObjBill()
        {
            ListDetail = new List<ObjProduct>();//初始化明细数据对象
        }
        public int BillID { get; set; }
        public Guid BillGUID { get; set; }
        public string BillCode { get; set; }
        public DateTime BillDate { get; set; }
        public Guid SourceGUID { get; set; }//源单据编号
        public string SourceCode { get; set; }//源单据GUID、主要用于自动冲红时记录被冲红单据号
        public Guid UserGUID { get; set; }
        public Guid ItemGUID { get; set; }//维修项目
        public string ItemName { get; set; }//维修项目名称
        public string ItemContent { get; set; }//维修内容
        public Guid CustomerGUID { get; set; }//供应商
        public Guid CarGUID { get; set; }
        public string CarCode { get; set; }
        public string VIN { get; set; }
        public string LicensePlate { get; set; }
        public Guid ModelGUID { get; set; }
        public string ModelName { get; set; }
        public int TotalMileage { get; set; }//上次总里程
        public int CarMileage { get; set; }//本次总里程
        public int Mileage { get; set; }//本次行驶里程
        public DateTime NextMaintenanceDate { get; set; }//下次保养日期
        public string InsuranceCompany { get; set; }//保险公司
        public DateTime InsuranceDate { get; set; }//保险到期日期
        public string CustomerName { get; set; }
        public string SendName { get; set; }
        public string MobilePhone { get; set; }
        public decimal TotalDebitNumber { get; set; }//借方总数
        public decimal TotalDebitAmount { get; set; }//借方总额
        public decimal TotalCreditNumber { get; set; }//贷方总数
        public decimal TotalCreditAmount { get; set; }//贷方总额
        public decimal TotalSalesNumber { get; set; }//销售总数
        public decimal TotalSalesAmount { get; set; }//销售总额
        public decimal TotalChargeAmount { get; set; }//收款金额
        public decimal TotalDiffAmount { get; set; }//折扣金额
        public int TypeID { get; set; }//单据类别
        public int StatusID { get; set; }//流程
        public string Remark { get; set; }//备注
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
        public DateTime CompleteTime { get; set; }
        public bool IsCharge { get; set; }
        public Guid ChargeGUID { get; set; }
        public DateTime ChargeTime { get; set; }
        //明细数据
        public IList<ObjProduct> ListDetail { get; set; }
        //维修记录
        public Guid ProductGUID { get; set; }
        public string ProductName { get; set; }
        public decimal CreditNumber { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal CreditCost { get; set; }//出库单位成本
        public decimal CreditCostAmount { get; set; }//出库成本金额
        public decimal SalesPrice { get; set; }//销售单价
        public decimal SalesAmount { get; set; }//销售额
        public decimal Price { get; set; }//售价
        //查询条件
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
