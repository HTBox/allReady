using System;

namespace AllReady.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AtNoon(this DateTime dateTime)
        {
            var oneDayBeforeAtNoon = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 12, 0, 0);
            return oneDayBeforeAtNoon;
        }
    }
}