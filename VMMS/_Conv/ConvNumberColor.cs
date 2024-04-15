using System;
using System.Windows.Data;
using System.Windows.Media;

namespace VMMS
{
    ///// 定义转换器       
    [ValueConversion(typeof(decimal), typeof(SolidColorBrush))]
    public class ConvNumberColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal i = 0M;
            if (decimal.TryParse(value.ToString(), out i) == true)
            {
                if (i < 0)
                {
                    try
                    {
                        return new SolidColorBrush(System.Windows.Media.Colors.Red);
                    }
                    catch
                    {
                        throw;
                    }
                }
                else if(i > 0)
                {
                    try
                    {
                        return new SolidColorBrush(System.Windows.Media.Colors.Black);
                    }
                    catch
                    {
                        throw;
                    }
                }
                else 
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
