using System;

namespace AllReady.Providers
{
    public interface IConvertDateTimeOffset
    {
        DateTimeOffset ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
    }

    public class DateTimeOffsetConverter : IConvertDateTimeOffset
    {
        public DateTimeOffset ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, timeZoneInfo.GetUtcOffset(dateTimeOffset));
        }
    }
}