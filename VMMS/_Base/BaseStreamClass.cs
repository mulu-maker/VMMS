using System.IO;
using System.Text;

namespace VMMS
{
    /// <summary>
    /// 数据流操作类
    /// </summary>
    public static class BaseStreamClass
    {
        public static byte[] StringToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ByteArrayToString(byte[] byteArray)
        {
            string str = null;
            if (byteArray != null)
            {
                if (byteArray.Length > 0)
                {
                    StreamReader sr = new StreamReader(new MemoryStream(byteArray));
                    str = sr.ReadToEnd();
                }
            }
            return str;
        }
    }
}
