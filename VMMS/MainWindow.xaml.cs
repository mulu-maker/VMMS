using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace VMMS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetConfig(); 
            IList<ObjBill> l = DalBill.GetNotCompleteList();
            if (BaseListClass.CheckNull(l) == false)
            {
                txtNotComplete.Text = "超期1月未完成提醒：" + l.Count;
            }
            RefreshKanbanData();
            WebApi webApi = new WebApi();
            webApi.StartWebApi();
        }

        private void GetConfig()
        {
            var storeSetting = StoreSetting.Instance;
            if (DalSQLite.CheckDb() == true)//检测数据库文件是否存在\正常访问
            {
                try
                {
                    string ver = "数据库异常";
                    if (DalSQLite.GetVersion(ref ver) == true)//读取数据版本号并检测是否与软件要求的数据库版本一致
                    {
                        labelLoginName.Content = "软件版本：" + DalDataConfig.SoftVerion + "    数据版本：" + ver;
                        DalLogin.LoginedUser = new ObjUser();//初始化登录员工
                    }
                    DalSQLite.GetAutobakcupPath();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
        }

        private void MiManagePwd_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new WindowChangePassword());
        }

        private void MiDataBackup_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new WindowDataBackup());
        }

        private void MiDataRestore_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new WindowDataRestore());
        }

        private void MiExit_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.AppExit();
        }

        private void MiCustomerManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentCustomerManage());
        }

        private void MiBillManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentBillManage());
        }

        private void MiInventoryList_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentInventoryList());
        }

        private void MiUnitManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentUnitManage());
        }

        private void MiProductTypeManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentProductTypeManage());
        }

        private void MiProductManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentProductManage());
        }

        private void MiLocationManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentLocationManage());
        }

        private void BtnIn_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new WindowBillIn());
        }

        private void BtnRepair_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new WindowBillRepair());
        }

        private void BtnBillManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentBillManage());
        }

        private void MiBillProductList_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentProductInOutList());
        }

        private void MiUserManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentUserManage());
        }

        private void MiModelManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentModelManage());
        }

        private void MiCarManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentCarManage());
        }

        private void MiCarLog_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentCarLog());
        }

        private void MiCarNext_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentCarNext());
        }

        private void MiCarDate_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentCarDate());
        }

        private void MiInsuranceNext_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentInsuranceNext());
        }

        private void MiItemManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentItemManage());
        }

        private void MiContentManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentContentManage());
        }

        private void MiInsuranceCompanyManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentInsuranceCompanyManage());
        }

        private void MiRemarkManage_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentRemarkManage());
        }
        private void StoreSetup_Click(object sender, RoutedEventArgs e)
        {
            //打开门店设置
            BaseWindowClass.OpenActionWindow(new WindowStore());

        }

        private void MiAboutBox_Click(object sender, RoutedEventArgs e)
        {
            new WindowAbout().ShowDialog();
        }

        /// <summary>
        /// 库存重算
        /// 用于即时库存表出现错误结果时，删除现有库存表数据、并根据出入库单据重新计算即时库存表数据并更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiCalc_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("库存重算结果不可恢复，确定开始进行库存重算？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<string> l = new List<string>();
                IList<ObjBill> bills = DalBill.GetCalcList();
                if (BaseListClass.CheckNull(bills) == false)
                {
                    bills = bills.Where(p => p.StatusID > (int)EnumBillStatus.保存).ToList();
                    l.Add(string.Format("Delete FROM crs_inventory"));//删除即时库存表
                    foreach (ObjBill i in bills)
                    {
                        i.ListDetail = DalBill.GetDetailList(i.BillGUID);
                        bool isOut = true;//定义减少库存参数
                        if (i.TypeID == (int)EnumBillType.入库单 || i.TypeID == (int)EnumBillType.退库单)
                        {
                            isOut = false;
                        }
                        DalBill.AddUpdateInventorySqlString(i, l, isOut);
                    }
                }
                DalSQLite.Complete(l);
            }
        }

        private void MiSalesCostList_Click(object sender, RoutedEventArgs e)
        {
            BaseWindowClass.OpenActionWindow(new ContentSalesCostList());
        }

        private void MiRepeat_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = DalNPOI.ImportExcel();//获取导入Excel文件生成数据表
            string[] strArray = new string[] { "唯一", "替换" };//指定列名
            if (BaseDataTable.CheckColumns(dt, strArray) == true)//检查被导入文件是否空白？缺少指定列？
            {
                List<string> l = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)//由Row循环增加ObjUser对象
                {
                    string code = BaseStringClass.NumberToLengthString(int.Parse(dt.Rows[i]["唯一"].ToString()), DalDataConfig.ProductCodeWidth);//获取唯一备件编号
                    ObjProduct p = GetLocation(code);//从数据库获取唯一备件对象
                    if (p != null)
                    {
                        string repeat = dt.Rows[i]["替换"].ToString();//获取被替换的备件编号字符串（规则最后几位非0数字，以","间隔）
                        if (string.IsNullOrEmpty(repeat) == false)//有需要替换的备件
                        {
                            string[] array = repeat.Split(Convert.ToChar(","));//被替换备件编号数组
                            if (array.Length > 0)//有需要替换的备件
                            {
                                for (int j = 0; j < array.Length; j++)//循环添加SQl语句
                                {
                                    string jcode = BaseStringClass.NumberToLengthString(int.Parse(array[j]), DalDataConfig.ProductCodeWidth);//生成被替换备件编号
                                    ObjProduct jp = GetObject(jcode);//从数据库获取唯一被替换备件对象
                                    if (jp != null)//有
                                    {
                                        if (DalSQLite.GetDataRecordCount(string.Format("SELECT COUNT(*) FROM crs_bill_detail WHERE ProductGUID='{0}'", jp.ProductGUID)) > 0)//单据明细表有可替换的备件资料
                                        {
                                            l.Add(string.Format("UPDATE crs_bill_detail SET ProductGUID='{0}',ProductCode='{1}',ProductName='{2}',UnitGUID='{3}',TypeGUID='{4}',PropertyID={5} WHERE ProductGUID='{6}'", p.ProductGUID, p.ProductCode, p.ProductName, p.UnitGUID, p.TypeGUID, p.PropertyID, jp.ProductGUID));//替换单据明细表中备件资料
                                        }
                                        IList<ObjProduct> inven = DalSQLite.GetIList<ObjProduct>(string.Format("SELECT * FROM crs_inventory WHERE (ProductGUID='{0}' AND InventoryNumber<>0) OR (ProductGUID='{0}' AND InventoryAmount<>0)", jp.ProductGUID));
                                        if (BaseListClass.CheckNull(inven)==false)//即时库存表有可替换的备件库存
                                        {
                                            foreach (ObjProduct tmp in inven)//防止有多个库位
                                            {
                                                l.Add(string.Format("UPDATE crs_inventory SET InventoryNumber=InventoryNumber+{1},InventoryAmount=InventoryAmount+{2} WHERE ProductGUID='{0}'", p.ProductGUID, tmp.InventoryNumber, tmp.InventoryAmount));//循环更新增加唯一备件库存
                                                l.Add(string.Format("DELETE FROM crs_inventory WHERE ProductGUID='{0}' ", tmp.ProductGUID));//删库被替换备件即时库存表数据
                                            }
                                        }
                                        l.Add(string.Format("UPDATE crs_product SET DeleteMark=1 WHERE ProductGUID='{0}' ", jp.ProductGUID));//作废被替换备件
                                    }
                                    else//无：清空SQL语句，终止循环
                                    {
                                        MessageBox.Show("被替换的：" + jcode + "没有查找到对应的备件！");
                                        l = new List<string>();
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("唯一的：" + dt.Rows[i]["唯一"].ToString() + "没有查找到对应的备件！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("唯一的：" + dt.Rows[i]["唯一"].ToString() + "没有查找到对应的备件！");
                        l = new List<string>();
                        break;
                    }
                }
                if (DalSQLite.Complete(l) == true)//被导入数据集合保存到数据库
                {
                    MessageBox.Show("批量替换并作废成功！");
                }
            }
        }



        /// <summary>
        /// 返回数据对象
        /// </summary>
        /// <param name="g">ProductGUID</param>
        /// <returns>ObjProduct</returns>
        public static ObjProduct GetObject(string code)
        {
            ObjProduct obj = null;
            if (string.IsNullOrEmpty(code) == false)
            {
                string sqlString = string.Format("SELECT * FROM crs_product WHERE ProductCode='{0}'", code);
                IList<ObjProduct> list = DalSQLite.GetIList<ObjProduct>(sqlString);
                if (BaseListClass.CheckNull(list) == false)
                {
                    if (list.Count == 1)
                    {
                        obj = list[0];
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 返回数据对象
        /// </summary>
        /// <param name="g">ProductGUID</param>
        /// <returns>ObjProduct</returns>
        public static ObjProduct GetLocation(string code)
        {
            ObjProduct obj = null;
            if (string.IsNullOrEmpty(code) == false)
            {
                string sqlString = string.Format("SELECT a.*,b.LocationGUID FROM crs_product AS a JOIN crs_inventory As b ON a.ProductGUID=b.ProductGUID WHERE a.ProductCode='{0}'", code);
                IList<ObjProduct> list = DalSQLite.GetIList<ObjProduct>(sqlString);
                if (BaseListClass.CheckNull(list) == false)
                {
                    if (list.Count == 1)
                    {
                        obj = list[0];
                    }
                }
            }
            return obj;
        }

        private void RefreshKanbanData()
        {
            ObjCar s = new ObjCar();
            DateTime d = DateTime.Now;
            s.DateStart = d.Date;
            s.DateEnd = d.AddDays(30).Date;

            // 确保DalCar.GetNextList(s)返回非null列表
            var carList = DalCar.GetNextList(s) ?? new List<ObjCar>(); // 使用null合并操作符确保列表不为null

            ObjBill objs = new ObjBill();
            objs.TypeID = (int)EnumBillType.维修单;
            objs.StatusID = (int)EnumBillStatus.保存;

            // 确保DalBill.GetList(objs)返回非null列表
            var billList = DalBill.GetList(objs) ?? new List<ObjBill>(); // 使用null合并操作符确保列表不为null

            // 确保txtNotComplete已实例化
            if (lbNotComplete != null)
            {
                lbNotComplete.Content = string.Format("30日内需保养的汽车数量：{0}        未完成维修单数量：{1}", carList.Count, billList.Count);
            }

        }
        private void lbRefresh_Click(object sender, RoutedEventArgs e)
        {
            lbNotComplete.Content = "";
            RefreshKanbanData();
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            new WindowBillMove().ShowDialog();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("请选择一行完成状态的“入库单”后点击“退货单”！");
            ContentBillManage child = new ContentBillManage();
            child.s.TypeID = (int)EnumBillType.入库单;
            BaseWindowClass.OpenActionWindow(child);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("请选择一行完成状态的“维修单”后点击“退库单”！");
            ContentBillManage child = new ContentBillManage();
            child.s.TypeID = (int)EnumBillType.维修单;
            BaseWindowClass.OpenActionWindow(child);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            BaseWindowClass.AppExit();
        }

        private void hlNext_Click(object sender, RoutedEventArgs e)
        {
            ContentBillManage child = new ContentBillManage();
            child.l = DalBill.GetNotCompleteList();
            child.ShowDialog();
        }
    }
}
