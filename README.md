# Kanoe

### Stream Helper Tool

- Chat with custom CSS
- Event system (for interactions between):
    - Twitch
    - VTube Studio
    - Soundboard & TTS

## Run

Just run .exe

| Argument    | Default | Description                      |
|-------------|---------|----------------------------------|
| --port      | 5000    | Launch with given port           |
| --twitchid  | null    | Use custom Client ID from Twitch |

Then you can open `http://localhost:5000/` for configuration

### Chat

`http://localhost:5000/chat/{channel}?limit=*`

- `{channel}` - Twitch channel login
- `limit` - Limits amount of messages (Optional)

Example: `http://localhost:5000/chat/ovrog?limit=4`

### Soundboard & TTS

`http://localhost:5000/alerts`

## Dev
Instal client libraries:

    libman restore
    dotnet restore

Add libman (VS Code):

    dotnet tool install -g Microsoft.Web.LibraryManager.Cli

Run:

    dotnet watch run --launch-profile http --non-interactive

Secrets Example:
    
    {
        "TwitchAppID": *ClientID*
    }