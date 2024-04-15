using System;
using System.Windows;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// WindowDataRestore.xaml 的交互逻辑
    /// </summary>
    public partial class WindowDataRestore : Window
    {
        public WindowDataRestore()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtFolderName);
        }

        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            txtFolderName.Text = BaseWindowClass.GetSelectFile("备份文件(*.bak)|*.bak");
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string path = @txtFolderName.Text;
            if (DalSQLite.Restore(path))
            {
                System.Environment.Exit(0);
            }
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
