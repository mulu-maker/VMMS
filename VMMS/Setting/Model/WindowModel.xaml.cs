using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowModel.xaml 的交互逻辑
    /// </summary>
    public partial class WindowModel : Window
    {
        public bool IsAdd = true;
        public ObjModel obj;//定义数据对象

        public WindowModel()
        {
            InitializeComponent();
            obj = new ObjModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = obj;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsNull() == false)
            {
                if (IsRepeat() == false)
                {
                    if (IsAdd == true)//新增模式
                    {
                        if (DalModel.Insert(obj) == true)
                        {
                            obj = new ObjModel();
                            this.DataContext = obj;
                            Keyboard.Focus(txtCode);
                        }
                    }
                    else//修改模式
                    {
                        if (DalModel.Update(obj) == true)
                        {
                            this.Close();
                        }
                    }
                }
            }
        }        

        private bool IsNull()
        {
            bool result = true;
            if ((string.IsNullOrWhiteSpace(obj.ModelCode) == false) && (string.IsNullOrWhiteSpace(obj.ModelName) == false))
            {
                result = false;
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.NotNull);
            }
            return result;
        }
        
        private bool IsRepeat()
        {
            bool result = true;
            if (DalModel.CheckRepeat(obj)==false)
            {
                result = false;
            }
            if(result==true)
            {
                if(MessageBox.Show("品牌型号有空值或重复值，是否继续保存？","提示",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    result = false;
                }
            }
            return result;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
