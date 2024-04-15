using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentUserManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentUserManage : Window
    {
        public ContentUserManage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalUser.GetFullList();//查询符合条件的数据并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowUser().ShowDialog();
            LoadDataGrid();//刷新datagrid
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjUser obj = dataGrid1.SelectedItem as ObjUser;
                if (obj.DeleteMark == false)
                {
                    WindowUser child = new WindowUser();
                    child.obj = obj;
                    child.IsAdd = false;
                    child.ShowDialog();
                    LoadDataGrid();
                }
                else
                {
                    MessageBox.Show("已删除的数据禁止修改");
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnDeleteMark_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (DalUser.DeleteMark(dataGrid1.SelectedItem as ObjUser) == true)
                {
                    LoadDataGrid();
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = DalNPOI.ImportExcel();//获取导入Excel文件生成数据表
            string[] strArray = new string[] { "编号", "姓名", "联系电话" };//指定列名
            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件是否空白？缺少指定列？
            {
                if (CheckCode(dt) == false)//检测导入文件中的编号是否与已有编号重复
                {
                    List<ObjUser> l = new List<ObjUser>();//新建被导入数据集合
                    for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjUser对象
                    {
                        l.Add(new ObjUser { UserCode = dt.Rows[i]["编号"].ToString(), UserName = dt.Rows[i]["姓名"].ToString(), MobilePhone = dt.Rows[i]["联系电话"].ToString() });//数据集合增加数据
                    }
                    if (DalUser.Import(l) == true)//被导入数据集合保存到数据库
                    {
                        LoadDataGrid();//刷新UI
                    }
                }
            }
            else//被导入文件缺少指定列的提示
            {
                MessageBox.Show(DalPrompt.ImportTableNotColumn + string.Join(",", strArray));
            }
        }

        /// <summary>
        /// 检查DataTable中的编号是否已存在
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>bool</returns>
        public static bool CheckCode(DataTable dt)
        {
            bool result = false;
            IList<ObjUser> l = DalUser.GetViewList();//获取已有数据
            if (l != null)//无已有数据、无需对比检测是否已存在
            {
                if (BaseDataTable.CheckNull(dt) == false)
                {
                    string str = string.Empty;//定义提示字符串
                    for (int i = 0; i < dt.Rows.Count; i++)//循环检测是否已存在
                    {
                        string code = dt.Rows[i]["编号"].ToString().Trim();
                        int count = l.Count(p => p.UserCode == code);
                        if (count > 0)
                        {
                            str += "编号：" + code + "重复;";
                            result = true;
                        }
                    }
                    if (string.IsNullOrEmpty(str) == false)//有提示、弹窗显示
                    {
                        MessageBox.Show(str);
                    }
                }
            }
            return result;
        }

        private void DocumentContent_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
