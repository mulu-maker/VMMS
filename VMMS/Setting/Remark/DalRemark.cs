using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 备注内容数据操作层
    /// </summary>
    public class DalRemark
    {
        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList<ObjRemark></returns>
        public static IList<ObjRemark> GetViewList()
        {
            return DalSQLite.GetIList<ObjRemark>("SELECT * FROM crs_remark WHERE DeleteMark=0 ORDER BY RemarkCode");
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjRemark obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        //返回插入SQL语句
        private static string GetInsertSqlString(ObjRemark obj)
        {
            if (obj.RemarkGUID == new Guid())
                obj.RemarkGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_remark (RemarkGUID, RemarkCode, RemarkName,Remark,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}','{4}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT RemarkCode FROM crs_remark WHERE RemarkCode='{1}' AND DeleteMark=0)", obj.RemarkGUID, obj.RemarkCode, obj.RemarkName, obj.Remark, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjRemark obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_remark SET  RemarkCode='{1}',RemarkName='{2}',Remark='{3}',UpGUID='{4}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND RemarkID={0} AND NOT EXISTS (SELECT RemarkCode FROM crs_remark WHERE RemarkCode='{1}' AND RemarkID<>{0})", obj.RemarkID, obj.RemarkCode, obj.RemarkName, obj.Remark, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="RemarkID"></param>
        public static bool DeleteMark(ObjRemark obj)
        {
            string sqlString = string.Format("UPDATE crs_remark SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE RemarkID={0} AND DeleteMark=0", obj.RemarkID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjRemark> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjRemark i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
