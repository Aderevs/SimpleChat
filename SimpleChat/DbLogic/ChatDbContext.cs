using Microsoft.EntityFrameworkCore;
using SimpleChat.DbLogic.Entities;
using System.Diagnostics;

namespace SimpleChat.DbLogic
{
    public class ChatDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ChatDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(user => user.ChatsConnectedTo)
                .WithMany(chat => chat.UsersInvited)
                .UsingEntity(j => j.ToTable("UsersInChats"));

            modelBuilder.Entity<Chat>()
                .HasOne(chat => chat.HostUser)
                .WithMany(user => user.ChatsCreated)
                .HasForeignKey(chat => chat.HostUserId);

            modelBuilder.Entity<Chat>()
                .HasMany(chat => chat.AllMessages)
                .WithOne(message => message.Chat)
                .HasForeignKey(message => message.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(message => message.User)
                .WithMany(user => user.MessagesWrote)
                .HasForeignKey(message => message.UserId);


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(_connectionString)
                .LogTo(e => Debug.WriteLine(e));
        }
    }
}
