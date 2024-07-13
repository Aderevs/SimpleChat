using SimpleChat.DbLogic.Entities;

namespace SimpleChat.DbLogic.Repositories
{
    public interface IUsersRepository
    {
        Task<User> AddAsync(User user);
        Task<bool> CheckIfUserWithSuchIdExistsAsync(int id);
        Task DeleteAsync(User user);
        Task DisconnectAllFromChatBeforeDeleteByChatIdAsync(int chatId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdIncludeChatsConnectedToOrDefaultAsync(int id);
        Task<User> GetByIdOrDefaultAsync(int id);
    }
}