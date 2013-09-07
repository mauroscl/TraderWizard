using System;

namespace TraderWizard.Extensoes
{
    public static class DateTimeExtension
    {
        public static bool IsDate(this object data)
        {
            DateTime dataConvertida;

            return DateTime.TryParse(Convert.ToString(data), out dataConvertida);

        }

        public static DateTime FirstDayOfMonth(this DateTime data )
        {
            return new DateTime(data.Year, data.Month, 1);
        }
    }
}
