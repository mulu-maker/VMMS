using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 单据类别数据操作类
    /// </summary>
    public class DalProductProperty
    {
        /// <summary>
        /// ComboBox绑定数据
        /// </summary>
        /// <param name="dataGrid1"></param>
        /// <param name="dgColumns"></param>
        public static void BindingComboBox(ComboBox cbo)
        {
            cbo.ItemsSource = GetList();
            cbo.SelectedValuePath = "PropertyID";
            cbo.DisplayMemberPath = "PropertyName";
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
            dgComboBoxColumn.SelectedValuePath = "PropertyID";
            dgComboBoxColumn.DisplayMemberPath = "PropertyName";
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary> 
        /// <returns>IList</returns>         
        public static IList<ObjProductProperty> GetList()
        {
            int[] ids = (int[])Enum.GetValues(typeof(EnumProductProperty));
            string[] names = Enum.GetNames(typeof(EnumProductProperty));
            List<ObjProductProperty> list = new List<ObjProductProperty>();
            int j = 0;
            foreach (int i in ids)
            {
                list.Add(new ObjProductProperty { PropertyID = i, PropertyName = names[j] });
                j++;
            }
            return list;
        }
    }
}
