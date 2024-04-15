using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentProductInOutList.xaml 的交互逻辑
    /// </summary>
    public partial class ContentProductInOutList : Window
    {        
        public ObjProduct s;//定义查询条件
        public bool IsShow = false;

        public ContentProductInOutList()
        {
            InitializeComponent();
            s = new ObjProduct();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = s;
            DalBillType.BindingComboBox(CboBillType);
            DalCustomer.BindingComboBox(CboCustomer);
            DalUser.BindingComboBox(CboUser);
            DalModel.BindingComboBox(CboModel);
            DalProductType.BindingComboBox(CboType);
            DalBillType.BindingDataGridComboBoxColumn(dataGrid1, 2);
            DalCustomer.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalUser.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 10);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 11);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 12);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 14);
            DalBillStatus.BindingDataGridComboBoxColumn(dataGrid1, 16);
            if (IsShow == false)
            {
                Clear();
            }
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
            IList<ObjProduct> l = DalProduct.GetBillProductList(obj);
            if (BaseListClass.CheckNull(l) == false)
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
                if (BaseListClass.CheckNull(l) == false)
                {
                    dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
                    dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号  
                    lb.Content = string.Format("入库数量合计：{0}， 出库数量合计：{2}", l.Sum(p => p.DebitNumber), l.Sum(p => p.DebitAmount), l.Sum(p => p.CreditNumber), l.Sum(p => p.CreditAmount));
                }
                else
                {
                    dataGrid1.ItemsSource = null;
                    lb.Content = null;
                }
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
                if (obj.TypeID == (int)EnumBillType.入库单)
                {
                    WindowBillIn child = new WindowBillIn();
                    child.obj = DalBill.GetObject(obj.BillGUID);
                    child.IsAdd = null;
                    child.ShowDialog();
                }
                else if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    if (obj.StatusID == (int)EnumBillStatus.保存)
                    {
                        WindowBillRepair child = new WindowBillRepair();
                        child.obj = DalBill.GetObject(obj.BillGUID);
                        child.IsAdd = false;
                        child.ShowDialog();
                    }
                    else
                    {
                        WindowBillRepair child = new WindowBillRepair();
                        child.obj = DalBill.GetObject(obj.BillGUID);
                        child.IsAdd = null;
                        child.ShowDialog();
                    }
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
