using System;
using System.Windows;
using System.Windows.Input;


namespace VMMS
{
    /// <summary>
    /// WindowDataBackup.xaml 的交互逻辑
    /// </summary>
    public partial class WindowDataBackup : Window
    {
        public WindowDataBackup()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtFolderName);
        }
        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            txtFolderName.Text = BaseWindowClass.GetSelectPath();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string path = @txtFolderName.Text;
            if (System.IO.Directory.Exists(path) == true)
            {
                string filePath = System.IO.Path.Combine(path, (DalDataConfig.SoftName + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak"));
                if (DalSQLite.Backup(filePath))
                {
                    this.Close();
                }
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
