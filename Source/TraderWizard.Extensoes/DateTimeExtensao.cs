using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraderWizard.Extensoes
{
    public static class DateTimeExtensao
    {
        public static bool IsDate(this object data)
        {
            DateTime dataConvertida;

            return DateTime.TryParse(data.ToString(), out dataConvertida);
        }

        public static DateTime FirstDayOfMonth(this DateTime data )
        {
            return new DateTime(data.Year, data.Month, 1);
        }
    }
}
