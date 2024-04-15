using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// WindowModelProduct.xaml 的交互逻辑
    /// </summary>
    public partial class WindowModelProduct : Window
    {
        public Guid g;//定义查询条件对象

        public WindowModelProduct()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 6);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 7);
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalModel.GetProductList(g);//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号           
        }
    }
}
