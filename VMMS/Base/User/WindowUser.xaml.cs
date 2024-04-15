using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowUser.xaml 的交互逻辑
    /// </summary>
    public partial class WindowUser : Window
    {
        public ObjUser obj;//定义数据对象
        public bool IsAdd = true;

        public WindowUser()
        {
            InitializeComponent();
            obj = new ObjUser();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtCode);//光标定位编号   
            this.DataContext = obj;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (IsAdd)//新增模式
                {
                    if (DalUser.Insert(obj))
                    {
                        this.Close();
                    }
                }
                else//更新模式
                {
                    if (DalUser.Update(obj))
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
            if (string.IsNullOrWhiteSpace(obj.UserCode) == false && string.IsNullOrWhiteSpace(obj.UserName) == false)
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
