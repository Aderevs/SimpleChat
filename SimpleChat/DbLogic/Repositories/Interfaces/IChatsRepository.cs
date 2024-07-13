using SimpleChat.DbLogic.Entities;

namespace SimpleChat.DbLogic.Repositories
{
    public interface IChatsRepository
    {
        Task<Chat> AddAsync(Chat chat);
        Task<bool> CheckIfChatWithSuchIdExistsAsync(int id);
        Task DeleteAsync(Chat chat);
        Task<IEnumerable<Chat>> GetAllAsync();
        Task<IEnumerable<Chat>> GetAllBySubstringNameIncludeUsersAndMessagesAsync(string substring);
        Task<Chat> GetByIdIncludeMessagesOrDefaultAsync(int id);
        Task<Chat> GetByIdIncludeUsersInvolvedOrDefaultAsync(int id);
        Task<Chat> GetByIdOrDefaultAsync(int id);
        Task<Chat> UpdateAsync(Chat chat);
    }
}