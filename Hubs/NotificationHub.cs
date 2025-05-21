using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AKhderApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, string title, string message)
        {
            // Send notification to a specific user
            await Clients.User(userId).SendAsync("ReceiveNotification", title, message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst("uid")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Optionally store the connection ID for the user
                await base.OnConnectedAsync();
            }
            else
            {
                throw new HubException("User not authenticated.");
            }
        }
    }
}
