using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Automate.Utils
{

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var red = new BrushConverter().ConvertFrom("#c50500") as SolidColorBrush;

                return boolValue ? red : Brushes.Transparent; // Choisissez la couleur ici
            }
            return Brushes.Transparent; // Valeur par défaut
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
