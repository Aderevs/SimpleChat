using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DTOs;
using SimpleChat.RequestModels;
using SimpleChat.Services;

namespace SimpleChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatsController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> AllChats()
        {
            var chats = await _chatService.GetAllChats();
            return Ok(chats);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> UserChats(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest($"{nameof(userId)} must be greater than 0");
            }
            var chats = await _chatService.GetAllChatsOfUserById(userId);
            return Ok(chats);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchChats([FromBody] RequestSearchQuery requestQuery)
        {
            if (requestQuery.Query == null)
            {
                return BadRequest($"{nameof(requestQuery.Query)} field is required to search chat");
            }
            var chats = await _chatService.SearchForChats(requestQuery.Query);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatDTO chat)
        {
            if (chat.ChatId <= 0)
            {
                return BadRequest($"{nameof(chat.ChatId)} field is required and must be greater than 0");
            }
            if (string.IsNullOrEmpty(chat.Name))
            {
                return BadRequest($"{nameof(chat.Name)} field is required");
            }
            if (chat.HostUser == null || chat.HostUser.UserId <= 0)
            {
                return BadRequest($"field {nameof(chat.HostUser.UserId)} of the field {nameof(chat.HostUser)} is required");
            }
            var createdChat = await _chatService.CreateChat(chat);
            return CreatedAtAction(nameof(CreateChat), createdChat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id, [FromBody] RequestUserId userIdModel)
        {
            int userId = userIdModel.UserId;
            if (id <= 0)
            {
                return BadRequest($"{nameof(id)} must be greater than 0");
            }
            if (userId <= 0)
            {
                return BadRequest($"{nameof(userId)} must be greater than 0");
            }
            await _chatService.DeleteChatAndDisconnectUsersViaTransactionAsync(id, userId);
            return Ok("Chat successfully deleted");
        }

        [HttpPost("{id}/connect")]
        public async Task<IActionResult> ConnectUser(int id, [FromBody] RequestUserId requestUserId)
        {
            if (requestUserId.UserId <= 0)
            {
                return BadRequest($"{nameof(requestUserId.UserId)} field must be greater than 0");
            }
            if (id <= 0)
            {
                return BadRequest($"{nameof(id)} must be greater than 0");
            }
            var userId = requestUserId.UserId;
            var chatId = id;
            await _chatService.ConnectUserToChat(userId, chatId);
            return Ok($"User id:{userId} successfully connected to chat id: {chatId}");
        }

        [HttpPost("{id}/disconnect")]
        public async Task<IActionResult> DisconnectUser(int id, [FromBody] RequestUserId requestUserId)
        {
            if (requestUserId.UserId <= 0)
            {
                return BadRequest($"{nameof(requestUserId.UserId)} field must be greater than 0");
            }
            if (id <= 0)
            {
                return BadRequest($"{nameof(id)} must be greater than 0");
            }
            var userId = requestUserId.UserId;
            var chatId = id;
            await _chatService.DisconnectUserFromChat(userId, chatId);
            return Ok($"User id:{userId} successfully disconnected form the chat id:{chatId}");
        }
    }
}
