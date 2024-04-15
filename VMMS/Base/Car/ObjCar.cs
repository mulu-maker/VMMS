using System;

namespace VMMS
{
    /// <summary>
    /// 车辆对象
    /// </summary>
    public class ObjCar
    {
        public int CarID { get; set; }
        public Guid CarGUID { get; set; }
        public string CarCode { get; set; }
        //public string CarName { get; set; }
        public string VIN { get; set; }
        public string LicensePlate { get; set; }//车牌号
        public int TotalMileage { get; set; }//总里程
        public Guid ModelGUID { get; set; }//车型
        public string ModelName { get; set; }//车型名称
        public string EngineModel { get; set; }//发动机型号
        public string EngineCapacity { get; set; }
        public string CarColor { get; set; }//车身颜色
        public DateTime ManufactureDate { get; set; }//生产日期
        public Guid CustomerGUID { get; set; }//车主
        public string CustomerName { get; set; }//车主姓名
        public string MobilePhone { get; set; }//手机号
        public string Remark { get; set; }//备注
        public bool DeleteMark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
        public DateTime NextMaintenanceDate { get; set; }//下次保养日期
        public string InsuranceCompany { get; set; }//保险公司
        public DateTime InsuranceDate { get; set; }//保险到期日期
        //查询条件
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        //报表属性
        public Guid BillGUID { get; set; }

    }
}
