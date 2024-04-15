using System.Windows;

namespace VMMS
{

    /// <summary>
    /// WindowPassword.xaml 的交互逻辑
    /// </summary>
    public partial class WindowPassword : Window
    {
        public WindowPassword()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Input.Keyboard.Focus(pb);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pb.Password) == true)//判定文本框输入是否为空
            {
                MessageBox.Show(DalPrompt.InputPwd);
            }
            else
            {
                if (DalLogin.GetPwd().Equals(BaseEncryptClass.GetPwdEncrypt(pb.Password)) == true)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(DalPrompt.PwdFail);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
