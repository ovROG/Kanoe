using System.Text;
using System.Xml.Serialization;

namespace Kanoe.Data.Models
{
    public enum EventType
    {
        Sound,
        TTS,
        VTSHotkey,
        VTSExpression,
        AIMP,
        TwitchChatMessage,
    }

    [XmlInclude(typeof(Sound))]
    [XmlInclude(typeof(TTS))]
    [XmlInclude(typeof(VTSHotkey))]
    [XmlInclude(typeof(VTSExpression))]
    [XmlInclude(typeof(AIMP))]
    [XmlInclude(typeof(TwitchChatMessage))]
    public abstract class Event : ICloneable
    {
        public abstract EventType Type { get; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }

    public struct ObservationEvent
    {
        public Event Event { get; set; }
        public Dictionary<string, string> Varibles { get; set; }
    }

    public class Sound : Event //TODO: Add "Random Sound" event
    {
        public override EventType Type { get { return EventType.Sound; } }
        public string File { get; set; } = default!;
        public double Volume { get; set; } = 0.8;

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class TTS : Event
    {
        public override EventType Type { get { return EventType.TTS; } }

        public static readonly string[] VariblesList = { "{name}", "{text}" };
        public enum Source
        {
            Browser,
            Local,
        }
        public string Template { get; set; } = "{name} said: {text}";
        public double Volume { get; set; } = 0.8;
        public Source SourceType { get; set; } = Source.Browser;
        public string? Voice { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public string FillTemplate(Dictionary<string, string> varibles)
        {
            StringBuilder stringBuilder = new(Template);
            foreach (KeyValuePair<string, string> var in varibles)
            {
                stringBuilder.Replace(var.Key, var.Value);
            }
            return stringBuilder.ToString();
        }
    }

    public class VTSHotkey : Event
    {
        public override EventType Type { get { return EventType.VTSHotkey; } }
        public string Id { get; set; } = default!;

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class VTSExpression : Event
    {
        public override EventType Type { get { return EventType.VTSExpression; } }

        public enum State
        {
            True,
            False,
            Invert
        }
        public string File { get; set; } = default!;
        public State Active { get; set; } = State.True;

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class AIMP : Event
    {
        public override EventType Type { get { return EventType.AIMP; } }
        public enum Command
        {
            NEXT_TRACK,
            PREV_TRACK
        }

        public Command CMD { get; set; } = Command.NEXT_TRACK;

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class TwitchChatMessage : Event
    {
        public override EventType Type { get { return EventType.TwitchChatMessage; } }

        public string Template { get; set; } = "Hey {name}";
        public override object Clone()
        {
            return MemberwiseClone();
        }
        public string FillTemplate(Dictionary<string, string> varibles)
        {
            StringBuilder stringBuilder = new(Template);
            foreach (KeyValuePair<string, string> var in varibles)
            {
                stringBuilder.Replace(var.Key, var.Value);
            }
            return stringBuilder.ToString();
        }
    }
}
