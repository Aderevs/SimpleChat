using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic.Entities;

namespace SimpleChat.DbLogic.Repositories
{
    public class MessagesRepository
    {
        private readonly ChatDbContext _context;

        public MessagesRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetByIdOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Messages.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
        public async Task<Message> UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }
        public async Task DeleteAsync(Message message)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAllFromChatByIdAsync(int chatId)
        {
            var messagesToDelete = await _context.Messages
                .Where(message => message.ChatId == chatId)
                .ToListAsync();
            _context.Messages.RemoveRange(messagesToDelete);
            await _context.SaveChangesAsync();
        }

    }
}
