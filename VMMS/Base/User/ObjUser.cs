using System;

namespace VMMS
{
    /// <summary>
    /// 员工对象
    /// </summary>
    public class ObjUser
    {
        public int UserID { get; set; }
        public Guid UserGUID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public Guid CompanyGUID { get; set; }
        public Guid WorkplaceGUID { get; set; }
        public string Office { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public bool DeleteMark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
        //显示属性
        public string CompanyName { get; set; }
        public string WorkplaceName { get; set; }
    }
}
