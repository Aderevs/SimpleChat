using SimpleChat.DbLogic;
using SimpleChat.DbLogic.Repositories;
using SimpleChat.Controllers;
using SimpleChat.Services;

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
                var connectionString = builder.Configuration["ConnectionString"];
                return new ChatDbContext(connectionString);
            });

            builder.Services.AddScoped<UsersRepository>();
            builder.Services.AddScoped<ChatsRepository>();
            builder.Services.AddScoped<MessagesRepository>();

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ChatService>();
            builder.Services.AddScoped<MessageService>();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ModelProfile>();
            });

                        builder.Services.AddEndpointsApiExplorer();

                        builder.Services.AddSwaggerGen();
            var app = builder.Build();

            //app.MapHub<ChatHub>("/chathub");
            app.UseRouting();
            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
            });*/
            /* app.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}");

                         app.MapChatDTOEndpoints();*/
            app.MapControllers();
            app.Run();
        }
    }
}
