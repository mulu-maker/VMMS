using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// WindowProductSelect.xaml 的交互逻辑
    /// </summary>
    public partial class WindowProductSelect : Window
    {
       public IList<ObjProduct> l;//定义数据集合对象

        public WindowProductSelect()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalProduct.TempProduct = null;
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 0);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 6);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 7); 
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号           
        }

        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(dataGrid1.SelectedItem!=null)
            {
                DalProduct.TempProduct = dataGrid1.SelectedItem as ObjProduct;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
