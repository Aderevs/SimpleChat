using SimpleChat.DbLogic;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.Controllers;
using SimpleChat.Services;
using SimpleChat.Hubs;

namespace SimpleChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddScoped(provider =>
            {
                var connectionString = builder.Configuration["ConnectionString"];//you have to write your connection string into the application.json
                return new ChatDbContext(connectionString);
            });

            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IChatsRepository, ChatsRepository>();
            builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IMessageService, MessageService>();

            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(pol =>
                {
                    pol.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ModelProfile>();
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            app.MapHub<ChatHub>("/chathub");
            app.UseRouting();
            app.UseCors();

            app.MapControllers();
            app.Run();
        }
    }
}
