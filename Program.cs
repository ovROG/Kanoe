using Kanoe2.Hubs;
using Kanoe2.Services;
using Kanoe2.Services.Mockups;
using Kanoe2.Services.Twitch;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddSingleton<Config>();
builder.Services.AddSingleton<TwitchApiService>();
builder.Services.AddSingleton<TwitchChatService>();
builder.Services.AddSingleton<ChatMockupService>();
builder.Services.AddSingleton<UserFiles>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Init();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapHub<Chat>("/chathub");

app.MapGet("/api/userdata/{*path}", async (HttpContext contex, UserFiles UF, string path) => await UF.GetLocalFile(contex, @$"\UserData\{path}"));

app.MapFallbackToPage("/_Host");

app.Run();

static void Init()
{
    string dir = Directory.GetCurrentDirectory();
    try
    {
        if (Directory.Exists(dir + "/UserData/temp/"))
        {
            Directory.Delete(dir + "/UserData/temp/", true);
        }
    }
    catch { }

    if (File.Exists(dir + "/UserData/uploads/chat.css") && !File.Exists(dir + "/UserData/temp/chat.css"))
    {
        Directory.CreateDirectory(dir + "/UserData/temp");
        File.Copy(dir + "/UserData/uploads/chat.css", dir + "/UserData/temp/chat.css");
    }
}