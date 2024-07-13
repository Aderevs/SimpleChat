/*using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DbLogic;
using SimpleChat.DTOs;
using SimpleChat.Services;

public class ChatServiceTests
{
    private Mock<IMapper> _mockMapper;
    private Mock<IChatsRepository> _mockChatsRepository;
    private Mock<IUsersRepository> _mockUsersRepository;
    private Mock<IMessagesRepository> _mockMessagesRepository;
    private Mock<ChatDbContext> _mockDbContext;
    private ChatService _chatService;

    [SetUp]
    public void SetUp()
    {
        _mockMapper = new Mock<IMapper>();
        _mockChatsRepository = new Mock<IChatsRepository>();
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockMessagesRepository = new Mock<IMessagesRepository>();
        _mockDbContext = new Mock<ChatDbContext>();

        _chatService = new ChatService(
            _mockMapper.Object,
            _mockChatsRepository.Object,
            _mockUsersRepository.Object,
            _mockDbContext.Object);
    }

    [Test]
    public async Task GetAllChats_ReturnsMappedChats()
    {
        // Arrange
        var chats = new List<Chat> { new Chat { ChatId = 1, Name = "Test Chat" } };
        _mockChatsRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(chats);
        _mockMapper.Setup(m => m.Map<IEnumerable<Chat>>(It.IsAny<IEnumerable<Chat>>())).Returns(chats);

        // Act
        var result = await _chatService.GetAllChats();

        // Assert
        Assert.That(chats, Is.EqualTo(result));
    }

    [Test]
    public void GetAllChatsOfUserById_UserNotFound_ThrowsArgumentException()
    {
        // Arrange
        _mockUsersRepository.Setup(repo => repo.GetByIdIncludeChatsConnectedToOrDefaultAsync(It.IsAny<int>())).ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.GetAllChatsOfUserById(1));
        Assert.That(ex.Message, Is.EqualTo("User with this ID was not found"));
    }

    [Test]
    public async Task CreateChat_ValidChat_ReturnsChatDTO()
    {
        // Arrange
        var chatDto = new ChatDTO 
        {
            ChatId = 1, 
            HostUser = new UserDTO
            {
                UserId = 1,
                NickName = "Name",
            }
        };
        var chat = new Chat { ChatId = 1, HostUserId = 1, HostUser = new User { UserId = 1 } };
        _mockChatsRepository.Setup(repo => repo.CheckIfChatWithSuchIdExistsAsync(It.IsAny<int>())).ReturnsAsync(false);
        _mockUsersRepository.Setup(repo => repo.GetByIdOrDefaultAsync(It.IsAny<int>())).ReturnsAsync(chat.HostUser);
        _mockChatsRepository.Setup(repo => repo.AddAsync(It.IsAny<Chat>())).ReturnsAsync(chat);
        _mockMapper.Setup(m => m.Map<Chat>(It.IsAny<ChatDTO>())).Returns(chat);
        _mockMapper.Setup(m => m.Map<ChatDTO>(It.IsAny<Chat>())).Returns(chatDto);

        // Act
        var result = await _chatService.CreateChat(chatDto);

        // Assert
        Assert.That(chatDto, Is.EqualTo(result));
    }

    [Test]
    public void DeleteChatAndDisconnectUsersViaTransactionAsync_ChatNotFound_ThrowsArgumentException()
    {
        // Arrange
        _mockChatsRepository.Setup(repo => repo.GetByIdOrDefaultAsync(It.IsAny<int>())).ReturnsAsync((Chat)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.DeleteChatAndDisconnectUsersViaTransactionAsync(1, 1));
        Assert.That(ex.Message, Is.EqualTo("Chat with this ID was not found"));
    }

    [Test]
    public void ConnectUserToChat_ChatNotFound_ThrowsArgumentException()
    {
        // Arrange
        _mockChatsRepository.Setup(repo => repo.GetByIdIncludeUsersInvolvedOrDefaultAsync(It.IsAny<int>())).ReturnsAsync((Chat)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.ConnectUserToChat(1, 1));
        Assert.That(ex.Message, Is.EqualTo("Chat 7with this ID was not found"));
    }

    [Test]
    public void DisconnectUserFromChat_ChatNotFound_ThrowsArgumentException()
    {
        // Arrange
        _mockChatsRepository.Setup(repo => repo.GetByIdIncludeUsersInvolvedOrDefaultAsync(It.IsAny<int>())).ReturnsAsync((Chat)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _chatService.DisconnectUserFromChat(1, 1));
        Assert.That(ex.Message, Is.EqualTo("Chat with this ID was not found"));
    }
}

*/
using NUnit.Framework;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleChat;
using SimpleChat.DTOs;
using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.DbLogic;
using SimpleChat.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
