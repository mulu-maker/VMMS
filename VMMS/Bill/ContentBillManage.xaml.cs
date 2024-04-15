using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentBillManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentBillManage : Window
    {
        public ObjBill s;//定义查询条件
        public IList<ObjBill> l;

        public ContentBillManage()
        {
            InitializeComponent();
            s = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalBillType.BindingComboBox(CboType);
            DalCustomer.BindingComboBox(CboCustomer);
            DalUser.BindingComboBox(CboUser);
            DalBillStatus.BindingComboBox(CboStatus);
            DalBillType.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalCustomer.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalUser.BindingDataGridComboBoxColumn(dataGrid1, 6);
            DalItem.BindingDataGridComboBoxColumn(dataGrid1, 7);
            DalBillStatus.BindingDataGridComboBoxColumn(dataGrid1, 23);
            DateTime d = DateTime.Now;
            s.DateStart = d.Date.AddDays(-30).Date;
            s.DateEnd = d;
            this.DataContext = s;
            if (BaseListClass.CheckNull(l) == false)
            {
                Sum();
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
            s.DateStart = d.Date.AddDays(-30).Date;
            s.DateEnd = d;
            dataGrid1.ItemsSource = null;
            this.DataContext = s;
        }

        private void LoadDataGrid(ObjBill obj)
        {
            l = DalBill.GetList(obj);
            if (BaseListClass.CheckNull(l) == false)
            {
                Sum();
            }
            else
            {
                dataGrid1.ItemsSource = null;
                lb.Content = null;
            }
        }

        private void Sum()
        {
            dataGrid1.ItemsSource = l;//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
            lb.Content = "入库数量合计：" + l.Sum(p => p.TotalDebitNumber) + "， 出库数量合计：" + l.Sum(p => p.TotalCreditNumber) + "， 销售金额合计：" + l.Sum(p => p.TotalSalesAmount).ToString("C") + "， 折扣金额合计：" + l.Sum(p => p.TotalDiffAmount).ToString("C") + "， 实收金额合计：" + l.Sum(p => p.TotalChargeAmount).ToString("C");
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid(s);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// 双击数据行查看单据明细
        /// </summary>
        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (obj.TypeID == (int)EnumBillType.入库单)
                {
                    WindowBillIn child = new WindowBillIn();
                    child.obj = obj;
                    child.IsAdd = false;
                    child.ShowDialog();
                }
                else if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    WindowBillRepair child = new WindowBillRepair();
                    child.obj = obj;
                    child.IsAdd = null;
                    child.ShowDialog();
                }
                else if (obj.TypeID == (int)EnumBillType.退库单)
                {
                    WindowBillBack child = new WindowBillBack();
                    child.obj = obj;
                    child.IsAdd = false;
                    child.ShowDialog();
                }
                else if (obj.TypeID == (int)EnumBillType.退货单)
                {
                    WindowBillExit child = new WindowBillExit();
                    child.obj = obj;
                    child.IsAdd = false;
                    child.ShowDialog();
                }
                LoadDataGrid(s);
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

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (DalBill.GetSource(obj.BillGUID) == false)//检查是否已生成退货单
                {
                    if (obj.TypeID == (int)EnumBillType.入库单)
                    {
                        if (obj.StatusID == (int)EnumBillStatus.完成)
                        {
                            WindowBillExit child = new WindowBillExit();
                            child.IsAdd = true;
                            child.obj = obj;
                            child.obj.SourceGUID = child.obj.BillGUID;
                            child.obj.SourceCode = child.obj.BillCode;
                            child.obj.BillGUID = new Guid();
                            child.obj.BillCode = string.Empty;
                            child.obj.ListDetail = DalBill.GetDetailList(obj.SourceGUID);
                            if (BaseListClass.CheckNull(child.obj.ListDetail) == false)
                            {
                                foreach (ObjProduct i in child.obj.ListDetail)
                                {
                                    i.DetailGUID = new Guid();
                                    i.DebitCost = 0 - i.DebitCost;
                                    i.DebitNumber = 0 - i.DebitNumber;
                                    i.DebitAmount = 0 - i.DebitAmount;
                                    i.DiffAmount = 0 - i.DiffAmount;
                                }
                                child.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("入库单无明细备件可退货！");
                            }
                        }
                        else
                        {
                            MessageBox.Show("仅完成状态的入库单可退货！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("仅入库单可退货！");
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("已有执行过退库操作，无法再次执行，请检查其他单据中的源单号：{0}！", obj.BillCode));
                }
            }
            else
            {
                MessageBox.Show("请选择一个完成状态的入库单，执行退货操作！");
            }
            LoadDataGrid(s);
        }

        private void BtnCharge_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    if (obj.StatusID == (int)EnumBillStatus.完成)
                    {
                        DalBill.Charge(obj);
                    }
                    else
                    {
                        MessageBox.Show("仅完成状态的维修单可收款！");
                    }
                }
                else
                {
                    MessageBox.Show("仅维修单可收款！");
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
            LoadDataGrid(s);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    if (obj.StatusID == (int)EnumBillStatus.保存)
                    {
                        WindowBillRepair child = new WindowBillRepair();
                        child.obj = obj;
                        child.IsAdd = false;
                        child.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("仅保存状态的维修单可修改！");
                    }
                }
                else
                {
                    MessageBox.Show("仅维修单可修改！");
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
            LoadDataGrid(s);
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    if (obj.StatusID >= (int)EnumBillStatus.完成)
                    {
                        obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                        // 创建 ObjBillPrint 的实例
                        ObjBillPrint print = new ObjBillPrint();
                        print.obj = obj;
                        //开始打印
                        print.PrintStartRepair();
                    }
                    else
                    {
                        MessageBox.Show("仅完成之后的维修单可打印！");
                    }
                }
                else
                {
                    MessageBox.Show("仅维修单可打印！");
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
            LoadDataGrid(s);
        }

        private void BtnDeleteMark_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    if (obj.StatusID == (int)EnumBillStatus.保存)
                    {
                        DalBill.DeleteMark(obj);
                    }
                    else
                    {
                        MessageBox.Show("仅保存状态的维修单可作废！");
                    }
                }
                else
                {
                    MessageBox.Show("仅维修单可作废！");
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
            LoadDataGrid(s);
        }

        private void BtnRepair_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new WindowBillRepair());
            LoadDataGrid(s);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjBill obj = dataGrid1.SelectedItem as ObjBill;
                if (DalBill.GetSource(obj.BillGUID) == false)//检查是否已生成退库单
                {
                    ObjCar car = DalCar.GetObject(obj.CarGUID);
                    if (obj.TypeID == (int)EnumBillType.维修单 && car != null)
                    {
                        if (obj.StatusID == (int)EnumBillStatus.完成)
                        {
                            WindowBillBack child = new WindowBillBack();
                            child.IsAdd = true;
                            child.obj = obj;
                            child.obj.SourceGUID = child.obj.BillGUID;
                            child.obj.SourceCode = child.obj.BillCode;
                            child.obj.BillGUID = new Guid();
                            child.obj.BillCode = string.Empty;
                            child.obj.CarCode = car.CarCode;
                            child.obj.VIN = car.VIN;
                            child.obj.CarMileage = obj.TotalMileage;
                            child.obj.Mileage = 0 - obj.Mileage;
                            child.obj.TotalMileage = obj.TotalMileage + obj.Mileage;
                            child.obj.ListDetail = DalBill.GetDetailList(obj.SourceGUID);
                            if (BaseListClass.CheckNull(child.obj.ListDetail) == false)
                            {
                                foreach (ObjProduct i in child.obj.ListDetail)
                                {
                                    i.DetailGUID = new Guid();
                                    i.CreditNumber = 0 - i.CreditNumber;
                                    i.CreditAmount = 0 - i.CreditAmount;
                                    i.SalesNumber = 0 - i.SalesNumber;
                                    i.SalesAmount = 0 - i.SalesAmount;
                                    i.ChargeAmount = 0 - i.ChargeAmount;
                                    i.DiffAmount = 0 - i.DiffAmount;
                                }
                            }
                            child.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("仅完成状态的维修单可退库！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("仅维修单可退库！");
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("已有执行过退库操作，无法再次执行，请检查其他单据中的源单号：{0}！", obj.BillCode));
                }
            }
            else
            {
                MessageBox.Show("请选择一个完成状态的维修单，执行已出库备件的退库操作！");
            }
            LoadDataGrid(s);
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            new WindowBillMove().ShowDialog();
            LoadDataGrid(s);
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}