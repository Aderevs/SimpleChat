using System.ComponentModel.DataAnnotations;

namespace SimpleChat.DbLogic
{
    public class User
    {
        [Key]
        public int UserId {  get; set; }

        [Required]
        [StringLength(50)]
        public string? NickName { get; set; }

        public List<Chat>? ChatsCreated { get; set; }
        public List<Chat>? ChatsConnectedTo {  get; set; }
        public List<Message>? MessagesWrote { get; set; }
    }
}
