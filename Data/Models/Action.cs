namespace Kanoe.Data.Models
{
    public class Action : ICloneable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Trigger> Triggers { get; set; }
        public List<Event> Events { get; set; }
        public TimeSpan RateLimit { get; set; }
        public bool AutoDisable { get; set; }


        public Action()
        {
            Id = Guid.NewGuid();
            Name = "";
            Triggers = new List<Trigger>();
            Events = new List<Event>();
            RateLimit = new TimeSpan(0);
            AutoDisable = false;
        }

        public object Clone()
        {
            Action clone = new()
            {
                Id = Id,
                Name = Name,
                Triggers = Triggers.ConvertAll(i => (Trigger)i.Clone()),
                Events = Events.ConvertAll(i => (Event)i.Clone()),
                RateLimit = RateLimit,
                AutoDisable = AutoDisable
            };
            return clone;
        }
    }
}
