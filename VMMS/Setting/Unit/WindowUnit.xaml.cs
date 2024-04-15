using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowUnit.xaml 的交互逻辑
    /// </summary>
    public partial class WindowUnit : Window
    {
        public bool IsAdd = true;
        public ObjUnit obj;

        public WindowUnit()
        {
            InitializeComponent();
            obj = new ObjUnit();
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
                    if (DalUnit.Insert(obj) == true)
                    {
                        obj = new ObjUnit();
                        this.DataContext = obj;
                        Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalUnit.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if ((string.IsNullOrWhiteSpace(obj.UnitCode) == false) && (string.IsNullOrWhiteSpace(obj.UnitName) == false))
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
