using System;
using System.Text;
using System.Text.RegularExpressions;


namespace VMMS
{
    /// <summary>
    /// 字符串操作类
    /// </summary>
    public class BaseStringClass
    {
        /// <summary> 
        /// 获取字符串中的数字 
        /// </summary> 
        /// <param name="str">字符串 </param> 
        /// <returns>数字 </returns> 
        public static decimal StringToDecimal(string str)
        {
            decimal result = 0;
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.） 
                str = Regex.Replace(str, @"[^\d.\d]", "");

                // 如果是数字,则转换为decimal类型 
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = decimal.Parse(str);
                }
            }
            return result;
        }

        /// <summary>
        ///  数字转指定长度字符串
        /// </summary>
        /// <param name="num">数字</param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string NumberToLengthString(int num, int length)
        {
            if (num.ToString().Length < length)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < length - num.ToString().Length; i++)
                {
                    sb.Append("0");
                }
                return sb.Append(num).ToString();
            }
            else
            {
                return num.ToString();
            }
        }

        /// <summary>
        /// 返回数字+1字符串
        /// </summary>
        /// <param name="code"></param>
        /// <param name="stringLength"></param>
        /// <param name="numberLength"></param>
        /// <returns></returns>
        public static string GetNewCode(string code)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(code) == false)
            {
                int number = -1;
                if (int.TryParse(code, out number) == true)
                {
                    result = NumberToLengthString(number + 1, code.Length);
                }
            }
            return result;
        }

        public static string GetNewMaxCode(object obj, int length)
        {
            string result = string.Empty;
            if (obj != null)
            {
                string code = obj.ToString();
                if (string.IsNullOrEmpty(code.Trim()) == false)
                {
                    if (code.Length == length)//编号数据符合长度
                    {
                        result = GetNewCode(code);
                    }
                    else//长度不符
                    {
                        result = NumberToLengthString(1, length);
                    }
                }
                else
                {
                    result = NumberToLengthString(1, length);
                }
            }
            else
            {
                result = NumberToLengthString(1, length);
            }
            return result;
        }

        /// <summary>
        /// 返回日期新编号(4位年+2位月+2位日+数字字符串)
        /// </summary>
        /// <param name="code">现有编号</param>
        /// <param name="length">编号长度</param>
        /// <returns></returns>
        public static string GetNewDateCode(object obj, int length, DateTime dbDate)
        {
            string result = string.Empty;
            if (length > 8)
            {
                if (obj != null)
                {
                    string code = obj.ToString();
                    if (code.Length > 8)//已有编号长度必须大于指定长度,因为最小长度不能小于日期长度8
                    {
                        string dateString = code.Substring(0, 8);
                        if (dateString == dbDate.ToString("yyyyMMdd"))//有当日编号
                        {
                            int number = Convert.ToInt32(code.Substring(8));
                            result = dateString + NumberToLengthString(number + 1, length - 8);//返回+1的当前年最大编号
                        }
                        else//无当日编号
                        {
                            result = dateString + NumberToLengthString(1, length - 8);
                        }
                    }
                }
                else
                {
                    result = dbDate.ToString("yyyyMMdd") + NumberToLengthString(1, length - 8);
                }
            }
            return result;
        }

        /// <summary>
        /// 返回指定库位数字字符串
        /// </summary>
        /// <param name="num">数字</param>
        /// <param name="i">数字库位</param>
        /// <returns>数字字符</returns>
        public static string GetLengthString(string s, int i)
        {
            int l = s.Length;//给定的字符串的长度
            string result = "";
            if (i <= l)
            {
                result = s.Substring(i, 1);
            }
            return result;
        }

        /// <summary>
        /// 检测字符串是否符合SQL安全性
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool ProcessSqlStr(string inputString)
        {
            string SqlStr = @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            try
            {
                if ((inputString != null) && (inputString != String.Empty))
                {
                    string str_Regex = @"\b(" + SqlStr + @")\b";

                    Regex Regex = new Regex(str_Regex, RegexOptions.IgnoreCase);
                    //string s = Regex.Match(inputString).Value; 
                    if (true == Regex.IsMatch(inputString))
                        return false;

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 返回字符串拼音缩写
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetPinYinSuoXie(string text)
        {
            char pinyin;
            byte[] array;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length);

            foreach (char c in text)
            {
                pinyin = c;
                array = System.Text.Encoding.Default.GetBytes(new char[] { c });
                if (array.Length == 2)
                {
                    int i = array[0] * 0x100 + array[1];
                    if (i < 0xB0A1) pinyin = c;
                    else
                        if (i < 0xB0C5) pinyin = 'a';
                    else
                            if (i < 0xB2C1) pinyin = 'b';
                    else
                                if (i < 0xB4EE) pinyin = 'c';
                    else
                                    if (i < 0xB6EA) pinyin = 'd';
                    else
                                        if (i < 0xB7A2) pinyin = 'e';
                    else
                                            if (i < 0xB8C1) pinyin = 'f';
                    else
                                                if (i < 0xB9FE) pinyin = 'g';
                    else
                                                    if (i < 0xBBF7) pinyin = 'h';
                    else
                                                        if (i < 0xBFA6) pinyin = 'j';
                    else
                                                            if (i < 0xC0AC) pinyin = 'k';
                    else
                                                                if (i < 0xC2E8) pinyin = 'l';
                    else
                                                                    if (i < 0xC4C3) pinyin = 'm';
                    else
                                                                        if (i < 0xC5B6) pinyin = 'n';
                    else
                                                                            if (i < 0xC5BE) pinyin = 'o';
                    else
                                                                                if (i < 0xC6DA) pinyin = 'p';
                    else
                                                                                    if (i < 0xC8BB) pinyin = 'q';
                    else
                                                                                        if (i < 0xC8F6) pinyin = 'r';
                    else
                                                                                            if (i < 0xCBFA) pinyin = 's';
                    else
                                                                                                if (i < 0xCDDA) pinyin = 't';
                    else
                                                                                                    if (i < 0xCEF4) pinyin = 'w';
                    else
                                                                                                        if (i < 0xD1B9) pinyin = 'x';
                    else
                                                                                                            if (i < 0xD4D1) pinyin = 'y';
                    else
                                                                                                                if (i < 0xD7FA) pinyin = 'z';
                }
                sb.Append(pinyin);
            }
            return sb.ToString();
        }

        /// <summary> 
        /// 转换人民币大小金额 
        /// </summary> 
        /// <param name="num">金额</param> 
        /// <returns>返回大写形式</returns> 
        public static string ToRMB(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字 
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字 
            string str3 = "";    //从原num值中取出的值 
            string str4 = "";    //数字的字符串形式 
            string str5 = "";  //人民币大写金额形式 
            int i;    //循环变量 
            int j;    //num的值乘以100的字符串长度 
            string ch1 = "";    //数字的汉语读法 
            string ch2 = "";    //数字位的汉字读法 
            int nzero = 0;  //用来计算连续的零值是几个 
            int temp;            //从原num值中取出的值 

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数 
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式 
            j = str4.Length;      //找出最高位 
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分 

            //循环取出每一位需要转换的值 
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值 
                temp = Convert.ToInt32(str3);      //转换为数字 
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时 
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿,亿,万,元位等关键位 
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位,则必须写上 
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时,加上“整” 
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }
    }
}
