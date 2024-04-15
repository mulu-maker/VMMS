using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace VMMS
{
    /// <summary>
    /// ContentModelManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentModelManage : Window
    {
        public ContentModelManage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalModel.GetViewList();//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号            
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowModel().ShowDialog();
            LoadDataGrid();//刷新datagrid
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                WindowModel child = new WindowModel();
                child.IsAdd = false;
                child.obj = dataGrid1.SelectedItem as ObjModel;
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
                if (DalModel.DeleteMark(dataGrid1.SelectedItem as ObjModel) == true)
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
            string[] strArray = new string[] { "编号", "品牌型号" };//指定列名
            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件缺少指定列？
            {
                if (BaseDataTable.CheckNull(dt) == false)//检查DataTable是否为空
                {
                    if (CheckCode(dt) == false)//检测导入文件中的编号\名称是否与已有编号\名称重复
                    {
                        List<ObjModel> l = new List<ObjModel>();//新建被导入数据集合
                        for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjCompany对象
                        {
                            ObjModel obj = new ObjModel { ModelGUID = Guid.NewGuid(), ModelCode = dt.Rows[i]["编号"].ToString(), ModelName = dt.Rows[i]["品牌型号"].ToString() };//新建对象
                            l.Add(obj);//数据集合增加数据
                        }
                        if (DalModel.Import(l) == true)//被导入数据集合保存到数据库
                        {
                            LoadDataGrid();//刷新UI
                        }
                    }
                }
                else
                {
                    MessageBox.Show(DalPrompt.NotDataNotImport);//被导入数据集合保存失败提示
                }
            }
            else//被导入文件缺少指定列的提示
            {
                MessageBox.Show("被导入文件中至少需要有以下列名：" + string.Join(",", strArray));
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
            IList<ObjModel> l = DalModel.GetFullList();//获取已有数据
            if (l != null)//无已有数据、无需对比检测是否已存在
            {
                if (BaseDataTable.CheckNull(dt) == false)
                {
                    string str = string.Empty;//定义提示字符串
                    for (int i = 0; i < dt.Rows.Count; i++)//循环检测是否已存在
                    {
                        string code = dt.Rows[i]["编号"].ToString().Trim();
                        string name = dt.Rows[i]["品牌型号"].ToString().Trim();
                        int count = l.Count(p => p.ModelCode == code || p.ModelName == name);
                        if (count > 0)
                        {
                            str += "编号：" + code + "或品牌型号：" + name + "重复;";
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

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                WindowModelProduct child = new WindowModelProduct();
                child.g = (dataGrid1.SelectedItem as ObjModel).ModelGUID;
                child.ShowDialog();
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }
    }
}
