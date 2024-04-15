using System;

namespace VMMS
{
    public class ObjCustomer
    {
        /// <summary>  
        /// 供应商对象类
        /// </summary>
        public int CustomerID { get; set; }
        public Guid CustomerGUID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string MnemonicCode { get; set; }//助记码
        public string Phone { get; set; }
        public string Email { get; set; }//电子邮件
        public string LinkAddress { get; set; }//联系地址
        public string LinkMan { get; set; }//联系人
        public string MobilePhone { get; set; }//手机号
        public string BankName { get; set; }//开户银行
        public string BankAccount { get; set; }//银行账号
        public string TaxNumber { get; set; }//税号
        public string Remark { get; set; }
        public bool DeleteMark { get; set; }
        public Guid UpGUID { get; set; }
        public DateTime Uptime { get; set; }
    }
}