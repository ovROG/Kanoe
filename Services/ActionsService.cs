using Kanoe.Data.Models;
using Kanoe.Hubs;
using Kanoe.Services.Twitch;
using Microsoft.AspNetCore.SignalR;

namespace Kanoe.Services
{
    public class ActionsService : IObservable<ObservationEvent>
    {
        private readonly Config config;
        private readonly IHubContext<Actions, IActionsClient> actionsHub;
        private readonly LocalSpeechService LocalSpeechService;
        private int ttsCounter; //Mabybe change in future

        List<IObserver<ObservationEvent>> observers = new();

        Dictionary<string, string> globalVaribles = new();

        public ActionsService(
            IHubContext<Actions, IActionsClient> aHub,
            Config configService,
            LocalSpeechService localSpeechService)
        {
            actionsHub = aHub;
            config = configService;
            LocalSpeechService = localSpeechService;
        }

        public ActionsService FireTrigger(Trigger trigger, Dictionary<string, string> varibles)
        {
            List<Data.Models.Action> triggeredActions = config.GetActionsByTrigger(trigger); //TODO: Figure out how to implement rate limit

            var fullVars = globalVaribles.Concat(varibles)
                .ToLookup(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, g => g.First()); ;

            foreach (Data.Models.Action action in triggeredActions)
            {
                foreach (Event e in action.Events)
                {
                    RunEvent(e, fullVars);
                }
            }
            return this;
        }

        public ActionsService SetVarible(string k, string v)
        {
            globalVaribles[k] = v;
            return this;
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ObservationEvent>> _observers;
            private IObserver<ObservationEvent> _observer;

            public Unsubscriber(List<IObserver<ObservationEvent>> observers, IObserver<ObservationEvent> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<ObservationEvent> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        public async void RunEvent(Event e, Dictionary<string, string> varibles)
        {
            switch (e)
            {
                case TTS ts:
                    switch (ts.SourceType)
                    {
                        case TTS.Source.Browser:
                            await actionsHub.Clients.All.TTS(ts.FillTemplate(varibles), ts.Volume);
                            break;
                        case TTS.Source.Local:
                            await LocalSpeechService.TTSToAudoFile(ts.FillTemplate(varibles), $@"\UserData\temp\tts{ttsCounter}.wav", ts.Voice);
                            await actionsHub.Clients.All.Sound($@"temp\tts{ttsCounter}.wav", ts.Volume);
                            ttsCounter++;
                            break;
                    }
                    break;
                case Sound sound:
                    await actionsHub.Clients.All.Sound(sound.File, sound.Volume);
                    break;
                default:
                    foreach (var observer in observers)
                        observer.OnNext(new ObservationEvent { Event = e, Varibles = varibles });
                    break;
            }
        }

    }
}