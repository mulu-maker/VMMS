using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMMS
{
    /// <summary>
    /// 出入库单据数据操作类
    /// </summary>
    public class DalBill
    {
        /// <summary>
        /// 按对象条件返回有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjBill> GetViewList(ObjBill s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_bill WHERE DeleteMark = 0 ");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_bill WHERE DeleteMark = 0  ");
                DalSQLite.AppendConditionString<ObjBill>(s, sqlString);
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())
                {
                    sqlString.Append(string.Format(" AND BillDate >= '{0}' AND BillDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
            }
            return DalSQLite.GetIList<ObjBill>(sqlString.ToString());
        }

        /// <summary>
        /// 按对象条件返回有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjBill> GetList(ObjBill s)
        {
            StringBuilder sqlString = new StringBuilder();
            if (s == null)
            {
                sqlString.Append("SELECT * FROM crs_bill");
            }
            else
            {
                sqlString.Append("SELECT * FROM crs_bill WHERE"); 
                if (s.DateStart != new DateTime() && s.DateEnd != new DateTime())
                {
                    sqlString.Append(string.Format(" BillDate >= '{0}' AND BillDate < '{1}'", s.DateStart.Date.ToString("s"), s.DateEnd.AddDays(1).Date.ToString("s")));
                }
                DalSQLite.AppendConditionString<ObjBill>(s, sqlString); 
            }
            return DalSQLite.GetIList<ObjBill>(sqlString.ToString());
        }

        /// <summary>
        /// 返回库存重算有效数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjBill> GetCalcList()
        {
            return DalSQLite.GetIList<ObjBill>("SELECT * FROM crs_bill WHERE DeleteMark = 0 ORDER BY CompleteTime");
        }

        /// <summary>
        /// 返回超期一月未完成单据数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjBill> GetNotCompleteList()
        {
            return DalSQLite.GetIList<ObjBill>(string.Format("SELECT * FROM crs_bill WHERE DeleteMark = 0 AND StatusID = {0} AND BuilderTime < '{1}' ORDER BY CompleteTime",(int)EnumBillStatus.保存,DateTime.Now.AddMonths(-1).Date.ToString("s")));
        }

        /// <summary>
        /// 返回车辆维修单数据集合
        /// </summary>
        /// <returns>IList</returns>
        public static IList<ObjBill> GetCarList(ObjCar obj)
        {
            return DalSQLite.GetIList<ObjBill>(string.Format("SELECT * FROM crs_bill WHERE DeleteMark = 0 AND CarGUID='{0}' AND TypeID={1} ORDER BY BuilderTime", obj.CarGUID,(int)EnumBillType.维修单));
        }

        /// <summary>
        /// 返回新编码
        /// </summary>
        /// <returns></returns>
        public static string GetNewCode()
        {
            DateTime d = DateTime.Now.Date;
            object obj = BaseDbSQLiteClass.GetSingle(string.Format("SELECT MAX(BillCode) FROM crs_bill WHERE  BillCode LIKE '{0}%'", d.ToString("yyyyMMdd")));//获取当年最大编码
            return BaseStringClass.GetNewDateCode(obj, DalDataConfig.BillCodeWidth, d);
        }

        /// <summary>
        /// 插入数据不可修改（变更库存）
        /// 仅用于入库单
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static bool Insert(ObjBill obj,int statusID,bool isOut)
        {
            bool result = false;
            List<string> l = new List<string>();
            if (obj != null)
            {
                if (BaseListClass.CheckNull(obj.ListDetail) == false)
                {
                    l.Add(GetInsertSqlString(obj, statusID)); //增加插入主表SQL语句  
                    AddInsertDetailSqlString(obj, l);//循环增加插入明细数据表SQL语句
                    AddUpdateInventorySqlString(obj, l,isOut);//循环增加插入明细数据变更库存表SQL语句
                    result = DalSQLite.Complete(l);
                }
            }
            return result;
        }

        /// <summary>
        /// 插入数据不可修改（增加库存）
        /// 仅用于退库单
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static bool Back(ObjBill obj)
        {
            bool result = false;
            List<string> l = new List<string>();
            if (obj != null)
            {
                if (BaseListClass.CheckNull(obj.ListDetail) == false)
                {
                    if (obj.BillGUID == new Guid())
                        obj.BillGUID = Guid.NewGuid();
                    l.Add((string.Format("INSERT INTO crs_bill (BillGUID,BillCode,BillDate,SourceGUID,SourceCode,CarGUID,CarCode,ModelGUID,ModelName,LicensePlate,VIN,CustomerGUID,CustomerName,SendName,MobilePhone,CarMileage,TotalMileage,Mileage,UserGUID,ItemGUID,ItemName,ItemContent,TotalDebitNumber,TotalDebitAmount,TotalCreditNumber,TotalCreditAmount,TotalSalesNumber,TotalSalesAmount,TotalDiffAmount,TotalChargeAmount,TypeID,StatusID,NextMaintenanceDate,Remark,UpGUID,Uptime,BuilderTime,CompleteTime) SELECT '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},{16},{17},'{18}','{19}','{20}','{21}',{22},{23},{24},{25},{26},{27},{28},{29},{30},'{31}','{32}','{33}','{34}',DateTime('Now', 'localtime'),DateTime('Now', 'localtime'),DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT BillGUID FROM crs_bill WHERE SourceGUID='{3}')", obj.BillGUID, obj.BillCode, obj.BillDate.ToString("s"), obj.SourceGUID, obj.SourceCode, obj.CarGUID, obj.CarCode, obj.ModelGUID, obj.ModelName, obj.LicensePlate, obj.VIN, obj.CustomerGUID, obj.CustomerName, obj.SendName, obj.MobilePhone, obj.CarMileage, obj.TotalMileage, obj.Mileage, obj.UserGUID, obj.ItemGUID, obj.ItemName, obj.ItemContent, obj.TotalDebitNumber, obj.TotalDebitAmount, obj.TotalCreditNumber, obj.TotalCreditAmount, obj.TotalSalesNumber, obj.TotalSalesAmount, obj.TotalDiffAmount, obj.TotalChargeAmount, obj.TypeID, (int)EnumBillStatus.完成, obj.NextMaintenanceDate.ToString("s"), obj.Remark, DalLogin.LoginedUser.UserGUID)));//增加插入主表SQL语句  
                    AddInsertDetailSqlString(obj, l);//循环增加插入明细数据表SQL语句
                    AddUpdateInventorySqlString(obj, l, false);//循环增加插入明细数据变更库存表SQL语句
                    l.Add(DalCar.GetUpdateCarMileageString(obj));
                    result = DalSQLite.Complete(l);
                }
            }
            return result;
        }



        /// <summary>
        /// 插入数据不可修改（变更库存）
        /// 仅用于调拨单
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static bool Tranfser(ObjBill obj)
        {
            bool result = false;
            List<string> l = new List<string>();
            if (obj != null)
            {
                
                if (BaseListClass.CheckNull(obj.ListDetail) == false)
                {
                    l.Add(GetInsertSqlString(obj, (int)EnumBillStatus.完成)); //增加插入主表SQL语句  
                    AddInsertDetailSqlString(obj, l);//循环增加插入明细数据表SQL语句
                    AddUpdateInventorySqlString(obj, l, false);//循环增加插入明细数据变更库存表SQL语句
                    result = DalSQLite.Complete(l);
                }
            }
            return result;
        }

        /// <summary>
        /// 插入数据后可修改（不变更库存）
        /// 仅用于维修单
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static bool RepairNew(ObjBill obj, int statusID)
        {
            bool result = false;
            if (obj.TypeID == (int)EnumBillType.维修单)
            {
                List<string> l = new List<string>();
                if (obj != null)
                {
                    if (BaseListClass.CheckNull(obj.ListDetail) == false)
                    {
                        l.Add(GetInsertSqlString(obj, statusID)); //增加插入主表SQL语句  
                        AddInsertDetailSqlString(obj, l);//循环增加插入明细数据SQL语句
                        result = DalSQLite.Save(l);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 更新数据后可修改（不变更库存）
        /// 仅用于维修单
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static bool RepairUpdate(ObjBill obj, int statusID)
        {
            bool result = false;
            if (obj.TypeID == (int)EnumBillType.维修单)
            {
                List<string> l = new List<string>();
                if (obj != null)
                {
                    if (BaseListClass.CheckNull(obj.ListDetail) == false)
                    {
                        l.Add(GetRepairUpdateSqlString(obj)); //增加更新主表SQL语句 
                        if (GetDetailCount(obj.BillGUID) > 0)//判断数据库有无明细数据
                        {
                            l.Add(string.Format("DELETE FROM crs_bill_detail WHERE BillGUID='{0}'", obj.BillGUID));//有则删除
                        }
                        AddInsertDetailSqlString(obj, l);//循环增加插入明细数据SQL语句
                        result = DalSQLite.Save(l);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 完成维修单（更新数据不再修改、库存变动）
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        public static bool RepairComplete(ObjBill obj, int statusID,bool? isAdd)
        {
            bool result = false;
            if (System.Windows.MessageBox.Show("完成后将不能修改单据，是否继续？", "提示", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                if (obj.TypeID == (int)EnumBillType.维修单)
                {
                    List<string> l = new List<string>();
                    if (obj != null)
                    {
                        if (BaseListClass.CheckNull(obj.ListDetail) == false)
                        {
                            if (isAdd == true)
                            {
                                l.Add(GetInsertSqlString(obj, statusID)); //增加插入主表SQL语句  
                            }
                            else
                            {
                                l.Add(GetRepairCompleteSqlString(obj)); //增加更新主表SQL语句 
                            }
                            l.Add(DalCar.GetUpdateCarMileageString(obj));//更新车辆里程
                            if(obj.NextMaintenanceDate>BaseDateTimeClass.BaseDate)//更新保养到期日期
                            {
                                l.Add(DalCar.GetUpdateNextMaintenanceDateSqlString(obj));
                            }
                            if (obj.InsuranceDate > BaseDateTimeClass.BaseDate)//更新车辆保险公司、到期日期
                            {
                                l.Add(DalCar.GetUpdateInsuranceSqlString(obj));
                            }
                            if (GetDetailCount(obj.BillGUID) > 0)//判断数据库有无明细数据
                            {
                                l.Add(string.Format("DELETE FROM crs_bill_detail WHERE BillGUID='{0}'", obj.BillGUID));//有则删除
                            }
                            foreach(ObjProduct i in obj.ListDetail)
                            {                               
                                ObjProduct inventory = DalProduct.GetInventory(i.ProductGUID, i.CreditLocationGUID);
                                if (inventory.InventoryNumber > 0)
                                {
                                    i.CreditCost = inventory.InventoryAmount / inventory.InventoryNumber;
                                    i.CreditAmount = i.CreditNumber * i.CreditCost;
                                }
                            }
                            AddInsertDetailSqlString(obj, l);//循环增加插入明细数据SQL语句
                            AddUpdateInventorySqlString(obj, l,true);//循环增加明细数据变更库存表SQL语句（减少库存）
                            result = DalSQLite.Insert(l);
                            //}
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 返回插入数据SQL语句
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="l"></param>
        private static string GetInsertSqlString(ObjBill obj,int statusID)
        {
            if (obj.BillGUID == new Guid())
                obj.BillGUID = Guid.NewGuid();
            if (statusID != (int)EnumBillStatus.完成)//未完成的单据
            {
                return string.Format("INSERT INTO crs_bill (BillGUID,BillCode,BillDate,SourceGUID,SourceCode,CarGUID,CarCode,ModelGUID,ModelName,LicensePlate,VIN,CustomerGUID,CustomerName,SendName,MobilePhone,CarMileage,TotalMileage,Mileage,UserGUID,ItemGUID,ItemName,ItemContent,TotalDebitNumber,TotalDebitAmount,TotalCreditNumber,TotalCreditAmount,TotalSalesNumber,TotalSalesAmount,TotalDiffAmount,TotalChargeAmount,TypeID,StatusID,NextMaintenanceDate,InsuranceCompany,InsuranceDate,Remark,UpGUID,Uptime,BuilderTime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},{16},{17},'{18}','{19}','{20}','{21}',{22},{23},{24},{25},{26},{27},{28},{29},{30},'{31}','{32}','{33}','{34}','{35}','{36}',DateTime('Now', 'localtime'),DateTime('Now', 'localtime')) ", obj.BillGUID, obj.BillCode, obj.BillDate.ToString("s"), obj.SourceGUID, obj.SourceCode, obj.CarGUID, obj.CarCode, obj.ModelGUID, obj.ModelName, obj.LicensePlate, obj.VIN, obj.CustomerGUID, obj.CustomerName, obj.SendName, obj.MobilePhone, obj.CarMileage, obj.TotalMileage, obj.Mileage, obj.UserGUID, obj.ItemGUID, obj.ItemName, obj.ItemContent, obj.TotalDebitNumber, obj.TotalDebitAmount, obj.TotalCreditNumber, obj.TotalCreditAmount, obj.TotalSalesNumber, obj.TotalSalesAmount, obj.TotalDiffAmount, obj.TotalChargeAmount, obj.TypeID, statusID, obj.NextMaintenanceDate.ToString("s"), obj.InsuranceCompany, obj.InsuranceDate.ToString("s"), obj.Remark, DalLogin.LoginedUser.UserGUID);
            }
            else//完成的单据
            {
                return string.Format("INSERT INTO crs_bill (BillGUID,BillCode,BillDate,SourceGUID,SourceCode,CarGUID,CarCode,ModelGUID,ModelName,LicensePlate,VIN,CustomerGUID,CustomerName,SendName,MobilePhone,CarMileage,TotalMileage,Mileage,UserGUID,ItemGUID,ItemName,ItemContent,TotalDebitNumber,TotalDebitAmount,TotalCreditNumber,TotalCreditAmount,TotalSalesNumber,TotalSalesAmount,TotalDiffAmount,TotalChargeAmount,TypeID,StatusID,NextMaintenanceDate,InsuranceCompany,InsuranceDate,Remark,UpGUID,Uptime,BuilderTime,CompleteTime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},{16},{17},'{18}','{19}','{20}','{21}',{22},{23},{24},{25},{26},{27},{28},{29},{30},'{31}','{32}','{33}','{34}','{35}','{36}',DateTime('Now', 'localtime'),DateTime('Now', 'localtime'),DateTime('Now', 'localtime')) ", obj.BillGUID, obj.BillCode, obj.BillDate.ToString("s"), obj.SourceGUID, obj.SourceCode, obj.CarGUID, obj.CarCode, obj.ModelGUID, obj.ModelName, obj.LicensePlate, obj.VIN, obj.CustomerGUID, obj.CustomerName, obj.SendName, obj.MobilePhone, obj.CarMileage, obj.TotalMileage, obj.Mileage, obj.UserGUID, obj.ItemGUID, obj.ItemName, obj.ItemContent, obj.TotalDebitNumber, obj.TotalDebitAmount, obj.TotalCreditNumber, obj.TotalCreditAmount, obj.TotalSalesNumber, obj.TotalSalesAmount, obj.TotalDiffAmount, obj.TotalChargeAmount, obj.TypeID, statusID, obj.NextMaintenanceDate.ToString("s"), obj.InsuranceCompany, obj.InsuranceDate.ToString("s"), obj.Remark,DalLogin.LoginedUser.UserGUID);
            }
        }

        /// <summary>
        /// 返回维修单更新数据SQL语句
        /// </summary>
        /// <param name="obj"></param>
        private static string GetRepairUpdateSqlString(ObjBill obj)
        {
            if (obj.BillGUID != new Guid())
            {
                int statusID = (int)EnumBillStatus.保存;
                return string.Format("UPDATE crs_bill SET MobilePhone='{1}',CarMileage={2},TotalMileage={3}, Mileage={4}, UserGUID='{5}',ItemGUID='{6}', ItemName='{7}', ItemContent='{8}', TotalDebitNumber={9}, TotalDebitAmount={10},TotalCreditNumber={11},TotalCreditAmount={12},TotalSalesNumber={13},TotalSalesAmount={14}, TotalDiffAmount={15},TotalChargeAmount={16},StatusID={17}, NextMaintenanceDate='{18}',Remark='{19}',InsuranceCompany='{20}', InsuranceDate='{21}',UpGUID='{22}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND BillGUID='{0}' AND StatusID={17}", obj.BillGUID, obj.MobilePhone, obj.CarMileage, obj.TotalMileage, obj.Mileage, obj.UserGUID, obj.ItemGUID, obj.ItemName, obj.ItemContent, obj.TotalDebitNumber, obj.TotalDebitAmount, obj.TotalCreditNumber, obj.TotalCreditAmount, obj.TotalSalesNumber, obj.TotalSalesAmount, obj.TotalDiffAmount, obj.TotalChargeAmount, statusID, obj.NextMaintenanceDate.ToString("s"), obj.Remark, obj.InsuranceCompany, obj.InsuranceDate.ToString("s"), DalLogin.LoginedUser.UserGUID);//更新表数据            
            }
            else//完成的单据
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 返回维修单完成数据SQL语句
        /// </summary>
        /// <param name="obj"></param>
        private static string GetRepairCompleteSqlString(ObjBill obj)
        {
            if (obj.BillGUID != new Guid())
            {
                return string.Format("UPDATE crs_bill SET MobilePhone='{1}',CarMileage={2},TotalMileage={3}, Mileage={4}, UserGUID='{5}',ItemGUID='{6}', ItemName='{7}', ItemContent='{8}', TotalDebitNumber={9}, TotalDebitAmount={10},TotalCreditNumber={11},TotalCreditAmount={12},TotalSalesNumber={13},TotalSalesAmount={14}, TotalDiffAmount={15},TotalChargeAmount={16},StatusID={17}, NextMaintenanceDate='{18}',Remark='{19}',InsuranceCompany='{20}', InsuranceDate='{21}',UpGUID='{22}',Uptime=DateTime('Now', 'localtime'), CompleteGUID='{22}',CompleteTime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND BillGUID='{0}'", obj.BillGUID, obj.MobilePhone, obj.CarMileage, obj.TotalMileage, obj.Mileage, obj.UserGUID, obj.ItemGUID, obj.ItemName, obj.ItemContent, obj.TotalDebitNumber, obj.TotalDebitAmount, obj.TotalCreditNumber, obj.TotalCreditAmount, obj.TotalSalesNumber, obj.TotalSalesAmount, obj.TotalDiffAmount, obj.TotalChargeAmount, (int)EnumBillStatus.完成, obj.NextMaintenanceDate.ToString("s"), obj.Remark, obj.InsuranceCompany, obj.InsuranceDate.ToString("s"), DalLogin.LoginedUser.UserGUID);//更新表数据          
            }
            else//完成的单据
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 循环增加插入明细数据SQL语句
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="l"></param>
        private static void AddInsertDetailSqlString(ObjBill obj, List<string> l)
        {
            if (BaseListClass.CheckNull<ObjProduct>(obj.ListDetail) == false)
            {
                int sn = 0;
                foreach (ObjProduct i in obj.ListDetail)
                {                  
                    sn++;
                    if (i.DetailGUID == new Guid())
                        i.DetailGUID = Guid.NewGuid();
                    if (obj.TypeID == (int)EnumBillType.入库单 || obj.TypeID == (int)EnumBillType.退库单)//入库单据
                    {
                        if (i.DebitNumber > 0)
                        {
                            i.DebitCost = i.DebitAmount / i.DebitNumber;//计算入库单位成本
                        }
                        l.Add(GetInsertDetailString(obj, i, sn));//插入明细表                       
                    }
                    else if (obj.TypeID == (int)EnumBillType.维修单 || obj.TypeID == (int)EnumBillType.退货单)//出库单据、不限制负库存出库
                    {
                        if (i.CreditNumber > 0)
                        {
                            i.SalesNumber = i.CreditNumber;
                            i.SalesPrice = i.SalesAmount / i.SalesNumber;//计算入库单位成本
                        }
                        l.Add(GetInsertDetailString(obj, i, sn));//插入明细表                       
                    }
                    else if(obj.TypeID == (int)EnumBillType.调拨单)
                    {
                        l.Add(GetInsertDetailString(obj, i, sn));//插入明细表
                    }
                }
            }
        }

        /// <summary>
        /// 返回插入明细表SQL字符串
        /// </summary>
        /// <param name="obj">ObjBill</param>
        /// <param name="i">ObjProduct</param>
        /// <param name="sn">序号</param>
        /// <returns></returns>
        private static string GetInsertDetailString(ObjBill obj, ObjProduct i, int sn)
        {
            return string.Format("INSERT INTO crs_bill_detail (DetailGUID,BillGUID,DetailSN, ProductGUID,ProductCode,ProductName,UnitGUID,TypeGUID,PropertyID,DebitLocationGUID,DebitNumber,DebitAmount,DebitCost,CreditLocationGUID,CreditNumber,CreditAmount,CreditCost,SalesNumber,SalesPrice,SalesAmount,DiffAmount,ChargeAmount,Uptime) VALUES ('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}',{8},'{9}',{10},{11},{12},'{13}',{14},{15},{16},{17},{18},{19},{20},{21},DateTime('Now', 'localtime'))", i.DetailGUID, obj.BillGUID, sn, i.ProductGUID,i.ProductCode,i.ProductName, i.UnitGUID, i.TypeGUID, i.PropertyID, i.DebitLocationGUID, i.DebitNumber, i.DebitAmount, i.DebitCost, i.CreditLocationGUID, i.CreditNumber, i.CreditAmount, i.CreditCost, i.SalesNumber, i.SalesPrice, i.SalesAmount, i.DiffAmount, i.ChargeAmount);//插入明细表
        }

        /// <summary>
        /// 循环更新库存表数据SQL语句
        /// </summary>
        /// <param name="obj">单据对象</param>
        /// <param name="l">SQL语句集合</param>
        /// <param name="isOut">减少库存参数bool</param>
        public static void AddUpdateInventorySqlString(ObjBill obj, List<string> l,bool isOut)
        {
            if (BaseListClass.CheckNull<ObjProduct>(obj.ListDetail) == false)
            {
                foreach (ObjProduct i in obj.ListDetail)
                {
                    if (i.DetailGUID == new Guid())
                        i.DetailGUID = Guid.NewGuid();
                    //插入或更新库存表的判断：通过单据类别区分(涉及到成本计算)，不限制是否当前业务期间，允许负库存（出库成本=0，易引起异常）
                    if (obj.TypeID == (int)EnumBillType.入库单 || obj.TypeID == (int)EnumBillType.退货单)//入库单据
                    {
                        if (obj.TypeID == (int)EnumBillType.入库单 && i.DebitNumber > 0)
                        {
                            i.DebitCost = i.DebitAmount / i.DebitNumber;
                        }
                        if (i.PropertyID == (int)EnumProductProperty.实物)//实物备件插入或增加库存表值，非实物备件无库存不进行库存数据操作、不影响库存表
                        {
                            l.Add(GetUpdateInventoryString(i, i.DebitLocationGUID, i.DebitNumber - i.CreditNumber, i.DebitAmount - i.CreditAmount, isOut));//借方发生数-贷方发生数
                        }
                    }
                    else if (obj.TypeID == (int)EnumBillType.维修单 ||obj.TypeID == (int)EnumBillType.退库单)//出库单据、不限制负库存出库
                    {
                        if (i.PropertyID == (int)EnumProductProperty.实物)//实物备件插入或更新库存表，非实物备件无库存不进行库存数据操作、不影响库存表
                        {
                            l.Add(GetUpdateInventoryString(i, i.CreditLocationGUID, i.DebitNumber - i.CreditNumber, i.DebitAmount - i.CreditAmount,isOut));//借方发生数-贷方发生数
                        }
                    }
                   else if (obj.TypeID == (int)EnumBillType.调拨单)
                    {
                        if (i.PropertyID == (int)EnumProductProperty.实物)//实物备件插入或更新库存表，非实物备件无库存不进行库存数据操作、不影响库存表
                        {
                            l.Add(GetUpdateInventoryString(i, i.CreditLocationGUID, 0 - i.CreditNumber, 0 - i.CreditAmount, true));//借方发生数-贷方发生数
                            l.Add(GetUpdateInventoryString(i, i.DebitLocationGUID, i.DebitNumber, i.DebitAmount, false));//借方发生数-贷方发生数
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 返回更新库存表SQL语句（有则UPDATEW无则INSERT）
        /// </summary>
        /// <param name="i">备件对象</param>
        /// <param name="locationGUID">库位GUID</param>
        /// <param name="number">数量</param>
        /// <param name="amount">金额</param>
        /// <param name="isOut">减少库存参数bool</param>
        /// <returns></returns>
        public static string GetUpdateInventoryString(ObjProduct i, Guid locationGUID, decimal number, decimal amount, bool isOut)
        {
            if (isOut == true)
            {
                return string.Format("UPDATE crs_inventory SET InventoryNumber=InventoryNumber+{2}, InventoryAmount=InventoryAmount+{3} WHERE ProductGUID='{0}' AND LocationGUID='{1}' AND (SELECT COUNT(InventoryID) FROM crs_inventory WHERE ProductGUID='{0}' AND LocationGUID='{1}')=1 AND InventoryNumber>={2}", i.ProductGUID, locationGUID, number, amount);//更新条件：除本行外无重复备件、库位,现有库存数量>=库存减少数量
            }
            else
            {
                return string.Format("UPDATE crs_inventory SET InventoryNumber=InventoryNumber+{2}, InventoryAmount=InventoryAmount+{3} WHERE ProductGUID='{0}' AND LocationGUID='{1}' AND (SELECT COUNT(InventoryID) FROM crs_inventory WHERE ProductGUID='{0}' AND LocationGUID='{1}')=1; INSERT INTO crs_inventory (ProductGUID, LocationGUID,InventoryNumber,InventoryAmount) SELECT '{0}','{1}',{2},{3} WHERE NOT EXISTS (SELECT InventoryID FROM crs_inventory WHERE ProductGUID='{0}' AND LocationGUID='{1}') AND (Select Changes() = 0); ", i.ProductGUID, locationGUID, number, amount);//更新或插入库存数，更新条件：除本行外无重复备件、库位
            }
        }

        /// <summary>
        /// 收款
        /// </summary>
        public static bool Charge(ObjBill obj)
        {
            bool result = false;
            string str = string.Format("单据号：{0}\r\n车牌号：{1}\r\n车型：{2}\r\n收款金额：{3}\r\n", obj.BillCode, obj.LicensePlate,obj.ModelName, obj.TotalChargeAmount);
            if (System.Windows.MessageBox.Show(str, "确认收款内容", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                string sqlString = string.Format("UPDATE crs_bill SET IsCharge=1,ChargeGUID='{2}',ChargeTime=DateTime('Now', 'localtime'),StatusID={3} WHERE BillGUID='{0}' AND DeleteMark=0 AND StatusID={1}", obj.BillGUID,(int)EnumBillStatus.完成,DalLogin.LoginedUser.UserGUID,(int)EnumBillStatus.已收款);
                result = DalSQLite.Charge(sqlString);
            }
            return result;
        }

        /// <summary>
        /// 作废
        /// </summary>
        public static bool DeleteMark(ObjBill obj)
        {
            bool result = false;
            string str = string.Format("单据号：{0}\r\n车牌号：{1}\r\n车型：{2}\r\n维修项目：{3}\r\n", obj.BillCode, obj.LicensePlate, obj.ModelName, obj.ItemName);
            if (System.Windows.MessageBox.Show(str, "确认作废内容", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                string sqlString = string.Format("UPDATE crs_bill SET DeleteMark=1,UpGUID='{2}',Uptime=DateTime('Now', 'localtime'),StatusID={3} WHERE (BillGUID='{0}' AND DeleteMark=0 AND StatusID={1}) ", obj.BillGUID, (int)EnumBillStatus.保存, DalLogin.LoginedUser.UserGUID, (int)EnumBillStatus.作废);
                result = DalSQLite.DeleteMark(sqlString);
            }
            return result;
        }

        /// <summary>
        /// 返回数据对象
        /// </summary>
        /// <param name="g">ProductGUID</param>
        /// <returns>ObjProduct</returns>
        public static ObjBill GetObject(Guid g)
        {
            ObjBill obj = null;
            if (g != null)
            {
                string sqlString = string.Format("SELECT * FROM crs_bill WHERE BillGUID='{0}'", g);
                IList<ObjBill> list = DalSQLite.GetIList<ObjBill>(sqlString);
                if (BaseListClass.CheckNull(list) == false)
                {
                    if (list.Count == 1)
                    {
                        obj = list[0];
                        obj.ListDetail = GetDetailList(g);
                    }
                }
            }
            return obj;
        }


        /// <summary>
        /// 返回明细数据
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static IList<ObjProduct> GetDetailList(Guid g)
        {
            return DalSQLite.GetIList<ObjProduct>(string.Format("SELECT * FROM crs_bill_detail WHERE BillGUID LIKE '{0}'", g));
        }

        /// <summary>
        /// 返回明细数据计数
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static int GetDetailCount(Guid g)
        {
            return DalSQLite.GetDataRecordCount(string.Format("SELECT COUNT(*) FROM crs_bill_detail WHERE BillGUID LIKE '{0}'", g));
        }

        /// <summary>
        /// 返回保存状态的维修单的明细数据
        /// 用于计算未完成的维修单锁定数据库存数
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static IList<ObjProduct> GetRepairSaveList()
        {
            return DalSQLite.GetIList<ObjProduct>(string.Format("SELECT ProductGUID,SUM(SalesNumber)*1.00 AS SalesNumber FROM crs_bill_detail WHERE BillGUID IN (SELECT BillGUID FROM crs_bill WHERE DeleteMark=0 AND StatusID={0} AND TypeID={1}) GROUP BY ProductGUID", (int)EnumBillStatus.保存, (int)EnumBillType.维修单));
        }

        /// <summary>
        /// 返回原单GUID字段中有无此GUID
        /// </summary>
        /// <param name="g">ProductGUID</param>
        /// <returns>ObjProduct</returns>
        public static bool GetSource(Guid g)
        {
            bool result = true;
            if (g != new Guid())
            {
                int i=DalSQLite.GetDataRecordCount(string.Format("SELECT COUNT(*) FROM crs_bill WHERE SourceGUID='{0}'", g));
                if(i == 0)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 合计明细数据值到单据合计属性
        /// </summary>
        /// <param name="obj">ObjBill</param>
        public static void Sum(ObjBill obj)
        {
            if (obj != null && obj.ListDetail != null)
            {
                obj.TotalDebitNumber = obj.ListDetail.Sum(p => p.DebitNumber);
                obj.TotalDebitAmount = obj.ListDetail.Sum(p => p.DebitAmount);
                obj.TotalCreditNumber = obj.ListDetail.Sum(p => p.CreditNumber);
                obj.TotalCreditAmount = obj.ListDetail.Sum(p => p.CreditAmount);
                obj.TotalSalesNumber = obj.ListDetail.Sum(p => p.SalesNumber);
                obj.TotalSalesAmount = obj.ListDetail.Sum(p => p.SalesAmount);
                obj.TotalDiffAmount = obj.ListDetail.Sum(p => p.DiffAmount);
                obj.TotalChargeAmount = obj.ListDetail.Sum(p => p.ChargeAmount);
            }
        }

        /// <summary>
        /// 返回指定车辆、日期后单据数量
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static int GetCarDateCount(ObjCar car,DateTime d)
        {
            return DalSQLite.GetDataRecordCount(string.Format("SELECT COUNT(*) FROM crs_bill WHERE DeleteMark=0 AND CarGUID='{0}' AND BillDate>='{1}'", car.CarGUID,d.Date.ToString("s")));
        }
    }
}
