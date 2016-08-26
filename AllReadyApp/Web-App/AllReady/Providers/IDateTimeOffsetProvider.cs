using System;

namespace AllReady.Providers
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset AdjustDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
        DateTimeOffset AdjustDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
        void AdjustDateTimeOffsetTo(string timeZoneId, DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset, out DateTimeOffset convertedStartDate, out DateTimeOffset convertedEndDate);
    }

    public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        public DateTimeOffset AdjustDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            return AdjustDateTimeOffsetTo(FindSystemTimeZoneBy(timeZoneId), dateTimeOffset, hour, minute, second);
        }

        public DateTimeOffset AdjustDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, timeZoneInfo.GetUtcOffset(dateTimeOffset));
        }

        public void AdjustDateTimeOffsetTo(string timeZoneId, DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset, out DateTimeOffset convertedStartDate, out DateTimeOffset convertedEndDate)
        {
            var timeZoneInfo = FindSystemTimeZoneBy(timeZoneId);
            convertedStartDate = new DateTimeOffset(startDateTimeOffset.Year, startDateTimeOffset.Month, startDateTimeOffset.Day, 0, 0, 0, timeZoneInfo.GetUtcOffset(startDateTimeOffset));
            convertedEndDate = new DateTimeOffset(endDateTimeOffset.Year, endDateTimeOffset.Month, endDateTimeOffset.Day, 0, 0, 0, timeZoneInfo.GetUtcOffset(endDateTimeOffset));
        }

        private static TimeZoneInfo FindSystemTimeZoneBy(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}