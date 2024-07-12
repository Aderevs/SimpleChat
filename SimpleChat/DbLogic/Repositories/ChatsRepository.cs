using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic.Entities;
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
        public async Task<Chat>  AddAsync(Chat chat)
        {
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat;
        }
        public async Task<Chat> UpdateAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
            return chat;
        }
        public async Task DeleteAsync(Chat chat)
        {
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfChatWithSuchIdExistsAsync(int id)
        {
            return await _context.Chats.AnyAsync(chat => chat.ChatId == id);
        }
    }
}
