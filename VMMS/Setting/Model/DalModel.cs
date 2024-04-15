using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 车型数据操作层
    /// </summary>
    public class DalModel
    {
        /// <summary>
        /// ComboBox绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetViewList();
            cbo.SelectedValuePath = "ModelGUID";
            cbo.DisplayMemberPath = "ModelName";
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
            dgComboBoxColumn.SelectedValuePath = "ModelGUID";
            dgComboBoxColumn.DisplayMemberPath = "ModelName";
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjModel> GetViewList()
        {
            return DalSQLite.GetIList<ObjModel>("SELECT * FROM crs_model WHERE DeleteMark =0 ORDER BY ModelCode");
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjModel> GetFullList()
        {
            return DalSQLite.GetIList<ObjModel>("SELECT * FROM crs_model ORDER BY ModelCode");
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjModel obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetInsertSqlString(ObjModel obj)
        {
            if (obj.ModelGUID == new Guid())
                obj.ModelGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_model (ModelGUID,ModelCode,ModelName,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT ModelCode FROM crs_model WHERE ModelCode='{1}' AND DeleteMark=0)", obj.ModelGUID, obj.ModelCode, obj.ModelName, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjModel obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_model SET  ModelCode='{1}',ModelName='{2}',UpGUID='{3}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND ModelID={0} AND NOT EXISTS (SELECT ModelCode FROM crs_model WHERE ModelCode='{1}' AND ModelID<>{0})", obj.ModelID, obj.ModelCode, obj.ModelName, DalLogin.LoginedUser.UserGUID));
        }


        /// <summary>
        /// 删除数据（做删除标记）
        /// </summary>
        /// <param name="ModelID">TypeID</param>
        public static bool DeleteMark(ObjModel obj)
        {
            string sqlString = string.Format("UPDATE crs_model SET DeleteMark=1 WHERE ModelID={0}", obj.ModelID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjModel> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjModel i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }

        /// <summary>
        /// 检查数据表指定列的数据是否在已有的数据集合中全部存在
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="ColName">列名</param>
        /// <param name="l">已有的数据集合</param>
        /// <param name="remark">缺少的名称</param>
        /// <returns></returns>
        public static bool ExistGUID(IList<ObjModel> l, ObjModel obj)
        {
            bool result = false;//结果
            if(BaseListClass.CheckNull(l)==false)
            {
                foreach(ObjModel i in l)
                {
                    if(obj.ModelGUID==i.ModelGUID)
                    {
                        result = true;
                        break;
                    }
                }
            }          
            return result;
        }

        /// <summary>
        /// 检查数据表指定列的数据是否在已有的数据集合中全部存在
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="ColName">列名</param>
        /// <param name="l">已有的数据集合</param>
        /// <param name="remark">缺少的名称</param>
        /// <returns></returns>
        public static bool Exist(DataTable dt, string ColName, IList<ObjModel> l, ref string remark)
        {
            bool result = false;//结果
            int sn = 0;//已有数据计数
            if (BaseDataTable.CheckNull(dt) == false && BaseListClass.CheckNull(l) == false)
            {
                for (int i = 0; i < dt.Rows.Count; i++)//循环数据行
                {
                    string name = dt.Rows[i][ColName].ToString().Trim();
                    int count = l.Count(p => p.ModelName == name);
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
        public static Guid GetGUID(string name, IList<ObjModel> l)
        {
            Guid g = new Guid();
            if (BaseListClass.CheckNull(l) == false)
            {
                foreach (ObjModel i in l)
                {
                    if (i.ModelName == name)
                    {
                        g = i.ModelGUID;
                        break;
                    }
                }
            }
            return g;
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProduct> GetProductList(Guid modelGUID)
        {
            return DalSQLite.GetIList<ObjProduct>(string.Format("SELECT a.* FROM crs_product AS a JOIN crs_product_model AS b ON a.ProductGUID=b.ProductGUID WHERE a.DeleteMark =0 AND b.ModelGUID='{0}' ORDER BY a.ProductCode", modelGUID));
        }

        public static bool CheckRepeat(ObjModel obj)
        {
            bool result = true;
            if(obj!=null && string.IsNullOrEmpty(obj.ModelName)==false)
            {
                int i = DalSQLite.GetDataRecordCount(string.Format("SELECT COUNT(*) FROM crs_model WHERE DeleteMark=0 AND ModelName='{0}'", obj.ModelName));
                if(i < 1)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
