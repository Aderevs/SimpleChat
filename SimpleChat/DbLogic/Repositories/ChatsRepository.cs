using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace SimpleChat.DbLogic.Repositories
{
    public class ChatsRepository
    {
        private readonly ChatDbContext _context;

        public ChatsRepository(ChatDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await _context.Chats.ToListAsync();
        }
        public async Task<Chat> GetByIdOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Chats.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<Chat> GetByIdIncludeUsersInvolvedOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Chats
                .Include(chat => chat.UsersInvited)
                .FirstOrDefaultAsync(chat => chat.ChatId == id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<Chat> GetByIdIncludeMessagesOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Chats
                .Include(chat => chat.AllMessages)
                .FirstOrDefaultAsync(chat => chat.ChatId == id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public void Add(Chat chat)
        {
            _context.Chats.Add(chat);
        }
        public void Update(Chat chat)
        {
            _context.Chats.Update(chat);
        }
        public void Delete(Chat chat)
        {
            _context.Chats.Remove(chat);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
