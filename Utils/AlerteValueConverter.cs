using Automate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using static Automate.Models.Jour;

namespace Automate.Utils
{
    public class AlerteValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int arrosages = 0;
            int semis = 0;
            if (value is List<Tache> taches)
            {
                foreach (Tache tache in taches)
                {
                    if (tache == Jour.Tache.Arrosage)
                        arrosages += 1;
                    else if (tache == Jour.Tache.Semis)
                        semis += 1;
                }
            }
            return $"Arrosages: {arrosages}\nSemis: {semis}";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
