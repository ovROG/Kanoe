using Kanoe2.Data.Models;
using System.Xml.Serialization;

namespace Kanoe2.Services
{
    public class Config
    {
        private readonly string ConfigPath = $"{Directory.GetCurrentDirectory()}{@"\UserData\config\"}";

        private TwitchConfig TwitchConfig { get; set; }

        public Config()
        {
            TwitchConfig = new TwitchConfig();

            Load();
        }

        public async Task<IEnumerable<AlertEvent>> GetAlerts()
        {
            return new List<AlertEvent>();
        }

        public TwitchConfig GetTwitchConfig()
        {
            return TwitchConfig;
        }

        public Config SetTwitchId(string id)
        {
            TwitchConfig.Id = id;
            return this;
        }

        public Config SetTwitchToken(string token)
        {
            TwitchConfig.Token = token;
            return this;
        }

        public Config SetTwitchLogin(string login)
        {
            TwitchConfig.Login = login;
            return this;
        }

        public Config Save()
        {
            XmlSerializer xmlSerializer = new(typeof(TwitchConfig));
            Directory.CreateDirectory(ConfigPath);
            using StreamWriter writer = new(ConfigPath + "twitch.cfg");
            xmlSerializer.Serialize(writer, TwitchConfig);
            return this;
        }

        public Config Load()
        {
            if (File.Exists(ConfigPath + "twitch.cfg"))
            {
                XmlSerializer serializer = new(typeof(TwitchConfig));
                using StreamReader reader = new(ConfigPath + "twitch.cfg");
                TwitchConfig = (TwitchConfig)serializer.Deserialize(reader)!;
            }
            return this;
        }
    }
}
