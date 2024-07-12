using AutoMapper;
using SimpleChat.DbLogic;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DTOs;

namespace SimpleChat.Services
{
    public class ChatService
    {
        private readonly IMapper _mapper;
        private readonly ChatsRepository _chatsRepository;
        private readonly UsersRepository _usersRepository;
        private readonly MessagesRepository _messagesRepository;
        private readonly ChatDbContext _dbContext;

        public ChatService(
            IMapper mapper,
            ChatsRepository chatsRepository,
            UsersRepository usersRepository,
            MessagesRepository messagesRepository,
            ChatDbContext dbContext)
        {
            _mapper = mapper;
            _chatsRepository = chatsRepository;
            _usersRepository = usersRepository;
            _messagesRepository = messagesRepository;
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Chat>> GetAllChats()
        {
            var chatsDb = await _chatsRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Chat>>(chatsDb);
        }
        public async Task<IEnumerable<ChatDTO>> GetAllChatsOfUserById(int userId)
        {
            var userDb = await _usersRepository.GetByIdIncludeChatsConnectedToOrDefaultAsync(userId);
            if (userDb == null)
            {
                throw new ArgumentException("User with this ID was not found");
            }
            return _mapper.Map<List<ChatDTO>>(userDb.ChatsConnectedTo);
        }
        public async Task<ChatDTO> CreateChat(ChatDTO chat)
        {
            if(await _chatsRepository.CheckIfChatWithSuchIdExistsAsync(chat.ChatId))
            {
                throw new ArgumentException("Chat with such is already exists");
            }
            var chatDb = _mapper.Map<Chat>(chat);
            var userDb = await _usersRepository.GetByIdOrDefaultAsync(chatDb.HostUserId);
            if (userDb == null)
            {
                throw new ArgumentException($"Not found user with {nameof(chatDb.HostUserId)} from chat from argument");
            }
            chatDb.HostUser = userDb;
            chatDb.UsersInvited = [userDb];
            var createdChat = await _chatsRepository.AddAsync(chatDb);
            return _mapper.Map<ChatDTO>(createdChat);
        }
        public async Task DeleteChatAndDisconnectUsersViaTransactionAsync(int chatId, int userId)
        {
            var chatDb = await _chatsRepository.GetByIdOrDefaultAsync(chatId);
            if (chatDb == null)
            {
                throw new ArgumentException("Chat with this ID was not found");
            }
            if (chatDb.HostUserId != userId)
            {
                throw new UnauthorizedAccessException("Delete chat can only its host");
            }
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _usersRepository.DisconnectAllFromChatBeforeDeleteByChatIdAsync(chatId);
                await _chatsRepository.DeleteAsync(chatDb);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<ChatDTO>> SearchForChats(string query)
        {
            var chatsDb = await _chatsRepository.GetAllAsync();
            chatsDb = chatsDb.Where(chat => chat.Name.Contains(query));
            return _mapper.Map<List<ChatDTO>>(chatsDb);
        }
        public async Task ConnectUserToChat(int userId, int chatId)
        {
            var chatDb = await _chatsRepository.GetByIdIncludeUsersInvolvedOrDefaultAsync(chatId);
            if (chatDb == null)
            {
                throw new ArgumentException("Chat with this ID was not found");
            }
            var userDb = await _usersRepository.GetByIdOrDefaultAsync(chatId);
            if (userDb == null)
            {
                throw new ArgumentException("User with this ID was not found");
            }
            if (chatDb.UsersInvited.Any(user => user.UserId == userId))
            {
                throw new InvalidOperationException("Such user is already in this chat");
            }
            chatDb.UsersInvited.Add(userDb);
            await _chatsRepository.UpdateAsync(chatDb);
        }
        public async Task DisconnectUserFromChat(int userId, int chatId)
        {
            var chatDb = await _chatsRepository.GetByIdIncludeUsersInvolvedOrDefaultAsync(chatId);
            if (chatDb == null)
            {
                throw new ArgumentException("Chat with this ID was not found");
            }
            if (!await _usersRepository.CheckIfUserWithSuchIdExistsAsync(userId))
            {
                throw new ArgumentException("User with this ID was not found");
            }
            if (!chatDb.UsersInvited.Any(user => user.UserId == userId))
            {
                throw new InvalidOperationException("Such user isn't a member of this chat");
            }
            var userToDisconnect = chatDb.UsersInvited.First(user => user.UserId == userId);
            chatDb.UsersInvited.Remove(userToDisconnect);
        }
    }
}
