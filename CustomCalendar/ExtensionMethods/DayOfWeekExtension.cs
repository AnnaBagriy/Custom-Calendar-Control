using System;
using System.Collections;
using System.Linq;

namespace CustomCalendar.ExtensionMethods
{
    public static class DayOfWeekExtension
    {
        public static int ChangeFirstDayOfWeek(this DayOfWeek dayOfWeek, DayOfWeek firstDayOfWeek)
        {
            var shift = (int)dayOfWeek - (int)firstDayOfWeek;

            return shift >= 0 ? shift : (7 + shift);
        }
    }
}
