using System;
using System.Collections.Generic;

namespace VMMS
{
    public class ObjLocation
    {
        /// <summary>  
        /// 库位数据对象类
        /// </summary>
        public int LocationID { get; set; }
        public Guid LocationGUID { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string Remark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}