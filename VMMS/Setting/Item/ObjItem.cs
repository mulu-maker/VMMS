using System;
using System.Collections.Generic;

namespace VMMS
{
    public class ObjItem
    {
        /// <summary>  
        /// 维修项目数据对象类
        /// </summary>
        public int ItemID { get; set; }
        public Guid ItemGUID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Remark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}