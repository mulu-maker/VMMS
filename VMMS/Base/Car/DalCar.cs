using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VMMS
{
    /// <summary>  
    /// 车辆数据操作层
    /// </summary>  
    public class DalCar
    {
        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList<ObjCar></returns>
        public static IList<ObjCar> GetViewList(ObjCar s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_car WHERE DeleteMark = 0 ");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_car WHERE DeleteMark = 0  ");
                DalSQLite.AppendConditionString<ObjCar>(s, sqlString);
            }
            return DalSQLite.GetIList<ObjCar>(sqlString.ToString());
        }

        /// <summary>
        /// 返回全部数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjCar> GetFullList(ObjCar s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_car WHERE ORDER BY CarCode");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_car WHERE (DeleteMark = 0 OR DeleteMark = 1) ");
                DalSQLite.AppendConditionString<ObjCar>(s, sqlString);
                sqlString.Append(" ORDER BY CarCode");
            }
            return DalSQLite.GetIList<ObjCar>(sqlString.ToString());
        }

        /// <summary>
        /// 返回下次保养日期到期数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjCar> GetNextList(ObjCar s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s != null)
            {           
                sqlString.Append("SELECT * FROM crs_car WHERE DeleteMark=0 "); 
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())
                {
                    sqlString.Append(string.Format(" AND NextMaintenanceDate >= '{0}' AND NextMaintenanceDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
                DalSQLite.AppendConditionString<ObjCar>(s, sqlString);
                sqlString.Append(" ORDER BY CarCode");
            }
            return DalSQLite.GetIList<ObjCar>(sqlString.ToString());
        }



        /// <summary>
        /// 返回下次保养日期到期数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjCar> GetInsuranceList(ObjCar s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s != null)
            {
                sqlString.Append("SELECT * FROM crs_car WHERE DeleteMark=0 ");
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())
                {
                    sqlString.Append(string.Format(" AND InsuranceDate >= '{0}' AND InsuranceDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
                DalSQLite.AppendConditionString<ObjCar>(s, sqlString);
                sqlString.Append(" ORDER BY CarCode");
            }
            return DalSQLite.GetIList<ObjCar>(sqlString.ToString());
        }

        /// <summary>
        /// 返回指定日期段未到店车辆数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjCar> GetDateList(ObjCar s)
        {
            IList<ObjCar> l = DalSQLite.GetIList<ObjCar>(string.Format("SELECT a.*,b.BillGUID FROM crs_car AS a LEFT JOIN (SELECT * FROM crs_bill WHERE DeleteMark=0 AND BillDate>='{0}' AND BillDate<'{1}') AS b WHERE a.DeleteMark = 0 ORDER BY CarCode", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
            if (l != null && l.Count > 0)
            {
                return l.Where(p => p.BillGUID == new Guid()).ToList();
            }
            else
            {
                return l;
            }
        }

        /// <summary>
        /// 返回新编码
        /// </summary>
        /// <returns></returns>
        public static string GetCode()
        {
            object o = BaseDbSQLiteClass.GetSingle("SELECT MAX(CarCode) FROM crs_car");//获取最大编号
            return BaseStringClass.GetNewMaxCode(o, DalDataConfig.CarCodeWidth);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjCar obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        /// <summary>
        /// 返回插入SQL语句
        /// </summary>
        public static string GetInsertSqlString(ObjCar obj)
        {
            if (obj.CarGUID == new Guid())
                obj.CarGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_car (CarGUID,CarCode,VIN,LicensePlate,TotalMileage,ModelGUID,EngineModel,EngineCapacity,ManufactureDate,CustomerGUID,CustomerName,MobilePhone,CarColor,Remark,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT CarID FROM crs_car WHERE CarCode='{1}')", obj.CarGUID, obj.CarCode,obj.VIN, obj.LicensePlate, obj.TotalMileage, obj.ModelGUID, obj.EngineModel, obj.EngineCapacity, obj.ManufactureDate.Date.ToString("s"), obj.CustomerGUID, obj.CustomerName, obj.MobilePhone, obj.CarColor, obj.Remark, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjCar obj)
        {
            string sqlString = string.Format("UPDATE crs_car SET  CarCode='{1}',VIN='{2}',LicensePlate='{3}',TotalMileage='{4}',ModelGUID='{5}',EngineModel='{6}',EngineCapacity='{7}',ManufactureDate='{8}',CustomerGUID='{9}',CustomerName='{10}',MobilePhone='{11}',CarColor='{12}',Remark='{13}',UpGUID='{14}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND CarID ={0} AND NOT EXISTS (SELECT CarID FROM crs_car WHERE CarCode='{1}' AND CarID<>{0})", obj.CarID, obj.CarCode,obj.VIN, obj.LicensePlate, obj.TotalMileage, obj.ModelGUID, obj.EngineModel, obj.EngineCapacity, obj.ManufactureDate.Date.ToString("s"), obj.CustomerGUID, obj.CustomerName, obj.MobilePhone,obj.CarColor, obj.Remark, DalLogin.LoginedUser.UserGUID);
             return DalSQLite.Update(sqlString);
        }

        /// <summary>
        /// 作废标记
        /// </summary>
        public static bool DeleteMark(ObjCar obj)
        {
            string sqlString = string.Format("UPDATE crs_car SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE CarID={0} AND DeleteMark=0", obj.CarID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 还原标记
        /// </summary>
        public static bool ReturnMark(ObjCar obj)
        {
            string sqlString = string.Format("UPDATE crs_car SET DeleteMark=0,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE CarGUID='{0}' AND DeleteMark=1", obj.CarGUID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.ReturnMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjCar> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjCar i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Import(listSqlStr);
        }

        /// <summary>
        /// 返回更新车辆总里程SQL字符串
        /// </summary>
        public static string GetUpdateCarMileageString(ObjBill obj)
        {
            return string.Format("UPDATE crs_car SET TotalMileage={1} WHERE DeleteMark=0 AND CarGUID='{0}'", obj.CarGUID, obj.CarMileage);//更新条件：除本行外无重复备件、库位
        }

        /// <summary>
        /// 返回更新车辆下次保养日期SQL字符串
        /// </summary>
        public static string GetUpdateNextMaintenanceDateSqlString(ObjBill obj)
        {
            return string.Format("UPDATE crs_car SET NextMaintenanceDate='{1}' WHERE CarGUID ='{0}'", obj.CarGUID, obj.NextMaintenanceDate.ToString("s"));
        }

        /// <summary>
        /// 返回更新车辆保险公司、保险到期日期SQL字符串
        /// </summary>
        public static string GetUpdateInsuranceSqlString(ObjBill obj)
        {
            return string.Format("UPDATE crs_car SET InsuranceCompany='{1}',InsuranceDate='{2}' WHERE CarGUID ='{0}'", obj.CarGUID, obj.InsuranceCompany, obj.InsuranceDate.ToString("s"));
        }

        /// <summary>
        /// 根据车牌号返回数据对象
        /// </summary>
        /// <param name="licensePlate">车牌号字符串</param>
        /// <returns>ObjCar</returns>
        public static ObjCar GetObject(string licensePlate)
        {
            ObjCar obj = null;
            if (string.IsNullOrEmpty(licensePlate) == false)
            {
                IList<ObjCar> list = DalSQLite.GetIList<ObjCar>(string.Format("SELECT a.*,b.ModelName FROM crs_car AS a LEFT JOIN crs_model AS b ON a.ModelGUID=b.ModelGUID WHERE a.DeleteMark=0 AND a.LicensePlate LIKE '%{0}%'", licensePlate));
                if (list != null)
                {
                    if (list.Count == 1)
                    {
                        obj = list[0];
                    }
                    else
                    {
                        obj = null;
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 返回数据对象
        /// </summary>
        /// <param name="g">CarGUID</param>
        /// <returns>ObjCar</returns>
        public static ObjCar GetObject(Guid g)
        {
            ObjCar obj = null;
            if (g != null)
            {
                string sqlString = string.Format("SELECT * FROM crs_car WHERE CarGUID='{0}'", g);
                IList<ObjCar> list = DalSQLite.GetIList<ObjCar>(sqlString);
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
        /// 启动新增备件窗体
        /// </summary>
        /// <returns>ObjProduct</returns>
        public static ObjCar InputNew()
        {
            WindowCar child = new WindowCar();
            Guid g = Guid.NewGuid();
            child.obj.CarGUID = g;
            child.ShowDialog();
            return DalCar.GetObject(g);
        }

        /// <summary>
        /// 返回车辆维修记录数据集合
        /// </summary>
        /// <param name="s">ObjBill</param>
        /// <returns>IList<ObjBill></returns>
        public static IList<ObjBill> GetBillList(ObjBill s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT a.*,b.ProductGUID,b.CreditNumber,b.CreditCost,b.CreditAmount,c.ProductName,b.SalesPrice,b.SalesAmount FROM crs_bill AS a JOIN crs_bill_detail AS b ON a.BillGUID=b.BillGUID JOIN crs_product AS c ON b.ProductGUID=c.ProductGUID WHERE a.DeleteMark = 0 ");
            }
            else
            {
                sqlString.Append("SELECT a.*,b.ProductGUID,b.ProductCode,b.CreditNumber,b.CreditCost,b.CreditAmount,c.ProductName,b.SalesPrice,b.SalesAmount FROM crs_bill AS a JOIN crs_bill_detail AS b ON a.BillGUID=b.BillGUID JOIN crs_product AS c ON b.ProductGUID=c.ProductGUID WHERE a.DeleteMark = 0 ");
                DalSQLite.AppendAliasString<ObjBill>(s, sqlString, "a");
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())//单据日期段
                {
                    sqlString.Append(string.Format(" AND a.BillDate >= '{0}' AND a.BillDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
                if (s.ModelGUID!=new Guid())
                {
                    sqlString.Append(string.Format(" AND a.ModelGUID='{0}'", s.ModelGUID));
                }
                sqlString.Append(" ORDER BY a.LicensePlate,a.BillCode,b.ProductCode");
            }
            return DalSQLite.GetIList<ObjBill>(sqlString.ToString());
        }


        /// <summary>
        /// 检查单据中输入的车辆里程、前次里程、行驶里程是否依次正确
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string CheckMileage(IList<ObjBill> l)
        {
            string remark = string.Empty;
            if (BaseListClass.CheckNull(l) == false)
            {
                int currentMileage = l[0].TotalMileage;//定义当前里程数=第一条数据的上次里程数
                int carMileage = l[0].CarMileage;//定义上次车辆里程
                string code = l[0].BillCode;
                foreach (ObjBill i in l)
                {
                    if (i.TotalMileage == currentMileage)//判断上次里程数正确
                    {
                        if(i.CarMileage < carMileage)
                        {
                            remark = remark + string.Format("车牌号{0}单据号{1}：车辆里程<上次车辆里程（上次单据号：{2}），请检查是否有误！\r\n", i.LicensePlate, i.BillCode, code);
                        }
                        if (i.CarMileage - i.TotalMileage == i.Mileage)
                        {
                            currentMileage = i.CarMileage;//当前里程数赋值
                        }
                        else
                        {
                            remark = remark + string.Format("车牌号{0}单据号{1}：车辆里程-上次车辆里程≠行驶里程，请检查是否有误！\r\n", i.LicensePlate, i.BillCode);
                            break;
                        }
                    }
                    else
                    {
                        remark = remark + string.Format("车牌号{0}单据号{1}：上次里程与上一单据{2}车辆里程不一致，请检查是否有误！\r\n", i.LicensePlate, i.BillCode, code);
                        break;
                    }
                    code = i.BillCode;
                }
            }
            return remark;
        }
    }
}
