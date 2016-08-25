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
            var timeZoneInfo = FindSystemTimeZoneBy(timeZoneId);
            return AdjustDateTimeOffsetTo(timeZoneInfo, dateTimeOffset, hour, minute, second);
        }

        public DateTimeOffset AdjustDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            var timeSpan = timeZoneInfo.GetUtcOffset(dateTimeOffset);
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, timeSpan);
        }

        //only thing that sucks about this is we lose the ability to pass in the hour, minute and second optionally... if we did, the sig of this method would start to look pretty ugly
        //OR, we put the onus on the caller to make sure the correct hour/minute/second are attached to the start/endDateTimeOffset values provided to the method
        public void AdjustDateTimeOffsetTo(string timeZoneId, DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset, out DateTimeOffset convertedStartDate, out DateTimeOffset convertedEndDate)
        {
            var timeZoneInfo = FindSystemTimeZoneBy(timeZoneId);
            var timeSpan = timeZoneInfo.GetUtcOffset(startDateTimeOffset);
            convertedStartDate = new DateTimeOffset(startDateTimeOffset.Year, startDateTimeOffset.Month, startDateTimeOffset.Day, 0, 0, 0, timeSpan);
            convertedEndDate = new DateTimeOffset(endDateTimeOffset.Year, endDateTimeOffset.Month, endDateTimeOffset.Day, 0, 0, 0, timeSpan);
        }

        private static TimeZoneInfo FindSystemTimeZoneBy(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}