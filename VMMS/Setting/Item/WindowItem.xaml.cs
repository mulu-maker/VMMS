using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowItem.xaml 的交互逻辑
    /// </summary>
    public partial class WindowItem : Window
    {
        public ObjItem obj;//定义数据对象
        public bool IsAdd = true;

        public WindowItem()
        {
            InitializeComponent();
            obj = new ObjItem();
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
                    if (DalItem.Insert(obj) == true)
                    {
                        obj = new ObjItem();
                        this.DataContext = obj;
                        System.Windows.Input.Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalItem.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if (string.IsNullOrEmpty(obj.ItemCode) == false && string.IsNullOrEmpty(obj.ItemName) == false)
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
