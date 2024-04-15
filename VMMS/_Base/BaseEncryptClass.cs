using System;
using System.IO;
using System.Security.Cryptography;//加密引用
using System.Text;

namespace VMMS
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public abstract class BaseEncryptClass
    {
        private const string DataKey = "VehicleMaintenanceManagementSystem";//唯一静态加密密文

        /// <summary>
        /// 返回加密的密码
        /// </summary>
        /// <param name="computerCode">密码</param>
        /// <returns>加密的密码</returns>
        public static string GetPwdEncrypt(string computerCode)
        {
            return BaseEncryptClass.MD5Encrypt(DataKey + computerCode);
        }

        /// <summary>
        /// MD5 加密静态方法
        /// </summary>
        /// <param name="EncryptString">待加密的密文</param>
        /// <returns>returns</returns>
        private static string MD5Encrypt(string encryptString)
        {
            if (string.IsNullOrWhiteSpace(encryptString)) { throw (new Exception("密文不得为空")); }
            MD5 m_ClassMD5 = new MD5CryptoServiceProvider();
            string m_strEncrypt = "";
            try
            {
                m_strEncrypt = BitConverter.ToString(m_ClassMD5.ComputeHash(Encoding.Default.GetBytes(encryptString))).Replace("-", "");
            }
            catch (ArgumentException ex) { throw ex; }
            catch (CryptographicException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            finally { m_ClassMD5.Clear(); }
            return m_strEncrypt;
        }

        /// <summary>
        /// RC2 加密(用变长密钥对大量数据进行加密)
        /// </summary>
        /// <param name="EncryptString">待加密密文</param>
        /// <param name="EncryptKey">加密密钥</param>
        /// <returns>returns</returns>
        private static string RC2Encrypt(string EncryptString, string EncryptKey)
        {
            if (string.IsNullOrWhiteSpace(EncryptString)) { throw (new Exception("密文不得为空")); }
            if (string.IsNullOrWhiteSpace(EncryptKey)) { throw (new Exception("密钥不得为空")); }
            if (EncryptKey.Length < 5 || EncryptKey.Length > 16) { throw (new Exception("密钥必须为5-16位")); }
            string m_strEncrypt = "";
            byte[] m_btIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            RC2CryptoServiceProvider m_RC2Provider = new RC2CryptoServiceProvider();
            try
            {
                byte[] m_btEncryptString = Encoding.Default.GetBytes(EncryptString);
                MemoryStream m_stream = new MemoryStream();
                CryptoStream m_cstream = new CryptoStream(m_stream, m_RC2Provider.CreateEncryptor(Encoding.Default.GetBytes(EncryptKey), m_btIV), CryptoStreamMode.Write);
                m_cstream.Write(m_btEncryptString, 0, m_btEncryptString.Length);
                m_cstream.FlushFinalBlock();
                m_strEncrypt = Convert.ToBase64String(m_stream.ToArray());
                m_stream.Close(); m_stream.Dispose();
                m_cstream.Close(); m_cstream.Dispose();
            }
            catch (IOException ex) { throw ex; }
            catch (CryptographicException ex) { throw ex; }
            catch (ArgumentException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            finally { m_RC2Provider.Clear(); }
            return m_strEncrypt;
        }
    }
}
