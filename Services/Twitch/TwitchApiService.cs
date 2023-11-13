using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomReward;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace Kanoe.Services.Twitch
{
    public class TwitchApiService
    {
        private readonly TwitchAPI api;
        private readonly Config config;

        public TwitchApiService(Config configService, IConfiguration _config)
        {
            api = new TwitchAPI();
            config = configService;
            if (_config["twitchid"] != null)
            {
                configService.SetTwitchId(_config["twitchid"]!);
            }
            api.Settings.ClientId = configService.GetTwitchId() ?? _config["TwitchAppID"]!;
            api.Settings.AccessToken = configService.GetTwitchToken();
        }

        public TwitchApiService SetClientId(string clientId)
        {
            api.Settings.ClientId = clientId;
            return this;
        }

        public TwitchApiService SetToken(string token)
        {
            api.Settings.AccessToken = token;
            return this;
        }

        public async Task<string> GetTwitchUserLogin(string? token = null)
        {
            GetUsersResponse responce = await api.Helix.Users.GetUsersAsync(null, null, token);
            return responce.Users.First().Login;
        }

        public async Task<string> GetTwitchUserId(string login)
        {
            GetUsersResponse responce = await api.Helix.Users.GetUsersAsync(null, new List<string> { login });
            return responce.Users.First().Id;
        }

        public async Task<CustomReward[]> GetPointRewards()
        {
            if (config.GetTwitchLogin() != null)
            {
                string? userId = config.GetTwitchUserId();
                if (userId == null)
                {
                    string login = config.GetTwitchLogin()!;
                    userId = await GetTwitchUserId(login);
                    config.SetTwitchUserId(userId);
                }
                GetCustomRewardsResponse res = await api.Helix.ChannelPoints.GetCustomRewardAsync(userId);
                return res.Data;
            }
            return Array.Empty<CustomReward>();
        }
    }
}
