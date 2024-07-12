using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic.Entities;
namespace SimpleChat.DbLogic.Repositories
{
    public class UsersRepository
    {
        private readonly ChatDbContext _context;

        public UsersRepository(ChatDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetByIdOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Users.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<User> GetByIdIncludeChatsConnectedToOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Users
                .Include(user => user.ChatsConnectedTo)
                .FirstOrDefaultAsync(user => user.UserId == id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task DisconnectAllFromChatBeforeDeleteByChatIdAsync(int chatId)
        {

            var usersFromChat = await _context.Users
                .Include(user => user.ChatsConnectedTo)
                .Where(user => user.ChatsConnectedTo.Any(chat => chat.ChatId == chatId))
                .ToListAsync();
            if (usersFromChat == null || usersFromChat.Count <= 0)
            {
                return;
            }
            var chatToDelete = usersFromChat[0].ChatsConnectedTo
                .First(chat => chat.ChatId == chatId);
            foreach (var user in usersFromChat)
            {
                user.ChatsConnectedTo.Remove(chatToDelete);
                _context.Users.Update(user);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfUserWithSuchIdExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(user => user.UserId == id);
        }
    }
}
