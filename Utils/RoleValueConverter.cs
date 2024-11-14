using System;
using System.Globalization;
using System.Windows.Data;
using Automate.Models;

namespace Automate.Utils
{
    public class RoleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Role role)
            {
                return role == Role.Admin;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
