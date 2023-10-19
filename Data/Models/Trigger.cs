namespace Kanoe2.Data.Models
{
    public enum TriggerType
    {
        TwitchPoints,
    }

    public abstract class Trigger
    {
        public abstract TriggerType Type { get; }
    }

    public class TwitchPoints : Trigger
    {
        public override TriggerType Type { get { return TriggerType.TwitchPoints; } }
        public string Id { get; set; } = default!;
    }
}
