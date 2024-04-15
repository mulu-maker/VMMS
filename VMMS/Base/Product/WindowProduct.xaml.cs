using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowProduct.xaml 的交互逻辑
    /// </summary>
    public partial class WindowProduct : Window
    {
        public bool IsAdd = true;
        public ObjProduct obj;//定义数据对象
        public bool IsSelect = false;

        public WindowProduct()
        {
            InitializeComponent();
            obj = new ObjProduct();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalUnit.BindingComboBox(CboUnit);
            DalProductType.BindingComboBox(CboType);
            DalProductProperty.BindingComboBox(CboProperty);
            DalModel.BindingComboBox(CboModel);
            DalLocation.BindingComboBox(CboLocation);
            if (IsAdd == true)
            {
                obj.ProductCode = DalProduct.GetCode();
                CboProperty.IsHitTestVisible = true;
            }
            this.DataContext = obj;
            CboUnit.SelectedIndex = 0;
            CboType.SelectedIndex = 0;
            CboProperty.SelectedIndex = 0;
            if(IsSelect==false)
            {
                lbLocation.Visibility = Visibility.Collapsed;
                CboLocation.Visibility = Visibility.Collapsed;
            }
            LoadDataGrid();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (IsAdd == true)//新增模式
                {
                    if (DalProduct.Insert(obj,IsSelect) == true)
                    {
                        this.Close();
                    }
                }
                else//修改模式
                {
                    if (DalProduct.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if ((string.IsNullOrWhiteSpace(obj.ProductCode) == false) && (string.IsNullOrWhiteSpace(obj.ProductName) == false) && obj.UnitGUID != new Guid() && obj.TypeGUID != new Guid() && obj.PropertyID > 0 && CboUnit.SelectedItem != null && CboType.SelectedItem != null && CboProperty.SelectedItem != null)
            {
                result = false;
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.NotNull);
            }
            return result;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) == false)
            {
                obj.MnemonicCode = BaseStringClass.GetPinYinSuoXie(txtName.Text).Trim().ToLower();
                obj.MnemonicCode = Regex.Replace(obj.MnemonicCode, @"\s", "");//去除字符串全部空格
                txtMnemonicCode.Text = obj.MnemonicCode;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CboModel.SelectedItem != null)
            {
                ObjModel m = CboModel.SelectedItem as ObjModel;
                if (IsAdd == true)//新增模式
                {
                    if (obj.ListModel == null)
                        obj.ListModel = new List<ObjModel>();
                    if (DalModel.ExistGUID(obj.ListModel, m) == false)
                    {
                        obj.ListModel.Add(m);
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = obj.ListModel;
                    }
                    else
                    {
                        MessageBox.Show("已有车型，无需重复添加！");
                    }
                }
                else//修改模式
                {
                    if (obj.ProductGUID != new Guid())
                    {
                        if (DalProduct.InsertModel(obj.ProductGUID, m.ModelGUID) == true)
                        {
                            LoadDataGrid();
                        }
                        else
                        {
                            MessageBox.Show("车型添加失败，请查看是否重复添加适配车型！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("未保存的备件数据！");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择一个车型！");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (IsAdd == true)
                {
                    obj.ListModel.Remove(dataGrid1.SelectedItem as ObjModel);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = obj.ListModel;
                }
                else
                { 
                    ObjModel m = dataGrid1.SelectedItem as ObjModel;
                    if (DalProduct.DeleteModel(m) == true)
                    {
                        LoadDataGrid();
                    }


                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void LoadDataGrid()
        {
            if (obj.ProductGUID != new Guid())
            {
                dataGrid1.ItemsSource = DalProduct.GetModelList(obj.ProductGUID);//读取数据绑定dataGrid数据源并刷新datagrid
                //dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号           
            }
        }

        private void txtPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPrice.Value != null)
            {
                obj.Price = txtPrice.Value.Value;
            }
        }

        private void CboProperty_SelectionChanged()
        {

        }

        private void CboProperty_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void CboType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
