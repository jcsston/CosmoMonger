namespace CosmoMonger.Models.Utility
{
    using System;
    using System.Collections.Generic;

    public class FormatTimeSpan
    {
        private struct FormatSpec 
        {
            public string format;
            public int startSeconds;
            public int endSeconds;

            public FormatSpec(int startSeconds, string format, int endSeconds)
            {
                this.format = format;
                this.startSeconds =startSeconds;
                this.endSeconds = endSeconds;
            }
        };

        static private FormatSpec [] TimeFormats = new FormatSpec[]
        {
            new FormatSpec(60, "Just Now", -1),
            new FormatSpec(90, "1 Minute", -1), 
            new FormatSpec(3600, "Minutes", 60),
            new FormatSpec(5400, "1 Hour", -1),
            new FormatSpec(86400, "Hours", 3600),
            new FormatSpec(129600, "1 Day", -1),
            new FormatSpec(604800, "Days", 86400), 
            new FormatSpec(907200, "1 Week", -1),
            new FormatSpec(2628000, "Weeks", 604800),
            new FormatSpec(3942000, "1 Month", -1),
            new FormatSpec(31536000, "Months", 2628000), 
            new FormatSpec(47304000, "1 Year", -1)
        };

        static public string HumaneFormat(TimeSpan ts)
        {
            string token = " Ago";
            double seconds = ts.TotalSeconds;

            if (seconds < 0) {
                seconds = Math.Abs(seconds);
                token = String.Empty;
            }

            foreach (FormatSpec format in FormatTimeSpan.TimeFormats) {
                if (seconds < format.startSeconds) {
                    if (format.endSeconds == -1) {
                        return format.format + (format.startSeconds > 60 ? token : String.Empty);
                    } else {
                        return Math.Round(seconds / format.endSeconds) + " " + format.format + (format.startSeconds > 60 ? token : String.Empty);
                    }
                }
            }

            // overflow for years
            return Math.Round(seconds / 47304000) + " Years " + token;
        }
    }
}
