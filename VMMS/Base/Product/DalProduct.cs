using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Reflection;
using System.Text;

namespace VMMS
{
    /// <summary>
    /// 备件资料数据操作层
    /// </summary>
    public class DalProduct
    {
        static public ObjProduct TempProduct;//定义全局参数，用于临时存储单个备件及其库存数据

        /// <summary>
        /// 返回数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProduct> GetFullList(ObjProduct s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_product WHERE ORDER BY ProductCode");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_product WHERE (DeleteMark = 0 OR DeleteMark = 1) ");
                AppendConditionString<ObjProduct>(s, sqlString);
                sqlString.Append(" ORDER BY ProductCode");
            }
            return DalSQLite.GetIList<ObjProduct>(sqlString.ToString());
        }

        /// <summary>
        /// /// <summary>
        /// 追加查询条件拼接SQL语句（排除ModelGUID）
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="s">被拼接的数据对象实例</param>
        /// <param name="sqlString">被追加的SQL字符串</param>
        static void AppendConditionString<T>(T s, StringBuilder sqlString)
        {
            Type t = s.GetType();//获得该类的Type
            //再用Type.GetProperties获得PropertyInfo[],然后就可以用foreach 遍历了
            foreach (PropertyInfo pi in t.GetProperties())
            {
                string name = pi.Name;//获得属性的名字,后面就可以根据名字判断来进行些自己想要的操作
                if (pi.PropertyType == typeof(string))
                {
                    string str = Convert.ToString(pi.GetValue(s, null));
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        string n = pi.Name.ToString();
                        sqlString.Append(" AND " + n + " LIKE '%" + str + "%' ");
                    }
                }
                else if (pi.PropertyType == typeof(Guid))//属性类型GUID
                {
                    Guid g = (Guid)(pi.GetValue(s, null));//获取条件值
                    if (g != (new Guid()))
                    {
                        if (name != "ModelGUID")//排除此条件，单独处理
                        {
                            sqlString.Append(" AND " + name + " = '" + g + "' ");//增加SQL条件语句：与条件值相等

                        }
                        else//ModelGUID属性单独处理
                        {
                            sqlString.Append(string.Format(" AND ProductGUID IN (SELECT ProductGUID FROM crs_product_model WHERE DeleteMark = 0 AND ModelGUID = '{0}')", g));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 返回新编码
        /// </summary>
        /// <returns></returns>
        public static string GetCode()
        {
            object o = BaseDbSQLiteClass.GetSingle("SELECT MAX(ProductCode) FROM crs_product");//获取最大编号
            return BaseStringClass.GetNewMaxCode(o, DalDataConfig.ProductCodeWidth);
        }

        /// <summary>
        /// 返回有效车型数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjModel> GetModelList(Guid productGUID)
        {
            return DalSQLite.GetIList<ObjModel>(string.Format("SELECT a.*,b.ModelCode,b.ModelName FROM crs_product_model AS a JOIN crs_model AS b ON a.ModelGUID=b.ModelGUID WHERE a.DeleteMark=0 AND a.ProductGUID='{0}'", productGUID));
        }

        /// <summary>
        /// 返回数据对象
        /// </summary>
        /// <param name="g">ProductGUID</param>
        /// <returns>ObjProduct</returns>
        public static ObjProduct GetObject(Guid g)
        {
            ObjProduct obj = null;
            if (g != null)
            {
                string sqlString = string.Format("SELECT * FROM crs_product WHERE ProductGUID='{0}' AND DeleteMark=0", g);
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
        /// 根据code、位置返回备件数据集合
        /// </summary>
        /// <param name="Barcode">Barcode</param>
        /// <returns>ObjProduct</returns>
        public static IList<ObjProduct> GetOutObject(string code, ObjLocation loc)
        {
            IList<ObjProduct> result = null;
            if (string.IsNullOrEmpty(code) == false)
            {
                if (loc == null)//未选择库位
                {
                    IList<ObjProduct> list = DalSQLite.GetIList<ObjProduct>(string.Format("SELECT a.*,b.LocationGUID,b.InventoryID,b.InventoryNumber,b.InventoryAmount FROM (SELECT * FROM crs_product WHERE DeleteMark=0) AS a JOIN  (SELECT * FROM crs_inventory WHERE InventoryNumber<>0 ) AS b ON a.ProductGUID=b.ProductGUID WHERE a.ProductCode LIKE '%{0}%' OR  a.Barcode='{0}' OR a.MnemonicCode LIKE '%{0}%' OR a.ProductName LIKE '%{0}%'", code));
                    if (list != null)
                    {
                        if (list.Count > 0)
                        {
                            result = list;
                        }
                    }
                }
                else//已选择库位
                {
                    IList<ObjProduct> list = DalSQLite.GetIList<ObjProduct>(string.Format("SELECT a.*,b.LocationGUID,b.InventoryID,b.InventoryNumber,b.InventoryAmount FROM (SELECT * FROM crs_product WHERE DeleteMark=0) AS a JOIN  (SELECT * FROM crs_inventory WHERE LocationGUID='{1}' AND InventoryNumber<>0 ) AS b ON a.ProductGUID=b.ProductGUID WHERE a.ProductCode LIKE '%{0}%' OR  a.Barcode='{0}' OR a.MnemonicCode LIKE '%{0}%' OR a.ProductName LIKE '%{0}%'", code, loc.LocationGUID));
                    if (list != null)
                    {
                        if (list.Count == 1)
                        {
                            result = list;
                        }
                        else
                        {
                            result = null;
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 根据TextBox返回备件对象
        /// 2022-03-25
        /// </summary>
        /// <returns>ObjProduct</returns>
        public static ObjProduct GetProduct(TextBox TxtProduct,bool isHaveZero = true)
        {
            ObjProduct p = null;
            IList<ObjProduct> list = GetOutList(TxtProduct.Text);//必须唯一库位、不能分库位存放，否则不返回备件数据对象
            if (list != null)
            {
                if (list.Count == 1)
                {
                    if (list[0].PropertyID == (int)EnumProductProperty.实物)
                    {
                        if(list[0].LocationGUID!=new Guid())//有唯一确定库位
                        {
                            p = list[0];
                        }
                        else
                        {
                            SelectLocation(list, ref p, isHaveZero);//无库位
                        }
                    }
                    else if(list[0].PropertyID == (int)EnumProductProperty.服务)
                    {
                        p = list[0];
                    }
                    else
                    {
                        p = NewTips();
                    }
                }
                else if (list.Count > 1)
                {
                    //SelectInventory(list, ref p);
                    SelectLocation(list, ref p, isHaveZero);
                }
                else
                {
                    p = NewTips();
                }
            }
            else
            {
                p = NewTips();
            }
            return p;
        }

        /// <summary>
        /// 提示后启动新增备件窗体、输入新的备件
        /// 2022-03-25
        /// </summary>
        /// <returns>ObjProduct</returns>
        public static ObjProduct NewTips()
        {
            ObjProduct p = null;
            if (System.Windows.MessageBox.Show("未获取到与此编号、名称、助记码、条码可以部分模糊匹配的备件，是否增加新的备件？", "提示", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                p = NewInput();
            }
            return p;
        }

        /// <summary>
        /// 启动新增备件窗体、输入新的备件
        /// 2022-03-25
        /// </summary>
        /// <returns>ObjProduct</returns>
        public static ObjProduct NewInput()
        {
            WindowProduct child = new WindowProduct();
            Guid g = Guid.NewGuid();
            child.obj.ProductGUID = g;
            child.ShowDialog();
            return GetObject(g);
        }

        /// <summary>
        /// 根据编号或名称或助记码或条码返回数据对象
        /// 2022-03-25
        /// </summary>
        /// <param name="code">ProductCode OR Barcode OR MnemonicCode OR ProductName</param>
        /// <returns>IList<ObjProduct></returns>
        public static IList<ObjProduct> GetOutList(string code)
        {
            IList<ObjProduct> list = null;
            if (string.IsNullOrEmpty(code) == false)
            {
                list = DalSQLite.GetIList<ObjProduct>(string.Format("SELECT a.*,b.LocationGUID,b.InventoryNumber,b.InventoryAmount FROM crs_product AS a LEFT JOIN crs_inventory AS b ON a.ProductGUID=b.ProductGUID WHERE (a.DeleteMark=0 AND a.ProductCode Like '%{0}%') OR (a.DeleteMark=0 AND a.Barcode Like '%{0}%') OR (a.DeleteMark=0 AND a.MnemonicCode Like '%{0}%') OR (a.DeleteMark=0 AND a.ProductName Like '%{0}%')", code.Trim()));
            }
            return list;
        }

        /// <summary>
        /// 根据编号或名称或助记码或条码、及性质返回数据对象
        /// </summary>
        /// <param name="code">（ProductCode OR Barcode OR MnemonicCode OR ProductName） AND PropertyID</param>
        /// <returns>IList<ObjProduct></returns>
        public static IList<ObjProduct> GetList(string code, int propertyID)
        {
            IList<ObjProduct> list = null;
            if (string.IsNullOrEmpty(code) == false)
            {
                list = DalSQLite.GetIList<ObjProduct>(string.Format("SELECT * FROM crs_product WHERE (DeleteMark=0 AND ProductCode='{0}' AND PropertyID={1}) OR (DeleteMark=0 AND Barcode='{0}' AND PropertyID={1}) OR (DeleteMark=0 AND MnemonicCode='{0}' AND PropertyID={1}) OR (DeleteMark=0 AND ProductName='{0}' AND PropertyID={1})", code, propertyID));
            }
            return list;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjProduct obj,bool isSelect)
        {
            List<string> l = new List<string>();
            l.Add(GetInsertSqlString(obj));
            if (BaseListClass.CheckNull(obj.ListModel) == false)//循环车型数据
            {
                foreach (ObjModel i in obj.ListModel)
                {
                    l.Add(GetInsertModelString(obj.ProductGUID, i.ModelGUID));
                }
            }
            if(obj!=null && isSelect==true && obj.LocationGUID!=new Guid())
            {
                l.Add(DalBill.GetUpdateInventoryString(obj,obj.LocationGUID,0,0,false));
            }
            return DalSQLite.Insert(l);
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetInsertSqlString(ObjProduct obj)
        {
            if (obj.ProductGUID == new Guid())
                obj.ProductGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_product (ProductGUID, ProductCode, ProductName, MnemonicCode, Barcode, UnitGUID, TypeGUID, PropertyID, Price, UpGUID, Uptime) SELECT '{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},'{9}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT ProductCode FROM crs_product WHERE ProductCode='{1}')", obj.ProductGUID, obj.ProductCode, obj.ProductName, obj.MnemonicCode, obj.Barcode, obj.UnitGUID, obj.TypeGUID, obj.PropertyID, obj.Price, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 新增适配车型
        /// </summary>
        public static bool InsertModel(Guid productGUID, Guid modelGUID)
        {
            bool result = false;
            if (productGUID != new Guid())
            {
                result = DalSQLite.ExecuteSql(GetInsertModelString(productGUID, modelGUID));
            }
            return result;
        }

        /// <summary>
        /// 返回插入适配车型SQL语句
        /// </summary>
        public static string GetInsertModelString(Guid productGUID, Guid modelGUID)
        {
            string sqlString = string.Empty;
            if (productGUID != new Guid() && modelGUID != new Guid())
            {
                sqlString = string.Format("INSERT INTO crs_product_model (ProductGUID, ModelGUID, UpGUID, Uptime) SELECT '{0}','{1}','{2}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT ModelID FROM crs_product_model WHERE DeleteMark=0 AND ProductGUID='{0}' AND ModelGUID='{1}')", productGUID, modelGUID, DalLogin.LoginedUser.UserGUID);
            }
            return sqlString;
        }

        /// <summary>
        /// 删除适配车型
        /// </summary>
        public static bool DeleteModel(ObjModel obj)
        {
            return DalSQLite.DeleteMark(string.Format("UPDATE crs_product_model SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE ModelID={0} AND ProductGUID='{1}' AND ModelGUID='{2}' AND DeleteMark=0", obj.ModelID, obj.ProductGUID, obj.ModelGUID, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjProduct obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_product SET  ProductCode='{1}',ProductName='{2}',MnemonicCode='{3}',Barcode='{4}',UnitGUID='{5}',TypeGUID='{6}', PropertyID={7}, Price={8}, UpGUID='{9}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND ProductID={0} AND NOT EXISTS (SELECT ProductCode FROM crs_product WHERE ProductCode='{1}' AND ProductID<>{0})", obj.ProductID, obj.ProductCode, obj.ProductName, obj.MnemonicCode, obj.Barcode, obj.UnitGUID, obj.TypeGUID, obj.PropertyID, obj.Price, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 作废标记
        /// </summary>
        public static bool DeleteMark(ObjProduct obj)
        {
            string sqlString = string.Format("UPDATE crs_product SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE ProductGUID='{0}' AND DeleteMark=0", obj.ProductGUID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 还原标记
        /// </summary>
        public static bool ReturnMark(ObjProduct obj)
        {
            string sqlString = string.Format("UPDATE crs_product SET DeleteMark=0,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE ProductGUID='{0}' AND DeleteMark=1", obj.ProductGUID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.ReturnMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(IList<ObjProduct> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjProduct i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }

        /// <summary>
        /// 检查Guid集合中是否有此GUID
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool CheckGuid(IList<ObjProduct> guids, Guid g)
        {
            bool result = false;
            if (BaseListClass.CheckNull(guids) == false)
            {
                foreach (ObjProduct i in guids)
                {
                    if (i.ProductGUID == g)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检查明细数据集合中是否有此备件库位重复
        /// </summary>
        /// <param name="guids">备件GUID集合</param>
        /// <param name="productGUID">备件GUID</param>
        /// <param name="debitLocationGUID">入库库位</param>
        /// <param name="creditLocationGUID">出库库位</param>
        /// <returns></returns>
        public static bool CheckDetail(IList<ObjProduct> guids, Guid productGUID, Guid debitLocationGUID, Guid creditLocationGUID)
        {
            bool result = false;
            if (BaseListClass.CheckNull(guids) == false)
            {
                foreach (ObjProduct i in guids)
                {
                    if (i.ProductGUID == productGUID && i.DebitLocationGUID == debitLocationGUID && i.CreditLocationGUID == creditLocationGUID)//备件、库位重复
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 检查明细数据集合中是否有此备件库位重复
        /// </summary>
        /// <param name="guids"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool CheckCreditInventory(IList<ObjProduct> l, ref string remark)
        {
            bool result = true;
            if (BaseListClass.CheckNull(l) == false)
            {
                foreach (ObjProduct i in l)
                {
                    if (i.PropertyID == (int)EnumProductProperty.实物)
                    {
                        if (DalSQLite.GetDataRecordCount(string.Format("SELECT COUNT(*) FROM crs_inventory WHERE ProductGUID='{0}' AND LocationGUID='{1}' AND InventoryNumber>={2}", i.ProductGUID, i.CreditLocationGUID, i.CreditNumber)) < 1)
                        {
                            result = false;
                            remark = remark + i.ProductCode+"、"+i.ProductName + "库存数量不足！\r\n";
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 返回备件指定条件数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProduct> GetInventoryList(ObjProduct s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT a.*, b.LocationGUID, b.InventoryNumber, b.InventoryAmount, c.LocationCode, c.LocationName FROM crs_product AS a LEFT JOIN crs_inventory AS b ON a.ProductGUID = b.ProductGUID JOIN crs_location AS c ON b.LocationGUID = c.LocationGUID WHERE a.DeleteMark = 0 ORDER BY a.ProductCode, c.LocationCode");
            }
            else
            {
                sqlString.Append("SELECT a.*, b.LocationGUID, b.InventoryNumber, b.InventoryAmount, c.LocationCode, c.LocationName FROM crs_product AS a JOIN crs_inventory AS b ON a.ProductGUID = b.ProductGUID JOIN crs_location AS c ON b.LocationGUID = c.LocationGUID WHERE a.DeleteMark = 0 ");
                DalSQLite.AppendAliasString<ObjProduct>(s, sqlString, "a");
                sqlString.Append(" ORDER BY a.ProductCode, c.LocationCode");
            }
            return DalSQLite.GetIList<ObjProduct>(sqlString.ToString());
        }

        /// <summary>
        /// 返回指定库位备件的即时库存
        /// </summary>
        /// <param name="productGUID"></param>
        /// <param name="locationGUID"></param>
        /// <returns></returns>
        public static ObjProduct GetInventory(Guid productGUID, Guid locationGUID)
        {
            ObjProduct result = new ObjProduct();
            IList<ObjProduct> list = DalSQLite.GetIList<ObjProduct>(string.Format("SELECT * FROM crs_inventory WHERE ProductGUID ='{0}' AND LocationGUID='{1}'", productGUID, locationGUID));
            if (list != null)
            {
                if (list.Count == 1)
                {
                    result = list[0];
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }

        /// <summary>
        /// 返回单据明细中指定条件数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProduct> GetBillProductList(ObjProduct s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append(string.Format("SELECT d.DebitLocationGUID,d.DebitNumber,d.CreditLocationGUID,d.CreditNumber,p.*,b.BillGUID,b.BillCode,b.BillDate,b.CustomerGUID,b.UserGUID,b.LicensePlate,b.TypeID,b.StatusID,b.CompleteTime FROM crs_bill_detail AS d LEFT JOIN crs_product AS p ON d.ProductGUID=p.ProductGUID LEFT JOIN crs_bill AS b ON d.BillGUID=b.BillGUID WHERE b.DeleteMark = 0 AND b.StatusID >= {0} AND b.StatudID <= {1} ORDER BY p.ProductCode", (int)EnumBillStatus.完成, (int)EnumBillStatus.已收款));
            }
            else
            {
                sqlString.Append("SELECT d.DebitLocationGUID,d.DebitNumber,d.CreditLocationGUID,d.CreditNumber,p.*,b.BillGUID,b.BillCode,b.BillDate,b.CustomerGUID,b.UserGUID,b.LicensePlate,b.TypeID,b.StatusID,b.CompleteTime FROM crs_bill_detail AS d LEFT JOIN crs_product AS p ON d.ProductGUID=p.ProductGUID LEFT JOIN crs_bill AS b ON d.BillGUID=b.BillGUID WHERE b.DeleteMark = 0 ");
                AppendAliasStringOnlyString<ObjProduct>(s, sqlString, "p");
                if (s.TypeID > 0)//单据类别
                {
                    sqlString.Append(string.Format(" AND b.TypeID = '{0}'", s.TypeID));
                }
                if (s.CustomerGUID != new Guid())//供应商
                {
                    sqlString.Append(string.Format(" AND b.CustomerGUID = '{0}'", s.CustomerGUID));
                }
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())//单据日期段
                {
                    sqlString.Append(string.Format(" AND b.BillDate >= '{0}' AND b.BillDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
                if (s.ModelGUID != new Guid())
                {
                    sqlString.Append(string.Format(" AND b.ModelGUID='{0}'", s.ModelGUID));
                }
                sqlString.Append(" ORDER BY b.BillCode,p.ProductCode");
            }
            return DalSQLite.GetIList<ObjProduct>(sqlString.ToString());
        }

        /// <summary>
        /// 返回销售收入成本明细数据（维修单据明细中指定条件数据集合）
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjProduct> GetSalesCostList(ObjProduct s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s != null)
            {
                sqlString.Append("SELECT d.CreditLocationGUID, d.CreditNumber, d.CreditCost, d.CreditAmount, d.SalesPrice, d.SalesAmount, d.ChargeAmount, p.*, b.BillGUID, b.BillCode, b.BillDate, b.CustomerGUID, b.UserGUID, b.TypeID, b.StatusID,b.CompleteTime FROM crs_bill_detail AS d LEFT JOIN crs_product AS p ON d.ProductGUID = p.ProductGUID LEFT JOIN crs_bill AS b ON d.BillGUID = b.BillGUID WHERE b.DeleteMark = 0 ");
                AppendAliasStringOnlyString<ObjProduct>(s, sqlString, "p");
                sqlString.Append(" AND d.CreditNumber <> 0");
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())//单据日期段
                {
                    sqlString.Append(string.Format(" AND b.BillDate >= '{0}' AND b.BillDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
                if(s.UserGUID !=new Guid())
                {
                    sqlString.Append(string.Format(" AND b.UserGUID = '{0}'",s.UserGUID));
                }
                sqlString.Append(" ORDER BY b.BillCode,p.ProductCode");
            }
            return DalSQLite.GetIList<ObjProduct>(sqlString.ToString());
        }

        /// <summary>
        /// 追加查询条件(仅限文本条件)拼接SQL语句(FROM 表名——别名=a)
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="s">被拼接的数据对象实例</param>
        /// <param name="sqlString">被追加的SQL语句</param>
        static void AppendAliasStringOnlyString<T>(T s, StringBuilder sqlString, string alias)
        {
            Type t = s.GetType();//获得该类的Type
            foreach (PropertyInfo pi in t.GetProperties()) //遍历Type.GetProperties获得的PropertyInfo[]
            {
                string name = pi.Name;//获取属性名  
                                      //根据属性类型条件判断是否拼接SQL语句
                if (pi.PropertyType == typeof(string))//属性类型字符串
                {
                    string str = Convert.ToString(pi.GetValue(s, null));//获取条件值
                    if (str != null)
                    {
                        if (str != "")
                        {
                            sqlString.Append(" AND " + alias + "." + name + " LIKE '%" + str + "%' ");//增加SQL条件语句：包含条件字符
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从指定的备件集合中选择备件、并重定义ObjProdcut
        /// 2022-03-25
        /// </summary>
        /// <param name="list">可选数据集合</param>
        /// <param name="p">重定义ObjProdcut</param>
        /// <returns>bool</returns>
        public static bool SelectLocation(IList<ObjProduct> list, ref ObjProduct p, bool IsHaveZero = true)
        {
            bool result = false;
            WindowProductSelectLocation select = new WindowProductSelectLocation();
            select.l = list;
            select.IsHaveZero = IsHaveZero;
            select.ShowDialog();
            if (select.DialogResult == true)
            {
                p = TempProduct;
                result = true;
            }
            return result;
        }
    }
}
