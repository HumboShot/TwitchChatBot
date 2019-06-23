using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchChatBot.Helpers
{
    public static class DateHelper
    {
        public static string TimeDiffFormatted(DateTime StartTime, DateTime EndTime)
        {
            var timeDiff = EndTime - StartTime;
            string time = string.Empty;

            if (timeDiff.Days > 0)
                time += $"{timeDiff.Days} Days ";

            if (timeDiff.Hours > 0)
                time += $"{timeDiff.Hours} Hours ";

            if (timeDiff.Minutes > 0)
                time += $"{timeDiff.Minutes} Minutes ";

            if (timeDiff.Seconds > 0)
                time += $"{timeDiff.Seconds} Seconds";

            return time;
        }
    }
}
