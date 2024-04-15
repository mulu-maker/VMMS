using System.Text.RegularExpressions;
using System.Windows;


namespace VMMS
{
    /// <summary>
    /// WindowCustomer.xaml 的交互逻辑
    /// </summary>
    public partial class WindowCustomer : Window
    {
        public ObjCustomer obj;//定义数据对象
        public bool IsAdd = true;

        public WindowCustomer()
        {
            InitializeComponent();
            obj = new ObjCustomer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsAdd == true)
            {
                obj.CustomerCode = DalCustomer.GetCode();
            }
            this.DataContext = obj;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (IsAdd == true)//新增模式
                {
                    if (DalCustomer.Insert(obj) == true)
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else//修改模式
                {
                    if (DalCustomer.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if ((string.IsNullOrWhiteSpace(obj.CustomerCode) == false) && (string.IsNullOrWhiteSpace(obj.CustomerName) == false))
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
                obj.MnemonicCode = BaseStringClass.GetPinYinSuoXie(txtName.Text);
                obj.MnemonicCode = Regex.Replace(obj.MnemonicCode, @"\s", "");
                txtMnemonicCode.Text = obj.MnemonicCode;
            }
        }
    }
}
