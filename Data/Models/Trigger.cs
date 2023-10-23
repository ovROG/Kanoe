using System.Xml.Serialization;

namespace Kanoe2.Data.Models
{
    public enum TriggerType
    {
        TwitchPoints,
    }

    [XmlInclude(typeof(TwitchPoints))]
    public abstract class Trigger : ICloneable
    {
        public abstract TriggerType Type { get; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class TwitchPoints : Trigger
    {
        public override TriggerType Type { get { return TriggerType.TwitchPoints; } }
        public string Id { get; set; } = default!;

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}
