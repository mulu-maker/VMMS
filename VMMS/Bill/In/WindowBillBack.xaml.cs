using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowBillBack.xaml 的交互逻辑
    /// </summary>
    public partial class WindowBillBack : Window
    {
        public bool IsAdd = true;
        public ObjBill obj;//定义单据对象

        public WindowBillBack()
        {
            InitializeComponent();
            obj = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalUser.BindingComboBox(CboUser);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 6);
           
            if (IsAdd == true)//新增模式
            {
                txtLicensePlate.IsHitTestVisible = false;
                obj.TypeID = (int)EnumBillType.退库单;
                this.Title = EnumBillType.退库单.ToString();
                obj.BillCode = DalBill.GetNewCode();
                obj.BillDate = DateTime.Now;
                if (obj.ListDetail == null)
                {
                    obj.ListDetail = new List<ObjProduct>();
                }
                Calc();
                this.DataContext = obj;
                LoadDataGrid();
            }
            else//查看模式
            {
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                DpDate.IsHitTestVisible = false;
                txtVIN.IsHitTestVisible = false;
                txtMobilePhone.IsHitTestVisible = false;
                BtnComplete.Visibility = Visibility.Collapsed;
                BtnCancel.Visibility = Visibility.Collapsed;
                CboUser.IsHitTestVisible = false;
                txtRemark.IsHitTestVisible = false; 
                ObjCar car = DalCar.GetObject(obj.CarGUID);
                obj.CarCode = car.CarCode;
                obj.VIN = car.VIN;
                this.DataContext = obj;
                LoadDataGrid();
            }
        }

        /// <summary>
        /// 计算合计
        /// </summary>
        private void Calc()
        {
            if (obj.ListDetail != null)
            {
                DalBill.Sum(obj);
                txtTotalDebitNumber.Text = obj.TotalDebitNumber.ToString();
                txtTotalDebitAmount.Text = obj.TotalDebitAmount.ToString("C");
                txtTotalSalesNumber.Text = obj.TotalSalesNumber.ToString();
                txtTotalSalesAmount.Text = obj.TotalSalesAmount.ToString("C");
            }
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = obj.ListDetail;
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (DalBill.Back(obj) == true)
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
                if (string.IsNullOrWhiteSpace(obj.BillCode) == false && string.IsNullOrWhiteSpace(obj.CarCode) == false && string.IsNullOrWhiteSpace(obj.LicensePlate) == false && obj.CarGUID != new Guid() && obj.UserGUID!=new Guid() && obj.BillDate > BaseDateTimeClass.BaseDate)
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
                if (DalBill.Back(obj) == true)
                {
                    Close();
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if(obj.SourceGUID!=new Guid())
            {
                ObjBill source = DalBill.GetObject(obj.SourceGUID);
                WindowBillRepair child = new WindowBillRepair();
                child.IsAdd = null;
                child.obj = source;
                child.ShowDialog();
            }
        }
    }
}