using System.Drawing;

namespace Kanoe2.Data.Models
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public string ColorHex { get; set; }
        public bool Hidden { get; set; }
        public List<Emote> EmoteSet { get; set; }

        public ChatMessage(string id, string message, string name, Color color, string colorHex, List<Emote> emoteSet, bool hidden)
        {
            Id = id;
            Message = message;
            Name = name;
            Color = color;
            ColorHex = colorHex;
            EmoteSet = emoteSet;
            Hidden = hidden;
        }
    }

    public class Emote
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Emote(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
