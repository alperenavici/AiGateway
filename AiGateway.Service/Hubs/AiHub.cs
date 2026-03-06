using Microsoft.AspNetCore.SignalR;

namespace AiGateway.Service.Hubs;

public class AiHub:Hub
{
    
    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
    }
    
}