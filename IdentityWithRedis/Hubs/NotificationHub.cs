using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityWithRedis.Hubs
{
    public class NotificationHub :Hub
    {
        public async Task sendNotification(string NotificationSenderUserName, string  NotificationReceiverUserName)
        {
            await Clients.All.receiveNotification(NotificationSenderUserName , NotificationReceiverUserName);
        }
    }
}