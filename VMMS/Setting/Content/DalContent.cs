using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VMMS
{
    /// <summary>
    /// 维修内容数据操作层
    /// </summary>
    public class DalContent
    {
        /// <summary>
        /// 返回有效数据集合
        /// </summary>
        /// <returns>IList<ObjContent></returns>
        public static IList<ObjContent> GetViewList()
        {
            return DalSQLite.GetIList<ObjContent>("SELECT * FROM crs_content WHERE DeleteMark=0 ORDER BY ContentCode");
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        public static bool Insert(ObjContent obj)
        {
            return DalSQLite.Insert(GetInsertSqlString(obj));
        }

        //返回插入SQL语句
        private static string GetInsertSqlString(ObjContent obj)
        {
            if (obj.ContentGUID == new Guid())
                obj.ContentGUID = Guid.NewGuid();
            return string.Format("INSERT INTO crs_content (ContentGUID, ContentCode, ContentName,Remark,UpGUID,Uptime) SELECT '{0}','{1}','{2}','{3}','{4}',DateTime('Now', 'localtime') WHERE NOT EXISTS (SELECT ContentCode FROM crs_content WHERE ContentCode='{1}' AND DeleteMark=0)", obj.ContentGUID, obj.ContentCode, obj.ContentName, obj.Remark, DalLogin.LoginedUser.UserGUID);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static bool Update(ObjContent obj)
        {
            return DalSQLite.Update(string.Format("UPDATE crs_content SET  ContentCode='{1}',ContentName='{2}',Remark='{3}',UpGUID='{4}',Uptime=DateTime('Now', 'localtime') WHERE DeleteMark=0 AND ContentID={0} AND NOT EXISTS (SELECT ContentCode FROM crs_content WHERE ContentCode='{1}' AND ContentID<>{0})", obj.ContentID, obj.ContentCode, obj.ContentName, obj.Remark, DalLogin.LoginedUser.UserGUID));
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        /// <param name="ContentID"></param>
        public static bool DeleteMark(ObjContent obj)
        {
            string sqlString = string.Format("UPDATE crs_content SET DeleteMark=1,UpGUID='{1}',Uptime=DateTime('Now', 'localtime') WHERE ContentID={0} AND DeleteMark=0", obj.ContentID, DalLogin.LoginedUser.UserGUID);
            return DalSQLite.DeleteMark(sqlString);
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        public static bool Import(List<ObjContent> l)
        {
            List<string> listSqlStr = new List<string>();
            foreach (ObjContent i in l)
            {
                listSqlStr.Add(GetInsertSqlString(i));
            }
            return DalSQLite.Insert(listSqlStr);
        }
    }
}
