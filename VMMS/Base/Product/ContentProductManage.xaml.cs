using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentProductManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentProductManage : Window
    {
        ObjProduct s;//定义查询条件对象

        public ContentProductManage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalProductType.BindingComboBox(CboType);
            DalModel.BindingComboBox(CboModel);
            DalUnit.BindingDataGridComboBoxColumn(dataGrid1, 5);
            DalProductType.BindingDataGridComboBoxColumn(dataGrid1, 6);
            DalProductProperty.BindingDataGridComboBoxColumn(dataGrid1, 7);
            Clear();
        }

        private void Clear()
        {
            s = new ObjProduct();
            this.DataContext = s;
        }

        private void LoadDataGrid()
        {
            dataGrid1.ItemsSource = DalProduct.GetFullList(s);//读取数据绑定dataGrid数据源并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号           
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowProduct child = new WindowProduct();
            child.ShowDialog();
            LoadDataGrid();//刷新datagrid
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjProduct obj = dataGrid1.SelectedItem as ObjProduct;
                if (obj.DeleteMark == false)
                {
                    WindowProduct child = new WindowProduct();
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
                if (DalProduct.DeleteMark(dataGrid1.SelectedItem as ObjProduct) == true)
                {
                    LoadDataGrid();
                }
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnReturnMark_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (DalProduct.ReturnMark(dataGrid1.SelectedItem as ObjProduct) == true)
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
            MessageBox.Show("导入备件性质将全部设置为：实物！");
            string colType = "类别";//指定列名
            string colUnit = "计量单位";//指定列名
            DataTable dt = DalNPOI.ImportExcel();//获取导入Excel文件生成数据表
            string[] strArray = new string[] { "编号", "名称", "助记码", "条码", colType, colUnit, "单价" };//指定列名
            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件是否空白？缺少指定列？
            {
                string remark = string.Empty;//备注
                if (BaseDataTable.CheckRequired(dt, new string[] { strArray[0], strArray[1], strArray[4], strArray[5] }, ref remark) == true)//检查必填项是否全部都有内容
                {
                    if (CheckCode(dt) == false)//检测导入文件中的编号是否与已有编号重复
                    {
                        IList<ObjProductType> types = DalProductType.GetViewList();//获取类别数据集合
                        if (DalProductType.Exist(dt, colType, types, ref remark) == true)//被导入文件中类别名称是否已存在？及DataTable是否为空？
                        {
                            IList<ObjUnit> units = DalUnit.GetViewList();//获取计量单位数据集合
                            if (DalUnit.Exist(dt, colUnit, units, ref remark) == true)//被导入文件中计量单位名称是否已存在？及DataTable是否为空？
                            {
                                IList<ObjProduct> l = new List<ObjProduct>();//新建被导入数据集合
                                for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjCustomer对象
                                {
                                    ObjProduct obj = new ObjProduct { ProductGUID = Guid.NewGuid(), ProductCode = dt.Rows[i]["编号"].ToString(), ProductName = dt.Rows[i]["名称"].ToString(), MnemonicCode = dt.Rows[i]["助记码"].ToString(), Barcode = dt.Rows[i]["条码"].ToString(), UnitGUID = DalUnit.GetGUID(dt.Rows[i]["计量单位"].ToString(), units), TypeGUID = DalProductType.GetGUID(dt.Rows[i]["类别"].ToString(), types), PropertyID=(int)EnumProductProperty.实物, Price=Convert.ToDecimal(dt.Rows[i]["单价"].ToString()) };//新建对象
                                    if (string.IsNullOrEmpty(obj.MnemonicCode) == true)
                                    {
                                        obj.MnemonicCode = BaseStringClass.GetPinYinSuoXie(obj.ProductName);
                                    }
                                    l.Add(obj);//数据集合增加数据
                                }
                                if (DalProduct.Import(l) == true)//被导入数据集合保存到数据库
                                {
                                    LoadDataGrid();//刷新UI
                                }
                            }
                            else
                            {
                                MessageBox.Show("请检查是否缺少以下计量单位设置：" + remark);
                            }
                        }
                        else
                        {
                            MessageBox.Show("请检查是否缺少以下备件类别设置：" + remark);
                        }
                    }
                    else//编号重复提示
                    {
                        MessageBox.Show("请检查编号重复！");
                    }
                }
                else//必填项空白提示
                {
                    MessageBox.Show("请检查必填项的是否有空白内容！");
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
            IList<ObjProduct> l = DalProduct.GetFullList(null);//获取全部数据
            if (l != null)//无已有数据、无需对比检测是否已存在
            {
                if (BaseDataTable.CheckNull(dt) == false)
                {
                    string str = string.Empty;//定义提示字符串
                    for (int i = 0; i < dt.Rows.Count; i++)//循环检测是否已存在
                    {
                        string code = dt.Rows[i]["编号"].ToString().Trim();
                        int count = l.Count(p => p.ProductCode == code);
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
    }
}
