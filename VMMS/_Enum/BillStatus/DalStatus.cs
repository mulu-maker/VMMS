using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>  
    /// 取号状态数据操作类
    /// 创建人：dinid
    /// 创建日期：20200416
    /// 版本：1.0
    /// 修改日期：
    /// </summary>
    public class DalBillStatus
    {
        /// <summary>
        /// ComboBox绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetList();
            cbo.SelectedValuePath = "StatusID";
            cbo.DisplayMemberPath = "StatusName";
        }

        /// <summary>
        /// DataGrid中ComboBox列绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingDataGridComboBoxColumn(DataGrid dataGrid1, int dgColumns)
        {
            DataGridComboBoxColumn dgComboBoxColumn = dataGrid1.Columns[dgColumns] as DataGridComboBoxColumn;
            dgComboBoxColumn.ItemsSource = GetList();
            dgComboBoxColumn.SelectedValuePath = "StatusID";
            dgComboBoxColumn.DisplayMemberPath = "StatusName";
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary> 
        /// <returns>IList</returns>         
        public static IList<ObjBillStatus> GetList()
        {
            List<ObjBillStatus> list = new List<ObjBillStatus>();
            int[] ids = (int[])Enum.GetValues(typeof(EnumBillStatus));
            string[] names = Enum.GetNames(typeof(EnumBillStatus));
            int j = 0;
            foreach (int i in ids)
            {
                list.Add(new ObjBillStatus { StatusID = i, StatusName = names[j] });
                j++;
            }
            return list;
        }

        /// <summary>
        /// 返回招生状态数据集合
        /// </summary> 
        /// <returns>IList</returns>         
        public static IList<ObjBillStatus> GetClueList()
        {
            List<ObjBillStatus> list = new List<ObjBillStatus>();
            int[] ids = (int[])Enum.GetValues(typeof(EnumBillStatus));
            string[] names = Enum.GetNames(typeof(EnumBillStatus));            
            for (int i =0;i<5;i++)
            {
                list.Add(new ObjBillStatus { StatusID = i, StatusName = names[i] });
            }
            return list;
        }

        /// <summary>
        /// 返回名称
        /// </summary>
        /// <returns>IList</returns>
        public static string GetName(int id)
        {
            IList<ObjBillStatus> list = GetList();
            string name = string.Empty;
            foreach (ObjBillStatus i in list)
            {
                if (i.StatusID == id)
                {
                    name = i.StatusName;
                    break;
                }
            }
            return name;
        }
    }
}
