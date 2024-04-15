using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowBillIn.xaml 的交互逻辑
    /// </summary>
    public partial class WindowBillIn : Window
    {
        public bool? IsAdd = true;
        public ObjBill obj;//定义单据对象

        public WindowBillIn()
        {
            InitializeComponent();
            obj = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalCustomer.BindingComboBox(CboCustomer);
            DalUser.BindingComboBox(CboUser);
            DalLocation.BindingComboBox(CboDebitLocation);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 6);
            if (IsAdd == true)//新增模式
            {
                obj.TypeID = (int)EnumBillType.入库单;
                this.Title = EnumBillType.入库单.ToString();
                obj.BillCode = DalBill.GetNewCode();
                obj.BillDate = DateTime.Now;
                obj.ListDetail = new List<ObjProduct>();
                System.Windows.Input.Keyboard.Focus(TxtNumber);
                CboDebitLocation.SelectedIndex = 0;
                this.DataContext = obj;
            }
            else //查看模式(含false\null)
            {
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                DpDate.IsHitTestVisible = false;
                BtnComplete.Visibility = Visibility.Collapsed;
                BtnCancel.Visibility = Visibility.Collapsed;
                spProduct.Visibility = Visibility.Collapsed;
                CboCustomer.IsHitTestVisible = false;
                CboUser.IsHitTestVisible = false;
                txtRemark.IsHitTestVisible = false;
                this.DataContext = obj;
                LoadDataGrid();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddDetail(DalProduct.GetProduct(TxtProduct));
        }

        /// <summary>
        /// 增加备件明细数据
        /// 2022-03-25
        /// </summary>
        /// <param name="Product">ObjProduct</param>
        private void AddDetail(ObjProduct product)
        {
            if (product != null)
            {
                if (CboDebitLocation.SelectedItem != null)//(CheckLocation(product))//检查备件存放库位
                {
                    Guid debitLocationGUID = (CboDebitLocation.SelectedItem as ObjLocation).LocationGUID;
                    if (DalProduct.CheckDetail(obj.ListDetail, product.ProductGUID, debitLocationGUID, new Guid()) == false) //(DalProduct.CheckGuid(obj.ListDetail, product.ProductGUID) == false)//检查备件、库位是否与单据明细重复
                    {
                        if (string.IsNullOrEmpty(TxtNumber.Text) == false)
                        {
                            decimal debitNumber = Convert.ToDecimal(TxtNumber.Text);
                            if (debitNumber > 0)
                            {
                                product.DebitNumber = debitNumber;
                                product.DebitAmount = Convert.ToDecimal(TxtAmount.Text);
                                product.DebitCost = product.DebitAmount / product.DebitNumber;
                                product.DebitLocationGUID = debitLocationGUID;
                                obj.ListDetail.Add(product);
                                TxtNumber.Text = string.Empty;
                                TxtAmount.Text = string.Empty;
                                TxtProduct.Text = string.Empty;
                                System.Windows.Input.Keyboard.Focus(TxtProduct);
                                Calc();
                                LoadDataGrid();
                                Keyboard.Focus(TxtNumber);
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
                else
                {
                    MessageBox.Show("请先选择库位！");
                }
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

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                int index = dataGrid1.SelectedIndex;
                if (index != -1)
                {
                    obj.ListDetail.RemoveAt(index);
                    Calc();
                    LoadDataGrid();
                }
                else
                {
                    System.Windows.MessageBox.Show(DalPrompt.SelectRow);
                }
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (DalBill.Insert(obj,(int)EnumBillStatus.完成,false) == true)
                {
                    Close();
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

        private void BtnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            AddDetail(DalProduct.NewInput());
        }

        private void DpDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsAdd==true)
            {
                //DalBill.CheckDate(obj);
            }
        }

        private void TxtAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.Focus(TxtProduct);
            }
        }

        private void TxtProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddDetail(DalProduct.GetProduct(TxtProduct));
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (DalBill.Insert(obj, (int)EnumBillStatus.完成,false) == true)
                {
                    Close();
                }
            }
        }

        private void TxtNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.Focus(TxtAmount);
            }
        }

        private void TxtProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}