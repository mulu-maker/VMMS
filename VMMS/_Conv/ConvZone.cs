using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VMMS
{
    /// <summary>
    /// 0-null 转换器
    /// </summary>
    public class ConvZone : IValueConverter
    {
        //当值从绑定源传播给绑定目标时,调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else if (value.ToString() == "0" || value.ToString() == "0.00")
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        //当值从绑定目标传播给绑定源时,调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
