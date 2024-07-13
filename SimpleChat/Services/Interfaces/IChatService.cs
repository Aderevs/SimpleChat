using SimpleChat.DTOs;

namespace SimpleChat.Services
{
    public interface IChatService
    {
        Task ConnectUserToChat(int userId, int chatId);
        Task<ChatDTO> CreateChat(ChatDTO chat);
        Task DeleteChatAndDisconnectUsersViaTransactionAsync(int chatId, int userId);
        Task DisconnectUserFromChat(int userId, int chatId);
        Task<IEnumerable<ChatDTO>> GetAllChats();
        Task<IEnumerable<ChatDTO>> GetAllChatsOfUserById(int userId);
        Task<IEnumerable<ChatDTO>> SearchForChats(string query);
    }
}