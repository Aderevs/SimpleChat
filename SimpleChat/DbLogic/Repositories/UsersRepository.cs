using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic;
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
        public void Add(User user)
        {
            _context.Users.Add(user);
        }
        public async Task DisconnectAllFromChatBeforeDeleteByChatIdAsync(int chatId)
        {

            var users = await _context.Users
                .Include(user => user.ChatsConnectedTo)
                .Where(user => user.ChatsConnectedTo.Any(chat => chat.ChatId == chatId))
                .ToListAsync();
            if(users == null)
            {
                return;
            }
            var chatToDelete = users[0].ChatsConnectedTo
                .First(chat => chat.ChatId == chatId);
            foreach (var user in users)
            {
                user.ChatsConnectedTo.Remove(chatToDelete);
                _context.Users.Update(user);
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
