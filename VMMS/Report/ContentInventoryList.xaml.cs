using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentInventoryList.xaml 的交互逻辑
    /// </summary>
    public partial class ContentInventoryList : Window
    {
        public ObjProduct s;//定义查询条件对象
        public ObjProduct result;//定义选择结果
        public bool IsSelect = false;//定时选择模式标识

        public ContentInventoryList()
        {
            InitializeComponent();
            s = new ObjProduct();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalProductType.BindingComboBox(CboType);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 6);
            if (IsSelect == false)
            {
                Clear();
                LoadDataGrid();
            }

        }

        private void Clear()
        {
            s = new ObjProduct();
            this.DataContext = s;
        }

        private void LoadDataGrid()
        {
            IList<ObjProduct> l = DalProduct.GetInventoryList(s);
            IList<ObjProduct> saves = DalBill.GetRepairSaveList();
            if (BaseListClass.CheckNull(l)==false)
            {
                for(int i =l.Count-1;i>=0;i--)
                {
                    if (l[i].InventoryNumber == 0 && l[i].InventoryAmount==0)
                    {
                        if(cbZone.IsChecked==false)
                        {
                            l.RemoveAt(i);
                        }
                    }
                    else
                    {
                        if (l[i].InventoryNumber != 0)
                        {
                            l[i].InventoryCost = l[i].InventoryAmount / l[i].InventoryNumber;
                        }
                        if(BaseListClass.CheckNull(saves)==false)//获取保存状态的维修单明细数据合计数
                        {
                            foreach(ObjProduct j in saves)
                            {
                                if(j.ProductGUID== l[i].ProductGUID)
                                {
                                    l[i].SalesNumber = j.SalesNumber;//保存状态的维修单备件合计数
                                    l[i].EndNumber = l[i].InventoryNumber - l[i].SalesNumber;//实际库存=库存表-保存状态的维修单备件合计数
                                }
                            }
                        }
                    }
                }
                //l.Add(new ObjProduct { LocationName = "    合计    ", InventoryNumber = l.Sum(p => p.InventoryNumber), InventoryAmount = l.Sum(p => p.InventoryAmount) });
            }           
            if (BaseListClass.CheckNull(l) == false)
            {
                dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
                dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号  (BaseWindowClass.DataGrid_LoadingRow);//显示行号  
                lb.Content = "库存数量合计：" + l.Sum(p => p.InventoryNumber) + "， 库存金额合计：" + l.Sum(p => p.InventoryAmount).ToString("C") + "， 维修锁定数量合计：" + l.Sum(p => p.SalesNumber) + "， 实际库存数量合计合计：" + l.Sum(p => p.EndNumber);
            }
            else
            {
                dataGrid1.ItemsSource = null;
                lb.Content = null;
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            ShowDetail();
        }

        private void ShowDetail()
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjProduct p = dataGrid1.SelectedItem as ObjProduct;
                ContentProductInOutList child = new ContentProductInOutList();
                child.IsShow = true;
                child.s.DateStart = DateTime.Now.AddYears(-1);
                child.s.DateEnd = DateTime.Now;
                child.s.ProductCode = p.ProductCode;
                child.ShowDialog();
            }
        }

        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {            
            if (dataGrid1.SelectedItem != null)
            {
                ShowDetail();
            }
        }
    }
}
