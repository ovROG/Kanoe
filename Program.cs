using Kanoe2.Hubs;
using Kanoe2.Services;
using Kanoe2.Services.Mockups;
using Kanoe2.Services.Twitch;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:" + (builder.Configuration["port"] ?? "5000"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddSingleton<Config>();
builder.Services.AddSingleton<UserFiles>();

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

app.MapGet("/api/userdata/{*path}", async (HttpContext contex, UserFiles UF, string path) => await UF.GetLocalFile(contex, @$"\UserData\{path}"));

app.MapFallbackToPage("/_Host");

app.Run();