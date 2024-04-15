using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VMMS
{
    /// <summary>
    /// 窗体操作类
    /// </summary>
    public class BaseWindowClass
    {
        public const string ExcelFile = "表格文件(*.xls,*.xlsx)|*.xls;*.xlsx";

        /// <summary>
        /// DataGrid行头自动增加行号
        /// </summary>
        public static void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        /// <summary>
        /// 返回选择的文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="IsMultiselect"></param>
        /// <returns></returns>
        public static string GetSelectFile(string filter)
        {
            string result = string.Empty;
            Microsoft.Win32.OpenFileDialog fileDialog1 = new Microsoft.Win32.OpenFileDialog();
            //fileDialog1.InitialDirectory =Directory.GetCurrentDirectory();//初始目录
            //fileDialog1.Filter = "打印模版(*.rdlc)|*.rdlc""图片文件(*.jpg)|*.jpg"
            fileDialog1.Filter = filter;//过滤文件的类型
            fileDialog1.Multiselect = false;//多选文件
            fileDialog1.FilterIndex = 1;
            fileDialog1.RestoreDirectory = false;//返回目录
            if (fileDialog1.ShowDialog() == true)
            {
                result = fileDialog1.FileName;
            }
            return result;
        }

        /// <summary>
        /// 返回选择的路径
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="IsMultiselect"></param>
        /// <returns></returns>
        public static string GetSelectPath()
        {
            string result = string.Empty;
            System.Windows.Forms.FolderBrowserDialog fileDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            fileDialog1.ShowDialog();
            if (fileDialog1.SelectedPath != string.Empty)
            {
                result = fileDialog1.SelectedPath;
            }
            return result;
        }

        /// <summary>
        /// 启动窗体
        /// </summary>
        /// <param name="child">窗体对象</param>
        public static void OpenActionWindow(Window child)
        {
            if (string.IsNullOrEmpty(DalLogin.GetPwd()) == true)
            {
                child.ShowDialog();
            }
            else
            {
                WindowPassword wp = new WindowPassword();
                wp.ShowDialog();
                bool? result = wp.DialogResult;
                if (result == true)
                {
                    child.ShowDialog();
                }
                else
                {
                    System.Windows.MessageBox.Show("密码验证未通过，无法使用授权功能！");
                }
            }
        }

        /// <summary>
        /// 退出当前应用程序
        /// 2022-04-23
        /// （自动备份）
        /// </summary>
        public static void AppExit()
        {
            if (System.Windows.MessageBox.Show("确定退出程序吗?", "退出", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                if (DalSQLite.Autobackup() == true)
                {
                    System.Environment.Exit(0);
                }
            }
        }
    }
}
