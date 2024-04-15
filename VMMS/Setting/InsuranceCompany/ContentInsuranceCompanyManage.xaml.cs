using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace VMMS
{
    /// <summary>
    /// ContentInsuranceCompanyManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentInsuranceCompanyManage : Window
    {
        public bool IsSelected = false;
        public string ResultString = string.Empty;

        public ContentInsuranceCompanyManage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalInsuranceCompany.GetViewList();//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号           
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowInsuranceCompany().ShowDialog();
            LoadDataGrid();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjInsuranceCompany obj = dataGrid1.SelectedItem as ObjInsuranceCompany;
                WindowInsuranceCompany child = new WindowInsuranceCompany();
                child.IsAdd = false;
                child.obj = obj;
                child.ShowDialog();
                LoadDataGrid();
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnDeleteMark_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (DalInsuranceCompany.DeleteMark(dataGrid1.SelectedItem as ObjInsuranceCompany) == true)
                {
                    LoadDataGrid();
                }
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = DalNPOI.ImportExcel();//获取导入Excel文件生成数据表
            string[] strArray = new string[] { "编号", "名称" };//指定列名

            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件缺少指定列？
            {
                if (CheckCode(dt) == false)//检测导入文件中的编号是否与已有编号重复
                {
                    List<ObjInsuranceCompany> l = new List<ObjInsuranceCompany>();//新建被导入数据集合
                    for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjInsuranceCompany对象
                    {
                        ObjInsuranceCompany obj = new ObjInsuranceCompany { InsuranceCompanyGUID = Guid.NewGuid(), InsuranceCompanyCode = dt.Rows[i]["编号"].ToString(), InsuranceCompanyName = dt.Rows[i]["名称"].ToString() };//新建对象
                        l.Add(obj);//数据集合增加数据
                    }
                    if (DalInsuranceCompany.Import(l) == true)//被导入数据集合保存到数据库
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
        private static bool CheckCode(DataTable dt)
        {
            bool result = false;
            IList<ObjInsuranceCompany> l = DalInsuranceCompany.GetViewList();//获取已有数据
            if (BaseListClass.CheckNull(l) == false)//无已有数据、无需对比检测是否已存在
            {
                if (BaseDataTable.CheckNull(dt) == false)
                {
                    string str = string.Empty;//定义提示字符串
                    for (int i = 0; i < dt.Rows.Count; i++)//循环检测是否已存在
                    {
                        string code = dt.Rows[i]["编号"].ToString().Trim();
                        int count = l.Count(p => p.InsuranceCompanyCode == code);
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

        private void dataGrid1_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsSelected == true)
            {
                if (dataGrid1.SelectedItem != null)
                {
                    ResultString = (dataGrid1.SelectedItem as ObjInsuranceCompany).InsuranceCompanyName;
                    this.DialogResult = true;
                }
            }
        }
    }
}
