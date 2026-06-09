using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MyCRM.Infrastructure.Hubs
{
    [Authorize] 
    public class NotificationHub : Hub
    {
    }
}