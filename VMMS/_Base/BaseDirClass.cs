using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VMMS
{
    /// <summary>
    /// 文件夹操作类
    /// </summary>
    public abstract class BaseDirClass
    {
        public static readonly string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;//程序文件夹绝对路径
        public static readonly string WorkPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Work");//操作子文件夹绝对路径
        public static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//桌面绝对路径


        /// <summary>
        /// 得到当前文件夹中所有文件列表string[]
        /// </summary>
        /// <param name="DirFullPath">要遍历的文件夹全路径</param>
        /// <returns>string[] 文件列表</returns>
        public static string[] GetDirFiles(string DirFullPath)
        {
            string[] FileList = null;
            if (Directory.Exists(DirFullPath) == true)
            {
                FileList = Directory.GetFiles(DirFullPath, "*.*", System.IO.SearchOption.TopDirectoryOnly);
            }
            else//文件夹不存在
            {
                return FileList;
            }
            return FileList;
        }

        /// <summary>
        /// 得到当前文件夹及下级文件夹中所有文件列表string[]
        /// </summary>
        /// <param name="DirFullPath">要遍历的文件夹全路径</param>
        /// <param name="SO">查找文件的选项,是否包含子级文件夹</param>
        /// <returns>string[] 文件列表</returns>
        public static string[] GetDirFiles(string DirFullPath, System.IO.SearchOption SO)
        {
            string[] FileList = null;
            if (Directory.Exists(DirFullPath) == true)
            {
                FileList = Directory.GetFiles(DirFullPath, "*.*", SO);
            }
            else//文件夹不存在
            {
                return FileList;
            }
            return FileList;
        }

        /// <summary>
        /// 得到当前文件夹中指定文件类型［扩展名］文件列表string[]
        /// </summary>
        /// <param name="DirFullPath">要遍历的文件夹全路径</param>
        /// <param name="SearchPattern">查找文件的扩展名如“*.*代表所有文件；*.doc代表所有doc文件”</param>
        /// <returns>string[] 文件列表</returns>
        public static string[] GetDirFiles(string DirFullPath, string SearchPattern)
        {
            string[] FileList = null;
            if (Directory.Exists(DirFullPath) == true)
            {
                FileList = Directory.GetFiles(DirFullPath, SearchPattern);
            }
            else//文件夹不存在
            {
                return FileList;
            }
            return FileList;
        }
    }
}