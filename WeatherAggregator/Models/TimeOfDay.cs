using System;
using System.Collections.Generic;

namespace WeatherAggregator.Models
{
    public enum TimeOfDay
    {
        Night,
        Morning,
        Noon,
        Evening
    };

    public static class TimeOfDayHelper
    {
        public static IEnumerable<TimeOfDay> Times = new List<TimeOfDay>
        {
            TimeOfDay.Night,
            TimeOfDay.Morning,
            TimeOfDay.Noon,
            TimeOfDay.Evening
        };
        public static TimeOfDay FromDateTime(DateTime time)
        {
            TimeOfDay result;
            if (time.Hour < 6)
            {
                result = TimeOfDay.Night;
            }
            else if (time.Hour < 12)
            {
                result = TimeOfDay.Morning;
            }
            else if (time.Hour < 18)
            {
                result = TimeOfDay.Noon;
            }
            else
            {
                result = TimeOfDay.Evening;
            }
            return result;
        }
        public static string ToString(TimeOfDay tod)
        {
            switch (tod)
            {
                case TimeOfDay.Night:
                    return "Ночь";
                case TimeOfDay.Morning:
                    return "Утро";
                case TimeOfDay.Noon:
                    return "День";
                case TimeOfDay.Evening:
                    return "Вечер";
                default:
                    return "";
            }
        }
    }
}