using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowBillMove.xaml 的交互逻辑
    /// </summary>
    public partial class WindowBillMove : Window
    {
        public bool? IsAdd = true;
        public ObjBill obj;//定义单据对象

        public WindowBillMove()
        {
            InitializeComponent();
            obj = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalUser.BindingComboBox(CboUser);
            DalLocation.BindingComboBox(CboDebitLocation);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 6);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 7);
            if (IsAdd == true)//新增模式
            {
                obj.TypeID = (int)EnumBillType.调拨单;
                this.Title = EnumBillType.调拨单.ToString();
                obj.BillCode = DalBill.GetNewCode();
                obj.BillDate = DateTime.Now;
                obj.ListDetail = new List<ObjProduct>();
                this.DataContext = obj;
                System.Windows.Input.Keyboard.Focus(CboDebitLocation);
            }
            else if (IsAdd == false)//修改模式
            {
                DpDate.IsHitTestVisible = false;
                this.DataContext = obj;
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                LoadDataGrid();
            }
            else//查看模式
            {
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                DpDate.IsHitTestVisible = false;
                StackPanel2.IsHitTestVisible = false;
                StackPanel3.Visibility = Visibility.Hidden;
                this.DataContext = obj;
                LoadDataGrid();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CboDebitLocation.SelectedItem != null)
            {
                Add();
            }
            else
            {
                MessageBox.Show("请选择一个入库库位！");
            }
        }

        void Add()
        {
            ObjProduct p = null;
            if (GetProduct(ref p) == true)
            {
                AddDetail(p);
            }
        }

        /// <summary>
        /// 根据TxtProduct.Text返回备件对象
        /// </summary>
        /// <returns>ObjProduct</returns>
        private bool GetProduct(ref ObjProduct p)
        {
            bool result = false;
            p = DalProduct.GetProduct(TxtProduct,false);
            if (p != null)
            {
                result = true;
            }
            return result;
        }

        private void AddDetail(ObjProduct p)
        {
            if (p != null)
            {
                if (CheckLocation(p) == true)
                {
                    if (DalProduct.CheckDetail(obj.ListDetail, p.ProductGUID, new Guid(), p.LocationGUID) == false)//检查备件、库位是否在单据明细重复
                    {
                        if (TxtNumber.Value != null && TxtNumber.Value.Value > 0)
                        {
                            decimal num = TxtNumber.Value.Value;//获取调拨数量
                            if (num > 0)
                            {
                                if (p.PropertyID == (int)EnumProductProperty.实物)//实物备件检测库存
                                {
                                    if (num <= p.InventoryNumber)
                                    {
                                        AddComplete(p, num);
                                    }
                                    else
                                    {
                                        System.Windows.MessageBox.Show("库位数量小于调拨数量，无法继续调拨");
                                    }
                                }
                                else if (p.PropertyID == (int)EnumProductProperty.服务)//服务备件直接添加
                                {
                                    MessageBox.Show("非实物备件无需调拨");
                                }
                            }
                            else
                            {
                                MessageBox.Show("数量必须大于0！");
                            }
                        }
                        else
                        {
                            MessageBox.Show("数量不能为空！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("备件、库位重复！");
                    }
                }
            }
        }

        /// <summary>
        /// 根据TxtProduct.Text返回备件对象
        /// </summary>
        /// <returns>ObjProduct</returns>
        private bool CheckLocation(ObjProduct p)
        {
            bool result = false;
            if (p != null)
            {
                if (p.LocationGUID == new Guid())//无库存
                {
                    MessageBox.Show("无库存备件，无法调拨！");
                }
                else//有库存
                {
                    ObjLocation debit = CboDebitLocation.SelectedItem as ObjLocation;
                    if (debit != null && debit.LocationGUID != new Guid())
                    {
                        if (debit.LocationGUID != p.LocationGUID)
                        {
                            result = true;
                        }
                        else
                        {
                            MessageBox.Show("出库、入库库位不能相同！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("入库库位不能为空！");
                    }
                }
            }

            return result;
        }


        private void AddComplete(ObjProduct p, decimal debitNumber)
        {
            ObjLocation debit = CboDebitLocation.SelectedItem as ObjLocation;
            if (debit != null && debit.LocationGUID != new Guid())
            {
                p.InventoryCost = p.InventoryAmount / p.InventoryNumber;
                p.CreditNumber = debitNumber;
                p.CreditLocationGUID = p.LocationGUID;
                p.CreditCost = p.InventoryCost;
                p.DebitNumber = debitNumber;
                p.DebitLocationGUID = debit.LocationGUID;
                p.DebitCost = p.InventoryCost;
                if(p.InventoryNumber == debitNumber)
                {
                    p.DebitAmount = p.InventoryAmount;
                    p.CreditAmount = p.InventoryAmount;
                }
                else if(debitNumber < p.InventoryNumber)
                {
                    p.DebitAmount = p.InventoryCost * debitNumber ;
                    p.CreditAmount = p.InventoryCost * debitNumber;
                }
                obj.ListDetail.Add(p);
                TxtNumber.Text = string.Empty;
                TxtProduct.Text = string.Empty;
                System.Windows.Input.Keyboard.Focus(TxtNumber);
                LoadDataGrid();
                Keyboard.Focus(TxtNumber);
            }
        }

        private void LoadDataGrid()
        {
            DalBill.Sum(obj);
            this.DataContext = null;
            this.DataContext = obj;
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = obj.ListDetail;
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                int index = dataGrid1.SelectedIndex;
                if (index != -1)
                {
                    obj.ListDetail.RemoveAt(index);
                    LoadDataGrid();
                }
                else
                {
                    System.Windows.MessageBox.Show(DalPrompt.SelectRow);
                }
            }
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
                if (string.IsNullOrWhiteSpace(obj.BillCode) == false && obj.UserGUID != new Guid() && obj.BillDate > BaseDateTimeClass.BaseDate)
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

        private void TxtProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add();
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                string remark = string.Empty;
                if (DalProduct.CheckCreditInventory(obj.ListDetail, ref remark) == true)
                {
                    if (DalBill.Tranfser(obj) == true)
                    {
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show(remark);
                }
            }
        }

        private void TxtNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.Focus(TxtProduct);
            }
        }
    }
}