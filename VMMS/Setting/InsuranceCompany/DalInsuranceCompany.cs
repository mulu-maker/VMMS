using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 维修项目数据操作层
    /// </summary>
    public class DalInsuranceCompany
    {
        /// <summary>
        /// ComboBox列绑定有效数据
        /// </summary>
        /// <param name="cbo">ComboBox控件名</param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetViewList();
            cbo.SelectedValuePath = "InsuranceCompanyGUID";
            cbo.DisplayMemberPath = "InsuranceCompanyName";
        }

        /// <summary>
        /// ComboBox列绑定有效数据
        /// </summary>
        /// <param name="cbo">ComboBox控件名</param>
        public static void BindingComboBox(ComboBox cbo, IList<ObjInsuranceCompany> list)
        {
            cbo.ItemsSource = list;
            cbo.SelectedValuePath = "InsuranceCompanyGUID";
            cbo.DisplayMemberPath = "InsuranceCompanyName";
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
            dgComboBoxColumn.SelectedValuePath = "InsuranceCompanyGUID";
            dgComboBoxColumn.DisplayMemberPath = "InsuranceCompanyName";
        }

        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList<ObjInsuranceCompany></returns>
        public static IList<ObjInsuranceCompany> GetViewList()
        {
            return DalSQLite.GetIList<ObjInsuranceCompany>("SELECT * FROM crs_insurancecompany WHERE DeleteMark=0 ORDER BY InsuranceCompanyCode");
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary>
        /// <returns>IList<ObjWorkplace></returns>
        public static IList<ObjInsuranceCompany> GetFullList()
        {
            return DalSQLite.GetIList<ObjInsuranceCompany>("SELECT * FROM crs_insurancecompany ORDER BY InsuranceCompanyCode");
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjInsuranceCompany obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        private static string GetInsertSqlString(ObjInsuranceCompany obj)
        {
            if (obj.InsuranceCompanyGUID == new Guid())
                obj.InsuranceCompanyGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_insurancecompany (InsuranceCompanyGUID, InsuranceCompanyCode, InsuranceCompanyName,Remark,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}','{4}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT InsuranceCompanyCode FROM crs_insurancecompany WHERE InsuranceCompanyCode='{1}' AND DeleteMark=0)", obj.InsuranceCompanyGUID, obj.InsuranceCompanyCode, obj.InsuranceCompanyName, obj.Remark, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjInsuranceCompany obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_insurancecompany SET  InsuranceCompanyCode='{1}',InsuranceCompanyName='{2}',Remark='{3}',UpGUID='{4}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND InsuranceCompanyID={0} AND NOT EXISTS (SELECT InsuranceCompanyCode FROM crs_insurancecompany WHERE InsuranceCompanyCode='{1}' AND InsuranceCompanyID<>{0})", obj.InsuranceCompanyID, obj.InsuranceCompanyCode, obj.InsuranceCompanyName, obj.Remark, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="InsuranceCompanyID"></param>
        public static bool DeleteMark(ObjInsuranceCompany obj)
        {
            string sqlString = string.Format("UPDATE crs_insurancecompany SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE InsuranceCompanyID={0} AND DeleteMark=0", obj.InsuranceCompanyID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjInsuranceCompany> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjInsuranceCompany i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
