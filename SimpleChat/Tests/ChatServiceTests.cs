using NUnit.Framework;
using Moq;
using AutoMapper;
using SimpleChat.DTOs;
using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DbLogic;
using SimpleChat.Services;

namespace YourNamespace.Tests
{
    [TestFixture]
    public class ChatServiceTests
    {
        private Mock<IChatsRepository> _chatsRepositoryMock;
        private Mock<IUsersRepository> _usersRepositoryMock;
        private ChatDbContext _dbContext;
        private DbContextOptions<ChatDbContext> _dbContextOptions;
        private Mock<IMapper> _mapperMock;
        private ChatService _chatService;

        [SetUp]
        public void SetUp()
        {
            _chatsRepositoryMock = new Mock<IChatsRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _dbContextOptions = new DbContextOptionsBuilder<ChatDbContext>()
                            .UseInMemoryDatabase(databaseName: "ChatDatabase")
            .Options;

            _dbContext = new ChatDbContext(_dbContextOptions);
            _mapperMock = new Mock<IMapper>();

            _chatService = new ChatService(
                _mapperMock.Object,
                _chatsRepositoryMock.Object,
                _usersRepositoryMock.Object,
                _dbContext
            );
        }

        [Test]
        public async Task GetAllChats_ShouldReturnMappedChats()
        {
            // Arrange
            var chatsDb = new List<Chat> { new Chat { ChatId = 1, Name = "Test Chat" } };
            _chatsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(chatsDb);
            var chatsDto = new List<ChatDTO> { new ChatDTO { ChatId = 1, Name = "Test Chat" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<ChatDTO>>(It.IsAny<IEnumerable<Chat>>())).Returns(chatsDto);

            // Act
            var result = await _chatService.GetAllChats();

            // Assert
            Assert.That(chatsDto, Is.EqualTo(result));
        }

        [Test]
        public void GetAllChatsOfUserById_UserNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            _usersRepositoryMock.Setup(repo => repo.GetByIdIncludeChatsConnectedToOrDefaultAsync(It.IsAny<int>()))
                                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.GetAllChatsOfUserById(1));
            Assert.That("User with this ID was not found", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task CreateChat_ChatExists_ShouldThrowArgumentException()
        {
            // Arrange
            var chatDto = new ChatDTO { ChatId = 1, Name = "Test Chat" };
            _chatsRepositoryMock.Setup(repo => repo.CheckIfChatWithSuchIdExistsAsync(chatDto.ChatId))
                                .ReturnsAsync(true);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.CreateChat(chatDto));
            Assert.That("Chat with such is already exists", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task CreateChat_UserNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var chatDto = new ChatDTO
            {
                ChatId = 1,
                Name = "Test Chat",
                HostUser = new UserDTO
                {
                    UserId = 1,
                    NickName = "User"
                }
            };
            _chatsRepositoryMock.Setup(repo => repo.CheckIfChatWithSuchIdExistsAsync(chatDto.ChatId))
                                .ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Chat>(chatDto)).Returns(new Chat { ChatId = 1, Name = "Test Chat", HostUserId = 1 });
            _usersRepositoryMock.Setup(repo => repo.GetByIdOrDefaultAsync(It.IsAny<int>()))
                                .ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.CreateChat(chatDto));
            Assert.That($"Not found user with {nameof(Chat.HostUserId)} from chat from argument", Is.EqualTo(ex.Message));
        }

    }
}
