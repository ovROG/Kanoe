namespace Kanoe2.Data.Models
{
    public enum EventType
    {
        Sound,
        TTS,
        VTSHotkey,
    }

    public abstract class Event
    {
        public abstract EventType EventType { get; }
    }

    public class Sound : Event
    {
        public override EventType EventType { get { return EventType.Sound; } }
        public required string File { get; set; }
        public required double Volume { get; set; }
    }

    public class TTS : Event
    {
        public override EventType EventType { get { return EventType.TTS; } }
        public required string Sound { get; set; }
        public required double Volume { get; set; }
    }

    public class VTSHotkey : Event
    {
        public override EventType EventType { get { return EventType.VTSHotkey; } }
        public required int Id { get; set; }
    }
}
