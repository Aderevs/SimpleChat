using System.ComponentModel.DataAnnotations;

namespace SimpleChat.DbLogic.Entities
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string? Text { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
