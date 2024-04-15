using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace VMMS
{
    /// <summary>
    /// DataTable操作类
    /// </summary>
    public class BaseDataTable
    {
        /// <summary>
        /// DataTable装为泛型集合
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns>泛型集合</returns>
        public static IList<T> DataTableToIList<T>(DataTable dt)
        {
            if (dt == null || dt.Rows.Count < 1)
                return null;
            IList<T> result = new List<T>();
            try
            {
                // 返回值初始化
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    T _t = (T)Activator.CreateInstance(typeof(T));
                    PropertyInfo[] propertys = _t.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            //属性与字段名称一致的进行赋值
                            if (pi.Name.Equals(dt.Columns[i].ColumnName))
                            {
                                if (dt.Columns[i].DataType == typeof(Int64))
                                {
                                    if (dt.Rows[j][i] != DBNull.Value)
                                        pi.SetValue(_t, int.Parse(dt.Rows[j][i].ToString()), null);
                                    else
                                        pi.SetValue(_t, null, null);
                                    break;
                                }
                                else if (dt.Columns[i].DataType == typeof(int))
                                {
                                    if (dt.Rows[j][i] != DBNull.Value)
                                        pi.SetValue(_t, int.Parse(dt.Rows[j][i].ToString()), null);
                                    else
                                        pi.SetValue(_t, null, null);
                                    break;
                                }
                                else if (dt.Columns[i].DataType == typeof(Boolean))
                                {
                                    if (dt.Rows[j][i] != DBNull.Value)
                                        pi.SetValue(_t, Convert.ToBoolean(dt.Rows[j][i].ToString()), null);
                                    else
                                        pi.SetValue(_t, null, null);
                                    break;
                                }
                                else if (dt.Columns[i].DataType == typeof(Guid))
                                {
                                    if (dt.Rows[j][i] != DBNull.Value)
                                        pi.SetValue(_t, new Guid(dt.Rows[j][i].ToString()), null);
                                    else
                                        pi.SetValue(_t, new Guid(), null);
                                    break;
                                }
                                else if (dt.Columns[i].DataType == typeof(double))//转换decimal类型值
                                {
                                    if (dt.Rows[j][i] != DBNull.Value)
                                        pi.SetValue(_t, Convert.ToDecimal(dt.Rows[j][i].ToString()), null);
                                    else
                                        pi.SetValue(_t, 0M, null);
                                    break;
                                }
                                else
                                {
                                    if (dt.Rows[j][i] != DBNull.Value)
                                        pi.SetValue(_t, dt.Rows[j][i], null);
                                    else
                                        pi.SetValue(_t, null, null);
                                    break;
                                }
                            }
                        }
                    }
                    result.Add(_t);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 判断DataTable是否空（无数据行）
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns>bool</returns>
        public static bool CheckNull(DataTable dt)
        {
            bool result = true;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    dt.AcceptChanges();
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 检查DataTable中指定列名是否存在
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="strArray">列名字符串数组）</param>
        /// <returns>bool</returns>
        public static bool CheckColumns(DataTable dt, string[] strArray)
        {
            bool result = false;
            if (CheckNull(dt) == false)
            {
                foreach (string str in strArray)
                {
                    if (CheckColumnName(str, dt) == false)
                    {
                        result = false;
                        break;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检查DataTable中指定列名是否存在
        /// </summary>
        /// <param name="columnName">列名字符串</param>
        /// <param name="dt">DataTable</param>
        /// <returns>bool</returns>
        private static bool CheckColumnName(string columnName, DataTable dt)
        {
            bool result = false;
            if (CheckNull(dt) == false)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (columnName == dt.Columns[i].ColumnName)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检测必填列是否有空白单元格？
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="requiredStringArray">必填列string[]</param>
        /// <param name="remark">提示字符串</param>
        /// <returns>bool</returns>
        public static bool CheckRequired(DataTable dt, string[] requiredStringArray, ref string remark)
        {
            bool result = true;
            if (BaseDataTable.CheckNull(dt) == false)
            {
                remark = string.Empty;
                for (int i = 0; i < requiredStringArray.Length; i++)
                {
                    string colName = requiredStringArray[i];
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        object cell = dt.Rows[j][colName];
                        if (cell == null)
                        {
                            result = false;
                            remark = remark + colName + "列第" + j + "行无内容；";
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
