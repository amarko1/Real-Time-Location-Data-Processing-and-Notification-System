using Microsoft.AspNetCore.SignalR;

namespace BackendTask.SignalR
{
    public class SearchHub : Hub
    {
        public async Task SendSearchNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveSearchNotification", message);
        }
    }
}
