using System;
using System.Collections.Generic;

namespace VMMS
{
    public class ObjInsuranceCompany
    {
        /// <summary>  
        /// 维修项目数据对象类
        /// </summary>
        public int InsuranceCompanyID { get; set; }
        public Guid InsuranceCompanyGUID { get; set; }
        public string InsuranceCompanyCode { get; set; }
        public string InsuranceCompanyName { get; set; }
        public string Remark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}