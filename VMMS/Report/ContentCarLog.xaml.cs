using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentCarLog.xaml 的交互逻辑
    /// </summary>
    public partial class ContentCarLog : Window
    {
        public ObjBill s;//定义查询条件
        public bool IsDetail = false;//定义是否未明细表

        public ContentCarLog()
        {
            InitializeComponent();
            s = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = s;
            DalModel.BindingComboBox(CboModel);
            DalUser.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalModel.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalBillStatus.BindingDataGridComboBoxColumn(dataGrid1, 15);
            if (IsDetail == false)
            {
                Clear();
            }
            else
            {
                LoadDataGrid(s);
            }
        }

        private void Clear()
        {
            s = new ObjBill();
            DateTime d = DateTime.Now;
            s.DateStart = BaseDateTimeClass.GetCurrentMonthStart(d).Date;
            s.DateEnd = BaseDateTimeClass.GetCurrentMonthEnd(d);
            dataGrid1.ItemsSource = null;
            this.DataContext = s;
        }

        private void LoadDataGrid(ObjBill obj)
        {
            IList<ObjBill> l = DalCar.GetBillList(obj); 
            if(BaseListClass.CheckNull(l)==false)
            {
                if (cbComplete.IsChecked == false)
                {
                    for (int i = l.Count - 1; i >= 0; i--)
                    {
                        if (l[i].StatusID < (int)EnumBillStatus.完成)
                        {
                            l.RemoveAt(i);
                        }
                    }
                }
                l = l.Where(p => p.CreditNumber != 0).ToList();
                //l.Add(new ObjBill { ProductName = "    合计    ", CreditNumber = l.Sum(p => p.CreditNumber), SalesAmount = l.Sum(p => p.SalesAmount) });
                if (BaseListClass.CheckNull(l) == false)
                {
                    dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
                    dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号  (BaseWindowClass.DataGrid_LoadingRow);//显示行号  
                    lb.Content = "记录数量：" + l.Count + "， 备件数量合计：" + l.Sum(p => p.CreditNumber) + "， 金额合计：" + l.Sum(p => p.SalesAmount).ToString("C");
                }
                else
                {
                    dataGrid1.ItemsSource = null;
                    lb.Content = null;
                }
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid(s);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    WindowBillRepair child = new WindowBillRepair();
                    child.obj = obj;
                    child.IsAdd = null;
                    child.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnIn_Click(object sender, RoutedEventArgs e)
        {
            WindowBillIn child = new WindowBillIn();
            child.ShowDialog();
            LoadDataGrid(s);
        }
    }
}
