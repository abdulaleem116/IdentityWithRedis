using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityWithRedis.Hubs
{
    public class OnlineUsers : Hub
    {
        static HashSet<string> CurrentConnections = new HashSet<string>();

        public override Task OnConnected()
        {

            var id = Context.ConnectionId;
            CurrentConnections.Add(id);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connection = CurrentConnections.FirstOrDefault(x => x == Context.ConnectionId);

            if (connection != null)
            {
                CurrentConnections.Remove(connection);
            }

            return base.OnDisconnected(stopCalled);
        }


        //return list of all active connections
        public List<string> GetAllActiveConnections()
        {
            List<string> connList = CurrentConnections.ToList();
            return CurrentConnections.ToList();
        }
    }
}