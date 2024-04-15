using System.Collections.Generic;

namespace VMMS
{
    /// <summary>
    /// 集合操作类
    /// </summary>
    public class BaseListClass
    {
        /// <summary>
        /// 检查集合是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        public static bool CheckNull<T>(IList<T> l)
        {
            bool result = true;
            if (l != null)
            {
                if (l.Count > 0)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}