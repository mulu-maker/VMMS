using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VMMS
{
    /// <summary>
    /// 日期空值-空白值 转换器
    /// </summary>
    public class ConvDateTimeNull : IValueConverter
    {
        //当值从绑定源传播给绑定目标时,调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else if (value.ToString() == BaseDateTimeClass.BaseDate.ToString() || value.ToString() == new DateTime().ToString())
            {
                return null;
            }
            else
            {
                DateTime date = (DateTime)value;
                return date.ToString("yyyy-MM-dd HH:mm");
            }
        }

        //当值从绑定目标传播给绑定源时,调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            DateTime txtDate;
            if (DateTime.TryParse(str, out txtDate))
            {
                return txtDate;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
