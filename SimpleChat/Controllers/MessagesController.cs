using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.DbLogic.Entities;
using SimpleChat.DTOs;
using SimpleChat.RequestModels;
using SimpleChat.Services;
using System;

namespace SimpleChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessagesController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> AllMessagesFromChat([FromBody] RequestChatId requestChatId)
        {
            if (requestChatId.ChatId <= 0)
            {
                return BadRequest($"{nameof(requestChatId.ChatId)} must be greater than 0");
            }
            var chatId = requestChatId.ChatId;
            var chats = await _messageService.GetAllMessagesOfChat(chatId);
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> WriteMessage([FromBody] MessageDTO message)
        {
            if (message.MessageId <= 0)
            {
                return BadRequest($"{nameof(message.MessageId)} field is required and must be greater than 0");
            }
            if (string.IsNullOrEmpty(message.Content))
            {
                return BadRequest($"{nameof(message.Content)} field is required");
            }
            if (message.UserId <= 0)
            {
                return BadRequest($"field {nameof(message.UserId)} is required");
            }
            if (message.ChatId <= 0)
            {
                return BadRequest($"field {nameof(message.ChatId)} is required");
            }
            var createdMessage = await _messageService.CreateMessage(message);
            return CreatedAtAction(nameof(WriteMessage), createdMessage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(int id, [FromBody] RequestMessageEditModel request)
        {
            if (id <= 0)
            {
                return BadRequest($"{nameof(id)} must be greater than 0");
            }
            if (request.UserId <= 0)
            {
                return BadRequest($"{nameof(request.UserId)} must be greater than 0");
            }
            if (string.IsNullOrEmpty(request.NewText))
            {
                return BadRequest($"{nameof(request.NewText)} field is required");
            }
            var messageId = id;
            var newText = request.NewText;
            var userId = request.UserId;    
            var updatedMessage = await _messageService.ChangeMessageText(messageId, newText, userId);
            return Ok(updatedMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, [FromBody] RequestUserId requestUserId)
        {
            if (id <= 0)
            {
                return BadRequest($"{nameof(id)} must be greater than 0");
            }
            if (requestUserId.UserId <= 0)
            {
                return BadRequest($"{nameof(requestUserId.UserId)} must be greater than 0");
            }
            var messageId = id;
            var userId = requestUserId.UserId;
            await _messageService.DeleteMessage(messageId, userId);
            return Ok("Message successfully deleted");
        }
    }
}
