using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>  
    /// 员工数据操作层
    /// </summary>  
    public class DalUser
    {
        /// <summary>
        /// ComboBox列绑定有效数据
        /// </summary>
        /// <param name="cbo">ComboBox控件名</param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetViewList();
            cbo.SelectedValuePath = "UserGUID";
            cbo.DisplayMemberPath = "UserName";
        }

        /// <summary>
        /// DataGridComboBoxColumn绑定有效数据
        /// </summary>
        /// <param name="dataGrid1">DataGrid控件名</param>
        /// <param name="dgColumns">DataGrid中DataGridComboBoxColumn列号</param>
        public static void BindingDataGridComboBoxColumn(DataGrid dataGrid1, int dgColumns)
        {
            DataGridComboBoxColumn dgComboBoxColumn = dataGrid1.Columns[dgColumns] as DataGridComboBoxColumn;
            dgComboBoxColumn.ItemsSource = GetFullList();
            dgComboBoxColumn.SelectedValuePath = "UserGUID";
            dgComboBoxColumn.DisplayMemberPath = "UserName";
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList<ObjUser></returns>
        public static IList<ObjUser> GetViewList()
        {
            return DalSQLite.GetIList<ObjUser>("SELECT * FROM sys_user WHERE DeleteMark=0 ORDER BY UserCode");
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary>
        /// <returns>IList<ObjWorkplace></returns>
        public static IList<ObjUser> GetFullList()
        {
            return DalSQLite.GetIList<ObjUser>("SELECT * FROM sys_user ORDER BY UserCode");
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjUser obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetInsertSqlString(ObjUser obj)
        {
            if (obj.UserGUID == new Guid())
                obj.UserGUID = Guid.NewGuid();
            return string.Format("INSERT INTO sys_user (UserGUID, UserCode, UserName,CompanyGUID,MobilePhone,Email,Office,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT UserCode FROM sys_user WHERE UserCode='{1}')", obj.UserGUID, obj.UserCode, obj.UserName, obj.CompanyGUID, obj.MobilePhone, obj.Email, obj.Office, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjUser obj)
        {
            return DalSQLite.Update(string.Format("UPDATE sys_user SET  UserCode='{1}',UserName='{2}',CompanyGUID='{3}',MobilePhone='{4}',Email='{5}',UpGUID='{6}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND UserID={0} AND NOT EXISTS (SELECT UserCode FROM sys_user WHERE UserCode='{1}' AND UserID<>{0})", obj.UserID, obj.UserCode, obj.UserName, obj.CompanyGUID, obj.MobilePhone, obj.Email, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="UserID"></param>
        public static bool DeleteMark(ObjUser obj)
        {
            string sqlString = string.Format("UPDATE sys_user SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE UserID={0} AND DeleteMark=0", obj.UserID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjUser> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjUser i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
