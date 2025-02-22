namespace CalendarNotificationBot.Infrastructure.DateTimeProvider
{
    /// <summary>
    /// Custom DateTime provider.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Current date in UTC.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Current machine time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Today date.
        /// </summary>
        DateTime Today { get; }
    }
}