using AutoMapper;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DTOs;

namespace SimpleChat.Services
{
    public class MessageService
    {
        private readonly IMapper _mapper;
        private readonly MessagesRepository _messagesRepository;
        private readonly ChatsRepository _chatsRepository;
        private readonly UsersRepository _usersRepository;

        public MessageService(
            IMapper mapper,
            MessagesRepository messagesRepository,
            ChatsRepository chatsRepository,
            UsersRepository usersRepository)
        {
            _mapper = mapper;
            _messagesRepository = messagesRepository;
            _chatsRepository = chatsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<MessageDTO> CreateMessage(MessageDTO messageDTO)
        {
            if (!await _chatsRepository.CheckIfChatWithSuchIdExistsAsync(messageDTO.Chat.ChatId))
            {
                throw new ArgumentException("No chat was found with an identifier matching this message field Chat.ChatId");
            }
            if (!await _usersRepository.CheckIfUserWithSuchIdExistsAsync(messageDTO.User.UserId))
            {
                throw new ArgumentException("No user was found with an identifier matching this message field User.UserId");
            }
            var messageDb = _mapper.Map<Message>(messageDTO);
            var createdMessage = await _messagesRepository.AddAsync(messageDb);
            return _mapper.Map<MessageDTO>(createdMessage);
        }
        public async Task<IEnumerable<MessageDTO>> GetAllMessagesOfChat(int chatId)
        {
            var chatDb = await _chatsRepository.GetByIdIncludeMessagesOrDefaultAsync(chatId);
            if (chatDb == null)
            {
                throw new ArgumentException("Chat with this ID was not found");
            }
            return _mapper.Map<List<MessageDTO>>(chatDb.AllMessages);
        }
        public async Task<MessageDTO> ChangeMessageText(int messageId, string newText, int userId)
        {
            var messageDb = await _messagesRepository.GetByIdOrDefaultAsync(messageId);
            if (messageDb == null)
            {
                throw new ArgumentException("Message with this ID was not found");
            }
            var userDb = await _usersRepository.GetByIdIncludeChatsConnectedToOrDefaultAsync(userId);
            if (userDb == null)
            {
                throw new ArgumentException("User with this ID was not found");
            }
            if (messageDb.UserId != userId)
            {
                throw new UnauthorizedAccessException("Only author of the message can edit it");
            }
            messageDb.Text = newText;
            var updatedMessage = await _messagesRepository.UpdateAsync(messageDb);
            return _mapper.Map<MessageDTO>(updatedMessage);
        }
        public async Task DeleteMessage(int messageId, int userId)
        {
            var messageDb = await _messagesRepository.GetByIdOrDefaultAsync(messageId);
            if (messageDb == null)
            {
                throw new ArgumentException("Message with this ID was not found");
            }
            var userDb = await _usersRepository.GetByIdIncludeChatsConnectedToOrDefaultAsync(userId);
            if (userDb == null)
            {
                throw new ArgumentException("User with this ID was not found");
            }
            if (!userDb.ChatsConnectedTo.Any(chat => chat.ChatId == messageDb.ChatId))
            {
                throw new UnauthorizedAccessException("Only member of the chat can delete messages from this chat");
            }
            await _messagesRepository.DeleteAsync(messageDb);
        }
    }
}
