using Microsoft.AspNetCore.SignalR;

namespace Reports.API.Hubs
{
    public class ReportsHub : Hub
    {
        public Task JoinGlobal() => Groups.AddToGroupAsync(Context.ConnectionId, "reports");
    }
}
