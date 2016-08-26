using System;

namespace AllReady.Providers
{
    //public interface IDateTimeOffsetProvider
    public interface IConvertDateTimeOffsets
    {
        DateTimeOffset ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
        DateTimeOffset ConvertDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
        void ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset, out DateTimeOffset convertedStartDateTimeOffset, out DateTimeOffset convertedEndDateTimeOffset);
    }

    //public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    public class DateTimeOffsetConverter : IConvertDateTimeOffsets
    {
        public DateTimeOffset ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            return ConvertDateTimeOffsetTo(FindSystemTimeZoneBy(timeZoneId), dateTimeOffset, hour, minute, second);
        }

        public DateTimeOffset ConvertDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, timeZoneInfo.GetUtcOffset(dateTimeOffset));
        }

        public void ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset startDateTimeOffset, DateTimeOffset endDateTimeOffset, out DateTimeOffset convertedStartDateTimeOffset, out DateTimeOffset convertedEndDateTimeOffset)
        {
            var timeZoneInfo = FindSystemTimeZoneBy(timeZoneId);
            convertedStartDateTimeOffset = new DateTimeOffset(startDateTimeOffset.Year, startDateTimeOffset.Month, startDateTimeOffset.Day, 0, 0, 0, timeZoneInfo.GetUtcOffset(startDateTimeOffset));
            convertedEndDateTimeOffset = new DateTimeOffset(endDateTimeOffset.Year, endDateTimeOffset.Month, endDateTimeOffset.Day, 0, 0, 0, timeZoneInfo.GetUtcOffset(endDateTimeOffset));
        }

        private static TimeZoneInfo FindSystemTimeZoneBy(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}