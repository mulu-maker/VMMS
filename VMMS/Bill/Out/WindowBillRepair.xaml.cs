using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowBillRepair.xaml 的交互逻辑
    /// </summary>
    public partial class WindowBillRepair : Window
    {
        public bool? IsAdd = true;
        public ObjBill obj;//定义单据对象

        public WindowBillRepair()
        {
            InitializeComponent();
            obj = new ObjBill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalUser.BindingComboBox(CboUser);
            DalItem.BindingComboBox(CboItem);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 3);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 4);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalLocation.BindingDataGridComboBoxColumn(dataGrid1, 6);
            if (IsAdd == true)//新增模式
            {
                obj.TypeID = (int)EnumBillType.维修单;
                this.Title = EnumBillType.维修单.ToString();
                obj.BillCode = DalBill.GetNewCode();
                obj.BillDate = DateTime.Now;
                obj.ListDetail = new List<ObjProduct>();
                this.DataContext = obj;
                System.Windows.Input.Keyboard.Focus(txtLicensePlate);
            }
            else if (IsAdd == false)//修改模式
            {
                DpDate.IsHitTestVisible = false;
                StackPanel2.IsHitTestVisible = false;
                StackPanel2a.Visibility = Visibility.Collapsed;
                StackPanel3a.IsHitTestVisible = false;//
                this.DataContext = obj;
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                LoadDataGrid();
            }
            else//查看模式
            {
                obj.ListDetail = DalBill.GetDetailList(obj.BillGUID);
                DpDate.IsHitTestVisible = false;
                ObjCar car = DalCar.GetObject(obj.CarGUID);
                obj.CarCode = car.CarCode;
                obj.VIN = car.VIN;
                StackPanel1.Visibility = Visibility.Collapsed;
                StackPanel2.IsHitTestVisible = false;
                StackPanel2a.Visibility = Visibility.Collapsed;
                StackPanel3.IsHitTestVisible = false;
                StackPanel4.IsHitTestVisible = false;
                spInsurance.IsHitTestVisible = false;
                spRemark.IsHitTestVisible = false;
                StackPanel6a.Visibility = Visibility.Hidden;
                spDiff.Visibility = Visibility.Hidden;
                this.DataContext = obj;
                LoadDataGrid();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
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
            p = DalProduct.GetProduct(TxtProduct);
            if(p!=null)
            {
                result = true;
            }
            return result;
        }

        private void AddDetail(ObjProduct p)
        {
            if (p != null)
            {
                if (CheckLocation(p)==true)
                {
                    if (DalProduct.CheckDetail(obj.ListDetail, p.ProductGUID, new Guid(), p.LocationGUID) == false)//检查备件、库位是否在单据明细重复
                    {
                        if (TxtNumber.Value!=null && TxtNumber.Value.Value>0)
                        {
                            decimal salesNumber = TxtNumber.Value.Value;
                            if (salesNumber > 0)
                            {
                                if (p.PropertyID == (int)EnumProductProperty.实物)//实物备件检测库存
                                {
                                    if (salesNumber <= p.InventoryNumber)
                                    {
                                        AddComplete(p, salesNumber, p.LocationGUID, p.InventoryNumber);
                                    }
                                    else
                                    {
                                        if (System.Windows.MessageBox.Show("此库位的备件数量小于出库数量，如继续出库会产生负库存，是否继续？", "库存提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                        {
                                            AddComplete(p, salesNumber, p.LocationGUID, p.InventoryNumber);
                                        }
                                    }
                                }
                                else if(p.PropertyID==(int)EnumProductProperty.服务)//服务备件直接添加
                                {
                                    AddComplete(p, salesNumber, p.LocationGUID, 0);
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
                    if (p.PropertyID == (int)EnumProductProperty.服务)
                    {
                        result = true;
                    }
                    else
                    {
                        if (MessageBox.Show("库位错误！请鼠标双击选择一行！", "错误提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            ContentInventoryList child = new ContentInventoryList();
                            child.IsSelect = true;
                            child.s.ProductCode = p.ProductCode;
                            child.cbZone.IsChecked = true;
                            child.ShowDialog();
                            if (child.result != null && child.result.LocationGUID != new Guid())
                            {
                                p.LocationGUID = child.result.LocationGUID;
                                if(CheckLocation(p)==true)
                                {
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
                else//有库存
                {
                    result = true;
                }
            }
            return result;
        }

        private void AddComplete(ObjProduct p, decimal creditNumber, Guid locationGUID, decimal inventoryNumber)
        {
            p.CreditNumber = creditNumber;
            p.CreditLocationGUID = locationGUID;
            p.SalesNumber = creditNumber;
            p.SalesPrice = p.Price;
            p.SalesAmount = p.SalesNumber * p.SalesPrice;
            p.ChargeAmount = p.SalesAmount;            
            p.InventoryNumber = inventoryNumber;
            obj.ListDetail.Add(p);
            TxtNumber.Text = string.Empty;
            TxtProduct.Text = string.Empty;
            System.Windows.Input.Keyboard.Focus(TxtNumber);
            LoadDataGrid();
            Keyboard.Focus(TxtNumber);
        }

        /// <summary>
        /// 计算合计
        /// </summary>
        //private void Calc()
        //{
            //DalBill.Sum(obj);
            //obj.TotalChargeAmount = obj.TotalSalesAmount;
            //obj.TotalDiffAmount = 0m;
            //this.DataContext = null;
            //this.DataContext = obj;
        //}

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
                if (string.IsNullOrWhiteSpace(obj.BillCode) == false && string.IsNullOrWhiteSpace(obj.CarCode) == false && string.IsNullOrWhiteSpace(obj.LicensePlate) == false && obj.CarGUID != new Guid() && obj.UserGUID != new Guid() && obj.CarMileage > 0 && obj.BillDate > BaseDateTimeClass.BaseDate)
                {
                    if (BaseListClass.CheckNull(obj.ListDetail) == false)
                    {
                        if (obj.Mileage > 0)
                        {
                            result = false;
                        }
                        else
                        {
                            if (MessageBox.Show("保存后车辆里程禁止修改，当前无行驶里程，是否继续？", "行驶里程计算提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                result = false;
                            }
                            else
                            {
                                MessageBox.Show("输入车辆里程后单击键盘回车键，计算行驶里程！");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(DalPrompt.InputEntry);
                    }
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.NotNullStar);
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

        private void TxtCar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetCar();
            }
        }

        private void GetCar()
        {
            if (txtLicensePlate.Text != null && string.IsNullOrEmpty(TxtCar.Text) == false)
            {
                ObjCar car = DalCar.GetObject(TxtCar.Text);//检查备件、库位是否在单据明细重复（不含库存属性）//必须唯一库位、不能分库位存放，否则不返回备件数据对象
                if (car != null)
                {
                    if (CheckCarBillDate(car) == false)
                    {
                        LoadCar(car);
                        TxtCar.Text = null;
                        Keyboard.Focus(TxtCarMileage);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("无此车辆！请输入正确的车牌号！");
                    TxtCar.Text = null;
                    Keyboard.Focus(TxtCar);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("请输入车牌号！");
                Keyboard.Focus(txtLicensePlate);
            }
        }

        private void BtnNewCar_Click(object sender, RoutedEventArgs e)
        {
            LoadCar(DalCar.InputNew());//检查备件、库位是否在单据明细重复（不含库存属性）//必须唯一库位、不能分库位存放，否则不返回备件数据对象

        }

        /// <summary>
        /// 检查30日内车辆是否有维修单
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        private bool CheckCarBillDate(ObjCar car)
        {
            bool result = false;
            if (car!=null && car.CarGUID!=new Guid() && obj.BillDate > BaseDateTimeClass.BaseDate)
            {
                int count = DalBill.GetCarDateCount(car, obj.BillDate.Date.AddDays(-30));
                if (count > 0)
                {
                    result = true;
                    if(MessageBox.Show("30日内此车辆已开具过维修单，是否继续？", "提示", MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                    {
                        result = false;
                    }
                }
            }
            return result;

        }
            private void LoadCar(ObjCar car)
        {
            if (car != null && car.CarGUID != new Guid())
            {
                obj.CustomerGUID = car.CustomerGUID;
                obj.CustomerName = car.CustomerName;
                obj.SendName = car.CustomerName;
                obj.MobilePhone = car.MobilePhone;
                obj.CarGUID = car.CarGUID;
                obj.CarCode = car.CarCode;
                obj.ModelGUID = car.ModelGUID;
                obj.ModelName = car.ModelName;
                obj.VIN = car.VIN;
                obj.LicensePlate = car.LicensePlate;
                obj.TotalMileage = car.TotalMileage;
                this.DataContext = null;
                this.DataContext = obj;
            }
        }
       
        private void TxtCarMileage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                obj.CarMileage = TxtCarMileage.Value.Value;
                obj.Mileage = obj.CarMileage - obj.TotalMileage;
                txtMileage.Text = obj.Mileage.ToString();
                Keyboard.Focus(CboUser);
            }
        }

        private void TxtCarMileage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (obj != null && TxtCarMileage.Value != null)
            {
                obj.CarMileage = TxtCarMileage.Value.Value;
            }
        }

        private void BtnGetCar_Click(object sender, RoutedEventArgs e)
        {
            GetCar();
        }

        private void BtnDiff_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (BaseListClass.CheckNull(obj.ListDetail) == false)
                {
                    if (TxtDiff.Value.Value > 0)
                    {
                        decimal charge = TxtDiff.Value.Value;
                        decimal sales = (dataGrid1.SelectedItem as ObjProduct).SalesAmount;
                        (dataGrid1.SelectedItem as ObjProduct).ChargeAmount = charge;
                        (dataGrid1.SelectedItem as ObjProduct).DiffAmount = sales-charge;
                        LoadDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("金额必须大于等于0！");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(DalPrompt.SelectRow);
                }
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e) 
        { 
            if (IsNull() == false)
            {
                if (IsAdd == true)//新增模式
                {
                    if (DalBill.RepairNew(obj, (int)EnumBillStatus.保存) == true)
                    {
                        Close();
                    }
                }
                else//保存模式
                {
                    if (DalBill.RepairUpdate(obj, (int)EnumBillStatus.保存) == true)
                    {
                        Close();
                    }
                }
            }
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                string remark = string.Empty;
                if (DalProduct.CheckCreditInventory(obj.ListDetail, ref remark) == true)
                {
                    if (DalBill.RepairComplete(obj, (int)EnumBillStatus.完成, IsAdd) == true)
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

        private void CboItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CboItem.SelectedItem!=null)
            {
                obj.ItemName = (CboItem.SelectedItem as ObjItem).ItemName;
            }
        }

        private void BtnContent_Click(object sender, RoutedEventArgs e)
        {
            ContentContentManage child = new ContentContentManage();
            child.IsSelected = true;
            child.ShowDialog();
            if(child.DialogResult==true)
            {
                string tmp = child.ResultString;
                obj.ItemContent= txtItemContent.Text + tmp;
                txtItemContent.Text = obj.ItemContent;
            }
        }

        private void BtnRemark_Click(object sender, RoutedEventArgs e)
        {
            ContentRemarkManage child = new ContentRemarkManage();
            child.IsSelected = true;
            child.ShowDialog();
            if (child.DialogResult == true)
            {
                string tmp = child.ResultString;
                obj.Remark= txtRemark.Text + tmp;
                txtRemark.Text = obj.Remark;
            }
        }

        private void TxtNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.Focus(TxtProduct);
            }
        }

        private void BtnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            WindowProduct child = new WindowProduct();
            child.IsSelect = true;
            child.ShowDialog();
        }

        private void BtnInsuranceCompany_Click(object sender, RoutedEventArgs e)
        {
            ContentInsuranceCompanyManage child = new ContentInsuranceCompanyManage();
            child.IsSelected = true;
            child.ShowDialog();
            if (child.DialogResult == true)
            {
                string tmp = child.ResultString;
                obj.InsuranceCompany = txtRemark.Text + tmp;
                txtInsuranceCompany.Text = obj.InsuranceCompany;
            }
        }

        private void txtItemContent_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}