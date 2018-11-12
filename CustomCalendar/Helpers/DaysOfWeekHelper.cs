using System;
using System.Collections.Generic;
using System.Linq;
using CustomCalendar.Strings;

namespace CustomCalendar.Helpers
{
    public static class DaysOfWeekHelper
    {
        // TODO: Cover it with unit tests
        public static Dictionary<DayOfWeek, string> GetWeekNames(DayOfWeek startWeekDay)
        {
            // TODO: Translate days of week
            var dayDictionary = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Sunday, Days.Sunday },
                { DayOfWeek.Monday, Days.Monday },
                { DayOfWeek.Tuesday, Days.Tuesday },
                { DayOfWeek.Wednesday, Days.Wednesday },
                { DayOfWeek.Thursday, Days.Thursday },
                { DayOfWeek.Friday, Days.Friday },
                { DayOfWeek.Saturday, Days.Saturday }
            };

            if (startWeekDay == DayOfWeek.Sunday)
            {
                return dayDictionary;
            }

            var newDayDictionary = dayDictionary.Where(x => x.Key >= startWeekDay).ToDictionary(c => c.Key, c => c.Value);
            var missedDays = dayDictionary.Where(x => x.Key < startWeekDay);

            foreach (var day in missedDays)
            {
                newDayDictionary.Add(day.Key, day.Value);
            }

            return newDayDictionary;
        }
    }
}
