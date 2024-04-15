using System;
using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowCar.xaml 的交互逻辑
    /// </summary>
    public partial class WindowCar : Window
    {
        public ObjCar obj;//定义数据对象
        public bool IsAdd = true;

        public WindowCar()
        {
            InitializeComponent();
            obj = new ObjCar();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalModel.BindingComboBox(cboModel);
            if (IsAdd == true)
            {
                obj.CarCode = DalCar.GetCode();
            }
            this.DataContext = obj;
            Keyboard.Focus(txtCode);//光标定位在编号   
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if(txtTotalMileage.Value!=null)
            {
                obj.TotalMileage = txtTotalMileage.Value.Value;
            }
            if (IsAdd)//新增模式
            {
                if (IsNull() == false)
                {
                    if (DalCar.Insert(obj))
                    {
                        this.Close();
                    }
                }
            }
            else//更新模式
            {
                if (IsNull() == false)
                {
                    if (DalCar.Update(obj))
                    {
                        this.Close();
                    }
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
            if (string.IsNullOrWhiteSpace(obj.CarCode) == false && string.IsNullOrWhiteSpace(obj.LicensePlate) == false)
            {
                result = false;
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
    }
}
