using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 备件类别数据操作层
    /// </summary>
    public class DalProductType
    {
        /// <summary>
        /// ComboBox绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetViewList();
            cbo.SelectedValuePath = "TypeGUID";
            cbo.DisplayMemberPath = "TypeName";
        }

        /// <summary>
        /// DataGrid中ComboBox列绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingDataGridComboBoxColumn(DataGrid dataGrid1, int dgColumns)
        {
            DataGridComboBoxColumn dgComboBoxColumn = dataGrid1.Columns[dgColumns] as DataGridComboBoxColumn;
            dgComboBoxColumn.ItemsSource = GetFullList();
            dgComboBoxColumn.SelectedValuePath = "TypeGUID";
            dgComboBoxColumn.DisplayMemberPath = "TypeName";
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProductType> GetViewList()
        {
            return DalSQLite.GetIList<ObjProductType>("SELECT * FROM crs_product_type WHERE DeleteMark =0 ORDER BY TypeCode");
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProductType> GetFullList()
        {
            return DalSQLite.GetIList<ObjProductType>("SELECT * FROM crs_product_type ORDER BY TypeCode");
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjProductType obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetInsertSqlString(ObjProductType obj)
        {
            if (obj.TypeGUID == new Guid())
                obj.TypeGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_product_type (TypeGUID,TypeCode, TypeName,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT TypeCode FROM crs_product_type WHERE TypeCode='{1}' AND DeleteMark=0)", obj.TypeGUID, obj.TypeCode, obj.TypeName, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjProductType obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_product_type SET  TypeCode='{1}',TypeName='{2}',UpGUID='{3}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND TypeID={0} AND NOT EXISTS (SELECT TypeCode FROM crs_product_type WHERE TypeCode='{1}' AND TypeID<>{0})", obj.TypeID, obj.TypeCode, obj.TypeName, DalLogin.LoginedUser.UserGUID));
        }


        /// <summary>
        /// 删除数据（做删除标记）
        /// </summary>
        /// <param name="TypeID">TypeID</param>
        public static bool DeleteMark(ObjProductType obj)
        {
            string sqlString = string.Format("UPDATE crs_product_type SET DeleteMark=1 WHERE TypeID={0}", obj.TypeID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 检查数据表指定列的数据是否在已有的数据集合中全部存在
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="ColName">列名</param>
        /// <param name="l">已有的数据集合</param>
        /// <param name="remark">缺少的名称</param>
        /// <returns></returns>
        public static bool Exist(DataTable dt, string ColName, IList<ObjProductType> l, ref string remark)
        {
            bool result = false;//结果
            int sn = 0;//已有数据计数
            if (BaseDataTable.CheckNull(dt) == false && BaseListClass.CheckNull(l) == false)
            {
                for (int i = 0; i < dt.Rows.Count; i++)//循环数据行
                {
                    string name = dt.Rows[i][ColName].ToString().Trim();
                    int count = l.Count(p => p.TypeName == name);
                    if (count == 1)
                    {
                        sn++;
                    }
                    else
                    {
                        remark = remark + name + ";";
                    }
                }
                if (sn == dt.Rows.Count)
                {
                    result = true;
                }
                else
                {
                    remark = "无以下数据，请先输入相应数据：" + remark + "";
                }
            }
            return result;
        }

        /// <summary>
        /// 返回数据集合中名称相等的GUID
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="l">数据集合</param>
        /// <returns>Guid</returns>
        public static Guid GetGUID(string name, IList<ObjProductType> l)
        {
            Guid g = new Guid();
            if (BaseListClass.CheckNull(l) == false)
            {
                foreach (ObjProductType i in l)
                {
                    if (i.TypeName == name)
                    {
                        g = i.TypeGUID;
                        break;
                    }
                }
            }
            return g;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjProductType> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjProductType i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
