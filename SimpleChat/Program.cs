using SimpleChat.DbLogic;
using SimpleChat.DbLogic.Repositories;

namespace SimpleChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            builder.Services.AddScoped(provider =>
            {
                var connectionString = builder.Configuration["ConnectionString"];
                return new ChatDbContext(connectionString);
            });

            builder.Services.AddScoped<UsersRepository>();
            builder.Services.AddScoped<ChatsRepository>();
            builder.Services.AddScoped<MessagesRepository>();
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ModelProfile>();
            });
            var app = builder.Build();

            //app.MapHub<ChatHub>("/chathub");
            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.Run();
        }
    }
}
