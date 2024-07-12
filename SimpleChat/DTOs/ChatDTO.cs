namespace SimpleChat.DTOs
{
    public class ChatDTO
    {
        public int ChatId { get; set; }
        public string? Name { get; set; }
        public UserDTO? HostUser { get; set; }
        public List<MessageDTO>? AllMessages { get; set; }
        public List<UserDTO>? UsersInvolved { get; set; }
    }
}
