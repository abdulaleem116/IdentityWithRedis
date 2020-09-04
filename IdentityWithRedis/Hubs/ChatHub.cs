using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityWithRedis.Hubs
{
    public class User
    {
        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, User> Users
        = new ConcurrentDictionary<string, User>();

        public override Task OnConnected()
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new User
            {
                Name = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {

                user.ConnectionIds.Add(connectionId);

                // TODO: Broadcast the connected user
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            User user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {

                lock (user.ConnectionIds)
                {

                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!user.ConnectionIds.Any())
                    {

                        User removedUser;
                        Users.TryRemove(userName, out removedUser);

                        // You might want to only broadcast this info if this 
                        // is the last connection of the user and the user actual is 
                        // now disconnected from all connections.
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
        public List<User> /*ConcurrentDictionary<string, User>*/ GetAllActiveConnectionss()
        {
            List<User> connectedUsers = new List<User>();
            var connList = Users;
            var items = Users.Select(d => d.Value).ToList();
            foreach (KeyValuePair<string,User> k in connList)
            {
                if(k.Key == Context.User.Identity.Name)
                {
                    var myValue = k.Value;
                }
            }
            return items;
        }

    }
}