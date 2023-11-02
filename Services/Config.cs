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

        //Actions

        public List<Data.Models.Action> GetActions()
        {
            return Actions.ConvertAll(i => (Data.Models.Action)i.Clone());
        }

        public List<Data.Models.Action> GetActionsByTrigger(Trigger trigger)
        {
            return Actions.Where(a => a.Triggers.Contains(trigger)).ToList().ConvertAll(i => (Data.Models.Action)i.Clone());
        }

        public Data.Models.Action? GetAction(Guid id)
        {
            return (Data.Models.Action?)Actions.Find(a => a.Id == id)?.Clone();
        }

        public Config SetAction(Data.Models.Action action)
        {
            int i = Actions.FindIndex(a => a.Id == action.Id);
            if (i != -1)
            {
                Actions[i] = action;
                Save();
            }
            return this;
        }

        public Config AddAction()
        {
            Data.Models.Action newAction = new()
            {
                Name = "Action Name"
            };
            Actions.Add(newAction);
            Save();
            return this;
        }

        public Config DeleteAction(Guid id)
        {
            Actions.RemoveAll(a => a.Id == id);
            Save();
            return this;
        }

        //Twitch

        public string? GetTwitchId()
        {
            return TwitchConfig.Id;
        }

        public Config SetTwitchId(string id)
        {
            TwitchConfig.Id = id;
            return this;
        }

        public string? GetTwitchToken()
        {
            return TwitchConfig.Token;
        }

        public Config SetTwitchToken(string token)
        {
            TwitchConfig.Token = token;
            return this;
        }

        public string? GetTwitchLogin()
        {
            return TwitchConfig.Login;
        }

        public Config SetTwitchLogin(string login)
        {
            TwitchConfig.Login = login;
            return this;
        }

        public string? GetTwitchUserId()
        {
            return TwitchConfig.UserId;
        }

        public Config SetTwitchUserId(string userId)
        {
            TwitchConfig.UserId = userId;
            return this;
        }

        //etc

        public Config Save()
        {
            Directory.CreateDirectory(ConfigPath);

            XmlSerializer TwitchSerializer = new(typeof(TwitchConfig)); //TODO: May be move to static in classes
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
                try
                {
                    XmlSerializer serializer = new(typeof(TwitchConfig));
                    using StreamReader reader = new(ConfigPath + "twitch.cfg");
                    TwitchConfig = (TwitchConfig)serializer.Deserialize(reader)!;
                }
                catch
                {
                    TwitchConfig = new();
                    Console.WriteLine("UNABLE TO READ TWITCH CONFIG");
                }
            }

            if (File.Exists(ConfigPath + "actions.cfg"))
            {
                try
                {
                    XmlSerializer serializer = new(typeof(List<Data.Models.Action>));
                    using StreamReader reader = new(ConfigPath + "actions.cfg");
                    Actions = (List<Data.Models.Action>)serializer.Deserialize(reader)!;
                }
                catch
                {
                    Actions = new();
                    Console.WriteLine("UNABLE TO READ ACTIONS CONFIG");
                }
            }

            return this;
        }
    }
}
