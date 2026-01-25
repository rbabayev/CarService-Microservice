using Microsoft.AspNetCore.SignalR;

namespace CarServiceBG.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task SendBidUpdate(Guid productId, decimal newPrice, string bidderId, DateTime newEndTime)
        {
            await Clients.All.SendAsync("ReceiveBidUpdate", productId, newPrice, bidderId, newEndTime);
        }
        public async Task NotifyAuctionStart(Guid productId, string title)
        {
            await Clients.All.SendAsync("AuctionStarted", productId, title);
        }

        public async Task NotifyAuctionWinner(Guid productId, string winnerName, decimal price)
        {
            await Clients.All.SendAsync("AuctionWinner", productId, winnerName, price);
        }

        public async Task BroadcastAuctionEnded(string message)
        {
            await Clients.All.SendAsync("AuctionEnded", message);
        }

        public async Task BroadcastAuctionStartingSoon(string message)
        {
            await Clients.All.SendAsync("AuctionStartingSoon", message);
        }


    }
}
