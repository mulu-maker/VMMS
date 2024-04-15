using System;
using System.Collections.Generic;

namespace VMMS
{
    /// <summary>
    /// 参数数据操作类
    /// </summary>
    public class DalDataConfig
    {    
        //程序名
        public static string SoftName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
        //版本
        public const string SoftVerion = "2.0.0";
        public const double DbVerion = 2.0425;
        //参数
        public const int CustomerCodeWidth = 6;//供应商编号宽度
        public const int CarCodeWidth = 7;//备件编号宽度
        public const int ProductCodeWidth = 8;//备件编号宽度
        public const int BillCodeWidth = 12;//单据编号宽度
        public static string AutobackupPath = string.Empty;
    }
}
