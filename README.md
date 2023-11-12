# Kanoe

### Stream Helper Tool

- Chat with custom CSS
- Event system *(for interactions between)*:
    - Twitch
    - VTube Studio
    - Soundboard & TTS

> [!NOTE]
> This my first attempt at creating something big with C# and Blazor so any help is welcome :)

## Run

Just run .exe

| Argument         | Default | Description                      |
|------------------|---------|----------------------------------|
| `--port`         | `5026`  | *Launch with given port*                                        |
| `--twitchappid`  | `null`  | *Set custom Client ID for Twitch* (**NOT** saved in config)     |
| `--twitchid`     | `null`  | *Set custom Client ID for Twitch* (**WILL** be saved in config) |

*(or use appsettings.json)*

Then you can open `http://localhost:5026/` for configuration

### Chat

`http://localhost:5026/chat/{channel}?limit=*`

- `{channel}` - Twitch channel login
- `limit` - Limits amount of messages (Optional)

Example: `http://localhost:5026/chat/ovrog?limit=4`

### Soundboard & TTS

`http://localhost:5026/alerts`

## Dev
Instal client libraries:

    libman restore
    dotnet restore

Add libman (VS Code):

    dotnet tool install -g Microsoft.Web.LibraryManager.Cli

Run:

    dotnet watch run --launch-profile http --non-interactive