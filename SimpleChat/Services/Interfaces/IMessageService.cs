using SimpleChat.DTOs;

namespace SimpleChat.Services
{
    public interface IMessageService
    {
        Task<MessageDTO> ChangeMessageText(int messageId, string newText, int userId);
        Task<MessageDTO> CreateMessage(MessageDTO messageDTO);
        Task DeleteMessage(int messageId, int userId);
        Task<IEnumerable<MessageDTO>> GetAllMessagesOfChat(int chatId);
    }
}