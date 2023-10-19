namespace Kanoe2.Data.Models
{
    public class Action
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
    }
}
