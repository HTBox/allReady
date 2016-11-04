using System;

namespace AllReady.Providers
{
    public interface IConvertDateTimeOffset
    {
        DateTimeOffset ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
        DateTimeOffset ConvertDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0);
    }

    public class DateTimeOffsetConverter : IConvertDateTimeOffset
    {
        public DateTimeOffset ConvertDateTimeOffsetTo(string timeZoneId, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            return ConvertDateTimeOffsetTo(FindSystemTimeZoneBy(timeZoneId), dateTimeOffset, hour, minute, second);
        }

        public DateTimeOffset ConvertDateTimeOffsetTo(TimeZoneInfo timeZoneInfo, DateTimeOffset dateTimeOffset, int hour = 0, int minute = 0, int second = 0)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, hour, minute, second, timeZoneInfo.GetUtcOffset(dateTimeOffset));

            //both of these implemenations lose the ability to specificy the hour, minute and second unless the given dateTimeOffset value being passed into this method
            //already has those values set
            //1.
            //return TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
            //2.
            //var timeSpan = timeZoneInfo.GetUtcOffset(dateTimeOffset);
            //return dateTimeOffset.ToOffset(timeSpan);
        }

        private static TimeZoneInfo FindSystemTimeZoneBy(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}