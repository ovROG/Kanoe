using System.Xml.Serialization;
using static Kanoe.Data.Models.AIMP;

namespace Kanoe.Data.Models
{
    public enum TriggerType
    {
        TwitchPoints,
        TwitchChatCommand,
        TwitchRaid,
    }

    [XmlInclude(typeof(TwitchPoints))]
    [XmlInclude(typeof(TwitchChatCommand))]
    [XmlInclude(typeof(TwitchRaid))]
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

    public class TwitchChatCommand : Trigger
    {
        public override TriggerType Type { get { return TriggerType.TwitchChatCommand; } }

        public string? Command { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object? obj)
        {
            if (obj is TwitchChatCommand tcc)
            {
                return tcc.Command == Command;
            }
            return false;
        }

        public override int GetHashCode() { return Command == null ? 0 : Command.GetHashCode(); }
    }

    public class TwitchRaid : Trigger
    {
        public override TriggerType Type { get { return TriggerType.TwitchRaid; } }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object? obj)
        {
            if (obj is TwitchRaid)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode() { return 0; }
    }
}
