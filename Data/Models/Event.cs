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
        public abstract EventType Type { get; }
    }

    public class Sound : Event
    {
        public override EventType Type { get { return EventType.Sound; } }
        public string File { get; set; } = default!;
        public double Volume { get; set; }
    }

    public class TTS : Event
    {
        public override EventType Type { get { return EventType.TTS; } }
        public string Sound { get; set; } = default!;
        public double Volume { get; set; }
    }

    public class VTSHotkey : Event
    {
        public override EventType Type { get { return EventType.VTSHotkey; } }
        public int Id { get; set; }
    }
}
