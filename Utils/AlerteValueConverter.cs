using Automate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using static Automate.Models.Jour;

namespace Automate.Utils
{
    public class AlerteValueConverter : IValueConverter
    {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is List<Tache> taches)
			{
				int arrosages = taches.Count(t => t == Jour.Tache.Arrosage);
				int semis = taches.Count(t => t == Jour.Tache.Semis);
				return $"Arrosages: {arrosages}\nSemis: {semis}";
			}
			return string.Empty;
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
