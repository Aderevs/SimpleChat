namespace SimpleChat.DTOs
{
    public class MessageDTO
    {
        public int MessageId {  get; set; }
        public string? Content { get; set; }
        public UserDTO? User { get; set; }
        public ChatDTO? Chat { get; set; }
    }
}
