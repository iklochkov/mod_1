using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MyCRM.Infrastructure.Hubs;

namespace MyCRM.Infrastructure.Services
{
    public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    
    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendDealNotification(int dealId, string dealName, string status)
    {
        string message = $"Сделка c id: {dealId} и названием: {dealName} завершилась со статусом: {status}";
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    }
    }
}