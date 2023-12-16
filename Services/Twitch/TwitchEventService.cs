using Kanoe.Data.Models;
using Kanoe.Shared;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace Kanoe.Services.Twitch
{
    public class TwitchEventsService
    {
        private readonly TwitchPubSub client;
        private readonly Config config;
        private readonly ActionsService actionsService;

        private bool IsConnected = false;

        public TwitchEventsService(Config configService, ActionsService aService)
        {
            client = new TwitchPubSub();
            config = configService;
            actionsService = aService;

            client.OnChannelPointsRewardRedeemed += Client_OnChannelPointsRewardRedeemed;

            client.OnListenResponse += Client_OnListenResponse;
            client.OnPubSubServiceConnected += Client_OnPubSubServiceConnected;
            client.OnPubSubServiceError += Client_OnPubSubServiceError;
        }

        private void Client_OnListenResponse(object? sender, OnListenResponseArgs e)
        {
            Logger.Log(e.Topic + "|" + e.Successful);
            Logger.Log(e.Response.Error ?? "No Error");
        }

        private void Client_OnChannelPointsRewardRedeemed(object? sender, OnChannelPointsRewardRedeemedArgs e)
        {
            Dictionary<string, string> varibles = new()
            {
                { "{name}", e.RewardRedeemed.Redemption.User.DisplayName }, //TODO: use list from Event type somehow
                { "{text}", e.RewardRedeemed.Redemption.UserInput }
            };
            actionsService.FireTrigger(new TwitchPoints() { Id = e.RewardRedeemed.Redemption.Reward.Id }, varibles);
        }

        private void Client_OnPubSubServiceConnected(object? sender, EventArgs e)
        {
            client.SendTopics(config.GetTwitchToken());
        }

        private void Client_OnPubSubServiceError(object? sender, OnPubSubServiceErrorArgs e)
        {
            Logger.Error(e.Exception.Message);
        }

        public void Connect()
        {
            if (!IsConnected)
            {
                string? userId = config.GetTwitchUserId();
                if (userId != null)
                {
                    client.ListenToChannelPoints(userId);
                    client.Connect();
                    IsConnected = true;
                }
            }
        }
    }
}