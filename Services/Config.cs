using Kanoe2.Data.Models;
using System.Xml.Serialization;

namespace Kanoe2.Services
{
    public class Config
    {
        private readonly string ConfigPath = $"{Directory.GetCurrentDirectory()}{@"\UserData\config\"}";

        private TwitchConfig TwitchConfig { get; set; }

        private List<Data.Models.Action> Actions { get; set; }

        public Config()
        {
            TwitchConfig = new TwitchConfig();
            Actions = new List<Data.Models.Action>();
            Load();
        }

        public List<Data.Models.Action> GetActions()
        {
            return Actions;
        }

        public void AddAction()
        {
            Data.Models.Action newAction = new Data.Models.Action();
            newAction.Name = "Action Name";
            Actions.Add(newAction);
            Save();
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
            Directory.CreateDirectory(ConfigPath);

            XmlSerializer TwitchSerializer = new(typeof(TwitchConfig));
            XmlSerializer ActionSerializer = new(typeof(List<Data.Models.Action>));

            using StreamWriter TwitchWriter = new(ConfigPath + "twitch.cfg");
            TwitchSerializer.Serialize(TwitchWriter, TwitchConfig);

            using StreamWriter ActionWriter = new(ConfigPath + "actions.cfg");
            ActionSerializer.Serialize(ActionWriter, Actions);

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

            if (File.Exists(ConfigPath + "actions.cfg"))
            {
                XmlSerializer serializer = new(typeof(List<Data.Models.Action>));
                using StreamReader reader = new(ConfigPath + "actions.cfg");
                Actions = (List<Data.Models.Action>)serializer.Deserialize(reader)!;
            }
            
            return this;
        }
    }
}
