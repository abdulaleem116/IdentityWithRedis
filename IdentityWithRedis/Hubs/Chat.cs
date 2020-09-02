using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityWithRedis.Hubs
{
    public class Chat : Hub
    {
        public async Task sendMessage(string senderName, string receiverName, string msginput)
        {
            await Clients.All.receiveMessage(senderName, receiverName, msginput);
        }
    }
}