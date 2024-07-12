using System.ComponentModel.DataAnnotations;

namespace SimpleChat.DbLogic.Entities
{
    public class Chat
    {
        [Key]
        public int ChatId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        public int HostUserId { get; set; }
        public User? HostUser { get; set; }

        public List<Message>? AllMessages { get; set; }
        public List<User>? UsersInvited { get; set; }
    }
}
