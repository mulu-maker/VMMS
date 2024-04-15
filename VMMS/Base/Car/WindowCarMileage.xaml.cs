using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// WindowCarMileage.xaml 的交互逻辑
    /// </summary>
    public partial class WindowCarMileage : Window
    {
        public ObjCar s;
        IList<ObjBill> l;
        public WindowCarMileage()
        {
            InitializeComponent();
            s = new ObjCar();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalBillType.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalBillStatus.BindingDataGridComboBoxColumn(dataGrid1, 11);
            if (s != null && s.CarGUID != new Guid())
            {

                LoadDataGrid();
            }
        }

        private void LoadDataGrid()
        {
            l = DalBill.GetCarList(s);
            if (BaseListClass.CheckNull(l) == false)
            {
                string sumMileage = l.Sum(p => p.Mileage).ToString();
                if (l[l.Count - 1].CarMileage == s.TotalMileage)
                {
                    lb.Content = "当前车辆里程：" + s.TotalMileage.ToString("N0") + "公里，行驶里程合计：" + sumMileage +"公里";
                }
                else
                {
                    lb.Content = "当前车辆里程：" + s.TotalMileage.ToString("N0") + "公里 ≠ 最后一张维修单"+ l[l.Count - 1].BillCode + "的车辆里程" + l[l.Count - 1].CarMileage + "，行驶里程合计：" + sumMileage +"公里";
                    lb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                }
                dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
                dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号  
            }
        }

        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            if (BaseListClass.CheckNull(l) == false)
            {
                string s = DalCar.CheckMileage(l);
                if (string.IsNullOrEmpty(s) == false)
                {
                    MessageBox.Show(s);
                }
                else
                {
                    MessageBox.Show("车辆里程重算正确！");
                }
            }
        }
    }
}
