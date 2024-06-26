﻿using Kanoe.Data.Models;
using Kanoe.Shared;
using System.Xml.Serialization;

namespace Kanoe.Services
{
    public class Config
    {
        private readonly string ConfigPath = $"{Directory.GetCurrentDirectory()}{@"\UserData\config\"}";

        private TwitchConfig TwitchConfig { get; set; }

        private VTSConfig VTSConfig { get; set; }

        private YoutubeConfig YoutubeConfig { get; set; }

        private List<Data.Models.Action> Actions { get; set; }

        public Config()
        {
            TwitchConfig = new TwitchConfig();
            VTSConfig = new VTSConfig();
            YoutubeConfig = new YoutubeConfig();
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

        public char GetTwitchChatPrefix()
        {
            return TwitchConfig.CommandPrefix;
        }

        public Config SetTwitchChatPrefix(char prefix)
        {
            TwitchConfig.CommandPrefix = prefix;
            Save();
            return this;
        }

        //VTS

        public string? GetVTSToken()
        {
            return VTSConfig.Token;
        }

        public Config SetVTSToken(string? token)
        {
            VTSConfig.Token = token;
            return this;
        }

        //Youtube

        public string? GetYoutubeApiKey()
        {
            return YoutubeConfig.APIKey;
        }

        public Config SetYoutubeApiKey(string? key)
        {
            YoutubeConfig.APIKey = key;
            Save();
            return this;
        }

        //etc

        public Config Save() //TODO: Make it less repetitive. Or even maybe separate it.
        {
            Directory.CreateDirectory(ConfigPath);

            XmlSerializer TwitchSerializer = new(typeof(TwitchConfig));
            XmlSerializer VTSSerializer = new(typeof(VTSConfig));
            XmlSerializer YoutubeSerializer = new(typeof(YoutubeConfig));
            XmlSerializer ActionSerializer = new(typeof(List<Data.Models.Action>));

            using StreamWriter TwitchWriter = new(ConfigPath + "twitch.cfg");
            TwitchSerializer.Serialize(TwitchWriter, TwitchConfig);

            using StreamWriter VTSWriter = new(ConfigPath + "vts.cfg");
            VTSSerializer.Serialize(VTSWriter, VTSConfig);

            using StreamWriter YoutubeWriter = new(ConfigPath + "youtube.cfg");
            YoutubeSerializer.Serialize(YoutubeWriter, YoutubeConfig);

            using StreamWriter ActionWriter = new(ConfigPath + "actions.cfg");
            ActionSerializer.Serialize(ActionWriter, Actions);

            return this;
        }

        public Config Load() //TODO: Make it less repetitive. Or even maybe separate it.
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
                    Logger.Error("UNABLE TO READ TWITCH CONFIG");
                }
            }

            if (File.Exists(ConfigPath + "youtube.cfg"))
            {
                try
                {
                    XmlSerializer serializer = new(typeof(YoutubeConfig));
                    using StreamReader reader = new(ConfigPath + "youtube.cfg");
                    YoutubeConfig = (YoutubeConfig)serializer.Deserialize(reader)!;
                }
                catch
                {
                    YoutubeConfig = new();
                    Logger.Error("UNABLE TO READ VTS CONFIG");
                }
            }

            if (File.Exists(ConfigPath + "vts.cfg"))
            {
                try
                {
                    XmlSerializer serializer = new(typeof(VTSConfig));
                    using StreamReader reader = new(ConfigPath + "vts.cfg");
                    VTSConfig = (VTSConfig)serializer.Deserialize(reader)!;
                }
                catch
                {
                    VTSConfig = new();
                    Logger.Error("UNABLE TO READ VTS CONFIG");
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
                    Logger.Error("UNABLE TO READ ACTIONS CONFIG");
                }
            }

            return this;
        }
    }
}
