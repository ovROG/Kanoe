namespace Kanoe2.Data.Models
{
    public class AlertEvent
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required List<Trigger> Triggers { get; set; }
        public required List<Event> Events { get; set; }
    }
}
