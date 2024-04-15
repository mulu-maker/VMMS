using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 单据类别数据操作类
    /// </summary>
    public class DalBillType
    {
        /// <summary>
        /// ComboBox绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetList();
            cbo.SelectedValuePath = "TypeID";
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
            dgComboBoxColumn.ItemsSource = GetList();
            dgComboBoxColumn.SelectedValuePath = "TypeID";
            dgComboBoxColumn.DisplayMemberPath = "TypeName";
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary> 
        /// <returns>IList</returns>         
        public static IList<ObjBillType> GetList()
        {
            int[] ids = (int[])Enum.GetValues(typeof(EnumBillType));
            string[] names = Enum.GetNames(typeof(EnumBillType));
            List<ObjBillType> list = new List<ObjBillType>();
            int j = 0;
            foreach (int i in ids)
            {
                list.Add(new ObjBillType { TypeID = i, TypeName = names[j] });
                j++;
            }
            return list;
        }
    }
}
