using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// ContentCarManage.xaml 的交互逻辑
    /// </summary>
    public partial class ContentCarManage : Window
    {
        public ObjCar s;//定义查询对象
        IList<ObjCar> l;

        public ContentCarManage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DalModel.BindingComboBox(CboModel);
            DalModel.BindingDataGridComboBoxColumn(dataGrid1, 4);
            Clear();
        }

        private void Clear()
        {
            s = new ObjCar();
            DateTime d = DateTime.Now;
            s.DateStart = BaseDateTimeClass.GetCurrentMonthStart(d).Date;
            s.DateEnd = BaseDateTimeClass.GetCurrentMonthEnd(d);
            dataGrid1.ItemsSource = null;
            this.DataContext = s;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            l = DalCar.GetFullList(s);
            dataGrid1.ItemsSource = l;//查询符合条件的数据并刷新datagrid
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(BaseWindowClass.DataGrid_LoadingRow);//显示行号
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            new WindowCar().ShowDialog();
            LoadDataGrid();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                ObjCar obj = dataGrid1.SelectedItem as ObjCar;
                if (obj.DeleteMark == false)
                {
                    WindowCar child = new WindowCar();
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
                if (DalCar.DeleteMark(dataGrid1.SelectedItem as ObjCar) == true)
                {
                    LoadDataGrid();
                }
            }
            else
            {
                MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnReturnMark_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                if (DalCar.ReturnMark(dataGrid1.SelectedItem as ObjCar) == true)
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
            string colName = "车型";//指定列名
            DataTable dt = DalNPOI.ImportExcel();//获取导入Excel文件生成数据表
            string[] strArray = new string[] { "编号", "VIN", "车牌号", "总里程", colName,"发动机","排量","车身颜色", "生产日期", "车主", "手机号" };//指定列名
            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件是否空白？缺少指定列？
            {
                if (BaseDataTable.CheckRequired(dt, new string[] { strArray[0], strArray[1] }, ref remark) == true)//检查必填项是否全部都有内容
                {
                    if (CheckCode(dt) == false)//检测导入文件中的编号是否与已有编号重复
                    {                       
                        IList<ObjModel> models = DalModel.GetViewList();//获取车型数据集合
                        if (DalModel.Exist(dt, colName, models, ref remark) == true)//被导入文件中车型名称是否已存在？及DataTable是否为空？
                        {
                            List<ObjCar> l = new List<ObjCar>();//新建被导入数据集合
                            for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjCustomer对象
                            {
                                DateTime d = new DateTime();
                                if(DateTime.TryParse(dt.Rows[i]["生产日期"].ToString(),out d)==false)
                                {
                                    d = new DateTime();
                                }
                                ObjCar obj = new ObjCar { CarGUID = Guid.NewGuid(), CarCode = dt.Rows[i]["编号"].ToString(), VIN = dt.Rows[i]["VIN"].ToString(), LicensePlate = dt.Rows[i]["车牌号"].ToString(), TotalMileage = int.Parse(dt.Rows[i]["总里程"].ToString()), ModelGUID = DalModel.GetGUID(dt.Rows[i][colName].ToString(), models),EngineModel = dt.Rows[i]["发动机"].ToString(), EngineCapacity= dt.Rows[i]["排量"].ToString(), CarColor= dt.Rows[i]["车身颜色"].ToString(),ManufactureDate=d,CustomerName = dt.Rows[i]["车主"].ToString(), MobilePhone = dt.Rows[i]["手机号"].ToString() };//新建对象                                 
                                l.Add(obj);//数据集合增加数据
                            }
                            if (DalCar.Import(l) == true)//被导入数据集合保存到数据库
                            {
                                LoadDataGrid();//刷新UI
                            }
                        }
                        else
                        {
                            MessageBox.Show(remark);
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
            IList<ObjCar> l = DalCar.GetViewList(null);//获取已有数据
            if (l != null)//无已有数据、无需对比检测是否已存在
            {
                if (BaseDataTable.CheckNull(dt) == false)
                {
                    string str = string.Empty;//定义提示字符串
                    for (int i = 0; i < dt.Rows.Count; i++)//循环检测是否已存在
                    {
                        string code = dt.Rows[i]["VIN"].ToString().Trim();
                        int count = l.Count(p => p.CarCode == code);
                        if (count > 0)
                        {
                            str += "VIN：" + code + "重复;";
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

        private void BtnMileage_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem != null)
            {
                WindowCarMileage child = new WindowCarMileage();
                child.s = dataGrid1.SelectedItem as ObjCar;
                child.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show(DalPrompt.SelectRow);
            }
        }

        private void BtnCheckMileage_Click(object sender, RoutedEventArgs e)
        {
            if (BaseListClass.CheckNull(l)==false)
            {
                string remark = string.Empty;
                foreach(ObjCar i in l)
                {
                    IList<ObjBill> l = DalBill.GetCarList(i);
                    remark = remark + DalCar.CheckMileage(l);
                }
                if(string.IsNullOrEmpty(remark)==true)
                {
                    MessageBox.Show("车辆里程重算正确！");
                }
                else
                {
                    MessageBox.Show(remark);
                }
            }
        }
    }
}
