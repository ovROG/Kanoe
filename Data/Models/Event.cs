using System.Xml.Serialization;

namespace Kanoe2.Data.Models
{
    public enum EventType
    {
        Sound,
        TTS,
        VTSHotkey,
    }

    [XmlInclude(typeof(Sound))]
    [XmlInclude(typeof(TTS))]
    [XmlInclude(typeof(VTSHotkey))]
    public abstract class Event : ICloneable
    {
        public abstract EventType Type { get; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class Sound : Event
    {
        public override EventType Type { get { return EventType.Sound; } }
        public string File { get; set; } = default!;
        public double Volume { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class TTS : Event
    {
        public override EventType Type { get { return EventType.TTS; } }
        public string Sound { get; set; } = default!;
        public double Volume { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class VTSHotkey : Event
    {
        public override EventType Type { get { return EventType.VTSHotkey; } }
        public int Id { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}
