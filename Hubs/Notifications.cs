using Microsoft.AspNetCore.SignalR;
using MudBlazor;

namespace Kanoe2.Hubs
{
    public interface INotificationsClient
    {
        Task Notify(string text, Severity severity = Severity.Normal);
    }

    public class Notifications : Hub<INotificationsClient>
    {
    }
}
