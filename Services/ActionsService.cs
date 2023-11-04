using Kanoe2.Data.Models;
using Kanoe2.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kanoe2.Services
{
    public class ActionsService
    {
        private readonly Config config;
        private readonly IHubContext<Actions, IActionsClient> hubContext;

        public ActionsService(Config configService, IHubContext<Actions, IActionsClient> hub)
        {
            config = configService;
            hubContext = hub;
        }

        public ActionsService FireTrigger(Trigger trigger, Dictionary<string, string> varibles)
        {
            List<Data.Models.Action> triggeredActions = config.GetActionsByTrigger(trigger); //TODO: Figure out how to implement rate limit
            foreach (Data.Models.Action action in triggeredActions)
            {
                foreach (Event e in action.Events)
                {
                    RunEvent(e, varibles);
                }
            }
            return this;
        }

        public void RunEvent(Event e, Dictionary<string, string> varibles)
        {
            switch (e)
            {
                case TTS ts:
                    hubContext.Clients.All.TTS(ts.FillTemplate(varibles), ts.Volume);
                    break;
                default:
                    break;
            }
        }

    }
}