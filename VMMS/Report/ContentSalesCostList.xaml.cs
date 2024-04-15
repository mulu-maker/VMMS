using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentSalesCostList.xaml 的交互逻辑
    /// </summary>
    public partial class ContentSalesCostList : Window
    {
        ObjProduct s;//定义查询条件

        public ContentSalesCostList()
        {
            InitializeComponent();
            s = new ObjProduct();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = s;
            DalUser.BindingComboBox(CboUser);
            //DalModel.BindingComboBox(CboModel);
            DalProductType.BindingComboBox(CboType);
            DalBillType.BindingDataGridComboBoxColumn(dataGrid1, 2);
            DalUser.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 9);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 10);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 11);
            Clear();
            LoadDataGrid(s);
        }

        private void Clear()
        {
            s = new ObjProduct();
            DateTime d = DateTime.Now;
            s.DateStart = BaseDateTimeClass.GetCurrentMonthStart(d).Date;
            s.DateEnd = BaseDateTimeClass.GetCurrentMonthEnd(d);
            dataGrid1.ItemsSource = null;
            this.DataContext = s;
        }

        private void LoadDataGrid(ObjProduct obj)
        {
            IList<ObjProduct> l = DalProduct.GetSalesCostList(obj);
            if (BaseListClass.CheckNull(l) == false)
            {
                //if (cbComplete.IsChecked == false)
                //{
                for (int i = l.Count - 1; i >= 0; i--)
                {
                    l[i].ProfitAmount = l[i].ChargeAmount - l[i].CreditAmount;
                    if (l[i].StatusID < (int)EnumBillStatus.完成)
                    {
                        l.RemoveAt(i);
                    }
                }
                //}
                //l.Add(new ObjProduct { Barcode = "        合计       ",  CreditNumber = l.Sum(p => p.CreditNumber), CreditAmount = l.Sum(p => p.CreditAmount), SalesNumber = l.Sum(p => p.SalesNumber), SalesAmount = l.Sum(p => p.SalesAmount),ChargeAmount= l.Sum(p => p.ChargeAmount), ProfitAmount = l.Sum(p => p.ProfitAmount) });
            }
            if (BaseListClass.CheckNull(l) == false)
            {
                dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
                dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号  
                lb.Content = "出库数量合计：" + l.Sum(p => p.CreditNumber) + "， 出库金额合计：" + l.Sum(p => p.CreditAmount).ToString("C") + "， 销售金额合计：" + l.Sum(p => p.SalesAmount).ToString("C") + "， 收款金额合计：" + l.Sum(p => p.ChargeAmount).ToString("C")+ "， 销售差价合计：" + l.Sum(p => p.ProfitAmount).ToString("C");
            }
            else
            {
                dataGrid1.ItemsSource = null;
                lb.Content = null;
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
                ObjProduct obj = dataGrid1.SelectedItem as ObjProduct;
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    WindowBillRepair child = new WindowBillRepair();
                    child.obj = DalBill.GetObject(obj.BillGUID);
                    child.IsAdd = false;
                    child.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }
    }
}
