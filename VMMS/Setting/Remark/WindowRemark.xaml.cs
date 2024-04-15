using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowRemark.xaml 的交互逻辑
    /// </summary>
    public partial class WindowRemark : Window
    {
        public ObjRemark obj;//定义数据对象
        public bool IsAdd = true;

        public WindowRemark()
        {
            InitializeComponent();
            obj = new ObjRemark();
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
                    if (DalRemark.Insert(obj) == true)
                    {
                        obj = new ObjRemark();
                        this.DataContext = obj;
                        System.Windows.Input.Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalRemark.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if (string.IsNullOrEmpty(obj.RemarkCode) == false && string.IsNullOrEmpty(obj.RemarkName) == false)
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
