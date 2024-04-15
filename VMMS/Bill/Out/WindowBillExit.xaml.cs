using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowBillExit.xaml 的交互逻辑
    /// </summary>
    public partial class WindowBillExit : Window
    {
        public bool IsAdd = true;
        public ObjBill obj;//定义单据对象

        public WindowBillExit()
        {
            InitializeComponent();
            obj = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalCustomer.BindingComboBox(CboCustomer);
            DalUser.BindingComboBox(CboUser);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 6);
            if (IsAdd == true)//新增模式
            {
                obj.TypeID = (int)EnumBillType.退货单;
                this.Title = EnumBillType.退货单.ToString();
                obj.BillCode = DalBill.GetNewCode();
                obj.BillDate = DateTime.Now;
                DpDate.IsHitTestVisible = false;
                CboCustomer.IsHitTestVisible = false;
                CboUser.IsHitTestVisible = false;
                txtRemark.IsHitTestVisible = false;
                Calc();
                this.DataContext = obj;
                LoadDataGrid();
            }
            else//查看模式
            {
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                DpDate.IsHitTestVisible = false;
                BtnComplete.Visibility = Visibility.Collapsed;
                BtnCancel.Visibility = Visibility.Collapsed;
                CboCustomer.IsHitTestVisible = false;
                CboUser.IsHitTestVisible = false;
                txtRemark.IsHitTestVisible = false; 
                Calc();
                this.DataContext = obj; 
                LoadDataGrid();
            }
        }

        /// <summary>
        /// 计算合计
        /// </summary>
        private void Calc()
        {
            DalBill.Sum(obj);
            txtTotalNumber.Text = obj.TotalDebitNumber.ToString();
            txtTotalAmount.Text = obj.TotalDebitAmount.ToString("C");
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = obj.ListDetail;
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
        }

        /// <summary>
        /// 检测必填项是否全部为空
        /// </summary>
        /// <returns></returns>
        private bool IsNull()
        {
            bool result = true;
            if (obj != null)
            {
                if (string.IsNullOrWhiteSpace(obj.BillCode) == false && obj.CustomerGUID != new Guid() && obj.BillDate > BaseDateTimeClass.BaseDate)
                {
                    if (BaseListClass.CheckNull(obj.ListDetail) == false)
                    {
                        result = false;
                    }
                    else
                    {
                        MessageBox.Show(DalPrompt.InputEntry);
                    }
                }
                else
                {
                    MessageBox.Show(DalPrompt.NotNullStar);
                }
            }
            return result;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (CheckInventory() == true)
                {
                    if (DalBill.Insert(obj, (int)EnumBillStatus.完成, true) == true)
                    {
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// 检查库存是否大于等于退货数量
        /// </summary>
        /// <returns></returns>
        bool CheckInventory()
        {
            bool result = true;
            if(obj!=null && BaseListClass.CheckNull(obj.ListDetail)==false)
            {
                foreach(ObjProduct i in obj.ListDetail)
                {
                    ObjProduct inventory = DalProduct.GetInventory(i.ProductGUID, i.DebitLocationGUID);
                    if(inventory!=null && inventory.InventoryNumber>0)
                    {
                        if(inventory.InventoryNumber < i.DebitNumber)
                        {
                            result = false;
                            MessageBox.Show(string.Format("备件编号：{0}，备件名称：{1}，即时库存：{2}在当前退货库位的库存数量少于退货数量，无法退货！",i.ProductCode,i.ProductName,inventory.InventoryNumber));
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("备件编号：{0}，备件名称：{1}在当前退货库位无库存，无法退货！", i.ProductCode, i.ProductName, inventory.InventoryNumber));
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (obj.SourceGUID != new Guid())
            {
                ObjBill source = DalBill.GetObject(obj.SourceGUID);
                WindowBillIn child = new WindowBillIn();
                child.IsAdd = false;
                child.obj = source;
                child.ShowDialog();
            }
        }
    }
}