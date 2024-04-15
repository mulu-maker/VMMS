using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowProductType.xaml 的交互逻辑
    /// </summary>
    public partial class WindowProductType : Window
    {
        public bool IsAdd = true;
        public ObjProductType obj;//定义数据对象

        public WindowProductType()
        {
            InitializeComponent();
            obj = new ObjProductType();
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
                    if (DalProductType.Insert(obj) == true)
                    {
                        obj = new ObjProductType();
                        this.DataContext = obj;
                        Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalProductType.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if ((string.IsNullOrWhiteSpace(obj.TypeCode) == false) && (string.IsNullOrWhiteSpace(obj.TypeName) == false))
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
