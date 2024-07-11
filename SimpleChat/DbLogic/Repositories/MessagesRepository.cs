namespace SimpleChat.DbLogic.Repositories
{
    public class MessagesRepository
    {
        private readonly ChatDbContext _context;

        public MessagesRepository(ChatDbContext context)
        {
            _context = context;
        }

        public void Add(Message message)
        {
            _context.Messages.Add(message);
        }
        public void Update(Message message)
        {
            _context.Messages.Update(message);
        }
        public void Delete(Message message)
        {
            _context.Messages.Remove(message);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
