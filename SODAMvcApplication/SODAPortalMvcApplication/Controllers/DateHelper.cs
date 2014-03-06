using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SODAPortalMvcApplication.Controllers
{
    //obselete. not being used because of medium trust issues with godaddy
    public static class DateHelper
    {
        public static DateTime UTCtoLocal(this DateTime dateTime, string usersTimezoneId)
        {
            Instant instant = Instant.FromDateTimeUtc(dateTime);
            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            
            var usersTimezone = timeZoneProvider[usersTimezoneId];
            var usersZonedDateTime = instant.InZone(usersTimezone);
            return usersZonedDateTime.ToDateTimeUnspecified();
        }
        // The DateTime here should have a "Kind" of Unspecified
        public static DateTime LocaltoUTC(this DateTime dateTime, string usersTimezoneId)
        {
            LocalDateTime localDateTime = LocalDateTime.FromDateTime(dateTime);

            IDateTimeZoneProvider timeZoneProvider = DateTimeZoneProviders.Tzdb;
            //var usersTimezoneId = "Europe/London";
            var usersTimezone = timeZoneProvider[usersTimezoneId];

            var zonedDbDateTime = usersTimezone.AtLeniently(localDateTime);
            return zonedDbDateTime.ToDateTimeUtc();
        }

        public static TimeZoneInfo getTimeZoneInto(string usersTimezoneId)
        {

            return TimeZoneInfo.FindSystemTimeZoneById(LoadTimeZoneMap(TzdbDateTimeZoneSource.Default)[usersTimezoneId]);
            
        }
        private static IDictionary<string, string> LoadTimeZoneMap(IDateTimeZoneSource source)
        {
            var nodaToWindowsMap = new Dictionary<string, string>();
            foreach (var bclZone in TimeZoneInfo.GetSystemTimeZones())
            {
                var nodaId = source.MapTimeZoneId(bclZone);
                if (nodaId != null)
                {
                    nodaToWindowsMap[nodaId] = bclZone.Id;
                }
            }
            return nodaToWindowsMap;
        }
    }
}