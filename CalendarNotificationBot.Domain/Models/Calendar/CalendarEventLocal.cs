using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

namespace CalendarNotificationBot.Domain.Models.Calendar
{
    /// <summary>
    /// Calendar event data.
    /// </summary>
    public class CalendarEventLocal
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="occurrence">Upcoming event</param>
        public CalendarEventLocal(Occurrence occurrence)
        {
            if (!(occurrence.Source is CalendarEvent calendarEvent))
            {
                throw new ArgumentException("Source is not a CalendarEvent");
            }

            Summary = calendarEvent.Summary;
            Description = calendarEvent.Description;
            Status = calendarEvent.Status;
            Location = calendarEvent.Location;
            
            StartTime = occurrence.Period.StartTime.Value;
            EndTime = occurrence.Period.EndTime.Value;
            Duration = occurrence.Period.Duration;
        }
    
        /// <summary>
        /// Описание.
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// Описание.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Статус.
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Где проводится.
        /// </summary>
        public string Location { get; set; }
    
        /// <summary>
        /// Дата начала события.
        /// </summary>
        public DateTime StartTime { get; set; }
    
        /// <summary>
        /// Дата конца события.
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// Протяженность.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Было ли оповещение.
        /// </summary>
        public bool HasBeenSent { get; set; } = false;
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Summary);
            hashCode.Add(Description);
            hashCode.Add(Status);
            hashCode.Add(Location);
            hashCode.Add(StartTime);
            hashCode.Add(EndTime);
            hashCode.Add(Duration);
            return hashCode.ToHashCode();
        }
    }
}