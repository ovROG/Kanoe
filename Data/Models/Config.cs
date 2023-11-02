namespace Kanoe2.Data.Models
{
    public abstract class Config
    {
        public string? Token { get; set; }
    }

    public class TwitchConfig : Config
    {
        public string? Id { get; set; }
        public string? Login { get; set; }
        public string? UserId { get; set; }
    }
}
