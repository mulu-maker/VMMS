using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowLocation.xaml 的交互逻辑
    /// </summary>
    public partial class WindowLocation : Window
    {
        public ObjLocation obj;
        public bool IsAdd = true;

        public WindowLocation()
        {
            InitializeComponent();
            obj = new ObjLocation();
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
                    if (DalLocation.Insert(obj) == true)
                    {
                        obj = new ObjLocation();
                        this.DataContext = obj;
                        System.Windows.Input.Keyboard.Focus(txtCode);
                    }
                }
                else//修改模式
                {
                    if (DalLocation.Update(obj) == true)
                    {
                        this.Close();
                    }
                }
            }
        }

        private bool IsNull()
        {
            bool result = true;
            if (string.IsNullOrEmpty(obj.LocationCode) == false && string.IsNullOrEmpty(obj.LocationName) == false)
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
