using Kanoe.Data.Models;
using Kanoe.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Kanoe.Services
{
    public class ActionsService
    {
        private readonly Config config;
        private readonly VTSService VTSService;
        private readonly IHubContext<Actions, IActionsClient> hubContext;
        private readonly LocalSpeechService LocalSpeechService;
        private int ttsCounter; //Mabybe change in future

        public ActionsService(Config configService, VTSService vtsService, IHubContext<Actions, IActionsClient> hub, LocalSpeechService localSpeechService)
        {
            config = configService;
            VTSService = vtsService;
            hubContext = hub;
            LocalSpeechService = localSpeechService;
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
                    switch (ts.SourceType)
                        {
                        case TTS.Source.Browser:
                            await hubContext.Clients.All.TTS(ts.FillTemplate(varibles), ts.Volume);
                            break;
                        case TTS.Source.Local:
                            await LocalSpeechService.TTSToAudoFile(ts.FillTemplate(varibles), $@"\UserData\temp\tts{ttsCounter}.wav", ts.Voice);
                            await hubContext.Clients.All.Sound($@"temp\tts{ttsCounter}.wav", ts.Volume);
                            ttsCounter++;
                            break;
                    }
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