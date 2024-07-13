using SimpleChat.DbLogic.Entities;

namespace SimpleChat.DbLogic.Repositories
{
    public interface IMessagesRepository
    {
        Task<Message> AddAsync(Message message);
        Task DeleteAllFromChatByIdAsync(int chatId);
        Task DeleteAsync(Message message);
        Task<Message> GetByIdOrDefaultAsync(int id);
        Task<Message> UpdateAsync(Message message);
    }
}