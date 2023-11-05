using Microsoft.AspNetCore.SignalR;

namespace Kanoe2.Hubs
{
    public interface IActionsClient
    {
        Task TTS(string message, double volume);
        Task Sound(string message, double volume);
    }

    public class Actions : Hub<IActionsClient>
    {
        
    }
}