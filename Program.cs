using Kanoe.Hubs;
using Kanoe.Services;
using Kanoe.Services.Mockups;
using Kanoe.Services.Twitch;
using MudBlazor;
using MudBlazor.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

string url = "http://localhost:" + (builder.Configuration["port"] ?? "5026");

UserFiles userFiles = new();
userFiles.ClearTempFolder();

builder.WebHost.UseUrls(url);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

builder.Services.AddSingleton<Config>();
builder.Services.AddSingleton<NativeOSMethodsService>();
builder.Services.AddSingleton(userFiles);
builder.Services.AddSingleton<LocalSpeechService>();
builder.Services.AddSingleton<FoobarService>();


builder.Services.AddSingleton<ActionsService>();

builder.Services.AddSingleton<TwitchApiService>();
builder.Services.AddSingleton<TwitchChatService>();
builder.Services.AddSingleton<TwitchEventsService>();

builder.Services.AddSingleton<VTSService>();

builder.Services.AddSingleton<ChatMockupService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapHub<Chat>("/chathub");
app.MapHub<Actions>("/actionshub");
app.MapHub<Notifications>("/notificationshub");

app.MapGet("/api/userdata/{*path}", async (HttpContext contex, UserFiles UF, string? path) => await UF.GetLocalFile(contex, @$"\UserData\{path}"));

app.MapFallbackToPage("/_Host");

Process.Start(new ProcessStartInfo // Open Browser
{
    FileName = url,
    UseShellExecute = true
});

app.Services.GetService<FoobarService>();
app.Services.GetService<VTSService>();

app.Run();