using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yoonite.Common.Enumerations;

namespace Yoonite.Common.Helpers
{
    public static class DateTimeConvert
    {
        public static DateTime GetTimeZoneDateTime(TimeZoneInfoId infoId)
        {
            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(GetEnumDescription(infoId));
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static DateTimeOffset GetTimeZoneDateTimeOffset(TimeZoneInfoId infoId)
        {
            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(GetEnumDescription(infoId));
                return new DateTimeOffset(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone));
            }
            catch
            {
                return new DateTimeOffset(DateTime.Now);
            }
        }

        //public static DateTime ConvertDateTimeOffsetToDateTime(DateTimeOffset dateTimeOffset, TimeZoneInfoId infoId)
        //{
        //    try
        //    {
        //        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(GetEnumDescription(infoId));
        //        var offset = cstZone.GetUtcOffset(DateTime.Now);
        //        return dateTimeOffset).Add(offset);
        //    }
        //    catch
        //    {
        //        return DateTime.Now;
        //    }
        //}

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}