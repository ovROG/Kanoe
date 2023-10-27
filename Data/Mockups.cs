using Kanoe2.Data.Models;
using System.Drawing;

namespace Kanoe2.Data
{
    public static class Mockups
    {
        static readonly Random rnd = new();

        static readonly List<string> mockupNames = new() {
            "chvetochek_",
            "darkness_greys",
            "human_wolf",
            "LIMURYA",
            "bloodhound139",
            "LightningKun",
            "neilee",
        };

        static readonly List<(string, List<Emote>)> mockupMessagesBase = new() {
            ("фига, ну игрушка явно интересная", new List<Emote>() ),
            ("Оврог помидорка", new List<Emote>()),
            ("The quick brown fox jumps over the lazy dog", new List<Emote>()),
            ("друзья почему нет краба не надо забывать что мы имеем дело с дикими морепродуктами", new List<Emote>()),
            ("DinoDance DinoDance DinoDance", new List<Emote>(){new Emote ("emotesv2_dcd06b30a5c24f6eb871e8f5edbd44f7", "DinoDance") }),
            ("NotLikeThis", new List<Emote>(){new Emote ("58765", "NotLikeThis") }),
            ("PoroSad текст", new List<Emote>(){new Emote ("emotesv2_4c39207000564711868f3196cc0a8748", "PoroSad") }),
        };

        public static string GetRandomName()
        {
            return mockupNames[rnd.Next(mockupNames.Count)];
        }

        public static (string, List<Emote>) GetRandomMessageBase()
        {
            return mockupMessagesBase[rnd.Next(mockupMessagesBase.Count)];
        }

        public static ChatMessage GetRandomMessage()
        {
            Color color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            (string text, List<Emote> emotes) = GetRandomMessageBase();

            return new ChatMessage(
                Guid.NewGuid().ToString(),
                text,
                GetRandomName(),
                color,
                ColorTranslator.ToHtml(color),
                emotes,
                false
                );
        }
    }
}
