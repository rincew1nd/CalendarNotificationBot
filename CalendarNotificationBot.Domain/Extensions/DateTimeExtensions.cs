using CalendarNotificationBot.Data.Entities;

namespace CalendarNotificationBot.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convert DateTime to user's DateTime.
        /// </summary>
        /// <remarks>
        /// Right now it's enough to just add user's timezone difference to UTC time.
        /// </remarks>
        /// <param name="dateTime">DateTime</param>
        /// <param name="user"><see cref="User"/></param>
        public static DateTime ToUserTimeZone(this DateTime dateTime, User user)
        {
            return dateTime.AddHours(user.TimeZone);
        }
    }
}