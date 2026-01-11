using Financial.DTO;
using Microsoft.AspNetCore.SignalR;

namespace Financial.Hubs
{
    public class TransactionNotificationHub : Hub
    {
        public async Task SendTransactionStatusUpdate(TransactionCreateDto dbo)
        {
            await Clients.All.SendAsync("TransactionUpdated", dbo);
        }
    }
}
