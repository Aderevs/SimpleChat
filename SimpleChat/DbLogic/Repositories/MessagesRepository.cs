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

        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
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
