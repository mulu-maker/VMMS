using System;

namespace VMMS
{
    public class ObjUnit
    {
        /// <summary>  
        /// 计量单位对象类
        /// </summary>
        public int UnitID { get; set; }
        public Guid UnitGUID { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}