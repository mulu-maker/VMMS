using System;
using System.Collections.Generic;

namespace VMMS
{
    public class ObjRemark
    {
        /// <summary>  
        /// 备注内容数据对象类
        /// </summary>
        public int RemarkID { get; set; }
        public Guid RemarkGUID { get; set; }
        public string RemarkCode { get; set; }
        public string RemarkName { get; set; }
        public string Remark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}