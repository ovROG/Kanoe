using Kanoe2.Data.Models;
using Kanoe2.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kanoe2.Services
{
    public class ActionsService
    {
        private readonly Config config;
        private readonly VTSService VTSService;
        private readonly IHubContext<Actions, IActionsClient> hubContext;

        public ActionsService(Config configService, VTSService vtsService, IHubContext<Actions, IActionsClient> hub)
        {
            config = configService;
            VTSService = vtsService;
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

        public async void RunEvent(Event e, Dictionary<string, string> varibles)
        {
            switch (e)
            {
                case TTS ts:
                    await hubContext.Clients.All.TTS(ts.FillTemplate(varibles), ts.Volume);
                    break;
                case Sound sound:
                    await hubContext.Clients.All.Sound(sound.File, sound.Volume);
                    break;
                case VTSHotkey vtshotkey:
                    await VTSService.SendHotkey(vtshotkey.Id);
                    break;
                case VTSExpression vtsexpression:
                    await VTSService.SendExpression(vtsexpression.File, vtsexpression.Active);
                    break;
                default:
                    break;
            }
        }

    }
}