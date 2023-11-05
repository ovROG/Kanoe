# Kanoe

Tool for some interaction betewn Twitch, VTube Studio, and other things

## Usage

Just run .exe

|Argument | Default | Description          |
|---------|---------|----------------------|
| --port  | 5000    |Launch with given port|


## Dev
Instal client libraries:

    libman restore
    dotnet restore

To add libman to VS Code use:

    dotnet tool install -g Microsoft.Web.LibraryManager.Cli

Run

    dotnet watch run --launch-profile http --non-interactive
