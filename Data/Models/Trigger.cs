using System.Xml.Serialization;

namespace Kanoe.Data.Models
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
        public string Id { get; set; } = "";

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object? obj)
        {
            if (obj is TwitchPoints tp)
            {
                return tp.Id == Id;
            }
            return false;
        }

        public override int GetHashCode() { return Id.GetHashCode(); }
    }
}
