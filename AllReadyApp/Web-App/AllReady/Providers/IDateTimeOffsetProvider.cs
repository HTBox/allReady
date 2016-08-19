using System;

namespace AllReady.Providers
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset GetDateTimeOffsetFor(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
        DateTimeOffset GetDateTimeOffsetFor(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
    }

    public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        public DateTimeOffset GetDateTimeOffsetFor(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            var timeZoneInfo = FindSystemTimeZoneBy(timeZoneId);
            return GetDateTimeOffsetFor(timeZoneInfo, dateTimeOffset, hour, minute, second);
        }

        public DateTimeOffset GetDateTimeOffsetFor(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            var timeSpan = timeZoneInfo.GetUtcOffset(dateTimeOffset);
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, timeSpan);
        }

        private static TimeZoneInfo FindSystemTimeZoneBy(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}