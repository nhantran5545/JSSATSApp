using JSSATSAPI.BussinessObjects.ResponseModels.ProductResponseModels;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSSATSAPI.BussinessObjects.InheritanceClass
{
    public class ProductHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task BroadcastProducts(IEnumerable<ProductResponse> products)
        {
            await Clients.All.SendAsync("ReceiveProducts", products);
        }

        public async Task BroadcastAvailableProducts(IEnumerable<ProductResponse> products)
        {
            await Clients.All.SendAsync("ReceiveAvailableProducts", products);
        }
    }
}
