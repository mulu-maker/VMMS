using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentCustomerManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentCustomerManage : Window
    {
        ObjCustomer s;//定义查询条件对象

        public ContentCustomerManage()
        {
            InitializeComponent();
            s = new ObjCustomer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            s = new ObjCustomer();
            this.DataContext = s;
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalCustomer.GetFullList(s);//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号            
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowCustomer().ShowDialog();
            LoadDataGrid();//刷新datagrid
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjCustomer obj = dataGrid1.SelectedItem as ObjCustomer;
                if (obj.DeleteMark == false)
                {
                    WindowCustomer child = new WindowCustomer();
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
                System.Windows.MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnDeleteMark_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (DalCustomer.DeleteMark(dataGrid1.SelectedItem as ObjCustomer) == true)
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
            string remark = string.Empty;
            DataTable dt = DalNPOI.ImportExcel();//获取导入Excel文件生成数据表
            string[] strArray = new string[] { "编号", "名称", "助记码", "联系电话", "电子邮件", "联系地址", "联系人", "手机号", "开户银行", "银行账号", "税号" };//指定列名
            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件是否空白？缺少指定列？
            {
                if (BaseDataTable.CheckRequired(dt, new string[] { strArray[0], strArray[1] }, ref remark) == true)//检查必填项是否全部都有内容
                {
                    if (CheckCode(dt) == false)//检测导入文件中的编号是否与已有编号重复
                    {
                        List<ObjCustomer> l = new List<ObjCustomer>();//新建被导入数据集合
                        for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjCustomer对象
                        {
                            ObjCustomer obj = new ObjCustomer { CustomerGUID = Guid.NewGuid(), CustomerCode = dt.Rows[i]["编号"].ToString(), CustomerName = dt.Rows[i]["名称"].ToString(), MnemonicCode = dt.Rows[i]["助记码"].ToString(), Phone = dt.Rows[i]["联系电话"].ToString(), Email = dt.Rows[i]["电子邮件"].ToString(), LinkAddress = dt.Rows[i]["联系地址"].ToString(), LinkMan = dt.Rows[i]["联系人"].ToString(), MobilePhone = dt.Rows[i]["手机号"].ToString(), BankName = dt.Rows[i]["开户银行"].ToString(), BankAccount = dt.Rows[i]["银行账号"].ToString(), TaxNumber = dt.Rows[i]["税号"].ToString() };//新建对象  
                            if (string.IsNullOrEmpty(obj.MnemonicCode) == true)
                            {
                                obj.MnemonicCode = BaseStringClass.GetPinYinSuoXie(obj.CustomerName);
                            }
                            l.Add(obj);//数据集合增加数据
                        }
                        if (DalCustomer.Import(l) == true)//被导入数据集合保存到数据库
                        {
                            LoadDataGrid();//刷新UI
                        }
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
            IList<ObjCustomer> l = DalCustomer.GetFullList(null);//获取已有数据
            if (l != null)//无已有数据、无需对比检测是否已存在
            {
                if (BaseDataTable.CheckNull(dt) == false)
                {
                    string str = string.Empty;//定义提示字符串
                    for (int i = 0; i < dt.Rows.Count; i++)//循环检测是否已存在
                    {
                        string code = dt.Rows[i]["编号"].ToString().Trim();
                        int count = l.Count(p => p.CustomerCode == code);
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

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
    }
}
