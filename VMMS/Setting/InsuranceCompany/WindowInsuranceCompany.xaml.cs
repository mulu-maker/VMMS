using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowInsuranceCompany.xaml 的交互逻辑
    /// </summary>
    public partial class WindowInsuranceCompany : Window
    {
        public ObjInsuranceCompany obj;//定义数据对象
        public bool IsAdd = true;

        public WindowInsuranceCompany()
        {
            InitializeComponent();
            obj = new ObjInsuranceCompany();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = obj;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {

                if (IsAdd == true)//新增模式
                {
                    if (DalInsuranceCompany.Insert(obj) == true)
                    {
                        obj = new ObjInsuranceCompany();
                        this.DataContext = obj;
                        System.Windows.Input.Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalInsuranceCompany.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if (string.IsNullOrEmpty(obj.InsuranceCompanyCode) == false && string.IsNullOrEmpty(obj.InsuranceCompanyName) == false)
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
    }
}
