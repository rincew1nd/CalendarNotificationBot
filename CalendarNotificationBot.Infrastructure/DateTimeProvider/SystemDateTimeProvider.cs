namespace CalendarNotificationBot.Infrastructure.DateTimeProvider
{
    /// <summary>
    /// System date time provider.
    /// </summary>
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc/>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <inheritdoc/>
        public DateTime Now => DateTime.Now;

        /// <inheritdoc/>
        public DateTime Today => DateTime.Today;
    }
}