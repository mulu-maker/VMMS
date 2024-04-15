using System;

namespace VMMS
{
    public class ObjModel
    {
        /// <summary>  
        /// 车型对象类
        /// </summary>
        public int ModelID { get; set; }
        public Guid ModelGUID { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public bool DeleteMark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
        //明细属性
        public Guid ProductGUID { get; set; }
    }
}