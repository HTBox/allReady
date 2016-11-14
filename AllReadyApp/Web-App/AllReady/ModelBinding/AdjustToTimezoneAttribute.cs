using System;

namespace AllReady.ModelBinding
{
    /// <summary>
    /// Adjust a date time offset to the timezone specified by the TimeZoneId property name
    /// </summary>
    public class AdjustToTimezoneAttribute : Attribute
    {
        public string TimeZoneIdPropertyName { get; set; }
    }
}
