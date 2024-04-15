using System;

namespace VMMS
{
    public class ObjProductType
    {
        /// <summary>  
        /// 备件类别对象类
        /// </summary>
        public int TypeID { get; set; }
        public Guid TypeGUID { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}