namespace VMMS
{
    /// <summary>
    /// 登录员工操作类
    /// </summary>
    public class DalLogin
    {
        public static ObjUser LoginedUser;//当前登录员工
        public const string PwdFileName = "pw.sys";//密码设置文件名

        public DalLogin()
        {
            LoginedUser = new ObjUser();
        }

        public static string GetPwd()
        {
            string result = string.Empty;
            if (System.IO.File.Exists(PwdFileName) == true)
            {
                string tmp = BaseFileClass.FileToString(PwdFileName);
                if (string.IsNullOrEmpty(tmp) == false)
                {
                    try
                    {
                        result = tmp;
                    }
                    catch
                    {
                        result = string.Empty;
                    }
                }
            }
            return result;
        }
    }
}
