using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace StudentsCommunity
{
    [HubName("CommentsHub")]
    public class CommentsHub : Hub
    {
        [HubMethodName("NotifyClients")]
        public static void NotifyCurrentUserInformationToAllClients()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CommentsHub>();

            // the update client method will update the connected client about any recent changes in the server data
            context.Clients.All.updatedClients();
        }
    }
}