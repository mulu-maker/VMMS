using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// WindowProductSelectLocation.xaml 的交互逻辑
    /// </summary>
    public partial class WindowProductSelectLocation : Window
    {
        public bool IsHaveZero=true;//定义是否包含0库存
        public IList<ObjProduct> l;//定义数据集合对象

        public WindowProductSelectLocation()
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
            if (BaseListClass.CheckNull(l) == false)
            {
                if (IsHaveZero == false)
                {
                    for (int i = l.Count - 1; i >= 0; i--)
                    {
                        if (l[i].InventoryNumber == 0 && l[i].InventoryAmount == 0)
                        {
                            l.RemoveAt(i);
                        }
                    }
                }
                dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
                dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号           
            }
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

        bool GetSelect(ref ObjProduct p)
        {
            bool result = false;
            if (p.PropertyID == (int)EnumProductProperty.实物)
            {
                if (p.LocationGUID != new Guid())//实物备件有库位
                {
                    result = true;
                }
                else//实物备件无库位
                {
                    if (CboLocation.SelectedItem != null)
                    {
                        p.LocationGUID = (CboLocation.SelectedItem as ObjLocation).LocationGUID;
                        result = true;
                    }
                    else
                    {
                        MessageBox.Show("无入库记录的备件请先选择一个库位！");
                    }
                }    
            }
            else if (p.PropertyID == (int)EnumProductProperty.服务)
            {
                result = true;
            }

            return result;
        }

    }
}
