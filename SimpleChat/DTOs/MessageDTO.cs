namespace SimpleChat.DTOs
{
    public class MessageDTO
    {
        public int MessageId {  get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
