using System.Text;
using System.Xml.Serialization;

namespace Kanoe2.Data.Models
{
    public enum EventType
    {
        Sound,
        TTS,
        VTSHotkey,
        VTSExpression
    }

    [XmlInclude(typeof(Sound))]
    [XmlInclude(typeof(TTS))]
    [XmlInclude(typeof(VTSHotkey))]
    [XmlInclude(typeof(VTSExpression))]
    public abstract class Event : ICloneable
    {
        public abstract EventType Type { get; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
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
        public string Template { get; set; } = "{name} said: {text}";
        public double Volume { get; set; } = 0.8;

        public static readonly string[] VariblesList = { "{name}", "{text}" };

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
}
