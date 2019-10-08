using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace StudentsCommunity
{
    [HubName("MessagesHub")]
    public class MessagesHub : Hub
    {
        [HubMethodName("NotifyClient")]
        public static void NotifyCurrentUserInformationToAllClients()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MessagesHub>();

            // the update client method will update the connected client about any recent changes in the server data
            context.Clients.All.updatedClients();
        }
    }
}