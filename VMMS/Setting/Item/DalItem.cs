using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 维修项目数据操作层
    /// </summary>
    public class DalItem
    {
        /// <summary>
        /// ComboBox列绑定有效数据
        /// </summary>
        /// <param name="cbo">ComboBox控件名</param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetViewList();
            cbo.SelectedValuePath = "ItemGUID";
            cbo.DisplayMemberPath = "ItemName";
        }

        /// <summary>
        /// ComboBox列绑定有效数据
        /// </summary>
        /// <param name="cbo">ComboBox控件名</param>
        public static void BindingComboBox(ComboBox cbo, IList<ObjItem> list)
        {
            cbo.ItemsSource = list;
            cbo.SelectedValuePath = "ItemGUID";
            cbo.DisplayMemberPath = "ItemName";
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
            dgComboBoxColumn.SelectedValuePath = "ItemGUID";
            dgComboBoxColumn.DisplayMemberPath = "ItemName";
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList<ObjItem></returns>
        public static IList<ObjItem> GetViewList()
        {
            return DalSQLite.GetIList<ObjItem>("SELECT * FROM crs_item WHERE DeleteMark=0 ORDER BY ItemCode");
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary>
        /// <returns>IList<ObjWorkplace></returns>
        public static IList<ObjItem> GetFullList()
        {
            return DalSQLite.GetIList<ObjItem>("SELECT * FROM crs_item ORDER BY ItemCode");
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjItem obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        private static string GetInsertSqlString(ObjItem obj)
        {
            if (obj.ItemGUID == new Guid())
                obj.ItemGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_item (ItemGUID, ItemCode, ItemName,Remark,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}','{4}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT ItemCode FROM crs_item WHERE ItemCode='{1}' AND DeleteMark=0)", obj.ItemGUID, obj.ItemCode, obj.ItemName, obj.Remark, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjItem obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_item SET  ItemCode='{1}',ItemName='{2}',Remark='{3}',UpGUID='{4}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND ItemID={0} AND NOT EXISTS (SELECT ItemCode FROM crs_item WHERE ItemCode='{1}' AND ItemID<>{0})", obj.ItemID, obj.ItemCode, obj.ItemName, obj.Remark, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="ItemID"></param>
        public static bool DeleteMark(ObjItem obj)
        {
            string sqlString = string.Format("UPDATE crs_item SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE ItemID={0} AND DeleteMark=0", obj.ItemID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjItem> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjItem i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
