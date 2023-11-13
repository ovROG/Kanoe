using Microsoft.AspNetCore.SignalR;

namespace Kanoe.Hubs
{
    public interface IActionsClient
    {
        Task TTS(string message, double volume);
        Task Sound(string file, double volume);
    }

    public class Actions : Hub<IActionsClient>
    {

    }
}