using Microsoft.AspNetCore.SignalR;

namespace SimpleChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int chatId, int userId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", chatId, userId, message);
        }

        public async Task UpdateMessage(int messageId, string newText)
        {
            await Clients.All.SendAsync("UpdateMessage", messageId, newText);
        }

        public async Task DeleteMessage(int messageId)
        {
            await Clients.All.SendAsync("DeleteMessage", messageId);
        }
    }
}
