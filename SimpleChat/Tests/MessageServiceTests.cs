using NUnit.Framework;
using AutoMapper;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DTOs;
using SimpleChat.Services;
using SimpleChat.DbLogic;

namespace YourNamespace.Tests
{
    public class MessageServiceIntegrationTests
    {
        private ChatDbContext _context;
        private IMapper _mapper;
        private IMessagesRepository _messagesRepository;
        private IChatsRepository _chatsRepository;
        private IUsersRepository _usersRepository;
        private IMessageService _messageService;

        [SetUp]
        public async Task Setup()
        {
            string connectionString = "";//Enter here your connection string for testing db

            _context = new ChatDbContext(connectionString);
            await _context.Database.EnsureCreatedAsync();
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Message, MessageDTO>().ReverseMap();
                cfg.CreateMap<Chat, ChatDTO>().ReverseMap();
                cfg.CreateMap<User, UserDTO>().ReverseMap();
            }).CreateMapper();

            _messagesRepository = new MessagesRepository(_context);
            _chatsRepository = new ChatsRepository(_context);
            _usersRepository = new UsersRepository(_context);
            _messageService = new MessageService(_mapper, _messagesRepository, _chatsRepository, _usersRepository);

            await SeedDatabase();
        }

        private async Task SeedDatabase()
        {
            var chat = new Chat { ChatId = 1, Name = "test chat", HostUserId = 1 };
            var user = new User { UserId = 1, NickName = "test user", ChatsConnectedTo = new List<Chat> { chat } };
            var user2 = new User { UserId = 2, NickName = "test user2" };
            _context.Users.Add(user);
            _context.Users.Add(user2);
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateMessage_ChatAndUserExist_MessageCreated()
        {
            // Arrange
            var messageDTO = new MessageDTO { ChatId = 1, UserId = 1, Content = "Hello" };

            // Act
            var result = await _messageService.CreateMessage(messageDTO);

            // Assert
            Assert.That("Hello", Is.EqualTo(result.Content));
            Assert.That(1, Is.EqualTo(result.ChatId));
            Assert.That(1, Is.EqualTo(result.UserId));

        }

        [Test]
        public void CreateMessage_ChatDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var messageDTO = new MessageDTO { ChatId = 999, UserId = 1, Content = "Hello" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _messageService.CreateMessage(messageDTO));
            Assert.That(ex.Message, Is.EqualTo("No chat was found with an identifier matching this message field Chat.ChatId"));
        }

        [Test]
        public void CreateMessage_UserDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            var messageDTO = new MessageDTO { ChatId = 1, UserId = 999, Content = "Hello" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _messageService.CreateMessage(messageDTO));
            Assert.That(ex.Message, Is.EqualTo("No user was found with an identifier matching this message field User.UserId"));
        }

        [Test]
        public async Task GetAllMessagesOfChat_ChatExists_ReturnsMessages()
        {
            // Arrange
            var message = new Message { ChatId = 1, UserId = 1, Content = "Hello" };
            await _messagesRepository.AddAsync(message);

            // Act
            var result = await _messageService.GetAllMessagesOfChat(1);

            // Assert
            Assert.That(1, Is.EqualTo(result.Count()));
            Assert.That("Hello", Is.EqualTo(result.First().Content));
        }

        [Test]
        public void GetAllMessagesOfChat_ChatDoesNotExist_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _messageService.GetAllMessagesOfChat(999));
            Assert.That(ex.Message, Is.EqualTo("Chat with this ID was not found"));
        }

        [Test]
        public async Task ChangeMessageText_MessageExists_ChangesText()
        {
            // Arrange
            var message = new Message { ChatId = 1, UserId = 1, Content = "Old Content" };
            await _messagesRepository.AddAsync(message);

            // Act
            var result = await _messageService.ChangeMessageText(message.MessageId, "New Content", 1);

            // Assert
            Assert.That("New Content", Is.EqualTo(result.Content));
        }

        [Test]
        public void ChangeMessageText_MessageDoesNotExist_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _messageService.ChangeMessageText(999, "New Content", 1));
            Assert.That(ex.Message, Is.EqualTo("Message with this ID was not found"));
        }

        [Test]
        public async Task ChangeMessageText_UserNotAuthor_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var message = new Message { ChatId = 1, UserId = 2, Content = "Old Content" };
            await _messagesRepository.AddAsync(message);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _messageService.ChangeMessageText(message.MessageId, "New Content", 1));
            Assert.That(ex.Message, Is.EqualTo("Only author of the message can edit it"));
        }

        [Test]
        public async Task DeleteMessage_MessageExistsAndUserAuthorized_MessageDeleted()
        {
            // Arrange
            var message = new Message { ChatId = 1, UserId = 1, Content = "Hello" };
            await _messagesRepository.AddAsync(message);

            // Act
            await _messageService.DeleteMessage(message.MessageId, 1);

            
            // Assert
            var deletedMessage = await _context.Messages.FindAsync(message.MessageId);
            Assert.That(deletedMessage, Is.Null);
        }

        [Test]
        public void DeleteMessage_MessageDoesNotExist_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _messageService.DeleteMessage(999, 1));
            Assert.That(ex.Message, Is.EqualTo("Message with this ID was not found"));
        }

        [Test]
        public async Task DeleteMessage_UserNotAuthorized_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var message = new Message { ChatId = 1, UserId = 1, Content = "Hello" };
            await _messagesRepository.AddAsync(message);

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _messageService.DeleteMessage(message.MessageId, 2));
            
            Assert.That(ex.Message, Is.EqualTo("Only member of the chat can delete messages from this chat"));
        }
    }
}
