using System;
using System.Windows;

namespace VMMS
{
    /// <summary>
    /// WindowChangePassword.xaml 的交互逻辑
    /// </summary>
    public partial class WindowChangePassword : Window
    {
        public WindowChangePassword()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string old = DalLogin.GetPwd();
            if (string.IsNullOrEmpty(old) == true) //无旧密码：为空或未设置旧密码
            {
                if (string.IsNullOrWhiteSpace(pwbNew1.Password) || string.IsNullOrWhiteSpace(pwbNew2.Password))//判定新密码文本框输入是否为空
                {
                    MessageBox.Show(DalPrompt.InputNewPwd);
                }
                else//新密码2个文本框均输入即保存密码
                {
                    if (SaveNewPwd(pwbNew1.Password, pwbNew2.Password) == true)
                    {
                        this.Close();
                    }
                }
            }
            else//有旧密码
            {
                if (string.IsNullOrWhiteSpace(pwbOld.Password) || string.IsNullOrWhiteSpace(pwbNew1.Password) || string.IsNullOrWhiteSpace(pwbNew2.Password))//判定文本框输入是否为空
                {
                    MessageBox.Show(DalPrompt.InputAll);
                }
                else//全部输入不为空
                {
                    if (old.Equals(pwbOld.Password) == true)//旧密码验证成功
                    {
                        SaveNewPwd(pwbNew1.Password, pwbNew2.Password);
                    }
                    else
                    {
                        MessageBox.Show(DalPrompt.PwdFail);
                    }
                }
            }
        }


        /// <summary>
        /// 保存新密码
        /// </summary>
        bool SaveNewPwd(string pwd1, string pwd2)
        {
            bool result = false;
            if (pwd1.Equals(pwd2) == true)
            {
                if (BaseFileClass.StringToFile(DalLogin.PwdFileName, BaseEncryptClass.GetPwdEncrypt(pwd1)) == true)
                {
                    MessageBox.Show(DalPrompt.PwdOK);
                    result = true;
                }
                else
                {
                    MessageBox.Show(DalPrompt.PwdFail);
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.PwdDisaccord);
            }
            return result;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //this.Close();
        }


    }
}
