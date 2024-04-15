using System;

namespace VMMS
{
    /// <summary>  
    /// 取号状态数据对象
    /// 创建人：dinid
    /// 创建日期：20200416
    /// 版本：1.0
    /// 修改日期：
    /// </summary>
    public class ObjBillStatus
    {
        public int StatusID { get; set; }
        public Guid StatusGUID { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
    }
}