using System;
using System.Collections.Generic;

namespace VMMS
{
    public class ObjContent
    {
        /// <summary>  
        /// 维修内容数据对象类
        /// </summary>
        public int ContentID { get; set; }
        public Guid ContentGUID { get; set; }
        public string ContentCode { get; set; }
        public string ContentName { get; set; }
        public string Remark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}