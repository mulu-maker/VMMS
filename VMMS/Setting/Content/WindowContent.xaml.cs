using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowContent.xaml 的交互逻辑
    /// </summary>
    public partial class WindowContent : Window
    {
        public ObjContent obj;//定义数据对象
        public bool IsAdd = true;

        public WindowContent()
        {
            InitializeComponent();
            obj = new ObjContent();
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
                    if (DalContent.Insert(obj) == true)
                    {
                        obj = new ObjContent();
                        this.DataContext = obj;
                        System.Windows.Input.Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalContent.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if (string.IsNullOrEmpty(obj.ContentCode) == false && string.IsNullOrEmpty(obj.ContentName) == false)
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
