using AIFinanceAdvisor.Core.Services;

using OpenAI.Chat;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "AIFinanceAdvisor.Cookie";
        config.LoginPath = "/Account/Login";
    });


builder.Services.AddControllersWithViews();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddUserSecrets<Program>();

//builder.Services.AddSingleton<IChatClient, ChatClient>();



var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseWebSockets(); // Bật WebSocket


app.UseAuthentication();
app.UseAuthorization();


app.Run();
