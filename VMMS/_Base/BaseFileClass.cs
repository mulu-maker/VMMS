using System;
using System.IO;

namespace VMMS
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class BaseFileClass
    {
        /// <summary>
        /// 根据传来的文件全路径,外部打开文件,默认用系统注册类型关联软件打开
        /// </summary>
        /// <param name="FileFullPath">文件全路径</param>
        /// <returns>bool 文件名称</returns>
        public static bool OpenFile(string FileFullPath)
        {
            if (File.Exists(FileFullPath) == true)
            {
                System.Diagnostics.Process.Start(FileFullPath);
                return true;
            }
            else//文件不存在
            {
                return false;
            }
        }

        /// <summary>
        /// 文件转换成二进制,返回二进制数组byte[]
        /// </summary>
        /// <param name="FileFullPath">文件全路径</param>
        /// <returns>byte[] 包含文件流的二进制数组</returns>
        public static string FileToString(string FileFullPath)
        {
            string str = null;
            if (File.Exists(FileFullPath) == true)
            {
                FileStream Fs = new FileStream(FileFullPath, System.IO.FileMode.Open);
                byte[] fileData = new byte[Fs.Length];
                Fs.Read(fileData, 0, fileData.Length);
                Fs.Close();
                str = BaseStreamClass.ByteArrayToString(fileData);
            }
            else//文件不存在
            {

            }
            return str;
        }

        /// <summary>
        /// 字符串保存为文件（如已存在文件，删除后保存）
        /// </summary>
        /// <param name="CreateFileFullPath"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StringToFile(string CreateFileFullPath, string str)
        {
            return ByteArrayToFile(CreateFileFullPath, BaseStreamClass.StringToByteArray(str), true);
        }

        /// <summary>
        /// 二进制数组byte[]生成文件,并验证文件是否存在,在存则先删除
        /// </summary>
        /// <param name="CreateFileFullPath">要生成的文件全路径</param>
        /// <param name="StreamByte">要生成文件的二进制 Byte 数组</param>
        /// <param name="FileExistsDelete">同路径文件存在是否先删除</param>
        /// <returns>bool 是否生成成功</returns>
        private static bool ByteArrayToFile(string CreateFileFullPath, byte[] StreamByte, bool FileExistsDelete)
        {
            bool result = false;
            try
            {
                if (File.Exists(CreateFileFullPath) == true)
                {
                    if (FileExistsDelete == true && DeleteFile(CreateFileFullPath) == false)
                    {
                        return false;
                    }
                }
                FileStream FS;
                FS = File.Create(CreateFileFullPath);
                FS.Write(StreamByte, 0, StreamByte.Length);
                FS.Close();
                result = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FileFullPath">文件全路径</param>
        /// <returns>bool 是否删除成功</returns>
        public static bool DeleteFile(string FileFullPath)
        {
            if (File.Exists(FileFullPath) == true)
            {
                if (File.GetAttributes(FileFullPath) == FileAttributes.Normal)
                {
                    File.Delete(FileFullPath);
                }
                else
                {
                    File.SetAttributes(FileFullPath, FileAttributes.Normal);
                    File.Delete(FileFullPath);
                }
                return true;
            }
            else//文件不存在
            {
                return false;
            }
        }

        /// <summary>
        /// 根据传来的文件全路径,获取文件名称部分
        /// </summary>
        /// <param name="FileFullPath">文件全路径</param>
        /// <param name="IncludeExtension">是否包括文件扩展名</param>
        /// <returns>string 文件名称</returns>
        public static string GetFileName(string FileFullPath, bool IncludeExtension)
        {
            if (File.Exists(FileFullPath) == true)
            {
                FileInfo F = new FileInfo(FileFullPath);
                if (IncludeExtension == true)
                {
                    return F.Name;
                }
                else
                {
                    return F.Name.Replace(F.Extension, "");
                }
            }
            else//文件不存在
            {
                return null;
            }
        }
    }
}