using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace VMMS
{
    /// </summary>
    /// 供应商数据操作类
    /// </summary>
    public class DalCustomer
    {
        /// <summary>
        /// ComboBox列绑定有效数据
        /// </summary>
        /// <param name="cbo">ComboBox控件名</param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetViewList(null);
            cbo.SelectedValuePath = "CustomerGUID";
            cbo.DisplayMemberPath = "CustomerName";
        }

        /// <summary>
        /// DataGridComboBoxColumn绑定有效数据
        /// </summary>
        /// <param name="dataGrid1">DataGrid控件名</param>
        /// <param name="dgColumns">DataGrid中DataGridComboBoxColumn列号</param>
        public static void BindingDataGridComboBoxColumn(DataGrid dataGrid1, int dgColumns)
        {
            DataGridComboBoxColumn dgComboBoxColumn = dataGrid1.Columns[dgColumns] as DataGridComboBoxColumn;
            dgComboBoxColumn.ItemsSource = GetFullList(null);
            dgComboBoxColumn.SelectedValuePath = "CustomerGUID";
            dgComboBoxColumn.DisplayMemberPath = "CustomerName";
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjCustomer> GetViewList(ObjCustomer s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_customer WHERE DeleteMark=0 ");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_customer WHERE DeleteMark=0");
                DalSQLite.AppendConditionString<ObjCustomer>(s, sqlString);
            }
            return DalSQLite.GetIList<ObjCustomer>(sqlString.ToString());
        }

        /// <summary>
        /// 返回数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjCustomer> GetFullList(ObjCustomer s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_customer ORDER BY CustomerCode");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_customer WHERE (DeleteMark = 0 OR DeleteMark = 1) ");
                DalSQLite.AppendConditionString<ObjCustomer>(s, sqlString);
                sqlString.Append(" ORDER BY CustomerCode");
            }
            return DalSQLite.GetIList<ObjCustomer>(sqlString.ToString());
        }

        /// <summary>
        /// 返回新编码
        /// </summary>
        /// <returns></returns>
        public static string GetCode()
        {
            object o = BaseDbSQLiteClass.GetSingle("SELECT MAX(CustomerCode) FROM crs_customer");//获取最大编号
            return BaseStringClass.GetNewMaxCode(o, DalDataConfig.CustomerCodeWidth);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjCustomer obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetInsertSqlString(ObjCustomer obj)
        {
            if (obj.CustomerGUID == new Guid())
                obj.CustomerGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_customer (CustomerGUID, CustomerCode,  CustomerName, MnemonicCode, Phone, Email, LinkAddress, LinkMan, MobilePhone, BankName, BankAccount, TaxNumber, Remark, UpGUID, Uptime) SELECT '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT CustomerCode FROM crs_customer WHERE CustomerCode='{1}')", obj.CustomerGUID, obj.CustomerCode, obj.CustomerName, obj.MnemonicCode, obj.Phone, obj.Email, obj.LinkAddress, obj.LinkMan, obj.MobilePhone, obj.BankName, obj.BankAccount, obj.TaxNumber, obj.Remark, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjCustomer obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_customer SET  CustomerCode='{1}',CustomerName='{2}',MnemonicCode='{3}', Phone='{4}', Email='{5}', LinkAddress='{6}', LinkMan='{7}', MobilePhone='{8}', BankName='{9}', BankAccount='{10}', TaxNumber='{11}', Remark='{12}', UpGUID='{13}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND CustomerID={0} AND NOT EXISTS (SELECT CustomerCode FROM crs_customer WHERE CustomerCode='{1}' AND CustomerID<>{0})", obj.CustomerID, obj.CustomerCode, obj.CustomerName, obj.MnemonicCode, obj.Phone, obj.Email, obj.LinkAddress, obj.LinkMan, obj.MobilePhone, obj.BankName, obj.BankAccount, obj.TaxNumber, obj.Remark, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 作废标记
        /// </summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool DeleteMark(ObjCustomer obj)
        {
            string sqlString = string.Format("UPDATE crs_customer SET DeleteMark=1 WHERE CustomerID={0}", obj.CustomerID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjCustomer> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjCustomer i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
