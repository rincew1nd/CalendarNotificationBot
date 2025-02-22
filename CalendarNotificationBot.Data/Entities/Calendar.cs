namespace CalendarNotificationBot.Data.Entities;

/// <summary>
/// User's calendar data.
/// </summary>
public class Calendar
{
    /// <summary>
    /// .ctor
    /// </summary>
    public Calendar(Guid userId, string calendarUrl, DateTime dateTime)
    {
        UserId = userId;
        CalendarUrl = calendarUrl;
        CreationDate = dateTime;
        ModificationDate = dateTime;
    }
    
    public Calendar() {}

    /// <summary>
    /// User's identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User's Bitrix identifier.
    /// </summary>
    /// <remarks>
    /// Will be filled if the data comes from Bitrix source.
    /// </remarks>
    public string? BitrixUserId { get; set; }
    
    /// <summary>
    /// Link to ICS calendar file.
    /// </summary>
    public string CalendarUrl  { get; set; } = null!;

    /// <summary>
    /// Record's creation date.
    /// </summary>
    public DateTime CreationDate { get; set; }
    
    /// <summary>
    /// Record's last modification date.
    /// </summary>
    public DateTime ModificationDate { get; set; }
}