using Automate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Automate.Models.Jour;
using System.Windows.Data;
using System.Windows.Media;

namespace Automate.Utils
{
    public class LegendeTacheConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Tache tache)
            {

                var legende = Brushes.Transparent;
                switch (tache)
                {
                    case Jour.Tache.Semis:
                        legende = Brushes.DarkRed;
                        break;
                    case Jour.Tache.Rempotage:
                        legende = Brushes.Green;
                        break;
                    case Jour.Tache.Entretien:
                        legende = Brushes.Blue;
                        break;
                    case Jour.Tache.Arrosage:
                        legende = Brushes.Black;
                        break;
                    case Jour.Tache.Recolte:
                        legende = Brushes.Gold;
                        break;
                    case Jour.Tache.Commandes:
                        legende = Brushes.Indigo;
                        break;
                    case Jour.Tache.Evenements:
                        legende = Brushes.DarkOliveGreen;
                        break;
                    default:
                        legende = Brushes.Transparent;
                        break;
                }
                return legende;
            }
            return Brushes.Transparent;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
