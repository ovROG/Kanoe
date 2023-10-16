using Kanoe2.Data.Models;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace Kanoe2.Services.Twitch
{
    public class TwitchApiService
    {
        private readonly TwitchAPI api;
        private readonly TwitchConfig config;

        public TwitchApiService(Services.Config configService)
        {
            api = new TwitchAPI();
            config = configService.GetTwitchConfig();
            api.Settings.ClientId = config.Id;
            api.Settings.AccessToken = config.Token;
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
    }
}
